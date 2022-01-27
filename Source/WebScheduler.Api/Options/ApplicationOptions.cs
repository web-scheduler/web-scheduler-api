namespace WebScheduler.Api.Options;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Orleans.Configuration;
using WebScheduler.Server.Options;

/// <summary>
/// All options for the application.
/// </summary>
public class ApplicationOptions
{
    public ApplicationOptions() => this.CacheProfiles = new CacheProfileOptions();

    [Required]
    public CacheProfileOptions CacheProfiles { get; }

    [Required]
    public CompressionOptions Compression { get; set; } = default!;

    [Required]
    public ForwardedHeadersOptions ForwardedHeaders { get; set; } = default!;

    [Required]
    public HostOptions Host { get; set; } = default!;

    [Required]
    public KestrelServerOptions Kestrel { get; set; } = default!;

    [Required]
    public RedisOptions Redis { get; set; } = default!;

    [Required]
    public ClusterOptions Cluster { get; set; } = default!;

    [Required]
    public StorageOptions Storage { get; set; } = default!;
}
