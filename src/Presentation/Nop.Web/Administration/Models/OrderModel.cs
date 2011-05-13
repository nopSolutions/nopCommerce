using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Core.Domain.Customers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Nop.Admin.Models
{
    public class OrderModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Orders.Fields.ID")]
        public override int Id { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.OrderTotal")]
        public string OrderTotal { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.OrderStatus")]
        public string OrderStatus { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.PaymentStatus")]
        public string PaymentStatus { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.ShippingStatus")]
        public string ShippingStatus { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.CustomerEmail")]
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.CreatedOn")]
        public string CreatedOn { get; set; }
    }
}