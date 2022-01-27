namespace WebScheduler.Api;
using WebScheduler.Api.Mappers;
using WebScheduler.Api.Repositories;
using WebScheduler.Api.Services;
using WebScheduler.Api.ViewModels;
using Boxed.Mapping;
using WebScheduler.Api.HostedServices;
using WebScheduler.Api.Commands.ScheduledTask;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods add project services.
/// </summary>
/// <remarks>
/// AddSingleton - Only one instance is ever created and returned.
/// AddScoped - A new instance is created and returned for each request/response cycle.
/// AddTransient - A new instance is created and returned each time.
/// </remarks>
internal static class ProjectServiceCollectionExtensions
{
    public static IServiceCollection AddProjectCommands(this IServiceCollection services) =>
        services
            .AddSingleton<DeleteScheduledTaskCommand>()
            .AddSingleton<GetScheduledTaskCommand>()
            .AddSingleton<GetScheduledTaskPageCommand>()
            .AddSingleton<PatchScheduledTaskCommand>()
            .AddSingleton<PostScheduledTaskCommand>()
            .AddSingleton<PutScheduledTaskCommand>()
            .AddSingleton<DeleteScheduledTaskCommand>()
            .AddSingleton<GetScheduledTaskCommand>()
            .AddSingleton<GetScheduledTaskPageCommand>()
            .AddSingleton<PatchScheduledTaskCommand>()
            .AddSingleton<PostScheduledTaskCommand>()
            .AddSingleton<PutScheduledTaskCommand>();

    public static IServiceCollection AddProjectMappers(this IServiceCollection services) =>
        services
            .AddSingleton<IMapper<Models.ScheduledTask, ScheduledTask>, ScheduledTaskToScheduledTaskMapper>()
            .AddSingleton<IMapper<Models.ScheduledTask, SaveScheduledTask>, ScheduledTaskToSaveScheduledTaskMapper>()
            .AddSingleton<IMapper<SaveScheduledTask, Models.ScheduledTask>, ScheduledTaskToSaveScheduledTaskMapper>()
            .AddSingleton<IMapper<Models.ScheduledTask, ScheduledTask>, ScheduledTaskToScheduledTaskMapper>()
            .AddSingleton<IMapper<Models.ScheduledTask, SaveScheduledTask>, ScheduledTaskToSaveScheduledTaskMapper>()
            .AddSingleton<IMapper<SaveScheduledTask, Models.ScheduledTask>, ScheduledTaskToSaveScheduledTaskMapper>();

    public static IServiceCollection AddProjectRepositories(this IServiceCollection services) =>
        services
            .AddSingleton<IScheduledTaskRepository, ScheduledTaskRepository>()
        .AddSingleton<IScheduledTaskRepository, ScheduledTaskRepository>();

    public static IServiceCollection AddProjectServices(this IServiceCollection services) =>
        services
            .AddSingleton<IClockService, ClockService>();

    public static IServiceCollection AddHostedServices(this IServiceCollection services) =>
     services.AddSingleton<ClusterClientHostedService>()
            .AddSingleton<IHostedService>(_ => _.GetRequiredService<ClusterClientHostedService>())
            .AddSingleton(_ => _.GetRequiredService<ClusterClientHostedService>().Client);
}
