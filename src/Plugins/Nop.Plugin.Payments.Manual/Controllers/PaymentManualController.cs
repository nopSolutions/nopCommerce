using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Payments.Manual.Models;
using Nop.Plugin.Payments.Manual.Services;
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.Manual.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class PaymentManualController : BasePaymentController
{
    #region Fields

    private readonly IEncryptionService _encryptionService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IOrderService _orderService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    
    #endregion

    #region Ctor

    public PaymentManualController(IEncryptionService encryptionService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IOrderService orderService,
        ISettingService settingService,
        IStoreContext storeContext)
    {
        _encryptionService = encryptionService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _orderService = orderService;
        _settingService = settingService;
        _storeContext = storeContext;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure()
    {
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var manualPaymentSettings = await _settingService.LoadSettingAsync<ManualPaymentSettings>(storeScope);

        var model = new ConfigurationModel
        {
            TransactModeId = Convert.ToInt32(manualPaymentSettings.TransactMode),
            AdditionalFee = manualPaymentSettings.AdditionalFee,
            AdditionalFeePercentage = manualPaymentSettings.AdditionalFeePercentage,
            TransactModeValues = await manualPaymentSettings.TransactMode.ToSelectListAsync(),
            ActiveStoreScopeConfiguration = storeScope
        };

        if (storeScope > 0)
        {
            model.TransactModeId_OverrideForStore = await _settingService.SettingExistsAsync(manualPaymentSettings, x => x.TransactMode, storeScope);
            model.AdditionalFee_OverrideForStore = await _settingService.SettingExistsAsync(manualPaymentSettings, x => x.AdditionalFee, storeScope);
            model.AdditionalFeePercentage_OverrideForStore = await _settingService.SettingExistsAsync(manualPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
        }

        return View("~/Plugins/Payments.Manual/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var manualPaymentSettings = await _settingService.LoadSettingAsync<ManualPaymentSettings>(storeScope);

        //save settings
        manualPaymentSettings.TransactMode = (TransactMode)model.TransactModeId;
        manualPaymentSettings.AdditionalFee = model.AdditionalFee;
        manualPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;

        //we do not clear cache after each setting update.
        //This behavior can increase performance because cached settings will not be cleared 
        //and loaded from database after each update
        await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.TransactMode, model.TransactModeId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);

        //now clear settings cache
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Orders.ORDERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> SaveCreditCardInfo()
    {
        if (!Request.HasFormContentType)
            return Ok();

        var form = await Request.ReadFormAsync();
        var order = await _orderService.GetOrderByIdAsync(int.Parse(form["orderId"]));

        if (order == null)
            return Ok();

        var creditCardInfo = new CreditCardInfo
        {
            CardType = _encryptionService.EncryptText(form["cardType"]),
            CardName = _encryptionService.EncryptText(form["cardName"]),
            CardNumber = _encryptionService.EncryptText(form["CardNumber"]),
            CardExpirationMonth = _encryptionService.EncryptText(int.Parse(form["cardExpirationMonth"]).ToString()),
            CardExpirationYear = _encryptionService.EncryptText(int.Parse(form["cardExpirationYear"]).ToString()),
            CardCvv2 = _encryptionService.EncryptText(form["cardCvv2"]),
            MaskedCreditCardNumber = _encryptionService.EncryptText(PaymentsManualHelper.GetMaskedCreditCardNumber(form["CardNumber"]))
        };

        var json = JsonConvert.SerializeObject(creditCardInfo);
        await _genericAttributeService.SaveAttributeAsync(order, nameof(CreditCardInfo), json);

        return Ok();
    }

    #endregion
}