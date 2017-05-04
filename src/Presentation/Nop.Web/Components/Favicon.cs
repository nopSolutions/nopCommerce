using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using System.Threading.Tasks;
using System;

namespace Nop.Web.Components
{
    public class FaviconViewComponent : ViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public FaviconViewComponent(ICommonModelFactory commonModelFactory)
        {
            this._commonModelFactory = commonModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = _commonModelFactory.PrepareFaviconModel();
            if (String.IsNullOrEmpty(model.FaviconUrl))
                return Content("");
            return View(model);
        }
    }
}
