using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class NeverSoldReportLineModel : BaseNopModel
    {
        public int ProductVariantId { get; set; }
        [NopResourceDisplayName("Admin.SalesReport.NeverSold.Fields.Name")]
        public string ProductVariantFullName { get; set; }
    }
}