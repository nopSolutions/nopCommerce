using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Tax;

namespace Nop.Plugin.Payments.Iyzico.Controllers
{
    public class PaymentIyzicoPCController : Controller
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly OrderSettings _orderSettings;
        private readonly IyzicoPaymentSettings _iyzicoPaymentSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly ITaxService _taxService;
        private readonly IShipmentService _shipmentService;
        private readonly ILocalizationService _localizationService;
        private readonly IGiftCardService _giftCardService;
        private readonly ILanguageService _languageService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IEncryptionService _encryptionService;

        #endregion

        #region Ctor

        public PaymentIyzicoPCController(ILogger logger,
            IHttpContextAccessor httpContextAccessor,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            ICustomerService customerService,
            IWorkContext workContext,
            IStoreContext storeContext,
            OrderSettings orderSettings,
            IyzicoPaymentSettings iyzicoPaymentSettings,
            LocalizationSettings localizationSettings,
            IShoppingCartService shoppingCartService,
            IProductService productService,
            ITaxService taxService,
            IShipmentService shipmentService,
            ILocalizationService localizationService,
            IGiftCardService giftCardService,
            ILanguageService languageService,
            IWorkflowMessageService workflowMessageService,
            IPaymentService paymentService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IRewardPointService rewardPointService,
            IEncryptionService encryptionService)
        {
            _orderProcessingService = orderProcessingService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _orderService = orderService;
            _customerService = customerService;
            _localizationService = localizationService;
            _localizationSettings = localizationSettings;
            _workContext = workContext;
            _storeContext = storeContext;
            _orderSettings = orderSettings;
            _iyzicoPaymentSettings = iyzicoPaymentSettings;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _taxService = taxService;
            _shipmentService = shipmentService;
            _localizationService = localizationService;
            _giftCardService = giftCardService;
            _languageService = languageService;
            _workflowMessageService = workflowMessageService;
            _paymentService = paymentService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _rewardPointService = rewardPointService;
            _encryptionService = encryptionService;
        }

        #endregion

