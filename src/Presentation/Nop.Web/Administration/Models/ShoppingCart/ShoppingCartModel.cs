using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Models.Common;
using Nop.Admin.Validators;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Nop.Admin.Models.ShoppingCart
{
    public class ShoppingCartModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.CurrentCarts.Customer")]
        public int CustomerId { get; set; }
        [NopResourceDisplayName("Admin.CurrentCarts.Customer")]
        public string CustomerName { get; set; }

        [NopResourceDisplayName("Admin.CurrentCarts.TotalItems")]
        public int TotalItems { get; set; }
    }
}