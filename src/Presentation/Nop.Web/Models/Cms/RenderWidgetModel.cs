using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Models.Cms
{
    public partial class RenderWidgetModel : BaseNopModel
    {
        public RenderWidgetModel()
        {
            WidgetViewComponentArguments = new RouteValueDictionary();
        }

        public string WidgetViewComponentName { get; set; }
        public RouteValueDictionary WidgetViewComponentArguments { get; set; }
    }
}