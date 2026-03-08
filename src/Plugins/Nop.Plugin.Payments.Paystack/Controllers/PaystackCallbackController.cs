using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core.Http;
using Nop.Plugin.Payments.Paystack.Models;
using Nop.Plugin.Payments.Paystack.Services;
using Nop.Services.Configuration;
using Nop.Services.Orders;

namespace Nop.Plugin.Payments.Paystack.Controllers;

/// <summary>
/// Public controller for Paystack redirect callback and webhook (no admin auth).
/// </summary>
[AllowAnonymous]
[IgnoreAntiforgeryToken]
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
    /// Customer redirect after payment. Paystack sends ?reference=xxx (and optionally trxref=xxx).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Callback(string reference, string? trxref = null)
    {
        var refToUse = !string.IsNullOrWhiteSpace(reference) ? reference : trxref;
        if (string.IsNullOrWhiteSpace(refToUse))
            return RedirectToRoute("Homepage");

        var transaction = await _transactionService.GetTransactionAsync(refToUse.Trim());
        if (transaction == null)
            return RedirectToRoute("Homepage");

        var order = await _orderService.GetOrderByIdAsync(transaction.OrderId);
        if (order == null)
            return RedirectToRoute("Homepage");

        var isPopup = string.Equals(Request.Cookies["PaystackPopup"], "1", StringComparison.OrdinalIgnoreCase);

        // Already paid (e.g. via webhook)
        if (order.PaymentStatus == Core.Domain.Payments.PaymentStatus.Paid)
        {
            if (isPopup)
            {
                Response.Cookies.Delete("PaystackPopup");
                return RedirectToRoute(PaystackDefaults.CompleteRouteName, new { orderId = order.Id });
            }
            return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
        }

        var settings = await _settingService.LoadSettingAsync<PaystackPaymentSettings>(order.StoreId);
        var verify = await _paymentClient.VerifyTransactionAsync(settings, refToUse.Trim());
        if (verify?.Status != true || verify.Data == null)
        {
            await _transactionService.UpdateTransactionStatusAsync(refToUse, "failed", verify?.Message);
            if (isPopup)
            {
                Response.Cookies.Delete("PaystackPopup");
                return RedirectToRoute(PaystackDefaults.CompleteRouteName, new { orderId = order.Id, status = "failed" });
            }
            return RedirectToRoute("OrderDetails", new { orderId = order.Id });
        }

        var data = verify.Data;
        await _transactionService.UpdateTransactionStatusAsync(refToUse, data.Status, data.GatewayResponse);

        if (string.Equals(data.Status, "success", StringComparison.OrdinalIgnoreCase) && _orderProcessingService.CanMarkOrderAsPaid(order))
            await _orderProcessingService.MarkOrderAsPaidAsync(order);

        if (isPopup)
        {
            Response.Cookies.Delete("PaystackPopup");
            return RedirectToRoute(PaystackDefaults.CompleteRouteName, new { orderId = order.Id });
        }

        return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
    }

    /// <summary>
    /// Webhook: Paystack sends POST with JSON body and X-Paystack-Signature (HMAC SHA512 of body).
    /// Return 200 OK quickly; process asynchronously if needed.
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

        var settings = await _settingService.LoadSettingAsync<PaystackPaymentSettings>(order.StoreId);
        var secret = !string.IsNullOrEmpty(settings.WebhookSecret) ? settings.WebhookSecret : settings.SecretKey;
        var signature = Request.Headers["X-Paystack-Signature"].FirstOrDefault();
        if (settings.EnableWebhookValidation && !string.IsNullOrEmpty(secret) && !_transactionService.ValidateWebhookSignature(payload, signature ?? string.Empty, secret))
            return StatusCode(401);

        if (order.PaymentStatus == Core.Domain.Payments.PaymentStatus.Paid)
            return Ok();

        await _transactionService.UpdateTransactionStatusAsync(reference, webhookEvent.Data.Status, webhookEvent.Data.GatewayResponse);

        if (string.Equals(webhookEvent.Data.Status, "success", StringComparison.OrdinalIgnoreCase) && _orderProcessingService.CanMarkOrderAsPaid(order))
            await _orderProcessingService.MarkOrderAsPaidAsync(order);

        return Ok();
    }

    /// <summary>
    /// Popup complete page: redirects opener to order page and closes the popup.
    /// </summary>
    [HttpGet]
    public IActionResult Complete(int orderId, string? status = null)
    {
        var redirectUrl = string.Equals(status, "failed", StringComparison.OrdinalIgnoreCase)
            ? Url.RouteUrl("OrderDetails", new { orderId })
            : Url.RouteUrl(NopRouteNames.Standard.CHECKOUT_COMPLETED, new { orderId });
        return View("~/Plugins/Payments.Paystack/Views/Complete.cshtml", redirectUrl ?? "");
    }
}
