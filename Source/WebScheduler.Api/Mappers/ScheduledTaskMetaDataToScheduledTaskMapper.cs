namespace WebScheduler.Api.Mappers;

using WebScheduler.Api.Constants;
using Boxed.Mapping;
using WebScheduler.Abstractions.Grains.Scheduler;

public class ScheduledTaskMetaDataToScheduledTaskMapper : IMapper<GuidIdWrapper<ScheduledTaskMetadata>, Models.ScheduledTask>, IMapper<Models.ScheduledTask, GuidIdWrapper<ScheduledTaskMetadata>>
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly LinkGenerator linkGenerator;

    public ScheduledTaskMetaDataToScheduledTaskMapper(
        IHttpContextAccessor httpContextAccessor,
        LinkGenerator linkGenerator)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.linkGenerator = linkGenerator;
    }

    // TODO: this is broken
    public void Map(GuidIdWrapper<ScheduledTaskMetadata> source, Models.ScheduledTask destination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        destination.ScheduledTaskId = source.Id;
        destination.IsEnabled = source.Value.IsEnabled;
        destination.Description = source.Value.Description;
        destination.Name = source.Value.Name;
        destination.CreatedAt = source.Value.CreatedAt;
        destination.ModifiedAt = source.Value.ModifiedAt;
        destination.CronExpression = source.Value.CronExpression;
        destination.NextRunAt = source.Value.NextRunAt;
        destination.LastRunAt = source.Value.LastRunAt;
        destination.HttpTriggerProperties = source.Value.HttpTriggerProperties;
        destination.TriggerType = source.Value.TriggerType;
        destination.Url = new Uri(this.linkGenerator.GetUriByRouteValues(
            this.httpContextAccessor.HttpContext!,
            ScheduledTasksControllerRoute.GetScheduledTask,
            new { source.Id })!);
    }

    public void Map(Models.ScheduledTask source, GuidIdWrapper<ScheduledTaskMetadata> destination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);
        //var 
        //destination.Id = source.ScheduledTaskId;
        //destination.IsEnabled = source.IsEnabled;
        //destination.Description = source.Value.Description;
        //destination.Name = source.Value.Name;
        //destination.Url = new Uri(this.linkGenerator.GetUriByRouteValues(
        //    this.httpContextAccessor.HttpContext!,
        //    ScheduledTasksControllerRoute.GetScheduledTask,
        //    new { source.Id })!);
    }
}

public record GuidIdWrapper<TValue>(Guid Id, TValue Value);
