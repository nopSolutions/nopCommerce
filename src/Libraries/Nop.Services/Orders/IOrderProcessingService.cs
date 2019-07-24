using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Payments;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order processing service interface
    /// </summary>
    public partial interface IOrderProcessingService
    {
        /// <summary>
        /// Checks order status
        /// </summary>
        /// <param name="order">Order</param>
        void CheckOrderStatus(Order order);

        /// <summary>
        /// Places an order
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Place order result</returns>
        PlaceOrderResult PlaceOrder(ProcessPaymentRequest processPaymentRequest);

        /// <summary>
        /// Update order totals
        /// </summary>
        /// <param name="updateOrderParameters">Parameters for the updating order</param>
        void UpdateOrderTotals(UpdateOrderParameters updateOrderParameters);

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        void DeleteOrder(Order order);

        /// <summary>
        /// Process next recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="paymentResult">Process payment result (info about last payment for automatic recurring payments)</param>
        /// <returns>Collection of errors</returns>
        IEnumerable<string> ProcessNextRecurringPayment(RecurringPayment recurringPayment, ProcessPaymentResult paymentResult = null);

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        IList<string> CancelRecurringPayment(RecurringPayment recurringPayment);

        /// <summary>
        /// Gets a value indicating whether a customer can cancel recurring payment
        /// </summary>
        /// <param name="customerToValidate">Customer</param>
        /// <param name="recurringPayment">Recurring Payment</param>
        /// <returns>value indicating whether a customer can cancel recurring payment</returns>
        bool CanCancelRecurringPayment(Customer customerToValidate, RecurringPayment recurringPayment);

        /// <summary>
        /// Gets a value indicating whether a customer can retry last failed recurring payment
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="recurringPayment">Recurring Payment</param>
        /// <returns>True if a customer can retry payment; otherwise false</returns>
        bool CanRetryLastRecurringPayment(Customer customer, RecurringPayment recurringPayment);

        /// <summary>
        /// Send a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        void Ship(Shipment shipment, bool notifyCustomer);
        
        /// <summary>
        /// Marks a shipment as delivered
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        void Deliver(Shipment shipment, bool notifyCustomer);

        /// <summary>
        /// Gets a value indicating whether cancel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether cancel is allowed</returns>
        bool CanCancelOrder(Order order);

        /// <summary>
        /// Cancels order
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        void CancelOrder(Order order, bool notifyCustomer);

        /// <summary>
        /// Gets a value indicating whether order can be marked as authorized
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as authorized</returns>
        bool CanMarkOrderAsAuthorized(Order order);

        /// <summary>
        /// Marks order as authorized
        /// </summary>
        /// <param name="order">Order</param>
        void MarkAsAuthorized(Order order);

        /// <summary>
        /// Gets a value indicating whether capture from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether capture from admin panel is allowed</returns>
        bool CanCapture(Order order);

        /// <summary>
        /// Capture an order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        IList<string> Capture(Order order);

        /// <summary>
        /// Gets a value indicating whether order can be marked as paid
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as paid</returns>
        bool CanMarkOrderAsPaid(Order order);

        /// <summary>
        /// Marks order as paid
        /// </summary>
        /// <param name="order">Order</param>
        void MarkOrderAsPaid(Order order);

        /// <summary>
        /// Gets a value indicating whether refund from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        bool CanRefund(Order order);

        /// <summary>
        /// Refunds an order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        IList<string> Refund(Order order);

        /// <summary>
        /// Gets a value indicating whether order can be marked as refunded
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as refunded</returns>
        bool CanRefundOffline(Order order);

        /// <summary>
        /// Refunds an order (offline)
        /// </summary>
        /// <param name="order">Order</param>
        void RefundOffline(Order order);

        /// <summary>
        /// Gets a value indicating whether partial refund from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        bool CanPartiallyRefund(Order order, decimal amountToRefund);

        /// <summary>
        /// Partially refunds an order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        IList<string> PartiallyRefund(Order order, decimal amountToRefund);

        /// <summary>
        /// Gets a value indicating whether order can be marked as partially refunded
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether order can be marked as partially refunded</returns>
        bool CanPartiallyRefundOffline(Order order, decimal amountToRefund);

        /// <summary>
        /// Partially refunds an order (offline)
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        void PartiallyRefundOffline(Order order, decimal amountToRefund);

        /// <summary>
        /// Gets a value indicating whether void from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether void from admin panel is allowed</returns>
        bool CanVoid(Order order);

        /// <summary>
        /// Voids order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Voided order</returns>
        IList<string> Void(Order order);

        /// <summary>
        /// Gets a value indicating whether order can be marked as voided
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as voided</returns>
        bool CanVoidOffline(Order order);

        /// <summary>
        /// Voids order (offline)
        /// </summary>
        /// <param name="order">Order</param>
        void VoidOffline(Order order);

        /// <summary>
        /// Place order items in current user shopping cart.
        /// </summary>
        /// <param name="order">The order</param>
        void ReOrder(Order order);
        
        /// <summary>
        /// Check whether return request is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        bool IsReturnRequestAllowed(Order order);

        /// <summary>
        /// Validate minimum order sub-total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum order sub-total amount is not reached</returns>
        bool ValidateMinOrderSubtotalAmount(IList<ShoppingCartItem> cart);

        /// <summary>
        /// Validate minimum order total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum order total amount is not reached</returns>
        bool ValidateMinOrderTotalAmount(IList<ShoppingCartItem> cart);

        /// <summary>
        /// Gets a value indicating whether payment workflow is required
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <returns>true - OK; false - minimum order total amount is not reached</returns>
        bool IsPaymentWorkflowRequired(IList<ShoppingCartItem> cart, bool? useRewardPoints = null);
    }
}
