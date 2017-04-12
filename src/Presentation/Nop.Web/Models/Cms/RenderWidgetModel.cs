#if NET451
using System.Web.Routing;
#endif
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Cms
{
    public partial class RenderWidgetModel : BaseNopModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
#if NET451
        public RouteValueDictionary RouteValues { get; set; }
#endif
    }
}