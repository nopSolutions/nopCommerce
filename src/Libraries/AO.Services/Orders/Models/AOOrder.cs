using Nop.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace AO.Services.Orders.Models
{
    public class AOOrder : BaseEntity
    {
        [Key]
        public virtual int OrderId { get; set; }

        public virtual decimal TotalOrderAmount { get; set; }

        public virtual string Currency { get; set; }

        public virtual DateTime OrderDateTime { get; set; }

        public virtual string UserName { get; set; }

        public virtual string CustomerInfo { get; set; }

        public virtual string CustomerEmail { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string ShippingInfo { get; set; }

        public virtual string CheckoutAttributeDescription { get; set; }

        public virtual string OrderItems { get; set; }

        public virtual string InternalOrderNotes { get; set; }

        public virtual int PaymentStatusId { get; set; }

        public virtual string PaymentMethodSystemName { get; set; }

        /// <summary>
        /// Contains the first shipment for the order in this format (ShipmentId;AdminComment)
        /// </summary>
        public virtual string Shipment { get; set; }

        public virtual string AuthorizationTransactionId { get; set; }

        public virtual string FirstName { get; set; }
    }
}
