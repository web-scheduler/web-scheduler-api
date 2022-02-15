namespace WebScheduler.Api.ViewModels;
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
    public DateTime ModifiedAt{ get; set; }
    public DateTime CreatedAt { get; set; }
    public string CronExpression { get; set; } = "* * * * *";

    public ScheduledTask ShallowCopy() => (ScheduledTask)this.MemberwiseClone();
}
