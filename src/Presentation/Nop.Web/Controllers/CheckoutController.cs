
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
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
using Nop.Services;
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
using Nop.Web.Models.Checkout;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;
using Nop.Web.Framework.Security;

namespace Nop.Web.Controllers
{
    [NopHttpsRequirement(SslRequirement.Yes)]
    public class CheckoutController : BaseNopController
    {
		#region Fields

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
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        

        private readonly OrderSettings _orderSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;

        #endregion

		#region Constructors

        public CheckoutController(IWorkContext workContext,
            IShoppingCartService shoppingCartService, ILocalizationService localizationService, 
            ITaxService taxService, ICurrencyService currencyService, 
            IPriceFormatter priceFormatter, IOrderProcessingService orderProcessingService,
            ICustomerService customerService,  ICountryService countryService,
            IStateProvinceService stateProvinceService, IShippingService shippingService, 
            IPaymentService paymentService, IOrderTotalCalculationService orderTotalCalculationService,
            ILogger logger, IOrderService orderService, 
            OrderSettings orderSettings, RewardPointsSettings rewardPointsSettings)
        {
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
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._logger = logger;
            this._orderService = orderService;

            this._orderSettings = orderSettings;
            this._rewardPointsSettings = rewardPointsSettings;
        }

        #endregion

        #region Utilities

        [NonAction]
        private bool IsPaymentWorkflowRequired(IList<ShoppingCartItem> cart)
        {
            bool result = true;

            //check whether order total equals zero
            decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart);
            if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value == decimal.Zero)
                result = false;
            return result;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return new HttpUnauthorizedResult();

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
            if (!_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("Checkout");

            return Content("UNDONE OnePageCheckout");
        }



        public ActionResult ShippingAddress()
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return new HttpUnauthorizedResult();

            if (!cart.RequiresShipping())
            {
                _workContext.CurrentCustomer.SetShippingAddress(null);
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                return RedirectToRoute("CheckoutBillingAddress");
            }

            //model
            var model = new CheckoutShippingAddressModel();
            //existing addresses
            var addresses = _workContext.CurrentCustomer.Addresses.Where(a => a.Country.AllowsShipping).ToList();
            foreach (var address in addresses)
                model.ExistingAddresses.Add(address.ToModel());

