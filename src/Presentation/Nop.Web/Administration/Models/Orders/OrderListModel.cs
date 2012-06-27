using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class OrderListModel : BaseNopModel
    {
        public OrderListModel()
        {
            AvailableOrderStatuses = new List<SelectListItem>();
            AvailablePaymentStatuses = new List<SelectListItem>();
            AvailableShippingStatuses = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Orders.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.CustomerEmail")]
        [AllowHtml]
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.OrderStatus")]
        public int OrderStatusId { get; set; }
        [NopResourceDisplayName("Admin.Orders.List.PaymentStatus")]
        public int PaymentStatusId { get; set; }
        [NopResourceDisplayName("Admin.Orders.List.ShippingStatus")]
        public int ShippingStatusId { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.OrderGuid")]
        [AllowHtml]
        public string OrderGuid { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.GoDirectlyToNumber")]
        [AllowHtml]
        public int GoDirectlyToNumber { get; set; }

        

        public IList<SelectListItem> AvailableOrderStatuses { get; set; }
        public IList<SelectListItem> AvailablePaymentStatuses { get; set; }
        public IList<SelectListItem> AvailableShippingStatuses { get; set; }
    }
}