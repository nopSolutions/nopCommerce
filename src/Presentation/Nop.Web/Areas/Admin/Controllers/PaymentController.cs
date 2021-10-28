using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Core.Events;
using Nop.Services.Common;
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

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class PaymentController : BaseAdminController
    {
        #region Fields

        protected ICountryService CountryService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INotificationService NotificationService { get; }
        protected IPaymentModelFactory PaymentModelFactory { get; }
        protected IPaymentPluginManager PaymentPluginManager { get; }
        protected IPermissionService PermissionService { get; }
        protected ISettingService SettingService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IWorkContext WorkContext { get; }
        protected PaymentSettings PaymentSettings { get; }

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
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
            PaymentSettings paymentSettings)
        {
            CountryService = countryService;
            EventPublisher = eventPublisher;
            LocalizationService = localizationService;
            NotificationService = notificationService;
            PaymentModelFactory = paymentModelFactory;
            PaymentPluginManager = paymentPluginManager;
            PermissionService = permissionService;
            SettingService = settingService;
            GenericAttributeService = genericAttributeService;
            WorkContext = workContext;
            PaymentSettings = paymentSettings;
        }

        #endregion

        #region Methods  

        public virtual IActionResult PaymentMethods()
        {
            return RedirectToAction("Methods");
        }

        public virtual async Task<IActionResult> Methods(bool showtour = false)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prepare model
            var model = await PaymentModelFactory.PreparePaymentMethodsModelAsync(new PaymentMethodsModel());

            //show configuration tour
            if (showtour)
            {
                var customer = await WorkContext.GetCurrentCustomerAsync();
                var hideCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
                var closeCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Methods(PaymentMethodSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await PaymentModelFactory.PreparePaymentMethodListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> MethodUpdate(PaymentMethodModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var pm = await PaymentPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
            if (PaymentPluginManager.IsPluginActive(pm))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    PaymentSettings.ActivePaymentMethodSystemNames.Remove(pm.PluginDescriptor.SystemName);
                    await SettingService.SaveSettingAsync(PaymentSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    PaymentSettings.ActivePaymentMethodSystemNames.Add(pm.PluginDescriptor.SystemName);
                    await SettingService.SaveSettingAsync(PaymentSettings);
                }
            }

            var pluginDescriptor = pm.PluginDescriptor;
            pluginDescriptor.FriendlyName = model.FriendlyName;
            pluginDescriptor.DisplayOrder = model.DisplayOrder;

            //update the description file
            pluginDescriptor.Save();

            //raise event
            await EventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> MethodRestrictions()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prepare model
            var model = await PaymentModelFactory.PreparePaymentMethodsModelAsync(new PaymentMethodsModel());

            return View(model);
        }

        //we ignore this filter for increase RequestFormLimits
        [IgnoreAntiforgeryToken]
        //we use 2048 value because in some cases default value (1024) is too small for this action
        [RequestFormLimits(ValueCountLimit = 2048)]
        [HttpPost, ActionName("MethodRestrictions")]
        public virtual async Task<IActionResult> MethodRestrictionsSave(PaymentMethodsModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var paymentMethods = await PaymentPluginManager.LoadAllPluginsAsync();
            var countries = await CountryService.GetAllCountriesAsync(showHidden: true);

            var form = model.Form;

            foreach (var pm in paymentMethods)
            {
                var formKey = "restrict_" + pm.PluginDescriptor.SystemName;
                var countryIdsToRestrict = (!StringValues.IsNullOrEmpty(form[formKey])
                        ? form[formKey].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
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

                await PaymentPluginManager.SaveRestrictedCountriesAsync(pm, newCountryIds);
            }

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Payment.MethodRestrictions.Updated"));
            
            return RedirectToAction("MethodRestrictions");
        }

        #endregion
    }
}