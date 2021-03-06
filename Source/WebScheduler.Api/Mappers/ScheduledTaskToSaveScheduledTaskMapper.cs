namespace WebScheduler.Api.Mappers;
using Boxed.Mapping;
using WebScheduler.Abstractions.Services;
using WebScheduler.Api.Models.ViewModels;

public class ScheduledTaskToSaveScheduledTaskMapper : IMapper<Models.ScheduledTask, SaveScheduledTask>, IMapper<SaveScheduledTask, Models.ScheduledTask>
{
    private readonly IClockService clockService;

    public ScheduledTaskToSaveScheduledTaskMapper(IClockService clockService) =>
        this.clockService = clockService;

    public void Map(Models.ScheduledTask source, SaveScheduledTask destination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        destination.IsEnabled = source.IsEnabled;
        destination.Description = source.Description;
        destination.Name = source.Name;
        destination.CronExpression = source.CronExpression[(source.CronExpression.IndexOf(' ') + 1)..]; // Strip off seconds
        destination.TriggerType = source.TriggerType;
        destination.HttpTriggerProperties = source.HttpTriggerProperties;
        destination.ScheduledTaskId = source.ScheduledTaskId;
    }

    public void Map(SaveScheduledTask source, Models.ScheduledTask destination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        var now = this.clockService.UtcNow;

        if (destination.CreatedAt == DateTime.MinValue)
        {
            destination.CreatedAt = now;
        }

        if (destination.ModifiedAt == DateTime.MinValue)
        {
            destination.ModifiedAt = now;
        }
        destination.IsEnabled = source.IsEnabled;
        destination.Description = source.Description;
        destination.Name = source.Name;
        destination.CronExpression = source.CronExpression;
        destination.TriggerType = source.TriggerType;
        destination.ScheduledTaskId = source.ScheduledTaskId;
        destination.HttpTriggerProperties = source.HttpTriggerProperties;
    }
}
