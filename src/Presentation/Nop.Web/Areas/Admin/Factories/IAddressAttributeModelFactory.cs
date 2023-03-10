using Nop.Core.Domain.Common;
using Nop.Web.Areas.Admin.Models.Common;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the address attribute model factory
    /// </summary>
    public partial interface IAddressAttributeModelFactory
    {
        /// <summary>
        /// Prepare address attribute search model
        /// </summary>
        /// <param name="searchModel">Address attribute search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address attribute search model
        /// </returns>
        Task<AddressAttributeSearchModel> PrepareAddressAttributeSearchModelAsync(AddressAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare paged address attribute list model
        /// </summary>
        /// <param name="searchModel">Address attribute search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address attribute list model
        /// </returns>
        Task<AddressAttributeListModel> PrepareAddressAttributeListModelAsync(AddressAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare address attribute model
        /// </summary>
        /// <param name="model">Address attribute model</param>
        /// <param name="addressAttribute">Address attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address attribute model
        /// </returns>
        Task<AddressAttributeModel> PrepareAddressAttributeModelAsync(AddressAttributeModel model,
            AddressAttribute addressAttribute, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged address attribute value list model
        /// </summary>
        /// <param name="searchModel">Address attribute value search model</param>
        /// <param name="addressAttribute">Address attribute</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address attribute value list model
        /// </returns>
        Task<AddressAttributeValueListModel> PrepareAddressAttributeValueListModelAsync(AddressAttributeValueSearchModel searchModel,
            AddressAttribute addressAttribute);

        /// <summary>
        /// Prepare address attribute value model
        /// </summary>
        /// <param name="model">Address attribute value model</param>
        /// <param name="addressAttribute">Address attribute</param>
        /// <param name="addressAttributeValue">Address attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address attribute value model
        /// </returns>
        Task<AddressAttributeValueModel> PrepareAddressAttributeValueModelAsync(AddressAttributeValueModel model,
            AddressAttribute addressAttribute, AddressAttributeValue addressAttributeValue, bool excludeProperties = false);

        /// <summary>
        /// Prepare custom address attributes
        /// </summary>
        /// <param name="models">List of address attribute models</param>
        /// <param name="address">Address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrepareCustomAddressAttributesAsync(IList<AddressModel.AddressAttributeModel> models, Address address);
    }
}