using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    public partial class OrderListModel : BaseNopModel
    {
        public OrderListModel()
        {
            AvailableOrderStatuses = new List<SelectListItem>();
            AvailablePaymentStatuses = new List<SelectListItem>();
            AvailableShippingStatuses = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
            AvailablePaymentMethods = new List<SelectListItem>();
            AvailableCountries = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Orders.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.OrderStatus")]
        public int[] OrderStatusIds { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.PaymentStatus")]
        public int[] PaymentStatusIds { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.ShippingStatus")]
        public int[] ShippingStatusIds { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.PaymentMethod")]
        public string PaymentMethodSystemName { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.Store")]
        public int StoreId { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.Vendor")]
        public int VendorId { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.Warehouse")]
        public int WarehouseId { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.Product")]
        public int ProductId { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.BillingEmail")]
        public string BillingEmail { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.BillingLastName")]
        public string BillingLastName { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.BillingCountry")]
        public int BillingCountryId { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.OrderNotes")]
        public string OrderNotes { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.GoDirectlyToNumber")]
        public string GoDirectlyToCustomOrderNumber { get; set; }

        public bool IsLoggedInAsVendor { get; set; }

        public IList<SelectListItem> AvailableOrderStatuses { get; set; }
        public IList<SelectListItem> AvailablePaymentStatuses { get; set; }
        public IList<SelectListItem> AvailableShippingStatuses { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }
        public IList<SelectListItem> AvailablePaymentMethods { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }
    }
}