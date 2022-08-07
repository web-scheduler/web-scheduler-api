namespace WebScheduler.Api.Http.Commands.ScheduledTask;

using Microsoft.AspNetCore.Mvc;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Api.Core.Repositories;

public class DeleteScheduledTaskCommand
{
    private readonly IScheduledTaskRepository scheduledTaskRepository;

    public DeleteScheduledTaskCommand(IScheduledTaskRepository scheduledTaskRepository) => this.scheduledTaskRepository = scheduledTaskRepository;

    public async Task<IActionResult> ExecuteAsync(Guid scheduledTaskId, CancellationToken cancellationToken)
    {
        try
        {
            await this.scheduledTaskRepository.DeleteAsync(scheduledTaskId, cancellationToken).ConfigureAwait(true);

            return new NoContentResult();
        }
        catch (ScheduledTaskNotFoundException)
        {
            return new NotFoundResult();
        }
    }
}
