namespace WebScheduler.Api.Mappers;
using Boxed.Mapping;
using WebScheduler.Abstractions.Grains.Scheduler;
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
        destination.CronExpression = source.CronExpression;
        destination.TriggerType = source.TriggerType;
        switch (source.TriggerType)
        {
            case TaskTriggerType.HttpTrigger:
                destination.HttpTriggerProperties = HttpTriggerProperties.FromKeyValuePair(source.TriggerProperties);
                break;
            default:
                throw new NotImplementedException($"Trigger type of: {nameof(source.TriggerType)}.");
        }
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
        destination.IsEnabled = source.IsEnabled;
        destination.Description = source.Description;
        destination.Name = source.Name;
        destination.CronExpression = source.CronExpression;
        destination.TriggerType = source.TriggerType;

        destination.TriggerProperties = source.TriggerType switch
        {
            TaskTriggerType.HttpTrigger => source.HttpTriggerProperties.GetKeyValuePairs(),
            _ => throw new NotImplementedException($"Trigger type of: {nameof(source.TriggerType)}."),
        };
    }
}
