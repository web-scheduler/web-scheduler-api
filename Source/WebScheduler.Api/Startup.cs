namespace WebScheduler.Api;

using Serilog;
using WebScheduler.Api.Constants;
using Boxed.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebScheduler.ConfigureOptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


/// <summary>
/// The main start-up class for the application.
/// </summary>
public class Startup
{
    private readonly IConfiguration configuration;
    private readonly IWebHostEnvironment webHostEnvironment;

    /// <summary>
    /// Initializes a new instance of the <see cref="Startup"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration, where key value pair settings are stored. See
    /// http://docs.asp.net/en/latest/fundamentals/configuration.html</param>
    /// <param name="webHostEnvironment">The environment the application is running under. This can be Development,
    /// Staging or Production by default. See http://docs.asp.net/en/latest/fundamentals/environments.html</param>
    public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        this.configuration = configuration;
        this.webHostEnvironment = webHostEnvironment;
    }

    /// <summary>
    /// Configures the services to add to the ASP.NET Core Injection of Control (IoC) container. This method gets
    /// called by the ASP.NET runtime. See
    /// http://blogs.msdn.com/b/webdev/archive/2014/06/17/dependency-injection-in-asp-net-vnext.aspx
    /// </summary>
    /// <param name="services">The services.</param>
    public virtual void ConfigureServices(IServiceCollection services) =>
        services
            .ConfigureOptions<ConfigureRequestLoggingOptions>()
            .AddStackExchangeRedisCache(options => { })
            .AddCors()
            .AddResponseCompression()
            .AddRouting()
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.MetadataAddress = "https://accounts.google.com/.well-known/openid-configuration";
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = this.configuration["Authentication:Google:ClientId"],
                    ValidIssuer = "https://accounts.google.com",

                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,// It forces tokens to expire exactly at token expiration time instead of 5 minutes later
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["Authentication:Google:ClientSecret"]))
                };
            }).Services
            .AddResponseCaching()
            .AddCustomHealthChecks(this.webHostEnvironment, this.configuration)
            .AddCustomOpenTelemetryTracing(this.webHostEnvironment)
            .AddSwaggerGen()
            .AddHttpContextAccessor()
            // Add useful interface for accessing the ActionContext outside a controller.
            .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
            .AddApiVersioning()
            .AddVersionedApiExplorer()
            .AddServerTiming()
            .AddControllers()
            .Services
            .AddCustomOptions(this.configuration)
            .AddCustomConfigureOptions()
            .AddProjectCommands()
            .AddProjectMappers()
            .AddProjectRepositories()
            .AddProjectServices()
            .AddHostedServices();

    /// <summary>
    /// Configures the application and HTTP request pipeline. Configure is called after ConfigureServices is
    /// called by the ASP.NET runtime.
    /// </summary>
    /// <param name="application">The application builder.</param>
    public virtual void Configure(IApplicationBuilder application) =>
        application
            .UseSerilogRequestLogging()
            .UseIf(
                this.webHostEnvironment.IsDevelopment(),
                x => x.UseServerTiming())
            .UseForwardedHeaders()
            .UseRouting()
            .UseCors(CorsPolicyName.AllowAny)
            .UseAuthentication()
            .UseAuthorization()
            .UseResponseCaching()
            .UseResponseCompression()
            .UseIf(
                this.webHostEnvironment.IsDevelopment(),
                x => x.UseDeveloperExceptionPage())
            .UseStaticFiles()
            .UseEndpoints(
                builder =>
                {
                    builder.MapControllers().RequireCors(CorsPolicyName.AllowAny);
                    builder
                        .MapHealthChecks("/status")
                        .RequireCors(CorsPolicyName.AllowAny);
                    builder
                        .MapHealthChecks("/status/self", new HealthCheckOptions() { Predicate = _ => false })
                        .RequireCors(CorsPolicyName.AllowAny);
                })
            .UseSwagger()
            .UseIf(
                this.webHostEnvironment.IsDevelopment(),
                x => x.UseSwaggerUI());
}
