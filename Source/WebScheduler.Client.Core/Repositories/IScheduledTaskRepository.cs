namespace WebScheduler.Client.Core.Repositories;

using WebScheduler.Client.Core.Models;

/// <summary>
/// The repository for scheduled task.
/// </summary>
public interface IScheduledTaskRepository
{
    /// <summary>
    /// Adds a new scheduled task.
    /// </summary>
    /// <param name="scheduledTask">task to add</param>
    /// <param name="cancellationToken">ct</param>
    /// <returns>The newly created scheduled task.</returns>
    Task<ScheduledTask> AddAsync(ScheduledTask scheduledTask, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a scheduled task
    /// </summary>
    /// <param name="scheduledTask">scheduled task</param>
    /// <param name="cancellationToken">ct</param>
    Task DeleteAsync(Guid scheduledTask, CancellationToken cancellationToken);
    /// <summary>
    /// Gets a scheduled task
    /// </summary>
    /// <param name="scheduledTaskId">the id</param>
    /// <param name="cancellationToken">ct</param>
    /// <returns>the scheduled task</returns>
    Task<ScheduledTask> GetAsync(Guid scheduledTaskId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a list of scheduled task by tenant id.
    /// </summary>
    /// <param name="offset">offset of tasks</param>
    /// <param name="pageSize">page size</param>
    /// <param name="cancellationToken">ct</param>
    /// <returns>a list of scheduled task</returns>
    Task<List<ScheduledTask>> GetScheduledTasksAsync(
      int offset,
      int pageSize,
      CancellationToken cancellationToken);

    /// <summary>
    /// gets count of all scheduled task for a tenant id
    /// </summary>
    /// <param name="cancellationToken">ct</param>
    /// <returns>the count</returns>
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Updates a scheduled task
    /// </summary>
    /// <param name="scheduledTask">scheduled task</param>
    /// <param name="cancellationToken">ct</param>
    /// <returns>updated scheduled task</returns>
    Task<ScheduledTask> UpdateAsync(ScheduledTask scheduledTask, CancellationToken cancellationToken);
}