            //new address model
            model.NewAddress = new AddressModel();
            model.NewAddress.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountriesForShipping())
                model.NewAddress.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            model.NewAddress.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.OtherNonUS"), Value = "0" });
            
            return View(model);
        }

        public ActionResult SelectShippingAddress(int addressId)
        {
            var address = _workContext.CurrentCustomer.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            if (address == null)
                return RedirectToRoute("CheckoutShippingAddress");

            _workContext.CurrentCustomer.SetShippingAddress(address);
            _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                
            return RedirectToRoute("CheckoutBillingAddress");
        }

        [HttpPost, ActionName("ShippingAddress")]
        [FormValueRequired("nextstep")]
        public ActionResult NewShippingAddress(CheckoutShippingAddressModel model)
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return new HttpUnauthorizedResult();

            if (!cart.RequiresShipping())
            {
                _workContext.CurrentCustomer.SetShippingAddress(null);
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                return RedirectToRoute("CheckoutBillingAddress");
            }

            if (ModelState.IsValid)
            {
                var address = model.NewAddress.ToEntity();
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                _workContext.CurrentCustomer.AddAddress(address);
                _workContext.CurrentCustomer.SetShippingAddress(address);
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                return RedirectToRoute("CheckoutBillingAddress");
            }


            //If we got this far, something failed, redisplay form
            //existing addresses
            var addresses = _workContext.CurrentCustomer.Addresses.Where(a => a.Country.AllowsShipping).ToList();
            foreach (var address in addresses)
                model.ExistingAddresses.Add(address.ToModel());

            //new address model
            //countries
            model.NewAddress.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries())
                model.NewAddress.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.NewAddress.CountryId) });
            //states
            var states = model.NewAddress.CountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(model.NewAddress.CountryId.Value).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.NewAddress.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.NewAddress.StateProvinceId) });
            }
            else
                model.NewAddress.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.OtherNonUS"), Value = "0" });


            return View(model);
        }


        public ActionResult BillingAddress()
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return new HttpUnauthorizedResult();
            
            //model
            var model = new CheckoutBillingAddressModel();
            //existing addresses
            var addresses = _workContext.CurrentCustomer.Addresses.Where(a => a.Country.AllowsBilling).ToList();
            foreach (var address in addresses)
                model.ExistingAddresses.Add(address.ToModel());

            //new address model
            model.NewAddress = new AddressModel();
            model.NewAddress.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountriesForBilling())
                model.NewAddress.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            model.NewAddress.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.OtherNonUS"), Value = "0" });

            return View(model);
        }

        public ActionResult SelectBillingAddress(int addressId)
        {
            var address = _workContext.CurrentCustomer.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            if (address == null)
                return RedirectToRoute("CheckoutBillingAddress");

            _workContext.CurrentCustomer.SetBillingAddress(address);
            _customerService.UpdateCustomer(_workContext.CurrentCustomer);

            return RedirectToRoute("CheckoutShippingMethod");
        }

        [HttpPost, ActionName("BillingAddress")]
        [FormValueRequired("nextstep")]
        public ActionResult NewBillingAddress(CheckoutBillingAddressModel model)
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                var address = model.NewAddress.ToEntity();
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                _workContext.CurrentCustomer.AddAddress(address);
                _workContext.CurrentCustomer.SetBillingAddress(address);
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                return RedirectToRoute("CheckoutShippingMethod");
            }


            //If we got this far, something failed, redisplay form
            //existing addresses
            var addresses = _workContext.CurrentCustomer.Addresses.Where(a => a.Country.AllowsBilling).ToList();
            foreach (var address in addresses)
                model.ExistingAddresses.Add(address.ToModel());

            //new address model
            //countries
            model.NewAddress.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries())
                model.NewAddress.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.NewAddress.CountryId) });
            //states
            var states = model.NewAddress.CountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(model.NewAddress.CountryId.Value).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.NewAddress.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.NewAddress.StateProvinceId) });
            }
            else
                model.NewAddress.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.OtherNonUS"), Value = "0" });


            return View(model);
        }



        public ActionResult ShippingMethod()
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return new HttpUnauthorizedResult();

            if (!cart.RequiresShipping())
            {
                _customerService.SaveCustomerAttribute<ShippingOption>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.LastShippingOption, null);
                return RedirectToRoute("CheckoutPaymentMethod");
            }
            
            
            //model
            var model = new CheckoutShippingMethodModel();
            
            var getShippingOptionResponse = _shippingService.GetShippingOptions(cart, _workContext.CurrentCustomer.ShippingAddress);
            if (getShippingOptionResponse.Success)
            {
                foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                {
                    var soModel = new CheckoutShippingMethodModel.ShippingMethodModel()
                    {
                        Name = shippingOption.Name,
                        Description = shippingOption.Description,
                        ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName,
                    };

                    //calculate discounted and taxed rate
                    Discount appliedDiscount = null;
                    decimal shippingTotalWithoutDiscount = shippingOption.Rate;
                    decimal discountAmount = _orderTotalCalculationService.GetShippingDiscount(_workContext.CurrentCustomer,
                        shippingTotalWithoutDiscount, out appliedDiscount);
                    decimal shippingTotalWithDiscount = shippingTotalWithoutDiscount - discountAmount;
                    if (shippingTotalWithDiscount < decimal.Zero)
                        shippingTotalWithDiscount = decimal.Zero;
                    shippingTotalWithDiscount = Math.Round(shippingTotalWithDiscount, 2);

                    decimal rateBase = _taxService.GetShippingPrice(shippingTotalWithDiscount, _workContext.CurrentCustomer);
                    decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                    soModel.Fee = _priceFormatter.FormatShippingPrice(rate, true);

                    model.ShippingMethods.Add(soModel);
                }
            }
            else
                foreach (var error in getShippingOptionResponse.Errors)
                    model.Warnings.Add(error);
            
            return View(model);
        }

        [HttpPost, ActionName("ShippingMethod")]
        [FormValueRequired("nextstep")]
        [ValidateInput(false)]
        public ActionResult SelectShippingMethod(string shippingoption)
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return new HttpUnauthorizedResult();

            if (!cart.RequiresShipping())
            {
                _customerService.SaveCustomerAttribute<ShippingOption>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.LastShippingOption, null);
                return RedirectToRoute("CheckoutPaymentMethod");
            }

            //parse selected method 
            if (String.IsNullOrEmpty(shippingoption))
                return ShippingMethod();
            var splittedOption = shippingoption.Split(new string[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedOption.Length != 2)
                return ShippingMethod();
            string selectedName = splittedOption[0];
            string shippingRateComputationMethodSystemName = splittedOption[1];

            //find it
            var shippingOptions = _shippingService.GetShippingOptions(cart, _workContext.CurrentCustomer.ShippingAddress, shippingRateComputationMethodSystemName);
            var shippingOption = shippingOptions.ShippingOptions.ToList()
                .Find(so => !String.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));
            if (shippingOption == null)
                return ShippingMethod();

            //save
            _customerService.SaveCustomerAttribute<ShippingOption>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.LastShippingOption, shippingOption);
            
            return RedirectToRoute("CheckoutPaymentMethod");
        }




        public ActionResult PaymentMethod()
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return new HttpUnauthorizedResult();

            //Check whether payment workflow is required
            bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart);
            if (!isPaymentWorkflowRequired)
            {
                _workContext.CurrentCustomer.SelectedPaymentMethodSystemName = "";
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                return RedirectToRoute("CheckoutPaymentInfo");
            }

            //model
            var model = new CheckoutPaymentMethodModel();

            //reward points
            if (_rewardPointsSettings.Enabled && !cart.IsRecurring())
            {
                int rewardPointsBalance = _workContext.CurrentCustomer.GetRewardPointsBalance();
                decimal rewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
                decimal rewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase, _workContext.WorkingCurrency);
                if (rewardPointsAmount > decimal.Zero)
                {
                    model.DisplayRewardPoints = true;
                    model.RewardPointsAmount = _priceFormatter.FormatPrice(rewardPointsAmount, true, false);
                    model.RewardPointsBalance = rewardPointsBalance;
                }
            }
            
            var boundPaymentMethods = _paymentService.LoadActivePaymentMethods();
            foreach (var pm in boundPaymentMethods)
            {
                var pmModel = new CheckoutPaymentMethodModel.PaymentMethodModel()
                {
                    Name = pm.FriendlyName,
                    PaymentMethodSystemName = pm.SystemName,
                };
                //payment method additional fee
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(pm.SystemName);
                decimal rateBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                if (rate > decimal.Zero)
                    pmModel.Fee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate, true);

                model.PaymentMethods.Add(pmModel);
            }

            return View(model);
        }

        [HttpPost, ActionName("PaymentMethod")]
        [FormValueRequired("nextstep")]
        [ValidateInput(false)]
        public ActionResult SelectPaymentMethod(string paymentmethod, CheckoutPaymentMethodModel model)
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return new HttpUnauthorizedResult();

            //Check whether payment workflow is required
            bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart);
            if (!isPaymentWorkflowRequired)
            {
                _workContext.CurrentCustomer.SelectedPaymentMethodSystemName = "";
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                return RedirectToRoute("CheckoutPaymentInfo");
            }
            //payment method 
            if (String.IsNullOrEmpty(paymentmethod))
                return PaymentMethod();

            //reward points
            _workContext.CurrentCustomer.UseRewardPointsDuringCheckout = model.UseRewardPoints;
            _customerService.UpdateCustomer(_workContext.CurrentCustomer);

            var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(paymentmethod);
            if (paymentMethodInst == null)
                //TODO check whether payment method is active if (paymentMethodInst == null && !paymentMethodInst.IsActive)
                return PaymentMethod();

            //save
            _workContext.CurrentCustomer.SelectedPaymentMethodSystemName = paymentmethod;
            _customerService.UpdateCustomer(_workContext.CurrentCustomer);

            return RedirectToRoute("CheckoutPaymentInfo");
        }


        public ActionResult PaymentInfo()
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return new HttpUnauthorizedResult();

            //Check whether payment workflow is required
            bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart);
            if (!isPaymentWorkflowRequired)
            {
                return RedirectToRoute("CheckoutConfirm");
            }
            
            var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(_workContext.CurrentCustomer.SelectedPaymentMethodSystemName);
            if (paymentMethod == null)
                return RedirectToRoute("CheckoutPaymentMethod");

            //model
            var model = new CheckoutPaymentInfoModel();
            string actionName;
            string controllerName;
            RouteValueDictionary routeValues;
            paymentMethod.GetPaymentInfoRoute(out actionName, out controllerName, out routeValues);
            model.PaymentInfoActionName = actionName;
            model.PaymentInfoControllerName = controllerName;
            model.PaymentInfoRouteValues = routeValues;

            return View(model);
        }

        [HttpPost, ActionName("PaymentInfo")]
        [FormValueRequired("nextstep")]
        [ValidateInput(false)]
        public ActionResult EnterPaymentInfo(FormCollection form)
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return new HttpUnauthorizedResult();

            //Check whether payment workflow is required
            bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart);
            if (!isPaymentWorkflowRequired)
            {
                return RedirectToRoute("CheckoutConfirm");
            }

            var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(_workContext.CurrentCustomer.SelectedPaymentMethodSystemName);
            if (paymentMethod == null)
                return RedirectToRoute("CheckoutPaymentMethod");

            var paymentControllerType = paymentMethod.GetControllerType();
            var paymentController = DependencyResolver.Current.GetService(paymentControllerType) as BaseNopPaymentController;
            var warnings = paymentController.ValidatePaymentForm(form);
            foreach (var warning in warnings)
                ModelState.AddModelError("", warning);
            if (ModelState.IsValid)
            {
                //get payment info
                var paymentInfo = paymentController.GetPaymentInfo(form);
                //session save
                this.Session["OrderPaymentInfo"] = paymentInfo;
                return RedirectToRoute("CheckoutConfirm");
            }

            //If we got this far, something failed, redisplay form
            //model
            var model = new CheckoutPaymentInfoModel();
            string actionName;
            string controllerName;
            RouteValueDictionary routeValues;
            paymentMethod.GetPaymentInfoRoute(out actionName, out controllerName, out routeValues);
            model.PaymentInfoActionName = actionName;
            model.PaymentInfoControllerName = controllerName;
            model.PaymentInfoRouteValues = routeValues;
            return View(model);
        }



        public ActionResult Confirm()
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return new HttpUnauthorizedResult();

            //model
            var model = new CheckoutConfirmModel();
            //min order amount validation
            bool minOrderTotalAmountOk = _orderProcessingService.ValidateMinOrderTotalAmount(cart);
            if (!minOrderTotalAmountOk)
            {
                decimal minOrderTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderTotalAmount, _workContext.WorkingCurrency);
                model.MinOrderTotalWarning =  string.Format(_localizationService.GetResource("Checkout.MinOrderTotalAmount"), _priceFormatter.FormatPrice(minOrderTotalAmount, true, false));
            }

            return View(model);
        }


        [HttpPost, ActionName("Confirm")]
        [ValidateInput(false)]
        public ActionResult ConfirmOrder()
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0)
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return new HttpUnauthorizedResult();


            //model
            var model = new CheckoutConfirmModel();
            try
            {
                ProcessPaymentRequest processPaymentRequest = this.Session["OrderPaymentInfo"] as ProcessPaymentRequest;
                if (processPaymentRequest == null)
                {
                    //Check whether payment workflow is required
                    if (IsPaymentWorkflowRequired(cart))
                        return RedirectToRoute("CheckoutPaymentInfo");
                    else
                        processPaymentRequest = new ProcessPaymentRequest();
                }

                processPaymentRequest.Customer = _workContext.CurrentCustomer;
                processPaymentRequest.PaymentMethodSystemName = _workContext.CurrentCustomer.SelectedPaymentMethodSystemName;
                var placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);
                this.Session["OrderPaymentInfo"] = null;
                if (placeOrderResult.Success)
                {
                    var postProcessPaymentRequest = new PostProcessPaymentRequest()
                    {
                        Order = placeOrderResult.PlacedOrder
                    };
                    _paymentService.PostProcessPayment(postProcessPaymentRequest);

                    return RedirectToRoute("CheckoutCompleted");
                }
                else
                {
                    foreach (var error in placeOrderResult.Errors)
                        model.Warnings.Add(error);
                }
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc);
                model.Warnings.Add(exc.Message);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult Completed()
        {
            //validation
            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                return new HttpUnauthorizedResult();

            //model
            var model = new CheckoutCompletedModel();

            var orders = _orderService.GetOrdersByCustomerId(_workContext.CurrentCustomer.Id);
            if (orders.Count == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                var lastOrder = orders[0];
                model.OrderId = lastOrder.Id;
            }

            return View(model);
        }
        
        #endregion
    }
}
