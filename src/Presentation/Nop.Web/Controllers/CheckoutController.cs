
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Controllers
{
    public class CheckoutController : BaseNopController
    {
		#region Fields

        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICustomerService _customerService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShippingService _shippingService;
        private readonly IPaymentService _paymentService;
        

        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;

        #endregion

		#region Constructors

        public CheckoutController(IProductService productService, IWorkContext workContext,
            IShoppingCartService shoppingCartService, ILocalizationService localizationService, 
            ITaxService taxService, ICurrencyService currencyService, 
            IPriceFormatter priceFormatter, IOrderProcessingService orderProcessingService,
            ICustomerService customerService,  ICountryService countryService,
            IStateProvinceService stateProvinceService, IShippingService shippingService, 
            IPaymentService paymentService,
            ShoppingCartSettings shoppingCartSettings, OrderSettings orderSettings,
            ShippingSettings shippingSettings,TaxSettings taxSettings)
        {
            this._productService = productService;
            this._workContext = workContext;
            this._shoppingCartService = shoppingCartService;
            this._localizationService = localizationService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._customerService = customerService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._shippingService = shippingService;
            this._paymentService = paymentService;

            this._shoppingCartSettings = shoppingCartSettings;
            this._orderSettings = orderSettings;
            this._shippingSettings = shippingSettings;
            this._taxSettings = taxSettings;
        }

		#endregion Constructors 

        #region Utilities

        #endregion

        #region Methods

        public ActionResult Index()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return RedirectToRoute("Login");

            //reset checkout data
            _customerService.ResetCheckoutData(_workContext.CurrentCustomer, false);

            //validation
            var scWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, _workContext.CurrentCustomer.CheckoutAttributes, true);
            if (scWarnings.Count > 0)
                return RedirectToRoute("ShoppingCart");
            else
            {
                foreach (ShoppingCartItem sci in cart)
                {
                    var sciWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                        sci.ShoppingCartType,
                            sci.ProductVariant,
                            sci.AttributesXml,
                            sci.CustomerEnteredPrice,
                            sci.Quantity);
                    if (sciWarnings.Count > 0)
                        return RedirectToRoute("ShoppingCart");
                }
            }


            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");
            else
                return RedirectToRoute("CheckoutShippingAddress");
        }

        public ActionResult OnePageCheckout()
        {
            return Content("UNDONE OnePageCheckout");
        }

        public ActionResult ShippingAddress()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return RedirectToRoute("Login");

            //TODO validation
            //UNDONE ShippingAddress
            return View();
        }

        public ActionResult BillingAddress()
        {
            return Content("UNDONE BillingAddress");
        }

        public ActionResult Shipping()
        {
            return Content("UNDONE Shipping");
        }

        public ActionResult PaymentMethod()
        {
            return Content("UNDONE PaymentMethod");
        }

        public ActionResult PaymentInfo()
        {
            return Content("UNDONE PaymentInfo");
        }

        public ActionResult Confirm()
        {
            return Content("UNDONE Confirm");
        }

        public ActionResult Complete()
        {
            return Content("UNDONE Complete");
        }
        
        #endregion
    }
}
