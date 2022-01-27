namespace WebScheduler.Api.Commands.ScheduledTask;

using WebScheduler.Api.Constants;
using WebScheduler.Api.Repositories;
using WebScheduler.Api.ViewModels;
using Boxed.Mapping;
using Microsoft.AspNetCore.Mvc;

public class PostScheduledTaskCommand
{
    private readonly IScheduledTaskRepository scheduledTaskRepository;
    private readonly IMapper<Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper;
    private readonly IMapper<SaveScheduledTask, Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper;

    public PostScheduledTaskCommand(
        IScheduledTaskRepository scheduledTaskRepository,
        IMapper<Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper,
        IMapper<SaveScheduledTask, Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper)
    {
        this.scheduledTaskRepository = scheduledTaskRepository;
        this.scheduledTaskToScheduledTaskMapper = scheduledTaskToScheduledTaskMapper;
        this.saveScheduledTaskToScheduledTaskMapper = saveScheduledTaskToScheduledTaskMapper;
    }

    public async Task<IActionResult> ExecuteAsync(SaveScheduledTask saveScheduledTask, CancellationToken cancellationToken)
    {
        var scheduledTask = this.saveScheduledTaskToScheduledTaskMapper.Map(saveScheduledTask);
        scheduledTask = await this.scheduledTaskRepository.AddAsync(scheduledTask, cancellationToken).ConfigureAwait(false);
        var scheduledTaskViewModel = this.scheduledTaskToScheduledTaskMapper.Map(scheduledTask);

        return new CreatedAtRouteResult(
            ScheduledTasksControllerRoute.GetScheduledTask,
            new { scheduledTaskViewModel.ScheduledTaskId },
            scheduledTaskViewModel);
    }
}
