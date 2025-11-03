using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.PeachPayments.Domains;
using Nop.Plugin.Payments.PeachPayments.Models;
using Nop.Plugin.Payments.PeachPayments.Services;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.PeachPayments.Controllers
{
    [Area(AreaNames.ADMIN)]
    [AutoValidateAntiforgeryToken]
    [ValidateIpAddress]
    [AuthorizeAdmin]
    public class PeachPaymentsController : BasePluginController
    {
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ICurrencyService _currencyService;
        private readonly ICommonModelFactory _commonModelFactory;

        public PeachPaymentsController(IPermissionService permissionService,ISettingService settingService, IStoreContext storeContext,ICurrencyService currencyService, ShoppingCartSettings shoppingCartSettings,ILocalizationService localizationService, INotificationService notificationService,ICommonModelFactory commonModelFactory)

        {
            _permissionService = permissionService;
            _storeContext = storeContext;
            _settingService = settingService;
            _shoppingCartSettings = shoppingCartSettings;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _currencyService = currencyService;
            _commonModelFactory = commonModelFactory;
        }
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS))
                return AccessDeniedView();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<PeachPaymentsSettings>(storeId);

            //we don't need some of the shared settings that loaded above, so load them separately for chosen store
            if (storeId > 0)
            {
                settings.WebhookUrl = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(PeachPaymentsSettings)}.{nameof(PeachPaymentsSettings.WebhookUrl)}", storeId: storeId);
                settings.UseSandbox = await _settingService
                    .GetSettingByKeyAsync<bool>($"{nameof(PeachPaymentsSettings)}.{nameof(PeachPaymentsSettings.UseSandbox)}", storeId: storeId);
                settings.ClientId = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(PeachPaymentsSettings)}.{nameof(PeachPaymentsSettings.ClientId)}", storeId: storeId);
                settings.SecretKey = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(PeachPaymentsSettings)}.{nameof(PeachPaymentsSettings.SecretKey)}", storeId: storeId);
                settings.Email = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(PeachPaymentsSettings)}.{nameof(PeachPaymentsSettings.Email)}", storeId: storeId);
                settings.MerchantGuid = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(PeachPaymentsSettings)}.{nameof(PeachPaymentsSettings.MerchantGuid)}", storeId: storeId);
                settings.SignUpUrl = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(PeachPaymentsSettings)}.{nameof(PeachPaymentsSettings.SignUpUrl)}", storeId: storeId);
            }
            var currency = await _currencyService.GetAllCurrenciesAsync(showHidden: true);
            List<SelectListItem> newList = new List<SelectListItem>();
            for (var i = 0; i < currency.Count; i++)
            {
                SelectListItem selListItem = new SelectListItem() { Value = currency[i].Id.ToString(), Text = currency[i].Name };

                newList.Add(selListItem);

            }
            //Add select list item to list of selectlistitems

            //Return the list of selectlistitems as a selectlist
            //settings.OrderStatus = OrderStatus.Complete;
            var systemmodel = await _commonModelFactory.PrepareSystemInfoModelAsync(new SystemInfoModel());
            string protocolreq = "http://";
            if(Request.IsHttps)
                protocolreq = "https://";
            string callbackurl = protocolreq+systemmodel.HttpHost + "/Plugins/PeachPayments/Callback";
            var model = new ConfigurationModel
            {
                CheckoutChannel = settings.CheckoutChannel,
                SecretToken = settings.SecretToken,
                CheckoutChannelSandbox = settings.CheckoutChannelSandbox,
                SecretTokenSandbox = settings.SecretTokenSandbox,
                Callbackurl = callbackurl,
                MerchantIdPrefix = settings.MerchantIdPrefix,
                PeachPaymentsCheckoutDisplayText = settings.PeachPaymentsCheckoutDisplayText,
                SortOrder = settings.SortOrder,
                SandBoxModeId = Convert.ToInt32(settings.SandBoxModeId),
                SandBoxModeValues = await settings.SandboxMode.ToSelectListAsync(),
                CurrencyValues = new SelectList(newList, "Value", "Text", null),
                CurrencyId=Convert.ToInt32(settings.CurrencyId),

                Email = settings.Email,
                SetCredentialsManually = settings.SetCredentialsManually,
                UseSandbox = settings.UseSandbox,
                ClientId = settings.SetCredentialsManually ? settings.ClientId : string.Empty,
                SecretKey = settings.SetCredentialsManually ? settings.SecretKey : string.Empty,
                PaymentTypeId = (int)settings.PaymentType,
                DisplayButtonsOnShoppingCart = settings.DisplayButtonsOnShoppingCart,
                DisplayButtonsOnProductDetails = settings.DisplayButtonsOnProductDetails,
                DisplayLogoInHeaderLinks = settings.DisplayLogoInHeaderLinks,
                LogoInHeaderLinks = settings.LogoInHeaderLinks,
                DisplayLogoInFooter = settings.DisplayLogoInFooter,
                DisplayPayLaterMessages = settings.DisplayPayLaterMessages,
                LogoInFooter = settings.LogoInFooter,
                ActiveStoreScopeConfiguration = storeId,
                IsConfigured = ServiceManager.IsConfigured(settings)
            };
            // await PrepareCredentialsAsync(model, settings, storeId);

            if (storeId > 0)
            {
                model.Email_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.Email, storeId);
                model.SetCredentialsManually_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.SetCredentialsManually, storeId);
                model.UseSandbox_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.UseSandbox, storeId);
                model.ClientId_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.ClientId, storeId);
                model.SecretKey_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.SecretKey, storeId);
                model.PaymentTypeId_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.PaymentType, storeId);
                model.DisplayButtonsOnShoppingCart_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.DisplayButtonsOnShoppingCart, storeId);
                model.DisplayButtonsOnProductDetails_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.DisplayButtonsOnProductDetails, storeId);
                model.DisplayLogoInHeaderLinks_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.DisplayLogoInHeaderLinks, storeId);
                model.LogoInHeaderLinks_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.LogoInHeaderLinks, storeId);
                model.DisplayLogoInFooter_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.DisplayLogoInFooter, storeId);
                model.DisplayPayLaterMessages_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.DisplayPayLaterMessages, storeId);
                model.LogoInFooter_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.LogoInFooter, storeId);
            }

            model.PaymentTypes = (await PaymentType.Capture.ToSelectListAsync(false))
                .Select(item => new SelectListItem(item.Text, item.Value))
                .ToList();

            //prices and total aren't rounded, so display warning
            if (model.IsConfigured && !_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                var url = Url.Action("AllSettings", "Setting", new { settingName = nameof(ShoppingCartSettings.RoundPricesDuringCalculation) });
                var warning = string.Format(await _localizationService.GetResourceAsync("Plugins.Payments.PeachPayments.RoundingWarning"), url);
                _notificationService.WarningNotification(warning, false);
            }

            return View("~/Plugins/Payments.PeachPayments/Views/Configure.cshtml", model);
        }

        [HttpPost]

        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var peachPaymentSettings = await _settingService.LoadSettingAsync<PeachPaymentsSettings>(storeScope);

            //save settings
            peachPaymentSettings.CheckoutChannel = model.CheckoutChannel;
            peachPaymentSettings.SecretToken = model.SecretToken;
            peachPaymentSettings.CheckoutChannelSandbox = model.CheckoutChannelSandbox;
            peachPaymentSettings.SecretTokenSandbox = model.SecretTokenSandbox;
            peachPaymentSettings.Callbackurl = model.Callbackurl;
            peachPaymentSettings.MerchantIdPrefix = model.MerchantIdPrefix;
            peachPaymentSettings.PeachPaymentsCheckoutDisplayText = model.PeachPaymentsCheckoutDisplayText;
            peachPaymentSettings.SortOrder = model.SortOrder;
            peachPaymentSettings.SandBoxModeId = model.SandBoxModeId;
            peachPaymentSettings.CurrencyId = model.CurrencyId;
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            await _settingService.SaveSettingOverridablePerStoreAsync(peachPaymentSettings, x => x.CheckoutChannel, model.CheckoutChannel_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(peachPaymentSettings, x => x.SecretToken, model.SecretToken_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(peachPaymentSettings, x => x.CheckoutChannelSandbox, model.CheckoutChannelSandbox_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(peachPaymentSettings, x => x.SecretTokenSandbox, model.SecretTokenSandbox_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(peachPaymentSettings, x => x.Callbackurl, model.Callbackurl_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(peachPaymentSettings, x => x.MerchantIdPrefix, model.MerchantIdPrefix_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(peachPaymentSettings, x => x.PeachPaymentsCheckoutDisplayText, model.PeachPaymentsCheckoutDisplayText_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(peachPaymentSettings, x => x.SortOrder, model.SortOrder_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(peachPaymentSettings, x => x.SandBoxModeId, model.SandBoxModeId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(peachPaymentSettings, x => x.CurrencyId, model.CurrencyId_OverrideForStore, storeScope, false);
            

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }


        public async Task<ActionResult> Return()
        {
            return RedirectToRoute("CheckoutCompleted", new { orderId = 2 });
        }
       public ActionResult Callback()
        {
            return Content("Call back");
        }
        public ActionResult Result()
        {
            return Content("Result");
        }

    }
}