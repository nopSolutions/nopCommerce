using System.Threading.Tasks;
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role search model
        /// </returns>
        Task<CustomerRoleSearchModel> PrepareCustomerRoleSearchModelAsync(CustomerRoleSearchModel searchModel);

        /// <summary>
        /// Prepare paged customer role list model
        /// </summary>
        /// <param name="searchModel">Customer role search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role list model
        /// </returns>
        Task<CustomerRoleListModel> PrepareCustomerRoleListModelAsync(CustomerRoleSearchModel searchModel);

        /// <summary>
        /// Prepare customer role model
        /// </summary>
        /// <param name="model">Customer role model</param>
        /// <param name="customerRole">Customer role</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role model
        /// </returns>
        Task<CustomerRoleModel> PrepareCustomerRoleModelAsync(CustomerRoleModel model, CustomerRole customerRole, bool excludeProperties = false);

        /// <summary>
        /// Prepare customer role product search model
        /// </summary>
        /// <param name="searchModel">Customer role product search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role product search model
        /// </returns>
        Task<CustomerRoleProductSearchModel> PrepareCustomerRoleProductSearchModelAsync(CustomerRoleProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged customer role product list model
        /// </summary>
        /// <param name="searchModel">Customer role product search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role product list model
        /// </returns>
        Task<CustomerRoleProductListModel> PrepareCustomerRoleProductListModelAsync(CustomerRoleProductSearchModel searchModel);
    }
}