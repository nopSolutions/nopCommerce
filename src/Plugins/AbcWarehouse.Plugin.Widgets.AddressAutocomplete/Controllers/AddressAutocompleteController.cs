using System.Threading.Tasks;
using AbcWarehouse.Plugin.Widgets.AddressAutocomplete.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Core;

namespace AbcWarehouse.Plugin.Widgets.AddressAutocomplete.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class AddressAutocompleteController : BasePluginController
    {
        private readonly AddressAutocompleteSettings _settings;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        public AddressAutocompleteController(
            AddressAutocompleteSettings settings,
            ISettingService settingService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _settings = settings;
            _settingService = settingService;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [AutoValidateAntiforgeryToken]
        public ActionResult Configure()
        {
            return View(
                "~/Plugins/Widgets.AddressAutocomplete/Views/Configure.cshtml",
                _settings.ToModel());
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> Configure(ConfigModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            await _settingService.SaveSettingAsync(AddressAutocompleteSettings.FromModel(model));

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return Configure();
        }
    }
}
