using System.Globalization;
using Nop.Plugin.Payments.PayPalCommerce.Domain;
using Nop.Plugin.Payments.PayPalCommerce.Models.Admin;
using Nop.Plugin.Payments.PayPalCommerce.Models.Public;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Services.Localization;
using Nop.Web.Factories;
using Nop.Web.Models.Checkout;

namespace Nop.Plugin.Payments.PayPalCommerce.Factories;

/// <summary>
/// Represents the plugin model factory
/// </summary>
public class PayPalCommerceModelFactory
{
    #region Fields

    private readonly ICheckoutModelFactory _checkoutModelFactory;
    private readonly ILocalizationService _localizationService;
    private readonly PayPalCommerceServiceManager _serviceManager;
    private readonly PayPalCommerceSettings _settings;

    #endregion

    #region Ctor

    public PayPalCommerceModelFactory(ICheckoutModelFactory checkoutModelFactory,
        ILocalizationService localizationService,
        PayPalCommerceServiceManager serviceManager,
        PayPalCommerceSettings settings)
    {
        _checkoutModelFactory = checkoutModelFactory;
        _localizationService = localizationService;
        _serviceManager = serviceManager;
        _settings = settings;
    }

    #endregion

    #region Methods

    #region Components

    /// <summary>
    /// Prepare the Pay Later messages model
    /// </summary>
    /// <param name="placement">Button placement</param>
    /// <param name="loadScript">Whether to load Pay Later JS script on the page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the Pay Later messages model
    /// </returns>
    public async Task<MessagesModel> PrepareMessagesModelAsync(ButtonPlacement placement, bool loadScript)
    {
        var ((messageConfig, amount, currencyCode), _) = await _serviceManager.PrepareMessagesAsync(_settings, placement);

        return new()
        {
            Placement = placement,
            LoadScript = loadScript,
            Config = messageConfig,
            Amount = amount,
            CurrencyCode = currencyCode,
            //Country = null //PayPal auto detects this
        };
    }

    /// <summary>
    /// Prepare the payment info model
    /// </summary>
    /// <param name="placement">Button placement</param>
    /// <param name="productId">Product id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the payment info model
    /// </returns>
    public async Task<PaymentInfoModel> PreparePaymentInfoModelAsync(ButtonPlacement placement, int? productId = null)
    {
        var (((scriptUrl, clientToken, userToken), (email, name), (messageConfig, amount), (isRecurring, isShippable)), _) = await _serviceManager
            .PreparePaymentDetailsAsync(_settings, placement, productId);

        return new()
        {
            Placement = placement,
            ProductId = productId,
            Script = (scriptUrl, clientToken, userToken),
            Customer = (email, name),
            MessagesModel = new() { Config = messageConfig, Amount = amount },
            Cart = (isRecurring, isShippable)
        };
    }

    #endregion

    #region Checkout

