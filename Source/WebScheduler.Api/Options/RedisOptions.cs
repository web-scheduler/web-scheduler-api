namespace WebScheduler.Api.Options;

using System.ComponentModel.DataAnnotations;
using StackExchange.Redis;

/// <summary>
/// TODO
/// </summary>
public class RedisOptions
{
    /// <summary>
    /// TODO
    /// </summary>
    [Required]
    public string? ConnectionString { get; set; }

    /// <summary>
    /// TODO
    /// </summary>
    public ConfigurationOptions ConfigurationOptions
    {
        get
        {
            var options = ConfigurationOptions.Parse(this.ConnectionString);
            options.ClientName = AssemblyInformation.Current.Product;
            return options;
        }
    }
}
