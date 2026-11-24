using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Http;
using Nop.Services.Affiliates;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Affiliate;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class AffiliateController : BasePublicController
{
    #region Fields

    protected readonly AffiliateSettings _affiliateSettings;
    protected readonly CaptchaSettings _captchaSettings;
    protected readonly IAddressService _addressService;
    protected readonly IAffiliateModelFactory _affiliateModelFactory;
    protected readonly IAffiliateService _affiliateService;
    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public AffiliateController(AffiliateSettings affiliateSettings,
        CaptchaSettings captchaSettings,
        IAddressService addressService,
        IAffiliateModelFactory affiliateModelFactory,
        IAffiliateService affiliateService,
        ICustomerService customerService,
        ILocalizationService localizationService,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings)
    {
        _affiliateSettings = affiliateSettings;
        _captchaSettings = captchaSettings;
        _addressService = addressService;
        _affiliateModelFactory = affiliateModelFactory;
        _affiliateService = affiliateService;
        _customerService = customerService;
        _localizationService = localizationService;
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
        _localizationSettings = localizationSettings;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> ApplyAffiliate()
    {
        if (!_affiliateSettings.AllowCustomersToApplyForAffiliateAccount)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        var model = new ApplyAffiliateModel();
        model = await _affiliateModelFactory.PrepareApplyAffiliateModelAsync(model);

        return View(model);
    }

    [HttpPost, ActionName("ApplyAffiliate")]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> ApplyAffiliateSubmit(ApplyAffiliateModel model, bool captchaValid, IFormCollection form)
    {
        if (!_affiliateSettings.AllowCustomersToApplyForAffiliateAccount)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnApplyAffiliatePage && !captchaValid)
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

        if (ModelState.IsValid)
        {
            if (int.TryParse(form["address-select"], out var addressId))
            {
                var address = await _addressService.GetAddressByIdAsync(addressId);
                address = _addressService.CloneAddress(address);
                await _addressService.InsertAddressAsync(address);

                var affiliate = new Affiliate
                {
                    AddressId = address.Id,
                    AssociatedCustomerId = customer.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                    FriendlyUrlName = model.FriendlyUrlName
                };

                await _affiliateService.InsertAffiliateAsync(affiliate);

                //notify store owner here (email)
                await _workflowMessageService.SendNewAffiliateAccountApplyStoreOwnerNotificationAsync(customer,
                    affiliate, _localizationSettings.DefaultAdminLanguageId);

                model.DisableFormInput = true;
                model.Result = await _localizationService.GetResourceAsync("Affiliate.ApplyAccount.Submitted");

                return View(model);
            }
        }

        //if we got this far, something failed, redisplay form
        model = await _affiliateModelFactory.PrepareApplyAffiliateModelAsync(model);

        return View(model);
    }

    public virtual async Task<IActionResult> Info(OrderHistoryPeriods limit, int? pageNumber)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        var affiliate = await _affiliateService.GetAffiliateByCustomerIdAsync(customer.Id);

        if (affiliate == null)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new AffiliateModel();
        model = await _affiliateModelFactory.PrepareAffiliateModelAsync(affiliate, model, limit, pageNumber);

        return View(model);
    }

    #endregion
}