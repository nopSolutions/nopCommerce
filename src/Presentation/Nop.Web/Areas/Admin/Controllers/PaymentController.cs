﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Payments;
using Nop.Core.Events;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Payments;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class PaymentController : BaseAdminController
{
    #region Fields

    protected readonly ICountryService _countryService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPaymentModelFactory _paymentModelFactory;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly PaymentSettings _paymentSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public PaymentController(ICountryService countryService,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPaymentModelFactory paymentModelFactory,
        IPaymentPluginManager paymentPluginManager,
        IPermissionService permissionService,
        ISettingService settingService,
        PaymentSettings paymentSettings)
    {
        _countryService = countryService;
        _eventPublisher = eventPublisher;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _paymentModelFactory = paymentModelFactory;
        _paymentPluginManager = paymentPluginManager;
        _permissionService = permissionService;
        _settingService = settingService;
        _paymentSettings = paymentSettings;
    }

    #endregion

    #region Methods  

    public virtual IActionResult PaymentMethods()
    {
        return RedirectToAction("Methods");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public virtual async Task<IActionResult> Methods()
    {
        //prepare model
        var model = await _paymentModelFactory.PreparePaymentMethodsModelAsync(new PaymentMethodsModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public virtual async Task<IActionResult> Methods(PaymentMethodSearchModel searchModel)
    {
        //prepare model
        var model = await _paymentModelFactory.PreparePaymentMethodListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public virtual async Task<IActionResult> MethodUpdate(PaymentMethodModel model)
    {
        var pm = await _paymentPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
        if (_paymentPluginManager.IsPluginActive(pm))
        {
            if (!model.IsActive)
            {
                //mark as disabled
                _paymentSettings.ActivePaymentMethodSystemNames.Remove(pm.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_paymentSettings);
            }
        }
        else
        {
            if (model.IsActive)
            {
                //mark as active
                _paymentSettings.ActivePaymentMethodSystemNames.Add(pm.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_paymentSettings);
            }
        }

        var pluginDescriptor = pm.PluginDescriptor;
        pluginDescriptor.FriendlyName = model.FriendlyName;
        pluginDescriptor.DisplayOrder = model.DisplayOrder;

        //update the description file
        pluginDescriptor.Save();

        //raise event
        await _eventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public virtual async Task<IActionResult> MethodRestrictions()
    {
        //prepare model
        var model = await _paymentModelFactory.PreparePaymentMethodsModelAsync(new PaymentMethodsModel());

        return View(model);
    }

    //we ignore this filter for increase RequestFormLimits
    [IgnoreAntiforgeryToken]
    //we use 2048 value because in some cases default value (1024) is too small for this action
    [RequestFormLimits(ValueCountLimit = 2048)]
    [HttpPost, ActionName("MethodRestrictions")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public virtual async Task<IActionResult> MethodRestrictionsSave(PaymentMethodsModel model, IFormCollection form)
    {
        var paymentMethods = await _paymentPluginManager.LoadAllPluginsAsync();
        var countries = await _countryService.GetAllCountriesAsync(showHidden: true);

        foreach (var pm in paymentMethods)
        {
            var formKey = "restrict_" + pm.PluginDescriptor.SystemName;
            var countryIdsToRestrict = (!StringValues.IsNullOrEmpty(form[formKey])
                    ? form[formKey].ToString().Split(_separator, StringSplitOptions.RemoveEmptyEntries).ToList()
                    : new List<string>())
                .Select(x => Convert.ToInt32(x)).ToList();

            var newCountryIds = new List<int>();
            foreach (var c in countries)
            {
                if (countryIdsToRestrict.Contains(c.Id))
                {
                    newCountryIds.Add(c.Id);
                }
            }

            await _paymentPluginManager.SaveRestrictedCountriesAsync(pm, newCountryIds);
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Payment.MethodRestrictions.Updated"));

        return RedirectToAction("MethodRestrictions");
    }

    #endregion
}