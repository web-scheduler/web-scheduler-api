namespace WebScheduler.Api.Commands.ScheduledTask;

using WebScheduler.Api.Repositories;
using WebScheduler.Api.ViewModels;
using Boxed.Mapping;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class PatchScheduledTaskCommand
{
    private readonly IActionContextAccessor actionContextAccessor;
    private readonly IObjectModelValidator objectModelValidator;
    private readonly IScheduledTaskRepository scheduledTaskRepository;
    private readonly IMapper<Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper;
    private readonly IMapper<Models.ScheduledTask, SaveScheduledTask> scheduledTaskToSaveScheduledTaskMapper;
    private readonly IMapper<SaveScheduledTask, Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper;

    public PatchScheduledTaskCommand(
        IActionContextAccessor actionContextAccessor,
        IObjectModelValidator objectModelValidator,
        IScheduledTaskRepository scheduledTaskRepository,
        IMapper<Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper,
        IMapper<Models.ScheduledTask, SaveScheduledTask> scheduledTaskToSaveScheduledTaskMapper,
        IMapper<SaveScheduledTask, Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper)
    {
        this.actionContextAccessor = actionContextAccessor;
        this.objectModelValidator = objectModelValidator;
        this.scheduledTaskRepository = scheduledTaskRepository;
        this.scheduledTaskToScheduledTaskMapper = scheduledTaskToScheduledTaskMapper;
        this.scheduledTaskToSaveScheduledTaskMapper = scheduledTaskToSaveScheduledTaskMapper;
        this.saveScheduledTaskToScheduledTaskMapper = saveScheduledTaskToScheduledTaskMapper;
    }

    public async Task<IActionResult> ExecuteAsync(
        Guid scheduledTaskId,
        JsonPatchDocument<SaveScheduledTask> patch,
        CancellationToken cancellationToken)
    {
        var scheduledTask = await this.scheduledTaskRepository.GetAsync(scheduledTaskId, cancellationToken).ConfigureAwait(false);
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

        this.saveScheduledTaskToScheduledTaskMapper.Map(saveScheduledTask, scheduledTask);
        _ = await this.scheduledTaskRepository.UpdateAsync(scheduledTask, cancellationToken).ConfigureAwait(false);
        var scheduledTaskViewModel = this.scheduledTaskToScheduledTaskMapper.Map(scheduledTask);

        return new OkObjectResult(scheduledTaskViewModel);
    }
}
