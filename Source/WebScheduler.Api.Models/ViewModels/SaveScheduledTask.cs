namespace WebScheduler.Api.ViewModels;

using System.ComponentModel.DataAnnotations;


/// <summary>
///  The ScheduledTask.
/// </summary>
public class SaveScheduledTask
{
    /// <summary>
    /// Gets or sets the name of the scheduled task.
    /// </summary>
    [Required]
    [Display(Name = "Task Name", Description = "The name of the task.", ShortName = "Name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the scheduled task.
    /// </summary>
    [Required]
    [Display(Name = "Task Description", Description = "The description of what the task does.", ShortName = "Description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets if the task is enabled.
    /// </summary>
    [Display(Name = "Enabled", Description = "Determines if the task is schedulable.", ShortName = "Enabled")]
    public bool IsEnabled { get; set; }
}
