namespace WebScheduler.Api.Controllers;
using WebScheduler.Api.Constants;
using WebScheduler.Api.ViewModels;
using Boxed.AspNetCore;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebScheduler.Api.Commands.ScheduledTask;
using WebScheduler.Abstractions.Grains.Scheduler;
using Microsoft.AspNetCore.Authorization;

[Route("[controller]")]
[ApiController]
[ApiVersion(ApiVersionName.V1)]
[SwaggerResponse(
    StatusCodes.Status500InternalServerError,
    "The MIME type in the Accept HTTP header is not acceptable.",
    typeof(ProblemDetails),
    ContentType.ProblemJson)]
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CA1062 // Validate arguments of public methods
public class ScheduledTasksController : ControllerBase
{
    //    /// <summary>
    //    /// Returns an Allow HTTP header with the allowed HTTP methods.
    //    /// </summary>
    //    /// <returns>A 200 OK response.</returns>
    //    [HttpOptions(Name = ScheduledTasksControllerRoute.OptionsScheduledTasks)]
    //    [SwaggerResponse(StatusCodes.Status200OK, "The allowed HTTP methods.")]
    //    public IActionResult Options()
    //    {
    //        this.HttpContext.Response.Headers.AppendCommaSeparatedValues(
    //            HeaderNames.Allow,
    //            HttpMethods.Get,
    //            HttpMethods.Head,
    //            HttpMethods.Options,
    //            HttpMethods.Post);
    //        return this.Ok();
    //    }

    //    /// <summary>
    //    /// Returns an Allow HTTP header with the allowed HTTP methods for a ScheduledTask with the specified unique identifier.
    //    /// </summary>
    //    /// <param name="scheduledTaskId">The ScheduledTasks unique identifier.</param>
    //    /// <returns>A 200 OK response.</returns>
    //    [HttpOptions("{ScheduledTaskId}", Name = ScheduledTasksControllerRoute.OptionsScheduledTask)]
    //    [SwaggerResponse(StatusCodes.Status200OK, "The allowed HTTP methods.")]
    //#pragma warning disable IDE0060, CA1801, RCS1163 // Remove unused parameter
    //    public IActionResult Options(Guid scheduledTaskId)
    //#pragma warning restore IDE0060, CA1801, RCS1163 // Remove unused parameter
    //    {
    //        this.HttpContext.Response.Headers.AppendCommaSeparatedValues(
    //            HeaderNames.Allow,
    //            HttpMethods.Delete,
    //            HttpMethods.Get,
    //            HttpMethods.Head,
    //            HttpMethods.Options,
    //            HttpMethods.Patch,
    //            HttpMethods.Post,
    //            HttpMethods.Put);
    //        return this.Ok();
    //    }

