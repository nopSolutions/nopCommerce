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
