using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public partial class HomepagePollsViewComponent : NopViewComponent
    {
        private readonly IPollModelFactory _pollModelFactory;

        public HomepagePollsViewComponent(IPollModelFactory pollModelFactory)
        {
            _pollModelFactory = pollModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _pollModelFactory.PrepareHomepagePollModelsAsync();
            if (!model.Any())
                return Content("");

            return View(model);
        }
    }
}
