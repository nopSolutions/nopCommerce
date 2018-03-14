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
        /// <param name="model">Customer search model</param>
        /// <returns>Customer search model</returns>
        CustomerSearchModel PrepareCustomerSearchModel(CustomerSearchModel model);

        /// <summary>
        /// Prepare paged customer list model
        /// </summary>
        /// <param name="searchModel">Customer search model</param>
        /// <returns>Customer list model</returns>
        CustomerListModel PrepareCustomerListModel(CustomerSearchModel searchModel);

        /// <summary>
        /// Prepare customer model
        /// </summary>
        /// <param name="model">Customer model</param>
        /// <param name="customer">Customer</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Customer model</returns>
        CustomerModel PrepareCustomerModel(CustomerModel model, Customer customer, bool excludeProperties = false);

        /// <summary>
        /// Prepare reward points search model
        /// </summary>
        /// <param name="model">Reward points search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Reward points search model</returns>
        CustomerRewardPointsSearchModel PrepareRewardPointsSearchModel(CustomerRewardPointsSearchModel model, Customer customer);

        /// <summary>
        /// Prepare paged reward points list model
        /// </summary>
        /// <param name="searchModel">Reward points search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Reward points list model</returns>
        CustomerRewardPointsListModel PrepareRewardPointsListModel(CustomerRewardPointsSearchModel searchModel, Customer customer);

        /// <summary>
        /// Prepare customer address search model
        /// </summary>
        /// <param name="model">Customer address search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer address search model</returns>
        CustomerAddressSearchModel PrepareCustomerAddressSearchModel(CustomerAddressSearchModel model, Customer customer);

        /// <summary>
        /// Prepare paged customer address list model
        /// </summary>
        /// <param name="searchModel">Customer address search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer address list model</returns>
        CustomerAddressListModel PrepareCustomerAddressListModel(CustomerAddressSearchModel searchModel, Customer customer);

        /// <summary>
        /// Prepare customer address model
        /// </summary>
        /// <param name="model">Customer address model</param>
        /// <param name="customer">Customer</param>
        /// <param name="address">Address</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Customer address model</returns>
        CustomerAddressModel PrepareCustomerAddressModel(CustomerAddressModel model,
            Customer customer, Address address, bool excludeProperties = false);

        /// <summary>
        /// Prepare customer order search model
        /// </summary>
        /// <param name="model">Customer order search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer order search model</returns>
        CustomerOrderSearchModel PrepareCustomerOrderSearchModel(CustomerOrderSearchModel model, Customer customer);

        /// <summary>
        /// Prepare paged customer order list model
        /// </summary>
        /// <param name="searchModel">Customer order search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer order list model</returns>
        CustomerOrderListModel PrepareCustomerOrderListModel(CustomerOrderSearchModel searchModel, Customer customer);

        /// <summary>
        /// Prepare customer reports search model
        /// </summary>
        /// <param name="model">Customer reports search model</param>
        /// <returns>Customer reports search model</returns>
        CustomerReportsSearchModel PrepareCustomerReportsSearchModel(CustomerReportsSearchModel model);

        /// <summary>
        /// Prepare best customers report search model
        /// </summary>
        /// <param name="model">Best customers report search model</param>
        /// <returns>Best customers report search model</returns>
        BestCustomersReportSearchModel PrepareBestCustomersReportSearchModel(BestCustomersReportSearchModel model);

        /// <summary>
        /// Prepare paged best customers report list model
        /// </summary>
        /// <param name="searchModel">Best customers report search model</param>
        /// <returns>Best customers report list model</returns>
        BestCustomersReportListModel PrepareBestCustomersReportListModel(BestCustomersReportSearchModel searchModel);

        /// <summary>
        /// Prepare registered customers report search model
        /// </summary>
        /// <param name="model">Registered customers report search model</param>
        /// <returns>Registered customers report search model</returns>
        RegisteredCustomersReportSearchModel PrepareRegisteredCustomersReportSearchModel(RegisteredCustomersReportSearchModel model);

        /// <summary>
        /// Prepare paged registered customers report list model
        /// </summary>
        /// <param name="searchModel">Registered customers report search model</param>
        /// <returns>Registered customers report list model</returns>
        RegisteredCustomersReportListModel PrepareRegisteredCustomersReportListModel(RegisteredCustomersReportSearchModel searchModel);

        /// <summary>
        /// Prepare customer shopping cart search model
        /// </summary>
        /// <param name="model">Customer shopping cart search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer shopping cart search model</returns>
        CustomerShoppingCartSearchModel PrepareCustomerShoppingCartSearchModel(CustomerShoppingCartSearchModel model,
            Customer customer);

        /// <summary>
        /// Prepare paged customer shopping cart list model
        /// </summary>
        /// <param name="searchModel">Customer shopping cart search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer shopping cart list model</returns>
        CustomerShoppingCartListModel PrepareCustomerShoppingCartListModel(CustomerShoppingCartSearchModel searchModel,
            Customer customer);

        /// <summary>
        /// Prepare customer activity log search model
        /// </summary>
        /// <param name="model">Customer activity log search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer activity log search model</returns>
        CustomerActivityLogSearchModel PrepareCustomerActivityLogSearchModel(CustomerActivityLogSearchModel model, Customer customer);

        /// <summary>
        /// Prepare paged customer activity log list model
        /// </summary>
        /// <param name="searchModel">Customer activity log search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer activity log list model</returns>
        CustomerActivityLogListModel PrepareCustomerActivityLogListModel(CustomerActivityLogSearchModel searchModel, Customer customer);

        /// <summary>
        /// Prepare customer back in stock subscriptions search model
        /// </summary>
        /// <param name="model">Customer back in stock subscriptions search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer back in stock subscriptions search model</returns>
        CustomerBackInStockSubscriptionSearchModel PrepareCustomerBackInStockSubscriptionSearchModel(
            CustomerBackInStockSubscriptionSearchModel model, Customer customer);

        /// <summary>
        /// Prepare paged customer back in stock subscriptions list model
        /// </summary>
        /// <param name="searchModel">Customer back in stock subscriptions search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer back in stock subscriptions list model</returns>
        CustomerBackInStockSubscriptionListModel PrepareCustomerBackInStockSubscriptionListModel(
            CustomerBackInStockSubscriptionSearchModel searchModel, Customer customer);
    }
}