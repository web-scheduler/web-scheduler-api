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
            await this.scheduledTaskRepository.DeleteAsync(scheduledTaskId, cancellationToken).ConfigureAwait(false);

            return new NoContentResult();
        }
        catch (ScheduledTaskNotFoundException)
        {
            return new NotFoundResult();
        }
    }
}
