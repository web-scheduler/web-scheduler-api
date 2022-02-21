namespace WebScheduler.Api.Repositories;
using System.Text.Json;
using Dapper;
using MySqlConnector;
using Orleans;
using Orleans.Runtime;
using WebScheduler.Abstractions.Constants;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Api.Models;
using WebScheduler.Server.Options;

public class ScheduledTaskRepository : IScheduledTaskRepository
{
    private readonly IClusterClient clusterClient;
    private readonly StorageOptions storageOptions;

    public ScheduledTaskRepository(IClusterClient clusterClient, StorageOptions storageOptions)
    {
        this.clusterClient = clusterClient;
        this.storageOptions = storageOptions;
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
        }).ConfigureAwait(false);

        return scheduledTask;
    }

    public async Task DeleteAsync(Guid scheduledTask, CancellationToken cancellationToken) => _ = await this.clusterClient.GetGrain<IScheduledTaskGrain>(scheduledTask.ToString()).DeleteAsync().ConfigureAwait(false);

    public async Task<ScheduledTask> GetAsync(Guid scheduledTaskId, CancellationToken cancellationToken)
    {
        var result = await this.clusterClient.GetGrain<IScheduledTaskGrain>(scheduledTaskId.ToString()).GetAsync().ConfigureAwait(false);
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

        const string sql = @"SELECT m.GrainIdExtensionString, m.PayloadJson FROM OrleansStorage AS m

        JOIN OrleansStorage t on t.GrainIdExtensionString = m.GrainIdExtensionString AND t.GrainTypeString='WebScheduler.Grains.Scheduler.ScheduledTaskGrain,WebScheduler.Grains.TenentState' AND JSON_EXTRACT(t.PayloadJson, '$.tenentId') = @TenentId

JOIN 
                    JSON_TABLE(
                      m.PayloadJson, 
                      '$' 
                      COLUMNS(
                        CreatedAt varchar(100) PATH '$.createdAt' DEFAULT '0' ON EMPTY
                      )
                    ) AS tt
                    ON m.GrainTypeString='WebScheduler.Grains.Scheduler.ScheduledTaskGrain,WebScheduler.Grains.ScheduledTaskMetadata'
            
          ORDER BY tt.CreatedAt ASC LIMIT @Offset, @PageSize";

        using var reader = await dbConnection.ExecuteReaderAsync(new CommandDefinition(sql, new
        {
            Offset = offset,
            PageSize = pageSize,
            TenentId = RequestContext.Get(RequestContextKeys.TenentId)
        }, cancellationToken: cancellationToken)).ConfigureAwait(false);

        var buffer = new List<ScheduledTask>(pageSize);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var result = JsonSerializer.Deserialize<ScheduledTaskMetadata>(reader.GetString(1), new JsonSerializerOptions(JsonSerializerDefaults.Web));
            if (result == null)
            {
                // TODO: handle this
                continue;
            }
            buffer.Add(new()
            {
                CreatedAt = result.CreatedAt,
                ModifiedAt = result.ModifiedAt,
                Description = result.Description,
                Name = result.Name,
                IsEnabled = result.IsEnabled,
                LastRunAt = result.LastRunAt,
                NextRunAt = result.NextRunAt,
                CronExpression = result.CronExpression,
                ScheduledTaskId = reader.GetGuid(0),
            });
        }

        return buffer;
    }
    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken)
    {
        // TODO: Figure out connection pooling
        using var dbConnection = new MySqlConnection(this.storageOptions.ConnectionString);

        const string sql = @"SELECT COUNT(*) from   OrleansStorage AS m

        JOIN OrleansStorage t on t.GrainIdExtensionString = m.GrainIdExtensionString AND t.GrainTypeString='WebScheduler.Grains.Scheduler.ScheduledTaskGrain,WebScheduler.Grains.TenentState' AND  JSON_EXTRACT(t.PayloadJson, '$.tenentId') = @TenentId AND
                    m.GrainTypeString='WebScheduler.Grains.Scheduler.ScheduledTaskGrain,WebScheduler.Grains.ScheduledTaskMetadata'";

        using var reader = await dbConnection.ExecuteReaderAsync(new CommandDefinition(sql, new
        {
            TenentId = RequestContext.Get(RequestContextKeys.TenentId)
        }, cancellationToken: cancellationToken)).ConfigureAwait(false);

        var buffer = new List<ScheduledTask>(10);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
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
        }).ConfigureAwait(false);

        return scheduledTask;
    }
}
