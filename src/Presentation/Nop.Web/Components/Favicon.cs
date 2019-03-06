using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class FaviconViewComponent : NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public FaviconViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var model = _commonModelFactory.PrepareFaviconAndAppIconsModel();
            if (string.IsNullOrEmpty(model.HeadCode))
                return Content("");
            return View(model);
        }
    }
}
