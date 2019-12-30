using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.Square.Models;
using Nop.Plugin.Payments.Square.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;

namespace Nop.Plugin.Payments.Square.Components
{
    /// <summary>
    /// Represents payment info view component
    /// </summary>
    [ViewComponent(Name = SquarePaymentDefaults.VIEW_COMPONENT_NAME)]
    public class PaymentSquareViewComponent : ViewComponent
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly SquarePaymentManager _squarePaymentManager;
        private readonly SquarePaymentSettings _squarePaymentSettings;

        #endregion

        #region Ctor

        public PaymentSquareViewComponent(CurrencySettings currencySettings,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IWorkContext workContext,
            SquarePaymentManager squarePaymentManager,
            SquarePaymentSettings squarePaymentSettings)
        {
            _currencySettings = currencySettings;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _workContext = workContext;
            _squarePaymentManager = squarePaymentManager;
            _squarePaymentSettings = squarePaymentSettings;
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
            var model = new PaymentInfoModel
            {
                //whether current customer is guest
                IsGuest = _customerService.IsGuest(_workContext.CurrentCustomer),

                //get postal code from the billing address or from the shipping one
                PostalCode = _customerService.GetCustomerBillingAddress(_workContext.CurrentCustomer)?.ZipPostalCode
                    ?? _customerService.GetCustomerShippingAddress(_workContext.CurrentCustomer)?.ZipPostalCode
            };

            //whether customer already has stored cards in current store
            var storeId = _storeContext.CurrentStore.Id;
            var customerId = _genericAttributeService
                .GetAttribute<string>(_workContext.CurrentCustomer, SquarePaymentDefaults.CustomerIdAttribute) ?? string.Empty;
            var customer = _squarePaymentManager.GetCustomer(customerId, storeId);
            if (customer?.Cards != null)
            {
                var cardNumberMask = _localizationService.GetResource("Plugins.Payments.Square.Fields.StoredCard.Mask");
                model.StoredCards = customer.Cards
                    .Select(card => new SelectListItem { Text = string.Format(cardNumberMask, card.Last4), Value = card.Id })
                    .ToList();
            }

            //add the special item for 'select card' with empty GUID value
            if (model.StoredCards.Any())
            {
                var selectCardText = _localizationService.GetResource("Plugins.Payments.Square.Fields.StoredCard.SelectCard");
                model.StoredCards.Insert(0, new SelectListItem { Text = selectCardText, Value = Guid.Empty.ToString() });
            }

            //set verfication details
            if (_squarePaymentSettings.Use3ds)
            {
                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer,
                    ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);
                model.OrderTotal = _orderTotalCalculationService.GetShoppingCartTotal(cart, false, false) ?? decimal.Zero;

                var currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
                model.Currency = currency?.CurrencyCode;

                var billingAddress = _customerService.GetCustomerBillingAddress(_workContext.CurrentCustomer);
                var country = _countryService.GetCountryByAddress(billingAddress);
                var stateProvince = _stateProvinceService.GetStateProvinceByAddress(billingAddress);

                model.BillingFirstName = billingAddress?.FirstName;
                model.BillingLastName = billingAddress?.LastName;
                model.BillingEmail = billingAddress?.Email;
                model.BillingCountry = country?.TwoLetterIsoCode;
                model.BillingState = stateProvince?.Name;
                model.BillingCity = billingAddress?.City;
                model.BillingPostalCode = billingAddress?.ZipPostalCode;
            }

            return View("~/Plugins/Payments.Square/Views/PaymentInfo.cshtml", model);
        }

        #endregion
    }
}