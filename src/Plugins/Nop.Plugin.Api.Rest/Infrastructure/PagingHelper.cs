using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Plugin.Api.Rest.Models;

namespace Nop.Plugin.Api.Rest.Infrastructure;

/// <summary>
/// Normalizes paging input and wraps results in the API response envelope
/// </summary>
public static class PagingHelper
{
    /// <summary>
    /// The page size applied when the caller does not ask for a specific one
    /// </summary>
    public const int DEFAULT_PAGE_SIZE = 20;

    /// <summary>
    /// The largest page a caller may ask for
    /// </summary>
    /// <remarks>
    /// The underlying services default to int.MaxValue, which would let a single request materialize
    /// the whole table
    /// </remarks>
    public const int MAX_PAGE_SIZE = 200;

    /// <summary>
    /// Clamps a requested page index to a usable value
    /// </summary>
    /// <param name="pageIndex">Requested zero based page index</param>
    /// <returns>The page index to query with</returns>
    public static int NormalizePageIndex(int pageIndex)
        => pageIndex < 0 ? 0 : pageIndex;

    /// <summary>
    /// Clamps a requested page size into the supported range
    /// </summary>
    /// <param name="pageSize">Requested page size</param>
    /// <returns>The page size to query with</returns>
    public static int NormalizePageSize(int pageSize)
        => pageSize <= 0 ? DEFAULT_PAGE_SIZE : Math.Min(pageSize, MAX_PAGE_SIZE);

    /// <summary>
    /// Wraps a list that the database already paged
    /// </summary>
    /// <typeparam name="T">Type of the items</typeparam>
    /// <param name="source">Paged list returned by a service</param>
    /// <returns>The response envelope</returns>
    public static PagedResult<T> ToPagedResult<T>(this IPagedList<T> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return new PagedResult<T>
        {
            Items = source.ToList(),
            PageIndex = source.PageIndex,
            PageSize = source.PageSize,
            TotalCount = source.TotalCount,
            TotalPages = source.TotalPages
        };
    }

    /// <summary>
    /// Projects the items of an envelope, keeping the paging metadata untouched
    /// </summary>
    /// <typeparam name="TSource">Type of the source items</typeparam>
    /// <typeparam name="TTarget">Type of the projected items</typeparam>
    /// <param name="source">Envelope to project</param>
    /// <param name="selector">Projection applied to every item</param>
    /// <returns>The projected envelope</returns>
    /// <remarks>
    /// Used to page against the database and then project, so that the totals come from the query
    /// rather than from the size of the returned page
    /// </remarks>
    public static PagedResult<TTarget> Map<TSource, TTarget>(this PagedResult<TSource> source,
        Func<TSource, TTarget> selector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        return new PagedResult<TTarget>
        {
            Items = source.Items.Select(selector).ToList(),
            PageIndex = source.PageIndex,
            PageSize = source.PageSize,
            TotalCount = source.TotalCount,
            TotalPages = source.TotalPages
        };
    }

    /// <summary>
    /// Wraps a list that was materialized in memory and has to be paged by hand
    /// </summary>
    /// <typeparam name="T">Type of the items</typeparam>
    /// <param name="all">Every matching item</param>
    /// <param name="pageIndex">Zero based page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>The response envelope</returns>
    public static PagedResult<T> ToInMemoryPagedResult<T>(IList<T> all, int pageIndex, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(all);

        var totalCount = all.Count;

        return new PagedResult<T>
        {
            Items = all.Skip(pageIndex * pageSize).Take(pageSize).ToList(),
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = pageSize <= 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}
