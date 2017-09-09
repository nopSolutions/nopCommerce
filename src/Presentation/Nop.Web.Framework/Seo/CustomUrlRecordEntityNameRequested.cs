using Microsoft.AspNetCore.Routing;
using Nop.Services.Seo;

namespace Nop.Web.Framework.Seo
{
    /// <summary>
    /// Represents event to handle unknow URL record entity names
    /// </summary>
    public class CustomUrlRecordEntityNameRequested
    {
        #region Properties

        /// <summary>
        /// Gets or sets information about the current routing path
        /// </summary>
        public RouteData RouteData { get; private set; }

        /// <summary>
        /// Gets or sets URL record
        /// </summary>
        public UrlRecordService.UrlRecordForCaching UrlRecord { get; private set; }

        #endregion

        #region Ctor

        public CustomUrlRecordEntityNameRequested(RouteData routeData, UrlRecordService.UrlRecordForCaching urlRecord)
        {
            RouteData = routeData;
            UrlRecord = urlRecord;
        }

        #endregion
    }
}