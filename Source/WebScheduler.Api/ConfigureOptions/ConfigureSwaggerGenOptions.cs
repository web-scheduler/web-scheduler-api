#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace WebScheduler.Api;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using WebScheduler.Api.OperationFilters;
using Boxed.AspNetCore.Swagger;
using Boxed.AspNetCore.Swagger.OperationFilters;
using Boxed.AspNetCore.Swagger.SchemaFilters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

/// <summary>
/// TODO
/// </summary>
public class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="provider"></param>
    public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider) =>
        this.provider = provider;

    /// <summary>
    /// Invoked to configure a instance.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    public void Configure(SwaggerGenOptions options)
    {
        options.DescribeAllParametersInCamelCase();
        options.EnableAnnotations();

        // Add the XML comment file for this assembly, so its contents can be displayed.
        _ = options.IncludeXmlCommentsIfExists(typeof(Startup).Assembly);

        options.OperationFilter<ApiVersionOperationFilter>();
        options.OperationFilter<ClaimsOperationFilter>();
        options.OperationFilter<ForbiddenResponseOperationFilter>();
        options.OperationFilter<ProblemDetailsOperationFilter>();
        options.OperationFilter<UnauthorizedResponseOperationFilter>();

        // Show a default and example model for JsonPatchDocument<T>.
        options.SchemaFilter<JsonPatchDocumentSchemaFilter>();
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()

                    }
        });
        foreach (var apiVersionDescription in this.provider.ApiVersionDescriptions)
        {
            var info = new OpenApiInfo()
            {
                Title = AssemblyInformation.Current.Product,
                Description = apiVersionDescription.IsDeprecated ?
                    $"{AssemblyInformation.Current.Description} This API version has been deprecated." :
                    AssemblyInformation.Current.Description,
                Version = apiVersionDescription.ApiVersion.ToString(),
            };
            options.SwaggerDoc(apiVersionDescription.GroupName, info);
        }
    }
}
