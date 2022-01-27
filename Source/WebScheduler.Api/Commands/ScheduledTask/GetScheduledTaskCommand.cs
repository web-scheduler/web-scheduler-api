namespace WebScheduler.Api.Commands.ScheduledTask;

using System.Globalization;
using WebScheduler.Api.Repositories;
using WebScheduler.Api.ViewModels;
using Boxed.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

public class GetScheduledTaskCommand
{
    private readonly IActionContextAccessor actionContextAccessor;
    private readonly IScheduledTaskRepository scheduledTaskRepository;
    private readonly IMapper<Models.ScheduledTask, ScheduledTask> scheduledTaskMapper;

    public GetScheduledTaskCommand(
        IActionContextAccessor actionContextAccessor,
        IScheduledTaskRepository scheduledTaskRepository,
        IMapper<Models.ScheduledTask, ScheduledTask> scheduledTaskMapper)
    {
        this.actionContextAccessor = actionContextAccessor;
        this.scheduledTaskRepository = scheduledTaskRepository;
        this.scheduledTaskMapper = scheduledTaskMapper;
    }

    public async Task<IActionResult> ExecuteAsync(Guid scheduledTaskId, CancellationToken cancellationToken)
    {
        var scheduledTask = await this.scheduledTaskRepository.GetAsync(scheduledTaskId, cancellationToken).ConfigureAwait(false);
        if (scheduledTask is null)
        {
            return new NotFoundResult();
        }

        var httpContext = this.actionContextAccessor.ActionContext!.HttpContext;
        var ifModifiedSince = httpContext.Request.Headers.IfModifiedSince;
        if (ifModifiedSince.Count > 0 &&
            DateTimeOffset.TryParse(ifModifiedSince, out var ifModifiedSinceDateTime) &&
            (ifModifiedSinceDateTime >= scheduledTask.Modified))
        {
            return new StatusCodeResult(StatusCodes.Status304NotModified);
        }

        var scheduledTaskViewModel = this.scheduledTaskMapper.Map(scheduledTask);
        httpContext.Response.Headers.LastModified = scheduledTask.Modified.ToString("R", CultureInfo.InvariantCulture);
        return new OkObjectResult(scheduledTaskViewModel);
    }
}
