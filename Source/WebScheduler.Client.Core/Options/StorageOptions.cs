namespace WebScheduler.Client.Core.Options;

using System.ComponentModel.DataAnnotations;

public class StorageOptions
{
    [Required]
    public string Invariant { get; set; } = default!;

    [Required]
    public string ConnectionString { get; set; } = default!;
}
