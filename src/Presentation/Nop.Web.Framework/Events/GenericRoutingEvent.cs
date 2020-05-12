using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Seo;

namespace Nop.Web.Framework.Events
{
    /// <summary>
    /// Represents an event that occurs when a generic route is processed and no default handlers are found
    /// </summary>
    public class GenericRoutingEvent
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="values">Route values</param>
        /// <param name="urlRecord">Found URL record</param>
        public GenericRoutingEvent(RouteValueDictionary values, UrlRecord urlRecord)
        {
            RouteValues = values;
            UrlRecord = urlRecord;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets route values
        /// </summary>
        public RouteValueDictionary RouteValues { get; private set; }

        /// <summary>
        /// Gets URL record found by the route slug
        /// </summary>
        public UrlRecord UrlRecord { get; private set; }

        #endregion
    }
}