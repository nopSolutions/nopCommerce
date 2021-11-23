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

        protected AddressSettings AddressSettings { get; }
        protected CustomerSettings CustomerSettings { get; }
        protected IAddressAttributeParser AddressAttributeParser { get; }
        protected IAddressService AddressService { get; }
        protected ICheckoutModelFactory CheckoutModelFactory { get; }
        protected ICountryService CountryService { get; }
        protected ICustomerService CustomerService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILogger Logger { get; }
        protected IOrderProcessingService OrderProcessingService { get; }
        protected IOrderService OrderService { get; }
        protected IPaymentPluginManager PaymentPluginManager { get; }
        protected IPaymentService PaymentService { get; }
        protected IProductService ProductService { get; }
        protected IShippingService ShippingService { get; }
        protected IShoppingCartService ShoppingCartService { get; }
        protected IStoreContext StoreContext { get; }
        protected IWebHelper WebHelper { get; }
        protected IWorkContext WorkContext { get; }
        protected OrderSettings OrderSettings { get; }
        protected PaymentSettings PaymentSettings { get; }
        protected RewardPointsSettings RewardPointsSettings { get; }
        protected ShippingSettings ShippingSettings { get; }

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
            AddressSettings = addressSettings;
            CustomerSettings = customerSettings;
            AddressAttributeParser = addressAttributeParser;
            AddressService = addressService;
            CheckoutModelFactory = checkoutModelFactory;
            CountryService = countryService;
            CustomerService = customerService;
            GenericAttributeService = genericAttributeService;
            LocalizationService = localizationService;
            Logger = logger;
            OrderProcessingService = orderProcessingService;
            OrderService = orderService;
            PaymentPluginManager = paymentPluginManager;
            PaymentService = paymentService;
            ProductService = productService;
            ShippingService = shippingService;
            ShoppingCartService = shoppingCartService;
            StoreContext = storeContext;
            WebHelper = webHelper;
            WorkContext = workContext;
            OrderSettings = orderSettings;
            PaymentSettings = paymentSettings;
            RewardPointsSettings = rewardPointsSettings;
            ShippingSettings = shippingSettings;
        }

        #endregion

        #region Utilities

        protected virtual async Task<bool> IsMinimumOrderPlacementIntervalValidAsync(Customer customer)
        {
            //prevent 2 orders being placed within an X seconds time frame
            if (OrderSettings.MinimumOrderPlacementInterval == 0)
                return true;

            var store = await StoreContext.GetCurrentStoreAsync();

            var lastOrder = (await OrderService.SearchOrdersAsync(storeId: store.Id,
                customerId: customer.Id, pageSize: 1))
                .FirstOrDefault();
            if (lastOrder == null)
                return true;

            var interval = DateTime.UtcNow - lastOrder.CreatedOnUtc;
            return interval.TotalSeconds > OrderSettings.MinimumOrderPlacementInterval;
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
                _ = bool.TryParse(pickupInStoreParameter, out pickupInStore);

            return pickupInStore;
        }

        /// <summary>
        /// Parses the pickup option
        /// </summary>
        /// <param name="form">The form</param>
        /// <returns>
        /// The task result contains the pickup option
        /// </returns>
        protected virtual async Task<PickupPoint> ParsePickupOptionAsync(IFormCollection form)
        {
            var pickupPoint = form["pickup-points-id"].ToString().Split(new[] { "___" }, StringSplitOptions.None);

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var selectedPoint = (await ShippingService.GetPickupPointsAsync(customer.BillingAddressId ?? 0,
                customer, pickupPoint[1], store.Id)).PickupPoints.FirstOrDefault(x => x.Id.Equals(pickupPoint[0]));

            if (selectedPoint == null)
                throw new Exception("Pickup point is not allowed");

            return selectedPoint;
        }

        /// <summary>
        /// Saves the pickup option
        /// </summary>
        /// <param name="pickupPoint">The pickup option</param>
        protected virtual async Task SavePickupOptionAsync(PickupPoint pickupPoint)
        {
            var name = !string.IsNullOrEmpty(pickupPoint.Name) ?
                string.Format(await LocalizationService.GetResourceAsync("Checkout.PickupPoints.Name"), pickupPoint.Name) :
                await LocalizationService.GetResourceAsync("Checkout.PickupPoints.NullName");
            var pickUpInStoreShippingOption = new ShippingOption
            {
                Name = name,
                Rate = pickupPoint.PickupFee,
                Description = pickupPoint.Description,
                ShippingRateComputationMethodSystemName = pickupPoint.ProviderSystemName,
                IsPickupInStore = true
            };

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, pickUpInStoreShippingOption, store.Id);
            await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedPickupPointAttribute, pickupPoint, store.Id);
        }

        #endregion

        #region Methods (common)

        public virtual async Task<IActionResult> Index()
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            var cartProductIds = cart.Select(ci => ci.ProductId).ToArray();
            var downloadableProductsRequireRegistration =
                CustomerSettings.RequireRegistrationForDownloadableProducts && await ProductService.HasAnyDownloadableProductAsync(cartProductIds);

            if (await CustomerService.IsGuestAsync(customer) && (!OrderSettings.AnonymousCheckoutAllowed || downloadableProductsRequireRegistration))
                return Challenge();

            //if we have only "button" payment methods available (displayed on the shopping cart page, not during checkout),
            //then we should allow standard checkout
            //all payment methods (do not filter by country here as it could be not specified yet)
            var paymentMethods = await (await PaymentPluginManager
                .LoadActivePluginsAsync(customer, store.Id))
                .WhereAwait(async pm => !await pm.HidePaymentMethodAsync(cart)).ToListAsync();
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
            await CustomerService.ResetCheckoutDataAsync(customer, store.Id);

            //validation (cart)
            var checkoutAttributesXml = await GenericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.CheckoutAttributes, store.Id);
            var scWarnings = await ShoppingCartService.GetShoppingCartWarningsAsync(cart, checkoutAttributesXml, true);
            if (scWarnings.Any())
                return RedirectToRoute("ShoppingCart");
            //validation (each shopping cart item)
            foreach (var sci in cart)
            {
                var product = await ProductService.GetProductByIdAsync(sci.ProductId);

                var sciWarnings = await ShoppingCartService.GetShoppingCartItemWarningsAsync(customer,
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

            if (OrderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            return RedirectToRoute("CheckoutBillingAddress");
        }

        public virtual async Task<IActionResult> Completed(int? orderId)
        {
            //validation
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            Order order = null;
            if (orderId.HasValue)
            {
                //load order by identifier (if provided)
                order = await OrderService.GetOrderByIdAsync(orderId.Value);
            }
            if (order == null)
            {
                var store = await StoreContext.GetCurrentStoreAsync();
                order = (await OrderService.SearchOrdersAsync(storeId: store.Id,
                customerId: customer.Id, pageSize: 1))
                    .FirstOrDefault();
            }
            if (order == null || order.Deleted || customer.Id != order.CustomerId)
            {
                return RedirectToRoute("Homepage");
            }

            //disable "order completed" page?
            if (OrderSettings.DisableOrderCompletedPage)
            {
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });
            }

            //model
            var model = await CheckoutModelFactory.PrepareCheckoutCompletedModelAsync(order);
            return View(model);
        }

        /// <summary>
        /// Get specified Address by addresId
        /// </summary>
        /// <param name="addressId"></param>
        public virtual async Task<IActionResult> GetAddressById(int addressId)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var address = await CustomerService.GetCustomerAddressAsync(customer.Id, addressId);
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
        public virtual async Task<IActionResult> SaveEditAddress(CheckoutBillingAddressModel model, bool opc = false)
        {
            try
            {
                var customer = await WorkContext.GetCurrentCustomerAsync();
                var store = await StoreContext.GetCurrentStoreAsync();
                var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                //find address (ensure that it belongs to the current customer)
                var address = await CustomerService.GetCustomerAddressAsync(customer.Id, model.BillingNewAddress.Id);
                if (address == null)
                    throw new Exception("Address can't be loaded");

                address = model.BillingNewAddress.ToEntity(address);
                await AddressService.UpdateAddressAsync(address);

                customer.BillingAddressId = address.Id;
                await CustomerService.UpdateCustomerAsync(customer);

                if (!opc)
                {
                    return Json(new
                    {
                        redirect = Url.RouteUrl("CheckoutBillingAddress")
                    });
                }

                var billingAddressModel = await CheckoutModelFactory.PrepareBillingAddressModelAsync(cart, address.CountryId);
                return Json(new
                {
                    selected_id = model.BillingNewAddress.Id,
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "billing",
                        html = await RenderPartialViewToStringAsync("OpcBillingAddress", billingAddressModel)
                    }
                });
            }
            catch (Exception exc)
            {
                await Logger.WarningAsync(exc.Message, exc, await WorkContext.GetCurrentCustomerAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        /// <summary>
        /// Delete edited address
        /// </summary>
        /// <param name="addressId"></param>
        /// <param name="opc"></param>
        public virtual async Task<IActionResult> DeleteEditAddress(int addressId, bool opc = false)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            if (!cart.Any())
                throw new Exception("Your cart is empty");

            var address = await CustomerService.GetCustomerAddressAsync(customer.Id, addressId);
            if (address != null)
            {
                await CustomerService.RemoveCustomerAddressAsync(customer, address);
                await CustomerService.UpdateCustomerAsync(customer);
                await AddressService.DeleteAddressAsync(address);
            }

            if (!opc)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("CheckoutBillingAddress")
                });
            }

            var billingAddressModel = await CheckoutModelFactory.PrepareBillingAddressModelAsync(cart);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "billing",
                    html = await RenderPartialViewToStringAsync("OpcBillingAddress", billingAddressModel)
                }
            });
        }

        #endregion

        #region Methods (multistep checkout)

        public virtual async Task<IActionResult> BillingAddress(IFormCollection form)
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (OrderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //model
            var model = await CheckoutModelFactory.PrepareBillingAddressModelAsync(cart, prePopulateNewAddressWithCustomerFields: true);

            //check whether "billing address" step is enabled
            if (OrderSettings.DisableBillingAddressCheckoutStep && model.ExistingAddresses.Any())
            {
                if (model.ExistingAddresses.Any())
                {
                    //choose the first one
                    return await SelectBillingAddress(model.ExistingAddresses.First().Id);
                }

                TryValidateModel(model);
                TryValidateModel(model.BillingNewAddress);
                return await NewBillingAddress(model);
            }

            return View(model);
        }

        public virtual async Task<IActionResult> SelectBillingAddress(int addressId, bool shipToSameAddress = false)
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var address = await CustomerService.GetCustomerAddressAsync(customer.Id, addressId);

            if (address == null)
                return RedirectToRoute("CheckoutBillingAddress");

            customer.BillingAddressId = address.Id;
            await CustomerService.UpdateCustomerAsync(customer);

            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            //ship to the same address?
            //by default Shipping is available if the country is not specified
            var shippingAllowed = !AddressSettings.CountryEnabled || ((await CountryService.GetCountryByAddressAsync(address))?.AllowsShipping ?? false);
            if (ShippingSettings.ShipToSameAddress && shipToSameAddress && await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart) && shippingAllowed)
            {
                customer.ShippingAddressId = customer.BillingAddressId;
                await CustomerService.UpdateCustomerAsync(customer);
                //reset selected shipping method (in case if "pick up in store" was selected)
                await GenericAttributeService.SaveAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, store.Id);
                await GenericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);
                //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available) 
                return RedirectToRoute("CheckoutShippingMethod");
            }

            return RedirectToRoute("CheckoutShippingAddress");
        }

        [HttpPost, ActionName("BillingAddress")]
        [FormValueRequired("nextstep")]
        public virtual async Task<IActionResult> NewBillingAddress(CheckoutBillingAddressModel model)
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (OrderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //custom address attributes
            var customAttributes = await AddressAttributeParser.ParseCustomAddressAttributesAsync(model.Form);
            var customAttributeWarnings = await AddressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            var newAddress = model.BillingNewAddress;

            if (ModelState.IsValid)
            {
                //try to find an address with the same values (don't duplicate records)
                var address = AddressService.FindAddress((await CustomerService.GetAddressesByCustomerIdAsync(customer.Id)).ToList(),
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

                    await AddressService.InsertAddressAsync(address);

                    await CustomerService.InsertCustomerAddressAsync(customer, address);
                }

                customer.BillingAddressId = address.Id;

                await CustomerService.UpdateCustomerAsync(customer);

                //ship to the same address?
                if (ShippingSettings.ShipToSameAddress && model.ShipToSameAddress && await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart))
                {
                    customer.ShippingAddressId = customer.BillingAddressId;
                    await CustomerService.UpdateCustomerAsync(customer);

                    //reset selected shipping method (in case if "pick up in store" was selected)
                    await GenericAttributeService.SaveAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, store.Id);
                    await GenericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);

                    //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available) 
                    return RedirectToRoute("CheckoutShippingMethod");
                }

                return RedirectToRoute("CheckoutShippingAddress");
            }

            //If we got this far, something failed, redisplay form
            model = await CheckoutModelFactory.PrepareBillingAddressModelAsync(cart,
                selectedCountryId: newAddress.CountryId,
                overrideAttributesXml: customAttributes);
            return View(model);
        }

        public virtual async Task<IActionResult> ShippingAddress()
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (OrderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            if (!await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart))
                return RedirectToRoute("CheckoutShippingMethod");

            //model
            var model = await CheckoutModelFactory.PrepareShippingAddressModelAsync(cart, prePopulateNewAddressWithCustomerFields: true);
            return View(model);
        }

        public virtual async Task<IActionResult> SelectShippingAddress(int addressId)
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var address = await CustomerService.GetCustomerAddressAsync(customer.Id, addressId);

            if (address == null)
                return RedirectToRoute("CheckoutShippingAddress");

            customer.ShippingAddressId = address.Id;
            await CustomerService.UpdateCustomerAsync(customer);

            if (ShippingSettings.AllowPickupInStore)
            {
                var store = await StoreContext.GetCurrentStoreAsync();
                //set value indicating that "pick up in store" option has not been chosen
                await GenericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);
            }

            return RedirectToRoute("CheckoutShippingMethod");
        }

        [HttpPost, ActionName("ShippingAddress")]
        [FormValueRequired("nextstep")]
        public virtual async Task<IActionResult> NewShippingAddress(CheckoutShippingAddressModel model)
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (OrderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            if (!await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart))
                return RedirectToRoute("CheckoutShippingMethod");

            var form = model.Form;

            //pickup point
            if (ShippingSettings.AllowPickupInStore && !OrderSettings.DisplayPickupInStoreOnShippingMethodPage)
            {
                var pickupInStore = ParsePickupInStore(form);
                if (pickupInStore)
                {
                    var pickupOption = await ParsePickupOptionAsync(form);
                    await SavePickupOptionAsync(pickupOption);

                    return RedirectToRoute("CheckoutPaymentMethod");
                }

                //set value indicating that "pick up in store" option has not been chosen
                await GenericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);
            }

            //custom address attributes
            var customAttributes = await AddressAttributeParser.ParseCustomAddressAttributesAsync(form);
            var customAttributeWarnings = await AddressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            var newAddress = model.ShippingNewAddress;

            if (ModelState.IsValid)
            {
                //try to find an address with the same values (don't duplicate records)
                var address = AddressService.FindAddress((await CustomerService.GetAddressesByCustomerIdAsync(customer.Id)).ToList(),
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

                    await AddressService.InsertAddressAsync(address);

                    await CustomerService.InsertCustomerAddressAsync(customer, address);

                }

                customer.ShippingAddressId = address.Id;
                await CustomerService.UpdateCustomerAsync(customer);

                return RedirectToRoute("CheckoutShippingMethod");
            }

            //If we got this far, something failed, redisplay form
            model = await CheckoutModelFactory.PrepareShippingAddressModelAsync(cart,
                selectedCountryId: newAddress.CountryId,
                overrideAttributesXml: customAttributes);
            return View(model);
        }

        public virtual async Task<IActionResult> ShippingMethod()
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (OrderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            if (!await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart))
            {
                await GenericAttributeService.SaveAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, store.Id);
                return RedirectToRoute("CheckoutPaymentMethod");
            }

            //check if pickup point is selected on the shipping address step
            if (!OrderSettings.DisplayPickupInStoreOnShippingMethodPage)
            {
                var selectedPickUpPoint = await GenericAttributeService
                    .GetAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, store.Id);
                if (selectedPickUpPoint != null)
                    return RedirectToRoute("CheckoutPaymentMethod");
            }

            //model
            var model = await CheckoutModelFactory.PrepareShippingMethodModelAsync(cart, await CustomerService.GetCustomerShippingAddressAsync(customer));

            if (ShippingSettings.BypassShippingMethodSelectionIfOnlyOne &&
                model.ShippingMethods.Count == 1)
            {
                //if we have only one shipping method, then a customer doesn't have to choose a shipping method
                await GenericAttributeService.SaveAttributeAsync(customer,
                    NopCustomerDefaults.SelectedShippingOptionAttribute,
                    model.ShippingMethods.First().ShippingOption,
                    store.Id);

                return RedirectToRoute("CheckoutPaymentMethod");
            }

            return View(model);
        }

        [HttpPost, ActionName("ShippingMethod")]
        [FormValueRequired("nextstep")]
        public virtual async Task<IActionResult> SelectShippingMethod(string shippingoption, IFormCollection form)
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (OrderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            if (!await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart))
            {
                await GenericAttributeService.SaveAttributeAsync<ShippingOption>(customer,
                    NopCustomerDefaults.SelectedShippingOptionAttribute, null, store.Id);
                return RedirectToRoute("CheckoutPaymentMethod");
            }

            //pickup point
            if (ShippingSettings.AllowPickupInStore && OrderSettings.DisplayPickupInStoreOnShippingMethodPage)
            {
                var pickupInStore = ParsePickupInStore(form);
                if (pickupInStore)
                {
                    var pickupOption = await ParsePickupOptionAsync(form);
                    await SavePickupOptionAsync(pickupOption);

                    return RedirectToRoute("CheckoutPaymentMethod");
                }

                //set value indicating that "pick up in store" option has not been chosen
                await GenericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);
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
            var shippingOptions = await GenericAttributeService.GetAttributeAsync<List<ShippingOption>>(customer,
                NopCustomerDefaults.OfferedShippingOptionsAttribute, store.Id);
            if (shippingOptions == null || !shippingOptions.Any())
            {
                //not found? let's load them using shipping service
                shippingOptions = (await ShippingService.GetShippingOptionsAsync(cart, await CustomerService.GetCustomerShippingAddressAsync(customer),
                    customer, shippingRateComputationMethodSystemName, store.Id)).ShippingOptions.ToList();
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
            await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, shippingOption, store.Id);

            return RedirectToRoute("CheckoutPaymentMethod");
        }

        public virtual async Task<IActionResult> PaymentMethod()
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (OrderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //Check whether payment workflow is required
            //we ignore reward points during cart total calculation
            var isPaymentWorkflowRequired = await OrderProcessingService.IsPaymentWorkflowRequiredAsync(cart, false);
            if (!isPaymentWorkflowRequired)
            {
                await GenericAttributeService.SaveAttributeAsync<string>(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, null, store.Id);
                return RedirectToRoute("CheckoutPaymentInfo");
            }

            //filter by country
            var filterByCountryId = 0;
            if (AddressSettings.CountryEnabled)
            {
                filterByCountryId = (await CustomerService.GetCustomerBillingAddressAsync(customer))?.CountryId ?? 0;
            }

            //model
            var paymentMethodModel = await CheckoutModelFactory.PreparePaymentMethodModelAsync(cart, filterByCountryId);

            if (PaymentSettings.BypassPaymentMethodSelectionIfOnlyOne &&
                paymentMethodModel.PaymentMethods.Count == 1 && !paymentMethodModel.DisplayRewardPoints)
            {
                //if we have only one payment method and reward points are disabled or the current customer doesn't have any reward points
                //so customer doesn't have to choose a payment method

                await GenericAttributeService.SaveAttributeAsync(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute,
                    paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName,
                    store.Id);
                return RedirectToRoute("CheckoutPaymentInfo");
            }

            return View(paymentMethodModel);
        }

        [HttpPost, ActionName("PaymentMethod")]
        [FormValueRequired("nextstep")]
        public virtual async Task<IActionResult> SelectPaymentMethod(string paymentmethod, CheckoutPaymentMethodModel model)
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (OrderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //reward points
            if (RewardPointsSettings.Enabled)
            {
                await GenericAttributeService.SaveAttributeAsync(customer,
                    NopCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, model.UseRewardPoints,
                    store.Id);
            }

            //Check whether payment workflow is required
            var isPaymentWorkflowRequired = await OrderProcessingService.IsPaymentWorkflowRequiredAsync(cart);
            if (!isPaymentWorkflowRequired)
            {
                await GenericAttributeService.SaveAttributeAsync<string>(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, null, store.Id);
                return RedirectToRoute("CheckoutPaymentInfo");
            }
            //payment method 
            if (string.IsNullOrEmpty(paymentmethod))
                return await PaymentMethod();

            if (!await PaymentPluginManager.IsPluginActiveAsync(paymentmethod, customer, store.Id))
                return await PaymentMethod();

            //save
            await GenericAttributeService.SaveAttributeAsync(customer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, paymentmethod, store.Id);

            return RedirectToRoute("CheckoutPaymentInfo");
        }

        public virtual async Task<IActionResult> PaymentInfo()
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (OrderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //Check whether payment workflow is required
            var isPaymentWorkflowRequired = await OrderProcessingService.IsPaymentWorkflowRequiredAsync(cart);
            if (!isPaymentWorkflowRequired)
            {
                return RedirectToRoute("CheckoutConfirm");
            }

            //load payment method
            var paymentMethodSystemName = await GenericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
            var paymentMethod = await PaymentPluginManager
                .LoadPluginBySystemNameAsync(paymentMethodSystemName, customer, store.Id);
            if (paymentMethod == null)
                return RedirectToRoute("CheckoutPaymentMethod");

            //Check whether payment info should be skipped
            if (paymentMethod.SkipPaymentInfo ||
                (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection && PaymentSettings.SkipPaymentInfoStepForRedirectionPaymentMethods))
            {
                //skip payment info page
                var paymentInfo = new ProcessPaymentRequest();

                //session save
                HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);

                return RedirectToRoute("CheckoutConfirm");
            }

            //model
            var model = await CheckoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);
            return View(model);
        }

        [HttpPost, ActionName("PaymentInfo")]
        [FormValueRequired("nextstep")]
        public virtual async Task<IActionResult> EnterPaymentInfo(IFormCollection form)
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (OrderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //Check whether payment workflow is required
            var isPaymentWorkflowRequired = await OrderProcessingService.IsPaymentWorkflowRequiredAsync(cart);
            if (!isPaymentWorkflowRequired)
            {
                return RedirectToRoute("CheckoutConfirm");
            }

            //load payment method
            var paymentMethodSystemName = await GenericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
            var paymentMethod = await PaymentPluginManager
                .LoadPluginBySystemNameAsync(paymentMethodSystemName, customer, store.Id);
            if (paymentMethod == null)
                return RedirectToRoute("CheckoutPaymentMethod");

            var warnings = await paymentMethod.ValidatePaymentFormAsync(form);
            foreach (var warning in warnings)
                ModelState.AddModelError("", warning);
            if (ModelState.IsValid)
            {
                //get payment info
                var paymentInfo = await paymentMethod.GetPaymentInfoAsync(form);
                //set previous order GUID (if exists)
                PaymentService.GenerateOrderGuid(paymentInfo);

                //session save
                HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);
                return RedirectToRoute("CheckoutConfirm");
            }

            //If we got this far, something failed, redisplay form
            //model
            var model = await CheckoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);
            return View(model);
        }

        public virtual async Task<IActionResult> Confirm()
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (OrderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //model
            var model = await CheckoutModelFactory.PrepareConfirmOrderModelAsync(cart);
            return View(model);
        }

        [HttpPost, ActionName("Confirm")]
        public virtual async Task<IActionResult> ConfirmOrder()
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (OrderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //model
            var model = await CheckoutModelFactory.PrepareConfirmOrderModelAsync(cart);
            try
            {
                //prevent 2 orders being placed within an X seconds time frame
                if (!await IsMinimumOrderPlacementIntervalValidAsync(customer))
                    throw new Exception(await LocalizationService.GetResourceAsync("Checkout.MinOrderPlacementInterval"));

                //place order
                var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                if (processPaymentRequest == null)
                {
                    //Check whether payment workflow is required
                    if (await OrderProcessingService.IsPaymentWorkflowRequiredAsync(cart))
                        return RedirectToRoute("CheckoutPaymentInfo");

                    processPaymentRequest = new ProcessPaymentRequest();
                }
                PaymentService.GenerateOrderGuid(processPaymentRequest);
                processPaymentRequest.StoreId = store.Id;
                processPaymentRequest.CustomerId = customer.Id;
                processPaymentRequest.PaymentMethodSystemName = await GenericAttributeService.GetAttributeAsync<string>(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
                HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", processPaymentRequest);
                var placeOrderResult = await OrderProcessingService.PlaceOrderAsync(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };
                    await PaymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

                    if (WebHelper.IsRequestBeingRedirected || WebHelper.IsPostBeingDone)
                    {
                        //redirection or POST has been done in PostProcessPayment
                        return Content(await LocalizationService.GetResourceAsync("Checkout.RedirectMessage"));
                    }

                    return RedirectToRoute("CheckoutCompleted", new { orderId = placeOrderResult.PlacedOrder.Id });
                }

                foreach (var error in placeOrderResult.Errors)
                    model.Warnings.Add(error);
            }
            catch (Exception exc)
            {
                await Logger.WarningAsync(exc.Message, exc);
                model.Warnings.Add(exc.Message);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Methods (one page checkout)

        protected virtual async Task<JsonResult> OpcLoadStepAfterShippingAddress(IList<ShoppingCartItem> cart)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var shippingMethodModel = await CheckoutModelFactory.PrepareShippingMethodModelAsync(cart, await CustomerService.GetCustomerShippingAddressAsync(customer));
            if (ShippingSettings.BypassShippingMethodSelectionIfOnlyOne &&
                shippingMethodModel.ShippingMethods.Count == 1)
            {
                var store = await StoreContext.GetCurrentStoreAsync();
                //if we have only one shipping method, then a customer doesn't have to choose a shipping method
                await GenericAttributeService.SaveAttributeAsync(customer,
                    NopCustomerDefaults.SelectedShippingOptionAttribute,
                    shippingMethodModel.ShippingMethods.First().ShippingOption,
                    store.Id);

                //load next step
                return await OpcLoadStepAfterShippingMethod(cart);
            }

            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "shipping-method",
                    html = await RenderPartialViewToStringAsync("OpcShippingMethods", shippingMethodModel)
                },
                goto_section = "shipping_method"
            });
        }

        protected virtual async Task<JsonResult> OpcLoadStepAfterShippingMethod(IList<ShoppingCartItem> cart)
        {
            //Check whether payment workflow is required
            //we ignore reward points during cart total calculation
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var isPaymentWorkflowRequired = await OrderProcessingService.IsPaymentWorkflowRequiredAsync(cart, false);
            if (isPaymentWorkflowRequired)
            {
                //filter by country
                var filterByCountryId = 0;
                if (AddressSettings.CountryEnabled)
                {
                    filterByCountryId = (await CustomerService.GetCustomerBillingAddressAsync(customer))?.CountryId ?? 0;
                }

                //payment is required
                var paymentMethodModel = await CheckoutModelFactory.PreparePaymentMethodModelAsync(cart, filterByCountryId);

                if (PaymentSettings.BypassPaymentMethodSelectionIfOnlyOne &&
                    paymentMethodModel.PaymentMethods.Count == 1 && !paymentMethodModel.DisplayRewardPoints)
                {
                    //if we have only one payment method and reward points are disabled or the current customer doesn't have any reward points
                    //so customer doesn't have to choose a payment method

                    var selectedPaymentMethodSystemName = paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName;
                    await GenericAttributeService.SaveAttributeAsync(customer,
                        NopCustomerDefaults.SelectedPaymentMethodAttribute,
                        selectedPaymentMethodSystemName, store.Id);

                    var paymentMethodInst = await PaymentPluginManager
                        .LoadPluginBySystemNameAsync(selectedPaymentMethodSystemName, customer, store.Id);
                    if (!PaymentPluginManager.IsPluginActive(paymentMethodInst))
                        throw new Exception("Selected payment method can't be parsed");

                    return await OpcLoadStepAfterPaymentMethod(paymentMethodInst, cart);
                }

                //customer have to choose a payment method
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "payment-method",
                        html = await RenderPartialViewToStringAsync("OpcPaymentMethods", paymentMethodModel)
                    },
                    goto_section = "payment_method"
                });
            }

            //payment is not required
            await GenericAttributeService.SaveAttributeAsync<string>(customer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, null, store.Id);

            var confirmOrderModel = await CheckoutModelFactory.PrepareConfirmOrderModelAsync(cart);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "confirm-order",
                    html = await RenderPartialViewToStringAsync("OpcConfirmOrder", confirmOrderModel)
                },
                goto_section = "confirm_order"
            });
        }

        protected virtual async Task<JsonResult> OpcLoadStepAfterPaymentMethod(IPaymentMethod paymentMethod, IList<ShoppingCartItem> cart)
        {
            if (paymentMethod.SkipPaymentInfo ||
                (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection && PaymentSettings.SkipPaymentInfoStepForRedirectionPaymentMethods))
            {
                //skip payment info page
                var paymentInfo = new ProcessPaymentRequest();

                //session save
                HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);

                var confirmOrderModel = await CheckoutModelFactory.PrepareConfirmOrderModelAsync(cart);
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "confirm-order",
                        html = await RenderPartialViewToStringAsync("OpcConfirmOrder", confirmOrderModel)
                    },
                    goto_section = "confirm_order"
                });
            }

            //return payment info page
            var paymenInfoModel = await CheckoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "payment-info",
                    html = await RenderPartialViewToStringAsync("OpcPaymentInfo", paymenInfoModel)
                },
                goto_section = "payment_info"
            });
        }

        public virtual async Task<IActionResult> OnePageCheckout()
        {
            //validation
            if (OrderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (!OrderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("Checkout");

            if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            var model = await CheckoutModelFactory.PrepareOnePageCheckoutModelAsync(cart);
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OpcSaveBilling(CheckoutBillingAddressModel model)
        {
            try
            {
                //validation
                if (OrderSettings.CheckoutDisabled)
                    throw new Exception(await LocalizationService.GetResourceAsync("Checkout.Disabled"));

                var customer = await WorkContext.GetCurrentCustomerAsync();
                var store = await StoreContext.GetCurrentStoreAsync();
                var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!OrderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                var form = model.Form;

                _ = int.TryParse(form["billing_address_id"], out var billingAddressId);

                if (billingAddressId > 0)
                {
                    //existing address
                    var address = await CustomerService.GetCustomerAddressAsync(customer.Id, billingAddressId)
                        ?? throw new Exception(await LocalizationService.GetResourceAsync("Checkout.Address.NotFound"));

                    customer.BillingAddressId = address.Id;
                    await CustomerService.UpdateCustomerAsync(customer);
                }
                else
                {
                    //new address
                    var newAddress = model.BillingNewAddress;

                    //custom address attributes
                    var customAttributes = await AddressAttributeParser.ParseCustomAddressAttributesAsync(form);
                    var customAttributeWarnings = await AddressAttributeParser.GetAttributeWarningsAsync(customAttributes);
                    foreach (var error in customAttributeWarnings)
                    {
                        ModelState.AddModelError("", error);
                    }

                    //validate model
                    if (!ModelState.IsValid)
                    {
                        //model is not valid. redisplay the form with errors
                        var billingAddressModel = await CheckoutModelFactory.PrepareBillingAddressModelAsync(cart,
                            selectedCountryId: newAddress.CountryId,
                            overrideAttributesXml: customAttributes);
                        billingAddressModel.NewAddressPreselected = true;
                        return Json(new
                        {
                            update_section = new UpdateSectionJsonModel
                            {
                                name = "billing",
                                html = await RenderPartialViewToStringAsync("OpcBillingAddress", billingAddressModel)
                            },
                            wrong_billing_address = true,
                        });
                    }

                    //try to find an address with the same values (don't duplicate records)
                    var address = AddressService.FindAddress((await CustomerService.GetAddressesByCustomerIdAsync(customer.Id)).ToList(),
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

                        await AddressService.InsertAddressAsync(address);

                        await CustomerService.InsertCustomerAddressAsync(customer, address);
                    }

                    customer.BillingAddressId = address.Id;

                    await CustomerService.UpdateCustomerAsync(customer);
                }

                if (await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart))
                {
                    //shipping is required
                    var address = await CustomerService.GetCustomerBillingAddressAsync(customer);

                    //by default Shipping is available if the country is not specified
                    var shippingAllowed = !AddressSettings.CountryEnabled || ((await CountryService.GetCountryByAddressAsync(address))?.AllowsShipping ?? false);
                    if (ShippingSettings.ShipToSameAddress && model.ShipToSameAddress && shippingAllowed)
                    {
                        //ship to the same address
                        customer.ShippingAddressId = address.Id;
                        await CustomerService.UpdateCustomerAsync(customer);
                        //reset selected shipping method (in case if "pick up in store" was selected)
                        await GenericAttributeService.SaveAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, store.Id);
                        await GenericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);
                        //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available) 
                        return await OpcLoadStepAfterShippingAddress(cart);
                    }

                    //do not ship to the same address
                    var shippingAddressModel = await CheckoutModelFactory.PrepareShippingAddressModelAsync(cart, prePopulateNewAddressWithCustomerFields: true);

                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "shipping",
                            html = await RenderPartialViewToStringAsync("OpcShippingAddress", shippingAddressModel)
                        },
                        goto_section = "shipping"
                    });
                }

                //shipping is not required
                await GenericAttributeService.SaveAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, store.Id);

                //load next step
                return await OpcLoadStepAfterShippingMethod(cart);
            }
            catch (Exception exc)
            {
                await Logger.WarningAsync(exc.Message, exc, await WorkContext.GetCurrentCustomerAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> OpcSaveShipping(CheckoutShippingAddressModel model)
        {
            try
            {
                //validation
                if (OrderSettings.CheckoutDisabled)
                    throw new Exception(await LocalizationService.GetResourceAsync("Checkout.Disabled"));

                var customer = await WorkContext.GetCurrentCustomerAsync();
                var store = await StoreContext.GetCurrentStoreAsync();
                var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!OrderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                if (!await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart))
                    throw new Exception("Shipping is not required");

                var form = model.Form;

                //pickup point
                if (ShippingSettings.AllowPickupInStore && !OrderSettings.DisplayPickupInStoreOnShippingMethodPage)
                {
                    var pickupInStore = ParsePickupInStore(form);
                    if (pickupInStore)
                    {
                        var pickupOption = await ParsePickupOptionAsync(form);
                        await SavePickupOptionAsync(pickupOption);

                        return await OpcLoadStepAfterShippingMethod(cart);
                    }

                    //set value indicating that "pick up in store" option has not been chosen
                    await GenericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);
                }

                _ = int.TryParse(form["shipping_address_id"], out var shippingAddressId);

                if (shippingAddressId > 0)
                {
                    //existing address
                    var address = await CustomerService.GetCustomerAddressAsync(customer.Id, shippingAddressId)
                        ?? throw new Exception(await LocalizationService.GetResourceAsync("Checkout.Address.NotFound"));

                    customer.ShippingAddressId = address.Id;
                    await CustomerService.UpdateCustomerAsync(customer);
                }
                else
                {
                    //new address
                    var newAddress = model.ShippingNewAddress;

                    //custom address attributes
                    var customAttributes = await AddressAttributeParser.ParseCustomAddressAttributesAsync(form);
                    var customAttributeWarnings = await AddressAttributeParser.GetAttributeWarningsAsync(customAttributes);
                    foreach (var error in customAttributeWarnings)
                    {
                        ModelState.AddModelError("", error);
                    }

                    //validate model
                    if (!ModelState.IsValid)
                    {
                        //model is not valid. redisplay the form with errors
                        var shippingAddressModel = await CheckoutModelFactory.PrepareShippingAddressModelAsync(cart,
                            selectedCountryId: newAddress.CountryId,
                            overrideAttributesXml: customAttributes);
                        shippingAddressModel.NewAddressPreselected = true;
                        return Json(new
                        {
                            update_section = new UpdateSectionJsonModel
                            {
                                name = "shipping",
                                html = await RenderPartialViewToStringAsync("OpcShippingAddress", shippingAddressModel)
                            }
                        });
                    }

                    //try to find an address with the same values (don't duplicate records)
                    var address = AddressService.FindAddress((await CustomerService.GetAddressesByCustomerIdAsync(customer.Id)).ToList(),
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

                        await AddressService.InsertAddressAsync(address);

                        await CustomerService.InsertCustomerAddressAsync(customer, address);
                    }

                    customer.ShippingAddressId = address.Id;

                    await CustomerService.UpdateCustomerAsync(customer);
                }

                return await OpcLoadStepAfterShippingAddress(cart);
            }
            catch (Exception exc)
            {
                await Logger.WarningAsync(exc.Message, exc, await WorkContext.GetCurrentCustomerAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> OpcSaveShippingMethod(string shippingoption, IFormCollection form)
        {
            try
            {
                //validation
                if (OrderSettings.CheckoutDisabled)
                    throw new Exception(await LocalizationService.GetResourceAsync("Checkout.Disabled"));

                var customer = await WorkContext.GetCurrentCustomerAsync();
                var store = await StoreContext.GetCurrentStoreAsync();
                var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!OrderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                if (!await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart))
                    throw new Exception("Shipping is not required");

                //pickup point
                if (ShippingSettings.AllowPickupInStore && OrderSettings.DisplayPickupInStoreOnShippingMethodPage)
                {
                    var pickupInStore = ParsePickupInStore(form);
                    if (pickupInStore)
                    {
                        var pickupOption = await ParsePickupOptionAsync(form);
                        await SavePickupOptionAsync(pickupOption);

                        return await OpcLoadStepAfterShippingMethod(cart);
                    }

                    //set value indicating that "pick up in store" option has not been chosen
                    await GenericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);
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
                var shippingOptions = await GenericAttributeService.GetAttributeAsync<List<ShippingOption>>(customer,
                    NopCustomerDefaults.OfferedShippingOptionsAttribute, store.Id);
                if (shippingOptions == null || !shippingOptions.Any())
                {
                    //not found? let's load them using shipping service
                    shippingOptions = (await ShippingService.GetShippingOptionsAsync(cart, await CustomerService.GetCustomerShippingAddressAsync(customer),
                        customer, shippingRateComputationMethodSystemName, store.Id)).ShippingOptions.ToList();
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
                await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, shippingOption, store.Id);

                //load next step
                return await OpcLoadStepAfterShippingMethod(cart);
            }
            catch (Exception exc)
            {
                await Logger.WarningAsync(exc.Message, exc, await WorkContext.GetCurrentCustomerAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> OpcSavePaymentMethod(string paymentmethod, CheckoutPaymentMethodModel model)
        {
            try
            {
                //validation
                if (OrderSettings.CheckoutDisabled)
                    throw new Exception(await LocalizationService.GetResourceAsync("Checkout.Disabled"));

                var customer = await WorkContext.GetCurrentCustomerAsync();
                var store = await StoreContext.GetCurrentStoreAsync();
                var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!OrderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                //payment method 
                if (string.IsNullOrEmpty(paymentmethod))
                    throw new Exception("Selected payment method can't be parsed");

                //reward points
                if (RewardPointsSettings.Enabled)
                {
                    await GenericAttributeService.SaveAttributeAsync(customer,
                        NopCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, model.UseRewardPoints,
                        store.Id);
                }

                //Check whether payment workflow is required
                var isPaymentWorkflowRequired = await OrderProcessingService.IsPaymentWorkflowRequiredAsync(cart);
                if (!isPaymentWorkflowRequired)
                {
                    //payment is not required
                    await GenericAttributeService.SaveAttributeAsync<string>(customer,
                        NopCustomerDefaults.SelectedPaymentMethodAttribute, null, store.Id);

                    var confirmOrderModel = await CheckoutModelFactory.PrepareConfirmOrderModelAsync(cart);
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            html = await RenderPartialViewToStringAsync("OpcConfirmOrder", confirmOrderModel)
                        },
                        goto_section = "confirm_order"
                    });
                }

                var paymentMethodInst = await PaymentPluginManager
                    .LoadPluginBySystemNameAsync(paymentmethod, customer, store.Id);
                if (!PaymentPluginManager.IsPluginActive(paymentMethodInst))
                    throw new Exception("Selected payment method can't be parsed");

                //save
                await GenericAttributeService.SaveAttributeAsync(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, paymentmethod, store.Id);

                return await OpcLoadStepAfterPaymentMethod(paymentMethodInst, cart);
            }
            catch (Exception exc)
            {
                await Logger.WarningAsync(exc.Message, exc, await WorkContext.GetCurrentCustomerAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> OpcSavePaymentInfo(IFormCollection form)
        {
            try
            {
                //validation
                if (OrderSettings.CheckoutDisabled)
                    throw new Exception(await LocalizationService.GetResourceAsync("Checkout.Disabled"));

                var customer = await WorkContext.GetCurrentCustomerAsync();
                var store = await StoreContext.GetCurrentStoreAsync();
                var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!OrderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                var paymentMethodSystemName = await GenericAttributeService.GetAttributeAsync<string>(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
                var paymentMethod = await PaymentPluginManager
                    .LoadPluginBySystemNameAsync(paymentMethodSystemName, customer, store.Id)
                    ?? throw new Exception("Payment method is not selected");

                var warnings = await paymentMethod.ValidatePaymentFormAsync(form);
                foreach (var warning in warnings)
                    ModelState.AddModelError("", warning);
                if (ModelState.IsValid)
                {
                    //get payment info
                    var paymentInfo = await paymentMethod.GetPaymentInfoAsync(form);
                    //set previous order GUID (if exists)
                    PaymentService.GenerateOrderGuid(paymentInfo);

                    //session save
                    HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);

                    var confirmOrderModel = await CheckoutModelFactory.PrepareConfirmOrderModelAsync(cart);
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            html = await RenderPartialViewToStringAsync("OpcConfirmOrder", confirmOrderModel)
                        },
                        goto_section = "confirm_order"
                    });
                }

                //If we got this far, something failed, redisplay form
                var paymenInfoModel = await CheckoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "payment-info",
                        html = await RenderPartialViewToStringAsync("OpcPaymentInfo", paymenInfoModel)
                    }
                });
            }
            catch (Exception exc)
            {
                await Logger.WarningAsync(exc.Message, exc, await WorkContext.GetCurrentCustomerAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> OpcConfirmOrder()
        {
            try
            {
                //validation
                if (OrderSettings.CheckoutDisabled)
                    throw new Exception(await LocalizationService.GetResourceAsync("Checkout.Disabled"));

                var customer = await WorkContext.GetCurrentCustomerAsync();
                var store = await StoreContext.GetCurrentStoreAsync();
                var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!OrderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                //prevent 2 orders being placed within an X seconds time frame
                if (!await IsMinimumOrderPlacementIntervalValidAsync(customer))
                    throw new Exception(await LocalizationService.GetResourceAsync("Checkout.MinOrderPlacementInterval"));

                //place order
                var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                if (processPaymentRequest == null)
                {
                    //Check whether payment workflow is required
                    if (await OrderProcessingService.IsPaymentWorkflowRequiredAsync(cart))
                    {
                        throw new Exception("Payment information is not entered");
                    }

                    processPaymentRequest = new ProcessPaymentRequest();
                }
                PaymentService.GenerateOrderGuid(processPaymentRequest);
                processPaymentRequest.StoreId = store.Id;
                processPaymentRequest.CustomerId = customer.Id;
                processPaymentRequest.PaymentMethodSystemName = await GenericAttributeService.GetAttributeAsync<string>(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
                HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", processPaymentRequest);
                var placeOrderResult = await OrderProcessingService.PlaceOrderAsync(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };

                    var paymentMethod = await PaymentPluginManager
                        .LoadPluginBySystemNameAsync(placeOrderResult.PlacedOrder.PaymentMethodSystemName, customer, store.Id);
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
                            redirect = $"{WebHelper.GetStoreLocation()}checkout/OpcCompleteRedirectionPayment"
                        });
                    }

                    await PaymentService.PostProcessPaymentAsync(postProcessPaymentRequest);
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
                        html = await RenderPartialViewToStringAsync("OpcConfirmOrder", confirmOrderModel)
                    },
                    goto_section = "confirm_order"
                });
            }
            catch (Exception exc)
            {
                await Logger.WarningAsync(exc.Message, exc, await WorkContext.GetCurrentCustomerAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        public virtual async Task<IActionResult> OpcCompleteRedirectionPayment()
        {
            try
            {
                //validation
                if (!OrderSettings.OnePageCheckoutEnabled)
                    return RedirectToRoute("Homepage");

                var customer = await WorkContext.GetCurrentCustomerAsync();
                if (await CustomerService.IsGuestAsync(customer) && !OrderSettings.AnonymousCheckoutAllowed)
                    return Challenge();

                //get the order
                var store = await StoreContext.GetCurrentStoreAsync();
                var order = (await OrderService.SearchOrdersAsync(storeId: store.Id,
                customerId: customer.Id, pageSize: 1)).FirstOrDefault();
                if (order == null)
                    return RedirectToRoute("Homepage");

                var paymentMethod = await PaymentPluginManager
                    .LoadPluginBySystemNameAsync(order.PaymentMethodSystemName, customer, store.Id);
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

                await PaymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

                if (WebHelper.IsRequestBeingRedirected || WebHelper.IsPostBeingDone)
                {
                    //redirection or POST has been done in PostProcessPayment
                    return Content(await LocalizationService.GetResourceAsync("Checkout.RedirectMessage"));
                }

                //if no redirection has been done (to a third-party payment page)
                //theoretically it's not possible
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }
            catch (Exception exc)
            {
                await Logger.WarningAsync(exc.Message, exc, await WorkContext.GetCurrentCustomerAsync());
                return Content(exc.Message);
            }
        }

        #endregion
    }
}
