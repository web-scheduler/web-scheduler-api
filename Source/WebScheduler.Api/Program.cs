namespace WebScheduler.Api;

using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Messaging;
using Orleans.Runtime.Membership;
using Serilog;
using WebScheduler.Api.Options;

#pragma warning disable RCS1102 // Make class static.
public class Program
#pragma warning restore RCS1102 // Make class static.
{
    public static async Task<int> Main(string[] args)
    {
        IHost? host = null;

        try
        {
            host = CreateHostBuilder(args).Build();

            host.LogApplicationStarted();
            await host.RunAsync().ConfigureAwait(false);
            host.LogApplicationStopped();

            return 0;
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
        {
            host!.LogApplicationTerminatedUnexpectedly(exception);

            return 1;
        }
    }

    private static IConfiguration? configuration;

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        new HostBuilder()

            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureHostConfiguration(
                configurationBuilder => configurationBuilder.AddCustomBootstrapConfiguration(args))
            .ConfigureAppConfiguration(
                (hostingContext, configurationBuilder) =>
                {
                    hostingContext.HostingEnvironment.ApplicationName = AssemblyInformation.Current.Product;
                    _ = configurationBuilder.AddCustomConfiguration(hostingContext.HostingEnvironment, args);
                })
            .UseSerilog(ConfigureReloadableLogger)
            .UseDefaultServiceProvider(
                (context, options) =>
                {
                    configuration = context.Configuration;
                    var isDevelopment = context.HostingEnvironment.IsDevelopment();
                    options.ValidateScopes = isDevelopment;
                    options.ValidateOnBuild = isDevelopment;
                })

            .UseOrleansClient(clientBuilder =>
                clientBuilder.Services.Configure<AdoNetClusteringClientOptions>(options =>
                {
                    options.Invariant = configuration?["Storage:Invariant"];
                    options.ConnectionString = configuration?["Storage:ConnectionString"];
                })
            .AddSingleton<IGatewayListProvider, AdoNetGatewayListProvider>()
            .AddSingleton<IConfigurationValidator, AdoNetClusteringClientOptionsValidator>()
               .Configure<ClusterOptions>(options =>
               {
                   options.ClusterId = configuration?["Cluster:ClusterId"];
                    options.ServiceId = configuration?["Cluster:ServiceId"];
               }))
            .ConfigureWebHost(ConfigureWebHostBuilder)
            .UseConsoleLifetime();

    private static void ConfigureWebHostBuilder(IWebHostBuilder webHostBuilder) =>
        webHostBuilder
            .UseKestrel(
                (builderContext, options) =>
                {
                    options.AddServerHeader = false;
                    options.Configure(
                        builderContext.Configuration.GetRequiredSection(nameof(ApplicationOptions.Kestrel)),
                        reloadOnChange: false);
                })
            .UseStartup<Startup>();

    /// <summary>
    /// Configures a logger used during the applications lifetime.
    /// <see href="https://nblumhardt.com/2020/10/bootstrap-logger/"/>.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    private static void ConfigureReloadableLogger(
        HostBuilderContext context,
        IServiceProvider services,
        LoggerConfiguration configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
            .WriteTo.Console();
}
