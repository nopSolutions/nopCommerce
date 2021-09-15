using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Nop.Services.Orders;

namespace Nop.Web.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a best customers report search model
    /// </summary>
    public partial record BestCustomersReportSearchModel : BaseSearchModel
    {
        #region Ctor

        public BestCustomersReportSearchModel()
        {
            AvailableOrderStatuses = new List<SelectListItem>();
            AvailablePaymentStatuses = new List<SelectListItem>();
            AvailableShippingStatuses = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public OrderByEnum OrderBy { get; set; }

        [NopResourceDisplayName("Admin.Reports.Customers.BestBy.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.Reports.Customers.BestBy.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("Admin.Reports.Customers.BestBy.OrderStatus")]
        public int OrderStatusId { get; set; }

        [NopResourceDisplayName("Admin.Reports.Customers.BestBy.PaymentStatus")]
        public int PaymentStatusId { get; set; }

        [NopResourceDisplayName("Admin.Reports.Customers.BestBy.ShippingStatus")]
        public int ShippingStatusId { get; set; }

        public IList<SelectListItem> AvailableOrderStatuses { get; set; }

        public IList<SelectListItem> AvailablePaymentStatuses { get; set; }

        public IList<SelectListItem> AvailableShippingStatuses { get; set; }

        #endregion
    }
}