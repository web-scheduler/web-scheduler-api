namespace WebScheduler.Api.Commands.ScheduledTask;

using WebScheduler.Api.Repositories;
using Boxed.Mapping;
using Microsoft.AspNetCore.Mvc;
using WebScheduler.Api.Models.ViewModels;

public class PutScheduledTaskCommand
{
    private readonly IScheduledTaskRepository scheduledTaskRepository;
    private readonly IMapper<Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper;
    private readonly IMapper<SaveScheduledTask, Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper;
    private static readonly Random RandomNumber = new();

    public PutScheduledTaskCommand(
        IScheduledTaskRepository scheduledTaskRepository,
        IMapper<Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper,
        IMapper<SaveScheduledTask, Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper)
    {
        this.scheduledTaskRepository = scheduledTaskRepository;
        this.scheduledTaskToScheduledTaskMapper = scheduledTaskToScheduledTaskMapper;
        this.saveScheduledTaskToScheduledTaskMapper = saveScheduledTaskToScheduledTaskMapper;
    }

    public async Task<IActionResult> ExecuteAsync(Guid scheduledTaskId, SaveScheduledTask saveScheduledTask, CancellationToken cancellationToken)
    {
        var scheduledTask = this.saveScheduledTaskToScheduledTaskMapper.Map(saveScheduledTask);
        scheduledTask.ScheduledTaskId = scheduledTaskId;

        // Append a seconds to stagger the task times
        scheduledTask.CronExpression = $"{RandomNumber.Next(0, 59)} {scheduledTask.CronExpression}";

        scheduledTask = await this.scheduledTaskRepository.UpdateAsync(scheduledTask, cancellationToken).ConfigureAwait(false);
        var scheduledTaskViewModel = this.scheduledTaskToScheduledTaskMapper.Map(scheduledTask);

        return new OkObjectResult(scheduledTaskViewModel);
    }
}
