namespace WebScheduler.Api.ConfigureOptions;

using Microsoft.AspNetCore.Hosting;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using WebScheduler.Api.ViewModels;

/// <summary>
/// Configures json
/// </summary>
public class ConfigureJsonOptions : IConfigureOptions<JsonOptions>
{
    private readonly IWebHostEnvironment webHostEnvironment;

    /// <summary>
    /// todo
    /// </summary>
    /// <param name="webHostEnvironment"></param>
    public ConfigureJsonOptions(IWebHostEnvironment webHostEnvironment) =>
        this.webHostEnvironment = webHostEnvironment;

    /// <summary>
    /// Invoked to configure an instance.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    public void Configure(JsonOptions options)
    {
        var jsonSerializerOptions = options.JsonSerializerOptions;
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        jsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;

        // Pretty print the JSON in development for easier debugging.
        jsonSerializerOptions.WriteIndented = this.webHostEnvironment.IsDevelopment() ||
            this.webHostEnvironment.IsEnvironment(Constants.EnvironmentName.Test);

        jsonSerializerOptions.AddContext<CustomJsonSerializerContext>();
    }
}
