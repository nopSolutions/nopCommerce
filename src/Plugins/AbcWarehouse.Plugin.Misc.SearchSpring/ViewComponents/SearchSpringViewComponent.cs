using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AbcWarehouse.Plugin.Misc.SearchSpring.Components
{
    [ViewComponent(Name = "SearchSpring")]
    public class SearchSpringViewComponent : Nop.Web.Framework.Components.NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/AbcWarehouse.Plugin.Misc.SearchSpring/Views/Shared/Components/SearchSpring/Default.cshtml");
        }
    }
}
