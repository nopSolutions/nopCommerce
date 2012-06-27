using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Customers
{
    public partial class BestCustomersReportModel : BaseNopModel
    {
        public BestCustomersReportModel()
        {
            AvailableOrderStatuses = new List<SelectListItem>();
            AvailablePaymentStatuses = new List<SelectListItem>();
            AvailableShippingStatuses = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Customers.Reports.BestBy.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.Customers.Reports.BestBy.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("Admin.Customers.Reports.BestBy.OrderStatus")]
        public int OrderStatusId { get; set; }
        [NopResourceDisplayName("Admin.Customers.Reports.BestBy.PaymentStatus")]
        public int PaymentStatusId { get; set; }
        [NopResourceDisplayName("Admin.Customers.Reports.BestBy.ShippingStatus")]
        public int ShippingStatusId { get; set; }

        public IList<SelectListItem> AvailableOrderStatuses { get; set; }
        public IList<SelectListItem> AvailablePaymentStatuses { get; set; }
        public IList<SelectListItem> AvailableShippingStatuses { get; set; }
    }
}