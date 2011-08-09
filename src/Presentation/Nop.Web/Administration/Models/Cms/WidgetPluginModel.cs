using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Cms
{
    public class WidgetPluginModel : BaseNopModel
    {
        public string SystemName { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Widgets.AvailableWidgetPlugins.FriendlyName")]
        public string FriendlyName { get; set; }
    }
}