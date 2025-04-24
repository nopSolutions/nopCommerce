using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml;
using ClosedXML.Excel;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.ExportImport.Help;
using Nop.Services.Forums;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;

namespace Nop.Services.ExportImport;

/// <summary>
/// Export manager
/// </summary>
public partial class ExportManager : IExportManager
{
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly SecuritySettings _securitySettings;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly CustomerSettings _customerSettings;
    protected readonly DateTimeSettings _dateTimeSettings;
    protected readonly ForumSettings _forumSettings;
    protected readonly IAddressService _addressService;
    protected readonly ICategoryService _categoryService;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly IAttributeFormatter<CustomerAttribute, CustomerAttributeValue> _customerAttributeFormatter;
    protected readonly ICustomerService _customerService;
    protected readonly IDateRangeService _dateRangeService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IDiscountService _discountService;
    protected readonly IForumService _forumService;
    protected readonly IGdprService _gdprService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IMeasureService _measureService;
    protected readonly IOrderService _orderService;
    protected readonly IPictureService _pictureService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly IProductTagService _productTagService;
    protected readonly IProductTemplateService _productTemplateService;
    protected readonly IShipmentService _shipmentService;
    protected readonly ISpecificationAttributeService _specificationAttributeService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IStoreService _storeService;
    protected readonly ITaxCategoryService _taxCategoryService;
    protected readonly IThumbService _thumbService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVendorService _vendorService;
    protected readonly IWorkContext _workContext;
    protected readonly OrderSettings _orderSettings;
    protected readonly ProductEditorSettings _productEditorSettings;

    #endregion

    #region Ctor

    public ExportManager(AddressSettings addressSettings,
        CatalogSettings catalogSettings,
        SecuritySettings securitySettings,
        CustomerSettings customerSettings,
        DateTimeSettings dateTimeSettings,
        ForumSettings forumSettings,
        IAddressService addressService,
        IAttributeFormatter<CustomerAttribute, CustomerAttributeValue> customerAttributeFormatter,
        ICategoryService categoryService,
        ICountryService countryService,
        ICurrencyService currencyService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IDateRangeService dateRangeService,
        IDateTimeHelper dateTimeHelper,
        IDiscountService discountService,
        IForumService forumService,
        IGdprService gdprService,
        IGenericAttributeService genericAttributeService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        IManufacturerService manufacturerService,
        IMeasureService measureService,
        IOrderService orderService,
        IPictureService pictureService,
        IPriceFormatter priceFormatter,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IProductTagService productTagService,
        IProductTemplateService productTemplateService,
        IShipmentService shipmentService,
        ISpecificationAttributeService specificationAttributeService,
        IStateProvinceService stateProvinceService,
        IStoreMappingService storeMappingService,
        IStoreService storeService,
        ITaxCategoryService taxCategoryService,
        IThumbService thumbService,
        IUrlRecordService urlRecordService,
        IVendorService vendorService,
        IWorkContext workContext,
        OrderSettings orderSettings,
        ProductEditorSettings productEditorSettings)
    {
        _addressSettings = addressSettings;
        _catalogSettings = catalogSettings;
        _securitySettings = securitySettings;
        _customerSettings = customerSettings;
        _dateTimeSettings = dateTimeSettings;
        _addressService = addressService;
        _customerAttributeFormatter = customerAttributeFormatter;
        _forumSettings = forumSettings;
        _categoryService = categoryService;
        _countryService = countryService;
        _currencyService = currencyService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _dateRangeService = dateRangeService;
        _dateTimeHelper = dateTimeHelper;
        _discountService = discountService;
        _forumService = forumService;
        _gdprService = gdprService;
        _genericAttributeService = genericAttributeService;
        _languageService = languageService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _manufacturerService = manufacturerService;
        _measureService = measureService;
        _orderService = orderService;
        _pictureService = pictureService;
        _priceFormatter = priceFormatter;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _productTagService = productTagService;
        _productTemplateService = productTemplateService;
        _shipmentService = shipmentService;
        _specificationAttributeService = specificationAttributeService;
        _stateProvinceService = stateProvinceService;
        _storeMappingService = storeMappingService;
        _storeService = storeService;
        _taxCategoryService = taxCategoryService;
        _thumbService = thumbService;
        _urlRecordService = urlRecordService;
        _vendorService = vendorService;
        _workContext = workContext;
        _orderSettings = orderSettings;
        _productEditorSettings = productEditorSettings;
    }

    #endregion

    #region Utilities

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<int> WriteCategoriesAsync(XmlWriter xmlWriter, int parentCategoryId, int totalCategories)
    {
        var categories = await _categoryService.GetAllCategoriesByParentCategoryIdAsync(parentCategoryId, true);
        if (categories == null || !categories.Any())
            return totalCategories;

        totalCategories += categories.Count;

        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        foreach (var category in categories)
        {
            await xmlWriter.WriteStartElementAsync("Category");

            await xmlWriter.WriteStringAsync("Id", category.Id);

            await WriteLocalizedPropertyXmlAsync(category, c => c.Name, xmlWriter, languages);
            await WriteLocalizedPropertyXmlAsync(category, c => c.Description, xmlWriter, languages);
            await xmlWriter.WriteStringAsync("CategoryTemplateId", category.CategoryTemplateId);
            await WriteLocalizedPropertyXmlAsync(category, c => c.MetaKeywords, xmlWriter, languages, await IgnoreExportCategoryPropertyAsync());
            await WriteLocalizedPropertyXmlAsync(category, c => c.MetaDescription, xmlWriter, languages, await IgnoreExportCategoryPropertyAsync());
            await WriteLocalizedPropertyXmlAsync(category, c => c.MetaTitle, xmlWriter, languages, await IgnoreExportCategoryPropertyAsync());
            await WriteLocalizedSeNameXmlAsync(category, xmlWriter, languages, await IgnoreExportCategoryPropertyAsync());
            await xmlWriter.WriteStringAsync("ParentCategoryId", category.ParentCategoryId);
            await xmlWriter.WriteStringAsync("PictureId", category.PictureId);
            await xmlWriter.WriteStringAsync("PageSize", category.PageSize, await IgnoreExportCategoryPropertyAsync());
            await xmlWriter.WriteStringAsync("AllowCustomersToSelectPageSize", category.AllowCustomersToSelectPageSize, await IgnoreExportCategoryPropertyAsync());
            await xmlWriter.WriteStringAsync("PageSizeOptions", category.PageSizeOptions, await IgnoreExportCategoryPropertyAsync());
            await xmlWriter.WriteStringAsync("PriceRangeFiltering", category.PriceRangeFiltering, await IgnoreExportCategoryPropertyAsync());
            await xmlWriter.WriteStringAsync("PriceFrom", category.PriceFrom, await IgnoreExportCategoryPropertyAsync());
            await xmlWriter.WriteStringAsync("PriceTo", category.PriceTo, await IgnoreExportCategoryPropertyAsync());
            await xmlWriter.WriteStringAsync("ManuallyPriceRange", category.ManuallyPriceRange, await IgnoreExportCategoryPropertyAsync());
            await xmlWriter.WriteStringAsync("ShowOnHomepage", category.ShowOnHomepage, await IgnoreExportCategoryPropertyAsync());
            await xmlWriter.WriteStringAsync("Published", category.Published, await IgnoreExportCategoryPropertyAsync());
            await xmlWriter.WriteStringAsync("Deleted", category.Deleted, true);
            await xmlWriter.WriteStringAsync("DisplayOrder", category.DisplayOrder);
            await xmlWriter.WriteStringAsync("CreatedOnUtc", category.CreatedOnUtc, await IgnoreExportCategoryPropertyAsync());
            await xmlWriter.WriteStringAsync("UpdatedOnUtc", category.UpdatedOnUtc, await IgnoreExportCategoryPropertyAsync());

            await xmlWriter.WriteStartElementAsync("Products");
            var productCategories = await _categoryService.GetProductCategoriesByCategoryIdAsync(category.Id, showHidden: true);
            foreach (var productCategory in productCategories)
            {
                var product = await _productService.GetProductByIdAsync(productCategory.ProductId);
                if (product == null || product.Deleted)
                    continue;

                await xmlWriter.WriteStartElementAsync("ProductCategory");
                await xmlWriter.WriteStringAsync("ProductCategoryId", productCategory.Id);
                await xmlWriter.WriteStringAsync("ProductId", productCategory.ProductId);
                await WriteLocalizedPropertyXmlAsync(product, p => p.Name, xmlWriter, languages, overriddenNodeName: "ProductName");
                await xmlWriter.WriteStringAsync("IsFeaturedProduct", productCategory.IsFeaturedProduct);
                await xmlWriter.WriteStringAsync("DisplayOrder", productCategory.DisplayOrder);
                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();

            await xmlWriter.WriteStartElementAsync("SubCategories");
            totalCategories = await WriteCategoriesAsync(xmlWriter, category.Id, totalCategories);
            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndElementAsync();
        }

        return totalCategories;
    }

    /// <summary>
    /// Returns the path to the image file by ID
    /// </summary>
    /// <param name="pictureId">Picture ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the path to the image file
    /// </returns>
    protected virtual async Task<string> GetPicturesAsync(int pictureId)
    {
        var picture = await _pictureService.GetPictureByIdAsync(pictureId);

        var (pictureUrl, _) = await _pictureService.GetPictureUrlAsync(picture);

        return await _thumbService.GetThumbLocalPathAsync(pictureUrl);
    }

    /// <summary>
    /// Returns the list of categories for a product separated by a ";"
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of categories
    /// </returns>
    protected virtual async Task<object> GetCategoriesAsync(Product product)
    {
        string categoryNames = null;
        foreach (var pc in await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, true))
        {
            if (_catalogSettings.ExportImportRelatedEntitiesByName)
            {
                var category = await _categoryService.GetCategoryByIdAsync(pc.CategoryId);
                categoryNames += _catalogSettings.ExportImportProductCategoryBreadcrumb
                    ? await _categoryService.GetFormattedBreadCrumbAsync(category)
                    : category.Name;
            }
            else
            {
                categoryNames += pc.CategoryId.ToString();
            }

            categoryNames += ";";
        }

        return categoryNames;
    }

    /// <summary>
    /// Returns the list of manufacturer for a product separated by a ";"
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of manufacturer
    /// </returns>
    protected virtual async Task<object> GetManufacturersAsync(Product product)
    {
        string manufacturerNames = null;
        foreach (var pm in await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id, true))
        {
            if (_catalogSettings.ExportImportRelatedEntitiesByName)
            {
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(pm.ManufacturerId);
                manufacturerNames += manufacturer.Name;
            }
            else
            {
                manufacturerNames += pm.ManufacturerId.ToString();
            }

            manufacturerNames += ";";
        }

