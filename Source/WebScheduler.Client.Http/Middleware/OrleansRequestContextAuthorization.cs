namespace WebScheduler.Client.Http.Middleware;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Orleans.Runtime;
using WebScheduler.Abstractions.Constants;

/// <summary>
/// Injects the TenantId of the current user into the Orleans <see cref="RequestContext"/>.
/// </summary>
public class OrleansRequestContextAuthorization
{
    private readonly RequestDelegate next;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="next">next middleware</param>
    public OrleansRequestContextAuthorization(RequestDelegate next) => this.next = next;

    /// <summary>
    /// The invoke
    /// </summary>
    /// <param name="context">http context</param>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated ?? false)
        {
            RequestContext.Set(RequestContextKeys.TenantId, context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        }
        // Call the next delegate/middleware in the pipeline.
        await this.next(context);
    }
}
