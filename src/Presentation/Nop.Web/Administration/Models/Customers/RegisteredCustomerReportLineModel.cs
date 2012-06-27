using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Customers
{
    public partial class RegisteredCustomerReportLineModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Customers.Reports.RegisteredCustomers.Fields.Period")]
        public string Period { get; set; }

        [NopResourceDisplayName("Admin.Customers.Reports.RegisteredCustomers.Fields.Customers")]
        public int Customers { get; set; }
    }
}