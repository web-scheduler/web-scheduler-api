namespace WebScheduler.Api.Core;
using Boxed.Mapping;
using Orleans;
using WebScheduler.Abstractions.Services;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Api.Models.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebScheduler.Api.Core.HostedServices;
using WebScheduler.Api.Core.Mappers;
using WebScheduler.Api.Core.Repositories;

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
    public static IServiceCollection AddProjectMappers(this IServiceCollection services) =>
        services
            .AddSingleton<IMapper<Models.ScheduledTask, ScheduledTask>, ScheduledTaskToScheduledTaskMapper>()
            .AddSingleton<IMapper<Models.ScheduledTask, SaveScheduledTask>, ScheduledTaskToSaveScheduledTaskMapper>()
            .AddSingleton<IMapper<SaveScheduledTask, Models.ScheduledTask>, ScheduledTaskToSaveScheduledTaskMapper>()
            .AddSingleton<IMapper<GuidIdWrapper<ScheduledTaskMetadata>, Models.ScheduledTask>, ScheduledTaskMetaDataToScheduledTaskMapper>()
            .AddSingleton<IMapper<Models.ScheduledTask, GuidIdWrapper<ScheduledTaskMetadata>>, ScheduledTaskMetaDataToScheduledTaskMapper>();

    public static IServiceCollection AddProjectRepositories(this IServiceCollection services) =>
        services
            .AddSingleton<IScheduledTaskRepository, ScheduledTaskRepository>();

    public static IServiceCollection AddProjectServices(this IServiceCollection services) =>
        services
            .AddSingleton<IClockService, ClockService>();

    public static IServiceCollection AddHostedServices(this IServiceCollection services) =>
     services
            .AddTransient<IClientBuilder, ClientBuilder>()
            .AddSingleton<ClusterClientHostedService>()
            .AddSingleton<IHostedService, ClusterClientHostedService>(_ => _.GetRequiredService<ClusterClientHostedService>())
            .AddSingleton(_ => _.GetRequiredService<ClusterClientHostedService>().Client);
}