        return manufacturerNames;
    }

    /// <summary>
    /// Returns the list of limited to stores for an entity separated by a ";"
    /// </summary>
    /// <param name="entity">IStoreMappingSupported entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of store
    /// </returns>
    protected virtual async Task<object> GetLimitedToStoresAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported
    {
        string limitedToStores = null;

        foreach (var storeMapping in await _storeMappingService.GetStoreMappingsAsync(entity))
        {
            var store = await _storeService.GetStoreByIdAsync(storeMapping.StoreId);

            if (store == null)
                continue;

            limitedToStores += _catalogSettings.ExportImportRelatedEntitiesByName ? store.Name : store.Id.ToString();

            limitedToStores += ";";
        }

        return limitedToStores;
    }

    /// <summary>
    /// Returns the list of product tag for a product separated by a ";"
    /// </summary>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of product tag
    /// </returns>
    protected virtual async Task<object> GetProductTagsAsync(Product product)
    {
        string productTagNames = null;

        var productTags = await _productTagService.GetAllProductTagsByProductIdAsync(product.Id);

        if (!productTags?.Any() ?? true)
            return null;

        foreach (var productTag in productTags)
        {
            productTagNames += _catalogSettings.ExportImportRelatedEntitiesByName
                ? productTag.Name
                : productTag.Id.ToString();

            productTagNames += ";";
        }

        return productTagNames;
    }

    /// <summary>
    /// Returns the image at specified index associated with the product
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="pictureIndex">Picture index to get</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the image thumb local path
    /// </returns>
    protected virtual async Task<string> GetPictureAsync(Product product, short pictureIndex)
    {
        // we need only the picture at a specific index, no need to get more pictures than that
        var recordsToReturn = pictureIndex + 1;
        var pictures = await _pictureService.GetPicturesByProductIdAsync(product.Id, recordsToReturn);

        if (pictures.Count <= pictureIndex)
            return null;

        var (pictureUrl, _) = await _pictureService.GetPictureUrlAsync(pictures[pictureIndex]);

        return await _thumbService.GetThumbLocalPathAsync(pictureUrl);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<bool> IgnoreExportProductPropertyAsync(Func<ProductEditorSettings, bool> func)
    {
        var productAdvancedMode = true;
        try
        {
            productAdvancedMode = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), "product-advanced-mode");
        }
        catch (ArgumentNullException)
        {
        }

        return !productAdvancedMode && !func(_productEditorSettings);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<bool> IgnoreExportCategoryPropertyAsync()
    {
        try
        {
            return !await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), "category-advanced-mode");
        }
        catch (ArgumentNullException)
        {
            return false;
        }
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<bool> IgnoreExportManufacturerPropertyAsync()
    {
        try
        {
            return !await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), "manufacturer-advanced-mode");
        }
        catch (ArgumentNullException)
        {
            return false;
        }
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<bool> ProductIgnoreExportLimitedToStoreAsync()
    {
        return _catalogSettings.IgnoreStoreLimitations ||
               !_catalogSettings.ExportImportProductUseLimitedToStores ||
               (await _storeService.GetAllStoresAsync()).Count == 1;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<bool> CategoryIgnoreExportLimitedToStoreAsync()
    {
        return _catalogSettings.IgnoreStoreLimitations ||
            !_catalogSettings.ExportImportCategoryUseLimitedToStores ||
            (await _storeService.GetAllStoresAsync()).Count == 1;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<TProperty> GetLocalizedAsync<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> keySelector,
        Language language) where TEntity : BaseEntity, ILocalizedEntity
    {
        if (entity == null)
            return default;

        return await _localizationService.GetLocalizedAsync(entity, keySelector, language.Id, false);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<PropertyManager<ExportProductAttribute>> GetProductAttributeManagerAsync(IList<Language> languages)
    {
        var attributeProperties = new[]
        {
            new PropertyByName<ExportProductAttribute>("AttributeId", (p, _) => p.AttributeId),
            new PropertyByName<ExportProductAttribute>("AttributeName", (p, _) => p.AttributeName),
            new PropertyByName<ExportProductAttribute>("DefaultValue", (p, _) => p.DefaultValue),
            new PropertyByName<ExportProductAttribute>("ValidationMinLength", (p, _) => p.ValidationMinLength),
            new PropertyByName<ExportProductAttribute>("ValidationMaxLength", (p, _) => p.ValidationMaxLength),
            new PropertyByName<ExportProductAttribute>("ValidationFileAllowedExtensions", (p, _) => p.ValidationFileAllowedExtensions),
            new PropertyByName<ExportProductAttribute>("ValidationFileMaximumSize", (p, _) => p.ValidationFileMaximumSize),
            new PropertyByName<ExportProductAttribute>("AttributeTextPrompt", (p, _) => p.AttributeTextPrompt),
            new PropertyByName<ExportProductAttribute>("AttributeIsRequired", (p, _) => p.AttributeIsRequired),
            new PropertyByName<ExportProductAttribute>("AttributeControlType", (p, _) => p.AttributeControlTypeId)
            {
                DropDownElements = await AttributeControlType.TextBox.ToSelectListAsync(useLocalization: false)
            },
            new PropertyByName<ExportProductAttribute>("AttributeDisplayOrder", (p, _) => p.AttributeDisplayOrder),
            new PropertyByName<ExportProductAttribute>("ProductAttributeValueId", (p, _) => p.Id),
            new PropertyByName<ExportProductAttribute>("ValueName", (p, _) => p.Name),
            new PropertyByName<ExportProductAttribute>("AttributeValueType", (p, _) => p.AttributeValueTypeId)
            {
                DropDownElements = await AttributeValueType.Simple.ToSelectListAsync(useLocalization: false)
            },

            new PropertyByName<ExportProductAttribute>("AssociatedProductId", (p, _) => p.AssociatedProductId),
            new PropertyByName<ExportProductAttribute>("ColorSquaresRgb", (p, _) => p.ColorSquaresRgb),
            new PropertyByName<ExportProductAttribute>("ImageSquaresPictureId", (p, _) => p.ImageSquaresPictureId),
            new PropertyByName<ExportProductAttribute>("PriceAdjustment", (p, _) => p.PriceAdjustment),
            new PropertyByName<ExportProductAttribute>("PriceAdjustmentUsePercentage", (p, _) => p.PriceAdjustmentUsePercentage),
            new PropertyByName<ExportProductAttribute>("WeightAdjustment", (p, _) => p.WeightAdjustment),
            new PropertyByName<ExportProductAttribute>("Cost", (p, _) => p.Cost),
            new PropertyByName<ExportProductAttribute>("CustomerEntersQty", (p, _) => p.CustomerEntersQty),
            new PropertyByName<ExportProductAttribute>("Quantity", (p, _) => p.Quantity),
            new PropertyByName<ExportProductAttribute>("IsPreSelected", (p, _) => p.IsPreSelected),
            new PropertyByName<ExportProductAttribute>("DisplayOrder", (p, _) => p.DisplayOrder),
            new PropertyByName<ExportProductAttribute>("PictureIds", async (p, _) => string.Join(",",
                (await _productAttributeService.GetProductAttributeValuePicturesAsync(p.Id)).Select(vp => vp.PictureId)))
        };

        var localizedProperties = new[]
        {
            new PropertyByName<ExportProductAttribute>("DefaultValue", async (p, l) =>
                await GetLocalizedAsync(await _productAttributeService.GetProductAttributeMappingByIdAsync(p.AttributeMappingId), x => x.DefaultValue, l)),
            new PropertyByName<ExportProductAttribute>("AttributeTextPrompt", async (p, l) =>
                await GetLocalizedAsync(await _productAttributeService.GetProductAttributeMappingByIdAsync(p.AttributeMappingId), x => x.TextPrompt, l)),
            new PropertyByName<ExportProductAttribute>("ValueName", async (p, l) =>
                await GetLocalizedAsync(await _productAttributeService.GetProductAttributeValueByIdAsync(p.Id), x => x.Name, l)),
        };

        return new PropertyManager<ExportProductAttribute>(attributeProperties, _catalogSettings, localizedProperties, languages);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<PropertyManager<ExportSpecificationAttribute>> GetSpecificationAttributeManagerAsync(IList<Language> languages)
    {
        var attributeProperties = new[]
        {
            new PropertyByName<ExportSpecificationAttribute>("AttributeType", (p, _) => p.AttributeTypeId)
            {
                DropDownElements = await SpecificationAttributeType.Option.ToSelectListAsync(useLocalization: false)
            },
            new PropertyByName<ExportSpecificationAttribute>("SpecificationAttribute", (p, _) => p.SpecificationAttributeId)
            {
                DropDownElements = (await _specificationAttributeService.GetAllSpecificationAttributesAsync()).Select(sa => sa as BaseEntity).ToSelectList(p => (p as SpecificationAttribute)?.Name ?? string.Empty)
            },
            new PropertyByName<ExportSpecificationAttribute>("CustomValue", (p, _) => p.CustomValue),
            new PropertyByName<ExportSpecificationAttribute>("SpecificationAttributeOptionId", (p, _) => p.SpecificationAttributeOptionId),
            new PropertyByName<ExportSpecificationAttribute>("AllowFiltering", (p, _) => p.AllowFiltering),
            new PropertyByName<ExportSpecificationAttribute>("ShowOnProductPage", (p, _) => p.ShowOnProductPage),
            new PropertyByName<ExportSpecificationAttribute>("DisplayOrder", (p, _) => p.DisplayOrder)
        };

        var localizedProperties = new[]
        {
            new PropertyByName<ExportSpecificationAttribute>("CustomValue", async (p, l) =>
                await GetLocalizedAsync(await _specificationAttributeService.GetProductSpecificationAttributeByIdAsync(p.Id), x => x.CustomValue, l)),
        };

        return new PropertyManager<ExportSpecificationAttribute>(attributeProperties, _catalogSettings, localizedProperties, languages);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<PropertyManager<ExportTierPrice>> GetTierPriceManagerAsync(IList<Language> languages)
    {
        var tierPriceProperties = new[]
        {
            new PropertyByName<ExportTierPrice>("TierPriceId", (p, _) => p.Id),
            new PropertyByName<ExportTierPrice>("Store", (p, _) => p.StoreId)
            {
                DropDownElements = (await _storeService.GetAllStoresAsync()).ToSelectList(p=>(p as Store)?.Name ?? string.Empty)
            },
            new PropertyByName<ExportTierPrice>("CustomerRole", (p, _) => p.CustomerRoleId ?? 0)
            {
                DropDownElements = (await _customerService.GetAllCustomerRolesAsync()).ToSelectList(p=>(p as CustomerRole)?.Name ?? string.Empty)
            },
            new PropertyByName<ExportTierPrice>("Quantity", (p, _) => p.Quantity),
            new PropertyByName<ExportTierPrice>("Price", (p, _) => p.Price),
            new PropertyByName<ExportTierPrice>("StartDateTimeUtc", (p, _) => p.StartDateTimeUtc),
            new PropertyByName<ExportTierPrice>("EndDateTimeUtc", (p, _) => p.EndDateTimeUtc)
        };

        return new PropertyManager<ExportTierPrice>(tierPriceProperties, _catalogSettings, languages: languages);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<byte[]> ExportProductsToXlsxWithAdditionalInfoAsync(PropertyByName<Product>[] properties, PropertyByName<Product>[] localizedProperties, IEnumerable<Product> itemsToExport, IList<Language> languages)
    {
        var productAttributeManager = await GetProductAttributeManagerAsync(languages);
        var specificationAttributeManager = await GetSpecificationAttributeManagerAsync(languages);
        var tierPriceManager = await GetTierPriceManagerAsync(languages);

        await using var stream = new MemoryStream();
        // ok, we can run the real code of the sample now
        using (var workbook = new XLWorkbook())
        {
            // uncomment this line if you want the XML written out to the outputDir
            //xlPackage.DebugMode = true; 

            // get handles to the worksheets
            // Worksheet names cannot be more than 31 characters
            var worksheet = workbook.Worksheets.Add(nameof(Product));
            var fpWorksheet = workbook.Worksheets.Add("ProductsFilters");
            fpWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;
            var fbaWorksheet = workbook.Worksheets.Add("ProductAttributesFilters");
            fbaWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;
            var fsaWorksheet = workbook.Worksheets.Add("SpecificationAttributesFilters");
            fsaWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;

            //create Headers and format them 
            var manager = new PropertyManager<Product>(properties, _catalogSettings, localizedProperties, languages);
            manager.WriteDefaultCaption(worksheet);

            var localizedWorksheets = new List<(Language Language, IXLWorksheet Worksheet)>();
            if (languages.Count >= 2)
            {
                foreach (var language in languages)
                {
                    var lws = workbook.Worksheets.Add(language.UniqueSeoCode);
                    localizedWorksheets.Add(new(language, lws));
                    manager.WriteLocalizedCaption(lws);
                }
            }

            var row = 2;
            foreach (var item in itemsToExport)
            {
                manager.CurrentObject = item;
                await manager.WriteDefaultToXlsxAsync(worksheet, row, fWorksheet: fpWorksheet);

                foreach (var lws in localizedWorksheets)
                {
                    manager.CurrentLanguage = lws.Language;
                    await manager.WriteLocalizedToXlsxAsync(lws.Worksheet, row, fWorksheet: fpWorksheet);
                }
                row++;

                if (_catalogSettings.ExportImportProductAttributes)
                    row = await ExportProductAttributesAsync(item, productAttributeManager, worksheet, localizedWorksheets, row, fbaWorksheet);

                if (_catalogSettings.ExportImportProductSpecificationAttributes)
                    row = await ExportSpecificationAttributesAsync(item, specificationAttributeManager, worksheet, localizedWorksheets, row, fsaWorksheet);

                if (_catalogSettings.ExportImportTierPrices)
                    row = await ExportTierPricesAsync(item, tierPriceManager, worksheet, localizedWorksheets, row, fsaWorksheet);
            }

            workbook.SaveAs(stream);
        }

        return stream.ToArray();
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<int> ExportProductAttributesAsync(Product item, PropertyManager<ExportProductAttribute> attributeManager,
        IXLWorksheet worksheet, IList<(Language Language, IXLWorksheet Worksheet)> localizedWorksheets, int row, IXLWorksheet faWorksheet)
    {
        var attributes = await (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(item.Id))
            .SelectManyAwait(async pam =>
            {
                var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);

                var values = await _productAttributeService.GetProductAttributeValuesAsync(pam.Id);

                if (values?.Any() ?? false)
                {
                    IEnumerable<ExportProductAttribute> productAttributes = await values.SelectAwait(async pav =>
                        new ExportProductAttribute
                        {
                            AttributeId = productAttribute.Id,
                            AttributeName = productAttribute.Name,
                            AttributeTextPrompt = pam.TextPrompt,
                            AttributeIsRequired = pam.IsRequired,
                            AttributeControlTypeId = pam.AttributeControlTypeId,
                            AttributeMappingId = pav.ProductAttributeMappingId,
                            AssociatedProductId = pav.AssociatedProductId,
                            AttributeDisplayOrder = pam.DisplayOrder,
                            Id = pav.Id,
                            Name = pav.Name,
                            AttributeValueTypeId = pav.AttributeValueTypeId,
                            ColorSquaresRgb = pav.ColorSquaresRgb,
                            ImageSquaresPictureId = pav.ImageSquaresPictureId,
                            PriceAdjustment = pav.PriceAdjustment,
                            PriceAdjustmentUsePercentage = pav.PriceAdjustmentUsePercentage,
                            WeightAdjustment = pav.WeightAdjustment,
                            Cost = pav.Cost,
                            CustomerEntersQty = pav.CustomerEntersQty,
                            Quantity = pav.Quantity,
                            IsPreSelected = pav.IsPreSelected,
                            DisplayOrder = pav.DisplayOrder,
                            PictureIds = string.Join(";",
                                (await _productAttributeService.GetProductAttributeValuePicturesAsync(pav.Id)).Select(vp => vp.PictureId))
                        }).ToListAsync();

                    return productAttributes;
                }

                var attribute = new ExportProductAttribute
                {
                    AttributeId = productAttribute.Id,
                    AttributeName = productAttribute.Name,
                    AttributeTextPrompt = pam.TextPrompt,
                    AttributeIsRequired = pam.IsRequired,
                    AttributeControlTypeId = pam.AttributeControlTypeId,
                };

                //validation rules
                if (!pam.ValidationRulesAllowed())
                    return new List<ExportProductAttribute> { attribute };

                attribute.ValidationMinLength = pam.ValidationMinLength;
                attribute.ValidationMaxLength = pam.ValidationMaxLength;
                attribute.ValidationFileAllowedExtensions = pam.ValidationFileAllowedExtensions;
                attribute.ValidationFileMaximumSize = pam.ValidationFileMaximumSize;
                attribute.DefaultValue = pam.DefaultValue;

                return new List<ExportProductAttribute>
                {
                    attribute
                };
            }).ToListAsync();

        if (!attributes.Any())
            return row;

        attributeManager.WriteDefaultCaption(worksheet, row, ExportImportDefaults.ProductAdditionalInfoCellOffset);
        worksheet.Row(row).OutlineLevel = 1;
        worksheet.Row(row).Collapse();

        foreach (var lws in localizedWorksheets)
        {
            attributeManager.WriteLocalizedCaption(lws.Worksheet, row, ExportImportDefaults.ProductAdditionalInfoCellOffset);
            lws.Worksheet.Row(row).OutlineLevel = 1;
            lws.Worksheet.Row(row).Collapse();
        }

        foreach (var exportProductAttribute in attributes)
        {
            row++;
            attributeManager.CurrentObject = exportProductAttribute;
            await attributeManager.WriteDefaultToXlsxAsync(worksheet, row, ExportImportDefaults.ProductAdditionalInfoCellOffset, faWorksheet);
            worksheet.Row(row).OutlineLevel = 1;
            worksheet.Row(row).Collapse();

            foreach (var lws in localizedWorksheets)
            {
                attributeManager.CurrentLanguage = lws.Language;
                await attributeManager.WriteLocalizedToXlsxAsync(lws.Worksheet, row, ExportImportDefaults.ProductAdditionalInfoCellOffset, faWorksheet);
                lws.Worksheet.Row(row).OutlineLevel = 1;
                lws.Worksheet.Row(row).Collapse();
            }
        }

        return row + 1;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<int> ExportSpecificationAttributesAsync(Product item, PropertyManager<ExportSpecificationAttribute> attributeManager,
        IXLWorksheet worksheet, IList<(Language Language, IXLWorksheet Worksheet)> localizedWorksheets, int row, IXLWorksheet faWorksheet)
    {
        var attributes = await (await _specificationAttributeService
            .GetProductSpecificationAttributesAsync(item.Id)).SelectAwait(
            async psa => await ExportSpecificationAttribute.CreateAsync(psa, _specificationAttributeService)).ToListAsync();

        if (!attributes.Any())
            return row;

        attributeManager.WriteDefaultCaption(worksheet, row, ExportImportDefaults.ProductAdditionalInfoCellOffset);
        worksheet.Row(row).OutlineLevel = 1;
        worksheet.Row(row).Collapse();

        foreach (var lws in localizedWorksheets)
        {
            attributeManager.WriteLocalizedCaption(lws.Worksheet, row, ExportImportDefaults.ProductAdditionalInfoCellOffset);
            lws.Worksheet.Row(row).OutlineLevel = 1;
            lws.Worksheet.Row(row).Collapse();
        }

        foreach (var exportProductAttribute in attributes)
        {
            row++;
            attributeManager.CurrentObject = exportProductAttribute;
            await attributeManager.WriteDefaultToXlsxAsync(worksheet, row, ExportImportDefaults.ProductAdditionalInfoCellOffset, faWorksheet);
            worksheet.Row(row).OutlineLevel = 1;
            worksheet.Row(row).Collapse();

            foreach (var lws in localizedWorksheets)
            {
                attributeManager.CurrentLanguage = lws.Language;
                await attributeManager.WriteLocalizedToXlsxAsync(lws.Worksheet, row, ExportImportDefaults.ProductAdditionalInfoCellOffset, faWorksheet);
                lws.Worksheet.Row(row).OutlineLevel = 1;
                lws.Worksheet.Row(row).Collapse();
            }
        }

        return row + 1;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<int> ExportTierPricesAsync(Product item, PropertyManager<ExportTierPrice> tierPriceManager,
        IXLWorksheet worksheet, IList<(Language Language, IXLWorksheet Worksheet)> localizedWorksheets, int row, IXLWorksheet faWorksheet)
    {
        var tierPrices = (await _productService.GetTierPricesByProductAsync(item.Id)).Select(p => new ExportTierPrice
        {
            Id = p.Id,
            CustomerRoleId = p.CustomerRoleId,
            Quantity = p.Quantity,
            Price = p.Price,
            StartDateTimeUtc = p.StartDateTimeUtc,
            EndDateTimeUtc = p.EndDateTimeUtc,
            StoreId = p.StoreId
        }).ToList();

        if (!tierPrices.Any())
            return row;

        tierPriceManager.WriteDefaultCaption(worksheet, row, ExportImportDefaults.ProductAdditionalInfoCellOffset);
        worksheet.Row(row).OutlineLevel = 1;
        worksheet.Row(row).Collapse();

        foreach (var tierPrice in tierPrices)
        {
            row++;
            tierPriceManager.CurrentObject = tierPrice;
            await tierPriceManager.WriteDefaultToXlsxAsync(worksheet, row, ExportImportDefaults.ProductAdditionalInfoCellOffset, faWorksheet);
            worksheet.Row(row).OutlineLevel = 1;
            worksheet.Row(row).Collapse();
        }

        return row + 1;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<byte[]> ExportOrderToXlsxWithProductsAsync(PropertyByName<Order>[] properties, IEnumerable<Order> itemsToExport)
    {
        var orderItemProperties = new[]
        {
            new PropertyByName<OrderItem>("OrderItemGuid", (oi, _) => oi.OrderItemGuid),
            new PropertyByName<OrderItem>("Name", async (oi, _) => (await _productService.GetProductByIdAsync(oi.ProductId)).Name),
            new PropertyByName<OrderItem>("Sku", async (oi, _) => await _productService.FormatSkuAsync(await _productService.GetProductByIdAsync(oi.ProductId), oi.AttributesXml)),
            new PropertyByName<OrderItem>("PriceExclTax", (oi, _) => oi.UnitPriceExclTax),
            new PropertyByName<OrderItem>("PriceInclTax", (oi, _) => oi.UnitPriceInclTax),
            new PropertyByName<OrderItem>("Quantity", (oi, _) => oi.Quantity),
            new PropertyByName<OrderItem>("DiscountExclTax", (oi, _) => oi.DiscountAmountExclTax),
            new PropertyByName<OrderItem>("DiscountInclTax", (oi, _) => oi.DiscountAmountInclTax),
            new PropertyByName<OrderItem>("TotalExclTax", (oi, _) => oi.PriceExclTax),
            new PropertyByName<OrderItem>("TotalInclTax", (oi, _) => oi.PriceInclTax)
        };

        var orderItemsManager = new PropertyManager<OrderItem>(orderItemProperties, _catalogSettings);

        await using var stream = new MemoryStream();
        // ok, we can run the real code of the sample now
        using (var workbook = new XLWorkbook())
        {
            // uncomment this line if you want the XML written out to the outputDir
            //xlPackage.DebugMode = true; 

            // get handles to the worksheets
            // Worksheet names cannot be more than 31 characters
            var worksheet = workbook.Worksheets.Add(typeof(Order).Name);
            var fpWorksheet = workbook.Worksheets.Add("DataForProductsFilters");
            fpWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;

            //create Headers and format them 
            var manager = new PropertyManager<Order>(properties, _catalogSettings);
            manager.WriteDefaultCaption(worksheet);

            var row = 2;
            foreach (var order in itemsToExport)
            {
                manager.CurrentObject = order;
                await manager.WriteDefaultToXlsxAsync(worksheet, row++);

                //a vendor should have access only to his products
                var vendor = await _workContext.GetCurrentVendorAsync();
                var orderItems = await _orderService.GetOrderItemsAsync(order.Id, vendorId: vendor?.Id ?? 0);

                if (!orderItems.Any())
                    continue;

                orderItemsManager.WriteDefaultCaption(worksheet, row, 2);
                worksheet.Row(row).OutlineLevel = 1;
                worksheet.Row(row).Collapse();

                foreach (var orderItem in orderItems)
                {
                    row++;
                    orderItemsManager.CurrentObject = orderItem;
                    await orderItemsManager.WriteDefaultToXlsxAsync(worksheet, row, 2, fpWorksheet);
                    worksheet.Row(row).OutlineLevel = 1;
                    worksheet.Row(row).Collapse();
                }

                row++;
            }

            workbook.SaveAs(stream);
        }

        return stream.ToArray();
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<object> GetCustomCustomerAttributesAsync(Customer customer)
    {
        return await _customerAttributeFormatter.FormatAttributesAsync(customer.CustomCustomerAttributesXML, ";");
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task WriteLocalizedPropertyXmlAsync<TEntity, TPropType>(TEntity entity, Expression<Func<TEntity, TPropType>> keySelector,
        XmlWriter xmlWriter, IList<Language> languages, bool ignore = false, string overriddenNodeName = null)
        where TEntity : BaseEntity, ILocalizedEntity
    {
        if (ignore)
            return;

        ArgumentNullException.ThrowIfNull(entity);

        if (keySelector.Body is not MemberExpression member)
            throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

        if (member.Member is not PropertyInfo propInfo)
            throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

        var localeKeyGroup = entity.GetType().Name;
        var localeKey = propInfo.Name;

        var nodeName = localeKey;
        if (!string.IsNullOrWhiteSpace(overriddenNodeName))
            nodeName = overriddenNodeName;

        await xmlWriter.WriteStartElementAsync(nodeName);
        await xmlWriter.WriteStringAsync("Standard", propInfo.GetValue(entity));

        if (languages.Count >= 2)
        {
            await xmlWriter.WriteStartElementAsync("Locales");

            var properties = await _localizedEntityService.GetEntityLocalizedPropertiesAsync(entity.Id, localeKeyGroup, localeKey);
            foreach (var language in languages)
                if (properties.FirstOrDefault(lp => lp.LanguageId == language.Id) is LocalizedProperty localizedProperty)
                    await xmlWriter.WriteStringAsync(language.UniqueSeoCode, localizedProperty.LocaleValue);

            await xmlWriter.WriteEndElementAsync();
        }

        await xmlWriter.WriteEndElementAsync();
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task WriteLocalizedSeNameXmlAsync<TEntity>(TEntity entity, XmlWriter xmlWriter, IList<Language> languages,
        bool ignore = false, string overriddenNodeName = null)
        where TEntity : BaseEntity, ISlugSupported
    {
        if (ignore)
            return;

        ArgumentNullException.ThrowIfNull(entity);

        var nodeName = "SEName";
        if (!string.IsNullOrWhiteSpace(overriddenNodeName))
            nodeName = overriddenNodeName;

        await xmlWriter.WriteStartElementAsync(nodeName);
        await xmlWriter.WriteStringAsync("Standard", await _urlRecordService.GetSeNameAsync(entity, 0));

        if (languages.Count >= 2)
        {
            await xmlWriter.WriteStartElementAsync("Locales");

            foreach (var language in languages)
                if (await _urlRecordService.GetSeNameAsync(entity, language.Id, returnDefaultValue: false) is string seName && !string.IsNullOrWhiteSpace(seName))
                    await xmlWriter.WriteStringAsync(language.UniqueSeoCode, seName);

            await xmlWriter.WriteEndElementAsync();
        }

        await xmlWriter.WriteEndElementAsync();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Export manufacturer list to XML
    /// </summary>
    /// <param name="manufacturers">Manufacturers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    public virtual async Task<string> ExportManufacturersToXmlAsync(IList<Manufacturer> manufacturers)
    {
        var settings = new XmlWriterSettings
        {
            Async = true,
            ConformanceLevel = ConformanceLevel.Auto
        };

        await using var stringWriter = new StringWriter();
        await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

        await xmlWriter.WriteStartDocumentAsync();
        await xmlWriter.WriteStartElementAsync("Manufacturers");
        await xmlWriter.WriteAttributeStringAsync("Version", NopVersion.CURRENT_VERSION);

        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        foreach (var manufacturer in manufacturers)
        {
            await xmlWriter.WriteStartElementAsync("Manufacturer");

            await xmlWriter.WriteStringAsync("ManufacturerId", manufacturer.Id.ToString());
            await WriteLocalizedPropertyXmlAsync(manufacturer, m => m.Name, xmlWriter, languages);
            await WriteLocalizedPropertyXmlAsync(manufacturer, m => m.Description, xmlWriter, languages);
            await xmlWriter.WriteStringAsync("ManufacturerTemplateId", manufacturer.ManufacturerTemplateId);
            await WriteLocalizedPropertyXmlAsync(manufacturer, m => m.MetaKeywords, xmlWriter, languages, await IgnoreExportManufacturerPropertyAsync());
            await WriteLocalizedPropertyXmlAsync(manufacturer, m => m.MetaDescription, xmlWriter, languages, await IgnoreExportManufacturerPropertyAsync());
            await WriteLocalizedPropertyXmlAsync(manufacturer, m => m.MetaTitle, xmlWriter, languages, await IgnoreExportManufacturerPropertyAsync());
            await WriteLocalizedSeNameXmlAsync(manufacturer, xmlWriter, languages, await IgnoreExportManufacturerPropertyAsync());
            await xmlWriter.WriteStringAsync("PictureId", manufacturer.PictureId);
            await xmlWriter.WriteStringAsync("PageSize", manufacturer.PageSize, await IgnoreExportManufacturerPropertyAsync());
            await xmlWriter.WriteStringAsync("AllowCustomersToSelectPageSize", manufacturer.AllowCustomersToSelectPageSize, await IgnoreExportManufacturerPropertyAsync());
            await xmlWriter.WriteStringAsync("PageSizeOptions", manufacturer.PageSizeOptions, await IgnoreExportManufacturerPropertyAsync());
            await xmlWriter.WriteStringAsync("PriceRangeFiltering", manufacturer.PriceRangeFiltering, await IgnoreExportManufacturerPropertyAsync());
            await xmlWriter.WriteStringAsync("PriceFrom", manufacturer.PriceFrom, await IgnoreExportManufacturerPropertyAsync());
            await xmlWriter.WriteStringAsync("PriceTo", manufacturer.PriceTo, await IgnoreExportManufacturerPropertyAsync());
            await xmlWriter.WriteStringAsync("ManuallyPriceRange", manufacturer.ManuallyPriceRange, await IgnoreExportManufacturerPropertyAsync());
            await xmlWriter.WriteStringAsync("Published", manufacturer.Published, await IgnoreExportManufacturerPropertyAsync());
            await xmlWriter.WriteStringAsync("Deleted", manufacturer.Deleted, true);
            await xmlWriter.WriteStringAsync("DisplayOrder", manufacturer.DisplayOrder);
            await xmlWriter.WriteStringAsync("CreatedOnUtc", manufacturer.CreatedOnUtc, await IgnoreExportManufacturerPropertyAsync());
            await xmlWriter.WriteStringAsync("UpdatedOnUtc", manufacturer.UpdatedOnUtc, await IgnoreExportManufacturerPropertyAsync());

            await xmlWriter.WriteStartElementAsync("Products");
            var productManufacturers = await _manufacturerService.GetProductManufacturersByManufacturerIdAsync(manufacturer.Id, showHidden: true);
            if (productManufacturers != null)
            {
                foreach (var productManufacturer in productManufacturers)
                {
                    var product = await _productService.GetProductByIdAsync(productManufacturer.ProductId);
                    if (product == null || product.Deleted)
                        continue;

                    await xmlWriter.WriteStartElementAsync("ProductManufacturer");
                    await xmlWriter.WriteStringAsync("ProductManufacturerId", productManufacturer.Id);
                    await xmlWriter.WriteStringAsync("ProductId", productManufacturer.ProductId);
                    await WriteLocalizedPropertyXmlAsync(product, p => p.Name, xmlWriter, languages, overriddenNodeName: "ProductName");
                    await xmlWriter.WriteStringAsync("IsFeaturedProduct", productManufacturer.IsFeaturedProduct);
                    await xmlWriter.WriteStringAsync("DisplayOrder", productManufacturer.DisplayOrder);
                    await xmlWriter.WriteEndElementAsync();
                }
            }

            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndElementAsync();
        }

        await xmlWriter.WriteEndElementAsync();
        await xmlWriter.WriteEndDocumentAsync();
        await xmlWriter.FlushAsync();

        //activity log
        await _customerActivityService.InsertActivityAsync("ExportManufacturers",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportManufacturers"), manufacturers.Count));

        return stringWriter.ToString();
    }

    /// <summary>
    /// Export manufacturers to XLSX
    /// </summary>
    /// <param name="manufacturers">Manufactures</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<byte[]> ExportManufacturersToXlsxAsync(IList<Manufacturer> manufacturers)
    {
        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        var localizedProperties = new[]
        {
            new PropertyByName<Manufacturer>("Id", (p, _) => p.Id),
            new PropertyByName<Manufacturer>("Name", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.Name, l.Id, false)),
            new PropertyByName<Manufacturer>("MetaKeywords", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaKeywords, l.Id, false)),
            new PropertyByName<Manufacturer>("MetaDescription", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaDescription, l.Id, false)),
            new PropertyByName<Manufacturer>("MetaTitle", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaTitle, l.Id, false)),
            new PropertyByName<Manufacturer>("SeName", async (p, l) => await _urlRecordService.GetSeNameAsync(p, l.Id, returnDefaultValue: false), await IgnoreExportManufacturerPropertyAsync())
        };

        //property manager 
        var manager = new PropertyManager<Manufacturer>(new[]
        {
            new PropertyByName<Manufacturer>("Id", (p, _) => p.Id),
            new PropertyByName<Manufacturer>("Name", (p, _) => p.Name),
            new PropertyByName<Manufacturer>("Description", (p, _) => p.Description),
            new PropertyByName<Manufacturer>("ManufacturerTemplateId", (p, _) => p.ManufacturerTemplateId),
            new PropertyByName<Manufacturer>("MetaKeywords", (p, _)=> p.MetaKeywords, await IgnoreExportManufacturerPropertyAsync()),
            new PropertyByName<Manufacturer>("MetaDescription", (p, _) => p.MetaDescription, await IgnoreExportManufacturerPropertyAsync()),
            new PropertyByName<Manufacturer>("MetaTitle", (p, _) => p.MetaTitle, await IgnoreExportManufacturerPropertyAsync()),
            new PropertyByName<Manufacturer>("SeName", async (p, _) => await _urlRecordService.GetSeNameAsync(p, 0), await IgnoreExportManufacturerPropertyAsync()),
            new PropertyByName<Manufacturer>("Picture", async (p, _) => await GetPicturesAsync(p.PictureId)),
            new PropertyByName<Manufacturer>("PageSize", (p, _) => p.PageSize, await IgnoreExportManufacturerPropertyAsync()),
            new PropertyByName<Manufacturer>("AllowCustomersToSelectPageSize", (p, _) => p.AllowCustomersToSelectPageSize, await IgnoreExportManufacturerPropertyAsync()),
            new PropertyByName<Manufacturer>("PageSizeOptions", (p, _) => p.PageSizeOptions, await IgnoreExportManufacturerPropertyAsync()),
            new PropertyByName<Manufacturer>("PriceRangeFiltering", (p, _) => p.PriceRangeFiltering, await IgnoreExportManufacturerPropertyAsync()),
            new PropertyByName<Manufacturer>("PriceFrom", (p, _) => p.PriceFrom, await IgnoreExportManufacturerPropertyAsync()),
            new PropertyByName<Manufacturer>("PriceTo", (p, _) => p.PriceTo, await IgnoreExportManufacturerPropertyAsync()),
            new PropertyByName<Manufacturer>("ManuallyPriceRange", (p, _) => p.ManuallyPriceRange, await IgnoreExportManufacturerPropertyAsync()),
            new PropertyByName<Manufacturer>("Published", (p, _) => p.Published, await IgnoreExportManufacturerPropertyAsync()),
            new PropertyByName<Manufacturer>("DisplayOrder", (p, _) => p.DisplayOrder)
        }, _catalogSettings, localizedProperties, languages);

        //activity log
        await _customerActivityService.InsertActivityAsync("ExportManufacturers",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportManufacturers"), manufacturers.Count()));

        return await manager.ExportToXlsxAsync(manufacturers);
    }

    /// <summary>
    /// Export category list to XML
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    public virtual async Task<string> ExportCategoriesToXmlAsync()
    {
        var settings = new XmlWriterSettings
        {
            Async = true,
            ConformanceLevel = ConformanceLevel.Auto
        };

        await using var stringWriter = new StringWriter();
        await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

        await xmlWriter.WriteStartDocumentAsync();
        await xmlWriter.WriteStartElementAsync("Categories");
        await xmlWriter.WriteAttributeStringAsync("Version", NopVersion.CURRENT_VERSION);
        var totalCategories = await WriteCategoriesAsync(xmlWriter, 0, 0);
        await xmlWriter.WriteEndElementAsync();
        await xmlWriter.WriteEndDocumentAsync();
        await xmlWriter.FlushAsync();

        //activity log
        await _customerActivityService.InsertActivityAsync("ExportCategories",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportCategories"), totalCategories));

        return stringWriter.ToString();
    }

    /// <summary>
    /// Export categories to XLSX
    /// </summary>
    /// <param name="categories">Categories</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<byte[]> ExportCategoriesToXlsxAsync(IList<Category> categories)
    {
        var parentCategories = new List<Category>();
        if (_catalogSettings.ExportImportCategoriesUsingCategoryName)
            //performance optimization, load all parent categories in one SQL request
            parentCategories.AddRange(await _categoryService.GetCategoriesByIdsAsync(categories.Select(c => c.ParentCategoryId).Where(id => id != 0).ToArray()));

        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        var localizedProperties = new[]
        {
            new PropertyByName<Category>("Id", (p, _) => p.Id),
            new PropertyByName<Category>("Name", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.Name, l.Id, false)),
            new PropertyByName<Category>("MetaKeywords", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaKeywords, l.Id, false)),
            new PropertyByName<Category>("MetaDescription", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaDescription, l.Id, false)),
            new PropertyByName<Category>("MetaTitle", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaTitle, l.Id, false)),
            new PropertyByName<Category>("SeName", async (p, l) => await _urlRecordService.GetSeNameAsync(p, l.Id, returnDefaultValue: false), await IgnoreExportCategoryPropertyAsync())
        };

        //property manager 
        var manager = new PropertyManager<Category>(new[]
        {
            new PropertyByName<Category>("Id", (p, _) => p.Id),
            new PropertyByName<Category>("Name", (p, _) => p.Name),
            new PropertyByName<Category>("Description", (p, _) => p.Description),
            new PropertyByName<Category>("CategoryTemplateId", (p, _) => p.CategoryTemplateId),
            new PropertyByName<Category>("MetaKeywords", (p, _) => p.MetaKeywords, await IgnoreExportCategoryPropertyAsync()),
            new PropertyByName<Category>("MetaDescription", (p, _) => p.MetaDescription, await IgnoreExportCategoryPropertyAsync()),
            new PropertyByName<Category>("MetaTitle", (p, _) => p.MetaTitle, await IgnoreExportCategoryPropertyAsync()),
            new PropertyByName<Category>("SeName", async (p, _) => await _urlRecordService.GetSeNameAsync(p, 0), await IgnoreExportCategoryPropertyAsync()),
            new PropertyByName<Category>("ParentCategoryId", (p, _) => p.ParentCategoryId),
            new PropertyByName<Category>("ParentCategoryName", async (p, _) =>
            {
                var category = parentCategories.FirstOrDefault(c => c.Id == p.ParentCategoryId);
                return category != null ? await _categoryService.GetFormattedBreadCrumbAsync(category) : null;

            }, !_catalogSettings.ExportImportCategoriesUsingCategoryName),
            new PropertyByName<Category>("Picture", async (p, _) => await GetPicturesAsync(p.PictureId)),
            new PropertyByName<Category>("PageSize", (p, _) => p.PageSize, await IgnoreExportCategoryPropertyAsync()),
            new PropertyByName<Category>("PriceRangeFiltering", (p, _) => p.PriceRangeFiltering, await IgnoreExportCategoryPropertyAsync()),
            new PropertyByName<Category>("PriceFrom", (p, _) => p.PriceFrom, await IgnoreExportCategoryPropertyAsync()),
            new PropertyByName<Category>("PriceTo", (p, _) => p.PriceTo, await IgnoreExportCategoryPropertyAsync()),
            new PropertyByName<Category>("ManuallyPriceRange", (p, _) => p.ManuallyPriceRange, await IgnoreExportCategoryPropertyAsync()),
            new PropertyByName<Category>("AllowCustomersToSelectPageSize", (p, _) => p.AllowCustomersToSelectPageSize, await IgnoreExportCategoryPropertyAsync()),
            new PropertyByName<Category>("PageSizeOptions", (p, _) => p.PageSizeOptions, await IgnoreExportCategoryPropertyAsync()),
            new PropertyByName<Category>("ShowOnHomepage", (p, _) => p.ShowOnHomepage, await IgnoreExportCategoryPropertyAsync()),
            new PropertyByName<Category>("IsLimitedToStores", (p, _) => p.LimitedToStores, await CategoryIgnoreExportLimitedToStoreAsync()),
            new PropertyByName<Category>("LimitedToStores",async (p, _) =>  await GetLimitedToStoresAsync(p), await CategoryIgnoreExportLimitedToStoreAsync()),
            new PropertyByName<Category>("Published", (p, _) => p.Published, await IgnoreExportCategoryPropertyAsync()),
            new PropertyByName<Category>("DisplayOrder", (p, _) => p.DisplayOrder)
        }, _catalogSettings, localizedProperties, languages);

        //activity log
        await _customerActivityService.InsertActivityAsync("ExportCategories",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportCategories"), categories.Count));

        return await manager.ExportToXlsxAsync(categories);
    }

    /// <summary>
    /// Export product list to XML
    /// </summary>
    /// <param name="products">Products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    public virtual async Task<string> ExportProductsToXmlAsync(IList<Product> products)
    {
        var settings = new XmlWriterSettings
        {
            Async = true,
            ConformanceLevel = ConformanceLevel.Auto
        };

        await using var stringWriter = new StringWriter();
        await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

        await xmlWriter.WriteStartDocumentAsync();
        await xmlWriter.WriteStartElementAsync("Products");
        await xmlWriter.WriteAttributeStringAsync("Version", NopVersion.CURRENT_VERSION);
        var currentVendor = await _workContext.GetCurrentVendorAsync();

        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        foreach (var product in products)
        {
            await xmlWriter.WriteStartElementAsync("Product");

            await xmlWriter.WriteStringAsync("ProductId", product.Id);
            await xmlWriter.WriteStringAsync("ProductTypeId", product.ProductTypeId, await IgnoreExportProductPropertyAsync(p => p.ProductType));
            await xmlWriter.WriteStringAsync("ParentGroupedProductId", product.ParentGroupedProductId, await IgnoreExportProductPropertyAsync(p => p.ProductType));
            await xmlWriter.WriteStringAsync("VisibleIndividually", product.VisibleIndividually, await IgnoreExportProductPropertyAsync(p => p.VisibleIndividually));
            await WriteLocalizedPropertyXmlAsync(product, p => p.Name, xmlWriter, languages);
            await WriteLocalizedPropertyXmlAsync(product, p => p.ShortDescription, xmlWriter, languages);
            await WriteLocalizedPropertyXmlAsync(product, p => p.FullDescription, xmlWriter, languages);
            await xmlWriter.WriteStringAsync("AdminComment", product.AdminComment, await IgnoreExportProductPropertyAsync(p => p.AdminComment));
            //vendor can't change this field
            await xmlWriter.WriteStringAsync("VendorId", product.VendorId, await IgnoreExportProductPropertyAsync(p => p.Vendor) || currentVendor != null);
            await xmlWriter.WriteStringAsync("ProductTemplateId", product.ProductTemplateId, await IgnoreExportProductPropertyAsync(p => p.ProductTemplate));
            //vendor can't change this field
            await xmlWriter.WriteStringAsync("ShowOnHomepage", product.ShowOnHomepage, await IgnoreExportProductPropertyAsync(p => p.ShowOnHomepage) || currentVendor != null);
            //vendor can't change this field
            await xmlWriter.WriteStringAsync("DisplayOrder", product.DisplayOrder, await IgnoreExportProductPropertyAsync(p => p.ShowOnHomepage) || currentVendor != null);
            await WriteLocalizedPropertyXmlAsync(product, p => p.MetaKeywords, xmlWriter, languages, await IgnoreExportProductPropertyAsync(p => p.Seo));
            await WriteLocalizedPropertyXmlAsync(product, p => p.MetaDescription, xmlWriter, languages, await IgnoreExportProductPropertyAsync(p => p.Seo));
            await WriteLocalizedPropertyXmlAsync(product, p => p.MetaTitle, xmlWriter, languages, await IgnoreExportProductPropertyAsync(p => p.Seo));
            await WriteLocalizedSeNameXmlAsync(product, xmlWriter, languages, await IgnoreExportProductPropertyAsync(p => p.Seo));
            await xmlWriter.WriteStringAsync("AllowCustomerReviews", product.AllowCustomerReviews, await IgnoreExportProductPropertyAsync(p => p.AllowCustomerReviews));
            await xmlWriter.WriteStringAsync("SKU", product.Sku);
            await xmlWriter.WriteStringAsync("ManufacturerPartNumber", product.ManufacturerPartNumber, await IgnoreExportProductPropertyAsync(p => p.ManufacturerPartNumber));
            await xmlWriter.WriteStringAsync("Gtin", product.Gtin, await IgnoreExportProductPropertyAsync(p => p.GTIN));
            await xmlWriter.WriteStringAsync("IsGiftCard", product.IsGiftCard, await IgnoreExportProductPropertyAsync(p => p.IsGiftCard));
            await xmlWriter.WriteStringAsync("GiftCardType", product.GiftCardType, await IgnoreExportProductPropertyAsync(p => p.IsGiftCard));
            await xmlWriter.WriteStringAsync("OverriddenGiftCardAmount", product.OverriddenGiftCardAmount, await IgnoreExportProductPropertyAsync(p => p.IsGiftCard));
            await xmlWriter.WriteStringAsync("RequireOtherProducts", product.RequireOtherProducts, await IgnoreExportProductPropertyAsync(p => p.RequireOtherProductsAddedToCart));
            await xmlWriter.WriteStringAsync("RequiredProductIds", product.RequiredProductIds, await IgnoreExportProductPropertyAsync(p => p.RequireOtherProductsAddedToCart));
            await xmlWriter.WriteStringAsync("AutomaticallyAddRequiredProducts", product.AutomaticallyAddRequiredProducts, await IgnoreExportProductPropertyAsync(p => p.RequireOtherProductsAddedToCart));
            await xmlWriter.WriteStringAsync("IsDownload", product.IsDownload, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct));
            await xmlWriter.WriteStringAsync("DownloadId", product.DownloadId, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct));
            await xmlWriter.WriteStringAsync("UnlimitedDownloads", product.UnlimitedDownloads, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct));
            await xmlWriter.WriteStringAsync("MaxNumberOfDownloads", product.MaxNumberOfDownloads, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct));
            await xmlWriter.WriteStringAsync("DownloadExpirationDays", product.DownloadExpirationDays, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct));
            await xmlWriter.WriteStringAsync("DownloadActivationType", product.DownloadActivationType, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct));
            await xmlWriter.WriteStringAsync("HasSampleDownload", product.HasSampleDownload, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct));
            await xmlWriter.WriteStringAsync("SampleDownloadId", product.SampleDownloadId, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct));
            await xmlWriter.WriteStringAsync("HasUserAgreement", product.HasUserAgreement, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct));
            await xmlWriter.WriteStringAsync("UserAgreementText", product.UserAgreementText, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct));
            await xmlWriter.WriteStringAsync("IsRecurring", product.IsRecurring, await IgnoreExportProductPropertyAsync(p => p.RecurringProduct));
            await xmlWriter.WriteStringAsync("RecurringCycleLength", product.RecurringCycleLength, await IgnoreExportProductPropertyAsync(p => p.RecurringProduct));
            await xmlWriter.WriteStringAsync("RecurringCyclePeriodId", product.RecurringCyclePeriodId, await IgnoreExportProductPropertyAsync(p => p.RecurringProduct));
            await xmlWriter.WriteStringAsync("RecurringTotalCycles", product.RecurringTotalCycles, await IgnoreExportProductPropertyAsync(p => p.RecurringProduct));
            await xmlWriter.WriteStringAsync("IsRental", product.IsRental, await IgnoreExportProductPropertyAsync(p => p.IsRental));
            await xmlWriter.WriteStringAsync("RentalPriceLength", product.RentalPriceLength, await IgnoreExportProductPropertyAsync(p => p.IsRental));
            await xmlWriter.WriteStringAsync("RentalPricePeriodId", product.RentalPricePeriodId, await IgnoreExportProductPropertyAsync(p => p.IsRental));
            await xmlWriter.WriteStringAsync("IsShipEnabled", product.IsShipEnabled);
            await xmlWriter.WriteStringAsync("IsFreeShipping", product.IsFreeShipping, await IgnoreExportProductPropertyAsync(p => p.FreeShipping));
            await xmlWriter.WriteStringAsync("ShipSeparately", product.ShipSeparately, await IgnoreExportProductPropertyAsync(p => p.ShipSeparately));
            await xmlWriter.WriteStringAsync("AdditionalShippingCharge", product.AdditionalShippingCharge, await IgnoreExportProductPropertyAsync(p => p.AdditionalShippingCharge));
            await xmlWriter.WriteStringAsync("DeliveryDateId", product.DeliveryDateId, await IgnoreExportProductPropertyAsync(p => p.DeliveryDate));
            await xmlWriter.WriteStringAsync("IsTaxExempt", product.IsTaxExempt);
            await xmlWriter.WriteStringAsync("TaxCategoryId", product.TaxCategoryId);
            await xmlWriter.WriteStringAsync("ManageInventoryMethodId", product.ManageInventoryMethodId);
            await xmlWriter.WriteStringAsync("ProductAvailabilityRangeId", product.ProductAvailabilityRangeId, await IgnoreExportProductPropertyAsync(p => p.ProductAvailabilityRange));
            await xmlWriter.WriteStringAsync("UseMultipleWarehouses", product.UseMultipleWarehouses, await IgnoreExportProductPropertyAsync(p => p.UseMultipleWarehouses));
            await xmlWriter.WriteStringAsync("WarehouseId", product.WarehouseId, await IgnoreExportProductPropertyAsync(p => p.Warehouse));
            await xmlWriter.WriteStringAsync("StockQuantity", product.StockQuantity);
            await xmlWriter.WriteStringAsync("DisplayStockAvailability", product.DisplayStockAvailability, await IgnoreExportProductPropertyAsync(p => p.DisplayStockAvailability));
            await xmlWriter.WriteStringAsync("DisplayStockQuantity", product.DisplayStockQuantity, await IgnoreExportProductPropertyAsync(p => p.DisplayStockAvailability));
            await xmlWriter.WriteStringAsync("MinStockQuantity", product.MinStockQuantity, await IgnoreExportProductPropertyAsync(p => p.MinimumStockQuantity));
            await xmlWriter.WriteStringAsync("LowStockActivityId", product.LowStockActivityId, await IgnoreExportProductPropertyAsync(p => p.LowStockActivity));
            await xmlWriter.WriteStringAsync("NotifyAdminForQuantityBelow", product.NotifyAdminForQuantityBelow, await IgnoreExportProductPropertyAsync(p => p.NotifyAdminForQuantityBelow));
            await xmlWriter.WriteStringAsync("BackorderModeId", product.BackorderModeId, await IgnoreExportProductPropertyAsync(p => p.Backorders));
            await xmlWriter.WriteStringAsync("AllowBackInStockSubscriptions", product.AllowBackInStockSubscriptions, await IgnoreExportProductPropertyAsync(p => p.AllowBackInStockSubscriptions));
            await xmlWriter.WriteStringAsync("OrderMinimumQuantity", product.OrderMinimumQuantity, await IgnoreExportProductPropertyAsync(p => p.MinimumCartQuantity));
            await xmlWriter.WriteStringAsync("OrderMaximumQuantity", product.OrderMaximumQuantity, await IgnoreExportProductPropertyAsync(p => p.MaximumCartQuantity));
            await xmlWriter.WriteStringAsync("AllowedQuantities", product.AllowedQuantities, await IgnoreExportProductPropertyAsync(p => p.AllowedQuantities));
            await xmlWriter.WriteStringAsync("AllowAddingOnlyExistingAttributeCombinations", product.AllowAddingOnlyExistingAttributeCombinations, await IgnoreExportProductPropertyAsync(p => p.AllowAddingOnlyExistingAttributeCombinations));
            await xmlWriter.WriteStringAsync("NotReturnable", product.NotReturnable, await IgnoreExportProductPropertyAsync(p => p.NotReturnable));
            await xmlWriter.WriteStringAsync("DisableBuyButton", product.DisableBuyButton, await IgnoreExportProductPropertyAsync(p => p.DisableBuyButton));
            await xmlWriter.WriteStringAsync("DisableWishlistButton", product.DisableWishlistButton, await IgnoreExportProductPropertyAsync(p => p.DisableWishlistButton));
            await xmlWriter.WriteStringAsync("AvailableForPreOrder", product.AvailableForPreOrder, await IgnoreExportProductPropertyAsync(p => p.AvailableForPreOrder));
            await xmlWriter.WriteStringAsync("PreOrderAvailabilityStartDateTimeUtc", product.PreOrderAvailabilityStartDateTimeUtc, await IgnoreExportProductPropertyAsync(p => p.AvailableForPreOrder));
            await xmlWriter.WriteStringAsync("CallForPrice", product.CallForPrice, await IgnoreExportProductPropertyAsync(p => p.CallForPrice));
            await xmlWriter.WriteStringAsync("Price", product.Price);
            await xmlWriter.WriteStringAsync("OldPrice", product.OldPrice, await IgnoreExportProductPropertyAsync(p => p.OldPrice));
            await xmlWriter.WriteStringAsync("ProductCost", product.ProductCost, await IgnoreExportProductPropertyAsync(p => p.ProductCost));
            await xmlWriter.WriteStringAsync("CustomerEntersPrice", product.CustomerEntersPrice, await IgnoreExportProductPropertyAsync(p => p.CustomerEntersPrice));
            await xmlWriter.WriteStringAsync("MinimumCustomerEnteredPrice", product.MinimumCustomerEnteredPrice, await IgnoreExportProductPropertyAsync(p => p.CustomerEntersPrice));
            await xmlWriter.WriteStringAsync("MaximumCustomerEnteredPrice", product.MaximumCustomerEnteredPrice, await IgnoreExportProductPropertyAsync(p => p.CustomerEntersPrice));
            await xmlWriter.WriteStringAsync("BasepriceEnabled", product.BasepriceEnabled, await IgnoreExportProductPropertyAsync(p => p.PAngV));
            await xmlWriter.WriteStringAsync("BasepriceAmount", product.BasepriceAmount, await IgnoreExportProductPropertyAsync(p => p.PAngV));
            await xmlWriter.WriteStringAsync("BasepriceUnitId", product.BasepriceUnitId, await IgnoreExportProductPropertyAsync(p => p.PAngV));
            await xmlWriter.WriteStringAsync("BasepriceBaseAmount", product.BasepriceBaseAmount, await IgnoreExportProductPropertyAsync(p => p.PAngV));
            await xmlWriter.WriteStringAsync("BasepriceBaseUnitId", product.BasepriceBaseUnitId, await IgnoreExportProductPropertyAsync(p => p.PAngV));
            await xmlWriter.WriteStringAsync("MarkAsNew", product.MarkAsNew, await IgnoreExportProductPropertyAsync(p => p.MarkAsNew));
            await xmlWriter.WriteStringAsync("MarkAsNewStartDateTimeUtc", product.MarkAsNewStartDateTimeUtc, await IgnoreExportProductPropertyAsync(p => p.MarkAsNew));
            await xmlWriter.WriteStringAsync("MarkAsNewEndDateTimeUtc", product.MarkAsNewEndDateTimeUtc, await IgnoreExportProductPropertyAsync(p => p.MarkAsNew));
            await xmlWriter.WriteStringAsync("Weight", product.Weight, await IgnoreExportProductPropertyAsync(p => p.Weight));
            await xmlWriter.WriteStringAsync("Length", product.Length, await IgnoreExportProductPropertyAsync(p => p.Dimensions));
            await xmlWriter.WriteStringAsync("Width", product.Width, await IgnoreExportProductPropertyAsync(p => p.Dimensions));
            await xmlWriter.WriteStringAsync("Height", product.Height, await IgnoreExportProductPropertyAsync(p => p.Dimensions));
            await xmlWriter.WriteStringAsync("Published", product.Published, await IgnoreExportProductPropertyAsync(p => p.Published));
            await xmlWriter.WriteStringAsync("CreatedOnUtc", product.CreatedOnUtc);
            await xmlWriter.WriteStringAsync("UpdatedOnUtc", product.UpdatedOnUtc);
            await xmlWriter.WriteStringAsync("AgeVerification", product.AgeVerification, await IgnoreExportProductPropertyAsync(p => p.AgeVerification));
            await xmlWriter.WriteStringAsync("MinimumAgeToPurchase", product.MinimumAgeToPurchase, await IgnoreExportProductPropertyAsync(p => p.AgeVerification));

            if (!await IgnoreExportProductPropertyAsync(p => p.Discounts))
            {
                await xmlWriter.WriteStartElementAsync("ProductDiscounts");

                foreach (var discount in await _discountService.GetAppliedDiscountsAsync(product))
                {
                    await xmlWriter.WriteStartElementAsync("Discount");
                    await xmlWriter.WriteStringAsync("DiscountId", discount.Id);
                    await xmlWriter.WriteStringAsync("Name", discount.Name);
                    await xmlWriter.WriteEndElementAsync();
                }

                await xmlWriter.WriteEndElementAsync();
            }

            if (!await IgnoreExportProductPropertyAsync(p => p.TierPrices))
            {
                await xmlWriter.WriteStartElementAsync("TierPrices");
                var tierPrices = await _productService.GetTierPricesByProductAsync(product.Id);
                foreach (var tierPrice in tierPrices)
                {
                    await xmlWriter.WriteStartElementAsync("TierPrice");
                    await xmlWriter.WriteStringAsync("TierPriceId", tierPrice.Id);
                    await xmlWriter.WriteStringAsync("StoreId", tierPrice.StoreId);
                    await xmlWriter.WriteStringAsync("CustomerRoleId", tierPrice.CustomerRoleId, defaulValue: "0");
                    await xmlWriter.WriteStringAsync("Quantity", tierPrice.Quantity);
                    await xmlWriter.WriteStringAsync("Price", tierPrice.Price);
                    await xmlWriter.WriteStringAsync("StartDateTimeUtc", tierPrice.StartDateTimeUtc);
                    await xmlWriter.WriteStringAsync("EndDateTimeUtc", tierPrice.EndDateTimeUtc);
                    await xmlWriter.WriteEndElementAsync();
                }

                await xmlWriter.WriteEndElementAsync();
            }

            if (!await IgnoreExportProductPropertyAsync(p => p.ProductAttributes))
            {
                await xmlWriter.WriteStartElementAsync("ProductAttributes");
                var productAttributMappings =
                    await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
                foreach (var productAttributeMapping in productAttributMappings)
                {
                    var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId);

                    await xmlWriter.WriteStartElementAsync("ProductAttributeMapping");
                    await xmlWriter.WriteStringAsync("ProductAttributeMappingId", productAttributeMapping.Id);
                    await xmlWriter.WriteStringAsync("ProductAttributeId", productAttributeMapping.ProductAttributeId);
                    await xmlWriter.WriteStringAsync("ProductAttributeName", productAttribute.Name);
                    await WriteLocalizedPropertyXmlAsync(productAttributeMapping, pam => pam.TextPrompt, xmlWriter, languages, overriddenNodeName: "TextPrompt");
                    await xmlWriter.WriteStringAsync("IsRequired", productAttributeMapping.IsRequired);
                    await xmlWriter.WriteStringAsync("AttributeControlTypeId", productAttributeMapping.AttributeControlTypeId);
                    await xmlWriter.WriteStringAsync("DisplayOrder", productAttributeMapping.DisplayOrder);
                    //validation rules
                    if (productAttributeMapping.ValidationRulesAllowed())
                    {
                        if (productAttributeMapping.ValidationMinLength.HasValue)
                        {
                            await xmlWriter.WriteStringAsync("ValidationMinLength",
                                productAttributeMapping.ValidationMinLength.Value);
                        }

                        if (productAttributeMapping.ValidationMaxLength.HasValue)
                        {
                            await xmlWriter.WriteStringAsync("ValidationMaxLength",
                                productAttributeMapping.ValidationMaxLength.Value);
                        }

                        if (string.IsNullOrEmpty(productAttributeMapping.ValidationFileAllowedExtensions))
                        {
                            await xmlWriter.WriteStringAsync("ValidationFileAllowedExtensions",
                                productAttributeMapping.ValidationFileAllowedExtensions);
                        }

                        if (productAttributeMapping.ValidationFileMaximumSize.HasValue)
                        {
                            await xmlWriter.WriteStringAsync("ValidationFileMaximumSize",
                                productAttributeMapping.ValidationFileMaximumSize.Value);
                        }

                        await WriteLocalizedPropertyXmlAsync(productAttributeMapping, pam => pam.DefaultValue, xmlWriter, languages, overriddenNodeName: "DefaultValue");
                    }
                    //conditions
                    await xmlWriter.WriteElementStringAsync("ConditionAttributeXml", productAttributeMapping.ConditionAttributeXml);

                    await xmlWriter.WriteStartElementAsync("ProductAttributeValues");
                    var productAttributeValues = await _productAttributeService.GetProductAttributeValuesAsync(productAttributeMapping.Id);
                    foreach (var productAttributeValue in productAttributeValues)
                    {
                        await xmlWriter.WriteStartElementAsync("ProductAttributeValue");
                        await xmlWriter.WriteStringAsync("ProductAttributeValueId", productAttributeValue.Id);
                        await WriteLocalizedPropertyXmlAsync(productAttributeValue, pav => pav.Name, xmlWriter, languages, overriddenNodeName: "Name");
                        await xmlWriter.WriteStringAsync("AttributeValueTypeId", productAttributeValue.AttributeValueTypeId);
                        await xmlWriter.WriteStringAsync("AssociatedProductId", productAttributeValue.AssociatedProductId);
                        await xmlWriter.WriteStringAsync("ColorSquaresRgb", productAttributeValue.ColorSquaresRgb);
                        await xmlWriter.WriteStringAsync("ImageSquaresPictureId", productAttributeValue.ImageSquaresPictureId);
                        await xmlWriter.WriteStringAsync("PriceAdjustment", productAttributeValue.PriceAdjustment);
                        await xmlWriter.WriteStringAsync("PriceAdjustmentUsePercentage", productAttributeValue.PriceAdjustmentUsePercentage);
                        await xmlWriter.WriteStringAsync("WeightAdjustment", productAttributeValue.WeightAdjustment);
                        await xmlWriter.WriteStringAsync("Cost", productAttributeValue.Cost);
                        await xmlWriter.WriteStringAsync("CustomerEntersQty", productAttributeValue.CustomerEntersQty);
                        await xmlWriter.WriteStringAsync("Quantity", productAttributeValue.Quantity);
                        await xmlWriter.WriteStringAsync("IsPreSelected", productAttributeValue.IsPreSelected);
                        await xmlWriter.WriteStringAsync("DisplayOrder", productAttributeValue.DisplayOrder);
                        await xmlWriter.WriteStringAsync("PictureIds", string.Join(",",
                            (await _productAttributeService.GetProductAttributeValuePicturesAsync(productAttributeValue.Id)).Select(vp => vp.PictureId)));
                        await xmlWriter.WriteEndElementAsync();
                    }

                    await xmlWriter.WriteEndElementAsync();
                    await xmlWriter.WriteEndElementAsync();
                }

                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteStartElementAsync("ProductPictures");
            var productPictures = await _productService.GetProductPicturesByProductIdAsync(product.Id);
            foreach (var productPicture in productPictures)
            {
                await xmlWriter.WriteStartElementAsync("ProductPicture");
                await xmlWriter.WriteStringAsync("ProductPictureId", productPicture.Id);
                await xmlWriter.WriteStringAsync("PictureId", productPicture.PictureId);
                await xmlWriter.WriteStringAsync("DisplayOrder", productPicture.DisplayOrder);
                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();

            await xmlWriter.WriteStartElementAsync("ProductCategories");
            var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, true);
            if (productCategories != null)
            {
                foreach (var productCategory in productCategories)
                {
                    await xmlWriter.WriteStartElementAsync("ProductCategory");
                    await xmlWriter.WriteStringAsync("ProductCategoryId", productCategory.Id);
                    await xmlWriter.WriteStringAsync("CategoryId", productCategory.CategoryId);
                    await xmlWriter.WriteStringAsync("IsFeaturedProduct", productCategory.IsFeaturedProduct);
                    await xmlWriter.WriteStringAsync("DisplayOrder", productCategory.DisplayOrder);
                    await xmlWriter.WriteEndElementAsync();
                }
            }

            await xmlWriter.WriteEndElementAsync();

            if (!await IgnoreExportProductPropertyAsync(p => p.Manufacturers))
            {
                await xmlWriter.WriteStartElementAsync("ProductManufacturers");
                var productManufacturers = await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id);
                if (productManufacturers != null)
                {
                    foreach (var productManufacturer in productManufacturers)
                    {
                        await xmlWriter.WriteStartElementAsync("ProductManufacturer");
                        await xmlWriter.WriteStringAsync("ProductManufacturerId", productManufacturer.Id);
                        await xmlWriter.WriteStringAsync("ManufacturerId", productManufacturer.ManufacturerId);
                        await xmlWriter.WriteStringAsync("IsFeaturedProduct", productManufacturer.IsFeaturedProduct);
                        await xmlWriter.WriteStringAsync("DisplayOrder", productManufacturer.DisplayOrder);
                        await xmlWriter.WriteEndElementAsync();
                    }
                }

                await xmlWriter.WriteEndElementAsync();
            }

            if (!await IgnoreExportProductPropertyAsync(p => p.SpecificationAttributes))
            {
                await xmlWriter.WriteStartElementAsync("ProductSpecificationAttributes");
                var productSpecificationAttributes = await _specificationAttributeService.GetProductSpecificationAttributesAsync(product.Id);
                foreach (var productSpecificationAttribute in productSpecificationAttributes)
                {
                    await xmlWriter.WriteStartElementAsync("ProductSpecificationAttribute");
                    await xmlWriter.WriteStringAsync("ProductSpecificationAttributeId", productSpecificationAttribute.Id);
                    await xmlWriter.WriteStringAsync("SpecificationAttributeOptionId", productSpecificationAttribute.SpecificationAttributeOptionId);
                    await xmlWriter.WriteStringAsync("CustomValue", productSpecificationAttribute.CustomValue);
                    await xmlWriter.WriteStringAsync("AllowFiltering", productSpecificationAttribute.AllowFiltering);
                    await xmlWriter.WriteStringAsync("ShowOnProductPage", productSpecificationAttribute.ShowOnProductPage);
                    await xmlWriter.WriteStringAsync("DisplayOrder", productSpecificationAttribute.DisplayOrder);
                    await xmlWriter.WriteEndElementAsync();
                }

                await xmlWriter.WriteEndElementAsync();
            }

            if (!await IgnoreExportProductPropertyAsync(p => p.ProductTags))
            {
                await xmlWriter.WriteStartElementAsync("ProductTags");
                var productTags = await _productTagService.GetAllProductTagsByProductIdAsync(product.Id);
                foreach (var productTag in productTags)
                {
                    await xmlWriter.WriteStartElementAsync("ProductTag");
                    await xmlWriter.WriteStringAsync("Id", productTag.Id);
                    await xmlWriter.WriteStringAsync("Name", productTag.Name);
                    await xmlWriter.WriteEndElementAsync();
                }

                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();
        }

        await xmlWriter.WriteEndElementAsync();
        await xmlWriter.WriteEndDocumentAsync();
        await xmlWriter.FlushAsync();

        //activity log
        await _customerActivityService.InsertActivityAsync("ExportProducts",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportProducts"), products.Count));

        return stringWriter.ToString();
    }

    /// <summary>
    /// Export products to XLSX
    /// </summary>
    /// <param name="products">Products</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<byte[]> ExportProductsToXlsxAsync(IEnumerable<Product> products)
    {
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        var localizedProperties = new[]
        {
            new PropertyByName<Product>("ProductId", (p, _) => p.Id),
            new PropertyByName<Product>("Name", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.Name, l.Id, false)),
            new PropertyByName<Product>("ShortDescription", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.ShortDescription, l.Id, false)),
            new PropertyByName<Product>("FullDescription", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.FullDescription, l.Id, false)),
            new PropertyByName<Product>("MetaKeywords", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaKeywords, l.Id, false)),
            new PropertyByName<Product>("MetaDescription", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaDescription, l.Id, false)),
            new PropertyByName<Product>("MetaTitle", async (p, l) => await _localizationService.GetLocalizedAsync(p, x => x.MetaTitle, l.Id, false)),
            new PropertyByName<Product>("SeName", async (p, l) => await _urlRecordService.GetSeNameAsync(p, l.Id, returnDefaultValue: false), await IgnoreExportProductPropertyAsync(p => p.Seo))
        };

        var productAdvancedMode = true;
        try
        {
            productAdvancedMode = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), "product-advanced-mode");
        }
        catch (ArgumentNullException)
        {
        }

        var properties = new[]
        {
            new PropertyByName<Product>("ProductId", (p, _) => p.Id),
            new PropertyByName<Product>("ProductType", (p, _) => p.ProductTypeId, await IgnoreExportProductPropertyAsync(p => p.ProductType))
            {
                DropDownElements = await ProductType.SimpleProduct.ToSelectListAsync(useLocalization: false)
            },
            new PropertyByName<Product>("ParentGroupedProductId", (p, _) => p.ParentGroupedProductId, await IgnoreExportProductPropertyAsync(p => p.ProductType)),
            new PropertyByName<Product>("VisibleIndividually", (p, _) => p.VisibleIndividually, await IgnoreExportProductPropertyAsync(p => p.VisibleIndividually)),
            new PropertyByName<Product>("Name", (p, _) => p.Name),
            new PropertyByName<Product>("ShortDescription", (p, _) => p.ShortDescription),
            new PropertyByName<Product>("FullDescription", (p, _) => p.FullDescription),
            //vendor can't change this field
            new PropertyByName<Product>("Vendor", (p, _) => p.VendorId, await IgnoreExportProductPropertyAsync(p => p.Vendor) || currentVendor != null)
            {
                DropDownElements = (await _vendorService.GetAllVendorsAsync(showHidden: true)).Select(v => v as BaseEntity).ToSelectList(p => (p as Vendor)?.Name ?? string.Empty),
                AllowBlank = true
            },
            new PropertyByName<Product>("ProductTemplate", (p, _) => p.ProductTemplateId, await IgnoreExportProductPropertyAsync(p => p.ProductTemplate))
            {
                DropDownElements = (await _productTemplateService.GetAllProductTemplatesAsync()).Select(pt => pt as BaseEntity).ToSelectList(p => (p as ProductTemplate)?.Name ?? string.Empty)
            },
            //vendor can't change this field
            new PropertyByName<Product>("ShowOnHomepage", (p, _) => p.ShowOnHomepage, await IgnoreExportProductPropertyAsync(p => p.ShowOnHomepage) || currentVendor != null),
            //vendor can't change this field
            new PropertyByName<Product>("DisplayOrder", (p, _) => p.DisplayOrder, await IgnoreExportProductPropertyAsync(p => p.ShowOnHomepage) || currentVendor != null),
            new PropertyByName<Product>("MetaKeywords", (p, _) => p.MetaKeywords, await IgnoreExportProductPropertyAsync(p => p.Seo)),
            new PropertyByName<Product>("MetaDescription", (p, _) => p.MetaDescription, await IgnoreExportProductPropertyAsync(p => p.Seo)),
            new PropertyByName<Product>("MetaTitle", (p, _) => p.MetaTitle, await IgnoreExportProductPropertyAsync(p => p.Seo)),
            new PropertyByName<Product>("SeName", async (p, _) => await _urlRecordService.GetSeNameAsync(p, 0), await IgnoreExportProductPropertyAsync(p => p.Seo)),
            new PropertyByName<Product>("AllowCustomerReviews", (p, _) => p.AllowCustomerReviews, await IgnoreExportProductPropertyAsync(p => p.AllowCustomerReviews)),
            new PropertyByName<Product>("Published", (p, _) => p.Published, await IgnoreExportProductPropertyAsync(p => p.Published)),
            new PropertyByName<Product>("SKU", (p, _) => p.Sku),
            new PropertyByName<Product>("ManufacturerPartNumber", (p, _) => p.ManufacturerPartNumber, await IgnoreExportProductPropertyAsync(p => p.ManufacturerPartNumber)),
            new PropertyByName<Product>("Gtin", (p, _) => p.Gtin, await IgnoreExportProductPropertyAsync(p => p.GTIN)),
            new PropertyByName<Product>("IsGiftCard", (p, _) => p.IsGiftCard, await IgnoreExportProductPropertyAsync(p => p.IsGiftCard)),
            new PropertyByName<Product>("GiftCardType", (p, _) => p.GiftCardTypeId, await IgnoreExportProductPropertyAsync(p => p.IsGiftCard))
            {
                DropDownElements = await GiftCardType.Virtual.ToSelectListAsync(useLocalization: false)
            },
            new PropertyByName<Product>("OverriddenGiftCardAmount", (p, _) => p.OverriddenGiftCardAmount, await IgnoreExportProductPropertyAsync(p => p.IsGiftCard)),
            new PropertyByName<Product>("RequireOtherProducts", (p, _) => p.RequireOtherProducts, await IgnoreExportProductPropertyAsync(p => p.RequireOtherProductsAddedToCart)),
            new PropertyByName<Product>("RequiredProductIds", (p, _) => p.RequiredProductIds, await IgnoreExportProductPropertyAsync(p => p.RequireOtherProductsAddedToCart)),
            new PropertyByName<Product>("AutomaticallyAddRequiredProducts", (p, _) => p.AutomaticallyAddRequiredProducts, await IgnoreExportProductPropertyAsync(p => p.RequireOtherProductsAddedToCart)),
            new PropertyByName<Product>("IsDownload", (p, _) => p.IsDownload, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
            new PropertyByName<Product>("DownloadId", (p, _) => p.DownloadId, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
            new PropertyByName<Product>("UnlimitedDownloads", (p, _) => p.UnlimitedDownloads, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
            new PropertyByName<Product>("MaxNumberOfDownloads", (p, _) => p.MaxNumberOfDownloads, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
            new PropertyByName<Product>("DownloadActivationType", (p, _) => p.DownloadActivationTypeId, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct))
            {
                DropDownElements = await DownloadActivationType.Manually.ToSelectListAsync(useLocalization: false)
            },
            new PropertyByName<Product>("HasSampleDownload", (p, _) => p.HasSampleDownload, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
            new PropertyByName<Product>("SampleDownloadId", (p, _) => p.SampleDownloadId, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
            new PropertyByName<Product>("HasUserAgreement", (p, _) => p.HasUserAgreement, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
            new PropertyByName<Product>("UserAgreementText", (p, _) => p.UserAgreementText, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
            new PropertyByName<Product>("IsRecurring", (p, _) => p.IsRecurring, await IgnoreExportProductPropertyAsync(p => p.RecurringProduct)),
            new PropertyByName<Product>("RecurringCycleLength", (p, _) => p.RecurringCycleLength, await IgnoreExportProductPropertyAsync(p => p.RecurringProduct)),
            new PropertyByName<Product>("RecurringCyclePeriod", (p, _) => p.RecurringCyclePeriodId, await IgnoreExportProductPropertyAsync(p => p.RecurringProduct))
            {
                DropDownElements = await RecurringProductCyclePeriod.Days.ToSelectListAsync(useLocalization: false),
                AllowBlank = true
            },
            new PropertyByName<Product>("RecurringTotalCycles", (p, _) => p.RecurringTotalCycles, await IgnoreExportProductPropertyAsync(p => p.RecurringProduct)),
            new PropertyByName<Product>("IsRental", (p, _) => p.IsRental, await IgnoreExportProductPropertyAsync(p => p.IsRental)),
            new PropertyByName<Product>("RentalPriceLength", (p, _) => p.RentalPriceLength, await IgnoreExportProductPropertyAsync(p => p.IsRental)),
            new PropertyByName<Product>("RentalPricePeriod", (p, _) => p.RentalPricePeriodId, await IgnoreExportProductPropertyAsync(p => p.IsRental))
            {
                DropDownElements = await RentalPricePeriod.Days.ToSelectListAsync(useLocalization: false),
                AllowBlank = true
            },
            new PropertyByName<Product>("IsShipEnabled", (p, _) => p.IsShipEnabled),
            new PropertyByName<Product>("IsFreeShipping", (p, _) => p.IsFreeShipping, await IgnoreExportProductPropertyAsync(p => p.FreeShipping)),
            new PropertyByName<Product>("ShipSeparately", (p, _) => p.ShipSeparately, await IgnoreExportProductPropertyAsync(p => p.ShipSeparately)),
            new PropertyByName<Product>("AdditionalShippingCharge", (p, _) => p.AdditionalShippingCharge, await IgnoreExportProductPropertyAsync(p => p.AdditionalShippingCharge)),
            new PropertyByName<Product>("DeliveryDate", (p, _) => p.DeliveryDateId, await IgnoreExportProductPropertyAsync(p => p.DeliveryDate))
            {
                DropDownElements = (await _dateRangeService.GetAllDeliveryDatesAsync()).Select(dd => dd as BaseEntity).ToSelectList(p => (p as DeliveryDate)?.Name ?? string.Empty),
                AllowBlank = true
            },
            new PropertyByName<Product>("IsTaxExempt", (p, _) => p.IsTaxExempt),
            new PropertyByName<Product>("TaxCategory", (p, _) => p.TaxCategoryId)
            {
                DropDownElements = (await _taxCategoryService.GetAllTaxCategoriesAsync()).Select(tc => tc as BaseEntity).ToSelectList(p => (p as TaxCategory)?.Name ?? string.Empty),
                AllowBlank = true
            },
            new PropertyByName<Product>("ManageInventoryMethod", (p, _) => p.ManageInventoryMethodId)
            {
                DropDownElements = await ManageInventoryMethod.DontManageStock.ToSelectListAsync(useLocalization: false)
            },
            new PropertyByName<Product>("ProductAvailabilityRange", (p, _) => p.ProductAvailabilityRangeId, await IgnoreExportProductPropertyAsync(p => p.ProductAvailabilityRange))
            {
                DropDownElements = (await _dateRangeService.GetAllProductAvailabilityRangesAsync()).Select(range => range as BaseEntity).ToSelectList(p => (p as ProductAvailabilityRange)?.Name ?? string.Empty),
                AllowBlank = true
            },
            new PropertyByName<Product>("UseMultipleWarehouses", (p, _) => p.UseMultipleWarehouses, await IgnoreExportProductPropertyAsync(p => p.UseMultipleWarehouses)),
            new PropertyByName<Product>("WarehouseId", (p, _) => p.WarehouseId, await IgnoreExportProductPropertyAsync(p => p.Warehouse)),
            new PropertyByName<Product>("StockQuantity", (p, _) => p.StockQuantity),
            new PropertyByName<Product>("DisplayStockAvailability", (p, _) => p.DisplayStockAvailability, await IgnoreExportProductPropertyAsync(p => p.DisplayStockAvailability)),
            new PropertyByName<Product>("DisplayStockQuantity", (p, _) => p.DisplayStockQuantity, await IgnoreExportProductPropertyAsync(p => p.DisplayStockAvailability)),
            new PropertyByName<Product>("MinStockQuantity", (p, _) => p.MinStockQuantity, await IgnoreExportProductPropertyAsync(p => p.MinimumStockQuantity)),
            new PropertyByName<Product>("LowStockActivity", (p, _) => p.LowStockActivityId, await IgnoreExportProductPropertyAsync(p => p.LowStockActivity))
            {
                DropDownElements = await LowStockActivity.Nothing.ToSelectListAsync(useLocalization: false)
            },
            new PropertyByName<Product>("NotifyAdminForQuantityBelow", (p, _) => p.NotifyAdminForQuantityBelow, await IgnoreExportProductPropertyAsync(p => p.NotifyAdminForQuantityBelow)),
            new PropertyByName<Product>("BackorderMode", (p, _) => p.BackorderModeId, await IgnoreExportProductPropertyAsync(p => p.Backorders))
            {
                DropDownElements = await BackorderMode.NoBackorders.ToSelectListAsync(useLocalization: false)
            },
            new PropertyByName<Product>("AllowBackInStockSubscriptions", (p, _) => p.AllowBackInStockSubscriptions, await IgnoreExportProductPropertyAsync(p => p.AllowBackInStockSubscriptions)),
            new PropertyByName<Product>("OrderMinimumQuantity", (p, _) => p.OrderMinimumQuantity, await IgnoreExportProductPropertyAsync(p => p.MinimumCartQuantity)),
            new PropertyByName<Product>("OrderMaximumQuantity", (p, _) => p.OrderMaximumQuantity, await IgnoreExportProductPropertyAsync(p => p.MaximumCartQuantity)),
            new PropertyByName<Product>("AllowedQuantities", (p, _) => p.AllowedQuantities, await IgnoreExportProductPropertyAsync(p => p.AllowedQuantities)),
            new PropertyByName<Product>("AllowAddingOnlyExistingAttributeCombinations", (p, _) => p.AllowAddingOnlyExistingAttributeCombinations, await IgnoreExportProductPropertyAsync(p => p.AllowAddingOnlyExistingAttributeCombinations)),
            new PropertyByName<Product>("NotReturnable", (p, _) => p.NotReturnable, await IgnoreExportProductPropertyAsync(p => p.NotReturnable)),
            new PropertyByName<Product>("DisableBuyButton", (p, _) => p.DisableBuyButton, await IgnoreExportProductPropertyAsync(p => p.DisableBuyButton)),
            new PropertyByName<Product>("DisableWishlistButton", (p, _) => p.DisableWishlistButton, await IgnoreExportProductPropertyAsync(p => p.DisableWishlistButton)),
            new PropertyByName<Product>("AvailableForPreOrder", (p, _) => p.AvailableForPreOrder, await IgnoreExportProductPropertyAsync(p => p.AvailableForPreOrder)),
            new PropertyByName<Product>("PreOrderAvailabilityStartDateTimeUtc", (p, _) => p.PreOrderAvailabilityStartDateTimeUtc, await IgnoreExportProductPropertyAsync(p => p.AvailableForPreOrder)),
            new PropertyByName<Product>("CallForPrice", (p, _) => p.CallForPrice, await IgnoreExportProductPropertyAsync(p => p.CallForPrice)),
            new PropertyByName<Product>("Price", (p, _) => p.Price),
            new PropertyByName<Product>("OldPrice", (p, _) => p.OldPrice, await IgnoreExportProductPropertyAsync(p => p.OldPrice)),
            new PropertyByName<Product>("ProductCost", (p, _) => p.ProductCost, await IgnoreExportProductPropertyAsync(p => p.ProductCost)),
            new PropertyByName<Product>("CustomerEntersPrice", (p, _) => p.CustomerEntersPrice, await IgnoreExportProductPropertyAsync(p => p.CustomerEntersPrice)),
            new PropertyByName<Product>("MinimumCustomerEnteredPrice", (p, _) => p.MinimumCustomerEnteredPrice, await IgnoreExportProductPropertyAsync(p => p.CustomerEntersPrice)),
            new PropertyByName<Product>("MaximumCustomerEnteredPrice", (p, _) => p.MaximumCustomerEnteredPrice, await IgnoreExportProductPropertyAsync(p => p.CustomerEntersPrice)),
            new PropertyByName<Product>("BasepriceEnabled", (p, _) => p.BasepriceEnabled, await IgnoreExportProductPropertyAsync(p => p.PAngV)),
            new PropertyByName<Product>("BasepriceAmount", (p, _) => p.BasepriceAmount, await IgnoreExportProductPropertyAsync(p => p.PAngV)),
            new PropertyByName<Product>("BasepriceUnit", (p, _) => p.BasepriceUnitId, await IgnoreExportProductPropertyAsync(p => p.PAngV))
            {
                DropDownElements = (await _measureService.GetAllMeasureWeightsAsync()).Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty),
                AllowBlank = true
            },
            new PropertyByName<Product>("BasepriceBaseAmount", (p, _) => p.BasepriceBaseAmount, await IgnoreExportProductPropertyAsync(p => p.PAngV)),
            new PropertyByName<Product>("BasepriceBaseUnit", (p, _) => p.BasepriceBaseUnitId, await IgnoreExportProductPropertyAsync(p => p.PAngV))
            {
                DropDownElements = (await _measureService.GetAllMeasureWeightsAsync()).Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty),
                AllowBlank = true
            },
            new PropertyByName<Product>("MarkAsNew", (p, _) => p.MarkAsNew, await IgnoreExportProductPropertyAsync(p => p.MarkAsNew)),
            new PropertyByName<Product>("MarkAsNewStartDateTimeUtc", (p, _) => p.MarkAsNewStartDateTimeUtc, await IgnoreExportProductPropertyAsync(p => p.MarkAsNew)),
            new PropertyByName<Product>("MarkAsNewEndDateTimeUtc", (p, _) => p.MarkAsNewEndDateTimeUtc, await IgnoreExportProductPropertyAsync(p => p.MarkAsNew)),
            new PropertyByName<Product>("Weight", (p, _) => p.Weight, await IgnoreExportProductPropertyAsync(p => p.Weight)),
            new PropertyByName<Product>("Length", (p, _) => p.Length, await IgnoreExportProductPropertyAsync(p => p.Dimensions)),
            new PropertyByName<Product>("Width", (p, _) => p.Width, await IgnoreExportProductPropertyAsync(p => p.Dimensions)),
            new PropertyByName<Product>("Height", (p, _) => p.Height, await IgnoreExportProductPropertyAsync(p => p.Dimensions)),
            new PropertyByName<Product>("Categories", async (p, _) =>  await GetCategoriesAsync(p)),
            new PropertyByName<Product>("Manufacturers", async (p, _) =>  await GetManufacturersAsync(p), await IgnoreExportProductPropertyAsync(p => p.Manufacturers)),
            new PropertyByName<Product>("ProductTags", async (p, _) =>  await GetProductTagsAsync(p), await IgnoreExportProductPropertyAsync(p => p.ProductTags)),
            new PropertyByName<Product>("IsLimitedToStores", (p, _) => p.LimitedToStores, await ProductIgnoreExportLimitedToStoreAsync()),
            new PropertyByName<Product>("LimitedToStores",async (p, _) =>  await GetLimitedToStoresAsync(p), await ProductIgnoreExportLimitedToStoreAsync()),
            new PropertyByName<Product>("DisplayAttributeCombinationImagesOnly",(p, _) =>  p.DisplayAttributeCombinationImagesOnly, !productAdvancedMode),
            new PropertyByName<Product>("AgeVerification", (p, _) => p.AgeVerification, await IgnoreExportProductPropertyAsync(p => p.AgeVerification)),
            new PropertyByName<Product>("MinimumAgeToPurchase", (p, _) => p.MinimumAgeToPurchase, await IgnoreExportProductPropertyAsync(p => p.AgeVerification)),
            new PropertyByName<Product>("Picture1", async (p, _) => await GetPictureAsync(p, 0)),
            new PropertyByName<Product>("Picture2", async (p, _) => await GetPictureAsync(p, 1)),
            new PropertyByName<Product>("Picture3", async (p, _) => await GetPictureAsync(p, 2))
        };

        var productList = products.ToList();

        if (!_catalogSettings.ExportImportProductAttributes && !_catalogSettings.ExportImportProductSpecificationAttributes)
            return await new PropertyManager<Product>(properties, _catalogSettings).ExportToXlsxAsync(productList);

        //activity log
        await _customerActivityService.InsertActivityAsync("ExportProducts",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportProducts"), productList.Count));

        if (productAdvancedMode || _productEditorSettings.ProductAttributes || _productEditorSettings.SpecificationAttributes || _productEditorSettings.TierPrices)
            return await ExportProductsToXlsxWithAdditionalInfoAsync(properties, localizedProperties, productList, languages);

        return await new PropertyManager<Product>(properties, _catalogSettings, localizedProperties, languages).ExportToXlsxAsync(productList);
    }

    /// <summary>
    /// Export order list to XML
    /// </summary>
    /// <param name="orders">Orders</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    public virtual async Task<string> ExportOrdersToXmlAsync(IList<Order> orders)
    {
        //a vendor should have access only to part of order information
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        var ignore = currentVendor != null;

        var settings = new XmlWriterSettings
        {
            Async = true,
            ConformanceLevel = ConformanceLevel.Auto
        };

        await using var stringWriter = new StringWriter();
        await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

        await xmlWriter.WriteStartDocumentAsync();
        await xmlWriter.WriteStartElementAsync("Orders");
        await xmlWriter.WriteAttributeStringAsync("Version", NopVersion.CURRENT_VERSION);

        foreach (var order in orders)
        {
            await xmlWriter.WriteStartElementAsync("Order");

            await xmlWriter.WriteStringAsync("OrderId", order.Id);
            await xmlWriter.WriteStringAsync("OrderGuid", order.OrderGuid, ignore);
            await xmlWriter.WriteStringAsync("StoreId", order.StoreId);
            await xmlWriter.WriteStringAsync("CustomerId", order.CustomerId, ignore);
            await xmlWriter.WriteStringAsync("OrderStatusId", order.OrderStatusId, ignore);
            await xmlWriter.WriteStringAsync("PaymentStatusId", order.PaymentStatusId, ignore);
            await xmlWriter.WriteStringAsync("ShippingStatusId", order.ShippingStatusId, ignore);
            await xmlWriter.WriteStringAsync("CustomerLanguageId", order.CustomerLanguageId, ignore);
            await xmlWriter.WriteStringAsync("CustomerTaxDisplayTypeId", order.CustomerTaxDisplayTypeId, ignore);
            await xmlWriter.WriteStringAsync("CustomerIp", order.CustomerIp, ignore);
            await xmlWriter.WriteStringAsync("OrderSubtotalInclTax", order.OrderSubtotalInclTax, ignore);
            await xmlWriter.WriteStringAsync("OrderSubtotalExclTax", order.OrderSubtotalExclTax, ignore);
            await xmlWriter.WriteStringAsync("OrderSubTotalDiscountInclTax", order.OrderSubTotalDiscountInclTax, ignore);
            await xmlWriter.WriteStringAsync("OrderSubTotalDiscountExclTax", order.OrderSubTotalDiscountExclTax, ignore);
            await xmlWriter.WriteStringAsync("OrderShippingInclTax", order.OrderShippingInclTax, ignore);
            await xmlWriter.WriteStringAsync("OrderShippingExclTax", order.OrderShippingExclTax, ignore);
            await xmlWriter.WriteStringAsync("PaymentMethodAdditionalFeeInclTax", order.PaymentMethodAdditionalFeeInclTax, ignore);
            await xmlWriter.WriteStringAsync("PaymentMethodAdditionalFeeExclTax", order.PaymentMethodAdditionalFeeExclTax, ignore);
            await xmlWriter.WriteStringAsync("TaxRates", order.TaxRates, ignore);
            await xmlWriter.WriteStringAsync("OrderTax", order.OrderTax, ignore);
            await xmlWriter.WriteStringAsync("OrderTotal", order.OrderTotal, ignore);
            await xmlWriter.WriteStringAsync("RefundedAmount", order.RefundedAmount, ignore);
            await xmlWriter.WriteStringAsync("OrderDiscount", order.OrderDiscount, ignore);
            await xmlWriter.WriteStringAsync("CurrencyRate", order.CurrencyRate);
            await xmlWriter.WriteStringAsync("CustomerCurrencyCode", order.CustomerCurrencyCode);
            await xmlWriter.WriteStringAsync("AffiliateId", order.AffiliateId, ignore);
            await xmlWriter.WriteStringAsync("AllowStoringCreditCardNumber", order.AllowStoringCreditCardNumber, ignore);
            await xmlWriter.WriteStringAsync("CardType", order.CardType, ignore);
            await xmlWriter.WriteStringAsync("CardName", order.CardName, ignore);
            await xmlWriter.WriteStringAsync("CardNumber", order.CardNumber, ignore);
            await xmlWriter.WriteStringAsync("MaskedCreditCardNumber", order.MaskedCreditCardNumber, ignore);
            await xmlWriter.WriteStringAsync("CardCvv2", order.CardCvv2, ignore);
            await xmlWriter.WriteStringAsync("CardExpirationMonth", order.CardExpirationMonth, ignore);
            await xmlWriter.WriteStringAsync("CardExpirationYear", order.CardExpirationYear, ignore);
            await xmlWriter.WriteStringAsync("PaymentMethodSystemName", order.PaymentMethodSystemName, ignore);
            await xmlWriter.WriteStringAsync("AuthorizationTransactionId", order.AuthorizationTransactionId, ignore);
            await xmlWriter.WriteStringAsync("AuthorizationTransactionCode", order.AuthorizationTransactionCode, ignore);
            await xmlWriter.WriteStringAsync("AuthorizationTransactionResult", order.AuthorizationTransactionResult, ignore);
            await xmlWriter.WriteStringAsync("CaptureTransactionId", order.CaptureTransactionId, ignore);
            await xmlWriter.WriteStringAsync("CaptureTransactionResult", order.CaptureTransactionResult, ignore);
            await xmlWriter.WriteStringAsync("SubscriptionTransactionId", order.SubscriptionTransactionId, ignore);
            await xmlWriter.WriteStringAsync("PaidDateUtc", order.PaidDateUtc == null ? string.Empty : order.PaidDateUtc.Value.ToString(CultureInfo.InvariantCulture), ignore);
            await xmlWriter.WriteStringAsync("ShippingMethod", order.ShippingMethod);
            await xmlWriter.WriteStringAsync("ShippingRateComputationMethodSystemName", order.ShippingRateComputationMethodSystemName, ignore);
            await xmlWriter.WriteStringAsync("CustomValuesXml", order.CustomValuesXml, ignore);
            await xmlWriter.WriteStringAsync("VatNumber", order.VatNumber, ignore);
            await xmlWriter.WriteStringAsync("Deleted", order.Deleted, ignore);
            await xmlWriter.WriteStringAsync("CreatedOnUtc", order.CreatedOnUtc);

            if (_orderSettings.ExportWithProducts)
            {
                //a vendor should have access only to his products
                var orderItems = await _orderService.GetOrderItemsAsync(order.Id, vendorId: currentVendor?.Id ?? 0);

                if (orderItems.Any())
                {
                    await xmlWriter.WriteStartElementAsync("OrderItems");
                    foreach (var orderItem in orderItems)
                    {
                        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                        await xmlWriter.WriteStartElementAsync("OrderItem");
                        await xmlWriter.WriteStringAsync("Id", orderItem.Id);
                        await xmlWriter.WriteStringAsync("OrderItemGuid", orderItem.OrderItemGuid);
                        await xmlWriter.WriteStringAsync("Name", product.Name);
                        await xmlWriter.WriteStringAsync("Sku", await _productService.FormatSkuAsync(product, orderItem.AttributesXml));
                        await xmlWriter.WriteStringAsync("PriceExclTax", orderItem.UnitPriceExclTax);
                        await xmlWriter.WriteStringAsync("PriceInclTax", orderItem.UnitPriceInclTax);
                        await xmlWriter.WriteStringAsync("Quantity", orderItem.Quantity);
                        await xmlWriter.WriteStringAsync("DiscountExclTax", orderItem.DiscountAmountExclTax);
                        await xmlWriter.WriteStringAsync("DiscountInclTax", orderItem.DiscountAmountInclTax);
                        await xmlWriter.WriteStringAsync("TotalExclTax", orderItem.PriceExclTax);
                        await xmlWriter.WriteStringAsync("TotalInclTax", orderItem.PriceInclTax);
                        await xmlWriter.WriteEndElementAsync();
                    }

                    await xmlWriter.WriteEndElementAsync();
                }
            }

            //shipments
            var shipments = (await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).OrderBy(x => x.CreatedOnUtc).ToList();
            if (shipments.Any())
            {
                await xmlWriter.WriteStartElementAsync("Shipments");
                foreach (var shipment in shipments)
                {
                    await xmlWriter.WriteStartElementAsync("Shipment");
                    await xmlWriter.WriteElementStringAsync("ShipmentId", null, shipment.Id.ToString());
                    await xmlWriter.WriteElementStringAsync("TrackingNumber", null, shipment.TrackingNumber);
                    await xmlWriter.WriteElementStringAsync("TotalWeight", null, shipment.TotalWeight?.ToString() ?? string.Empty);
                    await xmlWriter.WriteElementStringAsync("ShippedDateUtc", null, shipment.ShippedDateUtc.HasValue ? shipment.ShippedDateUtc.ToString() : string.Empty);
                    await xmlWriter.WriteElementStringAsync("DeliveryDateUtc", null, shipment.DeliveryDateUtc?.ToString() ?? string.Empty);
                    await xmlWriter.WriteElementStringAsync("CreatedOnUtc", null, shipment.CreatedOnUtc.ToString(CultureInfo.InvariantCulture));
                    await xmlWriter.WriteEndElementAsync();
                }

                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();
        }

        await xmlWriter.WriteEndElementAsync();
        await xmlWriter.WriteEndDocumentAsync();
        await xmlWriter.FlushAsync();

        //activity log
        await _customerActivityService.InsertActivityAsync("ExportOrders",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportOrders"), orders.Count));

        return stringWriter.ToString();
    }

    /// <summary>
    /// Export orders to XLSX
    /// </summary>
    /// <param name="orders">Orders</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<byte[]> ExportOrdersToXlsxAsync(IList<Order> orders)
    {
        //a vendor should have access only to part of order information
        var ignore = await _workContext.GetCurrentVendorAsync() != null;

        //lambda expressions for choosing correct order address
        async Task<Address> orderAddress(Order o) => await _addressService.GetAddressByIdAsync((o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) ?? 0);
        async Task<Address> orderBillingAddress(Order o) => await _addressService.GetAddressByIdAsync(o.BillingAddressId);

        //property array
        var properties = new[]
        {
            new PropertyByName<Order>("OrderId", (p, _) => p.Id),
            new PropertyByName<Order>("StoreId", (p, _) => p.StoreId),
            new PropertyByName<Order>("OrderGuid", (p, _) => p.OrderGuid, ignore),
            new PropertyByName<Order>("CustomerId", (p, _) => p.CustomerId, ignore),
            new PropertyByName<Order>("CustomerGuid", async (p, _) => (await _customerService.GetCustomerByIdAsync(p.CustomerId))?.CustomerGuid, ignore),
            new PropertyByName<Order>("OrderStatus", (p, _) => p.OrderStatusId, ignore)
            {
                DropDownElements = await OrderStatus.Pending.ToSelectListAsync(useLocalization: false)
            },
            new PropertyByName<Order>("PaymentStatus", (p, _) => p.PaymentStatusId, ignore)
            {
                DropDownElements = await PaymentStatus.Pending.ToSelectListAsync(useLocalization: false)
            },
            new PropertyByName<Order>("ShippingStatus", (p, _) => p.ShippingStatusId, ignore)
            {
                DropDownElements = await ShippingStatus.ShippingNotRequired.ToSelectListAsync(useLocalization: false)
            },
            new PropertyByName<Order>("OrderSubtotalInclTax", (p, _) => p.OrderSubtotalInclTax, ignore),
            new PropertyByName<Order>("OrderSubtotalExclTax", (p, _) => p.OrderSubtotalExclTax, ignore),
            new PropertyByName<Order>("OrderSubTotalDiscountInclTax", (p, _) => p.OrderSubTotalDiscountInclTax, ignore),
            new PropertyByName<Order>("OrderSubTotalDiscountExclTax", (p, _) => p.OrderSubTotalDiscountExclTax, ignore),
            new PropertyByName<Order>("OrderShippingInclTax", (p, _) => p.OrderShippingInclTax, ignore),
            new PropertyByName<Order>("OrderShippingExclTax", (p, _) => p.OrderShippingExclTax, ignore),
            new PropertyByName<Order>("PaymentMethodAdditionalFeeInclTax", (p, _) => p.PaymentMethodAdditionalFeeInclTax, ignore),
            new PropertyByName<Order>("PaymentMethodAdditionalFeeExclTax", (p, _) => p.PaymentMethodAdditionalFeeExclTax, ignore),
            new PropertyByName<Order>("TaxRates", (p, _) => p.TaxRates, ignore),
            new PropertyByName<Order>("OrderTax", (p, _) => p.OrderTax, ignore),
            new PropertyByName<Order>("OrderTotal", (p, _) => p.OrderTotal, ignore),
            new PropertyByName<Order>("RefundedAmount", (p, _) => p.RefundedAmount, ignore),
            new PropertyByName<Order>("OrderDiscount", (p, _) => p.OrderDiscount, ignore),
            new PropertyByName<Order>("CurrencyRate", (p, _) => p.CurrencyRate),
            new PropertyByName<Order>("CustomerCurrencyCode", (p, _) => p.CustomerCurrencyCode),
            new PropertyByName<Order>("AffiliateId", (p, _) => p.AffiliateId, ignore),
            new PropertyByName<Order>("PaymentMethodSystemName", (p, _) => p.PaymentMethodSystemName, ignore),
            new PropertyByName<Order>("ShippingPickupInStore", (p, _) => p.PickupInStore, ignore),
            new PropertyByName<Order>("ShippingMethod", (p, _) => p.ShippingMethod),
            new PropertyByName<Order>("ShippingRateComputationMethodSystemName", (p, _) => p.ShippingRateComputationMethodSystemName, ignore),
            new PropertyByName<Order>("CustomValuesXml", (p, _) => p.CustomValuesXml, ignore),
            new PropertyByName<Order>("VatNumber", (p, _) => p.VatNumber, ignore),
            new PropertyByName<Order>("CreatedOnUtc", (p, _) => p.CreatedOnUtc),
            new PropertyByName<Order>("BillingFirstName", async (p, _) => (await orderBillingAddress(p))?.FirstName ?? string.Empty),
            new PropertyByName<Order>("BillingLastName", async (p, _) => (await orderBillingAddress(p))?.LastName ?? string.Empty),
            new PropertyByName<Order>("BillingEmail", async (p, _) => (await orderBillingAddress(p))?.Email ?? string.Empty),
            new PropertyByName<Order>("BillingCompany", async (p, _) => (await orderBillingAddress(p))?.Company ?? string.Empty),
            new PropertyByName<Order>("BillingCountry", async (p, _) => (await _countryService.GetCountryByAddressAsync(await orderBillingAddress(p)))?.Name ?? string.Empty),
            new PropertyByName<Order>("BillingCountryCode", async (p, _) => (await _countryService.GetCountryByAddressAsync(await orderBillingAddress(p)))?.TwoLetterIsoCode, ignore),
            new PropertyByName<Order>("BillingStateProvince", async (p, _) => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderBillingAddress(p)))?.Name ?? string.Empty),
            new PropertyByName<Order>("BillingStateProvinceAbbreviation", async (p, _) => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderBillingAddress(p)))?.Abbreviation, ignore),
            new PropertyByName<Order>("BillingCounty", async (p, _) => (await orderBillingAddress(p))?.County ?? string.Empty),
            new PropertyByName<Order>("BillingCity", async (p, _) => (await orderBillingAddress(p))?.City ?? string.Empty),
            new PropertyByName<Order>("BillingAddress1", async (p, _) => (await orderBillingAddress(p))?.Address1 ?? string.Empty),
            new PropertyByName<Order>("BillingAddress2", async (p, _) => (await orderBillingAddress(p))?.Address2 ?? string.Empty),
            new PropertyByName<Order>("BillingZipPostalCode", async (p, _) => (await orderBillingAddress(p))?.ZipPostalCode ?? string.Empty),
            new PropertyByName<Order>("BillingPhoneNumber", async (p, _) => (await orderBillingAddress(p))?.PhoneNumber ?? string.Empty),
            new PropertyByName<Order>("BillingFaxNumber", async (p, _) => (await orderBillingAddress(p))?.FaxNumber ?? string.Empty),
            new PropertyByName<Order>("ShippingFirstName", async (p, _) => (await orderAddress(p))?.FirstName ?? string.Empty),
            new PropertyByName<Order>("ShippingLastName", async (p, _) => (await orderAddress(p))?.LastName ?? string.Empty),
            new PropertyByName<Order>("ShippingEmail", async (p, _) => (await orderAddress(p))?.Email ?? string.Empty),
            new PropertyByName<Order>("ShippingCompany", async (p, _) => (await orderAddress(p))?.Company ?? string.Empty),
            new PropertyByName<Order>("ShippingCountry", async (p, _) => (await _countryService.GetCountryByAddressAsync(await orderAddress(p)))?.Name ?? string.Empty),
            new PropertyByName<Order>("ShippingCountryCode", async (p, _) => (await _countryService.GetCountryByAddressAsync(await orderAddress(p)))?.TwoLetterIsoCode, ignore),
            new PropertyByName<Order>("ShippingStateProvince", async (p, _) => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderAddress(p)))?.Name ?? string.Empty),
            new PropertyByName<Order>("ShippingStateProvinceAbbreviation", async (p, _) => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderAddress(p)))?.Abbreviation, ignore),
            new PropertyByName<Order>("ShippingCounty", async (p, _) => (await orderAddress(p))?.County ?? string.Empty),
            new PropertyByName<Order>("ShippingCity", async (p, _) => (await orderAddress(p))?.City ?? string.Empty),
            new PropertyByName<Order>("ShippingAddress1", async (p, _) => (await orderAddress(p))?.Address1 ?? string.Empty),
            new PropertyByName<Order>("ShippingAddress2", async (p, _) => (await orderAddress(p))?.Address2 ?? string.Empty),
            new PropertyByName<Order>("ShippingZipPostalCode", async (p, _) => (await orderAddress(p))?.ZipPostalCode ?? string.Empty),
            new PropertyByName<Order>("ShippingPhoneNumber", async (p, _) => (await orderAddress(p))?.PhoneNumber ?? string.Empty),
            new PropertyByName<Order>("ShippingFaxNumber", async (p, _) => (await orderAddress(p))?.FaxNumber ?? string.Empty)
        };

        //activity log
        await _customerActivityService.InsertActivityAsync("ExportOrders",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportOrders"), orders.Count));

        return _orderSettings.ExportWithProducts
            ? await ExportOrderToXlsxWithProductsAsync(properties, orders)
            : await new PropertyManager<Order>(properties, _catalogSettings).ExportToXlsxAsync(orders);
    }

    /// <summary>
    /// Export customer list to XLSX
    /// </summary>
    /// <param name="customers">Customers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<byte[]> ExportCustomersToXlsxAsync(IList<Customer> customers)
    {
        var vendors = await _vendorService.GetVendorsByCustomerIdsAsync(customers.Select(c => c.Id).ToArray());

        object getVendor(Customer customer)
        {
            if (!_catalogSettings.ExportImportRelatedEntitiesByName)
                return customer.VendorId;

            return vendors.FirstOrDefault(v => v.Id == customer.VendorId)?.Name ?? string.Empty;
        }

        async Task<object> getCountry(Customer customer)
        {
            var countryId = customer.CountryId;

            if (!_catalogSettings.ExportImportRelatedEntitiesByName)
                return countryId;

            var country = await _countryService.GetCountryByIdAsync(countryId);

            return country?.Name ?? string.Empty;
        }

        async Task<object> getStateProvince(Customer customer)
        {
            var stateProvinceId = customer.StateProvinceId;

            if (!_catalogSettings.ExportImportRelatedEntitiesByName)
                return stateProvinceId;

            var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(stateProvinceId);

            return stateProvince?.Name ?? string.Empty;
        }

        //property manager 
        var manager = new PropertyManager<Customer>(new[]
        {
            new PropertyByName<Customer>("CustomerId", (p, _) => p.Id),
            new PropertyByName<Customer>("CustomerGuid", (p, _) => p.CustomerGuid),
            new PropertyByName<Customer>("Email", (p, _) => p.Email),
            new PropertyByName<Customer>("Username", (p, _) => p.Username),
            new PropertyByName<Customer>("IsTaxExempt", (p, _) => p.IsTaxExempt),
            new PropertyByName<Customer>("AffiliateId", (p, _) => p.AffiliateId),
            new PropertyByName<Customer>("Vendor",  (p, _) => getVendor(p)),
            new PropertyByName<Customer>("Active", (p, _) => p.Active),
            new PropertyByName<Customer>("CustomerRoles",  async (p, _) =>  string.Join(", ",
                (await _customerService.GetCustomerRolesAsync(p)).Select(role => _catalogSettings.ExportImportRelatedEntitiesByName ? role.Name : role.Id.ToString()))),
            new PropertyByName<Customer>("IsGuest", async (p, _) => await _customerService.IsGuestAsync(p)),
            new PropertyByName<Customer>("IsRegistered", async (p, _) => await _customerService.IsRegisteredAsync(p)),
            new PropertyByName<Customer>("IsAdministrator", async (p, _) => await _customerService.IsAdminAsync(p)),
            new PropertyByName<Customer>("IsForumModerator", async (p, _) => await _customerService.IsForumModeratorAsync(p)),
            new PropertyByName<Customer>("IsVendor", async (p, _) => await _customerService.IsVendorAsync(p)),
            new PropertyByName<Customer>("CreatedOnUtc", (p, _) => p.CreatedOnUtc),
            //attributes
            new PropertyByName<Customer>("FirstName", (p, _) => p.FirstName, !_customerSettings.FirstNameEnabled),
            new PropertyByName<Customer>("LastName", (p, _) => p.LastName, !_customerSettings.LastNameEnabled),
            new PropertyByName<Customer>("Gender", (p, _) => p.Gender, !_customerSettings.GenderEnabled),
            new PropertyByName<Customer>("Company", (p, _) => p.Company, !_customerSettings.CompanyEnabled),
            new PropertyByName<Customer>("StreetAddress", (p, _) => p.StreetAddress, !_customerSettings.StreetAddressEnabled),
            new PropertyByName<Customer>("StreetAddress2", (p, _) => p.StreetAddress2, !_customerSettings.StreetAddress2Enabled),
            new PropertyByName<Customer>("ZipPostalCode", (p, _) => p.ZipPostalCode, !_customerSettings.ZipPostalCodeEnabled),
            new PropertyByName<Customer>("City", (p, _) => p.City, !_customerSettings.CityEnabled),
            new PropertyByName<Customer>("County", (p, _) => p.County, !_customerSettings.CountyEnabled),
            new PropertyByName<Customer>("Country",  async (p, _) => await getCountry(p), !_customerSettings.CountryEnabled),
            new PropertyByName<Customer>("StateProvince",  async (p, _) => await getStateProvince(p), !_customerSettings.StateProvinceEnabled),
            new PropertyByName<Customer>("Phone", (p, _) => p.Phone, !_customerSettings.PhoneEnabled),
            new PropertyByName<Customer>("Fax", (p, _) => p.Fax, !_customerSettings.FaxEnabled),
            new PropertyByName<Customer>("VatNumber", (p, _) => p.VatNumber),
            new PropertyByName<Customer>("VatNumberStatus", (p, _) => p.VatNumberStatusId)
            {
                DropDownElements = await VatNumberStatus.Unknown.ToSelectListAsync(useLocalization: false)
            },
            new PropertyByName<Customer>("TimeZone", (p, _) => p.TimeZoneId, !_dateTimeSettings.AllowCustomersToSetTimeZone),
            new PropertyByName<Customer>("AvatarPictureId", async (p, _) => await _genericAttributeService.GetAttributeAsync<int>(p, NopCustomerDefaults.AvatarPictureIdAttribute), !_customerSettings.AllowCustomersToUploadAvatars),
            new PropertyByName<Customer>("ForumPostCount", async (p, _) => await _genericAttributeService.GetAttributeAsync<int>(p, NopCustomerDefaults.ForumPostCountAttribute)),
            new PropertyByName<Customer>("Signature", async (p, _) => await _genericAttributeService.GetAttributeAsync<string>(p, NopCustomerDefaults.SignatureAttribute)),
            new PropertyByName<Customer>("CustomCustomerAttributes", async (p, _) => await GetCustomCustomerAttributesAsync(p)),
            new PropertyByName<Customer>("CustomCustomerAttributesXML", (p, _) => p.CustomCustomerAttributesXML),
            new PropertyByName<Customer>("Password", async (p, _) =>
            {
                if (!_securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword)
                    return string.Empty;

                var password = await _customerService.GetCurrentPasswordAsync(p.Id);

                if(password == null)
                    return string.Empty;

                if (password.PasswordFormat == PasswordFormat.Hashed)
                    return password.Password;

                return string.Empty;
            },  !_securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword),
            new PropertyByName<Customer>("PasswordSalt", async (p, _) =>
            {
                if (!_securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword)
                    return string.Empty;

                var password = await _customerService.GetCurrentPasswordAsync(p.Id);

                if(password == null)
                    return string.Empty;

                if (password.PasswordFormat == PasswordFormat.Hashed)
                    return password.PasswordSalt;

                return string.Empty;

            }, !_securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword),

        }, _catalogSettings);

        //activity log
        await _customerActivityService.InsertActivityAsync("ExportCustomers",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportCustomers"), customers.Count));

        return await manager.ExportToXlsxAsync(customers);
    }

    /// <summary>
    /// Export customer list to XML
    /// </summary>
    /// <param name="customers">Customers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    public virtual async Task<string> ExportCustomersToXmlAsync(IList<Customer> customers)
    {
        var settings = new XmlWriterSettings
        {
            Async = true,
            ConformanceLevel = ConformanceLevel.Auto
        };

        await using var stringWriter = new StringWriter();
        await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

        await xmlWriter.WriteStartDocumentAsync();
        await xmlWriter.WriteStartElementAsync("Customers");
        await xmlWriter.WriteAttributeStringAsync("Version", NopVersion.CURRENT_VERSION);

        foreach (var customer in customers)
        {
            await xmlWriter.WriteStartElementAsync("Customer");
            await xmlWriter.WriteElementStringAsync("CustomerId", null, customer.Id.ToString());
            await xmlWriter.WriteElementStringAsync("CustomerGuid", null, customer.CustomerGuid.ToString());
            await xmlWriter.WriteElementStringAsync("Email", null, customer.Email);
            await xmlWriter.WriteElementStringAsync("Username", null, customer.Username);

            await xmlWriter.WriteElementStringAsync("IsTaxExempt", null, customer.IsTaxExempt.ToString());
            await xmlWriter.WriteElementStringAsync("AffiliateId", null, customer.AffiliateId.ToString());
            await xmlWriter.WriteElementStringAsync("VendorId", null, customer.VendorId.ToString());
            await xmlWriter.WriteElementStringAsync("Active", null, customer.Active.ToString());

            await xmlWriter.WriteElementStringAsync("IsGuest", null, (await _customerService.IsGuestAsync(customer)).ToString());
            await xmlWriter.WriteElementStringAsync("IsRegistered", null, (await _customerService.IsRegisteredAsync(customer)).ToString());
            await xmlWriter.WriteElementStringAsync("IsAdministrator", null, (await _customerService.IsAdminAsync(customer)).ToString());
            await xmlWriter.WriteElementStringAsync("IsForumModerator", null, (await _customerService.IsForumModeratorAsync(customer)).ToString());
            await xmlWriter.WriteElementStringAsync("CreatedOnUtc", null, customer.CreatedOnUtc.ToString(CultureInfo.InvariantCulture));

            await xmlWriter.WriteElementStringAsync("FirstName", null, customer.FirstName);
            await xmlWriter.WriteElementStringAsync("LastName", null, customer.LastName);
            await xmlWriter.WriteElementStringAsync("Gender", null, customer.Gender);
            await xmlWriter.WriteElementStringAsync("Company", null, customer.Company);

            await xmlWriter.WriteElementStringAsync("CountryId", null, customer.CountryId.ToString());
            await xmlWriter.WriteElementStringAsync("StreetAddress", null, customer.StreetAddress);
            await xmlWriter.WriteElementStringAsync("StreetAddress2", null, customer.StreetAddress2);
            await xmlWriter.WriteElementStringAsync("ZipPostalCode", null, customer.ZipPostalCode);
            await xmlWriter.WriteElementStringAsync("City", null, customer.City);
            await xmlWriter.WriteElementStringAsync("County", null, customer.County);
            await xmlWriter.WriteElementStringAsync("StateProvinceId", null, customer.StateProvinceId.ToString());
            await xmlWriter.WriteElementStringAsync("Phone", null, customer.Phone);
            await xmlWriter.WriteElementStringAsync("Fax", null, customer.Fax);
            await xmlWriter.WriteElementStringAsync("VatNumber", null, customer.VatNumber);
            await xmlWriter.WriteElementStringAsync("VatNumberStatusId", null, customer.VatNumberStatusId.ToString());
            await xmlWriter.WriteElementStringAsync("TimeZoneId", null, customer.TimeZoneId);

            await xmlWriter.WriteElementStringAsync("AvatarPictureId", null, (await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute)).ToString());
            await xmlWriter.WriteElementStringAsync("ForumPostCount", null, (await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.ForumPostCountAttribute)).ToString());
            await xmlWriter.WriteElementStringAsync("Signature", null, await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SignatureAttribute));

            if (!string.IsNullOrEmpty(customer.CustomCustomerAttributesXML))
            {
                var selectedCustomerAttributes = new StringReader(customer.CustomCustomerAttributesXML);
                var selectedCustomerAttributesXmlReader = XmlReader.Create(selectedCustomerAttributes);
                await xmlWriter.WriteNodeAsync(selectedCustomerAttributesXmlReader, false);
            }

            await xmlWriter.WriteEndElementAsync();
        }

        await xmlWriter.WriteEndElementAsync();
        await xmlWriter.WriteEndDocumentAsync();
        await xmlWriter.FlushAsync();

        //activity log
        await _customerActivityService.InsertActivityAsync("ExportCustomers",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportCustomers"), customers.Count));

        return stringWriter.ToString();
    }

    /// <summary>
    /// Export newsletter subscribers to TXT
    /// </summary>
    /// <param name="subscriptions">Subscriptions</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in TXT (string) format
    /// </returns>
    public virtual async Task<string> ExportNewsLetterSubscribersToTxtAsync(IList<NewsLetterSubscription> subscriptions)
    {
        ArgumentNullException.ThrowIfNull(subscriptions);

        const char separator = ',';
        var sb = new StringBuilder();

        sb.Append(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Fields.Email"));
        sb.Append(separator);
        sb.Append(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Fields.Active"));
        sb.Append(separator);
        sb.Append(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Fields.SubscriptionType"));
        sb.Append(separator);
        sb.Append(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Fields.Store"));
        sb.Append(separator);
        sb.Append(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Fields.Language"));
        sb.Append(Environment.NewLine);

        foreach (var subscription in subscriptions)
        {
            sb.Append(subscription.Email);
            sb.Append(separator);
            sb.Append(subscription.Active);
            sb.Append(separator);
            sb.Append(subscription.TypeId);
            sb.Append(separator);
            sb.Append(subscription.StoreId);
            sb.Append(separator);
            sb.Append(subscription.LanguageId);
            sb.Append(Environment.NewLine);
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("ExportNewsLetterSubscriptions",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportNewsLetterSubscriptions"), subscriptions.Count));

        return sb.ToString();
    }

    /// <summary>
    /// Export states to TXT
    /// </summary>
    /// <param name="states">States</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in TXT (string) format
    /// </returns>
    public virtual async Task<string> ExportStatesToTxtAsync(IList<StateProvince> states)
    {
        ArgumentNullException.ThrowIfNull(states);

        const char separator = ',';
        var sb = new StringBuilder();
        foreach (var state in states)
        {
            sb.Append((await _countryService.GetCountryByIdAsync(state.CountryId)).TwoLetterIsoCode);
            sb.Append(separator);
            sb.Append(state.Name);
            sb.Append(separator);
            sb.Append(state.Abbreviation);
            sb.Append(separator);
            sb.Append(state.Published);
            sb.Append(separator);
            sb.Append(state.DisplayOrder);
            sb.Append(Environment.NewLine); //new line
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("ExportStates",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportStates"), states.Count));

        return sb.ToString();
    }

    /// <summary>
    /// Export customer info (GDPR request) to XLSX 
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer GDPR info
    /// </returns>
    public virtual async Task<byte[]> ExportCustomerGdprInfoToXlsxAsync(Customer customer, int storeId)
    {
        ArgumentNullException.ThrowIfNull(customer);

        //lambda expressions for choosing correct order address
        async Task<Address> orderAddress(Order o) => await _addressService.GetAddressByIdAsync((o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) ?? 0);
        async Task<Address> orderBillingAddress(Order o) => await _addressService.GetAddressByIdAsync(o.BillingAddressId);

        //customer info and customer attributes
        var customerManager = new PropertyManager<Customer>(new[]
        {
            new PropertyByName<Customer>("Email", (p, _) => p.Email),
            new PropertyByName<Customer>("Username", (p, _) => p.Username, !_customerSettings.UsernamesEnabled), 
            //attributes
            new PropertyByName<Customer>("First name", (p, _) => p.FirstName, !_customerSettings.FirstNameEnabled),
            new PropertyByName<Customer>("Last name", (p, _) => p.LastName, !_customerSettings.LastNameEnabled),
            new PropertyByName<Customer>("Gender", (p, _) => p.Gender, !_customerSettings.GenderEnabled),
            new PropertyByName<Customer>("Date of birth", (p, _) => p.DateOfBirth, !_customerSettings.DateOfBirthEnabled),
            new PropertyByName<Customer>("Company", (p, _) => p.Company, !_customerSettings.CompanyEnabled),
            new PropertyByName<Customer>("Street address", (p, _) => p.StreetAddress, !_customerSettings.StreetAddressEnabled),
            new PropertyByName<Customer>("Street address 2", (p, _) => p.StreetAddress2, !_customerSettings.StreetAddress2Enabled),
            new PropertyByName<Customer>("Zip / postal code", (p, _) => p.ZipPostalCode, !_customerSettings.ZipPostalCodeEnabled),
            new PropertyByName<Customer>("City", (p, _) => p.City, !_customerSettings.CityEnabled),
            new PropertyByName<Customer>("County", (p, _) => p.County, !_customerSettings.CountyEnabled),
            new PropertyByName<Customer>("Country", async (p, _) => (await _countryService.GetCountryByIdAsync(p.CountryId))?.Name ?? string.Empty, !_customerSettings.CountryEnabled),
            new PropertyByName<Customer>("State province", async (p, _) => (await _stateProvinceService.GetStateProvinceByIdAsync(p.StateProvinceId))?.Name ?? string.Empty, !(_customerSettings.StateProvinceEnabled && _customerSettings.CountryEnabled)),
            new PropertyByName<Customer>("Phone", (p, _) => p.Phone, !_customerSettings.PhoneEnabled),
            new PropertyByName<Customer>("Fax", (p, _) => p.Fax, !_customerSettings.FaxEnabled),
            new PropertyByName<Customer>("Customer attributes",  async (p, _) => await GetCustomCustomerAttributesAsync(p))
        }, _catalogSettings);

        //customer orders
        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
        var orderManager = new PropertyManager<Order>(new[]
        {
            new PropertyByName<Order>("Order Number", (p, _) => p.CustomOrderNumber),
            new PropertyByName<Order>("Order status", async (p, _) => await _localizationService.GetLocalizedEnumAsync(p.OrderStatus)),
            new PropertyByName<Order>("Order total", async (p, _) => await _priceFormatter.FormatPriceAsync(_currencyService.ConvertCurrency(p.OrderTotal, p.CurrencyRate), true, p.CustomerCurrencyCode, false, currentLanguage.Id)),
            new PropertyByName<Order>("Shipping method", (p, _) => p.ShippingMethod),
            new PropertyByName<Order>("Created on", async (p, _) => (await _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc)).ToString("D")),
            new PropertyByName<Order>("Billing first name", async (p, _) => (await orderBillingAddress(p))?.FirstName ?? string.Empty),
            new PropertyByName<Order>("Billing last name", async (p, _) => (await orderBillingAddress(p))?.LastName ?? string.Empty),
            new PropertyByName<Order>("Billing email", async (p, _) => (await orderBillingAddress(p))?.Email ?? string.Empty),
            new PropertyByName<Order>("Billing company", async (p, _) => (await orderBillingAddress(p))?.Company ?? string.Empty, !_addressSettings.CompanyEnabled),
            new PropertyByName<Order>("Billing country", async (p, _) => await _countryService.GetCountryByAddressAsync(await orderBillingAddress(p)) is Country country ? await _localizationService.GetLocalizedAsync(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
            new PropertyByName<Order>("Billing state province", async (p, _) => await _stateProvinceService.GetStateProvinceByAddressAsync(await orderBillingAddress(p)) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
            new PropertyByName<Order>("Billing county", async (p, _) => (await orderBillingAddress(p))?.County ?? string.Empty, !_addressSettings.CountyEnabled),
            new PropertyByName<Order>("Billing city", async (p, _) => (await orderBillingAddress(p))?.City ?? string.Empty, !_addressSettings.CityEnabled),
            new PropertyByName<Order>("Billing address 1", async (p, _) => (await orderBillingAddress(p))?.Address1 ?? string.Empty, !_addressSettings.StreetAddressEnabled),
            new PropertyByName<Order>("Billing address 2", async (p, _) => (await orderBillingAddress(p))?.Address2 ?? string.Empty, !_addressSettings.StreetAddress2Enabled),
            new PropertyByName<Order>("Billing zip postal code", async (p, _) => (await orderBillingAddress(p))?.ZipPostalCode ?? string.Empty, !_addressSettings.ZipPostalCodeEnabled),
            new PropertyByName<Order>("Billing phone number", async (p, _) => (await orderBillingAddress(p))?.PhoneNumber ?? string.Empty, !_addressSettings.PhoneEnabled),
            new PropertyByName<Order>("Billing fax number", async (p, _) => (await orderBillingAddress(p))?.FaxNumber ?? string.Empty, !_addressSettings.FaxEnabled),
            new PropertyByName<Order>("Shipping first name", async (p, _) => (await orderAddress(p))?.FirstName ?? string.Empty),
            new PropertyByName<Order>("Shipping last name", async (p, _) => (await orderAddress(p))?.LastName ?? string.Empty),
            new PropertyByName<Order>("Shipping email", async (p, _) => (await orderAddress(p))?.Email ?? string.Empty),
            new PropertyByName<Order>("Shipping company", async (p, _) => (await orderAddress(p))?.Company ?? string.Empty, !_addressSettings.CompanyEnabled),
            new PropertyByName<Order>("Shipping country", async (p, _) => await _countryService.GetCountryByAddressAsync(await orderAddress(p)) is Country country ? await _localizationService.GetLocalizedAsync(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
            new PropertyByName<Order>("Shipping state province", async (p, _) => await _stateProvinceService.GetStateProvinceByAddressAsync(await orderAddress(p)) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
            new PropertyByName<Order>("Shipping county", async (p, _) => (await orderAddress(p))?.County ?? string.Empty, !_addressSettings.CountyEnabled),
            new PropertyByName<Order>("Shipping city", async (p, _) => (await orderAddress(p))?.City ?? string.Empty, !_addressSettings.CityEnabled),
            new PropertyByName<Order>("Shipping address 1", async (p, _) => (await orderAddress(p))?.Address1 ?? string.Empty, !_addressSettings.StreetAddressEnabled),
            new PropertyByName<Order>("Shipping address 2", async (p, _) => (await orderAddress(p))?.Address2 ?? string.Empty, !_addressSettings.StreetAddress2Enabled),
            new PropertyByName<Order>("Shipping zip postal code",
                async (p, _) => (await orderAddress(p))?.ZipPostalCode ?? string.Empty, !_addressSettings.ZipPostalCodeEnabled),
            new PropertyByName<Order>("Shipping phone number", async (p, _) => (await orderAddress(p))?.PhoneNumber ?? string.Empty, !_addressSettings.PhoneEnabled),
            new PropertyByName<Order>("Shipping fax number", async (p, _) => (await orderAddress(p))?.FaxNumber ?? string.Empty, !_addressSettings.FaxEnabled)
        }, _catalogSettings);

        var orderItemsManager = new PropertyManager<OrderItem>(new[]
        {
            new PropertyByName<OrderItem>("SKU", async (oi, _) => await _productService.FormatSkuAsync(await _productService.GetProductByIdAsync(oi.ProductId), oi.AttributesXml)),
            new PropertyByName<OrderItem>("Name", async (oi, _) => await _localizationService.GetLocalizedAsync(await _productService.GetProductByIdAsync(oi.ProductId), p => p.Name)),
            new PropertyByName<OrderItem>("Price", async (oi, _) => await _priceFormatter.FormatPriceAsync(_currencyService.ConvertCurrency((await _orderService.GetOrderByIdAsync(oi.OrderId)).CustomerTaxDisplayType == TaxDisplayType.IncludingTax ? oi.UnitPriceInclTax : oi.UnitPriceExclTax, (await _orderService.GetOrderByIdAsync(oi.OrderId)).CurrencyRate), true, (await _orderService.GetOrderByIdAsync(oi.OrderId)).CustomerCurrencyCode, false, currentLanguage.Id)),
            new PropertyByName<OrderItem>("Quantity", (oi, _) => oi.Quantity),
            new PropertyByName<OrderItem>("Total", async (oi, _) => await _priceFormatter.FormatPriceAsync((await _orderService.GetOrderByIdAsync(oi.OrderId)).CustomerTaxDisplayType == TaxDisplayType.IncludingTax ? oi.PriceInclTax : oi.PriceExclTax))
        }, _catalogSettings);

        var orders = await _orderService.SearchOrdersAsync(customerId: customer.Id);

        //customer addresses
        var addressManager = new PropertyManager<Address>(new[]
        {
            new PropertyByName<Address>("First name", (p, _) => p.FirstName),
            new PropertyByName<Address>("Last name", (p, _) => p.LastName),
            new PropertyByName<Address>("Email", (p, _) => p.Email),
            new PropertyByName<Address>("Company", (p, _) => p.Company, !_addressSettings.CompanyEnabled),
            new PropertyByName<Address>("Country", async (p, _) => await _countryService.GetCountryByAddressAsync(p) is Country country ? await _localizationService.GetLocalizedAsync(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
            new PropertyByName<Address>("State province", async (p, _) => await _stateProvinceService.GetStateProvinceByAddressAsync(p) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
            new PropertyByName<Address>("County", (p, _) => p.County, !_addressSettings.CountyEnabled),
            new PropertyByName<Address>("City", (p, _) => p.City, !_addressSettings.CityEnabled),
            new PropertyByName<Address>("Address 1", (p, _) => p.Address1, !_addressSettings.StreetAddressEnabled),
            new PropertyByName<Address>("Address 2", (p, _) => p.Address2, !_addressSettings.StreetAddress2Enabled),
            new PropertyByName<Address>("Zip / postal code", (p, _) => p.ZipPostalCode, !_addressSettings.ZipPostalCodeEnabled),
            new PropertyByName<Address>("Phone number", (p, _) => p.PhoneNumber, !_addressSettings.PhoneEnabled),
            new PropertyByName<Address>("Fax number", (p, _) => p.FaxNumber, !_addressSettings.FaxEnabled),
            new PropertyByName<Address>("Custom attributes", async (p, _) => await _customerAttributeFormatter.FormatAttributesAsync(p.CustomAttributes, ";"))
        }, _catalogSettings);

        //customer private messages
        var privateMessageManager = new PropertyManager<PrivateMessage>(new[]
        {
            new PropertyByName<PrivateMessage>("From", async (pm, _) => await _customerService.GetCustomerByIdAsync(pm.FromCustomerId) is Customer cFrom ? (_customerSettings.UsernamesEnabled ? cFrom.Username : cFrom.Email) : string.Empty),
            new PropertyByName<PrivateMessage>("To", async (pm, _) => await _customerService.GetCustomerByIdAsync(pm.ToCustomerId) is Customer cTo ? (_customerSettings.UsernamesEnabled ? cTo.Username : cTo.Email) : string.Empty),
            new PropertyByName<PrivateMessage>("Subject", (pm, _) => pm.Subject),
            new PropertyByName<PrivateMessage>("Text", (pm, _) => pm.Text),
            new PropertyByName<PrivateMessage>("Created on", async (pm, _) => (await _dateTimeHelper.ConvertToUserTimeAsync(pm.CreatedOnUtc, DateTimeKind.Utc)).ToString("D"))
        }, _catalogSettings);

        List<PrivateMessage> pmList = null;
        if (_forumSettings.AllowPrivateMessages)
        {
            pmList = (await _forumService.GetAllPrivateMessagesAsync(storeId, customer.Id, 0, null, null, null, null)).ToList();
            pmList.AddRange((await _forumService.GetAllPrivateMessagesAsync(storeId, 0, customer.Id, null, null, null, null)).ToList());
        }

        //customer GDPR logs
        var gdprLogManager = new PropertyManager<GdprLog>(new[]
        {
            new PropertyByName<GdprLog>("Request type", async (log, _) => await _localizationService.GetLocalizedEnumAsync(log.RequestType)),
            new PropertyByName<GdprLog>("Request details", (log, _) => log.RequestDetails),
            new PropertyByName<GdprLog>("Created on", async (log, _) => (await _dateTimeHelper.ConvertToUserTimeAsync(log.CreatedOnUtc, DateTimeKind.Utc)).ToString("D"))
        }, _catalogSettings);

        var gdprLog = await _gdprService.GetAllLogAsync(customer.Id);

        await using var stream = new MemoryStream();
        // ok, we can run the real code of the sample now
        using (var workbook = new XLWorkbook())
        {
            // uncomment this line if you want the XML written out to the outputDir
            //xlPackage.DebugMode = true; 

            // get handles to the worksheets
            // Worksheet names cannot be more than 31 characters
            var customerInfoWorksheet = workbook.Worksheets.Add("Customer info");
            var fWorksheet = workbook.Worksheets.Add("DataForFilters");
            fWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;

            //customer info and customer attributes
            var customerInfoRow = 2;
            customerManager.CurrentObject = customer;
            customerManager.WriteDefaultCaption(customerInfoWorksheet);
            await customerManager.WriteDefaultToXlsxAsync(customerInfoWorksheet, customerInfoRow);

            //customer addresses
            if (await _customerService.GetAddressesByCustomerIdAsync(customer.Id) is IList<Address> addresses && addresses.Any())
            {
                customerInfoRow += 2;

                var cell = customerInfoWorksheet.Row(customerInfoRow).Cell(1);
                cell.Value = "Address List";
                customerInfoRow += 1;
                addressManager.SetCaptionStyle(cell);
                addressManager.WriteDefaultCaption(customerInfoWorksheet, customerInfoRow);

                foreach (var customerAddress in addresses)
                {
                    customerInfoRow += 1;
                    addressManager.CurrentObject = customerAddress;
                    await addressManager.WriteDefaultToXlsxAsync(customerInfoWorksheet, customerInfoRow);
                }
            }

            //customer orders
            if (orders.Any())
            {
                var ordersWorksheet = workbook.Worksheets.Add("Orders");

                orderManager.WriteDefaultCaption(ordersWorksheet);

                var orderRow = 1;

                foreach (var order in orders)
                {
                    orderRow += 1;
                    orderManager.CurrentObject = order;
                    await orderManager.WriteDefaultToXlsxAsync(ordersWorksheet, orderRow);

                    //products
                    var orederItems = await _orderService.GetOrderItemsAsync(order.Id);

                    if (!orederItems.Any())
                        continue;

                    orderRow += 1;

                    orderItemsManager.WriteDefaultCaption(ordersWorksheet, orderRow, 2);
                    ordersWorksheet.Row(orderRow).OutlineLevel = 1;
                    ordersWorksheet.Row(orderRow).Collapse();

                    foreach (var orederItem in orederItems)
                    {
                        orderRow++;
                        orderItemsManager.CurrentObject = orederItem;
                        await orderItemsManager.WriteDefaultToXlsxAsync(ordersWorksheet, orderRow, 2, fWorksheet);
                        ordersWorksheet.Row(orderRow).OutlineLevel = 1;
                        ordersWorksheet.Row(orderRow).Collapse();
                    }
                }
            }

            //customer private messages
            if (pmList?.Any() ?? false)
            {
                var privateMessageWorksheet = workbook.Worksheets.Add("Private messages");
                privateMessageManager.WriteDefaultCaption(privateMessageWorksheet);

                var privateMessageRow = 1;

                foreach (var privateMessage in pmList)
                {
                    privateMessageRow += 1;

                    privateMessageManager.CurrentObject = privateMessage;
                    await privateMessageManager.WriteDefaultToXlsxAsync(privateMessageWorksheet, privateMessageRow);
                }
            }

            //customer GDPR logs
            if (gdprLog.Any())
            {
                var gdprLogWorksheet = workbook.Worksheets.Add("GDPR requests (log)");
                gdprLogManager.WriteDefaultCaption(gdprLogWorksheet);

                var gdprLogRow = 1;

                foreach (var log in gdprLog)
                {
                    gdprLogRow += 1;

                    gdprLogManager.CurrentObject = log;
                    await gdprLogManager.WriteDefaultToXlsxAsync(gdprLogWorksheet, gdprLogRow);
                }
            }

            workbook.SaveAs(stream);
        }

        return stream.ToArray();
    }

    #endregion
}