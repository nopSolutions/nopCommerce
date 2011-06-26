using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Customers
{
    public class CustomerReportsModel : BaseNopModel
    {
        public BestCustomersReportModel BestCustomersByOrderTotal { get; set; }
        public BestCustomersReportModel BestCustomersByNumberOfOrders { get; set; }
    }
}