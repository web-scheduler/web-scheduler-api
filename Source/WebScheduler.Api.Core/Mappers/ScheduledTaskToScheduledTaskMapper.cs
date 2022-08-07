namespace WebScheduler.Api.Core.Mappers;

using Boxed.Mapping;

public class ScheduledTaskToScheduledTaskMapper : IMapper<Models.ScheduledTask, Api.Models.ViewModels.ScheduledTask>
{
    public void Map(Models.ScheduledTask source, Api.Models.ViewModels.ScheduledTask destination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        destination.ScheduledTaskId = source.ScheduledTaskId;
        destination.IsEnabled = source.IsEnabled;
        destination.Description = source.Description;
        destination.Name = source.Name;
        destination.CreatedAt = source.CreatedAt;
        destination.ModifiedAt = source.ModifiedAt;
        destination.NextRunAt = source.NextRunAt;
        destination.LastRunAt = source.LastRunAt;
        destination.CronExpression = source.CronExpression;
        destination.TriggerType = source.TriggerType;
        destination.HttpTriggerProperties = source.HttpTriggerProperties;
    }
}
