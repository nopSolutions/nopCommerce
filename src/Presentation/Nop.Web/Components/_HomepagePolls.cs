using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Components
{
    public class HomepagePollsViewComponent : ViewComponent
    {
        private readonly IPollModelFactory _pollModelFactory;

        public HomepagePollsViewComponent(IPollModelFactory pollModelFactory)
        {
            this._pollModelFactory = pollModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = _pollModelFactory.PrepareHomePagePollModels();
            if (!model.Any())
                Content("");

            return View(model);
        }
    }
}
