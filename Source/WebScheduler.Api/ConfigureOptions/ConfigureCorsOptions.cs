namespace WebScheduler.Api.ConfigureOptions;

using WebScheduler.Api.Constants;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

/// <summary>
/// Configures cross-origin resource sharing (CORS) policies.
/// See https://docs.asp.net/en/latest/security/cors.html.
/// </summary>
public class ConfigureCorsOptions : IConfigureOptions<CorsOptions>
{
    /// <summary>
    ///  The ctor.
    /// </summary>
    public ConfigureCorsOptions() { }
    /// <summary>
    /// Invoked to configure a  instance.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    public void Configure(CorsOptions options) =>
        // Create named CORS policies here which you can consume using application.UseCors("PolicyName")
        // or a [EnableCors("PolicyName")] attribute on your controller or action.
        options.AddPolicy(
            CorsPolicyName.AllowAny,
            x => x
                .SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
}
