using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the product model factory
    /// </summary>
    public partial interface IProductModelFactory
    {
        /// <summary>
        /// Prepare product search model
        /// </summary>
        /// <param name="searchModel">Product search model</param>
        /// <returns>Product search model</returns>
        ProductSearchModel PrepareProductSearchModel(ProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged product list model
        /// </summary>
        /// <param name="searchModel">Product search model</param>
        /// <returns>Product list model</returns>
        ProductListModel PrepareProductListModel(ProductSearchModel searchModel);

        /// <summary>
        /// Prepare product model
        /// </summary>
        /// <param name="model">Product model</param>
        /// <param name="product">Product</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Product model</returns>
        ProductModel PrepareProductModel(ProductModel model, Product product, bool excludeProperties = false);

        /// <summary>
        /// Prepare required product search model to add to the product
        /// </summary>
        /// <param name="searchModel">Required product search model to add to the product</param>
        /// <returns>Required product search model to add to the product</returns>
        AddRequiredProductSearchModel PrepareAddRequiredProductSearchModel(AddRequiredProductSearchModel searchModel);

        /// <summary>
        /// Prepare required product list model to add to the product
        /// </summary>
        /// <param name="searchModel">Required product search model to add to the product</param>
        /// <returns>Required product list model to add to the product</returns>
        AddRequiredProductListModel PrepareAddRequiredProductListModel(AddRequiredProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged related product list model
        /// </summary>
        /// <param name="searchModel">Related product search model</param>
        /// <param name="product">Product</param>
        /// <returns>Related product list model</returns>
        RelatedProductListModel PrepareRelatedProductListModel(RelatedProductSearchModel searchModel, Product product);

        /// <summary>
        /// Prepare related product search model to add to the product
        /// </summary>
        /// <param name="searchModel">Related product search model to add to the product</param>
        /// <returns>Related product search model to add to the product</returns>
        AddRelatedProductSearchModel PrepareAddRelatedProductSearchModel(AddRelatedProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged related product list model to add to the product
        /// </summary>
        /// <param name="searchModel">Related product search model to add to the product</param>
        /// <returns>Related product list model to add to the product</returns>
        AddRelatedProductListModel PrepareAddRelatedProductListModel(AddRelatedProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged cross-sell product list model
        /// </summary>
        /// <param name="searchModel">Cross-sell product search model</param>
        /// <param name="product">Product</param>
        /// <returns>Cross-sell product list model</returns>
        CrossSellProductListModel PrepareCrossSellProductListModel(CrossSellProductSearchModel searchModel, Product product);

        /// <summary>
        /// Prepare cross-sell product search model to add to the product
        /// </summary>
        /// <param name="searchModel">Cross-sell product search model to add to the product</param>
        /// <returns>Cross-sell product search model to add to the product</returns>
        AddCrossSellProductSearchModel PrepareAddCrossSellProductSearchModel(AddCrossSellProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged cross-sell product list model to add to the product
        /// </summary>
        /// <param name="searchModel">Cross-sell product search model to add to the product</param>
        /// <returns>Cross-sell product list model to add to the product</returns>
        AddCrossSellProductListModel PrepareAddCrossSellProductListModel(AddCrossSellProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged associated product list model
        /// </summary>
        /// <param name="searchModel">Associated product search model</param>
        /// <param name="product">Product</param>
        /// <returns>Associated product list model</returns>
        AssociatedProductListModel PrepareAssociatedProductListModel(AssociatedProductSearchModel searchModel, Product product);

        /// <summary>
        /// Prepare associated product search model to add to the product
        /// </summary>
        /// <param name="searchModel">Associated product search model to add to the product</param>
        /// <returns>Associated product search model to add to the product</returns>
        AddAssociatedProductSearchModel PrepareAddAssociatedProductSearchModel(AddAssociatedProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged associated product list model to add to the product
        /// </summary>
        /// <param name="searchModel">Associated product search model to add to the product</param>
        /// <returns>Associated product list model to add to the product</returns>
        AddAssociatedProductListModel PrepareAddAssociatedProductListModel(AddAssociatedProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged product picture list model
        /// </summary>
        /// <param name="searchModel">Product picture search model</param>
        /// <param name="product">Product</param>
        /// <returns>Product picture list model</returns>
        ProductPictureListModel PrepareProductPictureListModel(ProductPictureSearchModel searchModel, Product product);

        /// <summary>
        /// Prepare paged product specification attribute list model
        /// </summary>
        /// <param name="searchModel">Product specification attribute search model</param>
        /// <param name="product">Product</param>
        /// <returns>Product specification attribute list model</returns>
        ProductSpecificationAttributeListModel PrepareProductSpecificationAttributeListModel(
            ProductSpecificationAttributeSearchModel searchModel, Product product);

        /// <summary>
        /// Prepare product tag search model
        /// </summary>
        /// <param name="searchModel">Product tag search model</param>
        /// <returns>Product tag search model</returns>
        ProductTagSearchModel PrepareProductTagSearchModel(ProductTagSearchModel searchModel);

        /// <summary>
        /// Prepare paged product tag list model
        /// </summary>
        /// <param name="searchModel">Product tag search model</param>
        /// <returns>Product tag list model</returns>
        ProductTagListModel PrepareProductTagListModel(ProductTagSearchModel searchModel);

        /// <summary>
        /// Prepare product tag model
        /// </summary>
        /// <param name="model">Product tag model</param>
        /// <param name="productTag">Product tag</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Product tag model</returns>
        ProductTagModel PrepareProductTagModel(ProductTagModel model, ProductTag productTag, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged product order list model
        /// </summary>
        /// <param name="searchModel">Product order search model</param>
        /// <param name="product">Product</param>
        /// <returns>Product order list model</returns>
        ProductOrderListModel PrepareProductOrderListModel(ProductOrderSearchModel searchModel, Product product);
        
        /// <summary>
        /// Prepare bulk edit product search model
        /// </summary>
        /// <param name="searchModel">Bulk edit product search model</param>
        /// <returns>Bulk edit product search model</returns>
        BulkEditProductSearchModel PrepareBulkEditProductSearchModel(BulkEditProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged bulk edit product list model
        /// </summary>
        /// <param name="searchModel">Bulk edit product search model</param>
        /// <returns>Bulk edit product list model</returns>
        BulkEditProductListModel PrepareBulkEditProductListModel(BulkEditProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged tier price list model
        /// </summary>
        /// <param name="searchModel">Tier price search model</param>
        /// <param name="product">Product</param>
        /// <returns>Tier price list model</returns>
        TierPriceListModel PrepareTierPriceListModel(TierPriceSearchModel searchModel, Product product);

        /// <summary>
        /// Prepare tier price model
        /// </summary>
        /// <param name="model">Tier price model</param>
        /// <param name="product">Product</param>
        /// <param name="tierPrice">Tier price</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Tier price model</returns>
        TierPriceModel PrepareTierPriceModel(TierPriceModel model,
            Product product, TierPrice tierPrice, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged stock quantity history list model
        /// </summary>
        /// <param name="searchModel">Stock quantity history search model</param>
        /// <param name="product">Product</param>
        /// <returns>Stock quantity history list model</returns>
        StockQuantityHistoryListModel PrepareStockQuantityHistoryListModel(StockQuantityHistorySearchModel searchModel, Product product);

        /// <summary>
        /// Prepare paged product attribute mapping list model
        /// </summary>
        /// <param name="searchModel">Product attribute mapping search model</param>
        /// <param name="product">Product</param>
        /// <returns>Product attribute mapping list model</returns>
        ProductAttributeMappingListModel PrepareProductAttributeMappingListModel(ProductAttributeMappingSearchModel searchModel,
            Product product);

        /// <summary>
        /// Prepare product attribute mapping model
        /// </summary>
        /// <param name="model">Product attribute mapping model</param>
        /// <param name="product">Product</param>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Product attribute mapping model</returns>
        ProductAttributeMappingModel PrepareProductAttributeMappingModel(ProductAttributeMappingModel model,
            Product product, ProductAttributeMapping productAttributeMapping, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged product attribute value list model
        /// </summary>
        /// <param name="searchModel">Product attribute value search model</param>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        /// <returns>Product attribute value list model</returns>
        ProductAttributeValueListModel PrepareProductAttributeValueListModel(ProductAttributeValueSearchModel searchModel,
            ProductAttributeMapping productAttributeMapping);

        /// <summary>
        /// Prepare product attribute value model
        /// </summary>
        /// <param name="model">Product attribute value model</param>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        /// <param name="productAttributeValue">Product attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Product attribute value model</returns>
        ProductAttributeValueModel PrepareProductAttributeValueModel(ProductAttributeValueModel model,
            ProductAttributeMapping productAttributeMapping, ProductAttributeValue productAttributeValue, bool excludeProperties = false);

        /// <summary>
        /// Prepare product model to associate to the product attribute value
        /// </summary>
        /// <param name="searchModel">Product model to associate to the product attribute value</param>
        /// <returns>Product model to associate to the product attribute value</returns>
        AssociateProductToAttributeValueSearchModel PrepareAssociateProductToAttributeValueSearchModel(
            AssociateProductToAttributeValueSearchModel searchModel);

        /// <summary>
        /// Prepare paged product model to associate to the product attribute value
        /// </summary>
        /// <param name="searchModel">Product model to associate to the product attribute value</param>
        /// <returns>Product model to associate to the product attribute value</returns>
        AssociateProductToAttributeValueListModel PrepareAssociateProductToAttributeValueListModel(
            AssociateProductToAttributeValueSearchModel searchModel);

        /// <summary>
        /// Prepare paged product attribute combination list model
        /// </summary>
        /// <param name="searchModel">Product attribute combination search model</param>
        /// <param name="product">Product</param>
        /// <returns>Product attribute combination list model</returns>
        ProductAttributeCombinationListModel PrepareProductAttributeCombinationListModel(
            ProductAttributeCombinationSearchModel searchModel, Product product);

        /// <summary>
        /// Prepare product attribute combination model
        /// </summary>
        /// <param name="model">Product attribute combination model</param>
        /// <param name="product">Product</param>
        /// <param name="productAttributeCombination">Product attribute combination</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Product attribute combination model</returns>
        ProductAttributeCombinationModel PrepareProductAttributeCombinationModel(ProductAttributeCombinationModel model,
            Product product, ProductAttributeCombination productAttributeCombination, bool excludeProperties = false);
    }
}