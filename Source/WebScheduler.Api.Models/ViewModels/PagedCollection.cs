namespace WebScheduler.Api.ViewModels;

/// <summary>
/// A paged collection of items.
/// </summary>
/// <typeparam name="T">The type of the items in the list.</typeparam>
public class PagedCollection<T>
{
    public PagedCollection() => this.Items = new List<T>();
    public PagedCollection(List<T> items) => this.Items = items;

    /// <summary>
    /// Gets or sets the total count of items.
    /// </summary>
    /// <example>100</example>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the page information.
    /// </summary>
    public PageInfo PageInfo { get; set; } = default!;

    /// <summary>
    /// Gets the items.
    /// </summary>
    public List<T> Items { get; set; }
}
