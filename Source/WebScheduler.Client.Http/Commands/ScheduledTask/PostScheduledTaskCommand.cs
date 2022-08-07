namespace WebScheduler.Api.Commands.ScheduledTask;

using Boxed.Mapping;
using Microsoft.AspNetCore.Mvc;
using WebScheduler.Client.Http.Models.ViewModels;
using WebScheduler.Abstractions.Grains.Scheduler;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using WebScheduler.Client.Core.Repositories;
using WebScheduler.Client.Http.Constants;

public class PostScheduledTaskCommand
{
    private readonly IScheduledTaskRepository scheduledTaskRepository;
    private readonly IMapper<Client.Core.Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper;
    private readonly IMapper<SaveScheduledTask, Client.Core.Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper;
    private readonly ILogger<PostScheduledTaskCommand> logger;
    private static readonly Random RandomNumber = new();

    public PostScheduledTaskCommand(
        IScheduledTaskRepository scheduledTaskRepository,
        IMapper<Client.Core.Models.ScheduledTask, ScheduledTask> scheduledTaskToScheduledTaskMapper,
        IMapper<SaveScheduledTask, Client.Core.Models.ScheduledTask> saveScheduledTaskToScheduledTaskMapper,
        ILogger<PostScheduledTaskCommand> logger)
    {
        this.scheduledTaskRepository = scheduledTaskRepository;
        this.scheduledTaskToScheduledTaskMapper = scheduledTaskToScheduledTaskMapper;
        this.saveScheduledTaskToScheduledTaskMapper = saveScheduledTaskToScheduledTaskMapper;
        this.logger = logger;
    }

    public async Task<IActionResult> ExecuteAsync(SaveScheduledTask saveScheduledTask, CancellationToken cancellationToken)
    {
        var scheduledTask = this.saveScheduledTaskToScheduledTaskMapper.Map(saveScheduledTask);

        if (scheduledTask.ScheduledTaskId == Guid.Empty)
        {
            scheduledTask.ScheduledTaskId = Guid.NewGuid();
        }

        // Append a seconds to stagger the task times
        scheduledTask.CronExpression = $"{RandomNumber.Next(0, 59)} {scheduledTask.CronExpression}";

        try
        {
            scheduledTask = await this.scheduledTaskRepository.AddAsync(scheduledTask, cancellationToken).ConfigureAwait(true);
            var scheduledTaskViewModel = this.scheduledTaskToScheduledTaskMapper.Map(scheduledTask);
            return new CreatedAtRouteResult(ScheduledTasksControllerRoute.GetScheduledTask, new
            {
                scheduledTaskViewModel.ScheduledTaskId
            }, scheduledTaskViewModel);
        }
        catch (ScheduledTaskAlreadyExistsException scheduledTaskAlreadyExistsException)
        {
            return new ConflictObjectResult(scheduledTaskAlreadyExistsException.Message);
        }
        catch (Exception ex)
        {
            this.logger.InternalServerErrorWhileCreatingScheduledTask(ex);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

/// <summary>
/// <see cref="ILogger"/> extension methods. Helps log messages using strongly typing and source generators.
/// </summary>
internal static partial class LoggerExtensions
{
    [LoggerMessage(
        EventId = 5009,
        Level = LogLevel.Error,
        Message = "Failed to create new scheduled task.")]
    public static partial void InternalServerErrorWhileCreatingScheduledTask(
        this ILogger logger,
        Exception exception);
}
