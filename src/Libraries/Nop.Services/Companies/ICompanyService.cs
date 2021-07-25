using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Companies;

namespace Nop.Services.Companies
{
    public partial interface ICompanyService
    {
        #region Companies
        Task<IPagedList<Company>> GetAllCompaniesAsync(string name = null, string email = null,
          int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);
        Task DeleteCompanyAsync(Company company);
        Task<Company> GetCompanyByIdAsync(int companyId);
        Task InsertCompanyAsync(Company company);
        Task UpdateCompanyAsync(Company company);

        #endregion

        #region Company Customer

        CompanyCustomer FindCompanyCustomer(IList<CompanyCustomer> source, int customerId, int companyId);
        Task DeleteCompanyCustomerAsync(CompanyCustomer companyCustomer);
        Task<IPagedList<CompanyCustomer>> GetCompanyCustomersByCompanyIdAsync(int companyId,
           int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);
        Task<IList<CompanyCustomer>> GetCompanyCustomersByCustomerIdAsync(int customerId, bool showHidden = false);
        Task<Company> GetCompanyByCustomerIdAsync(int customerId);
        Task<CompanyCustomer> GetCompanyCustomersByIdAsync(int companyCustomersId);
        Task InsertCompanyCustomerAsync(CompanyCustomer companyCustomer);
        Task UpdateCompanyCustomerAsync(CompanyCustomer companyCustomer);

        #endregion
    }
}