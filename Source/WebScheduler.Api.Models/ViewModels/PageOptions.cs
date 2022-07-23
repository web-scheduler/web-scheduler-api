namespace WebScheduler.Api.Models.ViewModels;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// The options used to request a page.
/// </summary>
public class PageOptions
{
    /// <summary>
    /// Gets or sets the number of items to retrieve from the page
    /// </summary>
    /// <example>10</example>
    [Range(1, 1000)]
    public int? PageSize { get; set; }

    /// <summary>
    /// Gets or sets the number of items to skip from the begining of the list.
    /// </summary>
    /// <example>10</example>
    [Range(0, int.MaxValue)]
    public int Offset { get; set; }
}
