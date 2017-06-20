using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Nop.Admin.Models.Settings;
using Nop.Core;
using Nop.Services.Common;

namespace Nop.Admin.Components
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