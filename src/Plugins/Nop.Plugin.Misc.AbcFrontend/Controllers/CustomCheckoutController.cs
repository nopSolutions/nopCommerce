using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using System.Linq;
using Nop.Services.Catalog;
using System.Collections.Generic;
using Nop.Services.Payments;
using System;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Payments;
using Nop.Services.Common;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using Nop.Services.Directory;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Core.Domain.Catalog;
using System.Text;
using System.Xml;
using System.Configuration;
using Nop.Services.Configuration;
using System.Net;
using Nop.Plugin.Misc.AbcFrontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Models.Checkout;
// needed for HttpContext
using Nop.Core.Http.Extensions;
// needed for .ToEntity()
using Nop.Web.Extensions;
using Nop.Plugin.Misc.AbcCore;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Plugin.Misc.AbcFrontend.Services;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Plugin.Misc.AbcExportOrder.Services;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcFrontend.Controllers
{
    public class CustomCheckoutController : CheckoutController
    {
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
        private readonly ICustomOrderService _orderService;
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
        private readonly CoreSettings _coreSettings;

        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;

        private readonly ISettingService _settingService;
        private readonly IGiftCardService _giftCardService;
        private readonly IIsamGiftCardService _isamGiftCardService;
        private readonly IWarrantyService _warrantyService;
        private readonly ITermLookupService _termLookupService;
        private readonly ICardCheckService _cardCheckService;

        public CustomCheckoutController(
            AddressSettings addressSettings,
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
            ICustomOrderService orderService,
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
            ShippingSettings shippingSettings,
            CoreSettings coreSettings,
            IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,
            ISettingService settingService,
            IGiftCardService giftCardService,
            IIsamGiftCardService isamGiftCardService,
            IWarrantyService warrantyService,
            ITermLookupService termLookupService,
            ICardCheckService cardCheckService
        ) : base(addressSettings, customerSettings, addressAttributeParser, 
            addressService, checkoutModelFactory, countryService, customerService,
            genericAttributeService, localizationService, logger, orderProcessingService,
            orderService, paymentPluginManager, paymentService, productService,
            shippingService, shoppingCartService, storeContext, webHelper,
            workContext, orderSettings, paymentSettings, rewardPointsSettings,
            shippingSettings)
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
            _coreSettings = coreSettings;

            _productAttributeService = productAttributeService;
            _productAttributeParser = productAttributeParser;
            _settingService = settingService;
            _giftCardService = giftCardService;
            _isamGiftCardService = isamGiftCardService;
            _warrantyService = warrantyService;
            _termLookupService = termLookupService;
            _cardCheckService = cardCheckService;
        }

        #region Methods (one page checkout)
        private async Task<string> SendExternalShippingMethodRequestAsync()
        {
            var defaultTransPromo = (await _settingService.GetSettingAsync("ordersettings.defaulttranspromo"))?.Value;
            if (defaultTransPromo == "101")
            {
                await _logger.WarningAsync("DefaultTransPromo is 101, term lookup skipped");
                return string.Empty;
            }

            try
            {
                var cart = await _shoppingCartService.GetShoppingCartAsync(
                    await _workContext.GetCurrentCustomerAsync(),
                    ShoppingCartType.ShoppingCart,
                    (await _storeContext.GetCurrentStoreAsync()).Id);

                if (cart.Any())
                {
                    var termLookup = await _termLookupService.GetTermAsync(cart);
                    HttpContext.Session.Set("TransPromo", termLookup.termNo ?? defaultTransPromo);
                    HttpContext.Session.SetString("TransDescription", $"{termLookup.description} {termLookup.link}");
                }
            }
            catch (IsamException e)
            {
                await _logger.ErrorAsync("Failure occurred during ISAM Term Lookup", e);
                HttpContext.Session.SetString("TransPromo", defaultTransPromo);
            }
            return "";
        }

        private async Task<(string status_code, string response_message)> SendPaymentRequestAsync(ProcessPaymentRequest paymentInfo)
        {
            var status_code = "";
            var response_message = "";

            var cart = await _shoppingCartService.GetShoppingCartAsync(
                    await _workContext.GetCurrentCustomerAsync(),
                    ShoppingCartType.ShoppingCart,
                    (await _storeContext.GetCurrentStoreAsync()).Id);
            var customer = await _customerService.GetCustomerByIdAsync(
                cart.FirstOrDefault().CustomerId
            );
            var billingAddress = await _customerService.GetCustomerBillingAddressAsync(
                customer
            );
            var domain = (await _storeContext.GetCurrentStoreAsync()).Url;
            var ip = _webHelper.GetCurrentIpAddress();

            try
            {
                var cardCheck = await _cardCheckService.CheckCardAsync(
                    paymentInfo,
                    billingAddress,
                    domain,
                    ip
                );

                HttpContext.Session.Set("Auth_No", cardCheck.AuthNo ?? "");
                HttpContext.Session.SetString("Ref_No", cardCheck.RefNo ?? "");
                status_code = cardCheck.StatusCode ?? "00";
                response_message = cardCheck.ResponseMessage;
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync("Error occurred when making external payment request. Setting status code to 00 and Ref_No Auth_No to null.", e);
                status_code = "00";
                HttpContext.Session.SetString("Ref_No", "");
                HttpContext.Session.Set("Auth_No", "");
            }

            return (status_code, response_message);
        }

        private async Task ValidateGiftCardAmountsAsync()
        {
            // grab every gift card that the customer applied to this order
            IList<GiftCard> appliedGiftCards = await _giftCardService.GetActiveGiftCardsAppliedByCustomerAsync(
                await _workContext.GetCurrentCustomerAsync()
            );

            if (appliedGiftCards.Count > 1)
            {
                throw new Exception("Only one gift card may be applied to an order");
            }

            foreach (GiftCard nopGiftCard in appliedGiftCards)
            {
                // check isam to make sure each gift card has the right $$
                GiftCard isamGiftCard = _isamGiftCardService.GetGiftCardInfo(nopGiftCard.GiftCardCouponCode).GiftCard;

                decimal nopAmtLeft = nopGiftCard.Amount;
                List<GiftCardUsageHistory> nopGcHistory = (await _giftCardService.GetGiftCardUsageHistoryAsync(nopGiftCard)).ToList();

                foreach (var history in nopGcHistory)
                {
                    nopAmtLeft -= history.UsedValue;
                }

                if (isamGiftCard.Amount != nopAmtLeft)
                {
                    throw new Exception("A gift card has been used since it was placed on this order");

                }
            }
        }

        #endregion

        #region Utilities
        [NonAction]
        private async Task<IDictionary<ShoppingCartItem, List<ProductAttributeValue>>> GetWarrantiesAsync(
            IList<ShoppingCartItem> cart
        )
        {
            var warranties =
                new Dictionary<ShoppingCartItem, List<ProductAttributeValue>>();
            foreach (var sci in cart)
            {
                var mappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(
                        sci.ProductId
                    );
                var warrantyMappings = new List<ProductAttributeMapping>();

                foreach (var mapping in mappings)
                {
                    var pa = await _productAttributeService.GetProductAttributeByIdAsync(mapping.ProductAttributeId);

                    if (pa != null && pa.Name == "Warranty")
                    {
                        warrantyMappings.Add(mapping);
                    }
                }

                var options = new List<ProductAttributeValue>();
                foreach (var mapping in warrantyMappings)
                {
                    options.AddRange(
                        await _productAttributeService.GetProductAttributeValuesAsync(mapping.Id));
                }
                if (options.Any())
                {
                    warranties.Add(sci, options);
                }
            }

            return warranties;
        }

        #endregion

        public override async Task<IActionResult> ShippingMethod()
        {
            var cart = await _shoppingCartService.GetShoppingCartAsync(
                await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart,
                (await _storeContext.GetCurrentStoreAsync()).Id
            );
            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");
            if (await _customerService.IsGuestAsync(
                await _workContext.GetCurrentCustomerAsync()) && !_orderSettings.AnonymousCheckoutAllowed)
            {
                return Challenge();
            }

            var model = await _checkoutModelFactory.PrepareShippingMethodModelAsync(
                cart,
                await _customerService.GetCustomerShippingAddressAsync(await _workContext.GetCurrentCustomerAsync())
            );

            // this will blow up if there's more than one, which is how ABC is
            // currently set up.
            var shippingMethod = model.ShippingMethods.Single();
            if (shippingMethod.Fee == "$0.00")
            {
                await _genericAttributeService.SaveAttributeAsync(
                    await _workContext.GetCurrentCustomerAsync(),
                    NopCustomerDefaults.SelectedShippingOptionAttribute,
                    shippingMethod.ShippingOption,
                    (await _storeContext.GetCurrentStoreAsync()).Id
                );
                await SendExternalShippingMethodRequestAsync();
                return RedirectToRoute("CheckoutPaymentMethod");
            }
                
            return await base.ShippingMethod();
        }

        // Makes an external request
        [HttpPost, ActionName("ShippingMethod")]
        [FormValueRequired("nextstep")]
        public override async Task<IActionResult> SelectShippingMethod(string shippingoption, IFormCollection form)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStoreAsync()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            if (!(await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart)))
            {
                await _genericAttributeService.SaveAttributeAsync<ShippingOption>(await _workContext.GetCurrentCustomerAsync(),
                    NopCustomerDefaults.SelectedShippingOptionAttribute, null, (await _storeContext.GetCurrentStoreAsync()).Id);
                return RedirectToRoute("CheckoutPaymentMethod");
            }

            //pickup point
            if (_shippingSettings.AllowPickupInStore && _orderSettings.DisplayPickupInStoreOnShippingMethodPage)
            {
                var pickupInStore = ParsePickupInStore(form);
                if (pickupInStore)
                {
                    var pickupOption = await ParsePickupOptionAsync(form);
                    await SavePickupOptionAsync(pickupOption);

                    return RedirectToRoute("CheckoutPaymentMethod");
                }

                //set value indicating that "pick up in store" option has not been chosen
                await _genericAttributeService.SaveAttributeAsync<PickupPoint>(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.SelectedPickupPointAttribute, null, (await _storeContext.GetCurrentStoreAsync()).Id);
            }

            //parse selected method 
            if (string.IsNullOrEmpty(shippingoption))
                return RedirectToAction("ShippingMethod");
            var splittedOption = shippingoption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedOption.Length != 2)
                return RedirectToAction("ShippingMethod");
            var selectedName = splittedOption[0];
            var shippingRateComputationMethodSystemName = splittedOption[1];

            //find it
            //performance optimization. try cache first
            var shippingOptions = await _genericAttributeService.GetAttributeAsync<List<ShippingOption>>(await _workContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.OfferedShippingOptionsAttribute, (await _storeContext.GetCurrentStoreAsync()).Id);
            if (shippingOptions == null || !shippingOptions.Any())
            {
                //not found? let's load them using shipping service
                shippingOptions = (await _shippingService.GetShippingOptionsAsync(cart, await _customerService.GetCustomerShippingAddressAsync(await _workContext.GetCurrentCustomerAsync()),
                    await _workContext.GetCurrentCustomerAsync(), shippingRateComputationMethodSystemName, (await _storeContext.GetCurrentStoreAsync()).Id)).ShippingOptions.ToList();
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
                return RedirectToAction("ShippingMethod");

            //save
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.SelectedShippingOptionAttribute, shippingOption, (await _storeContext.GetCurrentStoreAsync()).Id);

            await SendExternalShippingMethodRequestAsync();

            return RedirectToRoute("CheckoutPaymentMethod");
        }

        public async Task<IActionResult> WarrantySelection()
        {
            var cart = await _shoppingCartService.GetShoppingCartAsync(
                await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart,
                (await _storeContext.GetCurrentStoreAsync()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            return View(await GetWarrantiesAsync(cart));
        }

        [HttpPost, ActionName("WarrantySelection")]
        [FormValueRequired("nextstep")]
        public async Task<IActionResult> SelectWarranty(IFormCollection form)
        {
            var cart = await _shoppingCartService.GetShoppingCartAsync(
                await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart,
                (await _storeContext.GetCurrentStoreAsync()).Id);

            await SaveWarranty(cart, form);

            return RedirectToRoute("CheckoutPaymentMethod");
        }

        private async Task SaveWarranty(IList<ShoppingCartItem> cart, IFormCollection form)
        {
            foreach (var keyValue in await GetWarrantiesAsync(cart))
            {
                var value = form[keyValue.Key.Id.ToString()];
                var sci = keyValue.Key;

                //Remove currently selected warranty
                var pavs = await _productAttributeParser.ParseProductAttributeValuesAsync(sci.AttributesXml);
                List<ProductAttributeValue> warranties = new List<ProductAttributeValue>();

                foreach (var pav in pavs)
                {
                    var pam = await _productAttributeService.GetProductAttributeMappingByIdAsync(pav.ProductAttributeMappingId);
                    var pa = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);

                    var isWarranty = pa.Name == "Warranty";
                    if (isWarranty)
                    {
                        warranties.Add(pav);
                    }
                }

                if (warranties.Count > 0)
                {
                    var pams = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(
                            (await _productService.GetProductByIdAsync(sci.ProductId)).Id));
                    ProductAttributeMapping warrantyPam = null;

                    foreach (var pam in pams)
                    {
                        var isWarranty = (await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId)).Name == "Warranty";

                        if (isWarranty)
                        {
                            warrantyPam = pam;
                            continue;
                        }
                    }

                    sci.AttributesXml =
                        _productAttributeParser.RemoveProductAttribute(sci.AttributesXml, warrantyPam);
                }

                if (value != "NoWarranty")
                {
                    var pav = await _productAttributeService.GetProductAttributeValueByIdAsync(
                        int.Parse(value));

                    sci.AttributesXml = _productAttributeParser.AddProductAttribute(
                        sci.AttributesXml, await _productAttributeService.GetProductAttributeMappingByIdAsync(pav.ProductAttributeMappingId), pav.Id.ToString());
                }

                // get checkout state before it gets deleted

                // hold OfferedShippingOptions
                var shippingOptions =
                    await _genericAttributeService.GetAttributeAsync<List<ShippingOption>>(
                        await _workContext.GetCurrentCustomerAsync(),
                        NopCustomerDefaults.OfferedShippingOptionsAttribute,
                        (await _storeContext.GetCurrentStoreAsync()).Id);

                // hold selected shipping option
                var selectedShippingOption =
                    await _genericAttributeService.GetAttributeAsync<ShippingOption>(
                        await _workContext.GetCurrentCustomerAsync(),
                        NopCustomerDefaults.SelectedShippingOptionAttribute,
                        (await _storeContext.GetCurrentStoreAsync()).Id);

                // hold SelectedPaymentMethod
                //find a selected (previously) payment method
                var selectedPaymentMethodSystemName =
                    await _genericAttributeService.GetAttributeAsync<string>(
                        await _workContext.GetCurrentCustomerAsync(),
                        NopCustomerDefaults.SelectedPaymentMethodAttribute,
                        (await _storeContext.GetCurrentStoreAsync()).Id);

                // updating shopping cart resets the order state, so we are going to save the state, and re-initialize it after the reset
                await _shoppingCartService.UpdateShoppingCartItemAsync(
                    await _customerService.GetCustomerByIdAsync(sci.CustomerId),
                    sci.Id,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity);

                // re-add shopping cart state to what it was before

                // OfferedShippingOptions
                await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                                                       NopCustomerDefaults.OfferedShippingOptionsAttribute,
                                                       shippingOptions,
                                                       (await _storeContext.GetCurrentStoreAsync()).Id);

                // SelectedShippingOption
                await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                    NopCustomerDefaults.SelectedShippingOptionAttribute,
                    selectedShippingOption,
                    (await _storeContext.GetCurrentStoreAsync()).Id);

                // SelectedPaymentMethod
                await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                                                       NopCustomerDefaults.SelectedPaymentMethodAttribute,
                                                       selectedPaymentMethodSystemName,
                                                       (await _storeContext.GetCurrentStoreAsync()).Id);
            }
        }

        // Custom - uses CC_REF_NO and AUTH_CODE
        [HttpPost, ActionName("Confirm")]
        public override async Task<IActionResult> ConfirmOrder()
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCartAsync(
                await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart,
                (await _storeContext.GetCurrentStoreAsync()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            await ValidateGiftCardAmountsAsync();

            //model
            var model = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);
            try
            {
                //prevent 2 orders being placed within an X seconds time frame
                if (!(await IsMinimumOrderPlacementIntervalValidAsync(await _workContext.GetCurrentCustomerAsync())))
                    throw new Exception(await _localizationService.GetResourceAsync( "Checkout.MinOrderPlacementInterval"));

                //place order
                var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                if (processPaymentRequest == null)
                {
                    //Check whether payment workflow is required
                    if (await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart))
                        return RedirectToRoute("CheckoutPaymentInfo");

                    processPaymentRequest = new ProcessPaymentRequest();
                }

                _paymentService.GenerateOrderGuid(processPaymentRequest);
                processPaymentRequest.StoreId = (await _storeContext.GetCurrentStoreAsync()).Id;
                processPaymentRequest.CustomerId = (await _workContext.GetCurrentCustomerAsync()).Id;
                processPaymentRequest.PaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetCurrentCustomerAsync(),
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, (await _storeContext.GetCurrentStoreAsync()).Id);
                HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", processPaymentRequest);

                // Set ABC custom values
                var refNo = HttpContext.Session.GetString("Ref_No");
                if (refNo != null)
                {
                    processPaymentRequest.CustomValues.Add("CC_REFNO", refNo);
                    HttpContext.Session.Remove("Ref_No");
                }
                var authNo = HttpContext.Session.GetString("Auth_No");
                if (authNo != null)
                {
                    processPaymentRequest.CustomValues.Add("AuthCode", authNo);
                    HttpContext.Session.Remove("Auth_No");
                }

                var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);

                if (placeOrderResult.Success)
                {
                    HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };
                    await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

                    if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                    {
                        //redirection or POST has been done in PostProcessPayment
                        return Content(await _localizationService.GetResourceAsync( "Checkout.RedirectMessage"));
                    }

                    return RedirectToRoute("CheckoutCompleted", new { orderId = placeOrderResult.PlacedOrder.Id });
                }

                foreach (var error in placeOrderResult.Errors)
                    model.Warnings.Add(error);
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc);
                model.Warnings.Add(exc.Message);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        // Custom, makes payment request
        [HttpPost, ActionName("PaymentInfo")]
        [FormValueRequired("nextstep")]
        public override async Task<IActionResult> EnterPaymentInfo(IFormCollection form)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var cart = await _shoppingCartService.GetShoppingCartAsync(
                await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart,
                (await _storeContext.GetCurrentStoreAsync()).Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //Check whether payment workflow is required
            var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart);
            if (!isPaymentWorkflowRequired)
            {
                return RedirectToRoute("CheckoutConfirm");
            }

            //load payment method
            var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.SelectedPaymentMethodAttribute, (await _storeContext.GetCurrentStoreAsync()).Id);
            var paymentMethod = await _paymentPluginManager
                .LoadPluginBySystemNameAsync(paymentMethodSystemName, await _workContext.GetCurrentCustomerAsync(), (await _storeContext.GetCurrentStoreAsync()).Id);
            if (paymentMethod == null)
                return RedirectToRoute("CheckoutPaymentMethod");

            var warnings = await paymentMethod.ValidatePaymentFormAsync(form);
            foreach (var warning in warnings)
                ModelState.AddModelError("", warning);

            // custom code is here
            string status_code = string.Empty;
            string response_message = string.Empty;

            if (ModelState.IsValid)
            {
                //get payment info
                var paymentInfo = await paymentMethod.GetPaymentInfoAsync(form);
                //set previous order GUID (if exists)
                _paymentService.GenerateOrderGuid(paymentInfo);

                HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", paymentInfo);

                // ABC: if UniFi, just go to confirm
                if (paymentMethodSystemName == "Payments.UniFi")
                {
                    return RedirectToRoute("CheckoutConfirm");
                }

                var paymentResponse = await SendPaymentRequestAsync(paymentInfo);

                if (paymentResponse.status_code == "00")
                {
                    //session save
                    HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", paymentInfo);
                    return RedirectToRoute("CheckoutConfirm");
                }
                else
                {
                    paymentResponse.response_message = WebUtility.HtmlDecode(paymentResponse.response_message);
                    ModelState.AddModelError("", paymentResponse.response_message);
                }
            }

            //If we got this far, something failed, redisplay form
            //model
            var model = await _checkoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);
            return View(model);
        }
    }
}
