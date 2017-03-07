using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Factories
{
    public partial interface ICatalogModelFactory
    {
        #region Common

        void PrepareSortingOptions(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command);

        void PrepareViewModes(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command);

        void PreparePageSizeOptions(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command,
            bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize);

        #endregion

        #region Categories

        CategoryModel PrepareCategoryModel(Category category, CatalogPagingFilteringModel command);

        string PrepareCategoryTemplateViewPath(int templateId);

        CategoryNavigationModel PrepareCategoryNavigationModel(int currentCategoryId,
            int currentProductId);

        TopMenuModel PrepareTopMenuModel();

        List<CategoryModel> PrepareHomepageCategoryModels();

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <returns>Categories</returns>
        List<CategorySimpleModel> PrepareCategorySimpleModels();

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <param name="allCategories">All available categories; pass null to load them internally</param>
        /// <returns>Category models</returns>
        List<CategorySimpleModel> PrepareCategorySimpleModels(int rootCategoryId,
            bool loadSubCategories = true, IList<Category> allCategories = null);

        #endregion

        #region Manufacturers

        ManufacturerModel PrepareManufacturerModel(Manufacturer manufacturer, CatalogPagingFilteringModel command);

        string PrepareManufacturerTemplateViewPath(int templateId);

        List<ManufacturerModel> PrepareManufacturerAllModels();

        ManufacturerNavigationModel PrepareManufacturerNavigationModel(int currentManufacturerId);

        #endregion

        #region Vendors

        VendorModel PrepareVendorModel(Vendor vendor, CatalogPagingFilteringModel command);

        List<VendorModel> PrepareVendorAllModels();

        VendorNavigationModel PrepareVendorNavigationModel();

        #endregion

        #region Product tags

        PopularProductTagsModel PreparePopularProductTagsModel();

        ProductsByTagModel PrepareProductsByTagModel(ProductTag productTag,
            CatalogPagingFilteringModel command);

        PopularProductTagsModel PrepareProductTagsAllModel();

        #endregion

        #region Searching

        SearchModel PrepareSearchModel(SearchModel model, CatalogPagingFilteringModel command);

        SearchBoxModel PrepareSearchBoxModel();

        #endregion
    }
}
