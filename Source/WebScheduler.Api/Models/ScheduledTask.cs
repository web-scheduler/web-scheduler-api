namespace WebScheduler.Api.Models;

using Cronos;
using WebScheduler.Abstractions.Grains.Scheduler;

/// <summary>
/// A scheduled task.
/// </summary>
public class ScheduledTask
{
    public Guid ScheduledTaskId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

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

    /// <summary>
    /// When the task was last run.
    /// </summary>
    public DateTime? LastRunAt { get; set; }

    /// <summary>
    /// When the task will next run.
    /// </summary>
    public DateTime? NextRunAt { get; set; }

    /// <summary>
    /// The cron expression to use for the task schedule. We only want users to input expressions with per-minute granularity, hence the <seealso cref="CronFormat.Standard"/>.
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
}
