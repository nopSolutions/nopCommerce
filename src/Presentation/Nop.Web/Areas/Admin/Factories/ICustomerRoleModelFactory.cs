using Nop.Core.Domain.Customers;
using Nop.Web.Areas.Admin.Models.Customers;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the customer role model factory
    /// </summary>
    public partial interface ICustomerRoleModelFactory
    {
        /// <summary>
        /// Prepare customer role search model
        /// </summary>
        /// <param name="searchModel">Customer role search model</param>
        /// <returns>Customer role search model</returns>
        CustomerRoleSearchModel PrepareCustomerRoleSearchModel(CustomerRoleSearchModel searchModel);

        /// <summary>
        /// Prepare paged customer role list model
        /// </summary>
        /// <param name="searchModel">Customer role search model</param>
        /// <returns>Customer role list model</returns>
        CustomerRoleListModel PrepareCustomerRoleListModel(CustomerRoleSearchModel searchModel);

        /// <summary>
        /// Prepare customer role model
        /// </summary>
        /// <param name="model">Customer role model</param>
        /// <param name="customerRole">Customer role</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Customer role model</returns>
        CustomerRoleModel PrepareCustomerRoleModel(CustomerRoleModel model, CustomerRole customerRole, bool excludeProperties = false);

        /// <summary>
        /// Prepare customer role product search model
        /// </summary>
        /// <param name="searchModel">Customer role product search model</param>
        /// <returns>Customer role product search model</returns>
        CustomerRoleProductSearchModel PrepareCustomerRoleProductSearchModel(CustomerRoleProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged customer role product list model
        /// </summary>
        /// <param name="searchModel">Customer role product search model</param>
        /// <returns>Customer role product list model</returns>
        CustomerRoleProductListModel PrepareCustomerRoleProductListModel(CustomerRoleProductSearchModel searchModel);
    }
}