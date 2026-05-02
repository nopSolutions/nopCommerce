using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Factories;

public partial interface ICatalogModelFactory
{
    #region Categories

    /// <summary>
    /// Prepare category model
    /// </summary>
    /// <param name="category">Category</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category model
    /// </returns>
    Task<CategoryModel> PrepareCategoryModelAsync(Category category, CatalogProductsCommand command);

    /// <summary>
    /// Prepare category template view path
    /// </summary>
    /// <param name="templateId">Template identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category template view path
    /// </returns>
    Task<string> PrepareCategoryTemplateViewPathAsync(int templateId);

    /// <summary>
    /// Prepare category navigation model
    /// </summary>
    /// <param name="currentCategoryId">Current category identifier</param>
    /// <param name="currentProductId">Current product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category navigation model
    /// </returns>
    Task<CategoryNavigationModel> PrepareCategoryNavigationModelAsync(int currentCategoryId,
        int currentProductId);

    /// <summary>
    /// Prepare homepage category models
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of homepage category models
    /// </returns>
    Task<List<CategoryModel>> PrepareHomepageCategoryModelsAsync();

    /// <summary>
    /// Prepares the category products model
    /// </summary>
    /// <param name="category">Category</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category products model
    /// </returns>
    Task<CatalogProductsModel> PrepareCategoryProductsModelAsync(Category category, CatalogProductsCommand command);
    
    #endregion

    #region Manufacturers

    /// <summary>
    /// Prepare manufacturer model
    /// </summary>
    /// <param name="manufacturer">Manufacturer identifier</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer model
    /// </returns>
    Task<ManufacturerModel> PrepareManufacturerModelAsync(Manufacturer manufacturer, CatalogProductsCommand command);

    /// <summary>
    /// Prepares the manufacturer products model
    /// </summary>
    /// <param name="manufacturer">Manufacturer</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer products model
    /// </returns>
    Task<CatalogProductsModel> PrepareManufacturerProductsModelAsync(Manufacturer manufacturer, CatalogProductsCommand command);

    /// <summary>
    /// Prepare manufacturer template view path
    /// </summary>
    /// <param name="templateId">Template identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer template view path
    /// </returns>
    Task<string> PrepareManufacturerTemplateViewPathAsync(int templateId);

    /// <summary>
    /// Prepare manufacturer all models
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of manufacturer models
    /// </returns>
    Task<List<ManufacturerModel>> PrepareManufacturerAllModelsAsync();

    /// <summary>
    /// Prepare manufacturer navigation model
    /// </summary>
    /// <param name="currentManufacturerId">Current manufacturer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer navigation model
    /// </returns>
    Task<ManufacturerNavigationModel> PrepareManufacturerNavigationModelAsync(int currentManufacturerId);

    #endregion

    #region Vendors

    /// <summary>
    /// Prepare vendor model
    /// </summary>
    /// <param name="vendor">Vendor</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor model
    /// </returns>
    Task<VendorModel> PrepareVendorModelAsync(Vendor vendor, CatalogProductsCommand command);

    /// <summary>
    /// Prepares the vendor products model
    /// </summary>
    /// <param name="vendor">Vendor</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor products model
    /// </returns>
    Task<CatalogProductsModel> PrepareVendorProductsModelAsync(Vendor vendor, CatalogProductsCommand command);

    /// <summary>
    /// Prepare vendor all models
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of vendor models
    /// </returns>
    Task<List<VendorModel>> PrepareVendorAllModelsAsync();

    /// <summary>
    /// Prepare vendor navigation model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor navigation model
    /// </returns>
    Task<VendorNavigationModel> PrepareVendorNavigationModelAsync();

    /// <summary>
    /// Prepare review models for vendor products
    /// </summary>
    /// <returns>
    /// <param name="vendor">Vendor</param>
    /// <param name="pagingModel">Model to filter product reviews</param>
    /// A task that represents the asynchronous operation
    /// The task result contains a list of product reviews
    /// </returns>
    Task<VendorProductReviewsListModel> PrepareVendorProductReviewsModelAsync(Vendor vendor, VendorReviewsPagingFilteringModel pagingModel);

    #endregion

    #region Product tags

    /// <summary>
    /// Prepare popular product tags model
    /// </summary>
    /// <param name="numberTagsToReturn">The number of tags to be returned; pass 0 to get all tags</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product tags model
    /// </returns>
    Task<PopularProductTagsModel> PreparePopularProductTagsModelAsync(int numberTagsToReturn = 0);

    /// <summary>
    /// Prepare products by tag model
    /// </summary>
    /// <param name="productTag">Product tag</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the products by tag model
    /// </returns>
    Task<ProductsByTagModel> PrepareProductsByTagModelAsync(ProductTag productTag, CatalogProductsCommand command);

    /// <summary>
    /// Prepares the tag products model
    /// </summary>
    /// <param name="productTag">Product tag</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ag products model
    /// </returns>
    Task<CatalogProductsModel> PrepareTagProductsModelAsync(ProductTag productTag, CatalogProductsCommand command);

    #endregion

    #region New products

    /// <summary>
    /// Prepare new products model
    /// </summary>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the new products model
    /// </returns>
    Task<CatalogProductsModel> PrepareNewProductsModelAsync(CatalogProductsCommand command);

    #endregion

    #region Searching

    /// <summary>
    /// Prepare search model
    /// </summary>
    /// <param name="model">Search model</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search model
    /// </returns>
    Task<SearchModel> PrepareSearchModelAsync(SearchModel model, CatalogProductsCommand command);

    /// <summary>
    /// Prepares the search products model
    /// </summary>
    /// <param name="model">Search model</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search products model
    /// </returns>
    Task<CatalogProductsModel> PrepareSearchProductsModelAsync(SearchModel searchModel, CatalogProductsCommand command);

    /// <summary>
    /// Prepares the search products by filter level values model
    /// </summary>
    /// <param name="searchModel">Search filter level values model</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search products model
    /// </returns>
    Task<CatalogProductsModel> PrepareSearchProductsByFilterLevelValuesModelAsync(SearchFilterLevelValueModel searchModel, CatalogProductsCommand command);

    /// <summary>
    /// Prepare search box model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search box model
    /// </returns>
    Task<SearchBoxModel> PrepareSearchBoxModelAsync();

    #endregion

    #region Common

    /// <summary>
    /// Prepare sorting options
    /// </summary>
    /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
    /// <param name="command">Catalog paging filtering command</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PrepareSortingOptionsAsync(CatalogProductsModel pagingFilteringModel, CatalogProductsCommand command);

    /// <summary>
    /// Prepare view modes
    /// </summary>
    /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
    /// <param name="command">Catalog paging filtering command</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PrepareViewModesAsync(CatalogProductsModel pagingFilteringModel, CatalogProductsCommand command);

    /// <summary>
    /// Prepare page size options
    /// </summary>
    /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
    /// <param name="command">Catalog paging filtering command</param>
    /// <param name="allowCustomersToSelectPageSize">Are customers allowed to select page size?</param>
    /// <param name="pageSizeOptions">Page size options</param>
    /// <param name="fixedPageSize">Fixed page size</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PreparePageSizeOptionsAsync(CatalogProductsModel pagingFilteringModel, CatalogProductsCommand command,
        bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize);

    #endregion
}