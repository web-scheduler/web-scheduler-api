namespace WebScheduler.Api.Mappers;
using WebScheduler.Api.ViewModels;
using Boxed.Mapping;
using WebScheduler.Abstractions.Services;

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
    }

    public void Map(SaveScheduledTask source, Models.ScheduledTask destination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        var now = this.clockService.UtcNow;

        if (destination.Created == DateTimeOffset.MinValue)
        {
            destination.Created = now;
        }
        destination.IsEnabled = source.IsEnabled;
        destination.Description = source.Description;
        destination.Name = source.Name;
        destination.Modified = now;
    }
}
