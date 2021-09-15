using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Cms
{
    public partial record RenderWidgetModel : BaseNopModel
    {
        public string WidgetViewComponentName { get; set; }
        public object WidgetViewComponentArguments { get; set; }
    }
}