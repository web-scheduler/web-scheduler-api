namespace WebScheduler.Client.Core.Repositories;
using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Orleans;
using Orleans.Runtime;
using WebScheduler.Abstractions.Constants;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Client.Core.Models;
using WebScheduler.Client.Core.Options;

public class ScheduledTaskRepository : IScheduledTaskRepository
{
    private readonly IClusterClient clusterClient;
    private readonly StorageOptions storageOptions;
    private readonly ILogger<ScheduledTaskRepository> logger;
    public ScheduledTaskRepository(IClusterClient clusterClient, StorageOptions storageOptions, ILogger<ScheduledTaskRepository> logger)
    {
        this.clusterClient = clusterClient;
        this.storageOptions = storageOptions;
        this.logger = logger;
    }

    public async Task<ScheduledTask> AddAsync(ScheduledTask scheduledTask, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scheduledTask);

        var scheduledTaskGrain = this.clusterClient.GetGrain<IScheduledTaskGrain>(scheduledTask.ScheduledTaskId.ToString());
        _ = await scheduledTaskGrain.CreateAsync(new ScheduledTaskMetadata()
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
        }).ConfigureAwait(true);

        return scheduledTask;
    }

    public async Task DeleteAsync(Guid scheduledTask, CancellationToken cancellationToken) => await this.clusterClient.GetGrain<IScheduledTaskGrain>(scheduledTask.ToString()).DeleteAsync().ConfigureAwait(true);

    public async Task<ScheduledTask> GetAsync(Guid scheduledTaskId, CancellationToken cancellationToken)
    {
        var result = await this.clusterClient.GetGrain<IScheduledTaskGrain>(scheduledTaskId.ToString()).GetAsync().ConfigureAwait(true);
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

    public async Task<List<ScheduledTask>> GetScheduledTasksAsync(
        int offset,
        int pageSize,
       CancellationToken cancellationToken)
    {
        // TODO: Figure out connection pooling
        using var dbConnection = new MySqlConnection(this.storageOptions.ConnectionString);

        // TODO: after everything migrates, delete the or clause for ScheduledTaskMetadata
        const string sql = @"SELECT GrainIdExtensionString FROM OrleansStorage WHERE  JSON_EXTRACT(PayloadJson, '$.tenantId') = @TenantId AND IFNULL(JSON_EXTRACT(PayloadJson, '$.isDeleted'), 0)=0
        AND GrainTypeString='WebScheduler.Grains.Scheduler.ScheduledTaskGrain,WebScheduler.Grains.ScheduledTaskState'
        AND JSON_EXTRACT(PayloadJson, '$.isDeleted') is null
          ORDER BY JSON_EXTRACT(PayloadJson, '$.createdAt') ASC LIMIT @Offset, @PageSize";

        var tasks = new List<Task<ScheduledTaskMetadata>>(pageSize);
        var taskIds = new List<string>(pageSize);
        using (var reader = await dbConnection.ExecuteReaderAsync(new CommandDefinition(sql, new
        {
            Offset = offset,
            PageSize = pageSize,
            TenantId = RequestContext.Get(RequestContextKeys.TenantId)
        }, cancellationToken: cancellationToken)).ConfigureAwait(true))
        {
            var c = this.clusterClient;
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(true))
            {
                var taskId = reader.GetString(0);
                taskIds.Add(taskId);
                tasks.Add(c.GetGrain<IScheduledTaskGrain>(taskId).GetAsync().AsTask());
            }
            await reader.CloseAsync().ConfigureAwait(true);
        }

        var results = await Task.WhenAll(tasks).ConfigureAwait(true);
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

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken)
    {
        // TODO: Figure out connection pooling
        using var dbConnection = new MySqlConnection(this.storageOptions.ConnectionString);

        const string sql = @"SELECT COUNT(*) from OrleansStorage
         WHERE  JSON_EXTRACT(PayloadJson, '$.tenantId') = @TenantId AND IFNULL(JSON_EXTRACT(PayloadJson, '$.isDeleted'), 0)=0
        AND GrainTypeString='WebScheduler.Grains.Scheduler.ScheduledTaskGrain,WebScheduler.Grains.ScheduledTaskState'
        AND JSON_EXTRACT(PayloadJson, '$.isDeleted') is null";

        using var reader = await dbConnection.ExecuteReaderAsync(new CommandDefinition(sql, new
        {
            TenantId = RequestContext.Get(RequestContextKeys.TenantId)
        }, cancellationToken: cancellationToken)).ConfigureAwait(true);

        var buffer = new List<ScheduledTask>(10);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(true))
        {
            return reader.GetInt32(0);
        }
        return 0;
    }

    public async Task<ScheduledTask> UpdateAsync(ScheduledTask scheduledTask, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scheduledTask);

        var scheduledTaskGrain = this.clusterClient.GetGrain<IScheduledTaskGrain>(scheduledTask.ScheduledTaskId.ToString());
        _ = await scheduledTaskGrain.UpdateAsync(new ScheduledTaskMetadata()
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
        }).ConfigureAwait(true);

        return scheduledTask;
    }
}
