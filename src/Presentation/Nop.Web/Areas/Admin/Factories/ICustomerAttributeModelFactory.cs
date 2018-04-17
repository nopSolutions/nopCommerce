using Nop.Core.Domain.Customers;
using Nop.Web.Areas.Admin.Models.Customers;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the customer attribute model factory
    /// </summary>
    public partial interface ICustomerAttributeModelFactory
    {
        /// <summary>
        /// Prepare customer attribute search model
        /// </summary>
        /// <param name="searchModel">Customer attribute search model</param>
        /// <returns>Customer attribute search model</returns>
        CustomerAttributeSearchModel PrepareCustomerAttributeSearchModel(CustomerAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare paged customer attribute list model
        /// </summary>
        /// <param name="searchModel">Customer attribute search model</param>
        /// <returns>Customer attribute list model</returns>
        CustomerAttributeListModel PrepareCustomerAttributeListModel(CustomerAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare customer attribute model
        /// </summary>
        /// <param name="model">Customer attribute model</param>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Customer attribute model</returns>
        CustomerAttributeModel PrepareCustomerAttributeModel(CustomerAttributeModel model,
            CustomerAttribute customerAttribute, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged customer attribute value list model
        /// </summary>
        /// <param name="searchModel">Customer attribute value search model</param>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <returns>Customer attribute value list model</returns>
        CustomerAttributeValueListModel PrepareCustomerAttributeValueListModel(CustomerAttributeValueSearchModel searchModel,
            CustomerAttribute customerAttribute);

        /// <summary>
        /// Prepare customer attribute value model
        /// </summary>
        /// <param name="model">Customer attribute value model</param>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Customer attribute value model</returns>
        CustomerAttributeValueModel PrepareCustomerAttributeValueModel(CustomerAttributeValueModel model,
            CustomerAttribute customerAttribute, CustomerAttributeValue customerAttributeValue, bool excludeProperties = false);
    }
}