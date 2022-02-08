namespace WebScheduler.Api;
using WebScheduler.Api.Mappers;
using WebScheduler.Api.Repositories;
using WebScheduler.Api.ViewModels;
using Boxed.Mapping;
using WebScheduler.Api.HostedServices;
using WebScheduler.Api.Commands.ScheduledTask;
using Orleans;
using WebScheduler.Api.Commands.Car;
using WebScheduler.Abstractions.Services;

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
            .AddSingleton<DeleteCarCommand>()
            .AddSingleton<GetCarCommand>()
            .AddSingleton<GetCarPageCommand>()
            .AddSingleton<PatchCarCommand>()
            .AddSingleton<PostCarCommand>()
            .AddSingleton<PutCarCommand>()
            .AddSingleton<DeleteScheduledTaskCommand>()
            .AddSingleton<GetScheduledTaskCommand>()
            .AddSingleton<GetScheduledTaskPageCommand>()
            .AddSingleton<PatchScheduledTaskCommand>()
            .AddSingleton<PostScheduledTaskCommand>()
            .AddSingleton<PutScheduledTaskCommand>();

    public static IServiceCollection AddProjectMappers(this IServiceCollection services) =>
        services
            .AddSingleton<IMapper<Models.Car, Car>, CarToCarMapper>()
            .AddSingleton<IMapper<Models.Car, SaveCar>, CarToSaveCarMapper>()
            .AddSingleton<IMapper<SaveCar, Models.Car>, CarToSaveCarMapper>()
            .AddSingleton<IMapper<Models.ScheduledTask, ScheduledTask>, ScheduledTaskToScheduledTaskMapper>()
            .AddSingleton<IMapper<Models.ScheduledTask, SaveScheduledTask>, ScheduledTaskToSaveScheduledTaskMapper>()
            .AddSingleton<IMapper<SaveScheduledTask, Models.ScheduledTask>, ScheduledTaskToSaveScheduledTaskMapper>();

    public static IServiceCollection AddProjectRepositories(this IServiceCollection services) =>
        services
            .AddSingleton<ICarRepository, CarRepository>()
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
