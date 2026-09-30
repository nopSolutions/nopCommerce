using System.Collections.Generic;

namespace Nop.Plugin.Api.Rest.Models;

/// <summary>
/// A single page of results together with the metadata a client needs to reach the remaining pages
/// </summary>
/// <typeparam name="T">Type of the items in the page</typeparam>
/// <remarks>
/// A bare array cannot tell a client how many records matched, so it has to fetch every page to
/// discover that it reached the end. This envelope carries that metadata explicitly.
/// </remarks>
public record PagedResult<T>
{
    /// <summary>
    /// Gets or sets the items of the requested page
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the zero based index of the returned page
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of records matching the filters, across every page
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages
    /// </summary>
    public int TotalPages { get; set; }
}
