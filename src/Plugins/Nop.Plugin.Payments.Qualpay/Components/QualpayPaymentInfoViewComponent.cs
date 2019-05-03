using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.Qualpay.Models;
using Nop.Plugin.Payments.Qualpay.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Qualpay.Components
{
    /// <summary>
    /// Represents payment info view component
    /// </summary>
    [ViewComponent(Name = QualpayDefaults.PAYMENT_INFO_VIEW_COMPONENT_NAME)]
    public class QualpayPaymentInfoViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly QualpayManager _qualpayManager;
        private readonly QualpaySettings _qualpaySettings;

        #endregion

        #region Ctor

        public QualpayPaymentInfoViewComponent(ILocalizationService localizationService,
            ILogger logger,
            ISettingService settingService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWorkContext workContext,
            QualpayManager qualpayManager,
            QualpaySettings qualpaySettings)
        {
            _localizationService = localizationService;
            _logger = logger;
            _settingService = settingService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
            _qualpayManager = qualpayManager;
            _qualpaySettings = qualpaySettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            //prepare payment model
            var model = new PaymentInfoModel();

            //prepare years
            for (var i = 0; i < 15; i++)
            {
                var year = (DateTime.Now.Year + i).ToString();
                model.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
            }

            //prepare months
            for (var i = 1; i <= 12; i++)
            {
                model.ExpireMonths.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString(), });
            }

            //try to get transient key for Qualpay Embedded Fields
            if (_qualpaySettings.UseEmbeddedFields)
            {
                var key = _qualpayManager.GetTransientKey()?.TransientKey;
                if (string.IsNullOrEmpty(key))
                {
                    //Qualpay Embedded Fields cannot be invoked without a transient key
                    _qualpaySettings.UseEmbeddedFields = false;
                    _settingService.SaveSetting(_qualpaySettings);
                    _logger.Warning(_localizationService.GetResource("Plugins.Payments.Qualpay.Fields.UseEmbeddedFields.TransientKey.Required"));
                }
                else
                    model.TransientKey = key;
            }

            //prepare Qualpay Customer Vault model for non-guest customer
            model.IsGuest = _workContext.CurrentCustomer.IsGuest();
            if (_qualpaySettings.UseCustomerVault && !model.IsGuest)
            {
                //try to get customer billing cards
                model.BillingCards = _qualpayManager.GetCustomerCards(_workContext.CurrentCustomer.Id)
                    .Select(card => new SelectListItem { Text = card.CardNumber, Value = card.CardId }).ToList();

                if (model.BillingCards.Any())
                {
                    //select the first actual card by default
                    model.BillingCardId = model.BillingCards.FirstOrDefault().Value;

                    //add the special item for 'select card' with empty GUID value 
                    var selectCardText = _localizationService.GetResource("Plugins.Payments.Qualpay.Customer.Card.Select");
                    model.BillingCards.Insert(0, new SelectListItem { Text = selectCardText, Value = Guid.Empty.ToString() });
                }

                //whether it's a recurring order
                var currentShoppingCart = _shoppingCartService
                    .GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id).ToList();
                model.IsRecurringOrder = _shoppingCartService.ShoppingCartIsRecurring(currentShoppingCart);
            }

            return View("~/Plugins/Payments.Qualpay/Views/PaymentInfo.cshtml", model);
        }

        #endregion
    }
}