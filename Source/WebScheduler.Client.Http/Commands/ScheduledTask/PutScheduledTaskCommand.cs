namespace WebScheduler.Client.Http.Commands.ScheduledTask;

using Boxed.Mapping;
using Microsoft.AspNetCore.Mvc;
using WebScheduler.Client.Http.Models.ViewModels;
using WebScheduler.Client.Core.Repositories;

/// <summary>
/// Command
/// </summary>
public class PutScheduledTaskCommand
{
    private readonly IScheduledTaskRepository scheduledTaskRepository;
    private readonly IMapper<Core.Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper;
    private readonly IMapper<SaveScheduledTask, Core.Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper;
    private static readonly Random RandomNumber = new();

    /// <summary>
    /// Command
    /// </summary>
    /// <param name="scheduledTaskRepository"></param>
    /// <param name="scheduledTaskToScheduledTaskMapper"></param>
    /// <param name="saveScheduledTaskToScheduledTaskMapper"></param>
    public PutScheduledTaskCommand(
        IScheduledTaskRepository scheduledTaskRepository,
        IMapper<Core.Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper,
        IMapper<SaveScheduledTask, Core.Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper)
    {
        this.scheduledTaskRepository = scheduledTaskRepository;
        this.scheduledTaskToScheduledTaskMapper = scheduledTaskToScheduledTaskMapper;
        this.saveScheduledTaskToScheduledTaskMapper = saveScheduledTaskToScheduledTaskMapper;
    }

    /// <summary>
    /// Command
    /// </summary>
    /// <param name="scheduledTaskId"></param>
    /// <param name="saveScheduledTask"></param>
    /// <param name="cancellationToken"></param>
    public async Task<IActionResult> ExecuteAsync(Guid scheduledTaskId, SaveScheduledTask saveScheduledTask, CancellationToken cancellationToken)
    {
        var scheduledTask = this.saveScheduledTaskToScheduledTaskMapper.Map(saveScheduledTask);
        scheduledTask.ScheduledTaskId = scheduledTaskId;

        // Append a seconds to stagger the task times
        scheduledTask.CronExpression = $"{RandomNumber.Next(0, 59)} {scheduledTask.CronExpression}";

        scheduledTask = await this.scheduledTaskRepository.UpdateAsync(scheduledTask, cancellationToken);
        var scheduledTaskViewModel = this.scheduledTaskToScheduledTaskMapper.Map(scheduledTask);

        return new OkObjectResult(scheduledTaskViewModel);
    }
}
