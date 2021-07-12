using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Companies
{
    public partial record CompanyCustomerSearchModel : BaseSearchModel
    {
        public int CompanyId { get; set; }
    }
}