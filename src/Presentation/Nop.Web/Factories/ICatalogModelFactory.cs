using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Factories
{
    public partial interface ICatalogModelFactory
    {
        #region Products

        ProductReviewOverviewModel PrepareProductReviewOverviewModel(Product product);

        IEnumerable<ProductOverviewModel> PrepareProductOverviewModels(IEnumerable<Product> products,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false);

        IList<ProductSpecificationModel> PrepareProductSpecificationModel(Product product);

        #endregion

        #region Categories

        CategoryModel PrepareCategoryModel(Category category, CatalogPagingFilteringModel command);

        string PrepareCategoryTemplateViewPath(int templateId);

        CategoryNavigationModel PrepareCategoryNavigationModel(int currentCategoryId,
            int currentProductId);

        TopMenuModel PrepareTopMenuModel();

        List<CategoryModel> PrepareHomepageCategoryModels();

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
