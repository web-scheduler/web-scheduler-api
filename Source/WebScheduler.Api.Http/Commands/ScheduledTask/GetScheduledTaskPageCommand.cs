namespace WebScheduler.Api.Http.Commands.ScheduledTask;
using Boxed.AspNetCore;
using Boxed.Mapping;
using Microsoft.AspNetCore.Mvc;
using WebScheduler.Api.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using WebScheduler.Api.Core.Repositories;

public class GetScheduledTaskPageCommand
{
    private const int DefaultPageSize = 10;
    private readonly IScheduledTaskRepository scheduledTaskRepository;
    private readonly IMapper<Core.Models.ScheduledTask, ScheduledTask> scheduledTaskMapper;
    private readonly IHttpContextAccessor httpContextAccessor;

    public GetScheduledTaskPageCommand(
        IScheduledTaskRepository scheduledTaskRepository,
        IMapper<Core.Models.ScheduledTask, ScheduledTask> scheduledTaskMapper,
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

        await Task.WhenAll(getScheduledTasksTask, totalCountTask).ConfigureAwait(true);
        var scheduledTasks = await getScheduledTasksTask.ConfigureAwait(true);
        var totalCount = await totalCountTask.ConfigureAwait(true);

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
}
