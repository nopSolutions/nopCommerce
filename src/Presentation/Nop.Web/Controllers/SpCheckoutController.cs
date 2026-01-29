using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Http;
using Nop.Services.Attributes;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Checkout;
using Nop.Web.Models.Common;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Web.Controllers;

/**
 * Notes:
 * 
 * In the current OPC implementation the server decides the checkout flow,
 * since each responses includes goto_section. In the current design, on the
 * other hand, the user can interact with any part of the page. The JavasScript
 * client managees state, and decides what sections needs to be updated.
 * 
 * Checkout is treated as a shared state, whose fields gradually become known.
 * Each section is responsible for modifying one of the fields. Other sections
 * react to changes if thoey affect their own available options.
 * 
 * In modern SPAs, the rendering happens locally on the client, and components
 * re-render automatically. Meanwhile, in our case, the rendering happens on the
 * server, and the client must explicitly ask for this.
 * 
 * Each checkout section has a render endpoint, and the client-side checkout manager
 * decides when to re-render each component. Each rendering endpoint will build
 * a ViewModel based on the current checkout state, and then returns a partial view.
 * 
 * Design questions:
 * In the billing address section, should we add an option that represents
 * the null address (for example "Please select an address")?
 * In our current design, the select list is only rendered when there are
 * available addresses, and in this case no option is provided for selecting
 * a null address. Instead, the first available address is selected.
 * 
 * Handling "Ship To Same Address":
 * - When set to true for the first time, the shipping address should be updated.
 * - When true, the shipping address should change each time the billing address is changed.
 * - When true, the shipping address should change when a new billing address is added.
 * 
 * Pickup is not a shipping method. Instead, it should be treated as a shipping mode.
 * Shipping methods only exists when pickup is false.
 * 
 * I prefer having something like "Delivery Option" that could be "Shipping" or "Pickup"
 * instead of the current implementation.
 */
[AutoValidateAntiforgeryToken]
public class SpCheckoutController : BasePublicController
{
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CaptchaSettings _captchaSettings;
    protected readonly IAddressService _addressService;
    protected readonly IAttributeParser<AddressAttribute, AddressAttributeValue> _addressAttributeParser;
    protected readonly ICheckoutModelFactory _checkoutModelFactory;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderService _orderService;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPaymentService _paymentService;
    protected readonly IShippingService _shippingService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly OrderSettings _orderSettings;
    protected readonly PaymentSettings _paymentSettings;
    protected readonly RewardPointsSettings _rewardPointsSettings;
    protected readonly ShippingSettings _shippingSettings;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IPickupPluginManager _pickupPluginManager;
    private static readonly string[] _separator = ["___"];

    #endregion

    #region Ctor

    public SpCheckoutController(AddressSettings addressSettings,
        CaptchaSettings captchaSettings,
        IAddressService addressService,
        IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
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
        IShippingService shippingService,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IWebHelper webHelper,
        IWorkContext workContext,
        OrderSettings orderSettings,
        PaymentSettings paymentSettings,
        RewardPointsSettings rewardPointsSettings,
        ShippingSettings shippingSettings,
        IAddressModelFactory addressModelFactory,
        IPickupPluginManager pickupPluginManager)
    {
        _addressSettings = addressSettings;
        _captchaSettings = captchaSettings;
        _addressService = addressService;
        _addressAttributeParser = addressAttributeParser;
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
        _shippingService = shippingService;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _webHelper = webHelper;
        _workContext = workContext;
        _orderSettings = orderSettings;
        _paymentSettings = paymentSettings;
        _rewardPointsSettings = rewardPointsSettings;
        _shippingSettings = shippingSettings;
        _addressModelFactory = addressModelFactory;
        _pickupPluginManager = pickupPluginManager;
    }

    #endregion

    #region Utilities

    protected virtual async Task<bool> IsMinimumOrderPlacementIntervalValidAsync(Customer customer)
    {
        //prevent 2 orders being placed within an X seconds time frame
        if (_orderSettings.MinimumOrderPlacementInterval == 0)
            return true;

        var store = await _storeContext.GetCurrentStoreAsync();

        var lastOrder = (await _orderService.SearchOrdersAsync(storeId: store.Id,
                customerId: customer.Id, pageSize: 1))
            .FirstOrDefault();
        if (lastOrder == null)
            return true;

        var interval = DateTime.UtcNow - lastOrder.CreatedOnUtc;
        return interval.TotalMinutes > _orderSettings.MinimumOrderPlacementInterval;
    }

