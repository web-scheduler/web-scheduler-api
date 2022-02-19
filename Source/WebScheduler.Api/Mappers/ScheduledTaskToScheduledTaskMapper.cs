namespace WebScheduler.Api.Mappers;

using WebScheduler.Api.Constants;
using Boxed.Mapping;
using WebScheduler.Api.Models.ViewModels;

public class ScheduledTaskToScheduledTaskMapper : IMapper<Models.ScheduledTask, ScheduledTask>
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly LinkGenerator linkGenerator;

    public ScheduledTaskToScheduledTaskMapper(
        IHttpContextAccessor httpContextAccessor,
        LinkGenerator linkGenerator)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.linkGenerator = linkGenerator;
    }

    public void Map(Models.ScheduledTask source, ScheduledTask destination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        destination.ScheduledTaskId = source.ScheduledTaskId;
        destination.IsEnabled = source.IsEnabled;
        destination.Description = source.Description;
        destination.Name = source.Name;
        destination.CreatedAt = source.CreatedAt;
        destination.ModifiedAt = source.ModifiedAt;
        destination.NextRunAt = source.NextRunAt;
        destination.LastRunAt = source.LastRunAt;
        destination.Url = new Uri(this.linkGenerator.GetUriByRouteValues(
            this.httpContextAccessor.HttpContext!,
            ScheduledTasksControllerRoute.GetScheduledTask,
            new { source.ScheduledTaskId })!);
        destination.CronExpression = source.CronExpression;
        destination.TriggerType = source.TriggerType;
        destination.HttpTriggerProperties = source.HttpTriggerProperties;
    }
}
