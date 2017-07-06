using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    public partial class NeverSoldReportLineModel : BaseNopModel
    {
        public int ProductId { get; set; }
        [NopResourceDisplayName("Admin.SalesReport.NeverSold.Fields.Name")]
        public string ProductName { get; set; }
    }
}