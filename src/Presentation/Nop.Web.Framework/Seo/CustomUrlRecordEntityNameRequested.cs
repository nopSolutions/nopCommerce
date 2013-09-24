
using System.Web.Routing;

namespace Nop.Web.Framework.Seo
{
    /// <summary>
    /// Event to handle unknow URL record entity names
    /// </summary>
    public class CustomUrlRecordEntityNameRequested
    {
        public CustomUrlRecordEntityNameRequested(RouteData routeData, string urlRecordEntityName)
        {
            this.RouteData = routeData;
            this.UrlRecordEntityName = urlRecordEntityName;
        }

        public RouteData RouteData { get; private set; }
        public string UrlRecordEntityName { get; private set; }
    }
}
