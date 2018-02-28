using Nop.Core.Domain.Common;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Kendoui;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the address attribute model factory
    /// </summary>
    public partial interface IAddressAttributeModelFactory
    {
        /// <summary>
        /// Prepare paged address attribute list model for the grid
        /// </summary>
        /// <param name="command">Pagination parameters</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareAddressAttributeListGridModel(DataSourceRequest command);

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
        /// Prepare paged address attribute value list model for the grid
        /// </summary>
        /// <param name="command">Pagination parameters</param>
        /// <param name="AddressAttribute">Address attribute</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareAddressAttributeValueListGridModel(DataSourceRequest command, AddressAttribute addressAttribute);

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