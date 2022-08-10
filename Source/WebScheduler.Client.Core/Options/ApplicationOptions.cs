namespace WebScheduler.Client.Core.Options;
using System.ComponentModel.DataAnnotations;
using Orleans.Configuration;

/// <summary>
/// All options for the application.
/// </summary>
public class WebSchedulerClientConfigurationOptions
{
    /// <summary>
    /// 
    /// </summary>
    [Required]
    public ClusterOptions Cluster { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [Required]
    public StorageOptions Storage { get; set; } = default!;
}
