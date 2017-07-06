using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    public partial class CountryReportLineModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.SalesReport.Country.Fields.CountryName")]
        public string CountryName { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Country.Fields.TotalOrders")]
        public int TotalOrders { get; set; }

        [NopResourceDisplayName("Admin.SalesReport.Country.Fields.SumOrders")]
        public string SumOrders { get; set; }
    }
}