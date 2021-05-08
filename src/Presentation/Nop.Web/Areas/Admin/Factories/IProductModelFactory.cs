using System.Threading.Tasks;
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product search model
        /// </returns>
        Task<ProductSearchModel> PrepareProductSearchModelAsync(ProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged product list model
        /// </summary>
        /// <param name="searchModel">Product search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product list model
        /// </returns>
        Task<ProductListModel> PrepareProductListModelAsync(ProductSearchModel searchModel);

        /// <summary>
        /// Prepare product model
        /// </summary>
        /// <param name="model">Product model</param>
        /// <param name="product">Product</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product model
        /// </returns>
        Task<ProductModel> PrepareProductModelAsync(ProductModel model, Product product, bool excludeProperties = false);

        /// <summary>
        /// Prepare required product search model to add to the product
        /// </summary>
        /// <param name="searchModel">Required product search model to add to the product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the required product search model to add to the product
        /// </returns>
        Task<AddRequiredProductSearchModel> PrepareAddRequiredProductSearchModelAsync(AddRequiredProductSearchModel searchModel);

        /// <summary>
        /// Prepare required product list model to add to the product
        /// </summary>
        /// <param name="searchModel">Required product search model to add to the product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the required product list model to add to the product
        /// </returns>
        Task<AddRequiredProductListModel> PrepareAddRequiredProductListModelAsync(AddRequiredProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged related product list model
        /// </summary>
        /// <param name="searchModel">Related product search model</param>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the related product list model
        /// </returns>
        Task<RelatedProductListModel> PrepareRelatedProductListModelAsync(RelatedProductSearchModel searchModel, Product product);

        /// <summary>
        /// Prepare related product search model to add to the product
        /// </summary>
        /// <param name="searchModel">Related product search model to add to the product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the related product search model to add to the product
        /// </returns>
        Task<AddRelatedProductSearchModel> PrepareAddRelatedProductSearchModelAsync(AddRelatedProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged related product list model to add to the product
        /// </summary>
        /// <param name="searchModel">Related product search model to add to the product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the related product list model to add to the product
        /// </returns>
        Task<AddRelatedProductListModel> PrepareAddRelatedProductListModelAsync(AddRelatedProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged cross-sell product list model
        /// </summary>
        /// <param name="searchModel">Cross-sell product search model</param>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the cross-sell product list model
        /// </returns>
        Task<CrossSellProductListModel> PrepareCrossSellProductListModelAsync(CrossSellProductSearchModel searchModel, Product product);

        /// <summary>
        /// Prepare cross-sell product search model to add to the product
        /// </summary>
        /// <param name="searchModel">Cross-sell product search model to add to the product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the cross-sell product search model to add to the product
        /// </returns>
        Task<AddCrossSellProductSearchModel> PrepareAddCrossSellProductSearchModelAsync(AddCrossSellProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged cross-sell product list model to add to the product
        /// </summary>
        /// <param name="searchModel">Cross-sell product search model to add to the product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the cross-sell product list model to add to the product
        /// </returns>
        Task<AddCrossSellProductListModel> PrepareAddCrossSellProductListModelAsync(AddCrossSellProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged associated product list model
        /// </summary>
        /// <param name="searchModel">Associated product search model</param>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the associated product list model
        /// </returns>
        Task<AssociatedProductListModel> PrepareAssociatedProductListModelAsync(AssociatedProductSearchModel searchModel, Product product);

        /// <summary>
        /// Prepare associated product search model to add to the product
        /// </summary>
        /// <param name="searchModel">Associated product search model to add to the product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the associated product search model to add to the product
        /// </returns>
        Task<AddAssociatedProductSearchModel> PrepareAddAssociatedProductSearchModelAsync(AddAssociatedProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged associated product list model to add to the product
        /// </summary>
        /// <param name="searchModel">Associated product search model to add to the product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the associated product list model to add to the product
        /// </returns>
        Task<AddAssociatedProductListModel> PrepareAddAssociatedProductListModelAsync(AddAssociatedProductSearchModel searchModel);

        /// <summary>
        /// Prepare paged product picture list model
        /// </summary>
        /// <param name="searchModel">Product picture search model</param>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product picture list model
        /// </returns>
        Task<ProductPictureListModel> PrepareProductPictureListModelAsync(ProductPictureSearchModel searchModel, Product product);

        /// <summary>
        /// Prepare paged product specification attribute list model
        /// </summary>
        /// <param name="searchModel">Product specification attribute search model</param>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product specification attribute list model
        /// </returns>
        Task<ProductSpecificationAttributeListModel> PrepareProductSpecificationAttributeListModelAsync(
            ProductSpecificationAttributeSearchModel searchModel, Product product);

        /// <summary>
        /// Prepare paged product specification attribute model
        /// </summary>
        /// <param name="productId">Product id</param>
        /// <param name="specificationId">Specification attribute id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product specification attribute model
        /// </returns>
        Task<AddSpecificationAttributeModel> PrepareAddSpecificationAttributeModelAsync(int productId, int? specificationId);

        /// <summary>
        /// Prepare product tag search model
        /// </summary>
        /// <param name="searchModel">Product tag search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tag search model
        /// </returns>
        Task<ProductTagSearchModel> PrepareProductTagSearchModelAsync(ProductTagSearchModel searchModel);

        /// <summary>
        /// Prepare paged product tag list model
        /// </summary>
        /// <param name="searchModel">Product tag search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tag list model
        /// </returns>
        Task<ProductTagListModel> PrepareProductTagListModelAsync(ProductTagSearchModel searchModel);

        /// <summary>
        /// Prepare product tag model
        /// </summary>
        /// <param name="model">Product tag model</param>
        /// <param name="productTag">Product tag</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tag model
        /// </returns>
        Task<ProductTagModel> PrepareProductTagModelAsync(ProductTagModel model, ProductTag productTag, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged product order list model
        /// </summary>
        /// <param name="searchModel">Product order search model</param>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product order list model
        /// </returns>
        Task<ProductOrderListModel> PrepareProductOrderListModelAsync(ProductOrderSearchModel searchModel, Product product);

        /// <summary>
        /// Prepare paged tier price list model
        /// </summary>
        /// <param name="searchModel">Tier price search model</param>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ier price list model
        /// </returns>
        Task<TierPriceListModel> PrepareTierPriceListModelAsync(TierPriceSearchModel searchModel, Product product);

        /// <summary>
        /// Prepare tier price model
        /// </summary>
        /// <param name="model">Tier price model</param>
        /// <param name="product">Product</param>
        /// <param name="tierPrice">Tier price</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ier price model
        /// </returns>
        Task<TierPriceModel> PrepareTierPriceModelAsync(TierPriceModel model,
            Product product, TierPrice tierPrice, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged stock quantity history list model
        /// </summary>
        /// <param name="searchModel">Stock quantity history search model</param>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the stock quantity history list model
        /// </returns>
        Task<StockQuantityHistoryListModel> PrepareStockQuantityHistoryListModelAsync(StockQuantityHistorySearchModel searchModel, Product product);

        /// <summary>
        /// Prepare paged product attribute mapping list model
        /// </summary>
        /// <param name="searchModel">Product attribute mapping search model</param>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute mapping list model
        /// </returns>
        Task<ProductAttributeMappingListModel> PrepareProductAttributeMappingListModelAsync(ProductAttributeMappingSearchModel searchModel,
            Product product);

        /// <summary>
        /// Prepare product attribute mapping model
        /// </summary>
        /// <param name="model">Product attribute mapping model</param>
        /// <param name="product">Product</param>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute mapping model
        /// </returns>
        Task<ProductAttributeMappingModel> PrepareProductAttributeMappingModelAsync(ProductAttributeMappingModel model,
            Product product, ProductAttributeMapping productAttributeMapping, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged product attribute value list model
        /// </summary>
        /// <param name="searchModel">Product attribute value search model</param>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute value list model
        /// </returns>
        Task<ProductAttributeValueListModel> PrepareProductAttributeValueListModelAsync(ProductAttributeValueSearchModel searchModel,
            ProductAttributeMapping productAttributeMapping);

        /// <summary>
        /// Prepare product attribute value model
        /// </summary>
        /// <param name="model">Product attribute value model</param>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        /// <param name="productAttributeValue">Product attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute value model
        /// </returns>
        Task<ProductAttributeValueModel> PrepareProductAttributeValueModelAsync(ProductAttributeValueModel model,
            ProductAttributeMapping productAttributeMapping, ProductAttributeValue productAttributeValue, bool excludeProperties = false);

        /// <summary>
        /// Prepare product model to associate to the product attribute value
        /// </summary>
        /// <param name="searchModel">Product model to associate to the product attribute value</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product model to associate to the product attribute value
        /// </returns>
        Task<AssociateProductToAttributeValueSearchModel> PrepareAssociateProductToAttributeValueSearchModelAsync(
            AssociateProductToAttributeValueSearchModel searchModel);

        /// <summary>
        /// Prepare paged product model to associate to the product attribute value
        /// </summary>
        /// <param name="searchModel">Product model to associate to the product attribute value</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product model to associate to the product attribute value
        /// </returns>
        Task<AssociateProductToAttributeValueListModel> PrepareAssociateProductToAttributeValueListModelAsync(
            AssociateProductToAttributeValueSearchModel searchModel);

        /// <summary>
        /// Prepare paged product attribute combination list model
        /// </summary>
        /// <param name="searchModel">Product attribute combination search model</param>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute combination list model
        /// </returns>
        Task<ProductAttributeCombinationListModel> PrepareProductAttributeCombinationListModelAsync(
            ProductAttributeCombinationSearchModel searchModel, Product product);

        /// <summary>
        /// Prepare product attribute combination model
        /// </summary>
        /// <param name="model">Product attribute combination model</param>
        /// <param name="product">Product</param>
        /// <param name="productAttributeCombination">Product attribute combination</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute combination model
        /// </returns>
        Task<ProductAttributeCombinationModel> PrepareProductAttributeCombinationModelAsync(ProductAttributeCombinationModel model,
            Product product, ProductAttributeCombination productAttributeCombination, bool excludeProperties = false);
    }
}