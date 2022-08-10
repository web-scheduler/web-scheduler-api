namespace WebScheduler.Client.Http.Commands.ScheduledTask;

using Microsoft.AspNetCore.Mvc;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Client.Core.Repositories;

/// <summary>
/// TODO
/// </summary>
public class DeleteScheduledTaskCommand
{
    private readonly IScheduledTaskRepository scheduledTaskRepository;

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="scheduledTaskRepository"></param>
    public DeleteScheduledTaskCommand(IScheduledTaskRepository scheduledTaskRepository) => this.scheduledTaskRepository = scheduledTaskRepository;

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="scheduledTaskId"></param>
    /// <param name="cancellationToken"></param>
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
