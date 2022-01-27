namespace WebScheduler.Api.Models;
/// <summary>
/// A scheduled task.
/// </summary>
public class ScheduledTask
{
    public Guid ScheduledTaskId { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset Modified { get; set; }
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
}