    /// <summary>
    /// Prepare the checkout payment info model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the checkout payment info model
    /// </returns>
    public async Task<CheckoutPaymentInfoModel> PrepareCheckoutPaymentInfoModelAsync()
    {
        var (active, paymentMethod) = await _serviceManager.IsActiveAsync(_settings);
        if (!active || paymentMethod is null)
            return new();

        return await _checkoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);
    }

    /// <summary>
    /// Get the shopping cart warnings
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the validation warnings
    /// </returns>
    public async Task<IList<string>> GetShoppingCartWarningsAsync()
    {
        var (warnings, error) = await _serviceManager.ValidateShoppingCartAsync();
        if (!string.IsNullOrEmpty(error))
            return [error];

        return warnings;
    }

    /// <summary>
    /// Check whether the shipping is required for the current cart/product
    /// </summary>
    /// <param name="productId">Product id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the check result; error message if exists
    /// </returns>
    public async Task<(bool ShippingIsRequired, string Error)> CheckShippingIsRequiredAsync(int? productId)
    {
        return await _serviceManager.CheckShippingIsRequiredAsync(productId);
    }

    /// <summary>
    /// Prepare the order model
    /// </summary>
    /// <param name="placement">Button placement</param>
    /// <param name="orderId">Order id (when the order is already created)</param>
    /// <param name="paymentSource">Payment source (e.g. PayPal, Card, etc)</param>
    /// <param name="cardId">Saved card id</param>
    /// <param name="saveCard">Whether to save card payment token</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order model
    /// </returns>
    public async Task<OrderModel> PrepareOrderModelAsync(ButtonPlacement placement, string orderId, string paymentSource, int? cardId, bool saveCard)
    {
        var model = new OrderModel();
        (model.CheckoutIsEnabled, model.LoginIsRequired, _) = await _serviceManager.CheckoutIsEnabledAsync();

        //get the order or create a new one
        var (order, error) = string.IsNullOrEmpty(orderId)
            ? await _serviceManager.CreateOrderAsync(_settings, placement, paymentSource, cardId, saveCard)
            : await _serviceManager.GetOrderAsync(_settings, orderId);
        if (!string.IsNullOrEmpty(error) || order is null)
        {
            model.Error = string.IsNullOrEmpty(error)
                ? await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Error")
                : error;
            return model;
        }

        //set some order parameters
        model.OrderId = order.Id;
        model.Status = order.Status;
        model.PayerActionUrl = order.PayerActionUrl;

        return model;
    }

    /// <summary>
    /// Prepare the order shipping model
    /// </summary>
    /// <param name="model">Order shipping model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order shipping model
    /// </returns>
    public async Task<OrderShippingModel> PrepareOrderShippingModelAsync(OrderShippingModel model)
    {
        var (checkoutIsEnabled, loginIsRequired, _) = await _serviceManager.CheckoutIsEnabledAsync();
        if (!checkoutIsEnabled || loginIsRequired)
            return model;

        var (_, error) = await _serviceManager.UpdateOrderShippingAsync(_settings, model.OrderId,
            (model.AddressCity, model.AddressState, model.AddressCountryCode, model.AddressPostalCode),
            (model.OptionId, model.OptionType));
        if (!string.IsNullOrEmpty(error))
            model.Error = error;

        return model;
    }

    /// <summary>
    /// Prepare the order approved model
    /// </summary>
    /// <param name="orderId">Order id</param>
    /// <param name="liabilityShift">Liability shift</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order approved model
    /// </returns>
    public async Task<OrderApprovedModel> PrepareOrderApprovedModelAsync(string orderId, string liabilityShift)
    {
        var model = new OrderApprovedModel();
        (model.CheckoutIsEnabled, model.LoginIsRequired, var cart) = await _serviceManager.CheckoutIsEnabledAsync();
        if (cart?.Any() != true)
            return model;

        var ((order, payNow), error) = await _serviceManager.OrderIsApprovedAsync(_settings, orderId, null, liabilityShift);
        if (!string.IsNullOrEmpty(error))
            model.Error = error;
        else
            (model.OrderId, model.PayNow) = (order?.Id, payNow);

        return model;
    }

    /// <summary>
    /// Prepare the order confirmation model
    /// </summary>
    /// <param name="orderId">Order id</param>
    /// <param name="orderGuid">Internal order id (used in 3D Secure cases)</param>
    /// <param name="liabilityShift">Liability shift</param>
    /// <param name="approve">Whether the order is approved now; pass false to avoid order approval reprocessing</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order confirmation model
    /// </returns>
    public async Task<OrderConfirmModel> PrepareOrderConfirmModelAsync(string orderId, string orderGuid, string liabilityShift, bool approve)
    {
        var model = new OrderConfirmModel { OrderId = orderId, OrderGuid = orderGuid, LiabilityShift = liabilityShift };
        (model.CheckoutIsEnabled, model.LoginIsRequired, var cart) = await _serviceManager.CheckoutIsEnabledAsync();
        if (cart?.Any() != true)
            return model;

        //prepare common confirmation model parameters
        var checkoutConfirmModel = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);
        model.DisplayCaptcha = checkoutConfirmModel.DisplayCaptcha;
        model.MinOrderTotalWarning = checkoutConfirmModel.MinOrderTotalWarning;
        model.TermsOfServiceOnOrderConfirmPage = checkoutConfirmModel.TermsOfServiceOnOrderConfirmPage;
        model.TermsOfServicePopup = checkoutConfirmModel.TermsOfServicePopup;
        if (!approve)
            return model;

        //order is approved now
        var ((order, _), error) = await _serviceManager.OrderIsApprovedAsync(_settings, orderId, orderGuid, liabilityShift);
        if (!string.IsNullOrEmpty(error))
            model.Error = error;
        else
            model.OrderId = order?.Id;

        return model;
    }

    /// <summary>
    /// Prepare the order completed model
    /// </summary>
    /// <param name="orderId">Order id</param>
    /// <param name="liabilityShift">Liability shift</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order completed model
    /// </returns>
    public async Task<OrderCompletedModel> PrepareOrderCompletedModelAsync(string orderId, string liabilityShift)
    {
        var model = new OrderCompletedModel();
        (model.CheckoutIsEnabled, model.LoginIsRequired, var cart) = await _serviceManager.CheckoutIsEnabledAsync();
        if (cart?.Any() != true)
            return model;

        //first place an order
        var ((nopOrder, order), error) = await _serviceManager.PlaceOrderAsync(_settings, orderId, liabilityShift);
        if (!string.IsNullOrEmpty(error))
            model.Error = error;
        else if (order is null)
            model.Error = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Error");
        else
            model.OrderId = nopOrder.Id.ToString();

        if (nopOrder is null || order is null)
            return model;

        //then confirm the placed order
        var (_, warning) = await _serviceManager.ConfirmOrderAsync(_settings, nopOrder, order);
        if (!string.IsNullOrEmpty(warning))
            model.Warning = warning;

        return model;
    }

    /// <summary>
    /// Prepare the Apple Pay model
    /// </summary>
    /// <param name="placement">Button placement</param>
    /// <param name="shippingIsSet">Whether the shipping details are set</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the Apple Pay model
    /// </returns>
    public async Task<ApplePayModel> PrepareApplePayModelAsync(ButtonPlacement placement, bool shippingIsSet = false)
    {
        var model = new ApplePayModel();
        (model.CheckoutIsEnabled, model.LoginIsRequired, _) = await _serviceManager.CheckoutIsEnabledAsync();

        var ((amount, billingAddress, shippingAddress, shipping, storeName), error) = await _serviceManager
            .GetAppleTransactionInfoAsync(placement);
        if (!string.IsNullOrEmpty(error))
        {
            model.Error = error;
            return model;
        }

        //function to prepare items
        async Task<(string Type, string Price, string Status, string Label)> prepareItemAsync(string type, string value, string resourcePostfix)
        {
            if (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount) || amount <= decimal.Zero)
                return default;

            if (resourcePostfix == "Discount")
                amount = -amount;

            var price = amount.ToString("0.00", CultureInfo.InvariantCulture);
            var status = "final";
            var label = type == "TOTAL"
                ? storeName
                : await _localizationService.GetResourceAsync($"Plugins.Payments.PayPalCommerce.ApplePay.{resourcePostfix}");
            return (type, price, status, label);
        }

        //line items (subtotal, tax, etc)
        var items = new List<(string Type, string Price, string Status, string Label)>
        {
            await prepareItemAsync("TOTAL", amount.Value, "Total"),
            await prepareItemAsync("SUBTOTAL", amount.Breakdown.ItemTotal.Value, "Subtotal"),
            await prepareItemAsync("TAX", amount.Breakdown.TaxTotal.Value, "Tax"),
            await prepareItemAsync("SHIPPING", amount.Breakdown.Shipping.Value, "Shipping"),
            await prepareItemAsync("DISCOUNT", amount.Breakdown.Discount.Value, "Discount")
        };

        model.Placement = placement;
        model.CurrencyCode = amount.CurrencyCode;
        model.BillingAddress = billingAddress;
        model.ShippingAddress = shippingAddress;
        model.Items = items.Where(item => !string.IsNullOrEmpty(item.Type)).ToList();
        model.ShippingOptions = shipping?.Options
            ?.Select(option => ($"{option.Id}|{option.Type}", option.Id, option.Label, option.Amount.Value))
            .ToList();

        return model;
    }

    /// <summary>
    /// Prepare the Apple Pay shipping model
    /// </summary>
    /// <param name="model">Apple Pay shipping model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the Apple Pay shipping model
    /// </returns>
    public async Task<ApplePayShippingModel> PrepareApplePayShippingModelAsync(ApplePayShippingModel model)
    {
        //prepare updated shipping details
        var (shipping, error) = await _serviceManager.UpdateAppleShippingAsync(model.Placement,
            (model.AddressCity, model.AddressState, model.AddressCountryCode, model.AddressPostalCode),
            model.OptionId);
        if (!string.IsNullOrEmpty(error) || shipping?.Options is null)
        {
            model.Error = string.IsNullOrEmpty(error)
                ? await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Error")
                : error;
            return model;
        }

        //get line items from the main model
        var applePayModel = await PrepareApplePayModelAsync(model.Placement, true);
        if (!string.IsNullOrEmpty(applePayModel.Error))
        {
            model.Error = applePayModel.Error;
            return model;
        }

        model.Items = applePayModel.Items;
        model.ShippingOptions = shipping?.Options
            ?.Select(option => ($"{option.Id}|{option.Type}", option.Id, option.Label, option.Amount.Value))
            .ToList();

        return model;
    }

    /// <summary>
    /// Prepare the Google Pay model
    /// </summary>
    /// <param name="placement">Button placement</param>
    /// <param name="shippingIsSet">Whether the shipping details are set</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the Google Pay model
    /// </returns>
    public async Task<GooglePayModel> PrepareGooglePayModelAsync(ButtonPlacement placement, bool shippingIsSet = false)
    {
        var model = new GooglePayModel();
        (model.CheckoutIsEnabled, model.LoginIsRequired, _) = await _serviceManager.CheckoutIsEnabledAsync();

        var ((amount, country, shippingIsRequired), error) = await _serviceManager.GetGoogleTransactionInfoAsync(placement);
        if (!string.IsNullOrEmpty(error))
        {
            model.Error = error;
            return model;
        }

        shippingIsRequired &= placement != ButtonPlacement.PaymentMethod;

        //function to prepare items
        async Task<(string Type, string Price, string Status, string Label)> prepareItemAsync(string type, string value, string resourcePostfix)
        {
            if (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount) || amount <= decimal.Zero)
                return default;

            if (resourcePostfix == "Discount")
                amount = -amount;

            var price = amount.ToString("0.00", CultureInfo.InvariantCulture);
            var status = !shippingIsRequired || shippingIsSet ? "FINAL" : (type == "TOTAL" ? "ESTIMATED" : "PENDING");
            var label = await _localizationService.GetResourceAsync($"Plugins.Payments.PayPalCommerce.GooglePay.{resourcePostfix}");
            return (type, price, status, label);
        }

        //display items (subtotal, tax, etc)
        var items = new List<(string Type, string Price, string Status, string Label)>
        {
            await prepareItemAsync("TOTAL", amount.Value, "Total"),
            await prepareItemAsync("SUBTOTAL", amount.Breakdown.ItemTotal.Value, "Subtotal"),
            await prepareItemAsync("TAX", amount.Breakdown.TaxTotal.Value, "Tax"),
            await prepareItemAsync("LINE_ITEM", amount.Breakdown.Shipping.Value, "Shipping"),
            await prepareItemAsync("LINE_ITEM", amount.Breakdown.Discount.Value, "Discount")
        };

        model.Placement = placement;
        model.Country = country;
        model.CurrencyCode = amount.CurrencyCode;
        model.ShippingIsRequired = shippingIsRequired;
        model.Items = items.Where(item => !string.IsNullOrEmpty(item.Type)).ToList();

        return model;
    }

    /// <summary>
    /// Prepare the Google Pay shipping model
    /// </summary>
    /// <param name="model">Google Pay shipping model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the Google Pay shipping model
    /// </returns>
    public async Task<GooglePayShippingModel> PrepareGooglePayShippingModelAsync(GooglePayShippingModel model)
    {
        //prepare updated shipping details
        var (shipping, error) = await _serviceManager.UpdateGoogleShippingAsync(model.Placement,
            (model.AddressCity, model.AddressState, model.AddressCountryCode, model.AddressPostalCode),
            model.OptionId);
        if (!string.IsNullOrEmpty(error) || shipping?.Options is null)
        {
            model.Error = string.IsNullOrEmpty(error)
                ? await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Error")
                : error;
            return model;
        }

        //get common model parameters
        var googlePayModel = await PrepareGooglePayModelAsync(model.Placement, true);
        if (!string.IsNullOrEmpty(googlePayModel.Error))
        {
            model.Error = googlePayModel.Error;
            return model;
        }

        model.Country = googlePayModel.Country;
        model.CurrencyCode = googlePayModel.CurrencyCode;
        model.Items = googlePayModel.Items;
        model.Options = shipping?.Options?.Select(option =>
        {
            var id = $"{option.Id}|{option.Type}";
            var name = $"{option.Amount.Value} {option.Amount.CurrencyCode}: {option.Id}";

            if (string.IsNullOrEmpty(model.OptionId) && option.Selected == true)
                model.OptionId = id;

            return (id, name, option.Label);
        }).ToList();

        return model;
    }

    #endregion

    #region Payment tokens

    /// <summary>
    /// Prepare payment token list model
    /// </summary>
    /// <param name="deleteTokenId">Identifier of the token to delete</param>
    /// <param name="defaultTokenId">Identifier of the token to mark as default</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the payment token list model
    /// </returns>
    public async Task<PaymentTokenListModel> PreparePaymentTokenListModelAsync(int? deleteTokenId = null, int? defaultTokenId = null)
    {
        var model = new PaymentTokenListModel();

        var (active, _) = await _serviceManager.IsActiveAsync(_settings);
        if (!active)
            return model;

        //get all customer's payment tokens
        var (tokens, error) = await _serviceManager.GetPaymentTokensAsync(_settings, true, deleteTokenId, defaultTokenId);
        if (!string.IsNullOrEmpty(error))
        {
            model.VaultIsEnabled = true;
            model.Error = error;
            return model;
        }
        if (!_settings.UseVault && tokens?.Any() != true)
            return model;

        model.VaultIsEnabled = true;
        model.PaymentTokens = tokens.Select(token => new PaymentTokenModel
        {
            Id = token.Id,
            IsPrimaryMethod = token.IsPrimaryMethod,
            Type = token.Type,
            Title = token.Title,
            Expiration = token.Expiration
        }).ToList();

        return model;
    }

    /// <summary>
    /// Prepare saved card list model
    /// </summary>
    /// <param name="placement">Button placement</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the saved card list model
    /// </returns>
    public async Task<SavedCardListModel> PrepareSavedCardListModelAsync(ButtonPlacement placement)
    {
        var model = new SavedCardListModel();

        //get customer's card payment tokens
        var (tokens, error) = await _serviceManager.GetSavedCardsAsync(_settings, placement);
        if (!string.IsNullOrEmpty(error))
        {
            model.VaultIsEnabled = true;
            model.Error = error;
            return model;
        }
        if (tokens?.Any() != true)
            return model;

        model.VaultIsEnabled = true;
        var prefix = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Card.Prefix");
        model.PaymentTokens = tokens.Select(token => new PaymentTokenModel
        {
            Id = token.Id,
            IsPrimaryMethod = token.IsPrimaryMethod,
            Type = token.Type,
            Title = $"{prefix} {token.Title}",
            Expiration = token.Expiration
        }).ToList();

        return model;
    }

    #endregion

    #region Onboarding

    /// <summary>
    /// Prepare the merchant model
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the merchant model
    /// </returns>
    public async Task<MerchantModel> PrepareMerchantModelAsync(PayPalCommerceSettings settings)
    {
        var model = new MerchantModel { Messages = new() { Success = new(), Warning = new(), Error = new() } };

        //get merchant details
        var (merchant, error) = await _serviceManager.GetMerchantAsync(settings);
        if (merchant is null || !string.IsNullOrEmpty(error))
        {
            model.Messages.Error.Add(await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Onboarding.Error"));
            return model;
        }

        model.MerchantId = merchant.MerchantId;
        model.ConfiguratorSupported = merchant.ConfiguratorSupported;

        //check the features availability and prepare warning notifications
        model.AdvancedCardsEnabled = merchant.AdvancedCards.Active;
        if (!merchant.AdvancedCards.Active)
        {
            var message = "You are not able to offer Advanced Credit and Debit Card payments because its onboarding status is {0}. " +
                "Please reach out to PayPal for more information.";
            model.Messages.Warning.Add(string.Format(message, merchant.AdvancedCards.Status));
        }
        model.ApplePayEnabled = merchant.ApplePay.Active;
        if (!merchant.ApplePay.Active)
        {
            var message = "You are not able to offer Apple Pay because its onboarding status is {0}. " +
                "Please reach out to PayPal for more information.";
            model.Messages.Warning.Add(string.Format(message, merchant.ApplePay.Status));
        }
        model.GooglePayEnabled = merchant.GooglePay.Active;
        if (!merchant.GooglePay.Active)
        {
            var message = "You are not able to offer Google Pay because its onboarding status is {0}. " +
                "Please reach out to PayPal for more information.";
            model.Messages.Warning.Add(string.Format(message, merchant.GooglePay.Status));
        }
        model.VaultingEnabled = merchant.Vaulting.Active;
        if (!merchant.Vaulting.Active)
        {
            var message = "You are not able to offer the Vaulting functionality because its onboarding status is {0}. " +
                "Please reach out to PayPal for more information.";
            model.Messages.Warning.Add(string.Format(message, merchant.Vaulting.Status));
        }

        //check special details of "Advanced Cards" feature and prepare warning notifications
        if (merchant.AdvancedCardsDetails.BelowLimit)
        {
            model.Messages.Warning.Add("PayPal requires more information about your business on paypal.com to fully enable " +
                "Advanced Credit and Debit Card Payments beyond a $500 receiving limitation. " +
                "Please visit <a href=\"https://www.paypal.com/policy/hub/kyc\" target=\"_blank\">https://www.paypal.com/policy/hub/kyc</a>. " +
                "After reaching the $500 limit you will still be offering all other PayPal payment methods except " +
                "Advanced Credit and Debit Card Payments to your customers.");
        }
        if (merchant.AdvancedCardsDetails.OverLimit)
        {
            model.Messages.Warning.Add("PayPal requires more information about your business on paypal.com to fully enable " +
                "Advanced Credit and Debit Card Payments beyond a $500 receiving limitation. " +
                "Please visit <a href=\"https://www.paypal.com/policy/hub/kyc\" target=\"_blank\">https://www.paypal.com/policy/hub/kyc</a>. " +
                "You already surpassed the $500 limitation hence aren't able to process more " +
                "Advanced Credit and Debit Card Payments transactions but are still offering all other PayPal payment methods to your customers. " +
                "Once sorted, simply revisit this page to refresh the onboarding status.");
        }
        if (merchant.AdvancedCardsDetails.NeedMoreData)
        {
            model.Messages.Warning.Add("PayPal requires more information about your business on paypal.com to fully enable " +
                "Advanced Credit and Debit Card Payments. " +
                "Please visit <a href=\"https://www.paypal.com/policy/hub/kyc\" target=\"_blank\">https://www.paypal.com/policy/hub/kyc</a>. " +
                "Until then you are still offering all other PayPal payment methods to your customers. " +
                "Once sorted, simply revisit this page to refresh the onboarding status.");
        }
        if (merchant.AdvancedCardsDetails.OnReview)
        {
            model.Messages.Warning.Add("PayPal is currently reviewing your information after which you’ll be notified of your eligibility for " +
                "Advanced Credit and Debit Card Payments. Until then you are still offering all other PayPal payment methods to your customers.");
        }
        if (merchant.AdvancedCardsDetails.Denied)
        {
            model.Messages.Warning.Add(string.Format("PayPal denied your application to use Advanced Credit and Debit Card Payments. " +
                "You can retry in 90 days, on {0} on paypal.com. Until then you are still offering all other " +
                "PayPal payment methods to your customers.", DateTime.UtcNow.AddDays(90).ToShortDateString()));
        }

        //no need to check further details, if the plugin is already connected 
        if (PayPalCommerceServiceManager.IsConnected(settings))
            return model;

        //check merchant status
        model.DisplayStatus = true;
        model.AccountCreated = !string.IsNullOrEmpty(merchant.MerchantId);
        model.EmailConfirmed = merchant.PrimaryEmailConfirmed ?? false;
        model.PaymentsReceivable = merchant.PaymentsReceivable ?? false;
        if (!model.EmailConfirmed || !model.PaymentsReceivable)
        {
            model.Messages.Warning.Add(await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Onboarding.InProcess"));
            if (!model.PaymentsReceivable)
            {
                model.Messages.Warning.Add("Attention: You currently cannot receive payments due to possible restriction on your PayPal account. " +
                    "Please reach out to PayPal Customer Support or connect to " +
                    "<a href=\"https://www.paypal.com/\" target=\"_blank\">https://www.paypal.com/</a> for more information. " +
                    "Once sorted, simply revisit this page to refresh the onboarding status.");
            }
            if (!model.EmailConfirmed)
            {
                model.Messages.Warning.Add("Attention: Please confirm your email address on " +
                    "<a href=\"https://www.paypal.com/businessprofile/settings\" target=\"_blank\">https://www.paypal.com/businessprofile/settings</a>" +
                    " in order to receive payments! You currently cannot receive payments. " +
                    "Once done, simply revisit this page to refresh the onboarding status.");
            }

            return model;
        }

        if (!PayPalCommerceServiceManager.IsConfigured(settings))
        {
            model.Messages.Error.Add(await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Onboarding.Error"));
            return model;
        }

        model.Messages.Success.Add(await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Onboarding.Completed"));

        return model;
    }

    #endregion

    #endregion
}