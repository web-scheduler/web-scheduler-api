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
    public void Configure(CorsOptions options) =>
        // Create named CORS policies here which you can consume using application.UseCors("PolicyName")
        // or a [EnableCors("PolicyName")] attribute on your controller or action.
        options.AddPolicy(
            CorsPolicyName.AllowAny,
            x => x
                .WithOrigins("https://localhost:7099")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
}
