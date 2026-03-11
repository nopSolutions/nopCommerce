using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Paystack.Models;
using Nop.Plugin.Payments.Paystack.Services;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.Paystack.Controllers;

/// <summary>
/// Paystack popup controller for handling popup-based payments
/// </summary>
[AutoValidateAntiforgeryToken]
public class PaystackController : BasePluginController
{
    private readonly IPaystackTransactionService _transactionService;
    private readonly IStoreContext _storeContext;
    private readonly ISettingService _settingService;
    private readonly IOrderService _orderService;

    public PaystackController(
        IPaystackTransactionService transactionService,
        IStoreContext storeContext,
        ISettingService settingService,
        IOrderService orderService
    )
    {
        _transactionService = transactionService;
        _storeContext = storeContext;
        _settingService = settingService;
        _orderService = orderService;
    }

    /// <summary>
    /// Show popup page with Paystack JavaScript
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ShowPopupAsync(string accessCode, string reference, int orderId)
    {
        if (string.IsNullOrWhiteSpace(accessCode) || string.IsNullOrWhiteSpace(reference) || orderId <= 0)
            return RedirectToRoute("Homepage");

        var transaction = await _transactionService.GetTransactionAsync(reference);
        if (transaction == null || transaction.OrderId != orderId)
            return RedirectToRoute("Homepage");

        var store = await _storeContext.GetCurrentStoreAsync();
        var settings = await _settingService.LoadSettingAsync<PaystackPaymentSettings>(store.Id);

        var model = new ShowPopupModel
        {
            AccessCode = accessCode,
            Reference = reference,
            OrderId = orderId,
            CallbackUrl = Url.Action("Callback", "PaystackCallback", null, Request.Scheme)
        };

        return View("~/Plugins/Payments.PayStack/Views/ShowPopup.cshtml", model);
    }

    /// <summary>
    /// Cancel payment and cancel the order
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> CancelPaymentAsync(int orderId)
    {
        if (orderId <= 0)
            return RedirectToRoute("Homepage");

        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
            return RedirectToRoute("Homepage");

        if (order.OrderStatus != OrderStatus.Pending)
        {
            return RedirectToRoute("OrderDetails", new { orderId });
        }
        
        order.OrderStatus = OrderStatus.Cancelled;
        order.PaymentStatus = PaymentStatus.Voided;
            
        await _orderService.UpdateOrderAsync(order);
            
        await _orderService.InsertOrderNoteAsync(new OrderNote
        {
            OrderId = order.Id,
            Note = "Payment cancelled by user at Paystack popup",
            DisplayToCustomer = true,
            CreatedOnUtc = DateTime.UtcNow
        });

        return RedirectToRoute("OrderDetails", new { orderId });
    }
}