        #region Methods

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IActionResult> PaymentConfirm(string orderGuid)
        {
            //validation
            if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            Order order = null;

            if (string.IsNullOrEmpty(orderGuid) == false)
            {
                //load order by identifier (if provided)
                order = await _orderService.GetOrderByGuidAsync(Guid.Parse(orderGuid));
            }

            if (order == null)
            {
                order = (await _orderService.SearchOrdersAsync(storeId: order.StoreId,
                customerId: order.CustomerId)).FirstOrDefault();
            }

            if (order != null)
            {
                RetrieveCheckoutFormRequest request = new()
                {
                    ConversationId = order.CaptureTransactionId,
                    Token = order.CaptureTransactionResult
                };

                CheckoutForm checkoutForm = CheckoutForm.Retrieve(request, IyzicoHelper.GetOptions(_iyzicoPaymentSettings));

                List<string> errorStr = new();

                if (string.IsNullOrEmpty(checkoutForm.ErrorMessage) == false)
                {
                    errorStr.Add(checkoutForm.ErrorMessage);
                }

                order.PaymentStatus = IyzicoHelper.GetPaymentStatus(checkoutForm.Status, checkoutForm.PaymentStatus);

                switch (order.PaymentStatus)
                {
                    case PaymentStatus.Authorized:
                    case PaymentStatus.Paid:

                        if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
                        {
                            await _orderProcessingService.MarkAsAuthorizedAsync(order);
                        }

                        if (_orderProcessingService.CanMarkOrderAsPaid(order))
                        {
                            await _orderProcessingService.MarkOrderAsPaidAsync(order);
                        }

                        if (_iyzicoPaymentSettings.IsCardStorage)
                        {
                            order.OrderStatus = OrderStatus.Processing;
                            order.CardNumber = _encryptionService.EncryptText($"{checkoutForm.BinNumber}******{checkoutForm.LastFourDigits}");
                            order.CardType = _encryptionService.EncryptText(checkoutForm.CardAssociation.Replace("_", " "));
                            order.CardName = _encryptionService.EncryptText(checkoutForm.CardFamily);
                        }

                        await _orderService.UpdateOrderAsync(order);

                        List<string> PaymentInformation = new()
                        {
                            $"Iyzico Ödeme Id :  {checkoutForm.PaymentId}",
                            $"Tahsilat Tutarı :  {checkoutForm.PaidPrice:C2}",
                            $"İşlem Ücreti :  {checkoutForm.IyziCommissionRateAmount:C2}",
                            $"Komisyon Ücreti :  {checkoutForm.IyziCommissionFee:C2}"
                        };

                        checkoutForm.PaymentItems.ForEach((item) =>
                        {
                            PaymentInformation.Add($"Ürün Id: {item.ItemId} - Ürün Tutarı : {item.PaidPrice} - İşlem Kimliği : {item.PaymentTransactionId}");
                        });

                        //order note
                        await _orderService.InsertOrderNoteAsync(new OrderNote
                        {
                            OrderId = order.Id,
                            Note = string.Join(" | ", PaymentInformation),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });

                        return Redirect(_iyzicoPaymentSettings.PaymentSuccessUrl);

                    case PaymentStatus.Refunded:

                        order.OrderStatus = OrderStatus.Cancelled;

                        break;

                    case PaymentStatus.Pending:
                    case PaymentStatus.Voided:

                        if (_orderProcessingService.CanVoidOffline(order))
                            await _orderProcessingService.VoidOfflineAsync(order);

                        await MoveShoppingCartItemsToOrderItemsAsync(order);
                        order.OrderStatus = OrderStatus.Cancelled;

                        errorStr.Add($"Ödeme başarısız. Sipariş # {order.Id}.");

                        break;
                }

                await _orderService.UpdateOrderAsync(order);

                //sadece 1 olan işlemlerde ürünü kargoya vermelidir, 0 olan işlemler için bilgilendirme beklemelidir.
                switch (checkoutForm.FraudStatus)
                {
                    case -1://fraud risk skoru yüksek ise ödeme işlemi reddedilir ve -1 döner. 
                        errorStr.Add($"Iyzico: İşleme ait Fraud riski yüksek olduğu için ödeme kabul edilmemiştir. Sipariş # {order.Id}.");
                        break;
                    case 0://ödeme işlemi daha sonradan incelenip karar verilecekse 0 döner.
                        errorStr.Add($"Iyzico: İşleme ait Fraud riski bulunduğu için ödeme incelemeye alınmıştır. Sipariş # {order.Id}.");
                        break;
                }

                if (errorStr.Any())
                {
                    errorStr.ForEach((err) =>
                    {
                        //log
                        _logger.ErrorAsync(err);
                        //order note
                        _orderService.InsertOrderNoteAsync(new OrderNote
                        {
                            OrderId = order.Id,
                            Note = err,
                            DisplayToCustomer = true,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                    });

                    _httpContextAccessor.HttpContext.Response.Cookies.Append("iyzicoMessage", string.Join("<br>", errorStr));
                }
            }
            else
            {
                await _logger.ErrorAsync($"Iyzico: Sipariş Bulunamadı! Sipariş #{orderGuid}");

                return RedirectToAction("History", "Order");
            }

            return Redirect(_iyzicoPaymentSettings.PaymentErrorUrl);
        }

        protected virtual async Task MoveShoppingCartItemsToOrderItemsAsync(Order order)
        {
            var Customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            var CartString = _httpContextAccessor.HttpContext.Request.Cookies["CurrentShopCartTemp"];

            if (string.IsNullOrEmpty(CartString) == false)
            {
                IList<ShoppingCartItem> CartList = JsonConvert.DeserializeObject<IList<ShoppingCartItem>>(CartString);

                if (CartList != null && CartList.Any())
                {

                    _httpContextAccessor.HttpContext.Response.Cookies.Delete("CurrentShopCartTemp");

                    //add shopping cart
                    CartList.ToList().ForEach(async (item) =>
                    {
                        var product = await _productService.GetProductByIdAsync(item.ProductId);

                        await _shoppingCartService.AddToCartAsync(customer: Customer,
                            product: product,
                            shoppingCartType: item.ShoppingCartType,
                            storeId: order.StoreId,
                            attributesXml: item.AttributesXml,
                            quantity: item.Quantity);
                    });

                    await DeleteOrderAsync(order);
                }
            }
        }

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task DeleteOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //check whether the order wasn't cancelled before
            //if it already was cancelled, then there's no need to make the following adjustments
            //(such as reward points, inventory, recurring payments)
            //they already was done when cancelling the order
            if (order.OrderStatus != OrderStatus.Cancelled)
            {
                //return (add) back redeemded reward points
                await ReturnBackRedeemedRewardPointsAsync(order);
                //reduce (cancel) back reward points (previously awarded for this order)
                await ReduceRewardPointsAsync(order);

                //cancel recurring payments
                var recurringPayments = await _orderService.SearchRecurringPaymentsAsync(initialOrderId: order.Id);
                foreach (var rp in recurringPayments)
                    await CancelRecurringPaymentAsync(rp);

                //Adjust inventory for already shipped shipments
                //only products with "use multiple warehouses"
                foreach (var shipment in await _shipmentService.GetShipmentsByOrderIdAsync(order.Id))
                {
                    foreach (var shipmentItem in await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id))
                    {
                        var product = await _orderService.GetProductByOrderItemIdAsync(shipmentItem.OrderItemId);
                        if (product == null)
                            continue;

                        await _productService.ReverseBookedInventoryAsync(product, shipmentItem,
                            string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteOrder"), order.Id));
                    }
                }

                //Adjust inventory
                foreach (var orderItem in await _orderService.GetOrderItemsAsync(order.Id))
                {
                    var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                    await _productService.AdjustInventoryAsync(product, orderItem.Quantity, orderItem.AttributesXml,
                        string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteOrder"), order.Id));
                }
            }

            //deactivate gift cards
            if (_orderSettings.DeactivateGiftCardsAfterDeletingOrder)
                await SetActivatedValueForPurchasedGiftCardsAsync(order, false);

            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "Ödeme başarısız olduğu için sipariş silinmiştir.",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            //now delete an order
            await _orderService.DeleteOrderAsync(order);
        }

        /// <summary>
        /// Set IsActivated value for purchase gift cards for particular order
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="activate">A value indicating whether to activate gift cards; true - activate, false - deactivate</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task SetActivatedValueForPurchasedGiftCardsAsync(Order order, bool activate)
        {
            var giftCards = await _giftCardService.GetAllGiftCardsAsync(order.Id, isGiftCardActivated: !activate);
            foreach (var gc in giftCards)
            {
                if (activate)
                {
                    //activate
                    var isRecipientNotified = gc.IsRecipientNotified;
                    if (gc.GiftCardType == GiftCardType.Virtual)
                    {
                        //send email for virtual gift card
                        if (!string.IsNullOrEmpty(gc.RecipientEmail) &&
                            !string.IsNullOrEmpty(gc.SenderEmail))
                        {
                            var customerLang = await _languageService.GetLanguageByIdAsync(order.CustomerLanguageId) ??
                                               (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();
                            if (customerLang == null)
                                throw new Exception("No languages could be loaded");
                            var queuedEmailIds = await _workflowMessageService.SendGiftCardNotificationAsync(gc, customerLang.Id);
                            if (queuedEmailIds.Any())
                                isRecipientNotified = true;
                        }
                    }

                    gc.IsGiftCardActivated = true;
                    gc.IsRecipientNotified = isRecipientNotified;
                    await _giftCardService.UpdateGiftCardAsync(gc);
                }
                else
                {
                    //deactivate
                    gc.IsGiftCardActivated = false;
                    await _giftCardService.UpdateGiftCardAsync(gc);
                }
            }
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IList<string>> CancelRecurringPaymentAsync(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            var initialOrder = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId);
            if (initialOrder == null)
                return new List<string> { "Initial order could not be loaded" };

            var request = new CancelRecurringPaymentRequest();
            CancelRecurringPaymentResult result = null;
            try
            {
                request.Order = initialOrder;
                result = await _paymentService.CancelRecurringPaymentAsync(request);
                if (result.Success)
                {
                    //update recurring payment
                    recurringPayment.IsActive = false;
                    await _orderService.UpdateRecurringPaymentAsync(recurringPayment);

                    //add a note
                    await _orderService.InsertOrderNoteAsync(new OrderNote
                    {
                        OrderId = initialOrder.Id,
                        Note = "Recurring payment has been cancelled",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });

                    //notify a store owner
                    await _workflowMessageService
                        .SendRecurringPaymentCancelledStoreOwnerNotificationAsync(recurringPayment,
                        _localizationSettings.DefaultAdminLanguageId);
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new CancelRecurringPaymentResult();
                result.AddError($"Error: {exc.Message}. Full exception: {exc}");
            }

            //process errors
            var error = string.Empty;
            for (var i = 0; i < result.Errors.Count; i++)
            {
                error += $"Error {i}: {result.Errors[i]}";
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }

            if (string.IsNullOrEmpty(error))
                return result.Errors;

            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = initialOrder.Id,
                Note = $"Unable to cancel recurring payment. {error}",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            //log it
            var logError = $"Error cancelling recurring payment. Order #{initialOrder.Id}. Error: {error}";
            await _logger.InsertLogAsync(LogLevel.Error, logError, logError);
            return result.Errors;
        }

        /// <summary>
        /// Reduce (cancel) reward points (previously awarded for placing an order)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task ReduceRewardPointsAsync(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            var totalForRewardPoints = _orderTotalCalculationService
                .CalculateApplicableOrderTotalForRewardPoints(order.OrderShippingInclTax, order.OrderTotal);
            var points = totalForRewardPoints > decimal.Zero ?
                await _orderTotalCalculationService.CalculateRewardPointsAsync(customer, totalForRewardPoints) : 0;
            if (points == 0)
                return;

            //ensure that reward points were already earned for this order before
            if (!order.RewardPointsHistoryEntryId.HasValue)
                return;

            //get appropriate history entry
            var rewardPointsHistoryEntry = await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RewardPointsHistoryEntryId.Value);
            if (rewardPointsHistoryEntry != null && rewardPointsHistoryEntry.CreatedOnUtc > DateTime.UtcNow)
            {
                //just delete the upcoming entry (points were not granted yet)
                await _rewardPointService.DeleteRewardPointsHistoryEntryAsync(rewardPointsHistoryEntry);
            }
            else
            {
                //or reduce reward points if the entry already exists
                await _rewardPointService.AddRewardPointsHistoryEntryAsync(customer, -points, order.StoreId,
                    string.Format(await _localizationService.GetResourceAsync("RewardPoints.Message.ReducedForOrder"), order.CustomOrderNumber));
            }
        }

        /// <summary>
        /// Return back redeemed reward points to a customer (spent when placing an order)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task ReturnBackRedeemedRewardPointsAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            //were some reward points spend on the order
            var allRewardPoints = await _rewardPointService.GetRewardPointsHistoryAsync(order.CustomerId, order.StoreId, orderGuid: order.OrderGuid);
            if (allRewardPoints?.Any() == true)
            {
                // Here we get the wrong balance of bonus points and return the wrong number of points to the buyer's account, 
                // we need to wait one second, because when canceling an order, debiting and accruing bonuses takes less than one second, 
                // this is because the RewardPointsHistory.CreatedOnUtc property is created / mapped with no fraction for the datetime type, 
                // we should fix this in the next version.
                // https://github.com/nopSolutions/nopCommerce/issues/5595
                await Task.Delay(1000);

                foreach (var rewardPoints in allRewardPoints)
                    //return back
                    await _rewardPointService.AddRewardPointsHistoryEntryAsync(customer, -rewardPoints.Points, order.StoreId,
                        string.Format(await _localizationService.GetResourceAsync("RewardPoints.Message.ReturnedForOrder"), order.CustomOrderNumber));
            }
        }

        #endregion
    }
}