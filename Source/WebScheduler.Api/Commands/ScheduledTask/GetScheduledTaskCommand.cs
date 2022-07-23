namespace WebScheduler.Api.Commands.ScheduledTask;

using System.Globalization;
using WebScheduler.Api.Repositories;
using Boxed.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Api.Models.ViewModels;

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
        try
        {
            var scheduledTask = await this.scheduledTaskRepository.GetAsync(scheduledTaskId, cancellationToken).ConfigureAwait(true);

            var httpContext = this.actionContextAccessor.ActionContext!.HttpContext;
            var ifModifiedSince = httpContext.Request.Headers.IfModifiedSince;
            if (ifModifiedSince.Count > 0 &&
                DateTime.TryParse(ifModifiedSince, out var ifModifiedSinceDateTime) &&
                (ifModifiedSinceDateTime >= scheduledTask.ModifiedAt))
            {
                return new StatusCodeResult(StatusCodes.Status304NotModified);
            }

            var scheduledTaskViewModel = this.scheduledTaskMapper.Map(scheduledTask);
            httpContext.Response.Headers.LastModified = scheduledTask.ModifiedAt.ToString("R", CultureInfo.InvariantCulture);

            return new OkObjectResult(scheduledTaskViewModel);
        }
        catch (ScheduledTaskNotFoundException)
        {
            return new NotFoundResult();
        }
    }
}
