namespace WebScheduler.Client.Http.Commands.ScheduledTask;

using Boxed.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Client.Core.Repositories;
using WebScheduler.Client.Http.Models.ViewModels;

/// <summary>
/// Command
/// </summary>
public class PutScheduledTaskCommand
{
    private readonly ILogger<PutScheduledTaskCommand> logger;
    private readonly IScheduledTaskRepository scheduledTaskRepository;
    private readonly IMapper<Core.Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper;
    private readonly IMapper<SaveScheduledTask, Core.Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper;

    /// <summary>
    /// Command
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="scheduledTaskRepository"></param>
    /// <param name="scheduledTaskToScheduledTaskMapper"></param>
    /// <param name="saveScheduledTaskToScheduledTaskMapper"></param>
    public PutScheduledTaskCommand(
        ILogger<PutScheduledTaskCommand> logger,
        IScheduledTaskRepository scheduledTaskRepository,
        IMapper<Core.Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper,
        IMapper<SaveScheduledTask, Core.Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper)
    {
        this.logger = logger;
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
        try
        {
            var scheduledTask = this.saveScheduledTaskToScheduledTaskMapper.Map(saveScheduledTask);
            scheduledTask.ScheduledTaskId = scheduledTaskId;

            // preserve current seconds component
            var currentTask = await this.scheduledTaskRepository.GetAsync(scheduledTask.ScheduledTaskId, cancellationToken);
            var currentSecondsComponent = currentTask.CronExpression[..currentTask.CronExpression.IndexOf(' ')];

            // Append a seconds to stagger the task times
            scheduledTask.CronExpression = $"{currentSecondsComponent} {scheduledTask.CronExpression}";

            scheduledTask = await this.scheduledTaskRepository.UpdateAsync(scheduledTask, cancellationToken);
            var scheduledTaskViewModel = this.scheduledTaskToScheduledTaskMapper.Map(scheduledTask);

            return new OkObjectResult(scheduledTaskViewModel);
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
