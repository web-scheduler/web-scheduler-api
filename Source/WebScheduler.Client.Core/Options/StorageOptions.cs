namespace WebScheduler.Client.Core.Options;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// TODO
/// </summary>
public class StorageOptions
{
    /// <summary>
    /// TODO
    /// </summary>
    [Required]
    public string Invariant { get; set; } = default!;

    /// <summary>
    /// TODO
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = default!;
}
