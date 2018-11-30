using Microsoft.AspNetCore.Routing;

namespace Nop.Web.Framework.DataTables
{
    public partial class DataUrl
    {
        #region Ctor

        public DataUrl(string actionName, string controllerName, RouteValueDictionary routeValues = null)
        {
            this.ActionName = actionName;
            this.ControllerName = controllerName;
            this.RouteValues = routeValues;
        }

        public DataUrl(string url)
        {
            this.Url = url;
        }

        public DataUrl(string url, string dataId)
        {
            this.Url = url;
            this.DataId = dataId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the action.
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Gets or sets the name of the controller.
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the route values.
        /// </summary>
        public RouteValueDictionary RouteValues { get; set; }

        public string DataId { get; set; }

        #endregion
    }
}
