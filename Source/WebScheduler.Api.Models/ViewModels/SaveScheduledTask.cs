namespace WebScheduler.Api.Models.ViewModels;

using System.ComponentModel.DataAnnotations;
using Cronos;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Api.Models.Validators;

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

    /// <summary>
    /// The cron expression to use for the task schedule. We only want users to input expressions with per-minute granularity, hence the <seealso cref="CronFormat.Standard"/>.
    /// </summary>
    [Display(Name = "Schedule", Description = "The schedule to run the task on.")]
    [CronExpression(CronFormat.Standard)]
    public string CronExpression { get; set; } = "* * * * * ";

    /// <summary>
    /// The task trigger type.
    /// </summary>
    [Display(Name = "Trigger Type", Description = "The type of trigger to use for the task.")]
    [Required]
    public TaskTriggerType TriggerType { get; set; }

    /// <summary>
    /// The properties required to support a <see cref="TriggerType"/> value of <seealso cref="TaskTriggerType.HttpTrigger"/>
    /// </summary>
    [RequiredIf(nameof(TriggerType), TaskTriggerType.HttpTrigger)]
    [Display(Name = "Http Trigger Properties", Description = "Properties for the HttpTrigger")]
    public HttpTriggerProperties HttpTriggerProperties { get; set; } = new(new());

    /// <summary>
    /// Creates a shallow copy of the object instance.
    /// </summary>
    /// <returns>The copy.</returns>
    public SaveScheduledTask ShallowCopy() => (SaveScheduledTask)this.MemberwiseClone();
}
