namespace WebScheduler.Client.Http;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WebScheduler.Api.Commands.ScheduledTask;
using WebScheduler.Client.Core;
using WebScheduler.Client.Http.Commands.ScheduledTask;
using WebScheduler.Client.Http.Middleware;

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
    /// Adds WebScheduler HttpClient to an <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configuration">the configuration</param>
    /// <param name="addClusterClient">if the cluster client should be registered.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebSchedulerHttpClient(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration, bool addClusterClient = false) =>
        services
            .AddWebScheduler(configuration, addClusterClient)
            .AddProjectCommands();

    internal static IServiceCollection AddProjectCommands(this IServiceCollection services) =>
        services
            .AddSingleton<DeleteScheduledTaskCommand>()
            .AddSingleton<GetScheduledTaskCommand>()
            .AddSingleton<GetScheduledTaskPageCommand>()
            .AddSingleton<PatchScheduledTaskCommand>()
            .AddSingleton<PostScheduledTaskCommand>()
            .AddSingleton<PutScheduledTaskCommand>();

    /// <summary>
    ///  Adds the WebScheduler HttpClient middleware to the <see cref="IApplicationBuilder"/> request execution pipeline.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
    /// <returns> A reference to app after the operation has completed.</returns>
    public static IApplicationBuilder UseWebScheduler(this IApplicationBuilder app) =>
        app.UseMiddleware<OrleansRequestContextAuthorization>();
}
