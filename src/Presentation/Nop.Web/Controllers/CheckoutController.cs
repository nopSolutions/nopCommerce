using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Http.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Extensions;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Checkout;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class CheckoutController : BasePublicController
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressService _addressService;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IProductService _productService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShippingSettings _shippingSettings;

        #endregion

        #region Ctor

        public CheckoutController(AddressSettings addressSettings,
            CustomerSettings customerSettings,
            IAddressAttributeParser addressAttributeParser,
            IAddressService addressService,
            ICheckoutModelFactory checkoutModelFactory,
            ICountryService countryService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IProductService productService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext,
            OrderSettings orderSettings,
            PaymentSettings paymentSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings)
        {
            _addressSettings = addressSettings;
            _customerSettings = customerSettings;
            _addressAttributeParser = addressAttributeParser;
            _addressService = addressService;
            _checkoutModelFactory = checkoutModelFactory;
            _countryService = countryService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _productService = productService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _paymentSettings = paymentSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _shippingSettings = shippingSettings;
        }

        #endregion

        #region Utilities

        protected virtual async Task<bool> IsMinimumOrderPlacementIntervalValid(Customer customer)
        {
            //prevent 2 orders being placed within an X seconds time frame
            if (_orderSettings.MinimumOrderPlacementInterval == 0)
                return true;

            var lastOrder = (await _orderService.SearchOrders(storeId: (await _storeContext.GetCurrentStore()).Id,
                customerId: (await _workContext.GetCurrentCustomer()).Id, pageSize: 1))
                .FirstOrDefault();
            if (lastOrder == null)
                return true;

            var interval = DateTime.UtcNow - lastOrder.CreatedOnUtc;
            return interval.TotalSeconds > _orderSettings.MinimumOrderPlacementInterval;
        }

        /// <summary>
        /// Parses the value indicating whether the "pickup in store" is allowed
        /// </summary>
        /// <param name="form">The form</param>
        /// <returns>The value indicating whether the "pickup in store" is allowed</returns>
        protected virtual bool ParsePickupInStore(IFormCollection form)
        {
            var pickupInStore = false;

            var pickupInStoreParameter = form["PickupInStore"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(pickupInStoreParameter))
                bool.TryParse(pickupInStoreParameter, out pickupInStore);

            return pickupInStore;
        }

        /// <summary>
        /// Parses the pickup option
        /// </summary>
        /// <param name="form">The form</param>
        /// <returns>The pickup option</returns>
        protected virtual async Task<PickupPoint> ParsePickupOption(IFormCollection form)
        {
            var pickupPoint = form["pickup-points-id"].ToString().Split(new[] { "___" }, StringSplitOptions.None);

            var selectedPoint = (await _shippingService.GetPickupPoints((await _workContext.GetCurrentCustomer()).BillingAddressId ?? 0,
                await _workContext.GetCurrentCustomer(), pickupPoint[1], (await _storeContext.GetCurrentStore()).Id)).PickupPoints.FirstOrDefault(x => x.Id.Equals(pickupPoint[0]));

            if (selectedPoint == null)
                throw new Exception("Pickup point is not allowed");

            return selectedPoint;
        }

        /// <summary>
        /// Saves the pickup option
        /// </summary>
        /// <param name="pickupPoint">The pickup option</param>
        protected virtual async Task SavePickupOption(PickupPoint pickupPoint)
        {
            var pickUpInStoreShippingOption = new ShippingOption
            {
                Name = string.Format(await _localizationService.GetResource("Checkout.PickupPoints.Name"), pickupPoint.Name),
                Rate = pickupPoint.PickupFee,
                Description = pickupPoint.Description,
                ShippingRateComputationMethodSystemName = pickupPoint.ProviderSystemName,
                IsPickupInStore = true
            };

            await _genericAttributeService.SaveAttribute(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedShippingOptionAttribute, pickUpInStoreShippingOption, (await _storeContext.GetCurrentStore()).Id);
            await _genericAttributeService.SaveAttribute(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedPickupPointAttribute, pickupPoint, (await _storeContext.GetCurrentStore()).Id);
        }

        #endregion

        #region Methods (common)

        public virtual async Task<IActionResult> Index()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            var cartProductIds = cart.Select(ci => ci.ProductId).ToArray();
            var downloadableProductsRequireRegistration =
                _customerSettings.RequireRegistrationForDownloadableProducts && await _productService.HasAnyDownloadableProduct(cartProductIds);

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && (!_orderSettings.AnonymousCheckoutAllowed || downloadableProductsRequireRegistration))
                return Challenge();

            //if we have only "button" payment methods available (displayed on the shopping cart page, not during checkout),
            //then we should allow standard checkout
            //all payment methods (do not filter by country here as it could be not specified yet)
            var paymentMethods = _paymentPluginManager
                .LoadActivePlugins(await _workContext.GetCurrentCustomer(), (await _storeContext.GetCurrentStore()).Id)
                .Where(pm => !pm.HidePaymentMethod(cart).Result).ToList();
            //payment methods displayed during checkout (not with "Button" type)
            var nonButtonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType != PaymentMethodType.Button)
                .ToList();
            //"button" payment methods(*displayed on the shopping cart page)
            var buttonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
                .ToList();
            if (!nonButtonPaymentMethods.Any() && buttonPaymentMethods.Any())
                return RedirectToRoute("ShoppingCart");

            //reset checkout data
            await _customerService.ResetCheckoutData(await _workContext.GetCurrentCustomer(), (await _storeContext.GetCurrentStore()).Id);

            //validation (cart)
            var checkoutAttributesXml = await _genericAttributeService.GetAttribute<string>(await _workContext.GetCurrentCustomer(),
                NopCustomerDefaults.CheckoutAttributes, (await _storeContext.GetCurrentStore()).Id);
            var scWarnings = await _shoppingCartService.GetShoppingCartWarnings(cart, checkoutAttributesXml, true);
            if (scWarnings.Any())
                return RedirectToRoute("ShoppingCart");
            //validation (each shopping cart item)
            foreach (var sci in cart)
            {
                var product = await _productService.GetProductById(sci.ProductId);

                var sciWarnings = await _shoppingCartService.GetShoppingCartItemWarnings(await _workContext.GetCurrentCustomer(),
                    sci.ShoppingCartType,
                    product,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false,
                    sci.Id);
                if (sciWarnings.Any())
                    return RedirectToRoute("ShoppingCart");
            }

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            return RedirectToRoute("CheckoutBillingAddress");
        }

        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> Completed(int? orderId)
        {
            //validation
            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            Order order = null;
            if (orderId.HasValue)
            {
                //load order by identifier (if provided)
                order = await _orderService.GetOrderById(orderId.Value);
            }
            if (order == null)
            {
                order = (await _orderService.SearchOrders(storeId: (await _storeContext.GetCurrentStore()).Id,
                customerId: (await _workContext.GetCurrentCustomer()).Id, pageSize: 1))
                    .FirstOrDefault();
            }
            if (order == null || order.Deleted || (await _workContext.GetCurrentCustomer()).Id != order.CustomerId)
            {
                return RedirectToRoute("Homepage");
            }

            //disable "order completed" page?
            if (_orderSettings.DisableOrderCompletedPage)
            {
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });
            }

            //model
            var model = await _checkoutModelFactory.PrepareCheckoutCompletedModel(order);
            return View(model);
        }

        /// <summary>
        /// Get specified Address by addresId
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public virtual async Task<IActionResult> GetAddressById(int addressId)
        {
            var address = _customerService.GetCustomerAddress((await _workContext.GetCurrentCustomer()).Id, addressId);
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var json = JsonConvert.SerializeObject(address, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            return Content(json, "application/json");
        }

        /// <summary>
        /// Save edited address
        /// </summary>
        /// <param name="model"></param>
        /// <param name="opc"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> SaveEditAddress(CheckoutBillingAddressModel model, bool opc = false)
        {
            try
            {
                var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);
                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                var customer = await _workContext.GetCurrentCustomer();
                //find address (ensure that it belongs to the current customer)
                var address = await _customerService.GetCustomerAddress(customer.Id, model.BillingNewAddress.Id);
                if (address == null)
                    throw new Exception("Address can't be loaded");

                address = model.BillingNewAddress.ToEntity(address);
                await _addressService.UpdateAddress(address);

                (await _workContext.GetCurrentCustomer()).BillingAddressId = address.Id;
                await _customerService.UpdateCustomer(await _workContext.GetCurrentCustomer());

                if (!opc)
                {
                    return Json(new
                    {
                        redirect = Url.RouteUrl("CheckoutBillingAddress")
                    });
                }

                var billingAddressModel = _checkoutModelFactory.PrepareBillingAddressModel(cart, address.CountryId);
                return Json(new
                {
                    selected_id = model.BillingNewAddress.Id,
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "billing",
                        html = await RenderPartialViewToString("OpcBillingAddress", billingAddressModel)
                    }
                });
            }
            catch (Exception exc)
            {
                await _logger.Warning(exc.Message, exc, await _workContext.GetCurrentCustomer());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        /// <summary>
        /// Delete edited address
        /// </summary>
        /// <param name="addressId"></param>
        /// <param name="opc"></param>
        /// <returns></returns>
        public virtual async Task<IActionResult> DeleteEditAddress(int addressId, bool opc = false)
        {
            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);
            if (!cart.Any())
                throw new Exception("Your cart is empty");

            var customer = await _workContext.GetCurrentCustomer();

            var address = await _customerService.GetCustomerAddress(customer.Id, addressId);
            if (address != null)
            {
                await _customerService.RemoveCustomerAddress(customer, address);
                await _customerService.UpdateCustomer(customer);
                await _addressService.DeleteAddress(address);
            }

            if (!opc)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("CheckoutBillingAddress")
                });
            }

            var billingAddressModel = _checkoutModelFactory.PrepareBillingAddressModel(cart);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "billing",
                    html = await RenderPartialViewToString("OpcBillingAddress", billingAddressModel)
                }
            });
        }

        #endregion

        #region Methods (multistep checkout)

        public virtual async Task<IActionResult> BillingAddress(IFormCollection form)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //model
            var model = await _checkoutModelFactory.PrepareBillingAddressModel(cart, prePopulateNewAddressWithCustomerFields: true);

            //check whether "billing address" step is enabled
            if (_orderSettings.DisableBillingAddressCheckoutStep && model.ExistingAddresses.Any())
            {
                if (model.ExistingAddresses.Any())
                {
                    //choose the first one
                    return await SelectBillingAddress(model.ExistingAddresses.First().Id);
                }

                TryValidateModel(model);
                TryValidateModel(model.BillingNewAddress);
                return await NewBillingAddress(model, form);
            }

            return View(model);
        }

        public virtual async Task<IActionResult> SelectBillingAddress(int addressId, bool shipToSameAddress = false)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var address = await _customerService.GetCustomerAddress((await _workContext.GetCurrentCustomer()).Id, addressId);

            if (address == null)
                return RedirectToRoute("CheckoutBillingAddress");

            (await _workContext.GetCurrentCustomer()).BillingAddressId = address.Id;
            await _customerService.UpdateCustomer(await _workContext.GetCurrentCustomer());

            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

            //ship to the same address?
            //by default Shipping is available if the country is not specified
            var shippingAllowed = !_addressSettings.CountryEnabled || ((await _countryService.GetCountryByAddress(address))?.AllowsShipping ?? false);
            if (_shippingSettings.ShipToSameAddress && shipToSameAddress && _shoppingCartService.ShoppingCartRequiresShipping(cart) && shippingAllowed)
            {
                (await _workContext.GetCurrentCustomer()).ShippingAddressId = (await _workContext.GetCurrentCustomer()).BillingAddressId;
                await _customerService.UpdateCustomer(await _workContext.GetCurrentCustomer());
                //reset selected shipping method (in case if "pick up in store" was selected)
                await _genericAttributeService.SaveAttribute<ShippingOption>(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedShippingOptionAttribute, null, (await _storeContext.GetCurrentStore()).Id);
                await _genericAttributeService.SaveAttribute<PickupPoint>(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedPickupPointAttribute, null, (await _storeContext.GetCurrentStore()).Id);
                //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available) 
                return RedirectToRoute("CheckoutShippingMethod");
            }

            return RedirectToRoute("CheckoutShippingAddress");
        }

        [HttpPost, ActionName("BillingAddress")]
        [FormValueRequired("nextstep")]
        public virtual async Task<IActionResult> NewBillingAddress(CheckoutBillingAddressModel model, IFormCollection form)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //custom address attributes
            var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributes(form);
            var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            var newAddress = model.BillingNewAddress;

            if (ModelState.IsValid)
            {
                //try to find an address with the same values (don't duplicate records)
                var address = _addressService.FindAddress((await _customerService.GetAddressesByCustomerId((await _workContext.GetCurrentCustomer()).Id)).ToList(),
                    newAddress.FirstName, newAddress.LastName, newAddress.PhoneNumber,
                    newAddress.Email, newAddress.FaxNumber, newAddress.Company,
                    newAddress.Address1, newAddress.Address2, newAddress.City,
                    newAddress.County, newAddress.StateProvinceId, newAddress.ZipPostalCode,
                    newAddress.CountryId, customAttributes);

                if (address == null)
                {
                    //address is not found. let's create a new one
                    address = newAddress.ToEntity();
                    address.CustomAttributes = customAttributes;
                    address.CreatedOnUtc = DateTime.UtcNow;

                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    await _addressService.InsertAddress(address);

                    await _customerService.InsertCustomerAddress(await _workContext.GetCurrentCustomer(), address);
                }

                (await _workContext.GetCurrentCustomer()).BillingAddressId = address.Id;

                await _customerService.UpdateCustomer(await _workContext.GetCurrentCustomer());

                //ship to the same address?
                if (_shippingSettings.ShipToSameAddress && model.ShipToSameAddress && _shoppingCartService.ShoppingCartRequiresShipping(cart))
                {
                    (await _workContext.GetCurrentCustomer()).ShippingAddressId = (await _workContext.GetCurrentCustomer()).BillingAddressId;
                    await _customerService.UpdateCustomer(await _workContext.GetCurrentCustomer());

                    //reset selected shipping method (in case if "pick up in store" was selected)
                    await _genericAttributeService.SaveAttribute<ShippingOption>(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedShippingOptionAttribute, null, (await _storeContext.GetCurrentStore()).Id);
                    await _genericAttributeService.SaveAttribute<PickupPoint>(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedPickupPointAttribute, null, (await _storeContext.GetCurrentStore()).Id);

                    //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available) 
                    return RedirectToRoute("CheckoutShippingMethod");
                }

                return RedirectToRoute("CheckoutShippingAddress");
            }

            //If we got this far, something failed, redisplay form
            model = await _checkoutModelFactory.PrepareBillingAddressModel(cart,
                selectedCountryId: newAddress.CountryId,
                overrideAttributesXml: customAttributes);
            return View(model);
        }

        public virtual async Task<IActionResult> ShippingAddress()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            if (!_shoppingCartService.ShoppingCartRequiresShipping(cart))
                return RedirectToRoute("CheckoutShippingMethod");

            //model
            var model = await _checkoutModelFactory.PrepareShippingAddressModel(cart, prePopulateNewAddressWithCustomerFields: true);
            return View(model);
        }

        public virtual async Task<IActionResult> SelectShippingAddress(int addressId)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var address = await _customerService.GetCustomerAddress((await _workContext.GetCurrentCustomer()).Id, addressId);

            if (address == null)
                return RedirectToRoute("CheckoutShippingAddress");

            (await _workContext.GetCurrentCustomer()).ShippingAddressId = address.Id;
            await _customerService.UpdateCustomer(await _workContext.GetCurrentCustomer());

            if (_shippingSettings.AllowPickupInStore)
            {
                //set value indicating that "pick up in store" option has not been chosen
                await _genericAttributeService.SaveAttribute<PickupPoint>(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedPickupPointAttribute, null, (await _storeContext.GetCurrentStore()).Id);
            }

            return RedirectToRoute("CheckoutShippingMethod");
        }

        [HttpPost, ActionName("ShippingAddress")]
        [FormValueRequired("nextstep")]
        public virtual async Task<IActionResult> NewShippingAddress(CheckoutShippingAddressModel model, IFormCollection form)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            if (!_shoppingCartService.ShoppingCartRequiresShipping(cart))
                return RedirectToRoute("CheckoutShippingMethod");

            //pickup point
            if (_shippingSettings.AllowPickupInStore && !_orderSettings.DisplayPickupInStoreOnShippingMethodPage)
            {
                var pickupInStore = ParsePickupInStore(form);
                if (pickupInStore)
                {
                    var pickupOption = await ParsePickupOption(form);
                    await SavePickupOption(pickupOption);

                    return RedirectToRoute("CheckoutPaymentMethod");
                }

                //set value indicating that "pick up in store" option has not been chosen
                await _genericAttributeService.SaveAttribute<PickupPoint>(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedPickupPointAttribute, null, (await _storeContext.GetCurrentStore()).Id);
            }

            //custom address attributes
            var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributes(form);
            var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            var newAddress = model.ShippingNewAddress;

            if (ModelState.IsValid)
            {
                //try to find an address with the same values (don't duplicate records)
                var address = _addressService.FindAddress((await _customerService.GetAddressesByCustomerId((await _workContext.GetCurrentCustomer()).Id)).ToList(),
                    newAddress.FirstName, newAddress.LastName, newAddress.PhoneNumber,
                    newAddress.Email, newAddress.FaxNumber, newAddress.Company,
                    newAddress.Address1, newAddress.Address2, newAddress.City,
                    newAddress.County, newAddress.StateProvinceId, newAddress.ZipPostalCode,
                    newAddress.CountryId, customAttributes);

                if (address == null)
                {
                    address = newAddress.ToEntity();
                    address.CustomAttributes = customAttributes;
                    address.CreatedOnUtc = DateTime.UtcNow;
                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    await _addressService.InsertAddress(address);

                    await _customerService.InsertCustomerAddress(await _workContext.GetCurrentCustomer(), address);

                }

                (await _workContext.GetCurrentCustomer()).ShippingAddressId = address.Id;
                await _customerService.UpdateCustomer(await _workContext.GetCurrentCustomer());

                return RedirectToRoute("CheckoutShippingMethod");
            }

            //If we got this far, something failed, redisplay form
            model = await _checkoutModelFactory.PrepareShippingAddressModel(cart,
                selectedCountryId: newAddress.CountryId,
                overrideAttributesXml: customAttributes);
            return View(model);
        }

        public virtual async Task<IActionResult> ShippingMethod()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            if (!_shoppingCartService.ShoppingCartRequiresShipping(cart))
            {
                await _genericAttributeService.SaveAttribute<ShippingOption>(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedShippingOptionAttribute, null, (await _storeContext.GetCurrentStore()).Id);
                return RedirectToRoute("CheckoutPaymentMethod");
            }

            //check if pickup point is selected on the shipping address step
            if (!_orderSettings.DisplayPickupInStoreOnShippingMethodPage)
            {
                var selectedPickUpPoint = await _genericAttributeService
                    .GetAttribute<PickupPoint>(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedPickupPointAttribute, (await _storeContext.GetCurrentStore()).Id);
                if (selectedPickUpPoint != null)
                    return RedirectToRoute("CheckoutPaymentMethod");
            }

            //model
            var model = await _checkoutModelFactory.PrepareShippingMethodModel(cart, await _customerService.GetCustomerShippingAddress(await _workContext.GetCurrentCustomer()));

            if (_shippingSettings.BypassShippingMethodSelectionIfOnlyOne &&
                model.ShippingMethods.Count == 1)
            {
                //if we have only one shipping method, then a customer doesn't have to choose a shipping method
                await _genericAttributeService.SaveAttribute(await _workContext.GetCurrentCustomer(),
                    NopCustomerDefaults.SelectedShippingOptionAttribute,
                    model.ShippingMethods.First().ShippingOption,
                    (await _storeContext.GetCurrentStore()).Id);

                return RedirectToRoute("CheckoutPaymentMethod");
            }

            return View(model);
        }

        [HttpPost, ActionName("ShippingMethod")]
        [FormValueRequired("nextstep")]
        public virtual async Task<IActionResult> SelectShippingMethod(string shippingoption, IFormCollection form)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            if (!_shoppingCartService.ShoppingCartRequiresShipping(cart))
            {
                await _genericAttributeService.SaveAttribute<ShippingOption>(await _workContext.GetCurrentCustomer(),
                    NopCustomerDefaults.SelectedShippingOptionAttribute, null, (await _storeContext.GetCurrentStore()).Id);
                return RedirectToRoute("CheckoutPaymentMethod");
            }

            //pickup point
            if (_shippingSettings.AllowPickupInStore && _orderSettings.DisplayPickupInStoreOnShippingMethodPage)
            {
                var pickupInStore = ParsePickupInStore(form);
                if (pickupInStore)
                {
                    var pickupOption = await ParsePickupOption(form);
                    await SavePickupOption(pickupOption);

                    return RedirectToRoute("CheckoutPaymentMethod");
                }

                //set value indicating that "pick up in store" option has not been chosen
                await _genericAttributeService.SaveAttribute<PickupPoint>(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedPickupPointAttribute, null, (await _storeContext.GetCurrentStore()).Id);
            }

            //parse selected method 
            if (string.IsNullOrEmpty(shippingoption))
                return await ShippingMethod();
            var splittedOption = shippingoption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedOption.Length != 2)
                return await ShippingMethod();
            var selectedName = splittedOption[0];
            var shippingRateComputationMethodSystemName = splittedOption[1];

            //find it
            //performance optimization. try cache first
            var shippingOptions = await _genericAttributeService.GetAttribute<List<ShippingOption>>(await _workContext.GetCurrentCustomer(),
                NopCustomerDefaults.OfferedShippingOptionsAttribute, (await _storeContext.GetCurrentStore()).Id);
            if (shippingOptions == null || !shippingOptions.Any())
            {
                //not found? let's load them using shipping service
                shippingOptions = (await _shippingService.GetShippingOptions(cart, await _customerService.GetCustomerShippingAddress(await _workContext.GetCurrentCustomer()),
                    await _workContext.GetCurrentCustomer(), shippingRateComputationMethodSystemName, (await _storeContext.GetCurrentStore()).Id)).ShippingOptions.ToList();
            }
            else
            {
                //loaded cached results. let's filter result by a chosen shipping rate computation method
                shippingOptions = shippingOptions.Where(so => so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }

            var shippingOption = shippingOptions
                .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));
            if (shippingOption == null)
                return await ShippingMethod();

            //save
            await _genericAttributeService.SaveAttribute(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedShippingOptionAttribute, shippingOption, (await _storeContext.GetCurrentStore()).Id);

            return RedirectToRoute("CheckoutPaymentMethod");
        }

        public virtual async Task<IActionResult> PaymentMethod()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //Check whether payment workflow is required
            //we ignore reward points during cart total calculation
            var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequired(cart, false);
            if (!isPaymentWorkflowRequired)
            {
                await _genericAttributeService.SaveAttribute<string>(await _workContext.GetCurrentCustomer(),
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, null, (await _storeContext.GetCurrentStore()).Id);
                return RedirectToRoute("CheckoutPaymentInfo");
            }

            //filter by country
            var filterByCountryId = 0;
            if (_addressSettings.CountryEnabled)
            {
                filterByCountryId = (await _customerService.GetCustomerBillingAddress(await _workContext.GetCurrentCustomer()))?.CountryId ?? 0;
            }

            //model
            var paymentMethodModel = await _checkoutModelFactory.PreparePaymentMethodModel(cart, filterByCountryId);

            if (_paymentSettings.BypassPaymentMethodSelectionIfOnlyOne &&
                paymentMethodModel.PaymentMethods.Count == 1 && !paymentMethodModel.DisplayRewardPoints)
            {
                //if we have only one payment method and reward points are disabled or the current customer doesn't have any reward points
                //so customer doesn't have to choose a payment method

                await _genericAttributeService.SaveAttribute(await _workContext.GetCurrentCustomer(),
                    NopCustomerDefaults.SelectedPaymentMethodAttribute,
                    paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName,
                    (await _storeContext.GetCurrentStore()).Id);
                return RedirectToRoute("CheckoutPaymentInfo");
            }

            return View(paymentMethodModel);
        }

        [HttpPost, ActionName("PaymentMethod")]
        [FormValueRequired("nextstep")]
        public virtual async Task<IActionResult> SelectPaymentMethod(string paymentmethod, CheckoutPaymentMethodModel model)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //reward points
            if (_rewardPointsSettings.Enabled)
            {
                await _genericAttributeService.SaveAttribute(await _workContext.GetCurrentCustomer(),
                    NopCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, model.UseRewardPoints,
                    (await _storeContext.GetCurrentStore()).Id);
            }

            //Check whether payment workflow is required
            var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequired(cart);
            if (!isPaymentWorkflowRequired)
            {
                await _genericAttributeService.SaveAttribute<string>(await _workContext.GetCurrentCustomer(),
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, null, (await _storeContext.GetCurrentStore()).Id);
                return RedirectToRoute("CheckoutPaymentInfo");
            }
            //payment method 
            if (string.IsNullOrEmpty(paymentmethod))
                return await PaymentMethod();

            if (!_paymentPluginManager.IsPluginActive(paymentmethod, await _workContext.GetCurrentCustomer(), (await _storeContext.GetCurrentStore()).Id))
                return await PaymentMethod();

            //save
            await _genericAttributeService.SaveAttribute(await _workContext.GetCurrentCustomer(),
                NopCustomerDefaults.SelectedPaymentMethodAttribute, paymentmethod, (await _storeContext.GetCurrentStore()).Id);

            return RedirectToRoute("CheckoutPaymentInfo");
        }

        public virtual async Task<IActionResult> PaymentInfo()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //Check whether payment workflow is required
            var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequired(cart);
            if (!isPaymentWorkflowRequired)
            {
                return RedirectToRoute("CheckoutConfirm");
            }

            //load payment method
            var paymentMethodSystemName = await _genericAttributeService.GetAttribute<string>(await _workContext.GetCurrentCustomer(),
                NopCustomerDefaults.SelectedPaymentMethodAttribute, (await _storeContext.GetCurrentStore()).Id);
            var paymentMethod = _paymentPluginManager
                .LoadPluginBySystemName(paymentMethodSystemName, await _workContext.GetCurrentCustomer(), (await _storeContext.GetCurrentStore()).Id);
            if (paymentMethod == null)
                return RedirectToRoute("CheckoutPaymentMethod");

            //Check whether payment info should be skipped
            if (paymentMethod.SkipPaymentInfo ||
                (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection && _paymentSettings.SkipPaymentInfoStepForRedirectionPaymentMethods))
            {
                //skip payment info page
                var paymentInfo = new ProcessPaymentRequest();

                //session save
                HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);

                return RedirectToRoute("CheckoutConfirm");
            }

            //model
            var model = await _checkoutModelFactory.PreparePaymentInfoModel(paymentMethod);
            return View(model);
        }

        [HttpPost, ActionName("PaymentInfo")]
        [FormValueRequired("nextstep")]
        public virtual async Task<IActionResult> EnterPaymentInfo(IFormCollection form)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //Check whether payment workflow is required
            var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequired(cart);
            if (!isPaymentWorkflowRequired)
            {
                return RedirectToRoute("CheckoutConfirm");
            }

            //load payment method
            var paymentMethodSystemName = await _genericAttributeService.GetAttribute<string>(await _workContext.GetCurrentCustomer(),
                NopCustomerDefaults.SelectedPaymentMethodAttribute, (await _storeContext.GetCurrentStore()).Id);
            var paymentMethod = _paymentPluginManager
                .LoadPluginBySystemName(paymentMethodSystemName, await _workContext.GetCurrentCustomer(), (await _storeContext.GetCurrentStore()).Id);
            if (paymentMethod == null)
                return RedirectToRoute("CheckoutPaymentMethod");

            var warnings = await paymentMethod.ValidatePaymentForm(form);
            foreach (var warning in warnings)
                ModelState.AddModelError("", warning);
            if (ModelState.IsValid)
            {
                //get payment info
                var paymentInfo = await paymentMethod.GetPaymentInfo(form);
                //set previous order GUID (if exists)
                _paymentService.GenerateOrderGuid(paymentInfo);

                //session save
                HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);
                return RedirectToRoute("CheckoutConfirm");
            }

            //If we got this far, something failed, redisplay form
            //model
            var model = await _checkoutModelFactory.PreparePaymentInfoModel(paymentMethod);
            return View(model);
        }

        public virtual async Task<IActionResult> Confirm()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //model
            var model = await _checkoutModelFactory.PrepareConfirmOrderModel(cart);
            return View(model);
        }

        [HttpPost, ActionName("Confirm")]
        public virtual async Task<IActionResult> ConfirmOrder()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //model
            var model = await _checkoutModelFactory.PrepareConfirmOrderModel(cart);
            try
            {
                //prevent 2 orders being placed within an X seconds time frame
                if (!await IsMinimumOrderPlacementIntervalValid(await _workContext.GetCurrentCustomer()))
                    throw new Exception(await _localizationService.GetResource("Checkout.MinOrderPlacementInterval"));

                //place order
                var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                if (processPaymentRequest == null)
                {
                    //Check whether payment workflow is required
                    if (await _orderProcessingService.IsPaymentWorkflowRequired(cart))
                        return RedirectToRoute("CheckoutPaymentInfo");

                    processPaymentRequest = new ProcessPaymentRequest();
                }
                _paymentService.GenerateOrderGuid(processPaymentRequest);
                processPaymentRequest.StoreId = (await _storeContext.GetCurrentStore()).Id;
                processPaymentRequest.CustomerId = (await _workContext.GetCurrentCustomer()).Id;
                processPaymentRequest.PaymentMethodSystemName = await _genericAttributeService.GetAttribute<string>(await _workContext.GetCurrentCustomer(),
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, (await _storeContext.GetCurrentStore()).Id);
                HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", processPaymentRequest);
                var placeOrderResult = await _orderProcessingService.PlaceOrder(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };
                    await _paymentService.PostProcessPayment(postProcessPaymentRequest);

                    if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                    {
                        //redirection or POST has been done in PostProcessPayment
                        return Content(await _localizationService.GetResource("Checkout.RedirectMessage"));
                    }

                    return RedirectToRoute("CheckoutCompleted", new { orderId = placeOrderResult.PlacedOrder.Id });
                }

                foreach (var error in placeOrderResult.Errors)
                    model.Warnings.Add(error);
            }
            catch (Exception exc)
            {
                await _logger.Warning(exc.Message, exc);
                model.Warnings.Add(exc.Message);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Methods (one page checkout)

        protected virtual async Task<JsonResult> OpcLoadStepAfterShippingAddress(IList<ShoppingCartItem> cart)
        {
            var shippingMethodModel = await _checkoutModelFactory.PrepareShippingMethodModel(cart, await _customerService.GetCustomerShippingAddress(await _workContext.GetCurrentCustomer()));
            if (_shippingSettings.BypassShippingMethodSelectionIfOnlyOne &&
                shippingMethodModel.ShippingMethods.Count == 1)
            {
                //if we have only one shipping method, then a customer doesn't have to choose a shipping method
                await _genericAttributeService.SaveAttribute(await _workContext.GetCurrentCustomer(),
                    NopCustomerDefaults.SelectedShippingOptionAttribute,
                    shippingMethodModel.ShippingMethods.First().ShippingOption,
                    (await _storeContext.GetCurrentStore()).Id);

                //load next step
                return await OpcLoadStepAfterShippingMethod(cart);
            }

            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "shipping-method",
                    html = await RenderPartialViewToString("OpcShippingMethods", shippingMethodModel)
                },
                goto_section = "shipping_method"
            });
        }

        protected virtual async Task<JsonResult> OpcLoadStepAfterShippingMethod(IList<ShoppingCartItem> cart)
        {
            //Check whether payment workflow is required
            //we ignore reward points during cart total calculation
            var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequired(cart, false);
            if (isPaymentWorkflowRequired)
            {
                //filter by country
                var filterByCountryId = 0;
                if (_addressSettings.CountryEnabled)
                {
                    filterByCountryId = (await _customerService.GetCustomerBillingAddress(await _workContext.GetCurrentCustomer()))?.CountryId ?? 0;
                }

                //payment is required
                var paymentMethodModel = await _checkoutModelFactory.PreparePaymentMethodModel(cart, filterByCountryId);

                if (_paymentSettings.BypassPaymentMethodSelectionIfOnlyOne &&
                    paymentMethodModel.PaymentMethods.Count == 1 && !paymentMethodModel.DisplayRewardPoints)
                {
                    //if we have only one payment method and reward points are disabled or the current customer doesn't have any reward points
                    //so customer doesn't have to choose a payment method

                    var selectedPaymentMethodSystemName = paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName;
                    await _genericAttributeService.SaveAttribute(await _workContext.GetCurrentCustomer(),
                        NopCustomerDefaults.SelectedPaymentMethodAttribute,
                        selectedPaymentMethodSystemName, (await _storeContext.GetCurrentStore()).Id);

                    var paymentMethodInst = _paymentPluginManager
                        .LoadPluginBySystemName(selectedPaymentMethodSystemName, await _workContext.GetCurrentCustomer(), (await _storeContext.GetCurrentStore()).Id);
                    if (!_paymentPluginManager.IsPluginActive(paymentMethodInst))
                        throw new Exception("Selected payment method can't be parsed");

                    return await OpcLoadStepAfterPaymentMethod(paymentMethodInst, cart);
                }

                //customer have to choose a payment method
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "payment-method",
                        html = await RenderPartialViewToString("OpcPaymentMethods", paymentMethodModel)
                    },
                    goto_section = "payment_method"
                });
            }

            //payment is not required
            await _genericAttributeService.SaveAttribute<string>(await _workContext.GetCurrentCustomer(),
                NopCustomerDefaults.SelectedPaymentMethodAttribute, null, (await _storeContext.GetCurrentStore()).Id);

            var confirmOrderModel = await _checkoutModelFactory.PrepareConfirmOrderModel(cart);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "confirm-order",
                    html = await RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
                },
                goto_section = "confirm_order"
            });
        }

        protected virtual async Task<JsonResult> OpcLoadStepAfterPaymentMethod(IPaymentMethod paymentMethod, IList<ShoppingCartItem> cart)
        {
            if (paymentMethod.SkipPaymentInfo ||
                (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection && _paymentSettings.SkipPaymentInfoStepForRedirectionPaymentMethods))
            {
                //skip payment info page
                var paymentInfo = new ProcessPaymentRequest();

                //session save
                HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);

                var confirmOrderModel = await _checkoutModelFactory.PrepareConfirmOrderModel(cart);
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "confirm-order",
                        html = await RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
                    },
                    goto_section = "confirm_order"
                });
            }

            //return payment info page
            var paymenInfoModel = await _checkoutModelFactory.PreparePaymentInfoModel(paymentMethod);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "payment-info",
                    html = await RenderPartialViewToString("OpcPaymentInfo", paymenInfoModel)
                },
                goto_section = "payment_info"
            });
        }

        public virtual async Task<IActionResult> OnePageCheckout()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (!_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("Checkout");

            if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            var model = await _checkoutModelFactory.PrepareOnePageCheckoutModel(cart);
            return View(model);
        }

        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> OpcSaveBilling(CheckoutBillingAddressModel model, IFormCollection form)
        {
            try
            {
                //validation
                if (_orderSettings.CheckoutDisabled)
                    throw new Exception(await _localizationService.GetResource("Checkout.Disabled"));

                var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                int.TryParse(form["billing_address_id"], out var billingAddressId);

                if (billingAddressId > 0)
                {
                    //existing address
                    var address = await _customerService.GetCustomerAddress((await _workContext.GetCurrentCustomer()).Id, billingAddressId)
                        ?? throw new Exception(await _localizationService.GetResource("Checkout.Address.NotFound"));

                    (await _workContext.GetCurrentCustomer()).BillingAddressId = address.Id;
                    await _customerService.UpdateCustomer(await _workContext.GetCurrentCustomer());
                }
                else
                {
                    //new address
                    var newAddress = model.BillingNewAddress;

                    //custom address attributes
                    var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributes(form);
                    var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarnings(customAttributes);
                    foreach (var error in customAttributeWarnings)
                    {
                        ModelState.AddModelError("", error);
                    }

                    //validate model
                    if (!ModelState.IsValid)
                    {
                        //model is not valid. redisplay the form with errors
                        var billingAddressModel = await _checkoutModelFactory.PrepareBillingAddressModel(cart,
                            selectedCountryId: newAddress.CountryId,
                            overrideAttributesXml: customAttributes);
                        billingAddressModel.NewAddressPreselected = true;
                        return Json(new
                        {
                            update_section = new UpdateSectionJsonModel
                            {
                                name = "billing",
                                html = await RenderPartialViewToString("OpcBillingAddress", billingAddressModel)
                            },
                            wrong_billing_address = true,
                        });
                    }

                    //try to find an address with the same values (don't duplicate records)
                    var address = _addressService.FindAddress((await _customerService.GetAddressesByCustomerId((await _workContext.GetCurrentCustomer()).Id)).ToList(),
                        newAddress.FirstName, newAddress.LastName, newAddress.PhoneNumber,
                        newAddress.Email, newAddress.FaxNumber, newAddress.Company,
                        newAddress.Address1, newAddress.Address2, newAddress.City,
                        newAddress.County, newAddress.StateProvinceId, newAddress.ZipPostalCode,
                        newAddress.CountryId, customAttributes);

                    if (address == null)
                    {
                        //address is not found. let's create a new one
                        address = newAddress.ToEntity();
                        address.CustomAttributes = customAttributes;
                        address.CreatedOnUtc = DateTime.UtcNow;

                        //some validation
                        if (address.CountryId == 0)
                            address.CountryId = null;

                        if (address.StateProvinceId == 0)
                            address.StateProvinceId = null;

                        await _addressService.InsertAddress(address);

                        await _customerService.InsertCustomerAddress(await _workContext.GetCurrentCustomer(), address);
                    }

                    (await _workContext.GetCurrentCustomer()).BillingAddressId = address.Id;

                    await _customerService.UpdateCustomer(await _workContext.GetCurrentCustomer());
                }

                if (_shoppingCartService.ShoppingCartRequiresShipping(cart))
                {
                    //shipping is required
                    var address = await _customerService.GetCustomerBillingAddress(await _workContext.GetCurrentCustomer());

                    //by default Shipping is available if the country is not specified
                    var shippingAllowed = !_addressSettings.CountryEnabled || ((await _countryService.GetCountryByAddress(address))?.AllowsShipping ?? false);
                    if (_shippingSettings.ShipToSameAddress && model.ShipToSameAddress && shippingAllowed)
                    {
                        //ship to the same address
                        (await _workContext.GetCurrentCustomer()).ShippingAddressId = address.Id;
                        await _customerService.UpdateCustomer(await _workContext.GetCurrentCustomer());
                        //reset selected shipping method (in case if "pick up in store" was selected)
                        await _genericAttributeService.SaveAttribute<ShippingOption>(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedShippingOptionAttribute, null, (await _storeContext.GetCurrentStore()).Id);
                        await _genericAttributeService.SaveAttribute<PickupPoint>(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedPickupPointAttribute, null, (await _storeContext.GetCurrentStore()).Id);
                        //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available) 
                        return await OpcLoadStepAfterShippingAddress(cart);
                    }

                    //do not ship to the same address
                    var shippingAddressModel = await _checkoutModelFactory.PrepareShippingAddressModel(cart, prePopulateNewAddressWithCustomerFields: true);

                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "shipping",
                            html = await RenderPartialViewToString("OpcShippingAddress", shippingAddressModel)
                        },
                        goto_section = "shipping"
                    });
                }

                //shipping is not required
                await _genericAttributeService.SaveAttribute<ShippingOption>(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedShippingOptionAttribute, null, (await _storeContext.GetCurrentStore()).Id);

                //load next step
                return await OpcLoadStepAfterShippingMethod(cart);
            }
            catch (Exception exc)
            {
                await _logger.Warning(exc.Message, exc, await _workContext.GetCurrentCustomer());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> OpcSaveShipping(CheckoutShippingAddressModel model, IFormCollection form)
        {
            try
            {
                //validation
                if (_orderSettings.CheckoutDisabled)
                    throw new Exception(await _localizationService.GetResource("Checkout.Disabled"));

                var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                if (!_shoppingCartService.ShoppingCartRequiresShipping(cart))
                    throw new Exception("Shipping is not required");

                //pickup point
                if (_shippingSettings.AllowPickupInStore && !_orderSettings.DisplayPickupInStoreOnShippingMethodPage)
                {
                    var pickupInStore = ParsePickupInStore(form);
                    if (pickupInStore)
                    {
                        var pickupOption = await ParsePickupOption(form);
                        await SavePickupOption(pickupOption);

                        return await OpcLoadStepAfterShippingMethod(cart);
                    }

                    //set value indicating that "pick up in store" option has not been chosen
                    await _genericAttributeService.SaveAttribute<PickupPoint>(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedPickupPointAttribute, null, (await _storeContext.GetCurrentStore()).Id);
                }

                int.TryParse(form["shipping_address_id"], out var shippingAddressId);

                if (shippingAddressId > 0)
                {
                    //existing address
                    var address = await _customerService.GetCustomerAddress((await _workContext.GetCurrentCustomer()).Id, shippingAddressId)
                        ?? throw new Exception(await _localizationService.GetResource("Checkout.Address.NotFound"));

                    (await _workContext.GetCurrentCustomer()).ShippingAddressId = address.Id;
                    await _customerService.UpdateCustomer(await _workContext.GetCurrentCustomer());
                }
                else
                {
                    //new address
                    var newAddress = model.ShippingNewAddress;

                    //custom address attributes
                    var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributes(form);
                    var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarnings(customAttributes);
                    foreach (var error in customAttributeWarnings)
                    {
                        ModelState.AddModelError("", error);
                    }

                    //validate model
                    if (!ModelState.IsValid)
                    {
                        //model is not valid. redisplay the form with errors
                        var shippingAddressModel = await _checkoutModelFactory.PrepareShippingAddressModel(cart,
                            selectedCountryId: newAddress.CountryId,
                            overrideAttributesXml: customAttributes);
                        shippingAddressModel.NewAddressPreselected = true;
                        return Json(new
                        {
                            update_section = new UpdateSectionJsonModel
                            {
                                name = "shipping",
                                html = await RenderPartialViewToString("OpcShippingAddress", shippingAddressModel)
                            }
                        });
                    }

                    //try to find an address with the same values (don't duplicate records)
                    var address = _addressService.FindAddress((await _customerService.GetAddressesByCustomerId((await _workContext.GetCurrentCustomer()).Id)).ToList(),
                        newAddress.FirstName, newAddress.LastName, newAddress.PhoneNumber,
                        newAddress.Email, newAddress.FaxNumber, newAddress.Company,
                        newAddress.Address1, newAddress.Address2, newAddress.City,
                        newAddress.County, newAddress.StateProvinceId, newAddress.ZipPostalCode,
                        newAddress.CountryId, customAttributes);

                    if (address == null)
                    {
                        address = newAddress.ToEntity();
                        address.CustomAttributes = customAttributes;
                        address.CreatedOnUtc = DateTime.UtcNow;

                        await _addressService.InsertAddress(address);

                        await _customerService.InsertCustomerAddress(await _workContext.GetCurrentCustomer(), address);
                    }

                    (await _workContext.GetCurrentCustomer()).ShippingAddressId = address.Id;

                    await _customerService.UpdateCustomer(await _workContext.GetCurrentCustomer());
                }

                return await OpcLoadStepAfterShippingAddress(cart);
            }
            catch (Exception exc)
            {
                await _logger.Warning(exc.Message, exc, await _workContext.GetCurrentCustomer());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> OpcSaveShippingMethod(string shippingoption, IFormCollection form)
        {
            try
            {
                //validation
                if (_orderSettings.CheckoutDisabled)
                    throw new Exception(await _localizationService.GetResource("Checkout.Disabled"));

                var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                if (!_shoppingCartService.ShoppingCartRequiresShipping(cart))
                    throw new Exception("Shipping is not required");

                //pickup point
                if (_shippingSettings.AllowPickupInStore && _orderSettings.DisplayPickupInStoreOnShippingMethodPage)
                {
                    var pickupInStore = ParsePickupInStore(form);
                    if (pickupInStore)
                    {
                        var pickupOption = await ParsePickupOption(form);
                        await SavePickupOption(pickupOption);

                        return await OpcLoadStepAfterShippingMethod(cart);
                    }

                    //set value indicating that "pick up in store" option has not been chosen
                    await _genericAttributeService.SaveAttribute<PickupPoint>(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedPickupPointAttribute, null, (await _storeContext.GetCurrentStore()).Id);
                }

                //parse selected method 
                if (string.IsNullOrEmpty(shippingoption))
                    throw new Exception("Selected shipping method can't be parsed");
                var splittedOption = shippingoption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedOption.Length != 2)
                    throw new Exception("Selected shipping method can't be parsed");
                var selectedName = splittedOption[0];
                var shippingRateComputationMethodSystemName = splittedOption[1];

                //find it
                //performance optimization. try cache first
                var shippingOptions = await _genericAttributeService.GetAttribute<List<ShippingOption>>(await _workContext.GetCurrentCustomer(),
                    NopCustomerDefaults.OfferedShippingOptionsAttribute, (await _storeContext.GetCurrentStore()).Id);
                if (shippingOptions == null || !shippingOptions.Any())
                {
                    //not found? let's load them using shipping service
                    shippingOptions = (await _shippingService.GetShippingOptions(cart, await _customerService.GetCustomerShippingAddress(await _workContext.GetCurrentCustomer()),
                        await _workContext.GetCurrentCustomer(), shippingRateComputationMethodSystemName, (await _storeContext.GetCurrentStore()).Id)).ShippingOptions.ToList();
                }
                else
                {
                    //loaded cached results. let's filter result by a chosen shipping rate computation method
                    shippingOptions = shippingOptions.Where(so => so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                        .ToList();
                }

                var shippingOption = shippingOptions
                    .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));
                if (shippingOption == null)
                    throw new Exception("Selected shipping method can't be loaded");

                //save
                await _genericAttributeService.SaveAttribute(await _workContext.GetCurrentCustomer(), NopCustomerDefaults.SelectedShippingOptionAttribute, shippingOption, (await _storeContext.GetCurrentStore()).Id);

                //load next step
                return await OpcLoadStepAfterShippingMethod(cart);
            }
            catch (Exception exc)
            {
                await _logger.Warning(exc.Message, exc, await _workContext.GetCurrentCustomer());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> OpcSavePaymentMethod(string paymentmethod, CheckoutPaymentMethodModel model)
        {
            try
            {
                //validation
                if (_orderSettings.CheckoutDisabled)
                    throw new Exception(await _localizationService.GetResource("Checkout.Disabled"));

                var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                //payment method 
                if (string.IsNullOrEmpty(paymentmethod))
                    throw new Exception("Selected payment method can't be parsed");

                //reward points
                if (_rewardPointsSettings.Enabled)
                {
                    await _genericAttributeService.SaveAttribute(await _workContext.GetCurrentCustomer(),
                        NopCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, model.UseRewardPoints,
                        (await _storeContext.GetCurrentStore()).Id);
                }

                //Check whether payment workflow is required
                var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequired(cart);
                if (!isPaymentWorkflowRequired)
                {
                    //payment is not required
                    await _genericAttributeService.SaveAttribute<string>(await _workContext.GetCurrentCustomer(),
                        NopCustomerDefaults.SelectedPaymentMethodAttribute, null, (await _storeContext.GetCurrentStore()).Id);

                    var confirmOrderModel = await _checkoutModelFactory.PrepareConfirmOrderModel(cart);
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            html = await RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
                        },
                        goto_section = "confirm_order"
                    });
                }

                var paymentMethodInst = _paymentPluginManager
                    .LoadPluginBySystemName(paymentmethod, await _workContext.GetCurrentCustomer(), (await _storeContext.GetCurrentStore()).Id);
                if (!_paymentPluginManager.IsPluginActive(paymentMethodInst))
                    throw new Exception("Selected payment method can't be parsed");

                //save
                await _genericAttributeService.SaveAttribute(await _workContext.GetCurrentCustomer(),
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, paymentmethod, (await _storeContext.GetCurrentStore()).Id);

                return await OpcLoadStepAfterPaymentMethod(paymentMethodInst, cart);
            }
            catch (Exception exc)
            {
                await _logger.Warning(exc.Message, exc, await _workContext.GetCurrentCustomer());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> OpcSavePaymentInfo(IFormCollection form)
        {
            try
            {
                //validation
                if (_orderSettings.CheckoutDisabled)
                    throw new Exception(await _localizationService.GetResource("Checkout.Disabled"));

                var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                var paymentMethodSystemName = await _genericAttributeService.GetAttribute<string>(await _workContext.GetCurrentCustomer(),
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, (await _storeContext.GetCurrentStore()).Id);
                var paymentMethod = _paymentPluginManager
                    .LoadPluginBySystemName(paymentMethodSystemName, await _workContext.GetCurrentCustomer(), (await _storeContext.GetCurrentStore()).Id)
                    ?? throw new Exception("Payment method is not selected");

                var warnings = await paymentMethod.ValidatePaymentForm(form);
                foreach (var warning in warnings)
                    ModelState.AddModelError("", warning);
                if (ModelState.IsValid)
                {
                    //get payment info
                    var paymentInfo = await paymentMethod.GetPaymentInfo(form);
                    //set previous order GUID (if exists)
                    _paymentService.GenerateOrderGuid(paymentInfo);

                    //session save
                    HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);

                    var confirmOrderModel = await _checkoutModelFactory.PrepareConfirmOrderModel(cart);
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            html = await RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
                        },
                        goto_section = "confirm_order"
                    });
                }

                //If we got this far, something failed, redisplay form
                var paymenInfoModel = await _checkoutModelFactory.PreparePaymentInfoModel(paymentMethod);
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "payment-info",
                        html = await RenderPartialViewToString("OpcPaymentInfo", paymenInfoModel)
                    }
                });
            }
            catch (Exception exc)
            {
                await _logger.Warning(exc.Message, exc, await _workContext.GetCurrentCustomer());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> OpcConfirmOrder()
        {
            try
            {
                //validation
                if (_orderSettings.CheckoutDisabled)
                    throw new Exception(await _localizationService.GetResource("Checkout.Disabled"));

                var cart = await _shoppingCartService.GetShoppingCart(await _workContext.GetCurrentCustomer(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStore()).Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                //prevent 2 orders being placed within an X seconds time frame
                if (!await IsMinimumOrderPlacementIntervalValid(await _workContext.GetCurrentCustomer()))
                    throw new Exception(await _localizationService.GetResource("Checkout.MinOrderPlacementInterval"));

                //place order
                var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                if (processPaymentRequest == null)
                {
                    //Check whether payment workflow is required
                    if (await _orderProcessingService.IsPaymentWorkflowRequired(cart))
                    {
                        throw new Exception("Payment information is not entered");
                    }

                    processPaymentRequest = new ProcessPaymentRequest();
                }
                _paymentService.GenerateOrderGuid(processPaymentRequest);
                processPaymentRequest.StoreId = (await _storeContext.GetCurrentStore()).Id;
                processPaymentRequest.CustomerId = (await _workContext.GetCurrentCustomer()).Id;
                processPaymentRequest.PaymentMethodSystemName = await _genericAttributeService.GetAttribute<string>(await _workContext.GetCurrentCustomer(),
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, (await _storeContext.GetCurrentStore()).Id);
                HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", processPaymentRequest);
                var placeOrderResult = await _orderProcessingService.PlaceOrder(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };

                    var paymentMethod = _paymentPluginManager
                        .LoadPluginBySystemName(placeOrderResult.PlacedOrder.PaymentMethodSystemName, await _workContext.GetCurrentCustomer(), (await _storeContext.GetCurrentStore()).Id);
                    if (paymentMethod == null)
                        //payment method could be null if order total is 0
                        //success
                        return Json(new { success = 1 });

                    if (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
                    {
                        //Redirection will not work because it's AJAX request.
                        //That's why we don't process it here (we redirect a user to another page where he'll be redirected)

                        //redirect
                        return Json(new
                        {
                            redirect = $"{await _webHelper.GetStoreLocation()}checkout/OpcCompleteRedirectionPayment"
                        });
                    }

                    await _paymentService.PostProcessPayment(postProcessPaymentRequest);
                    //success
                    return Json(new { success = 1 });
                }

                //error
                var confirmOrderModel = new CheckoutConfirmModel();
                foreach (var error in placeOrderResult.Errors)
                    confirmOrderModel.Warnings.Add(error);

                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "confirm-order",
                        html = await RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
                    },
                    goto_section = "confirm_order"
                });
            }
            catch (Exception exc)
            {
                await _logger.Warning(exc.Message, exc, await _workContext.GetCurrentCustomer());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        public virtual async Task<IActionResult> OpcCompleteRedirectionPayment()
        {
            try
            {
                //validation
                if (!_orderSettings.OnePageCheckoutEnabled)
                    return RedirectToRoute("Homepage");

                if (await _customerService.IsGuest(await _workContext.GetCurrentCustomer()) && !_orderSettings.AnonymousCheckoutAllowed)
                    return Challenge();

                //get the order
                var order = (await _orderService.SearchOrders(storeId: (await _storeContext.GetCurrentStore()).Id,
                customerId: (await _workContext.GetCurrentCustomer()).Id, pageSize: 1)).FirstOrDefault();
                if (order == null)
                    return RedirectToRoute("Homepage");

                var paymentMethod = _paymentPluginManager
                    .LoadPluginBySystemName(order.PaymentMethodSystemName, await _workContext.GetCurrentCustomer(), (await _storeContext.GetCurrentStore()).Id);
                if (paymentMethod == null)
                    return RedirectToRoute("Homepage");
                if (paymentMethod.PaymentMethodType != PaymentMethodType.Redirection)
                    return RedirectToRoute("Homepage");

                //ensure that order has been just placed
                if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes > 3)
                    return RedirectToRoute("Homepage");

                //Redirection will not work on one page checkout page because it's AJAX request.
                //That's why we process it here
                var postProcessPaymentRequest = new PostProcessPaymentRequest
                {
                    Order = order
                };

                await _paymentService.PostProcessPayment(postProcessPaymentRequest);

                if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                {
                    //redirection or POST has been done in PostProcessPayment
                    return Content(await _localizationService.GetResource("Checkout.RedirectMessage"));
                }

                //if no redirection has been done (to a third-party payment page)
                //theoretically it's not possible
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }
            catch (Exception exc)
            {
                await _logger.Warning(exc.Message, exc, await _workContext.GetCurrentCustomer());
                return Content(exc.Message);
            }
        }

        #endregion
    }
}
