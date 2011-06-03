
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class OrderIncompleteReportLineModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.SalesReport.Incomplete.Item")]
        public string Item { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Incomplete.Total")]
        public string Total { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Incomplete.Count")]
        public int Count { get; set; }
    }
}
