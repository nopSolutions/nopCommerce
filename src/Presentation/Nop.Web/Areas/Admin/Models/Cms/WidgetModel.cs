using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Cms
{
    public partial class WidgetModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.ContentManagement.Widgets.Fields.FriendlyName")]
        public string FriendlyName { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Widgets.Fields.SystemName")]
        public string SystemName { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Widgets.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Widgets.Fields.IsActive")]
        public bool IsActive { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Widgets.Configure")]
        public string ConfigurationUrl { get; set; }

        public string WidgetViewComponentName { get; set; }
        public RouteValueDictionary WidgetViewComponentArguments { get; set; }
    }
}