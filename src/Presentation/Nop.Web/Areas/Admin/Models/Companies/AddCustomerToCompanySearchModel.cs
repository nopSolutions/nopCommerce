using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Companies
{
    public partial record AddCustomerToCompanySearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.Customer.Customers.List.SearchCustomerEmail")]
        public string SearchCustomerEmail { get; set; }

        public int CompanyId { get; set; }
    }
}