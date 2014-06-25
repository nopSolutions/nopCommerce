using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class CountryReportModel : BaseNopModel
    {
        public CountryReportModel()
        {
            AvailableOrderStatuses = new List<SelectListItem>();
            AvailablePaymentStatuses = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.SalesReport.Country.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Country.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }


        [NopResourceDisplayName("Admin.SalesReport.Country.OrderStatus")]
        public int OrderStatusId { get; set; }
        [NopResourceDisplayName("Admin.SalesReport.Country.PaymentStatus")]
        public int PaymentStatusId { get; set; }

        public IList<SelectListItem> AvailableOrderStatuses { get; set; }
        public IList<SelectListItem> AvailablePaymentStatuses { get; set; }
    }
}