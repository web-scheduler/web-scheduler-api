namespace WebScheduler.Api.Commands.ScheduledTask;

using WebScheduler.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using WebScheduler.Abstractions.Grains.Scheduler;

public class DeleteScheduledTaskCommand
{
    private readonly IScheduledTaskRepository scheduledTaskRepository;

    public DeleteScheduledTaskCommand(IScheduledTaskRepository scheduledTaskRepository) => this.scheduledTaskRepository = scheduledTaskRepository;

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
