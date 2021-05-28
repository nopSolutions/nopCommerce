using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Companies
{
    public partial record CompanyCustomerModel : BaseNopEntityModel
    {
        public int CompanyId { get; set; }

        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.Customer.Customers.Fields.CustomerFullName")]
        public string CustomerFullName { get; set; }

        [NopResourceDisplayName("Admin.Customer.Customers.Products.Fields.Email")]
        public string Email { get; set; }
    }
}