namespace WebScheduler.Client.Core.Options;
using System.ComponentModel.DataAnnotations;
using Orleans.Configuration;

/// <summary>
/// All options for the application.
/// </summary>
public class WebSchedulerClientConfigurationOptions
{
    [Required]
    public ClusterOptions Cluster { get; set; } = default!;

    [Required]
    public StorageOptions Storage { get; set; } = default!;
}