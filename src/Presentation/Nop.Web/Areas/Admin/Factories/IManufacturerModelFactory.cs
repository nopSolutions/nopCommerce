using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the manufacturer model factory
    /// </summary>
    public partial interface IManufacturerModelFactory
    {
        /// <summary>
        /// Prepare manufacturer search model
        /// </summary>
        /// <param name="searchModel">Manufacturer search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer search model
        /// </returns>
        Task<ManufacturerSearchModel> PrepareManufacturerSearchModelAsync(ManufacturerSearchModel searchModel);

        /// <summary>
        /// Prepare paged manufacturer list model
        /// </summary>
        /// <param name="searchModel">Manufacturer search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer list model
        /// </returns>
        Task<ManufacturerListModel> PrepareManufacturerListModelAsync(ManufacturerSearchModel searchModel);

        /// <summary>
        /// Prepare manufacturer model
        /// </summary>
        /// <param name="model">Manufacturer model</param>
        /// <param name="manufacturer">Manufacturer</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer model
        /// </returns>
        Task<ManufacturerModel> PrepareManufacturerModelAsync(ManufacturerModel model,
            Manufacturer manufacturer, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged manufacturer product list model
        /// </summary>
        /// <param name="searchModel">Manufacturer product search model</param>
        /// <param name="manufacturer">Manufacturer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer product list model
        /// </returns>
        Task<ManufacturerProductListModel> PrepareManufacturerProductListModelAsync(ManufacturerProductSearchModel searchModel,
            Manufacturer manufacturer);

        /// <summary>
        /// Prepare product search model to add to the manufacturer
        /// </summary>
        /// <param name="searchModel">Product search model to add to the manufacturer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product search model to add to the manufacturer
        /// </returns>
        Task<AddProductToManufacturerSearchModel> PrepareAddProductToManufacturerSearchModelAsync(AddProductToManufacturerSearchModel searchModel);

        /// <summary>
        /// Prepare paged product list model to add to the manufacturer
        /// </summary>
        /// <param name="searchModel">Product search model to add to the manufacturer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product list model to add to the manufacturer
        /// </returns>
        Task<AddProductToManufacturerListModel> PrepareAddProductToManufacturerListModelAsync(AddProductToManufacturerSearchModel searchModel);
    }
}