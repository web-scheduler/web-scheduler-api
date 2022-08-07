namespace WebScheduler.Api;
using Microsoft.Extensions.DependencyInjection;
using WebScheduler.Api.Commands.ScheduledTask;
using WebScheduler.Api.Http.Commands.ScheduledTask;

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
    public static IServiceCollection AddProjectCommands(this IServiceCollection services) =>
        services
            .AddSingleton<DeleteScheduledTaskCommand>()
            .AddSingleton<GetScheduledTaskCommand>()
            .AddSingleton<GetScheduledTaskPageCommand>()
            .AddSingleton<PatchScheduledTaskCommand>()
            .AddSingleton<PostScheduledTaskCommand>()
            .AddSingleton<PutScheduledTaskCommand>();
}
