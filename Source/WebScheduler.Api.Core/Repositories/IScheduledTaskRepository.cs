namespace WebScheduler.Api.Core.Repositories;

using WebScheduler.Api.Core.Models;

public interface IScheduledTaskRepository
{
    Task<ScheduledTask> AddAsync(ScheduledTask scheduledTask, CancellationToken cancellationToken);

    Task DeleteAsync(Guid scheduledTask, CancellationToken cancellationToken);

    Task<ScheduledTask> GetAsync(Guid scheduledTaskId, CancellationToken cancellationToken);

    Task<List<ScheduledTask>> GetScheduledTasksAsync(
      int offset,
      int pageSize,
      CancellationToken cancellationToken);

    Task<int> GetTotalCountAsync(CancellationToken cancellationToken);

    Task<ScheduledTask> UpdateAsync(ScheduledTask scheduledTask, CancellationToken cancellationToken);
}
