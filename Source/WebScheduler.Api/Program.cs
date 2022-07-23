namespace WebScheduler.Api;

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
            await host.RunAsync().ConfigureAwait(true);
            host.LogApplicationStopped();

            return 0;
        }
        catch (Exception exception)
        {
            host!.LogApplicationTerminatedUnexpectedly(exception);

            return 1;
        }
    }

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
                    var isDevelopment = context.HostingEnvironment.IsDevelopment();
                    options.ValidateScopes = isDevelopment;
                    options.ValidateOnBuild = isDevelopment;
                })

            .ConfigureWebHost(ConfigureWebHostBuilder)
            .UseConsoleLifetime();

    private static void ConfigureWebHostBuilder(IWebHostBuilder webHostBuilder) =>
        webHostBuilder
            .UseKestrel(
                (builderContext, options) =>
                {
                    options.AddServerHeader = false;
                    _ = options.Configure(
                        builderContext.Configuration.GetRequiredSection(nameof(ApplicationOptions.Kestrel)),
                        reloadOnChange: false);
                })
            // Used for IIS and IIS Express for in-process hosting. Use UseIISIntegration for out-of-process hosting.
            .UseIIS()
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
