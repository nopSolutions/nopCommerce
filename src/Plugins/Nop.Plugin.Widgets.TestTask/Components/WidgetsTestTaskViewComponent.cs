using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.TestTask.Models;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.TestTask.Components
{
    [ViewComponent(Name = "WidgetsTestTask")]
    public class WidgetsTestTaskViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;

        public WidgetsTestTaskViewComponent(IStoreContext storeContext, 
            ISettingService settingService)
        {
            this._storeContext = storeContext;
            this._settingService = settingService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var testTaskSettings = _settingService.LoadSetting<TestTaskSettings>(_storeContext.CurrentStore.Id);

            var model = new PublicInfoModel
            {
                PromoMessage = testTaskSettings.PromoMessage,
            };

            return View("~/Plugins/Widgets.TestTask/Views/PublicInfo.cshtml", model);
        }
    }
}
