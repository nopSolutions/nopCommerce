using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order service interface
    /// </summary>
    public partial interface IOrderService
    {
        #region Orders

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>Order</returns>
        Order GetOrderById(int orderId);

        /// <summary>
        /// Get orders by identifiers
        /// </summary>
        /// <param name="orderIds">Order identifiers</param>
        /// <returns>Order</returns>
        IList<Order> GetOrdersByIds(int[] orderIds);

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <returns>Order</returns>
        Order GetOrderByGuid(Guid orderGuid);

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        void DeleteOrder(Order order);

        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="storeId">Store identifier; null to load all orders</param>
        /// <param name="vendorId">Vendor identifier; null to load all orders</param>
        /// <param name="customerId">Customer identifier; null to load all orders</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="os">Order status; null to load all orders</param>
        /// <param name="ps">Order payment status; null to load all orders</param>
        /// <param name="ss">Order shippment status; null to load all orders</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="orderGuid">Search by order GUID (Global unique identifier) or part of GUID. Leave empty to load all records.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Order collection</returns>
        IPagedList<Order> SearchOrders(int storeId, int vendorId, int customerId,
            DateTime? createdFromUtc, DateTime? createdToUtc, 
            OrderStatus? os, PaymentStatus? ps, ShippingStatus? ss, 
            string billingEmail, string orderGuid, int pageIndex, int pageSize);
        
        /// <summary>
        /// Gets all orders by affiliate identifier
        /// </summary>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Orders</returns>
        IPagedList<Order> GetOrdersByAffiliateId(int affiliateId, int pageIndex, int pageSize);
        
        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="order">Order</param>
        void InsertOrder(Order order);

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="order">The order</param>
        void UpdateOrder(Order order);

        /// <summary>
        /// Deletes an order note
        /// </summary>
        /// <param name="orderNote">The order note</param>
        void DeleteOrderNote(OrderNote orderNote);

        /// <summary>
        /// Get an order by authorization transaction ID and payment method system name
        /// </summary>
        /// <param name="authorizationTransactionId">Authorization transaction ID</param>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>Order</returns>
        Order GetOrderByAuthorizationTransactionIdAndPaymentMethod(string authorizationTransactionId, string paymentMethodSystemName);
        
        #endregion

        #region Orders product variants
        
        /// <summary>
        /// Gets an order product variant
        /// </summary>
        /// <param name="orderProductVariantId">Order product variant identifier</param>
        /// <returns>Order product variant</returns>
        OrderProductVariant GetOrderProductVariantById(int orderProductVariantId);

        /// <summary>
        /// Gets an order product variant
        /// </summary>
        /// <param name="orderProductVariantGuid">Order product variant identifier</param>
        /// <returns>Order product variant</returns>
        OrderProductVariant GetOrderProductVariantByGuid(Guid orderProductVariantGuid);

        /// <summary>
        /// Gets all order product variants
        /// </summary>
        /// <param name="orderId">Order identifier; null to load all records</param>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Order shippment status; null to load all records</param>
        /// <param name="loadDownloableProductsOnly">Value indicating whether to load downloadable products only</param>
        /// <returns>Order collection</returns>
        IList<OrderProductVariant> GetAllOrderProductVariants(int? orderId,
           int? customerId, DateTime? createdFromUtc, DateTime? createdToUtc, 
           OrderStatus? os, PaymentStatus? ps, ShippingStatus? ss,
           bool loadDownloableProductsOnly = false);

        /// <summary>
        /// Delete an order product variant
        /// </summary>
        /// <param name="orderProductVariant">The order product variant</param>
        void DeleteOrderProductVariant(OrderProductVariant orderProductVariant);

        #endregion

        #region Recurring payments

        /// <summary>
        /// Deletes a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        void DeleteRecurringPayment(RecurringPayment recurringPayment);

        /// <summary>
        /// Gets a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <returns>Recurring payment</returns>
        RecurringPayment GetRecurringPaymentById(int recurringPaymentId);

        /// <summary>
        /// Inserts a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        void InsertRecurringPayment(RecurringPayment recurringPayment);

        /// <summary>
        /// Updates the recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        void UpdateRecurringPayment(RecurringPayment recurringPayment);

        /// <summary>
        /// Search recurring payments
        /// </summary>
        /// <param name="storeId">The store identifier; 0 to load all records</param>
        /// <param name="customerId">The customer identifier; 0 to load all records</param>
        /// <param name="initialOrderId">The initial order identifier; 0 to load all records</param>
        /// <param name="initialOrderStatus">Initial order status identifier; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Recurring payment collection</returns>
        IPagedList<RecurringPayment> SearchRecurringPayments(int storeId, 
            int customerId, int initialOrderId, OrderStatus? initialOrderStatus,
            int pageIndex, int pageSize, bool showHidden = false);

        #endregion

        #region Return requests

        /// <summary>
        /// Deletes a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        void DeleteReturnRequest(ReturnRequest returnRequest);

        /// <summary>
        /// Gets a return request
        /// </summary>
        /// <param name="returnRequestId">Return request identifier</param>
        /// <returns>Return request</returns>
        ReturnRequest GetReturnRequestById(int returnRequestId);
        
        /// <summary>
        /// Search return requests
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all entries</param>
        /// <param name="customerId">Customer identifier; 0 to load all entries</param>
        /// <param name="orderProductVariantId">Order product variant identifier; 0 to load all entries</param>
        /// <param name="rs">Return request status; null to load all entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Return requests</returns>
        IPagedList<ReturnRequest> SearchReturnRequests(int storeId, int customerId,
            int orderProductVariantId, ReturnRequestStatus? rs, int pageIndex, int pageSize);
        
        #endregion
    }
}
