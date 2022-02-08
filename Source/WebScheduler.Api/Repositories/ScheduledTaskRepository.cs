namespace WebScheduler.Api.Repositories;

using Orleans;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Api.Models;

public class ScheduledTaskRepository : IScheduledTaskRepository
{
    private readonly ILogger<ScheduledTaskRepository> logger;
    private readonly IClusterClient clusterClient;

    public ScheduledTaskRepository(ILogger<ScheduledTaskRepository> logger, IClusterClient clusterClient)
    {
        this.logger = logger;
        this.clusterClient = clusterClient;
    }

    public async Task<ScheduledTask> AddAsync(ScheduledTask scheduledTask, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scheduledTask);
        if (scheduledTask.ScheduledTaskId == Guid.Empty)
        {
            scheduledTask.ScheduledTaskId = Guid.NewGuid();
        }
        var scheduledTaskGrain = this.clusterClient.GetGrain<IScheduledTaskGrain>(scheduledTask.ScheduledTaskId.ToString());

        var result = await scheduledTaskGrain.CreateAsync(new ScheduledTaskMetadata()
        {
            Description = scheduledTask.Description,
            IsEnabled = scheduledTask.IsEnabled,
            Name = scheduledTask.Name,
            Created = scheduledTask.Created,
            Modified = scheduledTask.Modified,
        }).ConfigureAwait(false);

        return new()
        {
            Description = result.Description,
            IsEnabled = result.IsEnabled,
            Name = result.Name,
            Created = result.Created,
            Modified = result.Modified,
        };
    }

    public async Task<ScheduledTask> DeleteAsync(ScheduledTask scheduledTask, CancellationToken cancellationToken)
    {
        var scheduledTaskGrain = this.clusterClient.GetGrain<IScheduledTaskGrain>(scheduledTask.ScheduledTaskId.ToString());
        var result = await scheduledTaskGrain.DeleteAsync().ConfigureAwait(false);
        return new()
        {
            Description = result.Description,
            IsEnabled = result.IsEnabled,
            Name = result.Name,
            Created = result.Created,
            Modified = result.Modified,
        };
    }

    public async Task<ScheduledTask> GetAsync(Guid scheduledTaskId, CancellationToken cancellationToken)
    {
        var scheduledTask = await this.clusterClient.GetGrain<IScheduledTaskGrain>(scheduledTaskId.ToString())
            .GetAsync().ConfigureAwait(false);

        return new ScheduledTask()
        {
            Created = scheduledTask.Created,
            Description = scheduledTask.Description,
            IsEnabled = scheduledTask.IsEnabled,
            Modified = scheduledTask.Modified,
            Name = scheduledTask.Name,
            ScheduledTaskId = scheduledTaskId,
        };
    }

    public Task<List<ScheduledTask>> GetScheduledTasksAsync(
        int? first,
        DateTimeOffset? createdAfter,
        DateTimeOffset? createdBefore,
        CancellationToken cancellationToken) => Task.FromResult(new List<ScheduledTask>());

    public Task<List<ScheduledTask>> GetScheduledTasksReverseAsync(
        int? last,
        DateTimeOffset? createdAfter,
        DateTimeOffset? createdBefore,
        CancellationToken cancellationToken) => Task.FromResult(new List<ScheduledTask>());

    public Task<bool> GetHasNextPageAsync(
        int? first,
        DateTimeOffset? createdAfter,
        CancellationToken cancellationToken) => Task.FromResult(false);

    public Task<bool> GetHasPreviousPageAsync(
        int? last,
        DateTimeOffset? createdBefore,
        CancellationToken cancellationToken) => Task.FromResult(false);

    public Task<int> GetTotalCountAsync(CancellationToken cancellationToken) => Task.FromResult(0);

    public Task<ScheduledTask> UpdateAsync(ScheduledTask scheduledTask, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scheduledTask);

        //var existingScheduledTask = ScheduledTasks.First(x => x.ScheduledTaskId == ScheduledTask.ScheduledTaskId);
        //existingScheduledTask.Description = ScheduledTask.Description;
        //existingScheduledTask.Name = ScheduledTask.Name;
        //existingScheduledTask.IsEnabled = ScheduledTask.Model;
        return Task.FromResult(scheduledTask);
    }
}
