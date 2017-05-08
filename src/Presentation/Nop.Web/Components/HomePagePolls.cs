using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Components
{
    public class HomePagePollsViewComponent : ViewComponent
    {
        private readonly IPollModelFactory _pollModelFactory;

        public HomePagePollsViewComponent(IPollModelFactory pollModelFactory)
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
