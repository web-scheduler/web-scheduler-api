namespace WebScheduler.Api;

using Serilog;
using WebScheduler.Api.Constants;
using Boxed.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebScheduler.ConfigureOptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebScheduler.Api.Policies;
using IdentityServerHost.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using WebScheduler.Client.Http;

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
    public virtual void ConfigureServices(IServiceCollection services)
    {
        _ = services.AddDbContext<DataProtectionKeysDbContext>(b =>
          {
              _ = b.UseMySql(this.configuration.GetConnectionString("DataProtectionConnectionString"), ServerVersion.AutoDetect(this.configuration.GetConnectionString("DataProtectionConnectionString")),
                  dbOpts => dbOpts.MigrationsAssembly(typeof(Program).Assembly.FullName)
                      .EnableRetryOnFailure());
              if (this.webHostEnvironment.IsDevelopment())
              {
                  _ = b.LogTo(Console.WriteLine, LogLevel.Information)
                  .EnableSensitiveDataLogging()
                  .EnableDetailedErrors();
              }
          })
            .AddDataProtection().PersistKeysToDbContext<DataProtectionKeysDbContext>();

        _ = services
            .ConfigureOptions<ConfigureRequestLoggingOptions>()
            .AddStackExchangeRedisCache(_ => { })
            .AddCors()
            .AddResponseCompression()
            .AddRouting();

        _ = services
       .AddAuthentication(option =>
       {
           option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
           option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
       }).AddJwtBearer(options =>
            {
                options.Authority = this.configuration["Identity:Authority"];
                //options.Audience = this.configuration["Identity:ResourceId"];
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.MetadataAddress = new Uri(new Uri(this.configuration["Identity:Authority"]), "/.well-known/openid-configuration").ToString();
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    //ValidateAudience = true,
                    ValidAudience = this.configuration["Identity:Resource"],
                    ValidIssuer = this.configuration["Identity:Authority"],

                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,// It forces tokens to expire exactly at token expiration time instead of 5 minutes later
                    NameClaimType = "sub",
                    RoleClaimType = "role"
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = async (_) => await Task.FromResult(string.Empty).ConfigureAwait(true)
                };
            });

        _ = services.AddAuthorization(options => AccessPolicyAttribute.AddPolicy(options));

        _ = services.AddResponseCaching()
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
        .AddWebSchedulerHttpClient(this.configuration);
    }

    /// <summary>
    /// Configures the application and HTTP request pipeline. Configure is called after ConfigureServices is
    /// called by the ASP.NET runtime.
    /// </summary>
    /// <param name="application">The application builder.</param>
    public virtual void Configure(IApplicationBuilder application)
    {
        var app = application
            .UseSerilogRequestLogging()
            .UseIf(
                this.webHostEnvironment.IsDevelopment(),
                x => x.UseServerTiming())
            .UseForwardedHeaders()
            .UseAuthentication()
                    .UseRouting()
                    .UseCors(CorsPolicyName.AllowAny)

            .UseAuthorization()
            .UseWebScheduler()
            .UseResponseCaching()
            .UseResponseCompression()
            .UseIf(
            this.webHostEnvironment.IsDevelopment(),
            x => x.UseDeveloperExceptionPage())
            .UseStaticFiles()
            .UseEndpoints(
            builder =>
            {
                _ = builder.MapControllers()
                .RequireCors(CorsPolicyName.AllowAny)
                .RequireAuthorization();
                _ = builder
                    .MapHealthChecks("/status")
                    .RequireCors(CorsPolicyName.AllowAny);
                _ = builder
                    .MapHealthChecks("/status/self", new HealthCheckOptions() { Predicate = _ => false })
                    .RequireCors(CorsPolicyName.AllowAny);
            })
            .UseSwagger()
            .UseIf(
            this.webHostEnvironment.IsDevelopment(),
            x => x.UseSwaggerUI());

        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DataProtectionKeysDbContext>();
        db.Database.Migrate();
    }
}
