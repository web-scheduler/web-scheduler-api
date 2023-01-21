namespace WebScheduler.Client.Core.Repositories;
using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Orleans;
using Orleans.Runtime;
using WebScheduler.Abstractions.Constants;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Client.Core.Options;
using ScheduledTask = Models.ScheduledTask;

/// <summary>
/// Repository for <see cref="IScheduledTaskGrain"/>.
/// </summary>
public class ScheduledTaskRepository : IScheduledTaskRepository
{
    private readonly IClusterClient clusterClient;
    private readonly StorageOptions storageOptions;
    private readonly ILogger<ScheduledTaskRepository> logger;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="clusterClient">cluster client</param>
    /// <param name="storageOptions">storage options</param>
    /// <param name="logger">logger</param>
    public ScheduledTaskRepository(IClusterClient clusterClient, StorageOptions storageOptions, ILogger<ScheduledTaskRepository> logger)
    {
        this.clusterClient = clusterClient;
        this.storageOptions = storageOptions;
        this.logger = logger;
    }

    /// <summary>
    /// Adds a new scheduled task.
    /// </summary>
    /// <param name="scheduledTask">task to add</param>
    /// <param name="cancellationToken">ct</param>
    /// <returns>The newly created scheduled task.</returns>
    public async Task<ScheduledTask> AddAsync(ScheduledTask scheduledTask, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scheduledTask);

        var scheduledTaskGrain = this.clusterClient.GetGrain<IScheduledTaskGrain>(scheduledTask.ScheduledTaskId.ToString());
        var result = await scheduledTaskGrain.CreateAsync(new ScheduledTaskMetadata()
        {
            Description = scheduledTask.Description,
            IsEnabled = scheduledTask.IsEnabled,
            Name = scheduledTask.Name,
            CreatedAt = scheduledTask.CreatedAt,
            ModifiedAt = scheduledTask.ModifiedAt,
            LastRunAt = scheduledTask.LastRunAt,
            NextRunAt = scheduledTask.NextRunAt,
            CronExpression = scheduledTask.CronExpression,
            TriggerType = scheduledTask.TriggerType,
            HttpTriggerProperties = scheduledTask.HttpTriggerProperties
        });

