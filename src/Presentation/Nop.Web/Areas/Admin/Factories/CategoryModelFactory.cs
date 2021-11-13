using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the category model factory implementation
    /// </summary>
    public partial class CategoryModelFactory : ICategoryModelFactory
    {
        #region Fields

        protected CatalogSettings CatalogSettings { get; }
        protected CurrencySettings CurrencySettings { get; }
        protected ICurrencyService CurrencyService { get; }
        protected IAclSupportedModelFactory AclSupportedModelFactory { get; }
        protected IBaseAdminModelFactory BaseAdminModelFactory { get; }
        protected ICategoryService CategoryService { get; }
        protected IDiscountService DiscountService { get; }
        protected IDiscountSupportedModelFactory DiscountSupportedModelFactory { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedModelFactory LocalizedModelFactory { get; }
        protected IProductService ProductService { get; }
        protected IStoreMappingSupportedModelFactory StoreMappingSupportedModelFactory { get; }
        protected IUrlRecordService UrlRecordService { get; }

        #endregion

        #region Ctor

        public CategoryModelFactory(CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            ICurrencyService currencyService,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICategoryService categoryService,
            IDiscountService discountService,
            IDiscountSupportedModelFactory discountSupportedModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IProductService productService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IUrlRecordService urlRecordService)
        {
            CatalogSettings = catalogSettings;
            CurrencySettings = currencySettings;
            CurrencyService = currencyService;
            AclSupportedModelFactory = aclSupportedModelFactory;
            BaseAdminModelFactory = baseAdminModelFactory;
            CategoryService = categoryService;
            DiscountService = discountService;
            DiscountSupportedModelFactory = discountSupportedModelFactory;
            LocalizationService = localizationService;
            LocalizedModelFactory = localizedModelFactory;
            ProductService = productService;
            StoreMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            UrlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare category product search model
        /// </summary>
        /// <param name="searchModel">Category product search model</param>
        /// <param name="category">Category</param>
        /// <returns>Category product search model</returns>
        protected virtual CategoryProductSearchModel PrepareCategoryProductSearchModel(CategoryProductSearchModel searchModel, Category category)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (category == null)
                throw new ArgumentNullException(nameof(category));

            searchModel.CategoryId = category.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare category search model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category search model
        /// </returns>
        public virtual async Task<CategorySearchModel> PrepareCategorySearchModelAsync(CategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            await BaseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            searchModel.HideStoresList = CatalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = await LocalizationService.GetResourceAsync("Admin.Catalog.Categories.List.SearchPublished.All")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = await LocalizationService.GetResourceAsync("Admin.Catalog.Categories.List.SearchPublished.PublishedOnly")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = await LocalizationService.GetResourceAsync("Admin.Catalog.Categories.List.SearchPublished.UnpublishedOnly")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged category list model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category list model
        /// </returns>
        public virtual async Task<CategoryListModel> PrepareCategoryListModelAsync(CategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            //get categories
            var categories = await CategoryService.GetAllCategoriesAsync(categoryName: searchModel.SearchCategoryName,
                showHidden: true,
                storeId: searchModel.SearchStoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
                overridePublished: searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1));

            //prepare grid model
            var model = await new CategoryListModel().PrepareToGridAsync(searchModel, categories, () =>
            {
                return categories.SelectAwait(async category =>
                {
                    //fill in model values from the entity
                    var categoryModel = category.ToModel<CategoryModel>();

                    //fill in additional values (not existing in the entity)
                    categoryModel.Breadcrumb = await CategoryService.GetFormattedBreadCrumbAsync(category);
                    categoryModel.SeName = await UrlRecordService.GetSeNameAsync(category, 0, true, false);

                    return categoryModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="model">Category model</param>
        /// <param name="category">Category</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category model
        /// </returns>
        public virtual async Task<CategoryModel> PrepareCategoryModelAsync(CategoryModel model, Category category, bool excludeProperties = false)
        {
            Func<CategoryLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (category != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = category.ToModel<CategoryModel>();
                    model.SeName = await UrlRecordService.GetSeNameAsync(category, 0, true, false);
                }

                //prepare nested search model
                PrepareCategoryProductSearchModel(model.CategoryProductSearchModel, category);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await LocalizationService.GetLocalizedAsync(category, entity => entity.Name, languageId, false, false);
                    locale.Description = await LocalizationService.GetLocalizedAsync(category, entity => entity.Description, languageId, false, false);
                    locale.MetaKeywords = await LocalizationService.GetLocalizedAsync(category, entity => entity.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = await LocalizationService.GetLocalizedAsync(category, entity => entity.MetaDescription, languageId, false, false);
                    locale.MetaTitle = await LocalizationService.GetLocalizedAsync(category, entity => entity.MetaTitle, languageId, false, false);
                    locale.SeName = await UrlRecordService.GetSeNameAsync(category, languageId, false, false);
                };
            }

            //set default values for the new model
            if (category == null)
            {
                model.PageSize = CatalogSettings.DefaultCategoryPageSize;
                model.PageSizeOptions = CatalogSettings.DefaultCategoryPageSizeOptions;
                model.Published = true;
                model.IncludeInTopMenu = true;
                model.AllowCustomersToSelectPageSize = true;
                model.PriceRangeFiltering = true;
                model.ManuallyPriceRange = true;
                model.PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
                model.PriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
            }

            model.PrimaryStoreCurrencyCode = (await CurrencyService.GetCurrencyByIdAsync(CurrencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await LocalizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare available category templates
            await BaseAdminModelFactory.PrepareCategoryTemplatesAsync(model.AvailableCategoryTemplates, false);

            //prepare available parent categories
            await BaseAdminModelFactory.PrepareCategoriesAsync(model.AvailableCategories,
                defaultItemText: await LocalizationService.GetResourceAsync("Admin.Catalog.Categories.Fields.Parent.None"));

            //prepare model discounts
            var availableDiscounts = await DiscountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true);
            await DiscountSupportedModelFactory.PrepareModelDiscountsAsync(model, category, availableDiscounts, excludeProperties);

            //prepare model customer roles
            await AclSupportedModelFactory.PrepareModelCustomerRolesAsync(model, category, excludeProperties);

            //prepare model stores
            await StoreMappingSupportedModelFactory.PrepareModelStoresAsync(model, category, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare paged category product list model
        /// </summary>
        /// <param name="searchModel">Category product search model</param>
        /// <param name="category">Category</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category product list model
        /// </returns>
        public virtual async Task<CategoryProductListModel> PrepareCategoryProductListModelAsync(CategoryProductSearchModel searchModel, Category category)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (category == null)
                throw new ArgumentNullException(nameof(category));

            //get product categories
            var productCategories = await CategoryService.GetProductCategoriesByCategoryIdAsync(category.Id,
                showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new CategoryProductListModel().PrepareToGridAsync(searchModel, productCategories, () =>
            {
                return productCategories.SelectAwait(async productCategory =>
                {
                    //fill in model values from the entity
                    var categoryProductModel = productCategory.ToModel<CategoryProductModel>();

                    //fill in additional values (not existing in the entity)
                    categoryProductModel.ProductName = (await ProductService.GetProductByIdAsync(productCategory.ProductId))?.Name;

                    return categoryProductModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare product search model to add to the category
        /// </summary>
        /// <param name="searchModel">Product search model to add to the category</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product search model to add to the category
        /// </returns>
        public virtual async Task<AddProductToCategorySearchModel> PrepareAddProductToCategorySearchModelAsync(AddProductToCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available categories
            await BaseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await BaseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await BaseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await BaseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available product types
            await BaseAdminModelFactory.PrepareProductTypesAsync(searchModel.AvailableProductTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged product list model to add to the category
        /// </summary>
        /// <param name="searchModel">Product search model to add to the category</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product list model to add to the category
        /// </returns>
        public virtual async Task<AddProductToCategoryListModel> PrepareAddProductToCategoryListModelAsync(AddProductToCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get products
            var products = await ProductService.SearchProductsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddProductToCategoryListModel().PrepareToGridAsync(searchModel, products, () =>
            {
                return products.SelectAwait(async product =>
                {
                    var productModel = product.ToModel<ProductModel>();

                    productModel.SeName = await UrlRecordService.GetSeNameAsync(product, 0, true, false);

                    return productModel;
                });
            });

            return model;
        }

        #endregion
    }
}