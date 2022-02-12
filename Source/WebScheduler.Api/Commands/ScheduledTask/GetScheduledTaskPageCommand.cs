namespace WebScheduler.Api.Commands.ScheduledTask;

using WebScheduler.Api.Constants;
using WebScheduler.Api.Repositories;
using WebScheduler.Api.ViewModels;
using Boxed.AspNetCore;
using Boxed.Mapping;
using Microsoft.AspNetCore.Mvc;

public class GetScheduledTaskPageCommand
{
    private const int DefaultPageSize = 10;
    private readonly IScheduledTaskRepository scheduledTaskRepository;
    private readonly IMapper<Models.ScheduledTask, ScheduledTask> scheduledTaskMapper;
    private readonly IHttpContextAccessor httpContextAccessor;

    public GetScheduledTaskPageCommand(
        IScheduledTaskRepository scheduledTaskRepository,
        IMapper<Models.ScheduledTask, ScheduledTask> scheduledTaskMapper,
        IHttpContextAccessor httpContextAccessor)
    {
        this.scheduledTaskRepository = scheduledTaskRepository;
        this.scheduledTaskMapper = scheduledTaskMapper;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> ExecuteAsync(PageOptions pageOptions, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pageOptions);
        var httpContext = this.httpContextAccessor.HttpContext!;
        var getScheduledTasksTask = this.scheduledTaskRepository.GetScheduledTasksAsync(pageOptions.Offset, pageOptions.PageSize ?? DefaultPageSize, cancellationToken);
        var totalCountTask = this.scheduledTaskRepository.GetTotalCountAsync(cancellationToken);

        await Task.WhenAll(getScheduledTasksTask, totalCountTask).ConfigureAwait(false);
        var scheduledTasks = await getScheduledTasksTask.ConfigureAwait(false);
        var totalCount = await totalCountTask.ConfigureAwait(false);

        if (scheduledTasks is null)
        {
            return new NotFoundResult();
        }

        var (startCursor, endCursor) = Cursor.GetFirstAndLastCursor(scheduledTasks, x => x.Created);
        var scheduledTaskViewModels = this.scheduledTaskMapper.MapList(scheduledTasks);

        var collection = new PageResults<ScheduledTask>()
        {
            TotalCount = totalCount,
        };
        collection.Items.AddRange(scheduledTaskViewModels);

        return new OkObjectResult(collection);
    }
}
