namespace WebScheduler.Client.Core;
using Boxed.Mapping;
using Orleans;
using WebScheduler.Abstractions.Services;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Client.Http.Models.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebScheduler.Client.Core.HostedServices;
using WebScheduler.Client.Core.Mappers;
using WebScheduler.Client.Core.Repositories;
using Microsoft.Extensions.Configuration;
using WebScheduler.Client.Core.Options;
using Microsoft.Extensions.Options;
using Orleans.Configuration;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods add project services.
/// </summary>
/// <remarks>
/// AddSingleton - Only one instance is ever created and returned.
/// AddScoped - A new instance is created and returned for each request/response cycle.
/// AddTransient - A new instance is created and returned each time.
/// </remarks>
public static class ProjectServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="addClusterClient"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebScheduler(this IServiceCollection services, IConfiguration configuration, bool addClusterClient = false)
    {
        services
        .AddCustomOptions(configuration)
        .AddProjectRepositories()
        .AddProjectServices()
        .AddProjectMappers();

        if (addClusterClient)
        {
            services.AddClusterClient();
        }

        return services;
    }

    internal static IServiceCollection AddProjectMappers(this IServiceCollection services) =>
        services
            .AddSingleton<IMapper<Models.ScheduledTask, ScheduledTask>, ScheduledTaskToScheduledTaskMapper>()
            .AddSingleton<IMapper<Models.ScheduledTask, SaveScheduledTask>, ScheduledTaskToSaveScheduledTaskMapper>()
            .AddSingleton<IMapper<SaveScheduledTask, Models.ScheduledTask>, ScheduledTaskToSaveScheduledTaskMapper>()
            .AddSingleton<IMapper<GuidIdWrapper<ScheduledTaskMetadata>, Models.ScheduledTask>, ScheduledTaskMetaDataToScheduledTaskMapper>()
            .AddSingleton<IMapper<Models.ScheduledTask, GuidIdWrapper<ScheduledTaskMetadata>>, ScheduledTaskMetaDataToScheduledTaskMapper>();

    internal static IServiceCollection AddProjectRepositories(this IServiceCollection services) =>
        services
            .AddSingleton<IScheduledTaskRepository, ScheduledTaskRepository>();

    internal static IServiceCollection AddProjectServices(this IServiceCollection services) =>
        services
            .AddSingleton<IClockService, ClockService>();

    internal static IServiceCollection AddClusterClient(this IServiceCollection services) =>
     services
            .AddTransient<IClientBuilder, ClientBuilder>()
            .AddSingleton<ClusterClientHostedService>()
            .AddSingleton<IHostedService, ClusterClientHostedService>(_ => _.GetRequiredService<ClusterClientHostedService>())
            .AddSingleton(_ => _.GetRequiredService<ClusterClientHostedService>().Client);

    /// <summary>
    /// Configures the settings by binding the contents of the appsettings.json file to the specified Plain Old CLR
    /// Objects (POCO) and adding <see cref="IOptions{T}"/> objects to the services collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The services with options services added.</returns>
    internal static IServiceCollection AddCustomOptions(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            // ConfigureAndValidateSingleton registers IOptions<T> and also T as a singleton to the services collection.
            .ConfigureAndValidateSingleton<WebSchedulerClientConfigurationOptions>(configuration)
            .ConfigureAndValidateSingleton<ClusterOptions>(configuration.GetSection(nameof(WebSchedulerClientConfigurationOptions.Cluster)))
            .ConfigureAndValidateSingleton<StorageOptions>(configuration.GetSection(nameof(WebSchedulerClientConfigurationOptions.Storage)));

    /// <summary>
    /// Registers <see cref="IOptions{TOptions}"/> and <typeparamref name="TOptions"/> to the services container.
    /// Also runs data annotation validation on application startup.
    /// </summary>
    /// <typeparam name="TOptions">The type of the options.</typeparam>
    /// <param name="services">The services collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The same services collection.</returns>
    internal static IServiceCollection ConfigureAndValidateSingleton<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TOptions : class, new()
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        return services
            .AddOptions<TOptions>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Services
            .AddSingleton(x => x.GetRequiredService<IOptions<TOptions>>().Value);
    }
}
