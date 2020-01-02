using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class PollBlockViewComponent : NopViewComponent
    {
        private readonly IPollModelFactory _pollModelFactory;

        public PollBlockViewComponent(IPollModelFactory pollModelFactory)
        {
            _pollModelFactory = pollModelFactory;
        }

        public IViewComponentResult Invoke(string systemKeyword)
        {

            if (string.IsNullOrWhiteSpace(systemKeyword))
                return Content("");

            var model = _pollModelFactory.PreparePollModelBySystemName(systemKeyword);
            if (model == null)
                return Content("");

            return View(model);
        }
    }
}
