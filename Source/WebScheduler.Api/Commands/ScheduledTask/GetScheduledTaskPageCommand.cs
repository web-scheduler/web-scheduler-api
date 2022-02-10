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
    private readonly LinkGenerator linkGenerator;

    public GetScheduledTaskPageCommand(
        IScheduledTaskRepository scheduledTaskRepository,
        IMapper<Models.ScheduledTask, ScheduledTask> scheduledTaskMapper,
        IHttpContextAccessor httpContextAccessor,
        LinkGenerator linkGenerator)
    {
        this.scheduledTaskRepository = scheduledTaskRepository;
        this.scheduledTaskMapper = scheduledTaskMapper;
        this.httpContextAccessor = httpContextAccessor;
        this.linkGenerator = linkGenerator;
    }

    public async Task<IActionResult> ExecuteAsync(PageOptions pageOptions, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pageOptions);

        pageOptions.First = !pageOptions.First.HasValue && !pageOptions.Last.HasValue ? 1 : pageOptions.First;
        var createdAfter = Cursor.FromCursor<DateTimeOffset?>(pageOptions.After);
        var createdBefore = Cursor.FromCursor<DateTimeOffset?>(pageOptions.Before);

        var getScheduledTasksTask = this.GetScheduledTasksAsync(pageOptions.First, pageOptions.Last, createdAfter, createdBefore, cancellationToken);
        var getHasNextPageTask = this.GetHasNextPageAsync(pageOptions.First, createdAfter, createdBefore, cancellationToken);
        var getHasPreviousPageTask = this.GetHasPreviousPageAsync(pageOptions.Last, createdAfter, createdBefore, cancellationToken);
        var totalCountTask = this.scheduledTaskRepository.GetTotalCountAsync(cancellationToken);

        await Task.WhenAll(getScheduledTasksTask, getHasNextPageTask, getHasPreviousPageTask, totalCountTask).ConfigureAwait(false);
        var scheduledTasks = await getScheduledTasksTask.ConfigureAwait(false);
        var hasNextPage = await getHasNextPageTask.ConfigureAwait(false);
        var hasPreviousPage = await getHasPreviousPageTask.ConfigureAwait(false);
        var totalCount = await totalCountTask.ConfigureAwait(false);

        if (scheduledTasks is null)
        {
            return new NotFoundResult();
        }

        var (startCursor, endCursor) = Cursor.GetFirstAndLastCursor(scheduledTasks, x => x.Created);
        var scheduledTaskViewModels = this.scheduledTaskMapper.MapList(scheduledTasks);

        var httpContext = this.httpContextAccessor.HttpContext!;
        var connection = new PagedCollection<ScheduledTask>()
        {
            PageInfo = new PageInfo()
            {
                Count = scheduledTaskViewModels.Count,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPreviousPage,
                NextPageUrl = hasNextPage ? new Uri(this.linkGenerator.GetUriByRouteValues(
                    httpContext,
                    ScheduledTasksControllerRoute.GetScheduledTaskPage,
                    new PageOptions()
                    {
                        First = pageOptions.First ?? pageOptions.Last,
                        After = endCursor,
                    })!) : null,
                PreviousPageUrl = hasPreviousPage ? new Uri(this.linkGenerator.GetUriByRouteValues(
                    httpContext,
                    ScheduledTasksControllerRoute.GetScheduledTaskPage,
                    new PageOptions()
                    {
                        Last = pageOptions.First ?? pageOptions.Last,
                        Before = startCursor,
                    })!) : null,
                FirstPageUrl = new Uri(this.linkGenerator.GetUriByRouteValues(
                    httpContext,
                    ScheduledTasksControllerRoute.GetScheduledTaskPage,
                    new PageOptions()
                    {
                        First = pageOptions.First ?? pageOptions.Last,
                    })!),
                LastPageUrl = new Uri(this.linkGenerator.GetUriByRouteValues(
                    httpContext,
                    ScheduledTasksControllerRoute.GetScheduledTaskPage,
                    new PageOptions()
                    {
                        Last = pageOptions.First ?? pageOptions.Last,
                    })!),
            },
            TotalCount = totalCount,
        };
        connection.Items.AddRange(scheduledTaskViewModels);

        httpContext.Response.Headers.Link = connection.PageInfo.ToLinkHttpHeaderValue();
        return new OkObjectResult(connection);
    }

    private Task<List<Models.ScheduledTask>> GetScheduledTasksAsync(
        int? first,
        int? last,
        DateTimeOffset? createdAfter,
        DateTimeOffset? createdBefore,
        CancellationToken cancellationToken)
    {
        if (first.HasValue)
        {
            return this.scheduledTaskRepository.GetScheduledTasksAsync(first, createdAfter, createdBefore, cancellationToken);
        }

        return this.scheduledTaskRepository.GetScheduledTasksReverseAsync(last, createdAfter, createdBefore, cancellationToken);
    }

    private async Task<bool> GetHasNextPageAsync(
        int? first,
        DateTimeOffset? createdAfter,
        DateTimeOffset? createdBefore,
        CancellationToken cancellationToken)
    {
        if (first.HasValue)
        {
            return await this.scheduledTaskRepository
                .GetHasNextPageAsync(first, createdAfter, cancellationToken)
                .ConfigureAwait(false);
        }
        return createdBefore.HasValue;
    }

    private async Task<bool> GetHasPreviousPageAsync(
        int? last,
        DateTimeOffset? createdAfter,
        DateTimeOffset? createdBefore,
        CancellationToken cancellationToken)
    {
        if (last.HasValue)
        {
            return await this.scheduledTaskRepository
                .GetHasPreviousPageAsync(last, createdBefore, cancellationToken)
                .ConfigureAwait(false);
        }

        return createdAfter.HasValue;
    }
}
