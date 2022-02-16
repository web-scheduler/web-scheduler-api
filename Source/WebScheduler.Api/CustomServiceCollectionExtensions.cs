namespace WebScheduler.Api;

using WebScheduler.Api.ConfigureOptions;
using WebScheduler.Api.Constants;
using WebScheduler.Api.Options;
using Boxed.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WebScheduler.Server.Options;
using Orleans.Configuration;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods which extend ASP.NET Core services.
/// </summary>
internal static class CustomServiceCollectionExtensions
{
    /// <summary>
    /// Configures the settings by binding the contents of the appsettings.json file to the specified Plain Old CLR
    /// Objects (POCO) and adding <see cref="IOptions{T}"/> objects to the services collection.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The services with options services added.</returns>
    public static IServiceCollection AddCustomOptions(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            // ConfigureAndValidateSingleton registers IOptions<T> and also T as a singleton to the services collection.
            .ConfigureAndValidateSingleton<ApplicationOptions>(configuration)
            .ConfigureAndValidateSingleton<CacheProfileOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.CacheProfiles)))
            .ConfigureAndValidateSingleton<CompressionOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.Compression)))
            .ConfigureAndValidateSingleton<ForwardedHeadersOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.ForwardedHeaders)))
            .Configure<ForwardedHeadersOptions>(
                options =>
                {
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                })
            .ConfigureAndValidateSingleton<HostOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.Host)))
            .ConfigureAndValidateSingleton<RedisOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.Redis)))
            .ConfigureAndValidateSingleton<KestrelServerOptions>(configuration.GetRequiredSection(nameof(ApplicationOptions.Kestrel)))
            .ConfigureAndValidateSingleton<ClusterOptions>(configuration.GetSection(nameof(ApplicationOptions.Cluster)))
            .ConfigureAndValidateSingleton<StorageOptions>(configuration.GetSection(nameof(ApplicationOptions.Storage)));

    public static IServiceCollection AddCustomConfigureOptions(this IServiceCollection services) =>
        services
            .ConfigureOptions<ConfigureApiVersioningOptions>()
            .ConfigureOptions<ConfigureMvcOptions>()
            .ConfigureOptions<ConfigureCorsOptions>()
            .ConfigureOptions<ConfigureJsonOptions>()
            .ConfigureOptions<ConfigureRedisCacheOptions>()
            .ConfigureOptions<ConfigureResponseCompressionOptions>()
            .ConfigureOptions<ConfigureRouteOptions>()
            .ConfigureOptions<ConfigureSwaggerGenOptions>()
            .ConfigureOptions<ConfigureSwaggerUIOptions>()
            .ConfigureOptions<ConfigureStaticFileOptions>();

    public static IServiceCollection AddCustomHealthChecks(
        this IServiceCollection services,
        IWebHostEnvironment webHostEnvironment,
        IConfiguration configuration) =>
        services
            .AddHealthChecks()
            // Add health checks for external dependencies here. See https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks
            .AddIf(
                !webHostEnvironment.IsEnvironment(EnvironmentName.Test),
                x => x.AddRedis(configuration.GetRequiredSection(nameof(ApplicationOptions.Redis)).Get<RedisOptions>().ConfigurationOptions.ToString()))
            .Services;

    /// <summary>
    /// Adds Open Telemetry services and configures instrumentation and exporters.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="webHostEnvironment">The environment the application is running under.</param>
    /// <returns>The services with open telemetry added.</returns>
    public static IServiceCollection AddCustomOpenTelemetryTracing(this IServiceCollection services, IWebHostEnvironment webHostEnvironment) =>
        services.AddOpenTelemetryTracing(
            builder =>
            {
                _ = builder
                    .SetResourceBuilder(ResourceBuilder
                        .CreateEmpty()
                        .AddService(
                            webHostEnvironment.ApplicationName,
                            serviceVersion: AssemblyInformation.Current.Version)
                        .AddAttributes(
                            new KeyValuePair<string, object>[]
                            {
                                new(OpenTelemetryAttributeName.Deployment.Environment, webHostEnvironment.EnvironmentName),
                                new(OpenTelemetryAttributeName.Host.Name, Environment.MachineName),
                            })
                        .AddEnvironmentVariableDetector())
                    .AddAspNetCoreInstrumentation(
                        options =>
                        {
                            // Enrich spans with additional request and response meta data.
                            // See https://github.com/open-telemetry/opentelemetry-specification/blob/master/specification/trace/semantic_conventions/http.md
                            options.Enrich = (activity, eventName, obj) =>
                            {
                                if (obj is HttpRequest request)
                                {
                                    var context = request.HttpContext;
                                    _ = activity.AddTag(OpenTelemetryAttributeName.Http.Flavor, GetHttpFlavour(request.Protocol));
                                    _ = activity.AddTag(OpenTelemetryAttributeName.Http.Scheme, request.Scheme);
                                    _ = activity.AddTag(OpenTelemetryAttributeName.Http.ClientIP, context.Connection.RemoteIpAddress);
                                    _ = activity.AddTag(OpenTelemetryAttributeName.Http.RequestContentLength, request.ContentLength);
                                    _ = activity.AddTag(OpenTelemetryAttributeName.Http.RequestContentType, request.ContentType);

                                    var user = context.User;
                                    if (user.Identity?.Name is not null)
                                    {
                                        _ = activity.AddTag(OpenTelemetryAttributeName.EndUser.Id, user.Identity.Name);
                                        _ = activity.AddTag(OpenTelemetryAttributeName.EndUser.Scope, string.Join(',', user.Claims.Select(x => x.Value)));
                                    }
                                }
                                else if (obj is HttpResponse response)
                                {
                                    _ = activity.AddTag(OpenTelemetryAttributeName.Http.ResponseContentLength, response.ContentLength);
                                    _ = activity.AddTag(OpenTelemetryAttributeName.Http.ResponseContentType, response.ContentType);
                                }

                                static string GetHttpFlavour(string protocol)
                                {
                                    if (HttpProtocol.IsHttp10(protocol))
                                    {
                                        return OpenTelemetryHttpFlavour.Http10;
                                    }
                                    else if (HttpProtocol.IsHttp11(protocol))
                                    {
                                        return OpenTelemetryHttpFlavour.Http11;
                                    }
                                    else if (HttpProtocol.IsHttp2(protocol))
                                    {
                                        return OpenTelemetryHttpFlavour.Http20;
                                    }
                                    else if (HttpProtocol.IsHttp3(protocol))
                                    {
                                        return OpenTelemetryHttpFlavour.Http30;
                                    }

                                    throw new InvalidOperationException($"Protocol {protocol} not recognised.");
                                }
                            };

                            options.RecordException = true;
                        });
                _ = builder.AddRedisInstrumentation();

                if (webHostEnvironment.IsDevelopment())
                {
                    _ = builder.AddConsoleExporter(
                        options => options.Targets = ConsoleExporterOutputTargets.Console | ConsoleExporterOutputTargets.Debug);
                }

                // TODO: Add OpenTelemetry.Instrumentation.* NuGet packages and configure them to collect more span data.
                //       E.g. Add the OpenTelemetry.Instrumentation.Http package to instrument calls to HttpClient.
                // TODO: Add OpenTelemetry.Exporter.* NuGet packages and configure them here to export open telemetry span data.
                //       E.g. Add the OpenTelemetry.Exporter.OpenTelemetryProtocol package to export span data to Jaeger.
            });
}
