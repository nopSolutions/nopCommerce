using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.DateTimeFormat.Components
{
    [ViewComponent(Name = "WidgetsDateTimeFormat")]
    public class WidgetsDateTimeFormatViewComponent : NopViewComponent
    {
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public WidgetsDateTimeFormatViewComponent(ISettingService settingService, IStoreContext storeContext)
        {
            _settingService = settingService;
            _storeContext = storeContext;
        }
        public IViewComponentResult Invoke(string widgetZone)
        {
            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var dateTimeFormatSettings = _settingService.LoadSetting<DateTimeFormatSettings>(storeScope);
            var result = DateTime.Now.ToString(dateTimeFormatSettings.FormatString ?? "");

            return Content(result);
        }
    }
}
