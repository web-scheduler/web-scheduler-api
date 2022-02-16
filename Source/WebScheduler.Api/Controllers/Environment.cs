namespace WebScheduler.Api.Controllers;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;

public class EnvironmentController : Controller
{
    private readonly IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;

    public EnvironmentController(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider) => this.actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;

    [HttpGet("routes", Name = "ApiEnvironmentGetAllRoutes")]
    public IActionResult GetAllRoutes()
    {
        /* intentional use of var/anonymous class since this method is purely informational */
        var routes = this.actionDescriptorCollectionProvider.ActionDescriptors.Items
            .Where(ad => ad.AttributeRouteInfo != null)
            .Select(x => new
            {
                Action = x?.RouteValues?["action"] != null ? x.RouteValues["action"] : "n/a",
                Controller = x?.RouteValues?["controller"] != null ? x.RouteValues["controller"] : "n/a",
                Name = x?.AttributeRouteInfo?.Name ?? "n/a",
                Template = x?.AttributeRouteInfo?.Template ?? "n/a",
                Method = x?.ActionConstraints?.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods.First()
            }).ToList();
        return this.Ok(routes);
    }
}
