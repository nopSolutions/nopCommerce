using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Cms
{
    public class EditWidgetModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.ContentManagement.Widgets.Fields.WidgetZone")]
        public int WidgetZoneId { get; set; }
        public SelectList SupportedWidgetZones { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Widgets.Fields.PluginFriendlyName")]
        public string PluginFriendlyName { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Widgets.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }


        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}