using System.Collections.Generic;
using System.Xml.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Factories
{
    public partial interface ICatalogModelFactory
    {
        #region Common

        /// <summary>
        /// Prepare sorting options
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        void PrepareSortingOptions(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command);

        /// <summary>
        /// Prepare view modes
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        void PrepareViewModes(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command);

        /// <summary>
        /// Prepare page size options
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <param name="allowCustomersToSelectPageSize">Are customers allowed to select page size?</param>
        /// <param name="pageSizeOptions">Page size options</param>
        /// <param name="fixedPageSize">Fixed page size</param>
        void PreparePageSizeOptions(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command,
            bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize);

        #endregion

        #region Categories

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Category model</returns>
        CategoryModel PrepareCategoryModel(Category category, CatalogPagingFilteringModel command);

        /// <summary>
        /// Prepare category template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Category template view path</returns>
        string PrepareCategoryTemplateViewPath(int templateId);

        /// <summary>
        /// Prepare category navigation model
        /// </summary>
        /// <param name="currentCategoryId">Current category identifier</param>
        /// <param name="currentProductId">Current product identifier</param>
        /// <returns>Category navigation model</returns>
        CategoryNavigationModel PrepareCategoryNavigationModel(int currentCategoryId,
            int currentProductId);

        /// <summary>
        /// Prepare top menu model
        /// </summary>
        /// <returns>Top menu model</returns>
        TopMenuModel PrepareTopMenuModel();

        /// <summary>
        /// Prepare homepage category models
        /// </summary>
        /// <returns>List of homepage category models</returns>
        List<CategoryModel> PrepareHomepageCategoryModels();

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <returns>List of category (simple) models</returns>
        List<CategorySimpleModel> PrepareCategorySimpleModels();

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <returns>List of category (simple) models</returns>
        List<CategorySimpleModel> PrepareCategorySimpleModels(int rootCategoryId, bool loadSubCategories = true);

        /// <summary>
        /// Prepare category (simple) xml document
        /// </summary>
        /// <returns>Xml document of category (simple) models</returns>
        XDocument PrepareCategoryXmlDocument();

        /// <summary>
        /// Prepare root categories for menu
        /// </summary>
        /// <returns>List of category (simple) models</returns>
        List<CategorySimpleModel> PrepareRootCategories();

        /// <summary>
        /// Prepare subcategories for menu
        /// </summary>
        /// <param name="id">Id of category to get subcategory</param>
        /// <returns></returns>
        List<CategorySimpleModel> PrepareSubCategories(int id);

        #endregion

        #region Manufacturers

        /// <summary>
        /// Prepare manufacturer model
        /// </summary>
        /// <param name="manufacturer">Manufacturer identifier</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Manufacturer model</returns>
        ManufacturerModel PrepareManufacturerModel(Manufacturer manufacturer, CatalogPagingFilteringModel command);

        /// <summary>
        /// Prepare manufacturer template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Manufacturer template view path</returns>
        string PrepareManufacturerTemplateViewPath(int templateId);

        /// <summary>
        /// Prepare manufacturer all models
        /// </summary>
        /// <returns>List of manufacturer models</returns>
        List<ManufacturerModel> PrepareManufacturerAllModels();

        /// <summary>
        /// Prepare manufacturer navigation model
        /// </summary>
        /// <param name="currentManufacturerId">Current manufacturer identifier</param>
        /// <returns>Manufacturer navigation model</returns>
        ManufacturerNavigationModel PrepareManufacturerNavigationModel(int currentManufacturerId);

        #endregion

        #region Vendors

        /// <summary>
        /// Prepare vendor model
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Vendor model</returns>
        VendorModel PrepareVendorModel(Vendor vendor, CatalogPagingFilteringModel command);

        /// <summary>
        /// Prepare vendor all models
        /// </summary>
        /// <returns>List of vendor models</returns>
        List<VendorModel> PrepareVendorAllModels();

        /// <summary>
        /// Prepare vendor navigation model
        /// </summary>
        /// <returns>Vendor navigation model</returns>
        VendorNavigationModel PrepareVendorNavigationModel();

        #endregion

        #region Product tags

        /// <summary>
        /// Prepare popular product tags model
        /// </summary>
        /// <returns>Product tags model</returns>
        PopularProductTagsModel PreparePopularProductTagsModel();

        /// <summary>
        /// Prepare products by tag model
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Products by tag model</returns>
        ProductsByTagModel PrepareProductsByTagModel(ProductTag productTag,
            CatalogPagingFilteringModel command);

        /// <summary>
        /// Prepare product tags all model
        /// </summary>
        /// <returns>Popular product tags model</returns>
        PopularProductTagsModel PrepareProductTagsAllModel();

        #endregion

        #region Searching

        /// <summary>
        /// Prepare search model
        /// </summary>
        /// <param name="model">Search model</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Search model</returns>
        SearchModel PrepareSearchModel(SearchModel model, CatalogPagingFilteringModel command);

        /// <summary>
        /// Prepare search box model
        /// </summary>
        /// <returns>Search box model</returns>
        SearchBoxModel PrepareSearchBoxModel();

        #endregion
    }
}