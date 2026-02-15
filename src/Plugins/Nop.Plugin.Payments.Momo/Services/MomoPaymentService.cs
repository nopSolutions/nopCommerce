using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.Momo.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.Momo.Services;

/// <summary>
/// MTN MoMo payment service implementation
/// </summary>
public class MomoPaymentService : IMomoPaymentService
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly IOrderService _orderService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly MtnMomoConfigHttpClient _momoConfigClient;
    private readonly IMomoTransactionService _transactionService;
    private readonly MomoPaymentClient _momoPaymentClient;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IOrderTotalCalculationService _orderTotalCalculationService;

    #endregion

    #region Ctor

    public MomoPaymentService(
        ILocalizationService localizationService,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        ISettingService settingService,
        IStoreContext storeContext,
        IWorkContext workContext,
        MtnMomoConfigHttpClient momoConfigClient,
        IMomoTransactionService transactionService,
        MomoPaymentClient momoPaymentClient,
        IShoppingCartService shoppingCartService,
        IOrderTotalCalculationService orderTotalCalculationService)
    {
        _localizationService = localizationService;
        _orderProcessingService = orderProcessingService;
        _orderService = orderService;
        _settingService = settingService;
        _storeContext = storeContext;
        _workContext = workContext;
        _momoConfigClient = momoConfigClient;
        _transactionService = transactionService;
        _momoPaymentClient = momoPaymentClient;
        _shoppingCartService = shoppingCartService;
        _orderTotalCalculationService = orderTotalCalculationService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the configuration model
    /// </summary>
    public async Task<ConfigurationModel> GetConfigurationModelAsync()
    {
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var momoSettings = await _settingService.LoadSettingAsync<MomoPaymentSettings>(storeScope);

        var model = new ConfigurationModel
        {
            ActiveStoreScopeConfiguration = storeScope,
            SubscriptionKey = momoSettings.SubscriptionKey,
            ApiUser = momoSettings.ApiUser,
            ApiKey = momoSettings.ApiKey,
            CallbackUrl = momoSettings.CallbackUrl,
            UserCreated = !string.IsNullOrEmpty(momoSettings.ApiUser)
        };

        if (storeScope > 0)
        {
            model.SubscriptionKey_OverrideForStore = await _settingService.SettingExistsAsync(momoSettings, x => x.SubscriptionKey, storeScope);
            model.ApiUser_OverrideForStore = await _settingService.SettingExistsAsync(momoSettings, x => x.ApiUser, storeScope);
            model.ApiKey_OverrideForStore = await _settingService.SettingExistsAsync(momoSettings, x => x.ApiKey, storeScope);
            model.CallbackUrl_OverrideForStore = await _settingService.SettingExistsAsync(momoSettings, x => x.CallbackUrl, storeScope);
        }

        return model;
    }

    /// <summary>
    /// Saves the configuration
    /// </summary>
    public async Task SaveConfigurationAsync(ConfigurationModel model)
    {
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var momoSettings = await _settingService.LoadSettingAsync<MomoPaymentSettings>(storeScope);

        // Update settings
        momoSettings.SubscriptionKey = model.SubscriptionKey;
        momoSettings.CallbackUrl = model.CallbackUrl;

        // Save settings
        await _settingService.SaveSettingOverridablePerStoreAsync(momoSettings, x => x.SubscriptionKey, model.SubscriptionKey_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(momoSettings, x => x.CallbackUrl, model.CallbackUrl_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(momoSettings, x => x.ApiUser, model.ApiUser_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(momoSettings, x => x.ApiKey, model.ApiKey_OverrideForStore, storeScope, false);

        // Clear settings cache
        await _settingService.ClearCacheAsync();
    }

    /// <summary>
    /// Creates an API user
    /// </summary>
    public async Task<(bool Success, string Message)> CreateApiUserAsync(string apiUser)
    {
        try
        {
            if (string.IsNullOrEmpty(apiUser))
                return (false, await _localizationService.GetResourceAsync("Plugins.Payments.Momo.Error.ApiUserRequired"));

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var momoSettings = await _settingService.LoadSettingAsync<MomoPaymentSettings>(storeScope);

            // Update settings with new API User before making the request
            momoSettings.ApiUser = apiUser;
            await _settingService.SaveSettingAsync(momoSettings, x => x.ApiUser, storeScope);
            await _settingService.ClearCacheAsync();

            // Create API User
            var response = await _momoConfigClient.CreateUserAsync(momoSettings);
            if (response.IsSuccessStatusCode)
            {
                return (true, await _localizationService.GetResourceAsync("Plugins.Payments.Momo.ApiUserCreated"));
            }

            // Rollback on failure
            momoSettings.ApiUser = string.Empty;
            await _settingService.SaveSettingAsync(momoSettings, x => x.ApiUser, storeScope);
            await _settingService.ClearCacheAsync();

            var error = await response.Content.ReadAsStringAsync();
            var errorMessage = $"{await _localizationService.GetResourceAsync("Plugins.Payments.Momo.Error.ApiUserCreation")}: {error}";
            return (false, errorMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = $"{await _localizationService.GetResourceAsync("Plugins.Payments.Momo.Error.ApiUserCreation")}: {ex.Message}";
            return (false, errorMessage);
        }
    }

    /// <summary>
    /// Generates an API key
    /// </summary>
    public async Task<(bool Success, string Message)> GenerateApiKeyAsync()
    {
        try
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var momoSettings = await _settingService.LoadSettingAsync<MomoPaymentSettings>(storeScope);

            if (string.IsNullOrEmpty(momoSettings.ApiUser))
                return (false, await _localizationService.GetResourceAsync("Plugins.Payments.Momo.Error.ApiUserRequired"));

            // Generate API Key
            var response = await _momoConfigClient.GenerateApiKeyAsync();
            if (response.IsSuccessStatusCode)
            {
                var apiKeyString = await response.Content.ReadAsStringAsync();
                var apiKeyResponse = JsonConvert.DeserializeObject<ApiKeyResponse>(apiKeyString);

                // Save the API Key
                momoSettings.ApiKey = apiKeyResponse.ApiKey;
                await _settingService.SaveSettingAsync(momoSettings, x => x.ApiKey, storeScope);
                await _settingService.ClearCacheAsync();

                return (true, await _localizationService.GetResourceAsync("Plugins.Payments.Momo.ApiKeyGenerated"));
            }

            var error = await response.Content.ReadAsStringAsync();
            var errorMessage = $"{await _localizationService.GetResourceAsync("Plugins.Payments.Momo.Error.ApiKeyGeneration")}: {error}";
            return (false, errorMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = $"{await _localizationService.GetResourceAsync("Plugins.Payments.Momo.Error.ApiKeyGeneration")}: {ex.Message}";
            return (false, errorMessage);
        }
    }

    /// <summary>
    /// Initiates a payment
    /// </summary>
    /// <remarks>
    /// This method ONLY initiates the payment with MTN MoMo API.
    /// It does NOT create an order. The order is created later during the checkout confirmation
    /// step when ProcessPaymentAsync is called by the NopCommerce framework.
    /// </remarks>
    public async Task<(bool Success, string ReferenceId, string Message)> InitiatePaymentAsync(string phoneNumber)
    {
        try
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return (false, null, "Phone number is required");

            // Format phone number (remove leading zero if present)
            phoneNumber = phoneNumber.TrimStart('0');
            if (!phoneNumber.StartsWith("233"))
                phoneNumber = "233" + phoneNumber;

            // Get current shopping cart
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return (false, null, "Shopping cart is empty");

            // Calculate order total
            var (total, _, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart);
            if (!total.HasValue)
                return (false, null, "Could not calculate order total");

            var orderTotal = total.Value;

            // Make payment request to MoMo API
            var response = await _momoPaymentClient.RequestToPayAsync(phoneNumber, orderTotal);

            if (response.IsSuccessStatusCode)
            {
                // Get reference ID from response headers
                var referenceId = response.Headers.GetValues(MtnMomoDefaults.REFERENCE_ID).FirstOrDefault();
                
                // Create transaction record in database for payment tracking
                // OrderId is set to 0 (null equivalent) since order hasn't been created yet
                // When the order is created during checkout confirmation, we'll link them via the referenceId
                await _transactionService.CreateTransactionAsync(
                    phoneNumber, 
                    orderTotal, 
                    "GHS", 
                    0, // Order will be linked later when created
                    referenceId
                );
                
                return (true, referenceId, "Payment initiated successfully");
            }

            // Read error response
            var errorContent = await response.Content.ReadAsStringAsync();
            return (false, null, $"Failed to initiate payment: {errorContent}");
        }
        catch (Exception ex)
        {
            return (false, null, $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Checks the payment status
    /// </summary>
    /// <remarks>
    /// This method checks the payment status with MTN MoMo API and updates the local transaction record.
    /// It does NOT update any order status - that happens via the callback or when the user confirms payment.
    /// </remarks>
    public async Task<(bool Success, string Status, string Message, string RedirectUrl)> CheckPaymentStatusAsync(string referenceId)
    {
        try
        {
            if (string.IsNullOrEmpty(referenceId))
                return (false, "error", "Reference ID is required", null);

            // First check our local transaction status
            var transaction = await _transactionService.GetTransactionAsync(referenceId);
            if (transaction == null)
                return (false, "error", "Transaction not found", null);

            // Check with MTN MoMo for the latest status
            var response = await _momoPaymentClient.GetPaymentStatusAsync(referenceId);

            // Update our local transaction if we got a successful response
            if (response.Success)
            {
                if (response.Status != transaction.Status)
                {
                    await _transactionService.UpdateTransactionStatusAsync(
                        referenceId,
                        response.Status,
                        response.Message);
                }

                return (true, transaction.Status, "Payment status checked successfully", null);
            }

            // If MTN MoMo check failed, return the latest local status
            return (true, transaction.Status, transaction.ErrorMessage ?? "Payment status is being verified", null);
        }
        catch (Exception ex)
        {
            return (false, "error", $"Error checking payment status: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Links a transaction to an order after the order is created
    /// </summary>
    /// <remarks>
    /// This method is called after an order is successfully created during checkout confirmation.
    /// It links the payment transaction record to the newly created order.
    /// </remarks>
    public async Task<(bool Success, string Message)> LinkTransactionToOrderAsync(string referenceId, int orderId)
    {
        try
        {
            if (string.IsNullOrEmpty(referenceId))
                return (false, "Reference ID is required");

            if (orderId <= 0)
                return (false, "Order ID must be greater than 0");

            var transaction = await _transactionService.GetTransactionAsync(referenceId);
            if (transaction == null)
                return (false, "Transaction not found");

            // Update the transaction with the order ID
            await _transactionService.UpdateTransactionOrderIdAsync(referenceId, orderId);

            // If payment was already successful before the order was created, mark the order as paid
            if (transaction.Status == "SUCCESSFUL")
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order != null && _orderProcessingService.CanMarkOrderAsPaid(order))
                {
                    await _orderProcessingService.MarkOrderAsPaidAsync(order);
                }
            }

            return (true, "Transaction linked to order successfully");
        }
        catch (Exception ex)
        {
            return (false, $"Error linking transaction to order: {ex.Message}");
        }
    }

    /// <summary>
    /// Processes a callback from MTN MoMo
    /// </summary>
    /// <remarks>
    /// When MTN MoMo sends a callback, we update the transaction status.
    /// If an order exists (OrderId > 0), we also update the order status.
    /// If no order exists yet, the status will be updated when the user confirms payment in the UI.
    /// </remarks>
    public async Task<(bool Success, string Message)> ProcessCallbackAsync(MomoCallbackModel model)
    {
        try
        {
            // Validate callback
            if (!await _transactionService.ValidateCallbackSignatureAsync(model))
                return (false, "Invalid signature");

            // Get and validate transaction
            var transaction = await _transactionService.GetTransactionAsync(model.ReferenceId);
            if (transaction == null)
                return (false, "Transaction not found");

            // Update transaction status
            await _transactionService.UpdateTransactionStatusAsync(
                model.ReferenceId,
                model.Status,
                model.Reason);

            // Only update order if it exists (OrderId > 0)
            if (transaction.OrderId > 0)
            {
                var order = await _orderService.GetOrderByIdAsync(transaction.OrderId);
                if (order != null)
                {
                    // Update order status based on payment status
                    switch (model.Status.ToUpperInvariant())
                    {
                        case "SUCCESSFUL":
                            if (_orderProcessingService.CanMarkOrderAsPaid(order))
                            {
                                await _orderProcessingService.MarkOrderAsPaidAsync(order);
                            }
                            break;

                        case "FAILED":
                            if (_orderProcessingService.CanCancelOrder(order))
                            {
                                await _orderProcessingService.CancelOrderAsync(order, true);
                            }
                            break;
                    }
                }
            }

            return (true, "Callback processed successfully");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    #endregion

    #region Classes

    private class ApiKeyResponse
    {
        public string ApiKey { get; set; }
    }

    #endregion
}