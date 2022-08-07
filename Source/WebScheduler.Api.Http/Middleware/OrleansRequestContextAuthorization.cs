namespace WebScheduler.Api.Http.Middleware;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Orleans.Runtime;
using WebScheduler.Abstractions.Constants;

public class OrleansRequestContextAuthorization
{
    private readonly RequestDelegate next;

    public OrleansRequestContextAuthorization(RequestDelegate next) => this.next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated ?? false)
        {
            RequestContext.Set(RequestContextKeys.TenantId, context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        }
        // Call the next delegate/middleware in the pipeline.
        await this.next(context).ConfigureAwait(true);
    }
}
