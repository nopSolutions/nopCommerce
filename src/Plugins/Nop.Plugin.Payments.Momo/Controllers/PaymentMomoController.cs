using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.Momo.Models;
using Nop.Plugin.Payments.Momo.Services;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.Momo.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class PaymentMomoController : BasePaymentController
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderService _orderService;
    protected readonly IPaymentService _paymentService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly MtnMomoConfigHttpClient _momoConfigClient;
    protected readonly IMomoTransactionService _transactionService;
    protected readonly MomoPaymentClient _momoPaymentClient;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IWorkContext _workContext;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;

    #endregion

    #region Ctor

    public PaymentMomoController(
        ILocalizationService localizationService,
        INotificationService notificationService,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IPaymentService paymentService,
        IPermissionService permissionService,
        ISettingService settingService,
        IStoreContext storeContext,
        MtnMomoConfigHttpClient momoConfigClient,
        MomoPaymentClient momoPaymentClient,
        IMomoTransactionService transactionService,
        IShoppingCartService shoppingCartService, IWorkContext workContext, IOrderTotalCalculationService orderTotalCalculationService)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _orderProcessingService = orderProcessingService;
        _orderService = orderService;
        _paymentService = paymentService;
        _permissionService = permissionService;
        _settingService = settingService;
        _storeContext = storeContext;
        _momoConfigClient = momoConfigClient;
        _momoPaymentClient = momoPaymentClient;
        _transactionService = transactionService;
        _shoppingCartService = shoppingCartService;
        _workContext = workContext;
        _orderTotalCalculationService = orderTotalCalculationService;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure()
    {
        //load settings for a chosen store scope
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

        return View("~/Plugins/Payments.Momo/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

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

        //now clear settings cache
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> CreateApiUser(ConfigurationModel model)
    {
        try
        {
            if (string.IsNullOrEmpty(model.ApiUser))
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Payments.Momo.Error.ApiUserRequired"));
                return RedirectToAction(nameof(Configure));
            }

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var momoSettings = await _settingService.LoadSettingAsync<MomoPaymentSettings>(storeScope);

            // Update settings with new API User before making the request
            momoSettings.ApiUser = model.ApiUser;
            await _settingService.SaveSettingAsync(momoSettings, x => x.ApiUser, storeScope);
            await _settingService.ClearCacheAsync();

            // Create API User
            var response = await _momoConfigClient.CreateUserAsync(momoSettings);
            if (response.IsSuccessStatusCode)
            {
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Payments.Momo.ApiUserCreated"));
            }
            else
            {
                momoSettings.ApiUser = string.Empty;
                await _settingService.SaveSettingAsync(momoSettings, x => x.ApiUser, storeScope);
                await _settingService.ClearCacheAsync();
                var error = await response.Content.ReadAsStringAsync();
                _notificationService.ErrorNotification($"{await _localizationService.GetResourceAsync("Plugins.Payments.Momo.Error.ApiUserCreation")}: {error}");
            }
        }
        catch (Exception ex)
        {
            _notificationService.ErrorNotification($"{await _localizationService.GetResourceAsync("Plugins.Payments.Momo.Error.ApiUserCreation")}: {ex.Message}");
        }

        return RedirectToAction(nameof(Configure));
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> GenerateApiKey(ConfigurationModel model)
    {
        try
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var momoSettings = await _settingService.LoadSettingAsync<MomoPaymentSettings>(storeScope);

            if (string.IsNullOrEmpty(momoSettings.ApiUser))
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Payments.Momo.Error.ApiUserRequired"));
                return RedirectToAction(nameof(Configure));
            }

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

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Payments.Momo.ApiKeyGenerated"));
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _notificationService.ErrorNotification($"{await _localizationService.GetResourceAsync("Plugins.Payments.Momo.Error.ApiKeyGeneration")}: {error}");
            }
        }
        catch (Exception ex)
        {
            _notificationService.ErrorNotification($"{await _localizationService.GetResourceAsync("Plugins.Payments.Momo.Error.ApiKeyGeneration")}: {ex.Message}");
        }

        return RedirectToAction(nameof(Configure));
    }
    #endregion

    #region Public Methods

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> Callback([FromBody] MomoCallbackModel model)
    {
        try
        {
            // Validate callback
            if (!await _transactionService.ValidateCallbackSignatureAsync(model))
                return BadRequest("Invalid signature");

            // Get and validate transaction
            var transaction = await _transactionService.GetTransactionAsync(model.ReferenceId);
            if (transaction == null)
                return NotFound("Transaction not found");

            // Update transaction status
            await _transactionService.UpdateTransactionStatusAsync(
                model.ReferenceId,
                model.Status,
                model.Reason);

            // Get order
            var order = await _orderService.GetOrderByIdAsync(transaction.OrderId);
            if (order == null)
                return NotFound("Order not found");

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

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

[HttpPost]
[IgnoreAntiforgeryToken]
public async Task<IActionResult> InitiatePayment([FromBody] MomoRequest momoRequest)
{
    try
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        
        // Format phone number (remove leading zero if present)
        momoRequest.PhoneNumber = momoRequest.PhoneNumber.TrimStart('0');
        if (!momoRequest.PhoneNumber.StartsWith("233"))
            momoRequest.PhoneNumber = "233" + momoRequest.PhoneNumber;
        
        // Get current shopping cart
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
        
        if (!cart.Any())
            return Json(new { success = false, message = "Shopping cart is empty" });

        // Calculate order total
        var (total, _, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart);
        if (!total.HasValue)
            return Json(new { success = false, message = "Could not calculate order total" });

        var orderTotal = total.Value;            // Make payment request
            var response = await _momoPaymentClient.RequestToPayAsync(
                momoRequest.PhoneNumber,
                orderTotal);

            if (response.IsSuccessStatusCode)
            {
                // Get reference ID from response headers
                var referenceId = response.Headers.GetValues(MtnMomoDefaults.REFERENCE_ID).FirstOrDefault();

                return Json(new
                {
                    success = true,
                    referenceId,
                    message = "Payment initiated successfully"
                });
            }

            // Read error response
            var errorContent = await response.Content.ReadAsStringAsync();
            return Json(new
            {
                success = false,
                message = $"Failed to initiate payment: {errorContent}"
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = $"An error occurred: {ex.Message}"
            });
        }
    }

    [HttpGet]
    public virtual async Task<IActionResult> CheckStatus(string referenceId)
    {
        if (string.IsNullOrEmpty(referenceId))
            return Json(new { success = false, message = "Reference ID is required" });

        try
        {
            // First check our local transaction status
            var transaction = new MomoTransactionModel();/*await _transactionService.GetTransactionAsync(referenceId);*/
            if (transaction == null)
                return Json(new { success = false, status = "error", message = "Transaction not found" });

            // Check with MTN MoMo for the latest status
            var response = await _momoPaymentClient.GetPaymentStatusAsync(referenceId);

            // Update our local transaction if we got a successful response
            if (response.Success)
            {
                if (response.Status != transaction.Status)
                {
                    // transaction = await _transactionService.UpdateTransactionStatusAsync(
                    //     referenceId,
                    //     response.Status,
                    //     response.Message);
                }

                // Return the full status info
                return Json(new
                {
                    success = response.Success,
                    status = transaction.Status,
                    message = "Payment made successfully",
                    redirectUrl = response.Status == "SUCCESSFUL"
                        ? Url.RouteUrl("CheckoutCompleted", new { orderId = transaction.OrderId })
                        : null
                });
            }

            // If MTN MoMo check failed, return the latest local status
            return Json(new
            {
                success = true,
                status = transaction.Status,
                message = transaction.ErrorMessage ?? "Payment status is being verified",
                redirectUrl = transaction.Status == "SUCCESSFUL"
                    ? Url.RouteUrl("CheckoutCompleted", new { orderId = transaction.OrderId })
                    : null
            });
        }
        catch (Exception)
        {
            return Json(new { success = false, status = "error", message = "Error checking payment status" });
        }
    }


    #endregion

    #region Classes

    private class ApiKeyResponse
    {
        public string ApiKey { get; set; }
    }

    public class MomoRequest
    {
        [Required, StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 15 characters long")]
        public string PhoneNumber { get; set; }
    }

    #endregion
}