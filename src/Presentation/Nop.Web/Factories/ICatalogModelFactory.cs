using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Factories
{
    public partial interface ICatalogModelFactory
    {
        #region Categories

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>Category model</returns>
        Task<CategoryModel> PrepareCategoryModelAsync(Category category, CatalogProductsCommand command);

        /// <summary>
        /// Prepare category template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Category template view path</returns>
        Task<string> PrepareCategoryTemplateViewPathAsync(int templateId);

        /// <summary>
        /// Prepare category navigation model
        /// </summary>
        /// <param name="currentCategoryId">Current category identifier</param>
        /// <param name="currentProductId">Current product identifier</param>
        /// <returns>Category navigation model</returns>
        Task<CategoryNavigationModel> PrepareCategoryNavigationModelAsync(int currentCategoryId,
            int currentProductId);

        /// <summary>
        /// Prepare top menu model
        /// </summary>
        /// <returns>Top menu model</returns>
        Task<TopMenuModel> PrepareTopMenuModelAsync();

        /// <summary>
        /// Prepare homepage category models
        /// </summary>
        /// <returns>List of homepage category models</returns>
        Task<List<CategoryModel>> PrepareHomepageCategoryModelsAsync();

        /// <summary>
        /// Prepare root categories for menu
        /// </summary>
        /// <returns>List of category (simple) models</returns>
        Task<List<CategorySimpleModel>> PrepareRootCategoriesAsync();

        /// <summary>
        /// Prepare subcategories for menu
        /// </summary>
        /// <param name="id">Id of category to get subcategory</param>
        /// <returns></returns>
        Task<List<CategorySimpleModel>> PrepareSubCategoriesAsync(int id);

        /// <summary>
        /// Prepares the category products model
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>The category products model</returns>
        Task<CatalogProductsModel> PrepareCategoryProductsModelAsync(Category category, CatalogProductsCommand command);

        #endregion

        #region Manufacturers

        /// <summary>
        /// Prepare manufacturer model
        /// </summary>
        /// <param name="manufacturer">Manufacturer identifier</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>Manufacturer model</returns>
        Task<ManufacturerModel> PrepareManufacturerModelAsync(Manufacturer manufacturer, CatalogProductsCommand command);

        /// <summary>
        /// Prepares the manufacturer products model
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>The manufacturer products model</returns>
        Task<CatalogProductsModel> PrepareManufacturerProductsModelAsync(Manufacturer manufacturer, CatalogProductsCommand command);

        /// <summary>
        /// Prepare manufacturer template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Manufacturer template view path</returns>
        Task<string> PrepareManufacturerTemplateViewPathAsync(int templateId);

        /// <summary>
        /// Prepare manufacturer all models
        /// </summary>
        /// <returns>List of manufacturer models</returns>
        Task<List<ManufacturerModel>> PrepareManufacturerAllModelsAsync();

        /// <summary>
        /// Prepare manufacturer navigation model
        /// </summary>
        /// <param name="currentManufacturerId">Current manufacturer identifier</param>
        /// <returns>Manufacturer navigation model</returns>
        Task<ManufacturerNavigationModel> PrepareManufacturerNavigationModelAsync(int currentManufacturerId);

        #endregion

        #region Vendors

        /// <summary>
        /// Prepare vendor model
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>Vendor model</returns>
        Task<VendorModel> PrepareVendorModelAsync(Vendor vendor, CatalogProductsCommand command);

        /// <summary>
        /// Prepares the vendor products model
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>The vendor products model</returns>
        Task<CatalogProductsModel> PrepareVendorProductsModelAsync(Vendor vendor, CatalogProductsCommand command);

        /// <summary>
        /// Prepare vendor all models
        /// </summary>
        /// <returns>List of vendor models</returns>
        Task<List<VendorModel>> PrepareVendorAllModelsAsync();

        /// <summary>
        /// Prepare vendor navigation model
        /// </summary>
        /// <returns>Vendor navigation model</returns>
        Task<VendorNavigationModel> PrepareVendorNavigationModelAsync();

        #endregion

        #region Product tags

        /// <summary>
        /// Prepare popular product tags model
        /// </summary>
        /// <param name="numberTagsToReturn">The number of tags to be returned; pass 0 to get all tags</param>
        /// <returns>Product tags model</returns>
        Task<PopularProductTagsModel> PreparePopularProductTagsModelAsync(int numberTagsToReturn = 0);

        /// <summary>
        /// Prepare products by tag model
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>Products by tag model</returns>
        Task<ProductsByTagModel> PrepareProductsByTagModelAsync(ProductTag productTag, CatalogProductsCommand command);

        /// <summary>
        /// Prepares the tag products model
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>The tag products model</returns>
        Task<CatalogProductsModel> PrepareTagProductsModelAsync(ProductTag productTag, CatalogProductsCommand command);        

        #endregion

        #region Searching

        /// <summary>
        /// Prepare search model
        /// </summary>
        /// <param name="model">Search model</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>Search model</returns>
        Task<SearchModel> PrepareSearchModelAsync(SearchModel model, CatalogProductsCommand command);

        /// <summary>
        /// Prepares the search products model
        /// </summary>
        /// <param name="model">Search model</param>
        /// <param name="command">Model to get the catalog products</param>
        /// <returns>The search products model</returns>
        Task<CatalogProductsModel> PrepareSearchProductsModelAsync(SearchModel searchModel, CatalogProductsCommand command);

        /// <summary>
        /// Prepare search box model
        /// </summary>
        /// <returns>Search box model</returns>
        Task<SearchBoxModel> PrepareSearchBoxModelAsync();

        #endregion
    }
}