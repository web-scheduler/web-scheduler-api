namespace WebScheduler.Client.Http.Models.ViewModels;

/// <summary>
/// A paged collection of items.
/// </summary>
/// <typeparam name="T">The type of the items in the list.</typeparam>
public class PageResults<T>
{
    public PageResults() => this.Items = new List<T>();
    public PageResults(List<T> items) => this.Items = items;

    /// <summary>
    /// Gets or sets the total count of items.
    /// </summary>
    /// <example>100</example>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets the items.
    /// </summary>
    public List<T> Items { get; set; }
}
