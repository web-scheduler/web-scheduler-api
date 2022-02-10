namespace WebScheduler.Api.Repositories;

using WebScheduler.Api.Models;

public interface IScheduledTaskRepository
{
    Task<ScheduledTask> AddAsync(ScheduledTask scheduledTask, CancellationToken cancellationToken);

    Task DeleteAsync(Guid scheduledTask, CancellationToken cancellationToken);

    Task<ScheduledTask> GetAsync(Guid scheduledTaskId, CancellationToken cancellationToken);

    Task<List<ScheduledTask>> GetScheduledTasksAsync(
      int? first,
      DateTimeOffset? createdAfter,
      DateTimeOffset? createdBefore,
      CancellationToken cancellationToken);

    Task<List<ScheduledTask>> GetScheduledTasksReverseAsync(
        int? last,
        DateTimeOffset? createdAfter,
        DateTimeOffset? createdBefore,
        CancellationToken cancellationToken);

    Task<bool> GetHasNextPageAsync(
        int? first,
        DateTimeOffset? createdAfter,
        CancellationToken cancellationToken);

    Task<bool> GetHasPreviousPageAsync(
        int? last,
        DateTimeOffset? createdBefore,
        CancellationToken cancellationToken);

    Task<int> GetTotalCountAsync(CancellationToken cancellationToken);

    Task<ScheduledTask> UpdateAsync(ScheduledTask scheduledTask, CancellationToken cancellationToken);
}
