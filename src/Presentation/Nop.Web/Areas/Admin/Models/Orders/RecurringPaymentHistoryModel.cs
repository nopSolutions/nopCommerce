using System;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a recurring payment history model
    /// </summary>
    public partial class RecurringPaymentHistoryModel : BaseNopEntityModel
    {
        #region Properties

        public int OrderId { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.History.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }

        public int RecurringPaymentId { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.History.OrderStatus")]
        public string OrderStatus { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.History.PaymentStatus")]
        public string PaymentStatus { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.History.ShippingStatus")]
        public string ShippingStatus { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.History.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        #endregion
    }
}