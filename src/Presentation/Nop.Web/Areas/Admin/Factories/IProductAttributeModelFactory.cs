using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the product attribute model factory
    /// </summary>
    public partial interface IProductAttributeModelFactory
    {
        /// <summary>
        /// Prepare product attribute search model
        /// </summary>
        /// <param name="searchModel">Product attribute search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute search model
        /// </returns>
        Task<ProductAttributeSearchModel> PrepareProductAttributeSearchModelAsync(ProductAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare paged product attribute list model
        /// </summary>
        /// <param name="searchModel">Product attribute search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute list model
        /// </returns>
        Task<ProductAttributeListModel> PrepareProductAttributeListModelAsync(ProductAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare product attribute model
        /// </summary>
        /// <param name="model">Product attribute model</param>
        /// <param name="productAttribute">Product attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute model
        /// </returns>
        Task<ProductAttributeModel> PrepareProductAttributeModelAsync(ProductAttributeModel model,
            ProductAttribute productAttribute, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged predefined product attribute value list model
        /// </summary>
        /// <param name="searchModel">Predefined product attribute value search model</param>
        /// <param name="productAttribute">Product attribute</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the predefined product attribute value list model
        /// </returns>
        Task<PredefinedProductAttributeValueListModel> PreparePredefinedProductAttributeValueListModelAsync(
            PredefinedProductAttributeValueSearchModel searchModel, ProductAttribute productAttribute);

        /// <summary>
        /// Prepare predefined product attribute value model
        /// </summary>
        /// <param name="model">Predefined product attribute value model</param>
        /// <param name="productAttribute">Product attribute</param>
        /// <param name="productAttributeValue">Predefined product attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the predefined product attribute value model
        /// </returns>
        Task<PredefinedProductAttributeValueModel> PreparePredefinedProductAttributeValueModelAsync(PredefinedProductAttributeValueModel model,
            ProductAttribute productAttribute, PredefinedProductAttributeValue productAttributeValue, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged list model of products that use the product attribute
        /// </summary>
        /// <param name="searchModel">Search model of products that use the product attribute</param>
        /// <param name="productAttribute">Product attribute</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list model of products that use the product attribute
        /// </returns>
        Task<ProductAttributeProductListModel> PrepareProductAttributeProductListModelAsync(ProductAttributeProductSearchModel searchModel,
            ProductAttribute productAttribute);
    }
}