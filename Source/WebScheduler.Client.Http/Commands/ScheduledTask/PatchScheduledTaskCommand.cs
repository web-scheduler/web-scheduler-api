namespace WebScheduler.Client.Http.Commands.ScheduledTask;

using Boxed.Mapping;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WebScheduler.Client.Core.Repositories;
using WebScheduler.Client.Http.Models.ViewModels;

/// <summary>
/// TODO
/// </summary>
public class PatchScheduledTaskCommand
{
    private readonly IActionContextAccessor actionContextAccessor;
    private readonly IObjectModelValidator objectModelValidator;
    private readonly IScheduledTaskRepository scheduledTaskRepository;
    private readonly IMapper<Core.Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper;
    private readonly IMapper<Core.Models.ScheduledTask, SaveScheduledTask> scheduledTaskToSaveScheduledTaskMapper;
    private readonly IMapper<SaveScheduledTask, Core.Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper;

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="actionContextAccessor"></param>
    /// <param name="objectModelValidator"></param>
    /// <param name="scheduledTaskRepository"></param>
    /// <param name="scheduledTaskToScheduledTaskMapper"></param>
    /// <param name="scheduledTaskToSaveScheduledTaskMapper"></param>
    /// <param name="saveScheduledTaskToScheduledTaskMapper"></param>
    public PatchScheduledTaskCommand(
        IActionContextAccessor actionContextAccessor,
        IObjectModelValidator objectModelValidator,
        IScheduledTaskRepository scheduledTaskRepository,
        IMapper<Core.Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper,
        IMapper<Core.Models.ScheduledTask, SaveScheduledTask> scheduledTaskToSaveScheduledTaskMapper,
        IMapper<SaveScheduledTask, Core.Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper)
    {
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
        var scheduledTask = await this.scheduledTaskRepository.GetAsync(scheduledTaskId, cancellationToken).ConfigureAwait(true);
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

        _ = await this.scheduledTaskRepository.UpdateAsync(scheduledTask, cancellationToken).ConfigureAwait(true);
        var scheduledTaskViewModel = this.scheduledTaskToScheduledTaskMapper.Map(scheduledTask);

        return new OkObjectResult(scheduledTaskViewModel);
    }
}
