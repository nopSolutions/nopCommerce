using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Paystack.Models;
using Nop.Plugin.Payments.Paystack.Services;
using Nop.Services.Configuration;
using Nop.Services.Orders;

namespace Nop.Plugin.Payments.Paystack.Controllers;

/// <summary>
/// Paystack callback controller for popup payment flow
/// </summary>
[AllowAnonymous]
[AutoValidateAntiforgeryToken]
public class PaystackCallbackController : Controller
{
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly IOrderService _orderService;
    private readonly ISettingService _settingService;
    private readonly IPaystackTransactionService _transactionService;
    private readonly PaystackPaymentClient _paymentClient;

    public PaystackCallbackController(
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        ISettingService settingService,
        IPaystackTransactionService transactionService,
        PaystackPaymentClient paymentClient)
    {
        _orderProcessingService = orderProcessingService;
        _orderService = orderService;
        _settingService = settingService;
        _transactionService = transactionService;
        _paymentClient = paymentClient;
    }

    /// <summary>
    /// Customer redirect after popup payment completion
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> CallbackAsync(string reference, string? trxref = null)
    {
        var refToUse = !string.IsNullOrWhiteSpace(reference) ? reference : trxref;
        if (string.IsNullOrWhiteSpace(refToUse))
            return RedirectToRoute("Homepage");

        var transaction = await _transactionService.GetTransactionAsync(refToUse);
        if (transaction == null)
            return RedirectToRoute("Homepage");

        var order = await _orderService.GetOrderByIdAsync(transaction.OrderId);
        if (order == null)
            return RedirectToRoute("Homepage");

        if (order.PaymentStatus == PaymentStatus.Paid)
        {
            return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
        }

        var verify = await _paymentClient.VerifyTransactionAsync(refToUse);
        
        if (verify?.Status != true || verify.Data == null)
        {
            await _transactionService.UpdateTransactionStatusAsync(refToUse, "failed", verify?.Message);
            Response.Cookies.Delete("PaystackPopup");
            return RedirectToRoute("OrderDetails", new { orderId = order.Id });
        }

        var data = verify.Data;
        await _transactionService.UpdateTransactionStatusAsync(refToUse, data.Status, data.GatewayResponse);

        if (string.Equals(data.Status, "success", StringComparison.OrdinalIgnoreCase) && 
            _orderProcessingService.CanMarkOrderAsPaid(order))
        {
            await _orderProcessingService.MarkOrderAsPaidAsync(order);
        }

        return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
    }

    /// <summary>
    /// Webhook for payment status updates (optional but recommended)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Webhook()
    {
        string payload;
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            payload = await reader.ReadToEndAsync();

        var webhookEvent = JsonConvert.DeserializeObject<PaystackWebhookEvent>(payload);
        if (webhookEvent?.Data == null)
            return Ok();

        var reference = webhookEvent.Data.Reference;
        var transaction = await _transactionService.GetTransactionAsync(reference);
        if (transaction == null)
            return Ok();

        var order = await _orderService.GetOrderByIdAsync(transaction.OrderId);
        if (order == null)
            return Ok();

        // Validate webhook signature if enabled
        var settings = await _settingService.LoadSettingAsync<PaystackPaymentSettings>(order.StoreId);
        var secret = !string.IsNullOrEmpty(settings.WebhookSecret) ? settings.WebhookSecret : settings.SecretKey;
        var signature = Request.Headers["X-Paystack-Signature"].FirstOrDefault();
        
        if (settings.EnableWebhookValidation && 
            !string.IsNullOrEmpty(secret) && 
            !_transactionService.ValidateWebhookSignature(payload, signature ?? string.Empty, secret))
        {
            return StatusCode(401);
        }

        // Skip if already paid
        if (order.PaymentStatus == PaymentStatus.Paid)
            return Ok();

        await _transactionService.UpdateTransactionStatusAsync(reference, webhookEvent.Data.Status, webhookEvent.Data.GatewayResponse);

        if (string.Equals(webhookEvent.Data.Status, "success", StringComparison.OrdinalIgnoreCase) && 
            _orderProcessingService.CanMarkOrderAsPaid(order))
        {
            await _orderProcessingService.MarkOrderAsPaidAsync(order);
        }

        return Ok();
    }
 
    /// <summary>
    /// Popup complete page - closes popup and redirects parent
    /// </summary>
    [HttpGet]
    public IActionResult Complete(int orderId, string? status = null)
    {
        var redirectUrl = string.Equals(status, "failed", StringComparison.OrdinalIgnoreCase)
            ? Url.RouteUrl("OrderDetails", new { orderId })
            : Url.RouteUrl("CheckoutCompleted", new { orderId });
            
        return View("~/Plugins/Payments.Paystack/Views/Complete.cshtml", redirectUrl ?? "");
    }
}
