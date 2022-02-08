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
    private readonly IConfiguration configuration;

    public ConfigureCorsOptions(IConfiguration configuration) => this.configuration = configuration;
    public void Configure(CorsOptions options) =>
        // Create named CORS policies here which you can consume using application.UseCors("PolicyName")
        // or a [EnableCors("PolicyName")] attribute on your controller or action.
        options.AddPolicy(
            CorsPolicyName.AllowAny,
            x => x
                .SetIsOriginAllowed(c => c == this.configuration["Cors:Origin"])
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
}