    protected virtual async Task<IList<string>> ValidatePaymentInfo(
    IFormCollection paymentInfoForm,
    Customer customer,
    Store store,
    IList<ShoppingCartItem> cart)
    {
        var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart);
        if (isPaymentWorkflowRequired)
        {
            var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
            var paymentMethod = await _paymentPluginManager
                                    .LoadPluginBySystemNameAsync(paymentMethodSystemName, customer, store.Id)
                                ?? throw new Exception("Payment method is not selected");

            var warnings = await paymentMethod.ValidatePaymentFormAsync(paymentInfoForm);
            foreach (var warning in warnings)
            {
                ModelState.AddModelError("", warning);
            }

            if (ModelState.IsValid)
            {
                await _orderProcessingService.SetProcessPaymentRequestAsync(await paymentMethod.GetPaymentInfoAsync(paymentInfoForm));
            }
            else
            {
                return warnings;
            }
        }

        return new List<string>();
    }

    protected virtual async Task<bool> ShippingAllowedToAddressAsync(Customer customer, int addressId)
    {
        var address = await _customerService.GetCustomerAddressAsync(customer.Id, addressId)
            ?? throw new Exception(await _localizationService.GetResourceAsync("Checkout.Address.NotFound"));

        return !_addressSettings.CountryEnabled || ((await _countryService.GetCountryByAddressAsync(address))?.AllowsShipping ?? false);
    }

    #endregion

    public virtual async Task<IActionResult> SpCheckout()
    {
        //validation
        if (_orderSettings.CheckoutDisabled)
            return RedirectToRoute(NopRouteNames.General.CART);

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        if (!cart.Any())
            return RedirectToRoute(NopRouteNames.General.CART);

        if (!_orderSettings.OnePageCheckoutEnabled)
            return RedirectToRoute(NopRouteNames.Standard.CHECKOUT);

        if (await _customerService.IsGuestAsync(customer) && !_orderSettings.AnonymousCheckoutAllowed)
            return Challenge();

        return View();
    }

    [ValidateCaptcha]
    [HttpPost]
    public virtual async Task<IActionResult> ConfirmOrder(IFormCollection paymentInfoForm, bool captchaValid)
    {
        try
        {
            // Validation

            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (await _customerService.IsGuestAsync(customer) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            // Corresponds to EnterPaymentInfo()
            var results = await ValidatePaymentInfo(paymentInfoForm, customer, store, cart);
            if (results.Any())
            {
                return Json(new
                {
                    error = 1,
                    message = results
                });
            }

            var errors = new List<string>();

            var isCaptchaSettingEnabled = await _customerService.IsGuestAsync(customer) &&
                                          _captchaSettings.Enabled && _captchaSettings.ShowOnCheckoutPageForGuests;

            //captcha validation for guest customers
            if (!isCaptchaSettingEnabled || (isCaptchaSettingEnabled && captchaValid))
            {
                //prevent 2 orders being placed within an X seconds time frame
                if (!await IsMinimumOrderPlacementIntervalValidAsync(customer))
                    throw new Exception(await _localizationService.GetResourceAsync("Checkout.MinOrderPlacementInterval"));

                // Order placement

                var processPaymentRequest = await _orderProcessingService.GetProcessPaymentRequestAsync();
                processPaymentRequest.StoreId = store.Id;
                processPaymentRequest.CustomerId = customer.Id;

                // Get the payment method from the state.
                processPaymentRequest.PaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);

                await _orderProcessingService.SetProcessPaymentRequestAsync(processPaymentRequest);

                var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);

                // Payment processing

                if (placeOrderResult.Success)
                {
                    await _orderProcessingService.SetProcessPaymentRequestAsync(null);
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };

                    var paymentRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart);

                    if (!paymentRequired)
                    {
                        // Payment workflow could be not required if order total is 0
                        return Json(new { success = 1 });
                    }

                    var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(placeOrderResult.PlacedOrder.PaymentMethodSystemName, customer, store.Id);

                    if (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
                    {
                        // An AJAX request cannot cause the browser to navigate to a new page by returning an MVC redirect. The redirect would only be followed by the XMLHttpRequest, not by the top-level window. From the user’s point of view, nothing would happen.
                        // This is not an HTTP redirect. It is just data. On the client side, the JavaScript handling the AJAX response checks whether the response contains a redirect property. If it does, the script explicitly assigns window.location.href to that URL.

                        return Json(new
                        {
                            // This intermediate page exists for a few reasons. One,
                            // it exits the one - page checkout cleanly.Once the browser navigates to that page,
                            // nop is back in a normal, non-AJAX request context where redirects work as expected.
                            redirect = $"{_webHelper.GetStoreLocation()}checkout/OpcCompleteRedirectionPayment"
                        });
                    }

                    await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

                    return Json(new { success = 1 });
                }

                foreach (var error in placeOrderResult.Errors)
                {
                    errors.Add(error);
                }
            }
            else
            {
                errors.Add(await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            return Json(new
            {
                error = 1,
                message = errors
            });
        }
        catch (Exception exc)
        {
            await _logger.WarningAsync(exc.Message, exc);
            return Json(new { error = 1, message = exc.Message });
        }
    }

    public virtual async Task<IActionResult> GetCheckoutConfiguration()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        var config = await _checkoutModelFactory.PrepareCheckoutConfigurationModelAsync(cart);
        return Json(config);
    }

    public virtual async Task<IActionResult> GetCheckoutState()
    {
        return Json(new
        {
            state = await _checkoutModelFactory.PrepareCheckoutStateModelAsync(),
            requirements = await _checkoutModelFactory.PrepareCheckoutRequirementsModelAsync(),
        });
    }

    #region State management

    /**
     * Shipping option should be reset whenever the shipping address changes.
     */
    protected virtual async Task SetShippingAddressIdAsync(int? addressId)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();

        customer.ShippingAddressId = addressId;
        await _customerService.UpdateCustomerAsync(customer);

        // Reset selected shipping method
        await _genericAttributeService.SaveAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, store.Id);
        await _genericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);
    }

    /**
     * Payment method should be reset whenever the billing address changes.
     * Also, "Ship to Same Address" should be checked, to see if the
     * shipping address need to be updated too.
     */
    protected virtual async Task SetBillingAddressIdAsync(int? addressId)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();

        customer.BillingAddressId = addressId;

        await TryMatchShippingAddress();

        await _customerService.UpdateCustomerAsync(customer);

        await _genericAttributeService.SaveAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, null, store.Id);
    }

    #endregion

    #region Rendering endpoints

    public virtual async Task<IActionResult> RenderBillingAddress()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer);
        var store = await _storeContext.GetCurrentStoreAsync();

        var model = new CheckoutBillingAddressModel();

        await _checkoutModelFactory
            .PrepareBillingAddressModelAsync(model, cart);

        model.SelectedBillingAddressId = customer.BillingAddressId;

        if (model.ShipToSameAddressAllowed)
        {
            model.ShipToSameAddress = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.ShipToSameAddressAttribute, store.Id);
        }

        return PartialView("_SpcBillingAddress", model);
    }

    public virtual async Task<IActionResult> RenderShippingAddress()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer);
        var store = await _storeContext.GetCurrentStoreAsync();

        var shipToSameAddress = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.ShipToSameAddressAttribute, store.Id);

        //if (shipToSameAddress)
        //    return PartialView("_SpcStaticMessage", "Same as billing address.");

        var model = new CheckoutShippingAddressModel();
        await _checkoutModelFactory
            .PrepareShippingAddressModelAsync(model, cart);
        model.SelectedShippingAddressId = customer.ShippingAddressId;
        model.SameAsBillingAddress = shipToSameAddress;

        return PartialView("_SpcShippingAddress", model);
    }

    public virtual async Task<IActionResult> RenderShippingMethods()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer);

        if (customer.ShippingAddressId is null)
            return PartialView("_SpcStaticMessage", "Please select a shipping address to view the available shipping methods");

        var address = await _customerService.GetCustomerAddressAsync(customer.Id, customer.ShippingAddressId.Value);

        // The prepare method already selects the previously selected method if found.
        var model = await _checkoutModelFactory
            .PrepareShippingMethodModelAsync(cart, address);

        // TODO: Handle the case when there's only a single shipping method.

        return PartialView("_SpcShippingMethods", model);
    }

    public virtual async Task<IActionResult> RenderPaymentMethods()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer);

        if (customer.BillingAddressId is null)
            return PartialView("_SpcStaticMessage", "Please select a billing address to view the available payment methods");

        var filterByCountryId = 0;
        var address = await _customerService.GetCustomerAddressAsync(customer.Id, customer.BillingAddressId.Value);
        if (_addressSettings.CountryEnabled)
            filterByCountryId = address?.CountryId ?? 0;
        
        var model = await _checkoutModelFactory
            .PreparePaymentMethodModelAsync(cart, filterByCountryId);

        return PartialView("_SpcPaymentMethods", model);
    }

    // Assumptions:
    // If this is called, then payment is required, and entering a payment info is required.
    // This means there is no need to handle this in a user-friendly way (for example, by
    // displaying a message like "Payment is not required".
    public virtual async Task<IActionResult> RenderPaymentInfo()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer);
        var store = await _storeContext.GetCurrentStoreAsync();

        // TODO: Remove.
        //var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart);
        //if (!isPaymentWorkflowRequired)
        //    return PartialView("_SpcStaticMessage", "Payment is not required.");

        var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
            NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);

        if (paymentMethodSystemName is null)
            return PartialView("_SpcStaticMessage", "Please select a payment method first.");

        var paymentMethod = await _paymentPluginManager
            .LoadPluginBySystemNameAsync(paymentMethodSystemName, customer, store.Id);

        // Check whether payment info should be skipped

        // TODO:
        // Should it be become part of the state, so that the section can be disabled
        // from the checkout manager?
        if (paymentMethod.SkipPaymentInfo ||
            (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection && _paymentSettings.SkipPaymentInfoStepForRedirectionPaymentMethods))
        {
            await _orderProcessingService.SetProcessPaymentRequestAsync(new ProcessPaymentRequest());

            return PartialView("_SpcStaticMessage", "Entering payment info is not required.");
        }

        //model
        var model = await _checkoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);

        return PartialView("_SpcPaymentInfo", model);
    }

    public virtual async Task<IActionResult> RenderConfirmOrder()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer);

        var model = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);

        return PartialView("_SpcConfirmOrder", model);
    }

    public virtual async Task<IActionResult> RenderAddressEditor(int addressId, string addressType)
    {
        //model.BillingNewAddress.CountryId = selectedCountryId;

        int? selectedCountryId = null;
        bool prePopulateNewAddressWithCustomerFields = false;
        string overrideAttributesXml = "";
        var customer = await _workContext.GetCurrentCustomerAsync();

        var model = new AddressModel();

        Address? address = null;

        if (addressId > 0)
        address = await _customerService.GetCustomerAddressAsync(customer.Id, addressId)
              ?? throw new Exception(await _localizationService.GetResourceAsync("Checkout.Address.NotFound"));

        await _addressModelFactory.PrepareAddressModelAsync(model,
            address: address,
            excludeProperties: false,
            addressSettings: _addressSettings,
            loadCountries: async () =>
            {
                if (addressType == "billing")
                    return await _countryService.GetAllCountriesForBillingAsync((await _workContext.GetWorkingLanguageAsync()).Id);
                else if (addressType == "shipping")
                    return await _countryService.GetAllCountriesForShippingAsync((await _workContext.GetWorkingLanguageAsync()).Id);
                else
                    throw new ArgumentOutOfRangeException(nameof(addressType));

            },
            prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
            customer: customer,
            overrideAttributesXml: overrideAttributesXml);

        return PartialView("_AddressModal", model);
    }

    #endregion

    #region State mutation endpoints

    /**
     * Check if "Ship To Same Address" is enabled and try to update the shipping address
     * so that it matches the billing address. If this is not possible, disable
     * shipping to the same address and return false.
     */
    protected virtual async Task<bool> TryMatchShippingAddress()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        var shipToSameAddress = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.ShipToSameAddressAttribute, store.Id);

        if (shipToSameAddress && await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
        {
            if (customer.BillingAddressId is not null)
            {
                if (!await ShippingAllowedToAddressAsync(customer, customer.BillingAddressId.Value))
                {
                    await _genericAttributeService.SaveAttributeAsync<bool>(customer, NopCustomerDefaults.ShipToSameAddressAttribute, false, store.Id);
                    return false;
                }
            }

            // Shipping is allowed or address is null. Update in both cases.
            await SetShippingAddressIdAsync(customer.BillingAddressId);
        }

        return true;
    }

    [HttpPost]
    public virtual async Task<IActionResult> SelectShippingAddress([FromBody] UpdateCheckoutStateRequestModel request)
    {
        try
        {
            await SetShippingAddressIdAsync(request.ShippingAddressId);

            return Json(new
            {
                state = await _checkoutModelFactory.PrepareCheckoutStateModelAsync(),
                requirements = await _checkoutModelFactory.PrepareCheckoutRequirementsModelAsync(),
            });
        }
        catch (Exception exc)
        {
            await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentCustomerAsync());
            return Json(new { error = 1, message = exc.Message });
        }
    }

    [HttpPost]
    public virtual async Task<IActionResult> SelectBillingAddress([FromBody] UpdateCheckoutStateRequestModel request)
    {
        try
        {
            await SetBillingAddressIdAsync(request.BillingAddressId);

            return Json(new
            {
                state = await _checkoutModelFactory.PrepareCheckoutStateModelAsync(),
                requirements = await _checkoutModelFactory.PrepareCheckoutRequirementsModelAsync(),
            });
        }
        catch (Exception exc)
        {
            await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentCustomerAsync());
            return Json(new { error = 1, message = exc.Message });
        }
    }

    [HttpPost]
    public virtual async Task<IActionResult> SelectShippingMethod([FromBody] UpdateCheckoutStateRequestModel request)
    {
        try
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (string.IsNullOrEmpty(request.ShippingOption))
                throw new Exception("Selected shipping method can't be parsed");

            var splittedOption = request.ShippingOption.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
            if (splittedOption.Length != 2)
                throw new Exception("Selected shipping method can't be parsed");
            var selectedName = splittedOption[0];
            var shippingRateComputationMethodSystemName = splittedOption[1];

            //find it
            //performance optimization. try cache first
            var shippingOptions = await _genericAttributeService.GetAttributeAsync<List<ShippingOption>>(customer,
                NopCustomerDefaults.OfferedShippingOptionsAttribute, store.Id);
            if (shippingOptions == null || !shippingOptions.Any())
            {
                //not found? let's load them using shipping service
                shippingOptions = (await _shippingService.GetShippingOptionsAsync(cart, await _customerService.GetCustomerShippingAddressAsync(customer),
                    customer, shippingRateComputationMethodSystemName, store.Id)).ShippingOptions.ToList();
            }
            else
            {
                //loaded cached results. let's filter result by a chosen shipping rate computation method
                shippingOptions = shippingOptions.Where(so => so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }

            var shippingOption = shippingOptions.Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase))
                                 ?? throw new Exception("Selected shipping method can't be loaded");

            //save
            await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, shippingOption, store.Id);

            return Json(new
            {
                state = await _checkoutModelFactory.PrepareCheckoutStateModelAsync(),
                requirements = await _checkoutModelFactory.PrepareCheckoutRequirementsModelAsync(),
            });
        }
        catch (Exception exc)
        {
            await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentCustomerAsync());
            return Json(new { error = 1, message = exc.Message });
        }
    }

    [HttpPost]
    public virtual async Task<IActionResult> SelectPaymentMethod([FromBody] UpdateCheckoutStateRequestModel request)
    {
        try
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            // TODO: Handle reward points

            //if (_rewardPointsSettings.Enabled)
            //{
            //    await _genericAttributeService.SaveAttributeAsync(customer,
            //        NopCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, model.UseRewardPoints,
            //        store.Id);
            //}

            var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart);
            if (!isPaymentWorkflowRequired)
            {
                // if payment is not required, keep the method null.
                await _genericAttributeService.SaveAttributeAsync<string>(customer,
                  NopCustomerDefaults.SelectedPaymentMethodAttribute, null, store.Id);

                /** IMPORTANT:
                 * In the standard checkout, a null value meant that payment is not required.
                 * However, in the current workflow, a null value could mean that the user hasn't
                 * yet chosen the method (because the billing address is not selected yet for instance).
                 * Confirming an order wasn't possible in the previous workflow without choosing a method,
                 * but it is possible in the new one. To handle this, additional checks were added
                 * inside the new ConfirmOrder.
                 */

                return Json(new
                {
                    state = await _checkoutModelFactory.PrepareCheckoutStateModelAsync(),
                    requirements = await _checkoutModelFactory.PrepareCheckoutRequirementsModelAsync(),
                });
            }

            var paymentMethodInst = await _paymentPluginManager
                .LoadPluginBySystemNameAsync(request.PaymentMethodSystemName, customer, store.Id);

            if (!_paymentPluginManager.IsPluginActive(paymentMethodInst))
                throw new Exception("Selected payment method can't be parsed");

            // Save

            await _genericAttributeService.SaveAttributeAsync(customer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, request.PaymentMethodSystemName, store.Id);

            return Json(new
            {
                state = await _checkoutModelFactory.PrepareCheckoutStateModelAsync(),
                requirements = await _checkoutModelFactory.PrepareCheckoutRequirementsModelAsync(),
            });
        }
        catch (Exception exc)
        {
            await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentCustomerAsync());
            return Json(new { error = 1, message = exc.Message });
        }
    }

    [HttpPost]
    public virtual async Task<IActionResult> ToggleShipToSameAddress([FromBody] UpdateCheckoutStateRequestModel request)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        if (!_shippingSettings.ShipToSameAddress)
        {
            await _genericAttributeService.SaveAttributeAsync<bool>(customer, NopCustomerDefaults.ShipToSameAddressAttribute, false, store.Id);
            return Json(new
            {
                error = 1,
                message = "Shipping to the same address is not enabled",
                state = await _checkoutModelFactory.PrepareCheckoutStateModelAsync(),
                requirements = await _checkoutModelFactory.PrepareCheckoutRequirementsModelAsync(),
            });
        }

        if (request.ShipToSameAddress)
        {
            if (customer.BillingAddressId is not null)
            {
                if (!await ShippingAllowedToAddressAsync(customer, customer.BillingAddressId.Value))
                {
                    await _genericAttributeService.SaveAttributeAsync<bool>(customer, NopCustomerDefaults.ShipToSameAddressAttribute, false, store.Id);
                    return Json(new
                    {
                        error = 1,
                        message = "Shipping to this address is not supported",
                        state = await _checkoutModelFactory.PrepareCheckoutStateModelAsync(),
                        requirements = await _checkoutModelFactory.PrepareCheckoutRequirementsModelAsync(),
                    });
                }
            }

            // Shipping is allowed or address is null. Update in both cases.
            await SetShippingAddressIdAsync(customer.BillingAddressId);
        }

        // If shipping to the same address is disabled, don't modify the current shipping
        // address, and leave it as it is.
        await _genericAttributeService.SaveAttributeAsync<bool>(customer, NopCustomerDefaults.ShipToSameAddressAttribute, request.ShipToSameAddress, store.Id);

        return Json(new
        {
            state = await _checkoutModelFactory.PrepareCheckoutStateModelAsync(),
            requirements = await _checkoutModelFactory.PrepareCheckoutRequirementsModelAsync(),
        });
    }

    // In the previous implementation, we had to parse the "Pickup in Store" input toggle
    // when the form is submitted. We don't need this parsing anymore.
    [HttpPost]
    public virtual async Task<IActionResult> TogglePickupInStore([FromBody] UpdateCheckoutStateRequestModel request)
    {
        try
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (request.PickupInStore)
            {
                // How is "Pickup in Store" represented internally?
                // It is represented by a non-null shipping option whose property
                // IsPickupInStore is set to true.

                // In order to create a shipping option for "Pickup in Store" we have to select a pickup point.

                var pickupPointProviders = await _pickupPluginManager.LoadActivePluginsAsync(customer, store.Id);
                if (pickupPointProviders.Any())
                {
                    var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
                    var address = customer.BillingAddressId.HasValue
                        ? await _addressService.GetAddressByIdAsync(customer.BillingAddressId.Value)
                        : null;

                    var pickupPointsResponse = await _shippingService.GetPickupPointsAsync(cart, address,
                        customer, storeId: store.Id);
                    if (pickupPointsResponse.Success)
                    {
                        var pickupPoint = pickupPointsResponse.PickupPoints.FirstOrDefault();

                        var name = !string.IsNullOrEmpty(pickupPoint.Name) ?
                            string.Format(await _localizationService.GetResourceAsync("Checkout.PickupPoints.Name"), pickupPoint.Name) :
                            await _localizationService.GetResourceAsync("Checkout.PickupPoints.NullName");

                        var pickUpInStoreShippingOption = new ShippingOption
                        {
                            Name = name,
                            Rate = pickupPoint.PickupFee,
                            Description = pickupPoint.Description,
                            ShippingRateComputationMethodSystemName = pickupPoint.ProviderSystemName,
                            IsPickupInStore = true
                        };

                        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, pickUpInStoreShippingOption, store.Id);
                        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedPickupPointAttribute, pickupPoint, store.Id);
                    }
                }
            }
            else
            {
                await _genericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, store.Id);

                //set value indicating that "pick up in store" option has not been chosen
                await _genericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);
            }

            return Json(new
            {
                state = await _checkoutModelFactory.PrepareCheckoutStateModelAsync(),
                requirements = await _checkoutModelFactory.PrepareCheckoutRequirementsModelAsync(),
            });
        }
        catch (Exception exc)
        {
            await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentCustomerAsync());
            return Json(new { error = 1, message = exc.Message });
        }
    }

    // In the previous implementation, we had to parse the pickup point when the form is submitted.
    // We don't need this parsing anymore.
    [HttpPost]
    public virtual async Task<IActionResult> SelectPickupPoint([FromBody] UpdateCheckoutStateRequestModel request)
    {
        try
        {
            // Saving a pickup point requires editing the shipping option
            // that represents "Pickup in Store".

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            var pickupPoint = request.PickupPoint.ToString().Split(_separator, StringSplitOptions.None);

            var address = customer.BillingAddressId.HasValue
                ? await _addressService.GetAddressByIdAsync(customer.BillingAddressId.Value)
                : null;

            var selectedPoint = (await _shippingService.GetPickupPointsAsync(cart, address,
                                    customer, pickupPoint[1], store.Id)).PickupPoints.FirstOrDefault(x => x.Id.Equals(pickupPoint[0]))
                                ?? throw new Exception("Pickup point is not allowed");

            var pickUpInStoreShippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);

            if (pickUpInStoreShippingOption is not null)
            {
                var name = !string.IsNullOrEmpty(selectedPoint.Name) ?
                    string.Format(await _localizationService.GetResourceAsync("Checkout.PickupPoints.Name"), selectedPoint.Name) :
                    await _localizationService.GetResourceAsync("Checkout.PickupPoints.NullName");

                pickUpInStoreShippingOption.Name = name;
                pickUpInStoreShippingOption.Rate = selectedPoint.PickupFee;
                pickUpInStoreShippingOption.Description = selectedPoint.Description;
                pickUpInStoreShippingOption.ShippingRateComputationMethodSystemName = selectedPoint.ProviderSystemName;

                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, pickUpInStoreShippingOption, store.Id);
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedPickupPointAttribute, pickupPoint, store.Id);
            }

            return Json(new
            {
                state = await _checkoutModelFactory.PrepareCheckoutStateModelAsync(),
                requirements = await _checkoutModelFactory.PrepareCheckoutRequirementsModelAsync(),
            });
        }
        catch (Exception exc)
        {
            await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentCustomerAsync());
            return Json(new { error = 1, message = exc.Message });
        }
    }

    #endregion

    #region Address modification

    // The model is used for the traditional properties, while the form
    // captures all the custom attributes
    public virtual async Task<IActionResult> SaveEditBillingAddress(
        AddressModel model,
        IFormCollection form)
    {
        return await EditAddressAsync(model, form, async (customer, address) =>
        {
            /**
             * When editing an existing address, the following call
             * will not lead to a change in the checkout state, since
             * the address is already selected in the select list.
             * However, if we're creating a new address, then first we
             * want to select the newly created address. Second, if we
             * had 0 addresses before, then we want to select the newly
             * created address as a shipping address too. That's why we're
             * repreparing the state.
             *
             * Alternatively, we could create a endpoint for editing an
             * address (without changing the state), and a separate endpoint
             * for adding an address (which will change at least on of the
             * the addresses).
            */
            await SetBillingAddressIdAsync(address.Id);

            return Json(new { state = await _checkoutModelFactory.PrepareCheckoutStateModelAsync() });
        });
    }

    public virtual async Task<IActionResult> SaveEditShippingAddress(
        AddressModel model,
        IFormCollection form)
    {
        return await EditAddressAsync(model, form, async (customer, address) =>
        {
            await SetShippingAddressIdAsync(address.Id);

            return Json(new { state = await _checkoutModelFactory.PrepareCheckoutStateModelAsync() });
        });
    }

    // TODO: Rename to something like SaveAddress or AddOrUpdateAddress
    protected virtual async Task<JsonResult> EditAddressAsync(
        AddressModel addressModel,
        IFormCollection form,
        Func<Customer, Address, Task<JsonResult>> getResult)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.Where(p => p.Errors.Any()).SelectMany(p => p.Errors)
                    .Select(p => p.ErrorMessage));

                throw new Exception(errors);
            }

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            if (!cart.Any())
                throw new Exception("Your cart is empty");

            Address address = null;

            if (addressModel.Id > 0)
            {
                //find address (ensure that it belongs to the current customer)
                address = await _customerService.GetCustomerAddressAsync(customer.Id, addressModel.Id)
                              ?? throw new Exception("Address can't be loaded");

                //custom address attributes
                var customAttributes = await _addressAttributeParser.ParseCustomAttributesAsync(form, NopCommonDefaults.AddressAttributeControlName);
                var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);

                if (customAttributeWarnings.Any())
                    return Json(new { error = 1, message = customAttributeWarnings });

                address = addressModel.ToEntity(address);
                address.CustomAttributes = customAttributes;

                await _addressService.UpdateAddressAsync(address);
            }
            else // Originally in OpcSaveBilling()
            {
                // TODO: Handle VAT.

                //if (await _customerService.IsGuestAsync(customer) && _taxSettings.EuVatEnabled && _taxSettings.EuVatEnabledForGuests)
                //{
                //    var warning = await SaveCustomerVatNumberAsync(model.VatNumber, customer);
                //    if (!string.IsNullOrEmpty(warning))
                //        ModelState.AddModelError("", warning);
                //}

                //new address
                var newAddress = addressModel;

                //custom address attributes
                var customAttributes = await _addressAttributeParser.ParseCustomAttributesAsync(form, NopCommonDefaults.AddressAttributeControlName);
                var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);

                if (customAttributeWarnings.Any())
                    return Json(new { error = 1, message = customAttributeWarnings });

                //try to find an address with the same values (don't duplicate records)
                address = _addressService.FindAddress((await _customerService.GetAddressesByCustomerIdAsync(customer.Id)).ToList(),
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

                    await _addressService.InsertAddressAsync(address);

                    await _customerService.InsertCustomerAddressAsync(customer, address);
                }
            }

            return await getResult(customer, address);
        }
        catch (Exception exc)
        {
            await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentCustomerAsync());
            return Json(new { error = 1, message = exc.Message });
        }
    }

    /**
     * Since this implementation updates the selected address in real-time,
     * deleting an address is a state changing operation, since the current selected
     * address will no longer be valid.
     */
    [HttpPost]
    public virtual async Task<IActionResult> DeleteAddress([FromBody] int addressId)
    {
        try
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            if (!cart.Any())
                throw new Exception("Your cart is empty");

            var address = await _customerService.GetCustomerAddressAsync(customer.Id, addressId);
            if (address != null)
            {
                // This will reset the selected addresses.
                await _customerService.RemoveCustomerAddressAsync(customer, address);
                await _customerService.UpdateCustomerAsync(customer);

                await _addressService.DeleteAddressAsync(address);
            }

            var state = await _checkoutModelFactory.PrepareCheckoutStateModelAsync();
            return Json(new
            {
                state = await _checkoutModelFactory.PrepareCheckoutStateModelAsync(),
                requirements = await _checkoutModelFactory.PrepareCheckoutRequirementsModelAsync(),
            });
        }
        catch (Exception exc)
        {
            await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentCustomerAsync());
            return Json(new { error = 1, message = exc.Message });
        }
    }

    /// <summary>
    /// Get specified Address by addresId
    /// </summary>
    /// <param name="addressId"></param>
    public virtual async Task<IActionResult> GetAddressById(int addressId)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        Address address = null;

        if (addressId != 0)
        {
            address = await _customerService.GetCustomerAddressAsync(customer.Id, addressId);
            ArgumentNullException.ThrowIfNull(address);
        }

        var addressModel = new AddressModel();

        await _addressModelFactory.PrepareAddressModelAsync(addressModel,
            address: address,
            excludeProperties: false,
            addressSettings: _addressSettings,
            prePopulateWithCustomerFields: true,
            customer: customer);

        var json = JsonConvert.SerializeObject(addressModel, Formatting.Indented,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

        return Content(json, "application/json");
    }

    #endregion
}