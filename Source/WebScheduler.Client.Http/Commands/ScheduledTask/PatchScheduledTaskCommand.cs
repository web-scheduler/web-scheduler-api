namespace WebScheduler.Client.Http.Commands.ScheduledTask;

using Boxed.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Client.Core.Repositories;
using WebScheduler.Client.Http.Models.ViewModels;

/// <summary>
/// TODO
/// </summary>
public class PatchScheduledTaskCommand
{
    private readonly ILogger<PatchScheduledTaskCommand> logger;
    private readonly IActionContextAccessor actionContextAccessor;
    private readonly IObjectModelValidator objectModelValidator;
    private readonly IScheduledTaskRepository scheduledTaskRepository;
    private readonly IMapper<Core.Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper;
    private readonly IMapper<Core.Models.ScheduledTask, SaveScheduledTask> scheduledTaskToSaveScheduledTaskMapper;
    private readonly IMapper<SaveScheduledTask, Core.Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper;

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="actionContextAccessor"></param>
    /// <param name="objectModelValidator"></param>
    /// <param name="scheduledTaskRepository"></param>
    /// <param name="scheduledTaskToScheduledTaskMapper"></param>
    /// <param name="scheduledTaskToSaveScheduledTaskMapper"></param>
    /// <param name="saveScheduledTaskToScheduledTaskMapper"></param>
    public PatchScheduledTaskCommand(
        ILogger<PatchScheduledTaskCommand> logger,
        IActionContextAccessor actionContextAccessor,
        IObjectModelValidator objectModelValidator,
        IScheduledTaskRepository scheduledTaskRepository,
        IMapper<Core.Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper,
        IMapper<Core.Models.ScheduledTask, SaveScheduledTask> scheduledTaskToSaveScheduledTaskMapper,
        IMapper<SaveScheduledTask, Core.Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper)
    {
        this.logger = logger;
        this.actionContextAccessor = actionContextAccessor;
        this.objectModelValidator = objectModelValidator;
        this.scheduledTaskRepository = scheduledTaskRepository;
        this.scheduledTaskToScheduledTaskMapper = scheduledTaskToScheduledTaskMapper;
        this.scheduledTaskToSaveScheduledTaskMapper = scheduledTaskToSaveScheduledTaskMapper;
        this.saveScheduledTaskToScheduledTaskMapper = saveScheduledTaskToScheduledTaskMapper;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="scheduledTaskId"></param>
    /// <param name="patch"></param>
    /// <param name="cancellationToken"></param>
    public async Task<IActionResult> ExecuteAsync(
        Guid scheduledTaskId,
        JsonPatchDocument<SaveScheduledTask> patch,
        CancellationToken cancellationToken)
    {
        try
        {
            var scheduledTask = await this.scheduledTaskRepository.GetAsync(scheduledTaskId, cancellationToken);
            if (scheduledTask is null)
            {
                return new NotFoundResult();
            }
            var saveScheduledTask = this.scheduledTaskToSaveScheduledTaskMapper.Map(scheduledTask);
            var modelState = this.actionContextAccessor.ActionContext!.ModelState;
            patch.ApplyTo(saveScheduledTask, modelState);

            this.objectModelValidator.Validate(
                this.actionContextAccessor.ActionContext,
                validationState: null,
                prefix: string.Empty,
                model: saveScheduledTask);

            if (!modelState.IsValid)
            {
                return new BadRequestObjectResult(modelState);
            }

            // Preserve current seconds
            var currentSeconds = scheduledTask.CronExpression[..scheduledTask.CronExpression.IndexOf(' ')];

            this.saveScheduledTaskToScheduledTaskMapper.Map(saveScheduledTask, scheduledTask);

            if (scheduledTask.CronExpression[(currentSeconds.Length + 1)..] != saveScheduledTask.CronExpression)
            {
                scheduledTask.CronExpression = $"{currentSeconds} {saveScheduledTask.CronExpression}";
            }

            _ = await this.scheduledTaskRepository.UpdateAsync(scheduledTask, cancellationToken);
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
