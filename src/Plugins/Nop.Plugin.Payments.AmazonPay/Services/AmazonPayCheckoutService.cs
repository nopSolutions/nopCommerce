using Amazon.Pay.API.WebStore.CheckoutSession;
using Amazon.Pay.API.WebStore.Types;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Payments.AmazonPay.Enums;
using Nop.Plugin.Payments.AmazonPay.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Factories;
using CheckoutCompletedModel = Nop.Plugin.Payments.AmazonPay.Models.CheckoutCompletedModel;
using CheckoutConfirmModel = Nop.Plugin.Payments.AmazonPay.Models.CheckoutConfirmModel;

namespace Nop.Plugin.Payments.AmazonPay.Services;

/// <summary>
/// Represents the service for checkout related methods
/// </summary>
public class AmazonPayCheckoutService(AmazonPayApiService amazonPayApiService,
        AmazonPaySettings amazonPaySettings,
        CustomerSettings customerSettings,
        DisallowedProducts disallowedProducts,
        IAddressService addressService,
        ICheckoutModelFactory checkoutModelFactory,
        ICountryService countryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IExternalAuthenticationService externalAuthenticationService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        ILogger logger,
        INotificationService notificationService,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPaymentPluginManager paymentPluginManager,
        IPriceCalculationService priceCalculationService,
        IPriceFormatter priceFormatter,
        IProductService productService,
        IShippingService shippingService,
        IShoppingCartService shoppingCartService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IWebHelper webHelper,
        IWorkContext workContext,
        OrderSettings orderSettings)
{
    #region Utilities

    private async Task FillAddressAsync(CreateCheckoutSessionRequest request)
    {
        var customer = await workContext.GetCurrentCustomerAsync();
        var shippingAddress = await addressService.GetAddressByIdAsync(customer.ShippingAddressId ?? 0);

        if (shippingAddress == null)
            return;

        request.AddressDetails.Name = $"{shippingAddress.FirstName ?? customer.FirstName} {shippingAddress.LastName ?? customer.LastName}";
        request.AddressDetails.AddressLine1 = shippingAddress.Address1;
        request.AddressDetails.AddressLine2 = shippingAddress.Address2;
        request.AddressDetails.PhoneNumber = shippingAddress.PhoneNumber;
        request.AddressDetails.City = shippingAddress.City;
        request.AddressDetails.PostalCode = shippingAddress.ZipPostalCode;
        request.AddressDetails.DistrictOrCounty = shippingAddress.County;

        var country = await countryService.GetCountryByIdAsync(shippingAddress.CountryId ?? 0);

        if (country == null)
            return;

        request.AddressDetails.CountryCode = country.TwoLetterIsoCode;

        var state = await stateProvinceService.GetStateProvinceByIdAsync(shippingAddress.StateProvinceId ?? 0);

        if (state == null)
            return;

        request.AddressDetails.StateOrRegion = state.Abbreviation;
    }

    private async Task<bool> IsCartContainsNoAllowedProductsAsync(int? productId)
    {
        if (productId != null && await disallowedProducts.IsProductDisallowAsync(productId.Value))
            return true;

        var cart = await GetCartAsync();

        return await cart.AnyAwaitAsync(async item =>
            await disallowedProducts.IsProductDisallowAsync(item.ProductId));
    }

    private async Task<bool> IsMinimumOrderPlacementIntervalValidAsync(Customer customer)
    {
        //prevent 2 orders being placed within an X seconds time frame
        if (orderSettings.MinimumOrderPlacementInterval == 0)
            return true;

        var store = await storeContext.GetCurrentStoreAsync();
        var lastOrder = (await orderService.SearchOrdersAsync(store.Id, customerId: customer.Id, pageSize: 1)).FirstOrDefault();
        if (lastOrder == null)
            return true;

        var interval = DateTime.UtcNow - lastOrder.CreatedOnUtc;

        return interval.TotalSeconds > orderSettings.MinimumOrderPlacementInterval;
    }

    private async Task<(Order placedOrder, List<string> warnings)> PlaceOrderAsync(string orderGuid)
    {
        var warnings = new List<string>();

        try
        {
            var customer = await workContext.GetCurrentCustomerAsync();
            var store = await storeContext.GetCurrentStoreAsync();

            //prevent 2 orders being placed within an X seconds time frame
            if (!await IsMinimumOrderPlacementIntervalValidAsync(customer))
                throw new NopException(await localizationService.GetResourceAsync("Checkout.MinOrderPlacementInterval"));

            var request = new ProcessPaymentRequest
            {
                OrderGuid = Guid.TryParse(orderGuid, out var guid) ? guid : Guid.NewGuid(),
                OrderGuidGeneratedOnUtc = DateTime.UtcNow,
                StoreId = store.Id,
                CustomerId = customer.Id,
                PaymentMethodSystemName = AmazonPayDefaults.PluginSystemName
            };

            //place order
            var placeOrderResult = await orderProcessingService.PlaceOrderAsync(request);

            if (placeOrderResult.Success)
                return (placeOrderResult.PlacedOrder, warnings);

            foreach (var error in placeOrderResult.Errors)
                warnings.Add(error);
        }
        catch (Exception exc)
        {
            await logger.ErrorAsync(exc.Message, exc);
            warnings.Add(exc.Message);
        }

        return (null, warnings);
    }

    private string ReadCheckoutSessionId()
    {
        var checkoutSessionId = webHelper.QueryString<string>(AmazonPayDefaults.CheckoutSessionQueryParamName);

        return checkoutSessionId;
    }

    private async Task<CheckoutConfirmModel> FillCheckoutDataBySessionAsync()
    {
        var customer = await workContext.GetCurrentCustomerAsync();
        var store = await storeContext.GetCurrentStoreAsync();

        var model = new CheckoutConfirmModel
        {
            AmazonPayScript = amazonPayApiService.AmazonPayScriptUrl
        };

        var skip = await genericAttributeService
            .GetAttributeAsync<bool?>(customer, AmazonPayDefaults.SkipFillDataBySessionAttributeName, store.Id);
        if (skip.HasValue && skip.Value)
            return model;

        // send the request
        var checkoutSessionId = await GetCheckoutSessionIdAsync();
        var result = await amazonPayApiService.PerformRequestAsync(client => client.GetCheckoutSession(checkoutSessionId));

        if (!string.IsNullOrEmpty(result.Buyer?.BuyerId) && !string.IsNullOrEmpty(result.Buyer.Email))
        {
            model.BuyerId = result.Buyer.BuyerId;
            model.BuyerEmail = result.Buyer.Email;
            model.BuyerName = result.Buyer.Name;

            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ExternalIdentifier = result.Buyer.BuyerId,
                ProviderSystemName = AmazonPayDefaults.PluginSystemName
            };
            var associatedCustomer = await externalAuthenticationService
                .GetUserByExternalAuthenticationParametersAsync(authenticationParameters);

            if (await customerService.IsGuestAsync(customer))
            {
                if (associatedCustomer is not null)
                    await externalAuthenticationService.AuthenticateAsync(authenticationParameters, webHelper.GetThisPageUrl(true));
                else
                {
                    var customerByEmail = await customerService.GetCustomerByEmailAsync(result.Buyer.Email);
                    if (customerByEmail is null)
                    {
                        if (customerSettings.UserRegistrationType != UserRegistrationType.Disabled)
                        {
                            model.CanCreateAccount = true;
                            notificationService.SuccessNotification(await localizationService.GetResourceAsync("Plugins.Payments.AmazonPay.SignIn.CreateAccount"));
                        }
                    }
                    else
                    {
                        model.CanAssociateAccount = true;
                        notificationService.SuccessNotification(await localizationService.GetResourceAsync("Plugins.Payments.AmazonPay.SignIn.LinkAccounts.ByEmail"));
                    }
                }
            }
            else if (associatedCustomer is null)
            {
                model.CanAssociateAccount = true;
                notificationService.SuccessNotification(await localizationService.GetResourceAsync("Plugins.Payments.AmazonPay.SignIn.LinkAccounts"));
            }
        }

        async Task<int> getAddressId(Address address)
        {
            var names = address.Name.Split(' ');

            var firstName = names[0];
            var lastName = names.Length > 1 ? names[1] : string.Empty;

            var countryId = (await countryService.GetCountryByTwoLetterIsoCodeAsync(address.CountryCode))?.Id ?? 0;
            var stateProvinceId = 0;
            if (!string.IsNullOrEmpty(address.StateOrRegion))
                stateProvinceId = (await stateProvinceService.GetStateProvinceByAbbreviationAsync(address.StateOrRegion))?.Id ?? 0;

            var email = result.Buyer.Email ?? customer.Email;

            var addressForSearch = new Core.Domain.Common.Address
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = address.PhoneNumber,
                Address1 = address.AddressLine1,
                Address2 = address.AddressLine2,
                City = address.City,
                County = address.County,
                StateProvinceId = stateProvinceId,
                ZipPostalCode = address.PostalCode,
                CountryId = countryId
            };

            var existsAddress = addressService.FindAddress((await customerService.GetAddressesByCustomerIdAsync(customer.Id)).ToList(),
                addressForSearch.FirstName, addressForSearch.LastName, addressForSearch.PhoneNumber,
                addressForSearch.Email, null, null,
                addressForSearch.Address1, addressForSearch.Address2, addressForSearch.City,
                addressForSearch.County, addressForSearch.StateProvinceId, addressForSearch.ZipPostalCode,
                addressForSearch.CountryId, string.Empty);

            if (existsAddress != null)
                return existsAddress.Id;

            //address is not found. let's create a new one
            addressForSearch.CreatedOnUtc = DateTime.UtcNow;

            //some validation
            if (addressForSearch.CountryId == 0)
                addressForSearch.CountryId = null;
            if (addressForSearch.StateProvinceId == 0)
                addressForSearch.StateProvinceId = null;

            await addressService.InsertAddressAsync(addressForSearch);
            await customerService.InsertCustomerAddressAsync(customer, addressForSearch);
            await genericAttributeService.SaveAttributeAsync(addressForSearch, AmazonPayDefaults.AddressAttributeName, true);

            return addressForSearch.Id;
        }

        var billingAddress = result.BillingAddress;
        var needCustomerUpdate = false;

        if (billingAddress != null)
        {
            var addressId = await getAddressId(billingAddress);
            if (addressId != customer.BillingAddressId)
            {
                customer.BillingAddressId = addressId;
                needCustomerUpdate = true;
            }
        }

        var shippingAddress = result.ShippingAddress;

        if (shippingAddress != null)
        {
            var addressId = await getAddressId(shippingAddress);
            if (addressId != customer.ShippingAddressId)
            {
                customer.ShippingAddressId = addressId;
                needCustomerUpdate = true;
                await SetShippingMethodToNullAsync();
            }
        }

        if (needCustomerUpdate)
            await customerService.UpdateCustomerAsync(customer);

        var paymentDescriptor = result.PaymentPreferences?.FirstOrDefault()?.PaymentDescriptor ?? string.Empty;

        await genericAttributeService
            .SaveAttributeAsync(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, paymentDescriptor, store.Id);

        return model;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get customer shopping cart
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping Cart
    /// </returns>
    public async Task<IList<ShoppingCartItem>> GetCartAsync(Customer customer = null)
    {
        customer ??= await workContext.GetCurrentCustomerAsync();
        var store = await storeContext.GetCurrentStoreAsync();
        var cart = await shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        return cart;
    }

    /// <summary>
    /// Validates whether this shopping cart is valid
    /// </summary>
    /// <param name="shoppingCart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public async Task<string> GetShoppingCartWarningsAsync(IList<ShoppingCartItem> shoppingCart)
    {
        var customer = await workContext.GetCurrentCustomerAsync();
        var store = await storeContext.GetCurrentStoreAsync();
        var checkoutAttributesXml = await genericAttributeService
            .GetAttributeAsync<string>(customer, NopCustomerDefaults.CheckoutAttributes, store.Id);

        var scWarnings = await shoppingCartService.GetShoppingCartWarningsAsync(shoppingCart, checkoutAttributesXml, true);

        return scWarnings.Any() ? string.Join("<br />", scWarnings) : string.Empty;
    }

    /// <summary>
    /// Check whether the checkout is allowed for customer
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains True if checkout is allowed, otherwise False
    /// </returns>
    public async Task<bool> IsAllowedToCheckoutAsync(Customer customer = null)
    {
        customer ??= await workContext.GetCurrentCustomerAsync();
        return !(await customerService.IsGuestAsync(customer) && !orderSettings.AnonymousCheckoutAllowed);
    }

    /// <summary>
    /// Prepare confirm order model
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the confirm order model
    /// </returns>
    public async Task<CheckoutConfirmModel> PrepareConfirmOrderModelAsync(IList<ShoppingCartItem> cart)
    {
        try
        {
            var customer = await workContext.GetCurrentCustomerAsync();
            var (_, currency) = await amazonPayApiService.GetCurrencyAsync();

            var model = await FillCheckoutDataBySessionAsync();

            if (await ShoppingCartRequiresShippingAsync(cart))
            {
                var shippingAddress = await addressService.GetAddressByIdAsync(customer.ShippingAddressId ?? 0);
                var shippingMethodModel = await checkoutModelFactory.PrepareShippingMethodModelAsync(await GetCartAsync(),
                    shippingAddress);

                model.SetShippingMethodData(shippingMethodModel);
            }

            //min order amount validation
            var minOrderTotalAmountOk = await orderProcessingService.ValidateMinOrderTotalAmountAsync(cart);
            if (minOrderTotalAmountOk)
                return model;

            var minOrderTotalAmount = await currencyService.ConvertFromPrimaryStoreCurrencyAsync(orderSettings.MinOrderTotalAmount, currency);
            minOrderTotalAmount = await priceCalculationService.RoundPriceAsync(minOrderTotalAmount, currency);
            model.MinOrderTotalWarning = string.Format(await localizationService.GetResourceAsync("Checkout.MinOrderTotalAmount"),
                await priceFormatter.FormatPriceAsync(minOrderTotalAmount, true, false));

            return model;
        }
        catch (Exception exception)
        {
            var logMessage = $"{AmazonPayDefaults.PluginSystemName} error:{System.Environment.NewLine}{exception.Message}";
            await logger.ErrorAsync(logMessage, exception, await workContext.GetCurrentCustomerAsync());

            return null;
        }
    }

    /// <summary>
    /// Indicates whether the shopping cart requires shipping
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the true if the shopping cart requires shipping; otherwise, false.
    /// </returns>
    public async Task<bool> ShoppingCartRequiresShippingAsync(IList<ShoppingCartItem> cart = null)
    {
        return await shoppingCartService.ShoppingCartRequiresShippingAsync(cart ?? await GetCartAsync());
    }

    /// <summary>
    /// Remove customer addresses
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the identifiers of removed addresses
    /// </returns>
    public async Task<List<int>> RemoveCustomerAddressesAsync(Customer customer = null)
    {
        async Task<int?> removeAddressAsync(int addressId)
        {
            var address = await addressService.GetAddressByIdAsync(addressId);
            if (address is null)
                return null;

            var isAmazonAddress = await genericAttributeService.GetAttributeAsync<bool>(address, AmazonPayDefaults.AddressAttributeName);
            if (!isAmazonAddress)
                return null;

            await customerService.RemoveCustomerAddressAsync(customer, address);
            await customerService.UpdateCustomerAsync(customer);
            await genericAttributeService.SaveAttributeAsync<bool?>(address, AmazonPayDefaults.AddressAttributeName, null);
            await addressService.DeleteAddressAsync(address);

            return addressId;
        }

        //per Amazon policy, it is prohibited to use Amazon provided details (e.g. addresses),
        //so we unlink them from customers and leave only in orders
        customer ??= await workContext.GetCurrentCustomerAsync();
        var addressIds = new List<int?>
        {
            await removeAddressAsync(customer.BillingAddressId ?? 0),
            await removeAddressAsync(customer.ShippingAddressId ?? 0)
        }.Where(id => id.HasValue).Select(id => id.Value).ToList();

        return addressIds;
    }

    /// <summary>
    /// Reset shipping method attribute
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SetShippingMethodToNullAsync()
    {
        var customer = await workContext.GetCurrentCustomerAsync();
        var store = await storeContext.GetCurrentStoreAsync();
        await genericAttributeService
            .SaveAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, store.Id);
    }

    /// <summary>
    /// Save selected shipping method on the attribute
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <param name="selectedShippingOption">Selected shipping option</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the True if selected shipping method is saved, otherwise False
    /// </returns>
    public async Task<bool> SetShippingMethodAsync(IList<ShoppingCartItem> cart, string selectedShippingOption)
    {
        //parse selected method
        if (string.IsNullOrEmpty(selectedShippingOption))
            return false;

        var splittedOption = selectedShippingOption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
        if (splittedOption.Length != 2)
            return false;

        var selectedName = splittedOption[0];
        var shippingRateComputationMethodSystemName = splittedOption[1];

        var customer = await workContext.GetCurrentCustomerAsync();
        var store = await storeContext.GetCurrentStoreAsync();

        //find it
        //performance optimization. try cache first
        var shippingOptions = await genericAttributeService
            .GetAttributeAsync<List<ShippingOption>>(customer, NopCustomerDefaults.OfferedShippingOptionsAttribute, store.Id);
        if (shippingOptions == null || shippingOptions.Count == 0)
        {
            var shippingAddress = await addressService.GetAddressByIdAsync(customer.ShippingAddressId ?? 0);

            //not found? let's load them using shipping service
            shippingOptions = (await shippingService
                .GetShippingOptionsAsync(cart, shippingAddress,
                allowedShippingRateComputationMethodSystemName: shippingRateComputationMethodSystemName))
                .ShippingOptions
                .ToList();
        }
        else
            //loaded cached results. let's filter result by a chosen shipping rate computation method
            shippingOptions = shippingOptions.Where(so => so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

        var shippingOption = shippingOptions
            .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));
        if (shippingOption == null)
            return false;

        //save
        await genericAttributeService
            .SaveAttributeAsync(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, shippingOption, store.Id);

        return true;
    }

    /// <summary>
    /// Read and save checkoutSessionId parameter
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<string> ReadAndSaveCheckoutSessionIdAsync()
    {
        var checkoutSessionId = ReadCheckoutSessionId();
        var customer = await workContext.GetCurrentCustomerAsync();
        var store = await storeContext.GetCurrentStoreAsync();
        await genericAttributeService
            .SaveAttributeAsync(customer, AmazonPayDefaults.CheckoutSessionIdAttributeName, checkoutSessionId, store.Id);

        return checkoutSessionId;
    }

    /// <summary>
    /// Get stored checkoutSessionId parameter
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the checkoutSessionId parameter
    /// </returns>
    public async Task<string> GetCheckoutSessionIdAsync()
    {
        var customer = await workContext.GetCurrentCustomerAsync();
        var store = await storeContext.GetCurrentStoreAsync();
        var checkoutSessionId = await genericAttributeService
            .GetAttributeAsync<string>(customer, AmazonPayDefaults.CheckoutSessionIdAttributeName, store.Id);
        checkoutSessionId ??= await ReadAndSaveCheckoutSessionIdAsync();

        return checkoutSessionId;
    }

    /// <summary>
    /// Get payment descriptor
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the payment descriptor
    /// </returns>
    public async Task<string> GetPaymentDescriptorAsync()
    {
        var customer = await workContext.GetCurrentCustomerAsync();
        var store = await storeContext.GetCurrentStoreAsync();
        var paymentDescriptor = await genericAttributeService
            .GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);

        return paymentDescriptor;
    }

    /// <summary>
    /// Get payment info model
    /// </summary>
    /// <param name="placement">Button placement</param>
    /// <param name="productId">Product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the payment info model
    /// </returns>
    public async Task<PaymentInfoModel> GetPaymentInfoModelAsync(ButtonPlacement placement, int? productId = null)
    {
        try
        {
            var shippingIsRequired = await ShoppingCartRequiresShippingAsync();

            //when the model is prepared for the product page, this product is not yet in the cart, so we check it separately
            if (!shippingIsRequired && await productService.GetProductByIdAsync(productId ?? 0) is Product product)
                shippingIsRequired = product.IsShipEnabled;

            var (currencyCode, _) = await amazonPayApiService.GetCurrencyAsync();
            var model = new PaymentInfoModel
            {
                LedgerCurrency = amazonPayApiService.LedgerCurrency?.ToString(),
                Currency = currencyCode.ToString(),
                ButtonColor = amazonPaySettings.ButtonColor.ToString(),
                ProductType = shippingIsRequired
                    ? AmazonPayDefaults.ProductType.PayAndShip
                    : AmazonPayDefaults.ProductType.PayOnly,
                Placement = placement,
                AmazonPayScript = amazonPayApiService.AmazonPayScriptUrl,
                ProductId = productId,
                IsCartContainsNoAllowedProducts = await IsCartContainsNoAllowedProductsAsync(productId)
            };

            return model;
        }
        catch (Exception exception)
        {
            var logMessage = $"{AmazonPayDefaults.PluginSystemName} error:{System.Environment.NewLine}{exception.Message}";
            await logger.ErrorAsync(logMessage, exception, await workContext.GetCurrentCustomerAsync());

            return null;
        }
    }

    /// <summary>
    /// Create checkout session
    /// </summary>
    /// <param name="placement">Button placement</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the payload, signature and current shopping cart total to create button
    /// </returns>
    public async Task<(string payload, string signature, decimal cartTotal)> CreateCheckoutSessionAsync(ButtonPlacement placement)
    {
        try
        {
            var customer = await workContext.GetCurrentCustomerAsync();
            var store = await storeContext.GetCurrentStoreAsync();
            var (currencyCode, currency) = await amazonPayApiService.GetCurrencyAsync();

            await genericAttributeService
                .SaveAttributeAsync<string>(customer, AmazonPayDefaults.CheckoutSessionIdAttributeName, null, store.Id);
            await genericAttributeService
                .SaveAttributeAsync<bool?>(customer, AmazonPayDefaults.SkipFillDataBySessionAttributeName, null, store.Id);

            var scopes = new List<CheckoutSessionScope>
            {
                CheckoutSessionScope.Name,
                CheckoutSessionScope.Email,
                CheckoutSessionScope.PhoneNumber,
                CheckoutSessionScope.BillingAddress
            };

            if (!(placement == ButtonPlacement.PaymentMethod && await ShoppingCartRequiresShippingAsync()))
            {
                scopes.AddRange(new[]
                {
                    CheckoutSessionScope.ShippingAddress,
                    CheckoutSessionScope.PrimeStatus,
                    CheckoutSessionScope.PostalCode
                });
            }

            // prepare the request
            var request = new CreateCheckoutSessionRequest
            (
                checkoutReviewReturnUrl: amazonPayApiService.GetUrl(AmazonPayDefaults.ConfirmRouteName),
                storeId: amazonPaySettings.StoreId,
                scopes.ToArray()
            )
            { WebCheckoutDetails = { CheckoutCancelUrl = amazonPayApiService.GetUrl("ShoppingCart") } };

            request.PlatformId = AmazonPayDefaults.SpId;
            request.MerchantMetadata.CustomInformation = AmazonPayDefaults.IntegrationName;

            request.DeliverySpecifications.SpecialRestrictions.Add(SpecialRestriction.RestrictPOBoxes);

            //init address restrictions
            request.DeliverySpecifications.AddressRestrictions.Type = RestrictionType.NotAllowed;
            var method = await paymentPluginManager.LoadPluginBySystemNameAsync(AmazonPayDefaults.PluginSystemName);
            var restrictedCountries = await paymentPluginManager.GetRestrictedCountryIdsAsync(method);
            var countries = await countryService.GetCountriesByIdsAsync(restrictedCountries.ToArray());

            foreach (var country in countries)
                request.DeliverySpecifications.AddressRestrictions.AddCountryRestriction(country.TwoLetterIsoCode.ToUpper());

            var cart = await GetCartAsync();
            var cartTotal = (await orderTotalCalculationService.GetShoppingCartTotalAsync(cart)).shoppingCartTotal ?? 0;
            cartTotal = cartTotal != 0
                ? cartTotal
                : (await orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, true))
                .subTotalWithoutDiscount;
            cartTotal = await currencyService.ConvertFromPrimaryStoreCurrencyAsync(cartTotal, currency);
            cartTotal = await priceCalculationService.RoundPriceAsync(cartTotal, currency);

            request.PaymentDetails = new() { PresentmentCurrency = currencyCode };

            if (placement != ButtonPlacement.PaymentMethod)
                await SetShippingMethodToNullAsync();
            else
            {
                await genericAttributeService
                    .SaveAttributeAsync<bool?>(customer, AmazonPayDefaults.SkipFillDataBySessionAttributeName, true, store.Id);
            }

            if (placement == ButtonPlacement.PaymentMethod)
            {
                if (await ShoppingCartRequiresShippingAsync())
                    await FillAddressAsync(request);

                request.WebCheckoutDetails.CheckoutMode = CheckoutMode.ProcessOrder;
                request.WebCheckoutDetails.CheckoutResultReturnUrl = amazonPayApiService.GetUrl(AmazonPayDefaults.CheckoutResultHandlerRouteName);
                request.WebCheckoutDetails.CheckoutReviewReturnUrl = null;

                request.PaymentDetails = new()
                {
                    ChargeAmount = { Amount = cartTotal, CurrencyCode = currencyCode },
                    PaymentIntent = amazonPaySettings.PaymentType == PaymentType.Capture
                        ? PaymentIntent.AuthorizeWithCapture
                        : PaymentIntent.Authorize,
                    PresentmentCurrency = currencyCode
                };

                request.MerchantMetadata.MerchantStoreName = store.Name;
                request.MerchantMetadata.MerchantReferenceId = Guid.NewGuid().ToString().ToUpper();
            }

            if (await shoppingCartService.ShoppingCartIsRecurringAsync(cart))
            {
                var (error, cycleLength, cyclePeriod, _) = await shoppingCartService.GetRecurringCycleInfoAsync(cart);
                if (!string.IsNullOrEmpty(error))
                    throw new NopException(error);

                request.RecurringMetadata.Amount.Amount = cartTotal;
                request.RecurringMetadata.Amount.CurrencyCode = currencyCode;
                request.RecurringMetadata.Frequency.Value = cycleLength;
                request.ChargePermissionType = ChargePermissionType.Recurring;
                request.RecurringMetadata.Frequency.Unit = cyclePeriod switch
                {
                    RecurringProductCyclePeriod.Days => (FrequencyUnit?)FrequencyUnit.Day,
                    RecurringProductCyclePeriod.Weeks => (FrequencyUnit?)FrequencyUnit.Week,
                    RecurringProductCyclePeriod.Months => (FrequencyUnit?)FrequencyUnit.Month,
                    RecurringProductCyclePeriod.Years => (FrequencyUnit?)FrequencyUnit.Year,
                    _ => throw new ArgumentOutOfRangeException(),
                };
            }

            //generate the button signature
            var signature = await amazonPayApiService.GenerateButtonSignatureAsync(request);

            // the payload as JSON string that you must assign to the button in the next step
            var payload = request.ToJson();

            return (payload, signature, cartTotal);
        }
        catch (Exception exception)
        {
            var logMessage = $"{AmazonPayDefaults.PluginSystemName} error:{System.Environment.NewLine}{exception.Message}";
            await logger.ErrorAsync(logMessage, exception, await workContext.GetCurrentCustomerAsync());

            return (null, null, decimal.Zero);
        }
    }

    /// <summary>
    /// Update the checkout session and gets return URL
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the return URL
    /// </returns>
    public async Task<string> UpdateCheckoutSessionAsync()
    {
        var store = await storeContext.GetCurrentStoreAsync();
        var (currencyCode, currency) = await amazonPayApiService.GetCurrencyAsync();

        var cart = await GetCartAsync();
        var cartTotal = (await orderTotalCalculationService.GetShoppingCartTotalAsync(cart)).shoppingCartTotal ?? 0;
        cartTotal = await currencyService.ConvertFromPrimaryStoreCurrencyAsync(cartTotal, currency);
        cartTotal = await priceCalculationService.RoundPriceAsync(cartTotal, currency);

        // prepare the request
        var request = new UpdateCheckoutSessionRequest
        {
            WebCheckoutDetails =
            {
                CheckoutResultReturnUrl = amazonPayApiService.GetUrl(AmazonPayDefaults.CheckoutResultHandlerRouteName),
            },
            PaymentDetails =
            {
                CanHandlePendingAuthorization = false,
                ChargeAmount =
                {
                    Amount = cartTotal,
                    CurrencyCode = currencyCode
                },
                PaymentIntent = amazonPaySettings.PaymentType == PaymentType.Capture ? PaymentIntent.AuthorizeWithCapture : PaymentIntent.Authorize
            },
            MerchantMetadata =
            {
                MerchantStoreName = store.Name,
                MerchantReferenceId = Guid.NewGuid().ToString().ToUpper()
            }
        };

        if (await shoppingCartService.ShoppingCartIsRecurringAsync(cart))
        {
            var (error, cycleLength, cyclePeriod, _) = await shoppingCartService.GetRecurringCycleInfoAsync(cart);
            if (!string.IsNullOrEmpty(error))
                throw new NopException(error);

            request.RecurringMetadata.Amount.Amount = cartTotal;
            request.RecurringMetadata.Amount.CurrencyCode = currencyCode;
            request.RecurringMetadata.Frequency.Value = cycleLength;
            request.ChargePermissionType = ChargePermissionType.Recurring;
            request.RecurringMetadata.Frequency.Unit = cyclePeriod switch
            {
                RecurringProductCyclePeriod.Days => (FrequencyUnit?)FrequencyUnit.Day,
                RecurringProductCyclePeriod.Weeks => (FrequencyUnit?)FrequencyUnit.Week,
                RecurringProductCyclePeriod.Months => (FrequencyUnit?)FrequencyUnit.Month,
                RecurringProductCyclePeriod.Years => (FrequencyUnit?)FrequencyUnit.Year,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        // send the request
        var checkoutSessionId = await GetCheckoutSessionIdAsync();
        var result = await amazonPayApiService.PerformRequestAsync(client => client.UpdateCheckoutSession(checkoutSessionId, request, amazonPayApiService.Headers));

        return result.WebCheckoutDetails.AmazonPayRedirectUrl;
    }

    /// <summary>
    /// Complete checkout process
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the placed order, warnings if exist and the model
    /// </returns>
    public async Task<(Order placedOrder, List<string> warnings, CheckoutCompletedModel completedModel)> CompleteCheckoutAsync()
    {
        var customer = await workContext.GetCurrentCustomerAsync();
        var store = await storeContext.GetCurrentStoreAsync();
        var (currencyCode, currency) = await amazonPayApiService.GetCurrencyAsync();

        var cart = await GetCartAsync();
        var cartTotal = (await orderTotalCalculationService.GetShoppingCartTotalAsync(cart)).shoppingCartTotal ?? 0;
        cartTotal = await currencyService.ConvertFromPrimaryStoreCurrencyAsync(cartTotal, currency);
        cartTotal = await priceCalculationService.RoundPriceAsync(cartTotal, currency);

        var checkoutSessionId = ReadCheckoutSessionId();
        var checkoutSession = await amazonPayApiService.PerformRequestAsync(client => client.GetCheckoutSession(checkoutSessionId));

        // prepare the request
        var request = new CompleteCheckoutSessionRequest(cartTotal, currencyCode);

        // send the request
        var result = await amazonPayApiService.PerformRequestAsync(client => client.CompleteCheckoutSession(checkoutSessionId, request, amazonPayApiService.Headers));
        var chargeInfo = await amazonPayApiService.PerformRequestAsync(client => client.GetCharge(result.ChargeId));

        var chargePermissionId = await shoppingCartService.ShoppingCartIsRecurringAsync(cart)
            ? result.ChargePermissionId
            : string.Empty;

        var (order, warnings) = await PlaceOrderAsync(chargeInfo.MerchantMetadata?.MerchantReferenceId);

        if (order == null)
            return (null, warnings, null);

        order.SubscriptionTransactionId = chargePermissionId;

        switch (amazonPaySettings.PaymentType)
        {
            case PaymentType.Capture:
                order.CaptureTransactionId = result.ChargeId;
                await orderProcessingService.MarkOrderAsPaidAsync(order);
                break;
            case PaymentType.Authorize:
                order.AuthorizationTransactionId = result.ChargeId;
                await orderProcessingService.MarkAsAuthorizedAsync(order);
                break;
        }

        await genericAttributeService
            .SaveAttributeAsync<string>(customer, AmazonPayDefaults.CheckoutSessionIdAttributeName, null, store.Id);

        await SetShippingMethodToNullAsync();

        var model = new CheckoutCompletedModel
        {
            CustomOrderNumber = order.CustomOrderNumber,
            OrderId = order.Id,
            AmazonPayScript = amazonPayApiService.AmazonPayScriptUrl,
            BuyerId = checkoutSession.Buyer?.BuyerId
        };

        return (order, new List<string>(), model);
    }

    #endregion
}