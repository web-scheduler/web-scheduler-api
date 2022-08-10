namespace WebScheduler.Client.Core.Mappers;

using Boxed.Mapping;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Client.Core.Models;

/// <summary>
/// Mapper for ScheduledTaskMetadata and ScheduledTask.
/// </summary>
public class ScheduledTaskMetaDataToScheduledTaskMapper : IMapper<GuidIdWrapper<ScheduledTaskMetadata>, ScheduledTask>, IMapper<ScheduledTask, GuidIdWrapper<ScheduledTaskMetadata>>
{
    // TODO: this is broken
    /// <summary>
    /// Maps the specified source object into the destination object.
    /// </summary>
    /// <param name="source">The source object to map from.</param>
    /// <param name="destination">The destination object to map to.</param>
    public void Map(GuidIdWrapper<ScheduledTaskMetadata> source, ScheduledTask destination)
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
    }

    /// <summary>
    /// Maps the specified source object into the destination object.
    /// </summary>
    /// <param name="source">The source object to map from.</param>
    /// <param name="destination">The destination object to map to.</param>
    public void Map(ScheduledTask source, GuidIdWrapper<ScheduledTaskMetadata> destination)
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

/// <summary>
/// Wraps an value with a guid identifier
/// </summary>
/// <typeparam name="TValue">type to wrap</typeparam>
/// <param name="Id">the id</param>
/// <param name="Value">the value</param>
public record GuidIdWrapper<TValue>(Guid Id, TValue Value);
