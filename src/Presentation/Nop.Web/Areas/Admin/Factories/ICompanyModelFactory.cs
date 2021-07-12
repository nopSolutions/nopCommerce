using System.Threading.Tasks;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Companies;
using Nop.Web.Areas.Admin.Models.Companies;
using Nop.Web.Areas.Admin.Models.Customers;

namespace Nop.Web.Areas.Admin.Factories
{
    public partial interface ICompanyModelFactory
    {
         Task<CompanySearchModel> PrepareCompanySearchModelAsync(CompanySearchModel searchModel);
         Task<CompanyListModel> PrepareCompanyListModelAsync(CompanySearchModel searchModel);
         Task<CompanyModel> PrepareCompanyModelAsync(CompanyModel model, Company company, bool excludeProperties = false);
         Task<CompanyCustomerListModel> PrepareCompanyCustomerListModelAsync(CompanyCustomerSearchModel searchModel, Company company);
         Task<AddCustomerToCompanySearchModel> PrepareAddCustomerToCompanySearchModelAsync(AddCustomerToCompanySearchModel searchModel);
         Task<AddCustomerToCompanyListModel> PrepareAddCustomerToCompanyListModelAsync(AddCustomerToCompanySearchModel searchModel);
        Task<CustomerAddressListModel> PrepareCompanyCustomerAddressListModelAsync(CustomerAddressSearchModel searchModel, Company company);
        Task<CustomerAddressModel> PrepareCompanyCustomerAddressModelAsync(CustomerAddressModel model,
           Company company, Address address, bool excludeProperties = false);
    }
}