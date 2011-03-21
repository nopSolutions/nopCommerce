using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Payments;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order service
    /// </summary>
    public partial class OrderService : IOrderService
    {
        #region Fields

        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderProductVariant> _opvRepository;
        private readonly IRepository<ProductVariant> _pvRepository;
        private readonly IRepository<RecurringPayment> _recurringPaymentRepository;
        private readonly IRepository<RecurringPaymentHistory> _recurringPaymentHistoryRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly OrderSettings _orderSettings;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="orderRepository">Order repository</param>
        /// <param name="opvRepository">Order product variant repository</param>
        /// <param name="pvRepository">Product variant repository</param>
        /// <param name="recurringPaymentRepository">Recurring payment repository</param>
        /// <param name="recurringPaymentHistoryRepository">Recurring payment history repository</param>
        /// <param name="customerRepository">Customer repository</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="productService">Product service</param>
        /// <param name="paymentService">Payment service</param>
        /// <param name="logger">Logger</param>
        /// <param name="orderTotalCalculationService">Order total calculationservice</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="orderSettings">Order settings</param>
        public OrderService(IRepository<Order> orderRepository,
            IRepository<OrderProductVariant> opvRepository,
            IRepository<ProductVariant> pvRepository,
            IRepository<RecurringPayment> recurringPaymentRepository,
            IRepository<RecurringPaymentHistory> recurringPaymentHistoryRepository,
            IRepository<Customer> customerRepository,
            ILocalizationService localizationService,
            IProductService productService,
            IPaymentService paymentService,
            ILogger logger,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPriceFormatter priceFormatter,
            RewardPointsSettings rewardPointsSettings,
            OrderSettings orderSettings)
        {
            this._orderRepository = orderRepository;
            this._opvRepository = opvRepository;
            this._pvRepository = pvRepository;
            this._recurringPaymentRepository = recurringPaymentRepository;
            this._recurringPaymentHistoryRepository = recurringPaymentHistoryRepository;
            this._customerRepository = customerRepository;
            this._localizationService = localizationService;
            this._productService = productService;
            this._paymentService = paymentService;
            this._logger = logger;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._priceFormatter = priceFormatter;
            this._rewardPointsSettings = rewardPointsSettings;
            this._orderSettings = orderSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Sets an order status
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="os">New order status</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        protected void SetOrderStatus(Order order,
            OrderStatus os, bool notifyCustomer)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            OrderStatus prevOrderStatus = order.OrderStatus;
            if (prevOrderStatus == os)
                return;

            //set and save new order status
            order.OrderStatusId = (int)os;
            UpdateOrder(order);

            //order notes, notifications
            order.OrderNotes.Add(new OrderNote()
                {
                    Note = string.Format("Order status has been changed to {0}", os.ToString()),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
            UpdateOrder(order);


            if (prevOrderStatus != OrderStatus.Complete &&
                os == OrderStatus.Complete
                && notifyCustomer)
            {
                //TODO implement notification
                //int orderCompletedCustomerNotificationQueuedEmailId = _messageService.SendOrderCompletedCustomerNotification(order, order.CustomerLanguageId);
                //if (orderCompletedCustomerNotificationQueuedEmailId > 0)
                //{
                //    order.OrderNotes.Add(new OrderNote()
                //    {
                //        Note = string.Format("\"Order completed\" email (to customer) has been queued. Queued email identifier: {0}.", orderCompletedCustomerNotificationQueuedEmailId),
                //        DisplayToCustomer = false,
                //        CreatedOnUtc = DateTime.UtcNow
                //    });
                //    UpdateOrder(order);
                //}
            }

            if (prevOrderStatus != OrderStatus.Cancelled &&
                os == OrderStatus.Cancelled
                && notifyCustomer)
            {
                //TODO implement notification
                //int orderCancelledCustomerNotificationQueuedEmailId = _messageService.SendOrderCancelledCustomerNotification(order, order.CustomerLanguageId);
                //if (orderCancelledCustomerNotificationQueuedEmailId > 0)
                //{
                //    order.OrderNotes.Add(new OrderNote()
                //    {
                //        Note = string.Format("\"Order cancelled\" email (to customer) has been queued. Queued email identifier: {0}.", orderCancelledCustomerNotificationQueuedEmailId),
                //        DisplayToCustomer = false,
                //        CreatedOnUtc = DateTime.UtcNow
                //    });
                //    UpdateOrder(order);
                //}
            }

            //reward points
            if (_rewardPointsSettings.Enabled)
            {
                if (_rewardPointsSettings.PointsForPurchases_Amount > decimal.Zero)
                {
                    //Ensure that reward points are applied only to registered users
                    if (order.Customer != null)
                    {
                        //UNDONE ensure customer is not guest !order.Customer.IsGuest
                        int points = (int)Math.Truncate(order.OrderTotal / _rewardPointsSettings.PointsForPurchases_Amount * _rewardPointsSettings.PointsForPurchases_Points);
                        if (points != 0)
                        {
                            if (_rewardPointsSettings.PointsForPurchases_Awarded == order.OrderStatus)
                            {
                                order.Customer.AddRewardPointsHistoryEntry(points, 
                                    string.Format(_localizationService.GetResource("RewardPoints.Message.EarnedForOrder"), order.Id));
                                UpdateOrder(order);
                            }


                            if (_rewardPointsSettings.PointsForPurchases_Canceled == order.OrderStatus)
                            {
                                order.Customer.AddRewardPointsHistoryEntry(-points,
                                       string.Format(_localizationService.GetResource("RewardPoints.Message.ReducedForOrder"), order.Id));
                                UpdateOrder(order);
                            }
                        }
                    }
                }
            }

            //TODO implement gift cards activation
            //if (this.GiftCards_Activated.HasValue &&
            //   this.GiftCards_Activated.Value == order.OrderStatus)
            //{
            //    var giftCards = GetAllGiftCards(order.OrderId, null, null, null, null, null, null, false, string.Empty);
            //    foreach (var gc in giftCards)
            //    {
            //        bool isRecipientNotified = gc.IsRecipientNotified;
            //        switch (gc.PurchasedOrderProductVariant.ProductVariant.GiftCardType)
            //        {
            //            case (int)GiftCardType.Virtual:
            //                {
            //                    //send email for virtual gift card
            //                    if (!String.IsNullOrEmpty(gc.RecipientEmail) &&
            //                        !String.IsNullOrEmpty(gc.SenderEmail))
            //                    {
            //                        var customerLang = _languageService.GetLanguageById(order.CustomerLanguageId);
            //                        if (customerLang == null)
            //                            customerLang = NopContext.Current.WorkingLanguage;
            //                        int queuedEmailId = _messageService.SendGiftCardNotification(gc, customerLang.LanguageId);
            //                        if (queuedEmailId > 0)
            //                        {
            //                            isRecipientNotified = true;
            //                        }
            //                    }
            //                }
            //                break;
            //            case (int)GiftCardTypeEnum.Physical:
            //                {
            //                }
            //                break;
            //            default:
            //                break;
            //        }

            //        gc.IsGiftCardActivated = true;
            //        gc.IsRecipientNotified = isRecipientNotified;
            //        this.UpdateGiftCard(gc);
            //    }
            //}

            //TODO implement gift cards deactivation
            //if (this.GiftCards_Deactivated.HasValue &&
            //   this.GiftCards_Deactivated.Value == order.OrderStatus)
            //{
            //    var giftCards = GetAllGiftCards(order.OrderId,
            //        null, null, null, null, null, null, true, string.Empty);
            //    foreach (var gc in giftCards)
            //    {
            //        gc.IsGiftCardActivated = false;
            //        this.UpdateGiftCard(gc);
            //    }
            //}
        }

        /// <summary>
        /// Checks order status
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <returns>Validated order</returns>
        protected Order CheckOrderStatus(int orderId)
        {
            var order = GetOrderById(orderId);
            if (order == null)
                return null;

            if (order.OrderStatus == OrderStatus.Pending)
            {
                if (order.PaymentStatus == PaymentStatus.Authorized ||
                    order.PaymentStatus == PaymentStatus.Paid)
                {
                    SetOrderStatus(order, OrderStatus.Processing, false);
                }
            }

            if (order.OrderStatus == OrderStatus.Pending)
            {
                if (order.ShippingStatus == ShippingStatus.Shipped ||
                    order.ShippingStatus == ShippingStatus.Delivered)
                {
                    SetOrderStatus(order, OrderStatus.Processing, false);
                }
            }

            if (order.OrderStatus != OrderStatus.Cancelled &&
                order.OrderStatus != OrderStatus.Complete)
            {
                if (order.PaymentStatus == PaymentStatus.Paid)
                {
                    if (!CanShip(order) && !CanDeliver(order))
                    {
                        SetOrderStatus(order, OrderStatus.Complete, true);
                    }
                }
            }

            if (order.PaymentStatus == PaymentStatus.Paid && !order.PaidDateUtc.HasValue)
            {
                //ensure that paid date is set
                order.PaidDateUtc = DateTime.UtcNow;
                UpdateOrder(order);
            }

            return order;
        }

        #endregion

        #region Methods

        #region Orders

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>Order</returns>
        public Order GetOrderById(int orderId)
        {
            if (orderId == 0)
                return null;

            return _orderRepository.GetById(orderId);
        }

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <returns>Order</returns>
        public Order GetOrderByGuid(Guid orderGuid)
        {
            if (orderGuid == Guid.Empty)
                return null;

            var query = from o in _orderRepository.Table
                        where o.OrderGuid == orderGuid
                        select o;
            var order = query.FirstOrDefault();
            return order;
        }

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        public void DeleteOrder(Order order)
        {
            if (order == null)
                return;

            order.Deleted = true;
            UpdateOrder(order);
        }

        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="startTime">Order start time; null to load all orders</param>
        /// <param name="endTime">Order end time; null to load all orders</param>
        /// <param name="os">Order status; null to load all orders</param>
        /// <param name="ps">Order payment status; null to load all orders</param>
        /// <param name="ss">Order shippment status; null to load all orders</param>
        /// <param name="orderGuid">Search by order GUID (Global unique identifier) or part of GUID. Leave empty to load all orders.</param>
        /// <returns>Order collection</returns>
        public IList<Order> SearchOrders(DateTime? startTime, DateTime? endTime,
            OrderStatus? os, PaymentStatus? ps, ShippingStatus? ss, string orderGuid = null)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippingStatusId = null;
            if (ss.HasValue)
                shippingStatusId = (int)ss.Value;
            
            var query = from o in _orderRepository.Table
                        where (!startTime.HasValue || startTime.Value <= o.CreatedOnUtc) &&
                        (!endTime.HasValue || endTime.Value >= o.CreatedOnUtc) &&
                        (!orderStatusId.HasValue || orderStatusId == o.OrderStatusId) &&
                        (!paymentStatusId.HasValue || paymentStatusId.Value == o.PaymentStatusId) &&
                        (!shippingStatusId.HasValue || shippingStatusId.Value == o.ShippingStatusId) &&
                        !o.Deleted
                        orderby o.CreatedOnUtc descending
                        select o;

            var orders = query.ToList();
            
            //filter by GUID. Filter in BLL because EF doesn't support casting of GUID to string
            if (!String.IsNullOrEmpty(orderGuid))
                orders = orders.FindAll(o => o.OrderGuid.ToString().ToLowerInvariant().Contains(orderGuid.ToLowerInvariant()));
            
            return orders;
        }

        /// <summary>
        /// Load all orders
        /// </summary>
        /// <returns>Order collection</returns>
        public IList<Order> LoadAllOrders()
        {
            return SearchOrders(null, null, null, null, null, null);
        }

        /// <summary>
        /// Gets all orders by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Order collection</returns>
        public IList<Order> GetOrdersByCustomerId(int customerId)
        {
            
            var query = from o in _orderRepository.Table
                        orderby o.CreatedOnUtc descending
                        where !o.Deleted && o.CustomerId == customerId
                        select o;
            var orders = query.ToList();
            return orders;
        }

        /// <summary>
        /// Gets an order by authorization transaction identifier
        /// </summary>
        /// <param name="authorizationTransactionId">Authorization transaction identifier</param>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>Order</returns>
        public Order GetOrderByAuthorizationTransactionIdAndPaymentMethodId(string authorizationTransactionId,
            string paymentMethodSystemName)
        {
            //TODO remove this method? We need it only in Google Checkout payment method
            var query = from o in _orderRepository.Table
                        orderby o.CreatedOnUtc descending
                        where o.AuthorizationTransactionId == authorizationTransactionId &&
                        o.PaymentMethodSystemName == paymentMethodSystemName
                        select o;
            var order = query.FirstOrDefault();
            return order;
        }

        /// <summary>
        /// Gets all orders by affiliate identifier
        /// </summary>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <returns>Order collection</returns>
        public IList<Order> GetOrdersByAffiliateId(int affiliateId)
        {
            var query = from o in _orderRepository.Table
                        orderby o.CreatedOnUtc descending
                        where !o.Deleted && o.AffiliateId == affiliateId
                        select o;
            var orders = query.ToList();
            return orders;
        }

        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="order">Order</param>
        public void InsertOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            _orderRepository.Insert(order);
        }

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="order">The order</param>
        public void UpdateOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            _orderRepository.Update(order);
        }

        #endregion
        
        #region Orders product variants

        /// <summary>
        /// Gets an order product variant
        /// </summary>
        /// <param name="orderProductVariantGuid">Order product variant identifier</param>
        /// <returns>Order product variant</returns>
        public OrderProductVariant GetOrderProductVariantByGuid(Guid orderProductVariantGuid)
        {
            if (orderProductVariantGuid == Guid.Empty)
                return null;
            
            var query = from opv in _opvRepository.Table
                        where opv.OrderProductVariantGuid == orderProductVariantGuid
                        select opv;
            var orderProductVariant = query.FirstOrDefault();
            return orderProductVariant;
        }
        
        /// <summary>
        /// Gets all order product variants
        /// </summary>
        /// <param name="orderId">Order identifier; null to load all records</param>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <param name="startTime">Order start time; null to load all records</param>
        /// <param name="endTime">Order end time; null to load all records</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Order shippment status; null to load all records</param>
        /// <param name="loadDownloableProductsOnly">Value indicating whether to load downloadable products only</param>
        /// <returns>Order collection</returns>
        public IList<OrderProductVariant> GetAllOrderProductVariants(int? orderId,
            int? customerId, DateTime? startTime, DateTime? endTime,
            OrderStatus? os, PaymentStatus? ps, ShippingStatus? ss,
            bool loadDownloableProductsOnly)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippingStatusId = null;
            if (ss.HasValue)
                shippingStatusId = (int)ss.Value;
            

            var query = from opv in _opvRepository.Table
                        join o in _orderRepository.Table on opv.OrderId equals o.Id
                        join pv in _pvRepository.Table on opv.ProductVariantId equals pv.Id
                        where (!orderId.HasValue || orderId.Value == 0 || orderId == o.Id) &&
                        (!customerId.HasValue || customerId.Value == 0 || customerId == o.CustomerId) &&
                        (!startTime.HasValue || startTime.Value <= o.CreatedOnUtc) &&
                        (!endTime.HasValue || endTime.Value >= o.CreatedOnUtc) &&
                        (!orderStatusId.HasValue || orderStatusId == o.OrderStatusId) &&
                        (!paymentStatusId.HasValue || paymentStatusId.Value == o.PaymentStatusId) &&
                        (!shippingStatusId.HasValue || shippingStatusId.Value == o.ShippingStatusId) &&
                        (!loadDownloableProductsOnly || pv.IsDownload) &&
                        !o.Deleted
                        orderby o.CreatedOnUtc descending, opv.Id
                        select opv;

            var orderProductVariants = query.ToList();
            return orderProductVariants;
        }
        
        #endregion
        
        #region Recurring payments

        /// <summary>
        /// Deletes a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public void DeleteRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                return;

            recurringPayment.Deleted = true;
            UpdateRecurringPayment(recurringPayment);
        }

        /// <summary>
        /// Gets a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <returns>Recurring payment</returns>
        public RecurringPayment GetRecurringPaymentById(int recurringPaymentId)
        {
            if (recurringPaymentId == 0)
                return null;

           return _recurringPaymentRepository.GetById(recurringPaymentId);
        }

        /// <summary>
        /// Inserts a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public void InsertRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            _recurringPaymentRepository.Insert(recurringPayment);
        }

        /// <summary>
        /// Updates the recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public void UpdateRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            _recurringPaymentRepository.Update(recurringPayment);
        }

        /// <summary>
        /// Search recurring payments
        /// </summary>
        /// <param name="customerId">The customer identifier; 0 to load all records</param>
        /// <param name="initialOrderId">The initial order identifier; 0 to load all records</param>
        /// <param name="initialOrderStatus">Initial order status identifier; null to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Recurring payment collection</returns>
        public IList<RecurringPayment> SearchRecurringPayments(int customerId,
            int initialOrderId, OrderStatus? initialOrderStatus, bool showHidden = false)
        {
            int? initialOrderStatusId = null;
            if (initialOrderStatus.HasValue)
                initialOrderStatusId = (int)initialOrderStatus.Value;

            //TODO test (new implementation)
            var query1 = from rp in _recurringPaymentRepository.Table
                         //join o in _orderRepository.Table on rp.InitialOrderId equals o.Id
                         join c in _customerRepository.Table on rp.InitialOrder.CustomerId equals c.Id
                         where
                         (!rp.Deleted && !rp.InitialOrder.Deleted && !c.Deleted) &&
                         (showHidden || rp.IsActive) &&
                         (customerId == 0 || rp.InitialOrder.CustomerId == customerId) &&
                         (initialOrderId == 0 || rp.InitialOrder.Id == initialOrderId) &&
                         (!initialOrderStatusId.HasValue || initialOrderStatusId.Value == 0 || rp.InitialOrder.OrderStatusId == initialOrderStatusId.Value)
                         select rp.Id;

            var query2 = from rp in _recurringPaymentRepository.Table
                         where query1.Contains(rp.Id)
                         orderby rp.StartDateUtc, rp.Id
                         select rp;
            
            var recurringPayments = query2.ToList();
            return recurringPayments;
        }

        /// <summary>
        /// Search recurring payment history
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier; 0 to load all records</param>
        /// <param name="orderId">The order identifier; 0 to load all records</param>
        /// <returns>Recurring payment history collection</returns>
        public IList<RecurringPaymentHistory> SearchRecurringPaymentHistory(int recurringPaymentId, 
            int orderId)
        {
            //TODO test (new implementation)
            var query1 = from rph in _recurringPaymentHistoryRepository.Table
                         from rp in _recurringPaymentRepository.Table
                         .Where(rp => rp.Id == rph.RecurringPaymentId)
                         .DefaultIfEmpty()
                         where
                         (!rp.Deleted && !rph.Order.Deleted) &&
                         (recurringPaymentId == 0 || rph.RecurringPaymentId == recurringPaymentId) &&
                         (orderId == 0 || rph.Order.Id == orderId)
                         select rph.Id;

            var query2 = from rph in _recurringPaymentHistoryRepository.Table
                         where query1.Contains(rph.Id)
                         orderby rph.CreatedOnUtc, rph.Id
                         select rph;

            var recurringPaymentHistory = query2.ToList();
            return recurringPaymentHistory;
        }

        #endregion

        #region Workflow, etc
        
        /// <summary>
        /// Gets a value indicating whether download is allowed
        /// </summary>
        /// <param name="orderProductVariant">Order produvt variant to check</param>
        /// <returns>True if download is allowed; otherwise, false.</returns>
        public bool IsDownloadAllowed(OrderProductVariant orderProductVariant)
        {
            if (orderProductVariant == null)
                return false;

            var order = orderProductVariant.Order;
            if (order == null || order.Deleted)
                return false;

            //order status
            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            var productVariant = orderProductVariant.ProductVariant;
            if (productVariant == null || !productVariant.IsDownload)
                return false;

            //payment status
            switch (productVariant.DownloadActivationType)
            {
                case DownloadActivationType.WhenOrderIsPaid:
                    {
                        if (order.PaymentStatus == PaymentStatus.Paid && order.PaidDateUtc.HasValue)
                        {
                            //expiration date
                            if (productVariant.DownloadExpirationDays.HasValue)
                            {
                                if (order.PaidDateUtc.Value.AddDays(productVariant.DownloadExpirationDays.Value) > DateTime.UtcNow)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case DownloadActivationType.Manually:
                    {
                        if (orderProductVariant.IsDownloadActivated)
                        {
                            //expiration date
                            if (productVariant.DownloadExpirationDays.HasValue)
                            {
                                if (order.CreatedOnUtc.AddDays(productVariant.DownloadExpirationDays.Value) > DateTime.UtcNow)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether license download is allowed
        /// </summary>
        /// <param name="orderProductVariant">Order produvt variant to check</param>
        /// <returns>True if license download is allowed; otherwise, false.</returns>
        public bool IsLicenseDownloadAllowed(OrderProductVariant orderProductVariant)
        {
            if (orderProductVariant == null)
                return false;

            return IsDownloadAllowed(orderProductVariant) && orderProductVariant.LicenseDownloadId > 0;
        }
        
        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public IList<string> CancelRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            var initialOrder = recurringPayment.InitialOrder;
            if (initialOrder == null)
                return new List<string>() { "Initial order could not be loaded"};


            var request = new CancelRecurringPaymentRequest();
            CancelRecurringPaymentResult result = null;
            try
            {
                request.Order = initialOrder;
                result = _paymentService.CancelRecurringPayment(request);
                if (result.Success)
                {
                    //update recurring payment
                    recurringPayment.IsActive = false;
                    UpdateRecurringPayment(recurringPayment);


                    //add a note
                    initialOrder.OrderNotes.Add(new OrderNote()
                    {
                        Note = "Recurring payment has been cancelled",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    UpdateOrder(initialOrder);
                }
            }
            catch (Exception exc)
            {
                result = new CancelRecurringPaymentResult();
                result.AddError(string.Format("Error: {0}. Full exception: {1}", exc.Message, exc.ToString()));
            }


            //process errors
            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //add a note
                initialOrder.OrderNotes.Add(new OrderNote()
                {
                    Note = string.Format("Unable to cancel recurring payment. {0}", error),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                UpdateOrder(initialOrder);

                //log it
                string logError = string.Format("Error cancelling recurring payment. {0}", initialOrder.Id, error);
                _logger.InsertLog(LogLevel.Error, logError, logError);
            }
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether a customer can cancel recurring payment
        /// </summary>
        /// <param name="customerToValidate">Customer</param>
        /// <param name="recurringPayment">Recurring Payment</param>
        /// <returns>value indicating whether a customer can cancel recurring payment</returns>
        public bool CanCancelRecurringPayment(Customer customerToValidate, RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                return false;

            if (customerToValidate == null)
                return false;

            var initialOrder = recurringPayment.InitialOrder;
            if (initialOrder == null)
                return false;

            var customer = recurringPayment.InitialOrder.Customer;
            if (customer == null)
                return false;

            if (initialOrder.OrderStatus == OrderStatus.Cancelled)
                return false;

            //TODO check whether customer is admin
            //if (!customerToValidate.IsAdmin)
            {
                if (customer.Id != customerToValidate.Id)
                    return false;
            }

            if (!recurringPayment.NextPaymentDate.HasValue)
                return false;

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether shipping is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether shipping is allowed</returns>
        public bool CanShip(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (order.ShippingStatus == ShippingStatus.NotYetShipped)
                return true;

            return false;
        }

        /// <summary>
        /// Ships order
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        public void Ship(Order order, bool notifyCustomer)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanShip(order))
                throw new NopException("Can not do shipment for order.");

            order.ShippedDateUtc = DateTime.UtcNow;
            order.ShippingStatusId = (int)ShippingStatus.Shipped;
            UpdateOrder(order);

            //add a note
            order.OrderNotes.Add(new OrderNote()
                {
                    Note = "Order has been shipped",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
            UpdateOrder(order);

            if (notifyCustomer)
            {
                //TODO notify customer
                //int orderShippedCustomerNotificationQueuedEmailId = _messageService.SendOrderShippedCustomerNotification(order, order.CustomerLanguageId);
                //if (orderShippedCustomerNotificationQueuedEmailId > 0)
                //{
                //    order.OrderNotes.Add(new OrderNote()
                //    {
                //        Note = string.Format("\"Shipped\" email (to customer) has been queued. Queued email identifier: {0}.", orderShippedCustomerNotificationQueuedEmailId),
                //        DisplayToCustomer = false,
                //        CreatedOnUtc = DateTime.UtcNow
                //    });
                //    UpdateOrder(order);
                //}
            }

            //check order status
            order = CheckOrderStatus(order.Id);
        }

        /// <summary>
        /// Gets a value indicating whether order is delivered
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether shipping is delivered</returns>
        public bool CanDeliver(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (order.ShippingStatus == ShippingStatus.Shipped)
                return true;

            return false;
        }

        /// <summary>
        /// Marks order status as delivered
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        public void Deliver(Order order, bool notifyCustomer)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanDeliver(order))
                throw new NopException("Can not do delivery for order.");

            order.DeliveryDateUtc = DateTime.UtcNow;
            order.ShippingStatusId = (int)ShippingStatus.Delivered;
            UpdateOrder(order);

            //add a note
            order.OrderNotes.Add(new OrderNote()
            {
                Note = "Order has been delivered",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            UpdateOrder(order);

            if (notifyCustomer)
            {
                //TODO send email notification
                //int orderDeliveredCustomerNotificationQueuedEmailId = _messageService.SendOrderDeliveredCustomerNotification(order, order.CustomerLanguageId);
                //if (orderDeliveredCustomerNotificationQueuedEmailId > 0)
                //{
                //    order.OrderNotes.Add(new OrderNote()
                //    {
                //        Note = string.Format("\"Delivered\" email (to customer) has been queued. Queued email identifier: {0}.", orderDeliveredCustomerNotificationQueuedEmailId),
                //        DisplayToCustomer = false,
                //        CreatedOnUtc = DateTime.UtcNow
                //    });
                //    UpdateOrder(order);
                //}
            }

            //check order status
            order = CheckOrderStatus(order.Id);
        }

        /// <summary>
        /// Gets a value indicating whether cancel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether cancel is allowed</returns>
        public bool CanCancelOrder(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            return true;
        }

        /// <summary>
        /// Cancels order
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        public void CancelOrder(Order order, bool notifyCustomer)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanCancelOrder(order))
                throw new NopException("Can not do cancel for order.");

            //Cancel order
            SetOrderStatus(order, OrderStatus.Cancelled, notifyCustomer);

            //add a note
            order.OrderNotes.Add(new OrderNote()
            {
                Note = "Order has been cancelled",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            UpdateOrder(order);

            //cancel recurring payments
            var recurringPayments = SearchRecurringPayments(0, order.Id, null);
            foreach (var rp in recurringPayments)
            {
                var errors = CancelRecurringPayment(rp);
            }

            //Adjust inventory
            foreach (var opv in order.OrderProductVariants)
                _productService.AdjustInventory(opv.ProductVariant, false, opv.Quantity, opv.AttributesXml);
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as authorized
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as authorized</returns>
        public bool CanMarkOrderAsAuthorized(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatus.Pending)
                return true;

            return false;
        }

        /// <summary>
        /// Marks order as authorized
        /// </summary>
        /// <param name="order">Order</param>
        public void MarkAsAuthorized(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            order.PaymentStatusId = (int)PaymentStatus.Authorized;
            UpdateOrder(order);

            //add a note
            order.OrderNotes.Add(new OrderNote()
            {
                Note = "Order has been marked as authorized",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            UpdateOrder(order);

            //check order status
            order = CheckOrderStatus(order.Id);
        }

        /// <summary>
        /// Gets a value indicating whether capture from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether capture from admin panel is allowed</returns>
        public bool CanCapture(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatus.Cancelled ||
                order.OrderStatus == OrderStatus.Pending)
                return false;

            if (order.PaymentStatus == PaymentStatus.Authorized &&
                _paymentService.CanCapture(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Capture an order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        public IList<string> Capture(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanCapture(order))
                throw new NopException("Can not do capture for order.");

            var request = new CapturePaymentRequest();
            CapturePaymentResult result = null;
            try
            {
                //old info from placing order
                request.Order = order;
                result = _paymentService.Capture(request);

                if (result.Success)
                {
                    var paidDate = order.PaidDateUtc;
                    if (result.NewPaymentStatus == PaymentStatus.Paid)
                        paidDate = DateTime.UtcNow;

                    order.CaptureTransactionId = result.CaptureTransactionId;
                    order.CaptureTransactionResult = result.CaptureTransactionResult;
                    order.PaymentStatus = result.NewPaymentStatus;
                    order.PaidDateUtc = paidDate;
                    UpdateOrder(order);

                    //add a note
                    order.OrderNotes.Add(new OrderNote()
                    {
                        Note = "Order has been captured",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    UpdateOrder(order);

                    order = CheckOrderStatus(order.Id);

                    //TODO raise event         
                    //if (order.PaymentStatus == PaymentStatus.Paid)
                    //{
                    //    EventContext.Current.OnOrderPaid(null,
                    //        new OrderEventArgs() { Order = order });
                    //}
                }
            }
            catch (Exception exc)
            {
                result = new CapturePaymentResult();
                result.AddError(string.Format("Error: {0}. Full exception: {1}",exc.Message,exc.ToString()));
            }


            //process errors
            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //add a note
                order.OrderNotes.Add(new OrderNote()
                {
                    Note = string.Format("Unable to capture order. {0}", error),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                UpdateOrder(order);

                //log it
                string logError = string.Format("Error capturing order #{0}. {1}", order.Id, error);
                _logger.InsertLog(LogLevel.Error, logError, logError);
            }
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as paid
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as paid</returns>
        public bool CanMarkOrderAsPaid(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatus.Paid ||
                order.PaymentStatus == PaymentStatus.Refunded ||
                order.PaymentStatus == PaymentStatus.Voided)
                return false;

            return true;
        }

        /// <summary>
        /// Marks order as paid
        /// </summary>
        /// <param name="order">Order</param>
        public void MarkOrderAsPaid(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanMarkOrderAsPaid(order))
                throw new NopException("You can't mark this order as paid");

            order.PaymentStatusId = (int)PaymentStatus.Paid;
            order.PaidDateUtc = DateTime.UtcNow;
            UpdateOrder(order);

            //add a note
            order.OrderNotes.Add(new OrderNote()
            {
                Note = "Order has been marked as paid",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            UpdateOrder(order);

            order = CheckOrderStatus(order.Id);

            //TODO raise event         
            //if (order.PaymentStatus == PaymentStatus.Paid)
            //{
            //    EventContext.Current.OnOrderPaid(null,
            //        new OrderEventArgs() { Order = order });
            //}
        }

        /// <summary>
        /// Gets a value indicating whether refund from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        public bool CanRefund(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderTotal == decimal.Zero)
                return false;

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatus.Paid &&
                _paymentService.CanRefund(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Refunds an order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        public IList<string> Refund(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanRefund(order))
                throw new NopException("Can not do refund for order.");

            var request = new RefundPaymentRequest();
            RefundPaymentResult result = null;
            try
            {
                request.Order = order;
                request.AmountToRefund = order.OrderTotal;
                request.IsPartialRefund = false;
                result = _paymentService.Refund(request);
                if (result.Success)
                { 
                    //total amount refunded
                    decimal totalAmountRefunded = order.RefundedAmount + request.AmountToRefund;

                    //update order info
                    order.RefundedAmount = totalAmountRefunded;
                    order.PaymentStatus = result.NewPaymentStatus;
                    UpdateOrder(order);

                    //add a note
                    order.OrderNotes.Add(new OrderNote()
                    {
                        Note = string.Format("Order has been refunded. Amount = {0}", _priceFormatter.FormatPrice(request.AmountToRefund, true, false)),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    UpdateOrder(order);

                    //check order status
                    order = CheckOrderStatus(order.Id);
                }

            }
            catch (Exception exc)
            {
                result = new RefundPaymentResult();
                result.AddError(string.Format("Error: {0}. Full exception: {1}", exc.Message, exc.ToString()));
            }

            //process errors
            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //add a note
                order.OrderNotes.Add(new OrderNote()
                {
                    Note = string.Format("Unable to refund order. {0}", error),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                UpdateOrder(order);

                //log it
                string logError = string.Format("Error refunding order #{0}. {1}", order.Id, error);
                _logger.InsertLog(LogLevel.Error, logError, logError);
            }
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as refunded
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as refunded</returns>
        public bool CanRefundOffline(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderTotal == decimal.Zero)
                return false;

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatus.Paid)
                return true;

            return false;
        }

        /// <summary>
        /// Refunds an order (offline)
        /// </summary>
        /// <param name="order">Order</param>
        public void RefundOffline(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanRefundOffline(order))
                throw new NopException("You can't refund this order");

            //amout to refund
            decimal amountToRefund = order.OrderTotal;

            //total amount refunded
            decimal totalAmountRefunded = order.RefundedAmount + amountToRefund;

            //update order info
            order.RefundedAmount = totalAmountRefunded;
            order.PaymentStatus = PaymentStatus.Refunded;
            UpdateOrder(order);

            //add a note
            order.OrderNotes.Add(new OrderNote()
            {
                Note = string.Format("Order has been marked as refunded. Amount = {0}", _priceFormatter.FormatPrice(amountToRefund, true, false)),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            UpdateOrder(order);

            //check order status
            order = CheckOrderStatus(order.Id);
        }

        /// <summary>
        /// Gets a value indicating whether partial refund from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        public bool CanPartiallyRefund(Order order, decimal amountToRefund)
        {
            if (order == null)
                return false;

            if (order.OrderTotal == decimal.Zero)
                return false;

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            decimal canBeRefunded = order.OrderTotal - order.RefundedAmount;
            if (canBeRefunded <= decimal.Zero)
                return false;

            if (amountToRefund > canBeRefunded)
                return false;

            if ((order.PaymentStatus == PaymentStatus.Paid ||
                order.PaymentStatus == PaymentStatus.PartiallyRefunded) &&
                _paymentService.CanPartiallyRefund(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Partially refunds an order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        public IList<string> PartiallyRefund(Order order, decimal amountToRefund)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            amountToRefund = Math.Round(amountToRefund, 2);

            if (!CanPartiallyRefund(order, amountToRefund))
                throw new NopException("Can not do partial refund for order.");

            var request = new RefundPaymentRequest();
            RefundPaymentResult result = null;
            try
            {
                request.Order = order;
                request.AmountToRefund = amountToRefund;
                request.IsPartialRefund = true;

                result = _paymentService.Refund(request);

                if (result.Success)
                {
                    //total amount refunded
                    decimal totalAmountRefunded = order.RefundedAmount + amountToRefund;

                    //update order info
                    order.RefundedAmount = totalAmountRefunded;
                    order.PaymentStatus = result.NewPaymentStatus;
                    UpdateOrder(order);


                    //add a note
                    order.OrderNotes.Add(new OrderNote()
                    {
                        Note = string.Format("Order has been partially refunded. Amount = {0}", _priceFormatter.FormatPrice(amountToRefund, true, false)),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    UpdateOrder(order);

                    //check order status
                    order = CheckOrderStatus(order.Id);
                }
            }
            catch (Exception exc)
            {
                result = new RefundPaymentResult();
                result.AddError(string.Format("Error: {0}. Full exception: {1}", exc.Message, exc.ToString()));
            }

            //process errors
            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //add a note
                order.OrderNotes.Add(new OrderNote()
                {
                    Note = string.Format("Unable to partially refund order. {0}", error),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                UpdateOrder(order);

                //log it
                string logError = string.Format("Error refunding order #{0}. {1}", order.Id, error);
                _logger.InsertLog(LogLevel.Error, logError, logError);
            }
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as partially refunded
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether order can be marked as partially refunded</returns>
        public bool CanPartiallyRefundOffline(Order order, decimal amountToRefund)
        {
            if (order == null)
                return false;

            if (order.OrderTotal == decimal.Zero)
                return false;

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            decimal canBeRefunded = order.OrderTotal - order.RefundedAmount;
            if (canBeRefunded <= decimal.Zero)
                return false;

            if (amountToRefund > canBeRefunded)
                return false;

            if (order.PaymentStatus == PaymentStatus.Paid ||
                order.PaymentStatus == PaymentStatus.PartiallyRefunded)
                return true;

            return false;
        }

        /// <summary>
        /// Partially refunds an order (offline)
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        public void PartiallyRefundOffline(Order order, decimal amountToRefund)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            amountToRefund = Math.Round(amountToRefund, 2);

            if (!CanPartiallyRefundOffline(order, amountToRefund))
                throw new NopException("You can't partially refund (offline) this order");

            //total amount refunded
            decimal totalAmountRefunded = order.RefundedAmount + amountToRefund;

            //update order info
            order.RefundedAmount = totalAmountRefunded;
            order.PaymentStatus = PaymentStatus.PartiallyRefunded;
            UpdateOrder(order);
            
            //add a note
            order.OrderNotes.Add(new OrderNote()
            {
                Note = string.Format("Order has been marked as partially refunded. Amount = {0}", _priceFormatter.FormatPrice(amountToRefund, true, false)),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            UpdateOrder(order);
            
            //check order status
            order = CheckOrderStatus(order.Id);
        }

        /// <summary>
        /// Gets a value indicating whether void from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether void from admin panel is allowed</returns>
        public bool CanVoid(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderTotal == decimal.Zero)
                return false;

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatus.Authorized &&
                _paymentService.CanVoid(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Voids order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Voided order</returns>
        public IList<string> Void(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanVoid(order))
                throw new NopException("Can not do void for order.");

            var request = new VoidPaymentRequest();
            VoidPaymentResult result = null;
            try
            {
                request.Order = order;
                result = _paymentService.Void(request);

                if (result.Success)
                {
                    //update order info
                    order.PaymentStatus = result.NewPaymentStatus;
                    UpdateOrder(order);

                    //add a note
                    order.OrderNotes.Add(new OrderNote()
                    {
                        Note = "Order has been voided",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    UpdateOrder(order);

                    //check order status
                    order = CheckOrderStatus(order.Id);
                }
            }
            catch (Exception exc)
            {
                result = new VoidPaymentResult();
                result.AddError(string.Format("Error: {0}. Full exception: {1}", exc.Message, exc.ToString()));
            }

            //process errors
            string error = "";
            for (int i = 0; i < result.Errors.Count; i++)
            {
                error += string.Format("Error {0}: {1}", i, result.Errors[i]);
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }
            if (!String.IsNullOrEmpty(error))
            {
                //add a note
                order.OrderNotes.Add(new OrderNote()
                {
                    Note = string.Format("Unable to voiding order. {0}", error),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                UpdateOrder(order);

                //log it
                string logError = string.Format("Error voiding order #{0}. {1}", order.Id, error);
                _logger.InsertLog(LogLevel.Error, logError, logError);
            }
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as voided
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as voided</returns>
        public bool CanVoidOffline(Order order)
        {
            if (order == null)
                return false;

            if (order.OrderTotal == decimal.Zero)
                return false;

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatus.Authorized)
                return true;

            return false;
        }

        /// <summary>
        /// Voids order (offline)
        /// </summary>
        /// <param name="order">Order</param>
        public void VoidOffline(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanVoidOffline(order))
                throw new NopException("You can't void this order");

            order.PaymentStatusId = (int)PaymentStatus.Voided;
            UpdateOrder(order);

            //add a note
            order.OrderNotes.Add(new OrderNote()
            {
                Note = "Order has been marked as voided",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            UpdateOrder(order);

            //check orer status
            order = CheckOrderStatus(order.Id);
        }


        /// <summary>
        /// Valdiate minimum order sub-total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum order sub-total amount is not reached</returns>
        public bool ValidateMinOrderSubtotalAmount(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            bool result = true;
            //min order amount sub-total validation
            if (cart.Count > 0 && _orderSettings.MinOrderSubtotalAmount > decimal.Zero)
            {
                //subtotal
                decimal orderSubTotalDiscountAmountBase = decimal.Zero;
                Discount orderSubTotalAppliedDiscount = null;
                decimal subTotalWithoutDiscountBase = decimal.Zero;
                decimal subTotalWithDiscountBase = decimal.Zero;
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart,
                    out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscount,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);

                if (subTotalWithoutDiscountBase < _orderSettings.MinOrderSubtotalAmount)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Valdiate minimum order total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum order total amount is not reached</returns>
        public bool ValidateMinOrderTotalAmount(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (cart.Count > 0 && _orderSettings.MinOrderTotalAmount > decimal.Zero)
            {
                decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart);
                if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value < _orderSettings.MinOrderTotalAmount)
                    return false;
            }

            return true;
        }

        #endregion

        #endregion
    }
}
