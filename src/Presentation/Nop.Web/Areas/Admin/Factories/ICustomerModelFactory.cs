using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Web.Areas.Admin.Models.Customers;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the customer model factory
    /// </summary>
    public partial interface ICustomerModelFactory
    {
        /// <summary>
        /// Prepare customer search model
        /// </summary>
        /// <param name="searchModel">Customer search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer search model
        /// </returns>
        Task<CustomerSearchModel> PrepareCustomerSearchModelAsync(CustomerSearchModel searchModel);

        /// <summary>
        /// Prepare paged customer list model
        /// </summary>
        /// <param name="searchModel">Customer search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer list model
        /// </returns>
        Task<CustomerListModel> PrepareCustomerListModelAsync(CustomerSearchModel searchModel);

        /// <summary>
        /// Prepare customer model
        /// </summary>
        /// <param name="model">Customer model</param>
        /// <param name="customer">Customer</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer model
        /// </returns>
        Task<CustomerModel> PrepareCustomerModelAsync(CustomerModel model, Customer customer, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged reward points list model
        /// </summary>
        /// <param name="searchModel">Reward points search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the reward points list model
        /// </returns>
        Task<CustomerRewardPointsListModel> PrepareRewardPointsListModelAsync(CustomerRewardPointsSearchModel searchModel, Customer customer);

        /// <summary>
        /// Prepare paged customer address list model
        /// </summary>
        /// <param name="searchModel">Customer address search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer address list model
        /// </returns>
        Task<CustomerAddressListModel> PrepareCustomerAddressListModelAsync(CustomerAddressSearchModel searchModel, Customer customer);

        /// <summary>
        /// Prepare customer address model
        /// </summary>
        /// <param name="model">Customer address model</param>
        /// <param name="customer">Customer</param>
        /// <param name="address">Address</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer address model
        /// </returns>
        Task<CustomerAddressModel> PrepareCustomerAddressModelAsync(CustomerAddressModel model,
            Customer customer, Address address, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged customer order list model
        /// </summary>
        /// <param name="searchModel">Customer order search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer order list model
        /// </returns>
        Task<CustomerOrderListModel> PrepareCustomerOrderListModelAsync(CustomerOrderSearchModel searchModel, Customer customer);

        /// <summary>
        /// Prepare paged customer shopping cart list model
        /// </summary>
        /// <param name="searchModel">Customer shopping cart search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer shopping cart list model
        /// </returns>
        Task<CustomerShoppingCartListModel> PrepareCustomerShoppingCartListModelAsync(CustomerShoppingCartSearchModel searchModel,
            Customer customer);

        /// <summary>
        /// Prepare paged customer activity log list model
        /// </summary>
        /// <param name="searchModel">Customer activity log search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer activity log list model
        /// </returns>
        Task<CustomerActivityLogListModel> PrepareCustomerActivityLogListModelAsync(CustomerActivityLogSearchModel searchModel, Customer customer);

        /// <summary>
        /// Prepare paged customer back in stock subscriptions list model
        /// </summary>
        /// <param name="searchModel">Customer back in stock subscriptions search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer back in stock subscriptions list model
        /// </returns>
        Task<CustomerBackInStockSubscriptionListModel> PrepareCustomerBackInStockSubscriptionListModelAsync(
            CustomerBackInStockSubscriptionSearchModel searchModel, Customer customer);

        /// <summary>
        /// Prepare online customer search model
        /// </summary>
        /// <param name="searchModel">Online customer search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the online customer search model
        /// </returns>
        Task<OnlineCustomerSearchModel> PrepareOnlineCustomerSearchModelAsync(OnlineCustomerSearchModel searchModel);

        /// <summary>
        /// Prepare paged online customer list model
        /// </summary>
        /// <param name="searchModel">Online customer search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the online customer list model
        /// </returns>
        Task<OnlineCustomerListModel> PrepareOnlineCustomerListModelAsync(OnlineCustomerSearchModel searchModel);

        /// <summary>
        /// Prepare GDPR request (log) search model
        /// </summary>
        /// <param name="searchModel">GDPR request search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR request search model
        /// </returns>
        Task<GdprLogSearchModel> PrepareGdprLogSearchModelAsync(GdprLogSearchModel searchModel);

        /// <summary>
        /// Prepare paged GDPR request list model
        /// </summary>
        /// <param name="searchModel">GDPR request search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR request list model
        /// </returns>
        Task<GdprLogListModel> PrepareGdprLogListModelAsync(GdprLogSearchModel searchModel);
    }
}