using System;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    /// <summary>
    /// Represents a customer order model
    /// </summary>
    public partial class CustomerOrderModel : BaseNopEntityModel
    {
        #region Properties

        public override int Id { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Orders.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Orders.OrderStatus")]
        public string OrderStatus { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Orders.OrderStatus")]
        public int OrderStatusId { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Orders.PaymentStatus")]
        public string PaymentStatus { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Orders.ShippingStatus")]
        public string ShippingStatus { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Orders.OrderTotal")]
        public string OrderTotal { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Orders.Store")]
        public string StoreName { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Orders.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        #endregion
    }
}