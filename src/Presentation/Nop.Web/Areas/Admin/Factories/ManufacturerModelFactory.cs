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

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the manufacturer model factory implementation
/// </summary>
public partial class ManufacturerModelFactory : IManufacturerModelFactory
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyService _currencyService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IDiscountService _discountService;
    protected readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IProductService _productService;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
    protected readonly IUrlRecordService _urlRecordService;

    #endregion

    #region Ctor

    public ManufacturerModelFactory(CatalogSettings catalogSettings,
        CurrencySettings currencySettings,
        ICurrencyService currencyService,
        IBaseAdminModelFactory baseAdminModelFactory,
        IManufacturerService manufacturerService,
        IDiscountService discountService,
        IDiscountSupportedModelFactory discountSupportedModelFactory,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IProductService productService,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
        IUrlRecordService urlRecordService)
    {
        _catalogSettings = catalogSettings;
        _currencySettings = currencySettings;
        _currencyService = currencyService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _manufacturerService = manufacturerService;
        _discountService = discountService;
        _discountSupportedModelFactory = discountSupportedModelFactory;
        _localizationService = localizationService;
        _localizedModelFactory = localizedModelFactory;
        _productService = productService;
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
        _urlRecordService = urlRecordService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare manufacturer product search model
    /// </summary>
    /// <param name="searchModel">Manufacturer product search model</param>
    /// <param name="manufacturer">Manufacturer</param>
    /// <returns>Manufacturer product search model</returns>
    protected virtual ManufacturerProductSearchModel PrepareManufacturerProductSearchModel(ManufacturerProductSearchModel searchModel,
        Manufacturer manufacturer)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(manufacturer);

        searchModel.ManufacturerId = manufacturer.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare manufacturer search model
    /// </summary>
    /// <param name="searchModel">Manufacturer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer search model
    /// </returns>
    public virtual async Task<ManufacturerSearchModel> PrepareManufacturerSearchModelAsync(ManufacturerSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

        //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
        searchModel.AvailablePublishedOptions.Add(new SelectListItem
        {
            Value = "0",
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.List.SearchPublished.All")
        });
        searchModel.AvailablePublishedOptions.Add(new SelectListItem
        {
            Value = "1",
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.List.SearchPublished.PublishedOnly")
        });
        searchModel.AvailablePublishedOptions.Add(new SelectListItem
        {
            Value = "2",
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.List.SearchPublished.UnpublishedOnly")
        });

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged manufacturer list model
    /// </summary>
    /// <param name="searchModel">Manufacturer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer list model
    /// </returns>
    public virtual async Task<ManufacturerListModel> PrepareManufacturerListModelAsync(ManufacturerSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get manufacturers
        var manufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true,
            manufacturerName: searchModel.SearchManufacturerName,
            storeId: searchModel.SearchStoreId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
            overridePublished: searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1));

        //prepare grid model
        var model = await new ManufacturerListModel().PrepareToGridAsync(searchModel, manufacturers, () =>
        {
            //fill in model values from the entity
            return manufacturers.SelectAwait(async manufacturer =>
            {
                var manufacturerModel = manufacturer.ToModel<ManufacturerModel>();

                manufacturerModel.SeName = await _urlRecordService.GetSeNameAsync(manufacturer, 0, true, false);

                return manufacturerModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare manufacturer model
    /// </summary>
    /// <param name="model">Manufacturer model</param>
    /// <param name="manufacturer">Manufacturer</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer model
    /// </returns>
    public virtual async Task<ManufacturerModel> PrepareManufacturerModelAsync(ManufacturerModel model,
        Manufacturer manufacturer, bool excludeProperties = false)
    {
        Func<ManufacturerLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (manufacturer != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = manufacturer.ToModel<ManufacturerModel>();
                model.SeName = await _urlRecordService.GetSeNameAsync(manufacturer, 0, true, false);
            }

            //prepare nested search model
            PrepareManufacturerProductSearchModel(model.ManufacturerProductSearchModel, manufacturer);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(manufacturer, entity => entity.Name, languageId, false, false);
                locale.Description = await _localizationService.GetLocalizedAsync(manufacturer, entity => entity.Description, languageId, false, false);
                locale.MetaKeywords = await _localizationService.GetLocalizedAsync(manufacturer, entity => entity.MetaKeywords, languageId, false, false);
                locale.MetaDescription = await _localizationService.GetLocalizedAsync(manufacturer, entity => entity.MetaDescription, languageId, false, false);
                locale.MetaTitle = await _localizationService.GetLocalizedAsync(manufacturer, entity => entity.MetaTitle, languageId, false, false);
                locale.SeName = await _urlRecordService.GetSeNameAsync(manufacturer, languageId, false, false);
            };
        }

        //set default values for the new model
        if (manufacturer == null)
        {
            model.PageSize = _catalogSettings.DefaultManufacturerPageSize;
            model.PageSizeOptions = _catalogSettings.DefaultManufacturerPageSizeOptions;
            model.Published = true;
            model.AllowCustomersToSelectPageSize = true;
            model.PriceRangeFiltering = true;
            model.ManuallyPriceRange = true;
            model.PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
            model.PriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
        }

        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare available manufacturer templates
        await _baseAdminModelFactory.PrepareManufacturerTemplatesAsync(model.AvailableManufacturerTemplates, false);

        //prepare model discounts
        var availableDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToManufacturers, showHidden: true, isActive: null);
        await _discountSupportedModelFactory.PrepareModelDiscountsAsync(model, manufacturer, availableDiscounts, excludeProperties);
        
        //prepare model stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, manufacturer, excludeProperties);

        return model;
    }

    /// <summary>
    /// Prepare paged manufacturer product list model
    /// </summary>
    /// <param name="searchModel">Manufacturer product search model</param>
    /// <param name="manufacturer">Manufacturer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer product list model
    /// </returns>
    public virtual async Task<ManufacturerProductListModel> PrepareManufacturerProductListModelAsync(ManufacturerProductSearchModel searchModel,
        Manufacturer manufacturer)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(manufacturer);

        //get product manufacturers
        var productManufacturers = await _manufacturerService.GetProductManufacturersByManufacturerIdAsync(showHidden: true,
            manufacturerId: manufacturer.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new ManufacturerProductListModel().PrepareToGridAsync(searchModel, productManufacturers, () =>
        {
            return productManufacturers.SelectAwait(async productManufacturer =>
            {
                //fill in model values from the entity
                var manufacturerProductModel = productManufacturer.ToModel<ManufacturerProductModel>();

                //fill in additional values (not existing in the entity)
                manufacturerProductModel.ProductName = (await _productService.GetProductByIdAsync(productManufacturer.ProductId))?.Name;

                return manufacturerProductModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare product search model to add to the manufacturer
    /// </summary>
    /// <param name="searchModel">Product search model to add to the manufacturer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product search model to add to the manufacturer
    /// </returns>
    public virtual async Task<AddProductToManufacturerSearchModel> PrepareAddProductToManufacturerSearchModelAsync(AddProductToManufacturerSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available categories
        await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

        //prepare available manufacturers
        await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        //prepare available vendors
        await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

        //prepare available product types
        await _baseAdminModelFactory.PrepareProductTypesAsync(searchModel.AvailableProductTypes);

        //prepare page parameters
        searchModel.SetPopupGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged product list model to add to the manufacturer
    /// </summary>
    /// <param name="searchModel">Product search model to add to the manufacturer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product list model to add to the manufacturer
    /// </returns>
    public virtual async Task<AddProductToManufacturerListModel> PrepareAddProductToManufacturerListModelAsync(AddProductToManufacturerSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get products
        var products = await _productService.SearchProductsAsync(showHidden: true,
            categoryIds: new List<int> { searchModel.SearchCategoryId },
            manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
            storeId: searchModel.SearchStoreId,
            vendorId: searchModel.SearchVendorId,
            productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
            keywords: searchModel.SearchProductName,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new AddProductToManufacturerListModel().PrepareToGridAsync(searchModel, products, () =>
        {
            return products.SelectAwait(async product =>
            {
                var productModel = product.ToModel<ProductModel>();

                productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);

                return productModel;
            });
        });

        return model;
    }

    #endregion
}