    /// <summary>
    /// Deletes the ScheduledTask with the specified unique identifier.
    /// </summary>
    /// <param name="command">The action command.</param>
    /// <param name="scheduledTaskId">The ScheduledTasks unique identifier.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the HTTP request.</param>
    /// <returns>A 204 No Content response if the ScheduledTask was deleted or a 404 Not Found if a ScheduledTask with the specified
    /// unique identifier was not found.</returns>
    [HttpDelete("{ScheduledTaskId}", Name = ScheduledTasksControllerRoute.DeleteScheduledTask)]
    [SwaggerResponse(StatusCodes.Status410Gone, "The ScheduledTask with the specified unique identifier was deleted.")]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "A ScheduledTask with the specified unique identifier was not found.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    public Task<IActionResult> DeleteAsync(
        [FromServices] DeleteScheduledTaskCommand command,
        Guid scheduledTaskId,
        CancellationToken cancellationToken) => command.ExecuteAsync(scheduledTaskId, cancellationToken);

    /// <summary>
    /// Gets the ScheduledTask with the specified unique identifier.
    /// </summary>
    /// <param name="command">The action command.</param>
    /// <param name="scheduledTaskId">The ScheduledTasks unique identifier.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the HTTP request.</param>
    /// <returns>A 200 OK response containing the ScheduledTask or a 404 Not Found if a ScheduledTask with the specified unique
    /// identifier was not found.</returns>
    [HttpGet("{ScheduledTaskId}", Name = ScheduledTasksControllerRoute.GetScheduledTask)]
    [HttpHead("{ScheduledTaskId}", Name = ScheduledTasksControllerRoute.HeadScheduledTask)]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "The ScheduledTask with the specified unique identifier.",
        typeof(ScheduledTask),
        ContentType.RestfulJson,
        ContentType.Json)]
    [SwaggerResponse(
        StatusCodes.Status304NotModified,
        "The ScheduledTask has not changed since the date given in the If-Modified-Since HTTP header.")]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "A ScheduledTask with the specified unique identifier could not be found.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status406NotAcceptable,
        "The MIME type in the Accept HTTP header is not acceptable.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    public Task<IActionResult> GetAsync(
        [FromServices] GetScheduledTaskCommand command,
        Guid scheduledTaskId,
        CancellationToken cancellationToken) => command.ExecuteAsync(scheduledTaskId, cancellationToken);

    /// <summary>
    /// Gets a collection of ScheduledTasks.
    /// </summary>
    /// <param name="command">The action command.</param>
    /// <param name="pageOptions">The page options.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the HTTP request.</param>
    /// <returns>A 200 OK response containing a collection of ScheduledTasks, a 400 Bad Request if the page request
    /// parameters are invalid or a 404 Not Found if a page with the specified page number was not found.
    /// </returns>
    [HttpGet("", Name = ScheduledTasksControllerRoute.GetScheduledTaskPage)]
    [HttpHead("", Name = ScheduledTasksControllerRoute.HeadScheduledTaskPage)]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "A collection of ScheduledTasks for the specified page.",
        typeof(PagedCollection<ScheduledTask>),
        ContentType.RestfulJson,
        ContentType.Json)]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "The page request parameters are invalid.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "A page with the specified page number was not found.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status406NotAcceptable,
        "The MIME type in the Accept HTTP header is not acceptable.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    public Task<IActionResult> GetPageAsync(
        [FromServices] GetScheduledTaskPageCommand command,
        [FromQuery] PageOptions pageOptions,
        CancellationToken cancellationToken) => command.ExecuteAsync(pageOptions, cancellationToken);

    /// <summary>
    /// Patches the ScheduledTask with the specified unique identifier.
    /// </summary>
    /// <param name="command">The action command.</param>
    /// <param name="scheduledTaskId">The ScheduledTasks unique identifier.</param>
    /// <param name="patch">The patch document. See http://jsonpatch.com.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the HTTP request.</param>
    /// <returns>A 200 OK if the ScheduledTask was patched, a 400 Bad Request if the patch was invalid or a 404 Not Found
    /// if a ScheduledTask with the specified unique identifier was not found.</returns>
    [HttpPatch("{ScheduledTaskId}", Name = ScheduledTasksControllerRoute.PatchScheduledTask)]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "The patched ScheduledTask with the specified unique identifier.",
        typeof(ScheduledTask),
        ContentType.RestfulJson,
        ContentType.Json)]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "The patch document is invalid.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "A ScheduledTask with the specified unique identifier could not be found.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status406NotAcceptable,
        "The MIME type in the Accept HTTP header is not acceptable.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status415UnsupportedMediaType,
        "The MIME type in the Content-Type HTTP header is unsupported.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    public Task<IActionResult> PatchAsync(
        [FromServices] PatchScheduledTaskCommand command,
        Guid scheduledTaskId,
        [FromBody] JsonPatchDocument<SaveScheduledTask> patch,
        CancellationToken cancellationToken) => command.ExecuteAsync(scheduledTaskId, patch, cancellationToken);

    /// <summary>
    /// Creates a new ScheduledTask.
    /// </summary>
    /// <param name="command">The action command.</param>
    /// <param name="scheduledTask">The ScheduledTask to create.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the HTTP request.</param>
    /// <returns>A 201 Created response containing the newly created ScheduledTask or a 400 Bad Request if the ScheduledTask is
    /// invalid.</returns>
    [HttpPost("", Name = ScheduledTasksControllerRoute.PostScheduledTask)]
    [SwaggerResponse(
        StatusCodes.Status201Created,
        "The ScheduledTask was created.",
        typeof(ScheduledTask),
        ContentType.RestfulJson,
        ContentType.Json)]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "The ScheduledTask is invalid.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status406NotAcceptable,
        "The MIME type in the Accept HTTP header is not acceptable.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status415UnsupportedMediaType,
        "The MIME type in the Content-Type HTTP header is unsupported.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    public Task<IActionResult> PostAsync(
        [FromServices] PostScheduledTaskCommand command,
        [FromBody] SaveScheduledTask scheduledTask,
        CancellationToken cancellationToken) => command.ExecuteAsync(scheduledTask, cancellationToken);

    /// <summary>
    /// Updates an existing ScheduledTask with the specified unique identifier.
    /// </summary>
    /// <param name="command">The action command.</param>
    /// <param name="scheduledTaskId">The ScheduledTask identifier.</param>
    /// <param name="scheduledTask">The ScheduledTask to update.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the HTTP request.</param>
    /// <returns>A 200 OK response containing the newly updated ScheduledTask, a 400 Bad Request if the ScheduledTask is invalid or a
    /// or a 404 Not Found if a ScheduledTask with the specified unique identifier was not found.</returns>
    [HttpPut("{ScheduledTaskId}", Name = ScheduledTasksControllerRoute.PutScheduledTask)]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "The ScheduledTask was updated.",
        typeof(ScheduledTaskMetadata),
        ContentType.RestfulJson,
        ContentType.Json)]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "The ScheduledTask is invalid.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "A ScheduledTask with the specified unique identifier could not be found.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status406NotAcceptable,
        "The MIME type in the Accept HTTP header is not acceptable.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status415UnsupportedMediaType,
        "The MIME type in the Content-Type HTTP header is unsupported.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    public Task<IActionResult> PutAsync(
        [FromServices] PutScheduledTaskCommand command,
        Guid scheduledTaskId,
        [FromBody] SaveScheduledTask scheduledTask,
        CancellationToken cancellationToken) => command.ExecuteAsync(scheduledTaskId, scheduledTask, cancellationToken);
}
#pragma warning restore CA1062 // Validate arguments of public methods
#pragma warning restore CA1822 // Mark members as static
