using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    public partial class BestsellersReportLineModel : BaseNopModel
    {
        public int ProductId { get; set; }
        [NopResourceDisplayName("Admin.SalesReport.Bestsellers.Fields.Name")]
        public string ProductName { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Bestsellers.Fields.TotalAmount")]
        public string TotalAmount { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Bestsellers.Fields.TotalQuantity")]
        public decimal TotalQuantity { get; set; }
    }
}