using Nop.Core;
using Nop.Core.Domain.Seo;

namespace Nop.Web.Framework.Mvc.Routing;

/// <summary>
/// Represents a helper to build specific URLs within an application
/// </summary>
public partial interface INopUrlHelper
{
    /// <summary>
    /// Generate a generic URL for the specified entity type and route values
    /// </summary>
    /// <typeparam name="TEntity">Entity type that supports slug</typeparam>
    /// <param name="values">An object that contains route values</param>
    /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
    /// <param name="host">The host name for the URL</param>
    /// <param name="fragment">The fragment for the URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated URL
    /// </returns>
    Task<string> RouteGenericUrlAsync<TEntity>(object values = null, string protocol = null, string host = null, string fragment = null)
        where TEntity : BaseEntity, ISlugSupported;

    /// <summary>
    /// Generate a URL for topic by the specified system name
    /// </summary>
    /// <param name="systemName">Topic system name</param>
    /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
    /// <param name="host">The host name for the URL</param>
    /// <param name="fragment">The fragment for the URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated URL
    /// </returns>
    Task<string> RouteTopicUrlAsync(string systemName, string protocol = null, string host = null, string fragment = null);
}