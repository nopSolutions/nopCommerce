using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;

namespace Nop.Web.Components
{
    public class PollBlockViewComponent : ViewComponent
    {
        private readonly IPollModelFactory _pollModelFactory;

        public PollBlockViewComponent(IPollModelFactory pollModelFactory)
        {
            this._pollModelFactory = pollModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string systemKeyword)
        {

            if (String.IsNullOrWhiteSpace(systemKeyword))
                return Content("");

            var model = _pollModelFactory.PreparePollModelBySystemName(systemKeyword);
            if (model == null)
                return Content("");

            return View(model);
        }
    }
}
