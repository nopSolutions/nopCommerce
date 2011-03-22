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
using Nop.Core.Domain.Tax;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Core.Domain.Common;
using Nop.Services.Security;

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
        public OrderService(IRepository<Order> orderRepository,
            IRepository<OrderProductVariant> opvRepository,
            IRepository<ProductVariant> pvRepository,
            IRepository<RecurringPayment> recurringPaymentRepository,
            IRepository<RecurringPaymentHistory> recurringPaymentHistoryRepository,
            IRepository<Customer> customerRepository)
        {
            this._orderRepository = orderRepository;
            this._opvRepository = opvRepository;
            this._pvRepository = pvRepository;
            this._recurringPaymentRepository = recurringPaymentRepository;
            this._recurringPaymentHistoryRepository = recurringPaymentHistoryRepository;
            this._customerRepository = customerRepository;
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
        
        #endregion
    }
}
