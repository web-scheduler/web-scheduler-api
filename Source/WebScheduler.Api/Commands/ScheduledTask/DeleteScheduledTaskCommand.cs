namespace WebScheduler.Api.Commands.ScheduledTask;

using WebScheduler.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Boxed.Mapping;
using WebScheduler.Api.ViewModels;
using WebScheduler.Abstractions.Grains.Scheduler;

public class DeleteScheduledTaskCommand
{
    private readonly IScheduledTaskRepository scheduledTaskRepository;
    private readonly IMapper<Models.ScheduledTask, ScheduledTask> scheduledTaskMapper;

    public DeleteScheduledTaskCommand(IScheduledTaskRepository scheduledTaskRepository, IMapper<Models.ScheduledTask, ScheduledTask> scheduledTaskMapper)
    {
        this.scheduledTaskRepository = scheduledTaskRepository;
        this.scheduledTaskMapper = scheduledTaskMapper;
    }

    public async Task<IActionResult> ExecuteAsync(Guid scheduledTaskId, CancellationToken cancellationToken)
    {
        try
        {
            var scheduledTask = await this.scheduledTaskRepository.GetAsync(scheduledTaskId, cancellationToken).ConfigureAwait(false);

            var result = await this.scheduledTaskRepository.DeleteAsync(scheduledTask, cancellationToken).ConfigureAwait(false);
            var scheduledTaskViewModel = this.scheduledTaskMapper.Map(result);

            return new ObjectResult(scheduledTaskViewModel)
            {
                StatusCode = StatusCodes.Status410Gone
            };
        }
        catch (ScheduledTaskNotFoundException)
        {
            return new NotFoundResult();
        }
    }
}
