using System;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Orders;

namespace Nop.Plugin.Api.Models.OrdersParameters
{
    // JsonProperty is used only for swagger
    public class BaseOrdersParametersModel
    {
        public BaseOrdersParametersModel()
        {
            CreatedAtMax = null;
            CreatedAtMin = null;
            Status = null;
            PaymentStatus = null;
            ShippingStatus = null;
            CustomerId = null;
        }

        /// <summary>
        ///     Show orders created after date (format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("created_at_min")]
        public DateTime? CreatedAtMin { get; set; }

        /// <summary>
        ///     Show orders created before date(format: 2008-12-31 03:00)
        /// </summary>
        [JsonProperty("created_at_max")]
        public DateTime? CreatedAtMax { get; set; }

        /// <summary>
        ///     <ul>
        ///         <li>pending - All open orders (default)</li>
        ///         <li>processing - Show only payed or shipped orders</li>
        ///         <li>complete - Show only the complete orders</li>
        ///         <li>cancelled - Show only cancelled orders</li>
        ///     </ul>
        /// </summary>
        [JsonProperty("status")]
        public OrderStatus? Status { get; set; }

        /// <summary>
        ///     <ul>
        ///         <li>pending - Show all orders that are not payed</li>
        ///         <li>authorized - Show all orders that are authorized by the payment provider</li>
        ///         <li>paid - Show the paid orders</li>
        ///         <li>partiallyRefunded - Show only the partially refunded orders</li>
        ///         <li>refunded - Show only the refunded orders</li>
        ///         <li>voided - Show only the voided orders</li>
        ///     </ul>
        /// </summary>
        [JsonProperty("payment_status")]
        public PaymentStatus? PaymentStatus { get; set; }

        /// <summary>
        ///     <ul>
        ///         <li>shippingNotRequired - Show only the orders that do not require shipping</li>
        ///         <li>notYetShipped - Show only the orders that are not shipped</li>
        ///         <li>partiallyShipped - Show only the orders that are partially shippied</li>
        ///         <li>shipped - Show only the shipped orders</li>
        ///         <li>delivered - Show only the delivered orders</li>
        ///     </ul>
        /// </summary>
        [JsonProperty("shipping_status")]
        public ShippingStatus? ShippingStatus { get; set; }

        /// <summary>
        ///     Show all the orders for this customer
        /// </summary>
        [JsonProperty("customer_id")]
        public int? CustomerId { get; set; }
    }
}
