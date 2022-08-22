namespace WebScheduler.Client.Http.Commands.ScheduledTask;

using System.Globalization;
using Boxed.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Client.Core.Repositories;

/// <summary>
/// TODO
/// </summary>
public class GetScheduledTaskCommand
{
    private readonly ILogger<GetScheduledTaskCommand> logger;
    private readonly IActionContextAccessor actionContextAccessor;
    private readonly IScheduledTaskRepository scheduledTaskRepository;
    private readonly IMapper<Core.Models.ScheduledTask, Models.ViewModels.ScheduledTask> scheduledTaskMapper;

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="actionContextAccessor"></param>
    /// <param name="scheduledTaskRepository"></param>
    /// <param name="scheduledTaskMapper"></param>
    public GetScheduledTaskCommand(
        ILogger<GetScheduledTaskCommand> logger,
        IActionContextAccessor actionContextAccessor,
        IScheduledTaskRepository scheduledTaskRepository,
        IMapper<Core.Models.ScheduledTask, Models.ViewModels.ScheduledTask> scheduledTaskMapper)
    {
        this.logger = logger;
        this.actionContextAccessor = actionContextAccessor;
        this.scheduledTaskRepository = scheduledTaskRepository;
        this.scheduledTaskMapper = scheduledTaskMapper;
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
            var scheduledTask = await this.scheduledTaskRepository.GetAsync(scheduledTaskId, cancellationToken);

            var httpContext = this.actionContextAccessor.ActionContext!.HttpContext;
            var ifModifiedSince = httpContext.Request.Headers.IfModifiedSince;
            if (ifModifiedSince.Count > 0 &&
                DateTime.TryParse(ifModifiedSince, out var ifModifiedSinceDateTime) &&
                ifModifiedSinceDateTime >= scheduledTask.ModifiedAt)
            {
                return new StatusCodeResult(StatusCodes.Status304NotModified);
            }

            var scheduledTaskViewModel = this.scheduledTaskMapper.Map(scheduledTask);
            httpContext.Response.Headers.LastModified = scheduledTask.ModifiedAt.ToString("R", CultureInfo.InvariantCulture);

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
