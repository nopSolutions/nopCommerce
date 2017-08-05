using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Validators.Affiliates;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Affiliates
{
    [Validator(typeof(AffiliateValidator))]
    public partial class AffiliateModel : BaseNopEntityModel
    {
        public AffiliateModel()
        {
            Address = new AddressModel();
            AffiliatedOrderList = new AffiliatedOrderListModel();
        }

        [NopResourceDisplayName("Admin.Affiliates.Fields.URL")]
        public string Url { get; set; }
        
        [NopResourceDisplayName("Admin.Affiliates.Fields.AdminComment")]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.Fields.FriendlyUrlName")]
        public string FriendlyUrlName { get; set; }
        
        [NopResourceDisplayName("Admin.Affiliates.Fields.Active")]
        public bool Active { get; set; }

        public AddressModel Address { get; set; }

        public AffiliatedOrderListModel AffiliatedOrderList { get; set; }

        #region Nested classes

        public partial class AffiliatedOrderListModel : BaseNopModel
        {
            public AffiliatedOrderListModel()
            {
                AvailableOrderStatuses = new List<SelectListItem>();
                AvailablePaymentStatuses = new List<SelectListItem>();
                AvailableShippingStatuses = new List<SelectListItem>();
            }

            public int AffliateId { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.StartDate")]
            [UIHint("DateNullable")]
            public DateTime? StartDate { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.EndDate")]
            [UIHint("DateNullable")]
            public DateTime? EndDate { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.OrderStatus")]
            public int OrderStatusId { get; set; }
            [NopResourceDisplayName("Admin.Affiliates.Orders.PaymentStatus")]
            public int PaymentStatusId { get; set; }
            [NopResourceDisplayName("Admin.Affiliates.Orders.ShippingStatus")]
            public int ShippingStatusId { get; set; }

            public IList<SelectListItem> AvailableOrderStatuses { get; set; }
            public IList<SelectListItem> AvailablePaymentStatuses { get; set; }
            public IList<SelectListItem> AvailableShippingStatuses { get; set; }
        }

        public partial class AffiliatedOrderModel : BaseNopEntityModel
        {
            public override int Id { get; set; }
            [NopResourceDisplayName("Admin.Affiliates.Orders.CustomOrderNumber")]
            public string CustomOrderNumber { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.OrderStatus")]
            public string OrderStatus { get; set; }
            [NopResourceDisplayName("Admin.Affiliates.Orders.OrderStatus")]
            public int OrderStatusId { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.PaymentStatus")]
            public string PaymentStatus { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.ShippingStatus")]
            public string ShippingStatus { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.OrderTotal")]
            public string OrderTotal { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class AffiliatedCustomerModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Affiliates.Customers.Name")]
            public string Name { get; set; }
        }

        #endregion
    }
}