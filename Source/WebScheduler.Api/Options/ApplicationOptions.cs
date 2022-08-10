namespace WebScheduler.Api.Options;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Server.Kestrel.Core;

/// <summary>
/// All options for the application.
/// </summary>
public class ApplicationOptions
{
    /// <summary>
    /// TODO
    /// </summary>
    public ApplicationOptions() => this.CacheProfiles = new CacheProfileOptions();

    /// <summary>
    /// TODO
    /// </summary>
    [Required]
    public CacheProfileOptions CacheProfiles { get; }

    /// <summary>
    /// TODO
    /// </summary>
    [Required]
    public CompressionOptions Compression { get; set; } = default!;

    /// <summary>
    /// TODO
    /// </summary>
    [Required]
    public ForwardedHeadersOptions ForwardedHeaders { get; set; } = default!;

    /// <summary>
    /// TODO
    /// </summary>
    [Required]
    public HostOptions Host { get; set; } = default!;

    /// <summary>
    /// TODO
    /// </summary>
    [Required]
    public KestrelServerOptions Kestrel { get; set; } = default!;

    /// <summary>
    /// TODO
    /// </summary>
    [Required]
    public RedisOptions Redis { get; set; } = default!;
}
