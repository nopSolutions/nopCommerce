using System;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class RecurringPaymentModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.RecurringPayments.Fields.ID")]
        public override int Id { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.Fields.CycleLength")]
        public int CycleLength { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.Fields.CyclePeriod")]
        public int CyclePeriodId { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.Fields.CyclePeriod")]
        public string CyclePeriodStr { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.Fields.TotalCycles")]
        public int TotalCycles { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.Fields.StartDate")]
        public string StartDate { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.Fields.IsActive")]
        public bool IsActive { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.Fields.NextPaymentDate")]
        public string NextPaymentDate { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.Fields.CyclesRemaining")]
        public int CyclesRemaining { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.Fields.InitialOrder")]
        public int InitialOrderId { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.Fields.Customer")]
        public int CustomerId { get; set; }
        [NopResourceDisplayName("Admin.RecurringPayments.Fields.Customer")]
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Admin.RecurringPayments.Fields.PaymentType")]
        public string PaymentType { get; set; }
        
        public bool CanCancelRecurringPayment { get; set; }

        #region Nested classes


        public partial class RecurringPaymentHistoryModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.RecurringPayments.History.Order")]
            public int OrderId { get; set; }

            public int RecurringPaymentId { get; set; }

            [NopResourceDisplayName("Admin.RecurringPayments.History.OrderStatus")]
            public string OrderStatus { get; set; }

            [NopResourceDisplayName("Admin.RecurringPayments.History.PaymentStatus")]
            public string PaymentStatus { get; set; }

            [NopResourceDisplayName("Admin.RecurringPayments.History.ShippingStatus")]
            public string ShippingStatus { get; set; }

            [NopResourceDisplayName("Admin.RecurringPayments.History.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        #endregion
    }
}