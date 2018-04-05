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
        /// <returns>Address attribute search model</returns>
        AddressAttributeSearchModel PrepareAddressAttributeSearchModel(AddressAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare paged address attribute list model
        /// </summary>
        /// <param name="searchModel">Address attribute search model</param>
        /// <returns>Address attribute list model</returns>
        AddressAttributeListModel PrepareAddressAttributeListModel(AddressAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare address attribute model
        /// </summary>
        /// <param name="model">Address attribute model</param>
        /// <param name="addressAttribute">Address attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Address attribute model</returns>
        AddressAttributeModel PrepareAddressAttributeModel(AddressAttributeModel model,
            AddressAttribute addressAttribute, bool excludeProperties = false);

        /// <summary>
        /// Prepare address attribute value search model
        /// </summary>
        /// <param name="searchModel">Address attribute value search model</param>
        /// <param name="addressAttribute">Address attribute</param>
        /// <returns>Address attribute value search model</returns>
        AddressAttributeValueSearchModel PrepareAddressAttributeValueSearchModel(AddressAttributeValueSearchModel searchModel,
            AddressAttribute addressAttribute);

        /// <summary>
        /// Prepare paged address attribute value list model
        /// </summary>
        /// <param name="searchModel">Address attribute value search model</param>
        /// <param name="addressAttribute">Address attribute</param>
        /// <returns>Address attribute value list model</returns>
        AddressAttributeValueListModel PrepareAddressAttributeValueListModel(AddressAttributeValueSearchModel searchModel, 
            AddressAttribute addressAttribute);

        /// <summary>
        /// Prepare address attribute value model
        /// </summary>
        /// <param name="model">Address attribute value model</param>
        /// <param name="addressAttribute">Address attribute</param>
        /// <param name="addressAttributeValue">Address attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Address attribute value model</returns>
        AddressAttributeValueModel PrepareAddressAttributeValueModel(AddressAttributeValueModel model,
            AddressAttribute addressAttribute, AddressAttributeValue addressAttributeValue, bool excludeProperties = false);
    }
}