namespace WebScheduler.Api.Models.ViewModels;

using WebScheduler.Abstractions.Grains.Scheduler;

/// <summary>
/// A make and model of car.
/// </summary>
public class ScheduledTask
{
    /// <summary>
    /// Gets or sets the scheduled task's unique identifier.
    /// </summary>
    /// <example>1AE8039D-E152-46E6-ADD6-BA14F1C5EE4A</example>
    public Guid ScheduledTaskId { get; set; }

    /// <summary>
    /// The name of the scheduled task.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the scheduled task.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Specifies if the task is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the URL used to retrieve the resource conforming to REST'ful JSON http://restfuljson.org/.
    /// </summary>
    /// <example>/scheduledtask/1</example>
    public Uri Url { get; set; } = default!;

    public DateTime? LastRunAt { get; set; }
    public DateTime? NextRunAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The cron expression to use for the task schedule.
    /// </summary>
    public string CronExpression { get; set; } = string.Empty;

    /// <summary>
    /// The task trigger type.
    /// </summary>
    public TaskTriggerType TriggerType { get; set; }

    /// <summary>
    /// The properties required to support a <see cref="TriggerType"/> value of <seealso cref="TaskTriggerType.HttpTrigger"/>
    /// </summary>
    public HttpTriggerProperties HttpTriggerProperties { get; set; } = new();

    /// <summary>
    /// Creates a shallow copy of the object instance.
    /// </summary>
    /// <returns>The copy.</returns>
    public ScheduledTask ShallowCopy() => (ScheduledTask)this.MemberwiseClone();
}
