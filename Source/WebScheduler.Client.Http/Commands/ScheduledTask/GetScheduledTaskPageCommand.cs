namespace WebScheduler.Client.Http.Commands.ScheduledTask;
using Boxed.AspNetCore;
using Boxed.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebScheduler.Abstractions.Grains.Scheduler;
using WebScheduler.Client.Core.Repositories;
using WebScheduler.Client.Http.Models.ViewModels;

/// <summary>
/// TODO
/// </summary>
public class GetScheduledTaskPageCommand
{
    private const int DefaultPageSize = 10;
    private readonly ILogger<GetScheduledTaskPageCommand> logger;
    private readonly IScheduledTaskRepository scheduledTaskRepository;
    private readonly IMapper<Core.Models.ScheduledTask, ScheduledTask> scheduledTaskMapper;
    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="scheduledTaskRepository"></param>
    /// <param name="scheduledTaskMapper"></param>
    /// <param name="httpContextAccessor"></param>
    public GetScheduledTaskPageCommand(
        ILogger<GetScheduledTaskPageCommand> logger,
        IScheduledTaskRepository scheduledTaskRepository,
        IMapper<Core.Models.ScheduledTask, ScheduledTask> scheduledTaskMapper,
        IHttpContextAccessor httpContextAccessor)
    {
        this.logger = logger;
        this.scheduledTaskRepository = scheduledTaskRepository;
        this.scheduledTaskMapper = scheduledTaskMapper;
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="pageOptions"></param>
    /// <param name="cancellationToken"></param>
    public async Task<IActionResult> ExecuteAsync(PageOptions pageOptions, CancellationToken cancellationToken)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(pageOptions);
            var httpContext = this.httpContextAccessor.HttpContext!;
            var getScheduledTasksTask = this.scheduledTaskRepository.GetScheduledTasksAsync(pageOptions.Offset, pageOptions.PageSize ?? DefaultPageSize, cancellationToken);
            var totalCountTask = this.scheduledTaskRepository.GetTotalCountAsync(cancellationToken);

            await Task.WhenAll(getScheduledTasksTask, totalCountTask);
            var scheduledTasks = await getScheduledTasksTask;
            var totalCount = await totalCountTask;

            if (scheduledTasks is null)
            {
                return new NotFoundResult();
            }

            var (startCursor, endCursor) = Cursor.GetFirstAndLastCursor(scheduledTasks, x => x.CreatedAt);
            var scheduledTaskViewModels = this.scheduledTaskMapper.MapList(scheduledTasks);

            var collection = new PageResults<ScheduledTask>()
            {
                TotalCount = totalCount,
            };
            collection.Items.AddRange(scheduledTaskViewModels);

            return new OkObjectResult(collection);
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
