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
        /// <returns>Product attribute search model</returns>
        ProductAttributeSearchModel PrepareProductAttributeSearchModel(ProductAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare paged product attribute list model
        /// </summary>
        /// <param name="searchModel">Product attribute search model</param>
        /// <returns>Product attribute list model</returns>
        ProductAttributeListModel PrepareProductAttributeListModel(ProductAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare product attribute model
        /// </summary>
        /// <param name="model">Product attribute model</param>
        /// <param name="productAttribute">Product attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Product attribute model</returns>
        ProductAttributeModel PrepareProductAttributeModel(ProductAttributeModel model,
            ProductAttribute productAttribute, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged predefined product attribute value list model
        /// </summary>
        /// <param name="searchModel">Predefined product attribute value search model</param>
        /// <param name="productAttribute">Product attribute</param>
        /// <returns>Predefined product attribute value list model</returns>
        PredefinedProductAttributeValueListModel PreparePredefinedProductAttributeValueListModel(
            PredefinedProductAttributeValueSearchModel searchModel, ProductAttribute productAttribute);

        /// <summary>
        /// Prepare predefined product attribute value model
        /// </summary>
        /// <param name="model">Predefined product attribute value model</param>
        /// <param name="productAttribute">Product attribute</param>
        /// <param name="productAttributeValue">Predefined product attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Predefined product attribute value model</returns>
        PredefinedProductAttributeValueModel PreparePredefinedProductAttributeValueModel(PredefinedProductAttributeValueModel model,
            ProductAttribute productAttribute, PredefinedProductAttributeValue productAttributeValue, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged list model of products that use the product attribute
        /// </summary>
        /// <param name="searchModel">Search model of products that use the product attribute</param>
        /// <param name="productAttribute">Product attribute</param>
        /// <returns>List model of products that use the product attribute</returns>
        ProductAttributeProductListModel PrepareProductAttributeProductListModel(ProductAttributeProductSearchModel searchModel,
            ProductAttribute productAttribute);
    }
}