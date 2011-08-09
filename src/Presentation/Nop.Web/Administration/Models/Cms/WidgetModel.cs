using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core.Domain.Cms;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Cms
{
    public class WidgetModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.ContentManagement.Widgets.Widgets.WidgetZone")]
        public int WidgetZoneId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Widgets.Widgets.WidgetZone")]
        public string WidgetZoneName { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Widgets.Widgets.PluginFriendlyName")]
        public string PluginFriendlyName { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Widgets.Widgets.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}