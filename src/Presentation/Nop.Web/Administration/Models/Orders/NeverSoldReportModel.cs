using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class NeverSoldReportModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.SalesReport.NeverSold.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.NeverSold.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }
    }
}