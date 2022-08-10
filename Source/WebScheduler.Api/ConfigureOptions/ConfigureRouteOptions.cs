namespace WebScheduler.Api.ConfigureOptions;

using Microsoft.Extensions.Options;

/// <summary>
/// Configures custom routing settings which determines how URL's are generated.
/// </summary>
public class ConfigureRouteOptions : IConfigureOptions<RouteOptions>
{
    /// <summary>
    /// Invoked to configure a  instance.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    public void Configure(RouteOptions options) => options.LowercaseUrls = true;
}
