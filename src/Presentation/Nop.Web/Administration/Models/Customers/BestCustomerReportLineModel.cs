using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Customers
{
    public partial class BestCustomerReportLineModel : BaseNopModel
    {
        public int CustomerId { get; set; }
        [NopResourceDisplayName("Admin.Customers.Reports.BestBy.Fields.Customer")]
        public string CustomerName { get; set; }

        [NopResourceDisplayName("Admin.Customers.Reports.BestBy.Fields.OrderTotal")]
        public string OrderTotal { get; set; }

        [NopResourceDisplayName("Admin.Customers.Reports.BestBy.Fields.OrderCount")]
        public decimal OrderCount { get; set; }
    }
}