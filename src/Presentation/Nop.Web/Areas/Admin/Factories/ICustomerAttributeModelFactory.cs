using Nop.Core.Domain.Customers;
using Nop.Web.Areas.Admin.Models.Customers;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the customer attribute model factory
/// </summary>
public partial interface ICustomerAttributeModelFactory
{
    /// <summary>
    /// Prepare customer attribute search model
    /// </summary>
    /// <param name="searchModel">Customer attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer attribute search model
    /// </returns>
    Task<CustomerAttributeSearchModel> PrepareCustomerAttributeSearchModelAsync(CustomerAttributeSearchModel searchModel);

    /// <summary>
    /// Prepare paged customer attribute list model
    /// </summary>
    /// <param name="searchModel">Customer attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer attribute list model
    /// </returns>
    Task<CustomerAttributeListModel> PrepareCustomerAttributeListModelAsync(CustomerAttributeSearchModel searchModel);

    /// <summary>
    /// Prepare customer attribute model
    /// </summary>
    /// <param name="model">Customer attribute model</param>
    /// <param name="customerAttribute">Customer attribute</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer attribute model
    /// </returns>
    Task<CustomerAttributeModel> PrepareCustomerAttributeModelAsync(CustomerAttributeModel model,
        CustomerAttribute customerAttribute, bool excludeProperties = false);

    /// <summary>
    /// Prepare paged customer attribute value list model
    /// </summary>
    /// <param name="searchModel">Customer attribute value search model</param>
    /// <param name="customerAttribute">Customer attribute</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer attribute value list model
    /// </returns>
    Task<CustomerAttributeValueListModel> PrepareCustomerAttributeValueListModelAsync(CustomerAttributeValueSearchModel searchModel,
        CustomerAttribute customerAttribute);

    /// <summary>
    /// Prepare customer attribute value model
    /// </summary>
    /// <param name="model">Customer attribute value model</param>
    /// <param name="customerAttribute">Customer attribute</param>
    /// <param name="customerAttributeValue">Customer attribute value</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer attribute value model
    /// </returns>
    Task<CustomerAttributeValueModel> PrepareCustomerAttributeValueModelAsync(CustomerAttributeValueModel model,
        CustomerAttribute customerAttribute, CustomerAttributeValue customerAttributeValue, bool excludeProperties = false);
}