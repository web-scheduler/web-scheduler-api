namespace WebScheduler.Api.Commands.ScheduledTask;

using WebScheduler.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

public class DeleteScheduledTaskCommand
{
    private readonly IScheduledTaskRepository scheduledTaskRepository;

    public DeleteScheduledTaskCommand(IScheduledTaskRepository scheduledTaskRepository) =>
        this.scheduledTaskRepository = scheduledTaskRepository;

    public async Task<IActionResult> ExecuteAsync(Guid scheduledTaskId, CancellationToken cancellationToken)
    {
        var scheduledTask = await this.scheduledTaskRepository.GetAsync(scheduledTaskId, cancellationToken).ConfigureAwait(false);
        if (scheduledTask is null)
        {
            return new NotFoundResult();
        }

        await this.scheduledTaskRepository.DeleteAsync(scheduledTask, cancellationToken).ConfigureAwait(false);

        return new NoContentResult();
    }
}
