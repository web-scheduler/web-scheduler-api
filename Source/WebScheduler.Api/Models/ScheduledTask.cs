namespace WebScheduler.Api.Models;
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
    public DateTime? LastRunAt { get; set; }
    public DateTime? NextRunAt { get; set; }
    public string CronExpression { get; set; } = "* * * * *";
}
