using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Common;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components
{
    public class SettingModeViewComponent : NopViewComponent
    {
        private readonly IWorkContext _workContext;

        public SettingModeViewComponent(IWorkContext workContext)
        {
            this._workContext = workContext;
        }

        public IViewComponentResult Invoke(string modeName = "settings-advanced-mode")
        {
            var model = new ModeModel
            {
                ModeName = modeName,
                Enabled = _workContext.CurrentCustomer.GetAttribute<bool>(modeName)
            };

            return View(model);
        }
    }
}