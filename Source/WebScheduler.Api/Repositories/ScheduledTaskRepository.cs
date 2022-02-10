namespace WebScheduler.Api.Repositories;

using System.Runtime.CompilerServices;
using System.Text.Json;
using Boxed.Mapping;
using Dapper;
using MySqlConnector;
using Orleans;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Api.Mappers;
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

        var result = await scheduledTaskGrain.CreateAsync(new ScheduledTaskMetadata()
        {
            Description = scheduledTask.Description,
            IsEnabled = scheduledTask.IsEnabled,
            Name = scheduledTask.Name,
            Created = scheduledTask.Created,
            Modified = scheduledTask.Modified,
        }).ConfigureAwait(false);


        return scheduledTask;
    }

    public async Task DeleteAsync(Guid scheduledTask, CancellationToken cancellationToken)
    {
        _ = await this.clusterClient.GetGrain<IScheduledTaskGrain>(scheduledTask.ToString()).DeleteAsync().ConfigureAwait(false);
    }

    public async Task<ScheduledTask> GetAsync(Guid scheduledTaskId, CancellationToken cancellationToken)
    {
        var result = await this.clusterClient.GetGrain<IScheduledTaskGrain>(scheduledTaskId.ToString()).GetAsync().ConfigureAwait(false);
        return new()
        {
            Created = result.Created,
            Modified = result.Modified,
            Description = result.Description,
            Name = result.Name,
            IsEnabled = result.IsEnabled,
            ScheduledTaskId = scheduledTaskId,
        };
    }

    public async Task<List<ScheduledTask>> GetScheduledTasksAsync(
        int? first,
        DateTimeOffset? createdAfter,
        DateTimeOffset? createdBefore,
       CancellationToken cancellationToken)
    {
        // TODO: Figure out connection pooling
        using var dbConnection = new MySqlConnection(this.storageOptions.ConnectionString);
        
        var sql = @"SELECT m.GrainIdExtensionString, m.PayloadJson FROM OrleansStorage AS m JOIN 
                    JSON_TABLE(
                      m.PayloadJson, 
                      '$' 
                      COLUMNS(
                        Created varchar(100) PATH '$.created' DEFAULT '0' ON EMPTY
                      )
                    ) AS tt
                    ON m.GrainTypeString='WebScheduler.Grains.Scheduler.ScheduledTaskGrain,WebScheduler.Grains.ScheduledTaskMetadata'";
        if (createdAfter != null || createdBefore != null)
        {
            sql += @"
                    AND ";
            if (createdAfter != null)
            {
                sql += "tt.Created < @createdAfter";
            }
            if (createdAfter != null && createdBefore != null)
            {
                sql += " AND ";
            }
            if (createdAfter != null)
            {
                sql += "tt.Created > @createdbefore ";
            }
        }
        if (first != null)
        {
            sql += $" ORDER BY tt.Created LIMIT {first}, 10";
        }

        object parameters = new CreatedBeforeAndAfterClause(createdAfter, createdBefore) switch
        {
            (CreatedAfter: null, CreatedBefore: null) => new { },
            (CreatedAfter: not null, CreatedBefore: not null) => new { CreatedAfter = createdAfter, CreatedBefore = createdBefore },
            (CreatedAfter: not null, CreatedBefore: null) => new { CreatedAfter = createdAfter },
            (CreatedAfter: null, CreatedBefore: not null) => new { CreatedBefore = createdBefore },
        };
        using var reader = await dbConnection.ExecuteReaderAsync(new CommandDefinition(sql, cancellationToken: cancellationToken)).ConfigureAwait(false);

        var buffer = new List<ScheduledTask>(10);
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
                Created = result.Created,
                Modified = result.Modified,
                Description = result.Description,
                Name = result.Name,
                IsEnabled = result.IsEnabled,
                ScheduledTaskId = reader.GetGuid(0),
            });
        }

        return buffer;
    }
    private record struct CreatedBeforeAndAfterClause(DateTimeOffset? CreatedAfter, DateTimeOffset? CreatedBefore);

    public Task<List<ScheduledTask>> GetScheduledTasksReverseAsync(
        int? last,
        DateTimeOffset? createdAfter,
        DateTimeOffset? createdBefore,
        CancellationToken cancellationToken)
    {

        return Task.FromResult(new List<ScheduledTask>());
    }

    public async Task<bool> GetHasNextPageAsync(
        int? first,
        DateTimeOffset? createdAfter,
        CancellationToken cancellationToken) => (await this.GetTotalCountAsync(cancellationToken)) > (first ?? 0);

    public async Task<bool> GetHasPreviousPageAsync(
        int? last,
        DateTimeOffset? createdBefore,
        CancellationToken cancellationToken) => (await this.GetTotalCountAsync(cancellationToken)) < (last ?? 0);

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken)
    {
        // TODO: Figure out connection pooling
        using var dbConnection = new MySqlConnection(this.storageOptions.ConnectionString);

        var sql = @"SELECT COUNT(*) from OrleansStorage
                    where GrainTypeString='WebScheduler.Grains.Scheduler.ScheduledTaskGrain,WebScheduler.Grains.ScheduledTaskMetadata'";

        using var reader = await dbConnection.ExecuteReaderAsync(new CommandDefinition(sql, cancellationToken: cancellationToken)).ConfigureAwait(false);

        var buffer = new List<ScheduledTask>(10);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            return reader.GetInt32(0);
        }
        return 0;
    }

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
