using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<Order> GetOrderByIdAsync(int orderId);

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="customOrderNumber">The custom order number</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<Order> GetOrderByCustomOrderNumberAsync(string customOrderNumber);

        /// <summary>
        /// Gets an order by order item identifier
        /// </summary>
        /// <param name="orderItemId">The order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<Order> GetOrderByOrderItemAsync(int orderItemId);

        /// <summary>
        /// Get orders by identifiers
        /// </summary>
        /// <param name="orderIds">Order identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<IList<Order>> GetOrdersByIdsAsync(int[] orderIds);

        /// <summary>
        /// Get orders by guids
        /// </summary>
        /// <param name="orderGuids">Order guids</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the orders
        /// </returns>
        Task<IList<Order>> GetOrdersByGuidsAsync(Guid[] orderGuids);

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<Order> GetOrderByGuidAsync(Guid orderGuid);

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteOrderAsync(Order order);

        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="storeId">Store identifier; null to load all orders</param>
        /// <param name="vendorId">Vendor identifier; null to load all orders</param>
        /// <param name="customerId">Customer identifier; null to load all orders</param>
        /// <param name="productId">Product identifier which was purchased in an order; 0 to load all orders</param>
        /// <param name="affiliateId">Affiliate identifier; 0 to load all orders</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="warehouseId">Warehouse identifier, only orders with products from a specified warehouse will be loaded; 0 to load all orders</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="osIds">Order status identifiers; null to load all orders</param>
        /// <param name="psIds">Payment status identifiers; null to load all orders</param>
        /// <param name="ssIds">Shipping status identifiers; null to load all orders</param>
        /// <param name="billingPhone">Billing phone. Leave empty to load all records.</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the orders
        /// </returns>
        Task<IPagedList<Order>> SearchOrdersAsync(int storeId = 0,
            int vendorId = 0, int customerId = 0,
            int productId = 0, int affiliateId = 0, int warehouseId = 0,
            int billingCountryId = 0, string paymentMethodSystemName = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            string billingPhone = null, string billingEmail = null, string billingLastName = "",
            string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertOrderAsync(Order order);

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateOrderAsync(Order order);

        /// <summary>
        /// Parse tax rates
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="taxRatesStr"></param>
        /// <returns>Rates</returns>
        SortedDictionary<decimal, decimal> ParseTaxRates(Order order, string taxRatesStr);

        /// <summary>
        /// Gets a value indicating whether an order has items to be added to a shipment
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value indicating whether an order has items to be added to a shipment
        /// </returns>
        Task<bool> HasItemsToAddToShipmentAsync(Order order);

        /// <summary>
        /// Gets a value indicating whether an order has items to ship
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value indicating whether an order has items to ship
        /// </returns>
        Task<bool> HasItemsToShipAsync(Order order);

        /// <summary>
        /// Gets a value indicating whether there are shipment items to mark as 'ready for pickup' in order shipments.
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value indicating whether there are shipment items to mark as 'ready for pickup' in order shipments.
        /// </returns>
        Task<bool> HasItemsToReadyForPickupAsync(Order order);

        /// <summary>
        /// Gets a value indicating whether an order has items to deliver
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value indicating whether an order has items to deliver
        /// </returns>
        Task<bool> HasItemsToDeliverAsync(Order order);

        #endregion

        #region Orders items

        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemId">Order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order item
        /// </returns>
        Task<OrderItem> GetOrderItemByIdAsync(int orderItemId);

        /// <summary>
        /// Gets a product of specify order item
        /// </summary>
        /// <param name="orderItemId">Order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product
        /// </returns>
        Task<Product> GetProductByOrderItemIdAsync(int orderItemId);

        /// <summary>
        /// Gets a list items of order
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="isNotReturnable">Value indicating whether this product is returnable; pass null to ignore</param>
        /// <param name="isShipEnabled">Value indicating whether the entity is ship enabled; pass null to ignore</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<IList<OrderItem>> GetOrderItemsAsync(int orderId, bool? isNotReturnable = null, bool? isShipEnabled = null, int vendorId = 0);

        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemGuid">Order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order item
        /// </returns>
        Task<OrderItem> GetOrderItemByGuidAsync(Guid orderItemGuid);

        /// <summary>
        /// Gets all downloadable order items
        /// </summary>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order items
        /// </returns>
        Task<IList<OrderItem>> GetDownloadableOrderItemsAsync(int customerId);

        /// <summary>
        /// Delete an order item
        /// </summary>
        /// <param name="orderItem">The order item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteOrderItemAsync(OrderItem orderItem);

        /// <summary>
        /// Gets a total number of items in all shipments
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the total number of items in all shipments
        /// </returns>
        Task<int> GetTotalNumberOfItemsInAllShipmentsAsync(OrderItem orderItem);

        /// <summary>
        /// Gets a total number of already items which can be added to new shipments
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the total number of already delivered items which can be added to new shipments
        /// </returns>
        Task<int> GetTotalNumberOfItemsCanBeAddedToShipmentAsync(OrderItem orderItem);

        /// <summary>
        /// Gets a value indicating whether download is allowed
        /// </summary>
        /// <param name="orderItem">Order item to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the true if download is allowed; otherwise, false.
        /// </returns>
        Task<bool> IsDownloadAllowedAsync(OrderItem orderItem);

        /// <summary>
        /// Gets a value indicating whether license download is allowed
        /// </summary>
        /// <param name="orderItem">Order item to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the true if license download is allowed; otherwise, false.
        /// </returns>
        Task<bool> IsLicenseDownloadAllowedAsync(OrderItem orderItem);

        /// <summary>
        /// Inserts a order item
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertOrderItemAsync(OrderItem orderItem);

        /// <summary>
        /// Updates a order item
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateOrderItemAsync(OrderItem orderItem);

        #endregion

        #region Order notes

        /// <summary>
        /// Gets an order note
        /// </summary>
        /// <param name="orderNoteId">The order note identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order note
        /// </returns>
        Task<OrderNote> GetOrderNoteByIdAsync(int orderNoteId);

        /// <summary>
        /// Gets a list notes of order
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="displayToCustomer">Value indicating whether a customer can see a note; pass null to ignore</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<IList<OrderNote>> GetOrderNotesByOrderIdAsync(int orderId, bool? displayToCustomer = null);

        /// <summary>
        /// Deletes an order note
        /// </summary>
        /// <param name="orderNote">The order note</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteOrderNoteAsync(OrderNote orderNote);

        /// <summary>
        /// Formats the order note text
        /// </summary>
        /// <param name="orderNote">Order note</param>
        /// <returns>Formatted text</returns>
        string FormatOrderNoteText(OrderNote orderNote);

        /// <summary>
        /// Inserts an order note
        /// </summary>
        /// <param name="orderNote">The order note</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertOrderNoteAsync(OrderNote orderNote);

        #endregion

        #region Recurring payments

        /// <summary>
        /// Deletes a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteRecurringPaymentAsync(RecurringPayment recurringPayment);

        /// <summary>
        /// Gets a recurring payment
        /// </summary>
        /// <param name="recurringPaymentId">The recurring payment identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the recurring payment
        /// </returns>
        Task<RecurringPayment> GetRecurringPaymentByIdAsync(int recurringPaymentId);

        /// <summary>
        /// Inserts a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertRecurringPaymentAsync(RecurringPayment recurringPayment);

        /// <summary>
        /// Updates the recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateRecurringPaymentAsync(RecurringPayment recurringPayment);

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the recurring payments
        /// </returns>
        Task<IPagedList<RecurringPayment>> SearchRecurringPaymentsAsync(int storeId = 0,
            int customerId = 0, int initialOrderId = 0, OrderStatus? initialOrderStatus = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        #endregion

        #region Recurring payment history

        /// <summary>
        /// Inserts a recurring payment history entry
        /// </summary>
        /// <param name="recurringPaymentHistory">Recurring payment history entry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertRecurringPaymentHistoryAsync(RecurringPaymentHistory recurringPaymentHistory);

        /// <summary>
        /// Gets a recurring payment history
        /// </summary>
        /// <param name="recurringPayment">The recurring payment</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<IList<RecurringPaymentHistory>> GetRecurringPaymentHistoryAsync(RecurringPayment recurringPayment);

        #endregion
    }
}