using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Seo;

namespace Nop.Web.Framework.Seo
{
    /// <summary>
    /// Represents event to handle unknown URL record entity names
    /// </summary>
    public class CustomUrlRecordEntityNameRequestedEvent
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="routeData">Route data</param>
        /// <param name="urlRecord">URL record</param>
        public CustomUrlRecordEntityNameRequestedEvent(RouteData routeData, UrlRecord urlRecord)
        {
            RouteData = routeData;
            UrlRecord = urlRecord;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets information about the current routing path
        /// </summary>
        public RouteData RouteData { get; private set; }

        /// <summary>
        /// Gets or sets URL record
        /// </summary>
        public UrlRecord UrlRecord { get; private set; }

        #endregion

    }
}