using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Nop.Admin.Models
{
    public class SalesReportLineModel : BaseNopModel
    {
        public int ProductVariantId { get; set; }
        [NopResourceDisplayName("Admin.SalesReport.Fields.Name")]
        public string ProductVariantFullName { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Fields.TotalQuantity")]
        public decimal TotalQuantity { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Fields.TotalPrice")]
        public string TotalPrice { get; set; }
    }
}