using System.Collections.Generic;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Companies
{
    public partial record CompanyModel : BaseNopEntityModel, ILocalizedModel<CompanyLocalizedModel>
    {
        public CompanyModel()
        {
            CompanyCustomerSearchModel = new CompanyCustomerSearchModel();
            Locales = new List<CompanyLocalizedModel>();
            CustomerAddressSearchModel = new CustomerAddressSearchModel();
        }
        [NopResourceDisplayName("Admin.Companies.Company.Fields.Email")]
        public string Email { get; set; }
        
        [NopResourceDisplayName("Admin.Companies.Company.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Companies.Company.Fields.AmountLimit")]
        public decimal AmountLimit { get; set; }

        public CompanyCustomerSearchModel CompanyCustomerSearchModel { get; set; }

        public IList<CompanyLocalizedModel> Locales { get; set; }

        public CustomerAddressSearchModel CustomerAddressSearchModel { get; set; }

        public bool CustomerExist { get; set; }
    }

    public partial record CompanyLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Companies.Company.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Companies.Company.Fields.Email")]
        public string Email { get; set; }
    }
}