        return new()
        {
            CreatedAt = result.CreatedAt,
            ModifiedAt = result.ModifiedAt,
            Description = result.Description,
            Name = result.Name,
            IsEnabled = result.IsEnabled,
            ScheduledTaskId = scheduledTask.ScheduledTaskId,
            LastRunAt = result.LastRunAt,
            NextRunAt = result.NextRunAt,
            CronExpression = result.CronExpression,
            TriggerType = result.TriggerType,
            HttpTriggerProperties = result.HttpTriggerProperties,
        };
    }

    /// <summary>
    /// Deletes a scheduled task
    /// </summary>
    /// <param name="scheduledTask">scheduled task</param>
    /// <param name="cancellationToken">ct</param>
    public async Task DeleteAsync(Guid scheduledTask, CancellationToken cancellationToken) => await this.clusterClient.GetGrain<IScheduledTaskGrain>(scheduledTask.ToString()).DeleteAsync();

    /// <summary>
    /// Gets a scheduled task
    /// </summary>
    /// <param name="scheduledTaskId">the id</param>
    /// <param name="cancellationToken">ct</param>
    /// <returns>the scheduled task</returns>
    public async Task<ScheduledTask> GetAsync(Guid scheduledTaskId, CancellationToken cancellationToken)
    {
        var result = await this.clusterClient.GetGrain<IScheduledTaskGrain>(scheduledTaskId.ToString()).GetAsync();
        return new()
        {
            CreatedAt = result.CreatedAt,
            ModifiedAt = result.ModifiedAt,
            Description = result.Description,
            Name = result.Name,
            IsEnabled = result.IsEnabled,
            ScheduledTaskId = scheduledTaskId,
            LastRunAt = result.LastRunAt,
            NextRunAt = result.NextRunAt,
            CronExpression = result.CronExpression,
            TriggerType = result.TriggerType,
            HttpTriggerProperties = result.HttpTriggerProperties,
        };
    }

    /// <summary>
    /// Gets a list of scheduled task by tenant id.
    /// </summary>
    /// <param name="offset">offset of tasks</param>
    /// <param name="pageSize">page size</param>
    /// <param name="cancellationToken">ct</param>
    /// <returns>a list of scheduled task</returns>
    public async Task<List<ScheduledTask>> GetScheduledTasksAsync(
        int offset,
        int pageSize,
       CancellationToken cancellationToken)
    {
        // TODO: Figure out connection poolingg
        using var dbConnection = new MySqlConnection(this.storageOptions.ConnectionString);

        // TODO: after everything migrates, delete the or clause for ScheduledTaskMetadata
        const string sql = """
        SELECT GrainIdExtensionString 
            FROM OrleansStorage 
            WHERE  
                TenantId = @TenantId
                AND IsScheduledTaskDeleted = false
                AND GrainTypeHash=2108290596
            ORDER BY ScheduledTaskCreatedAt ASC
            LIMIT @Offset, @PageSize;
        """;

        var tasks = new List<Task<ScheduledTaskMetadata>>(pageSize);
        var taskIds = new List<string>(pageSize);
        using (var reader = await dbConnection.ExecuteReaderAsync(new CommandDefinition(sql, new
        {
            Offset = offset,
            PageSize = pageSize,
            TenantId = RequestContext.Get(RequestContextKeys.TenantId)
        }, cancellationToken: cancellationToken)))
        {
            var c = this.clusterClient;
            while (await reader.ReadAsync(cancellationToken))
            {
                var taskId = reader.GetString(0);
                taskIds.Add(taskId);
                tasks.Add(c.GetGrain<IScheduledTaskGrain>(taskId).GetAsync().AsTask());
            }
            await reader.CloseAsync();
        }

        var results = await Task.WhenAll(tasks);
        var buffer = new List<ScheduledTask>(results.Length);

        for (var i = 0; i < results.Length; i++)
        {
            try
            {
                buffer.Add(new()
                {
                    ScheduledTaskId = Guid.Parse(taskIds[i]),
                    CreatedAt = results[i].CreatedAt,
                    ModifiedAt = results[i].ModifiedAt,
                    Description = results[i].Description,
                    Name = results[i].Name,
                    IsEnabled = results[i].IsEnabled,
                    LastRunAt = results[i].LastRunAt,
                    NextRunAt = results[i].NextRunAt,
                    CronExpression = results[i].CronExpression,
                    TriggerType = results[i].TriggerType,
                    HttpTriggerProperties = results[i].HttpTriggerProperties,
                });
            }
            catch (Exception ex)
            {
                this.logger.GettingScheduledTasks(ex);
                throw;
            }
        }
        return buffer;
    }

    /// <summary>
    /// gets count of all scheduled task for a tenant id
    /// </summary>
    /// <param name="cancellationToken">ct</param>
    /// <returns>the count</returns>
    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken)
    {
        // TODO: Figure out connection pooling
        using var dbConnection = new MySqlConnection(this.storageOptions.ConnectionString);

        const string sql = """
        SELECT COUNT(*) 
            FROM OrleansStorage 
            WHERE  
                TenantId = @TenantId
                AND IsScheduledTaskDeleted = false
                AND GrainTypeHash=2108290596;
        """;

        using var reader = await dbConnection.ExecuteReaderAsync(new CommandDefinition(sql, new
        {
            TenantId = RequestContext.Get(RequestContextKeys.TenantId)
        }, cancellationToken: cancellationToken));

        var buffer = new List<ScheduledTask>(10);
        while (await reader.ReadAsync(cancellationToken))
        {
            return reader.GetInt32(0);
        }
        return 0;
    }

    /// <summary>
    /// Updates a scheduled task
    /// </summary>
    /// <param name="scheduledTask">scheduled task</param>
    /// <param name="cancellationToken">ct</param>
    /// <returns>updated scheduled task</returns>
    public async Task<ScheduledTask> UpdateAsync(ScheduledTask scheduledTask, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scheduledTask);

        var scheduledTaskGrain = this.clusterClient.GetGrain<IScheduledTaskGrain>(scheduledTask.ScheduledTaskId.ToString());
        var result = await scheduledTaskGrain.UpdateAsync(new ScheduledTaskMetadata()
        {
            Description = scheduledTask.Description,
            IsEnabled = scheduledTask.IsEnabled,
            Name = scheduledTask.Name,
            CreatedAt = scheduledTask.CreatedAt,
            ModifiedAt = scheduledTask.ModifiedAt,
            LastRunAt = scheduledTask.LastRunAt,
            NextRunAt = scheduledTask.NextRunAt,
            CronExpression = scheduledTask.CronExpression,
            TriggerType = scheduledTask.TriggerType,
            HttpTriggerProperties = scheduledTask.HttpTriggerProperties
        });

        return new()
        {
            CreatedAt = result.CreatedAt,
            ModifiedAt = result.ModifiedAt,
            Description = result.Description,
            Name = result.Name,
            IsEnabled = result.IsEnabled,
            ScheduledTaskId = scheduledTask.ScheduledTaskId,
            LastRunAt = result.LastRunAt,
            NextRunAt = result.NextRunAt,
            CronExpression = result.CronExpression,
            TriggerType = result.TriggerType,
            HttpTriggerProperties = result.HttpTriggerProperties,
        };
    }
}
