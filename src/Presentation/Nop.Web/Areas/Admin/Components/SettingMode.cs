using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Core;
using Nop.Services.Common;

namespace Nop.Web.Areas.Admin.Components
{
    public class SettingModeViewComponent : ViewComponent
    {
        private readonly IWorkContext _workContext;

        public SettingModeViewComponent(IWorkContext workContext)
        {
            this._workContext = workContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string modeName = "settings-advanced-mode")
        {
            var model = new ModeModel()
            {
                ModeName = modeName,
                Enabled = _workContext.CurrentCustomer.GetAttribute<bool>(modeName)
            };

            return View(model);
        }
    }
}