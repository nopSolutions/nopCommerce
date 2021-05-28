using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Companies
{
    public partial record CompanySearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.Companies.Company.List.SearchCompanyName")]
        public string SearchCompanyName { get; set; }
    }
}