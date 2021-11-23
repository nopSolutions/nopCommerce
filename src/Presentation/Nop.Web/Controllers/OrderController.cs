using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class OrderController : BasePublicController
    {
        #region Fields

        protected ICustomerService CustomerService { get; }
        protected IOrderModelFactory OrderModelFactory { get; }
        protected IOrderProcessingService OrderProcessingService { get; }
        protected IOrderService OrderService { get; }
        protected IPaymentService PaymentService { get; }
        protected IPdfService PdfService { get; }
        protected IShipmentService ShipmentService { get; }
        protected IWebHelper WebHelper { get; }
        protected IWorkContext WorkContext { get; }
        protected RewardPointsSettings RewardPointsSettings { get; }

        #endregion

		#region Ctor

        public OrderController(ICustomerService customerService,
            IOrderModelFactory orderModelFactory,
            IOrderProcessingService orderProcessingService, 
            IOrderService orderService, 
            IPaymentService paymentService, 
            IPdfService pdfService,
            IShipmentService shipmentService, 
            IWebHelper webHelper,
            IWorkContext workContext,
            RewardPointsSettings rewardPointsSettings)
        {
            CustomerService = customerService;
            OrderModelFactory = orderModelFactory;
            OrderProcessingService = orderProcessingService;
            OrderService = orderService;
            PaymentService = paymentService;
            PdfService = pdfService;
            ShipmentService = shipmentService;
            WebHelper = webHelper;
            WorkContext = workContext;
            RewardPointsSettings = rewardPointsSettings;
        }

        #endregion

        #region Methods

        //My account / Orders
        public virtual async Task<IActionResult> CustomerOrders()
        {
            if (!await CustomerService.IsRegisteredAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            var model = await OrderModelFactory.PrepareCustomerOrderListModelAsync();
            return View(model);
        }

        //My account / Orders / Cancel recurring order
        [HttpPost, ActionName("CustomerOrders")]
        [FormValueRequired(FormValueRequirement.StartsWith, "cancelRecurringPayment")]
        public virtual async Task<IActionResult> CancelRecurringPayment(IFormCollection form)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            //get recurring payment identifier
            var recurringPaymentId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("cancelRecurringPayment", StringComparison.InvariantCultureIgnoreCase))
                    recurringPaymentId = Convert.ToInt32(formValue["cancelRecurringPayment".Length..]);

            var recurringPayment = await OrderService.GetRecurringPaymentByIdAsync(recurringPaymentId);
            if (recurringPayment == null)
            {
                return RedirectToRoute("CustomerOrders");
            }

            if (await OrderProcessingService.CanCancelRecurringPaymentAsync(customer, recurringPayment))
            {
                var errors = await OrderProcessingService.CancelRecurringPaymentAsync(recurringPayment);

                var model = await OrderModelFactory.PrepareCustomerOrderListModelAsync();
                model.RecurringPaymentErrors = errors;

                return View(model);
            }

            return RedirectToRoute("CustomerOrders");
        }

        //My account / Orders / Retry last recurring order
        [HttpPost, ActionName("CustomerOrders")]
        [FormValueRequired(FormValueRequirement.StartsWith, "retryLastPayment")]
        public virtual async Task<IActionResult> RetryLastRecurringPayment(IFormCollection form)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            //get recurring payment identifier
            var recurringPaymentId = 0;
            if (!form.Keys.Any(formValue => formValue.StartsWith("retryLastPayment", StringComparison.InvariantCultureIgnoreCase) &&
                int.TryParse(formValue[(formValue.IndexOf('_') + 1)..], out recurringPaymentId)))
            {
                return RedirectToRoute("CustomerOrders");
            }

            var recurringPayment = await OrderService.GetRecurringPaymentByIdAsync(recurringPaymentId);
            if (recurringPayment == null)
                return RedirectToRoute("CustomerOrders");

            if (!await OrderProcessingService.CanRetryLastRecurringPaymentAsync(customer, recurringPayment))
                return RedirectToRoute("CustomerOrders");

            var errors = await OrderProcessingService.ProcessNextRecurringPaymentAsync(recurringPayment);
            var model = await OrderModelFactory.PrepareCustomerOrderListModelAsync();
            model.RecurringPaymentErrors = errors.ToList();

            return View(model);
        }

        //My account / Reward points
        public virtual async Task<IActionResult> CustomerRewardPoints(int? pageNumber)
        {
            if (!await CustomerService.IsRegisteredAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            if (!RewardPointsSettings.Enabled)
                return RedirectToRoute("CustomerInfo");

            var model = await OrderModelFactory.PrepareCustomerRewardPointsAsync(pageNumber);
            return View(model);
        }

        //My account / Order details page
        public virtual async Task<IActionResult> Details(int orderId)
        {
            var order = await OrderService.GetOrderByIdAsync(orderId);
            var customer = await WorkContext.GetCurrentCustomerAsync();

            if (order == null || order.Deleted || customer.Id != order.CustomerId)
                return Challenge();

            var model = await OrderModelFactory.PrepareOrderDetailsModelAsync(order);
            return View(model);
        }

        //My account / Order details page / Print
        public virtual async Task<IActionResult> PrintOrderDetails(int orderId)
        {
            var order = await OrderService.GetOrderByIdAsync(orderId);
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (order == null || order.Deleted || customer.Id != order.CustomerId)
                return Challenge();

            var model = await OrderModelFactory.PrepareOrderDetailsModelAsync(order);
            model.PrintMode = true;

            return View("Details", model);
        }

        //My account / Order details page / PDF invoice
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> GetPdfInvoice(int orderId)
        {
            var order = await OrderService.GetOrderByIdAsync(orderId);
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (order == null || order.Deleted || customer.Id != order.CustomerId)
                return Challenge();

            var orders = new List<Order>
            {
                order
            };
            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await PdfService.PrintOrdersToPdfAsync(stream, orders, (await WorkContext.GetWorkingLanguageAsync()).Id);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, $"order_{order.CustomOrderNumber}.pdf");
        }

        //My account / Order details page / re-order
        public virtual async Task<IActionResult> ReOrder(int orderId)
        {
            var order = await OrderService.GetOrderByIdAsync(orderId);
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (order == null || order.Deleted || customer.Id != order.CustomerId)
                return Challenge();

            await OrderProcessingService.ReOrderAsync(order);
            return RedirectToRoute("ShoppingCart");
        }

        //My account / Order details page / Complete payment
        [HttpPost, ActionName("Details")]
        
        [FormValueRequired("repost-payment")]
        public virtual async Task<IActionResult> RePostPayment(int orderId)
        {
            var order = await OrderService.GetOrderByIdAsync(orderId);
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (order == null || order.Deleted || customer.Id != order.CustomerId)
                return Challenge();

            if (!await PaymentService.CanRePostProcessPaymentAsync(order))
                return RedirectToRoute("OrderDetails", new { orderId = orderId });

            var postProcessPaymentRequest = new PostProcessPaymentRequest
            {
                Order = order
            };
            await PaymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

            if (WebHelper.IsRequestBeingRedirected || WebHelper.IsPostBeingDone)
            {
                //redirection or POST has been done in PostProcessPayment
                return Content("Redirected");
            }

            //if no redirection has been done (to a third-party payment page)
            //theoretically it's not possible
            return RedirectToRoute("OrderDetails", new { orderId = orderId });
        }

        //My account / Order details page / Shipment details page
        public virtual async Task<IActionResult> ShipmentDetails(int shipmentId)
        {
            var shipment = await ShipmentService.GetShipmentByIdAsync(shipmentId);
            if (shipment == null)
                return Challenge();

            var order = await OrderService.GetOrderByIdAsync(shipment.OrderId);
            var customer = await WorkContext.GetCurrentCustomerAsync();

            if (order == null || order.Deleted || customer.Id != order.CustomerId)
                return Challenge();

            var model = await OrderModelFactory.PrepareShipmentDetailsModelAsync(shipment);
            return View(model);
        }
        
        #endregion
    }
}