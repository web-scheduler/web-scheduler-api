namespace WebScheduler.Client.Http.Commands.ScheduledTask;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Client.Core.Repositories;

/// <summary>
/// TODO
/// </summary>
public class DeleteScheduledTaskCommand
{
    private readonly ILogger<DeleteScheduledTaskCommand> logger;
    private readonly IScheduledTaskRepository scheduledTaskRepository;

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="scheduledTaskRepository"></param>
    public DeleteScheduledTaskCommand(ILogger<DeleteScheduledTaskCommand> logger, IScheduledTaskRepository scheduledTaskRepository)
    {
        this.logger = logger;
        this.scheduledTaskRepository = scheduledTaskRepository;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="scheduledTaskId"></param>
    /// <param name="cancellationToken"></param>
    public async Task<IActionResult> ExecuteAsync(Guid scheduledTaskId, CancellationToken cancellationToken)
    {
        try
        {
            await this.scheduledTaskRepository.DeleteAsync(scheduledTaskId, cancellationToken);

            return new NoContentResult();
        }
        catch (UnauthorizedAccessException)
        {
            return new UnauthorizedResult();
        }
        catch (ScheduledTaskNotFoundException)
        {
            return new NotFoundResult();
        }
        catch (Exception ex)
        {
            this.logger.Exception(ex, ex.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
