namespace WebScheduler.Client.Core.Mappers;

using Boxed.Mapping;
using WebScheduler.Client.Http.Models.ViewModels;

public class ScheduledTaskToScheduledTaskMapper : IMapper<Models.ScheduledTask, ScheduledTask>
{
    public void Map(Models.ScheduledTask source, ScheduledTask destination)
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
