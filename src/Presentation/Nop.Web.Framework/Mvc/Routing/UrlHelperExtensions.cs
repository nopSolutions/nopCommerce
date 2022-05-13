using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Seo;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// Represents url helper extension methods
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Generate a generic URL for the specified entity type and route values
        /// </summary>
        /// <typeparam name="TEntity">Entity type that supports slug</typeparam>
        /// <param name="urlHelper">URL helper</param>
        /// <param name="values">An object that contains route values</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
        /// <param name="host">The host name for the URL</param>
        /// <param name="fragment">The fragment for the URL</param>
        /// <returns>The generated URL</returns>
        public static string RouteUrl<TEntity>(this IUrlHelper urlHelper, object values = null, string protocol = null, string host = null, string fragment = null)
            where TEntity : BaseEntity, ISlugSupported
        {
            var nopUrlHelper = EngineContext.Current.Resolve<INopUrlHelper>();
            return nopUrlHelper.RouteGenericUrlAsync<TEntity>(values, protocol, host, fragment).Result;
        }

        /// <summary>
        /// Generate a URL for topic by the specified system name
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        /// <param name="systemName">Topic system name</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
        /// <param name="host">The host name for the URL</param>
        /// <param name="fragment">The fragment for the URL</param>
        /// <returns>The generated URL</returns>
        public static string RouteTopicUrl(this IUrlHelper urlHelper, string systemName, string protocol = null, string host = null, string fragment = null)
        {
            var nopUrlHelper = EngineContext.Current.Resolve<INopUrlHelper>();
            return nopUrlHelper.RouteTopicUrlAsync(systemName, protocol, host, fragment).Result;
        }
    }
}