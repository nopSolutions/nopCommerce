using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.ExternalAuth.ExtendedAuthentication.Components
{
    public class AuthenticationViewComponent : NopViewComponent
    {
        private readonly ISettingService _settingService;

        public AuthenticationViewComponent(
            ISettingService settingService)
        {
            _settingService = settingService;
        }

        public IViewComponentResult Invoke()
        {
            var _externalAuthSettings = _settingService.LoadSettingAsync<ExternalAuthSettings>().Result;
            return View("~/Plugins/ExternalAuth.ExtendedAuth/Views/PublicInfo.cshtml", _externalAuthSettings);
        }
    }
}