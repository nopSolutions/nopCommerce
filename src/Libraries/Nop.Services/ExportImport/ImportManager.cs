using ClosedXML.Excel;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport.Help;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;

namespace Nop.Services.ExportImport;

/// <summary>
/// Import manager
/// </summary>
public partial class ImportManager : IImportManager
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAddressService _addressService;
    protected readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
    protected readonly ICategoryService _categoryService;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly ICustomNumberFormatter _customNumberFormatter;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IDateRangeService _dateRangeService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly ILogger _logger;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IMeasureService _measureService;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IOrderService _orderService;
    protected readonly IPictureService _pictureService;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly IProductTagService _productTagService;
    protected readonly IProductTemplateService _productTemplateService;
    protected readonly IServiceScopeFactory _serviceScopeFactory;
    protected readonly IShippingService _shippingService;
    protected readonly ISpecificationAttributeService _specificationAttributeService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IStoreService _storeService;
    protected readonly ITaxCategoryService _taxCategoryService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVendorService _vendorService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly SecuritySettings _securitySettings;
    protected readonly TaxSettings _taxSettings;
    protected readonly VendorSettings _vendorSettings;
    private static readonly string[] _separator = [">>"];

    #endregion

    #region Ctor

    public ImportManager(CatalogSettings catalogSettings,
        IAddressService addressService,
        IBackInStockSubscriptionService backInStockSubscriptionService,
        ICategoryService categoryService,
        ICountryService countryService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        ICustomNumberFormatter customNumberFormatter,
        INopDataProvider dataProvider,
        IDateRangeService dateRangeService,
        IGenericAttributeService genericAttributeService,
        IHttpClientFactory httpClientFactory,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        ILogger logger,
        IManufacturerService manufacturerService,
        IMeasureService measureService,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        INopFileProvider fileProvider,
        IOrderService orderService,
        IPictureService pictureService,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IProductTagService productTagService,
        IProductTemplateService productTemplateService,
        IServiceScopeFactory serviceScopeFactory,
        IShippingService shippingService,
        ISpecificationAttributeService specificationAttributeService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IStoreService storeService,
        ITaxCategoryService taxCategoryService,
        IUrlRecordService urlRecordService,
        IVendorService vendorService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        SecuritySettings securitySettings,
        TaxSettings taxSettings,
        VendorSettings vendorSettings)
    {
        _addressService = addressService;
        _backInStockSubscriptionService = backInStockSubscriptionService;
        _catalogSettings = catalogSettings;
        _categoryService = categoryService;
        _countryService = countryService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _customNumberFormatter = customNumberFormatter;
        _dataProvider = dataProvider;
        _dateRangeService = dateRangeService;
        _genericAttributeService = genericAttributeService;
        _httpClientFactory = httpClientFactory;
        _fileProvider = fileProvider;
        _languageService = languageService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _logger = logger;
        _manufacturerService = manufacturerService;
        _measureService = measureService;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _orderService = orderService;
        _pictureService = pictureService;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _productTagService = productTagService;
        _productTemplateService = productTemplateService;
        _serviceScopeFactory = serviceScopeFactory;
        _shippingService = shippingService;
        _specificationAttributeService = specificationAttributeService;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _storeService = storeService;
        _taxCategoryService = taxCategoryService;
        _urlRecordService = urlRecordService;
        _vendorService = vendorService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _securitySettings = securitySettings;
        _taxSettings = taxSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Utilities

    protected virtual ExportedAttributeType GetTypeOfExportedAttribute(IXLWorksheet defaultWorksheet, List<IXLWorksheet> localizedWorksheets, PropertyManager<ExportProductAttribute, Language> productAttributeManager, PropertyManager<ExportSpecificationAttribute, Language> specificationAttributeManager, int iRow)
    {
        productAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, iRow, ExportProductAttribute.ProductAttributeCellOffset);

        if (productAttributeManager.IsCaption)
        {
            foreach (var worksheet in localizedWorksheets)
                productAttributeManager.ReadLocalizedFromXlsx(worksheet, iRow, ExportProductAttribute.ProductAttributeCellOffset);

            return ExportedAttributeType.ProductAttribute;
        }

        specificationAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, iRow, ExportProductAttribute.ProductAttributeCellOffset);

        if (specificationAttributeManager.IsCaption)
        {
            foreach (var worksheet in localizedWorksheets)
                specificationAttributeManager.ReadLocalizedFromXlsx(worksheet, iRow, ExportProductAttribute.ProductAttributeCellOffset);

            return ExportedAttributeType.SpecificationAttribute;
        }

        return ExportedAttributeType.NotSpecified;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SetOutLineForSpecificationAttributeRowAsync(object cellValue, IXLWorksheet worksheet, int endRow)
    {
        var attributeType = (cellValue ?? string.Empty).ToString();

        if (attributeType.Equals("AttributeType", StringComparison.InvariantCultureIgnoreCase))
        {
            worksheet.Row(endRow).OutlineLevel = 1;
        }
        else
        {
            if ((await SpecificationAttributeType.Option.ToSelectListAsync(useLocalization: false))
                .Any(p => p.Text.Equals(attributeType, StringComparison.InvariantCultureIgnoreCase)))
                worksheet.Row(endRow).OutlineLevel = 1;
            else if (int.TryParse(attributeType, out var attributeTypeId) && Enum.IsDefined(typeof(SpecificationAttributeType), attributeTypeId))
                worksheet.Row(endRow).OutlineLevel = 1;
        }
    }

    protected virtual void CopyDataToNewFile(ImportProductMetadata metadata, IXLWorksheet worksheet, string filePath, int startRow, int endRow, int endCell)
    {
        using var workbook = new XLWorkbook();

        // get handles to the worksheets
        var outWorksheet = workbook.Worksheets.Add(nameof(Product));
        metadata.Manager.WriteDefaultCaption(outWorksheet);
        var outRow = 2;
        for (var row = startRow; row <= endRow; row++)
        {
            outWorksheet.Row(outRow).OutlineLevel = worksheet.Row(row).OutlineLevel;

            for (var cell = 1; cell <= endCell; cell++)
                outWorksheet.Row(outRow).Cell(cell).Value = worksheet.Row(row).Cell(cell).Value;

            outRow += 1;
        }

        workbook.SaveAs(filePath);
    }

    protected virtual int GetColumnIndex(string[] properties, string columnName)
    {
        ArgumentNullException.ThrowIfNull(properties);
        ArgumentNullException.ThrowIfNull(columnName);

        for (var i = 0; i < properties.Length; i++)
            if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                return i + 1; //excel indexes start from 1
        return 0;
    }

    protected virtual string GetMimeTypeFromFilePath(string filePath)
    {
        new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var mimeType);

        //set to jpeg in case mime type cannot be found
        return mimeType ?? _pictureService.GetPictureContentTypeByFileExtension(_fileProvider.GetFileExtension(filePath));
    }

    /// <summary>
    /// Creates or loads the image
    /// </summary>
    /// <param name="picturePath">The path to the image file</param>
    /// <param name="name">The name of the object</param>
    /// <param name="picId">Image identifier, may be null</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the image or null if the image has not changed
    /// </returns>
    protected virtual async Task<Picture> LoadPictureAsync(string picturePath, string name, int? picId = null)
    {
        if (string.IsNullOrEmpty(picturePath) || !_fileProvider.FileExists(picturePath))
            return null;

        var mimeType = GetMimeTypeFromFilePath(picturePath);
        if (string.IsNullOrEmpty(mimeType))
            return null;

        var newPictureBinary = await _fileProvider.ReadAllBytesAsync(picturePath);
        var pictureAlreadyExists = false;
        if (picId != null)
        {
            //compare with existing product pictures
            var existingPicture = await _pictureService.GetPictureByIdAsync(picId.Value);
            if (existingPicture != null)
            {
                var existingBinary = await _pictureService.LoadPictureBinaryAsync(existingPicture);
                //picture binary after validation (like in database)
                var validatedPictureBinary = await _pictureService.ValidatePictureAsync(newPictureBinary, mimeType, name);
                if (existingBinary.SequenceEqual(validatedPictureBinary) ||
                    existingBinary.SequenceEqual(newPictureBinary))
                {
                    pictureAlreadyExists = true;
                }
            }
        }

        if (pictureAlreadyExists)
            return null;

        var newPicture = await _pictureService.InsertPictureAsync(newPictureBinary, mimeType, await _pictureService.GetPictureSeNameAsync(name));
        return newPicture;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task LogPictureInsertErrorAsync(string picturePath, Exception ex)
    {
        var extension = _fileProvider.GetFileExtension(picturePath);
        var name = _fileProvider.GetFileNameWithoutExtension(picturePath);

        var point = string.IsNullOrEmpty(extension) ? string.Empty : ".";
        var fileName = _fileProvider.FileExists(picturePath) ? $"{name}{point}{extension}" : string.Empty;

        await _logger.ErrorAsync($"Insert picture failed (file name: {fileName})", ex);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportProductImagesUsingServicesAsync(IList<ProductPictureMetadata> productPictureMetadata)
    {
        foreach (var product in productPictureMetadata)
        {
            foreach (var picturePath in new[] { product.Picture1Path, product.Picture2Path, product.Picture3Path })
            {
                if (string.IsNullOrEmpty(picturePath))
                    continue;

                var mimeType = GetMimeTypeFromFilePath(picturePath);
                if (string.IsNullOrEmpty(mimeType))
                    continue;

                var newPictureBinary = await _fileProvider.ReadAllBytesAsync(picturePath);
                var pictureAlreadyExists = false;
                if (!product.IsNew)
                {
                    //compare with existing product pictures
                    var existingPictures = await _pictureService.GetPicturesByProductIdAsync(product.ProductItem.Id);
                    foreach (var existingPicture in existingPictures)
                    {
                        var existingBinary = await _pictureService.LoadPictureBinaryAsync(existingPicture);
                        //picture binary after validation (like in database)
                        var validatedPictureBinary = await _pictureService.ValidatePictureAsync(newPictureBinary, mimeType, picturePath);
                        if (!existingBinary.SequenceEqual(validatedPictureBinary) &&
                            !existingBinary.SequenceEqual(newPictureBinary))
                            continue;
                        //the same picture content
                        pictureAlreadyExists = true;
                        break;
                    }
                }

                if (pictureAlreadyExists)
                    continue;

                try
                {
                    var newPicture = await _pictureService.InsertPictureAsync(newPictureBinary, mimeType, await _pictureService.GetPictureSeNameAsync(product.ProductItem.Name));
                    await _productService.InsertProductPictureAsync(new ProductPicture
                    {
                        //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                        //pictures are duplicated
                        //maybe because entity size is too large
                        PictureId = newPicture.Id,
                        DisplayOrder = 1,
                        ProductId = product.ProductItem.Id
                    });
                    await _productService.UpdateProductAsync(product.ProductItem);
                }
                catch (Exception ex)
                {
                    await LogPictureInsertErrorAsync(picturePath, ex);
                }
            }
        }
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportProductImagesUsingHashAsync(IList<ProductPictureMetadata> productPictureMetadata, IList<Product> allProductsBySku)
    {
        //performance optimization, load all pictures hashes
        //it will only be used if the images are stored in the SQL Server database (not compact)
        var trimByteCount = _dataProvider.SupportedLengthOfBinaryHash - 1;
        var productsImagesIds = await _productService.GetProductsImagesIdsAsync(allProductsBySku.Select(p => p.Id).ToArray());

        var allProductPictureIds = productsImagesIds.SelectMany(p => p.Value);

        var allPicturesHashes = allProductPictureIds.Any() ? await _dataProvider.GetFieldHashesAsync<PictureBinary>(p => allProductPictureIds.Contains(p.PictureId),
            p => p.PictureId, p => p.BinaryData) : new Dictionary<int, string>();

        foreach (var product in productPictureMetadata)
        {
            foreach (var picturePath in new[] { product.Picture1Path, product.Picture2Path, product.Picture3Path })
            {
                if (string.IsNullOrEmpty(picturePath))
                    continue;
                try
                {
                    var mimeType = GetMimeTypeFromFilePath(picturePath);
                    if (string.IsNullOrEmpty(mimeType))
                        continue;

                    var newPictureBinary = await _fileProvider.ReadAllBytesAsync(picturePath);
                    var pictureAlreadyExists = false;
                    var seoFileName = await _pictureService.GetPictureSeNameAsync(product.ProductItem.Name);

                    if (!product.IsNew)
                    {
                        var newImageHash = HashHelper.CreateHash(
                            newPictureBinary,
                            ExportImportDefaults.ImageHashAlgorithm,
                            trimByteCount);

                        var newValidatedImageHash = HashHelper.CreateHash(
                            await _pictureService.ValidatePictureAsync(newPictureBinary, mimeType, seoFileName),
                            ExportImportDefaults.ImageHashAlgorithm,
                            trimByteCount);

                        var imagesIds = productsImagesIds.TryGetValue(product.ProductItem.Id, out var value) ? value : Array.Empty<int>();

                        pictureAlreadyExists = allPicturesHashes.Where(p => imagesIds.Contains(p.Key))
                            .Select(p => p.Value)
                            .Any(p =>
                                p.Equals(newImageHash, StringComparison.OrdinalIgnoreCase) ||
                                p.Equals(newValidatedImageHash, StringComparison.OrdinalIgnoreCase));
                    }

                    if (pictureAlreadyExists)
                        continue;

                    var newPicture = await _pictureService.InsertPictureAsync(newPictureBinary, mimeType, seoFileName);

                    await _productService.InsertProductPictureAsync(new ProductPicture
                    {
                        //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                        //pictures are duplicated
                        //maybe because entity size is too large
                        PictureId = newPicture.Id,
                        DisplayOrder = 1,
                        ProductId = product.ProductItem.Id
                    });

                    await _productService.UpdateProductAsync(product.ProductItem);
                }
                catch (Exception ex)
                {
                    await LogPictureInsertErrorAsync(picturePath, ex);
                }
            }
        }
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<(string seName, bool isParentCategoryExists)> UpdateCategoryByXlsxAsync(Category category, PropertyManager<Category, Language> manager, Dictionary<string, ValueTask<Category>> allCategories, bool isNew)
    {
        var seName = string.Empty;
        var isParentCategoryExists = true;
        var isParentCategorySet = false;

        foreach (var property in manager.GetDefaultProperties)
        {
            switch (property.PropertyName)
            {
                case "Name":
                    category.Name = property.StringValue.Split(_separator, StringSplitOptions.RemoveEmptyEntries).Last().Trim();
                    break;
                case "Description":
                    category.Description = property.StringValue;
                    break;
                case "CategoryTemplateId":
                    category.CategoryTemplateId = property.IntValue;
                    break;
                case "MetaKeywords":
                    category.MetaKeywords = property.StringValue;
                    break;
                case "MetaDescription":
                    category.MetaDescription = property.StringValue;
                    break;
                case "MetaTitle":
                    category.MetaTitle = property.StringValue;
                    break;
                case "ParentCategoryId":
                    if (!isParentCategorySet)
                    {
                        var parentCategory = await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Id == property.IntValue);
                        isParentCategorySet = parentCategory != null;

                        isParentCategoryExists = isParentCategorySet || property.IntValue == 0;

                        category.ParentCategoryId = parentCategory?.Id ?? property.IntValue;
                    }

                    break;
                case "ParentCategoryName":
                    if (_catalogSettings.ExportImportCategoriesUsingCategoryName && !isParentCategorySet)
                    {
                        var categoryName = manager.GetDefaultProperty("ParentCategoryName").StringValue;
                        if (!string.IsNullOrEmpty(categoryName))
                        {
                            var parentCategory = allCategories.TryGetValue(categoryName, out var value)
                                //try find category by full name with all parent category names
                                ? await value
                                //try find category by name
                                : await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Name.Equals(categoryName, StringComparison.InvariantCulture));

                            if (parentCategory != null)
                            {
                                category.ParentCategoryId = parentCategory.Id;
                                isParentCategorySet = true;
                            }
                            else
                            {
                                isParentCategoryExists = false;
                            }
                        }
                    }

                    break;
                case "Picture":
                    var picture = await LoadPictureAsync(manager.GetDefaultProperty("Picture").StringValue, category.Name, isNew ? null : (int?)category.PictureId);
                    if (picture != null)
                        category.PictureId = picture.Id;
                    break;
                case "PageSize":
                    category.PageSize = property.IntValue;
                    break;
                case "AllowCustomersToSelectPageSize":
                    category.AllowCustomersToSelectPageSize = property.BooleanValue;
                    break;
                case "PageSizeOptions":
                    category.PageSizeOptions = property.StringValue;
                    break;
                case "ShowOnHomepage":
                    category.ShowOnHomepage = property.BooleanValue;
                    break;
                case "PriceRangeFiltering":
                    category.PriceRangeFiltering = property.BooleanValue;
                    break;
                case "PriceFrom":
                    category.PriceFrom = property.DecimalValue;
                    break;
                case "PriceTo":
                    category.PriceTo = property.DecimalValue;
                    break;
                case "AutomaticallyCalculatePriceRange":
                    category.ManuallyPriceRange = property.BooleanValue;
                    break;
                case "IncludeInTopMenu":
                    category.IncludeInTopMenu = property.BooleanValue;
                    break;
                case "Published":
                    category.Published = property.BooleanValue;
                    break;
                case "DisplayOrder":
                    category.DisplayOrder = property.IntValue;
                    break;
                case "SeName":
                    seName = property.StringValue;
                    break;
            }
        }

        category.UpdatedOnUtc = DateTime.UtcNow;
        return (seName, isParentCategoryExists);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<(Category category, bool isNew, string curentCategoryBreadCrumb)> GetCategoryFromXlsxAsync(PropertyManager<Category, Language> manager, IXLWorksheet worksheet, int iRow, Dictionary<string, ValueTask<Category>> allCategories)
    {
        manager.ReadDefaultFromXlsx(worksheet, iRow);

        //try get category from database by ID
        var category = await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Id == manager.GetDefaultProperty("Id")?.IntValue);

        if (_catalogSettings.ExportImportCategoriesUsingCategoryName && category == null)
        {
            var categoryName = manager.GetDefaultProperty("Name").StringValue;
            if (!string.IsNullOrEmpty(categoryName))
            {
                category = allCategories.TryGetValue(categoryName, out var value)
                    //try find category by full name with all parent category names
                    ? await value
                    //try find category by name
                    : await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Name.Equals(categoryName, StringComparison.InvariantCulture));
            }
        }

        var isNew = category == null;

        category ??= new Category();

        var curentCategoryBreadCrumb = string.Empty;

        if (isNew)
        {
            category.CreatedOnUtc = DateTime.UtcNow;
            //default values
            category.PageSize = _catalogSettings.DefaultCategoryPageSize;
            category.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
            category.Published = true;
            category.IncludeInTopMenu = true;
            category.AllowCustomersToSelectPageSize = true;
        }
        else
            curentCategoryBreadCrumb = await _categoryService.GetFormattedBreadCrumbAsync(category);

        return (category, isNew, curentCategoryBreadCrumb);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SaveCategoryAsync(bool isNew, Category category, Dictionary<string, ValueTask<Category>> allCategories, string curentCategoryBreadCrumb, bool setSeName, string seName)
    {
        if (isNew)
            await _categoryService.InsertCategoryAsync(category);
        else
            await _categoryService.UpdateCategoryAsync(category);

        var categoryBreadCrumb = await _categoryService.GetFormattedBreadCrumbAsync(category);
        if (!allCategories.ContainsKey(categoryBreadCrumb))
            allCategories.Add(categoryBreadCrumb, new ValueTask<Category>(category));
        if (!string.IsNullOrEmpty(curentCategoryBreadCrumb) && allCategories.ContainsKey(curentCategoryBreadCrumb) &&
            categoryBreadCrumb != curentCategoryBreadCrumb)
            allCategories.Remove(curentCategoryBreadCrumb);

        //search engine name
        if (setSeName)
            await _urlRecordService.SaveSlugAsync(category, await _urlRecordService.ValidateSeNameAsync(category, seName, category.Name, true), 0);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportCategoryLocalizedAsync(Category category, WorkbookMetadata<Category> metadata, PropertyManager<Category, Language> manager, int iRow, IList<Language> languages)
    {
        if (!metadata.LocalizedWorksheets.Any())
            return;

        var setSeName = metadata.LocalizedProperties.Any(p => p.PropertyName == "SeName");
        foreach (var language in languages)
        {
            var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
            if (lWorksheet == null)
                continue;

            manager.CurrentLanguage = language;
            manager.ReadLocalizedFromXlsx(lWorksheet, iRow);

            foreach (var property in manager.GetLocalizedProperties)
            {
                string localizedName = null;

                switch (property.PropertyName)
                {
                    case "Name":
                        localizedName = property.StringValue;
                        await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.Name, localizedName, language.Id);
                        break;
                    case "Description":
                        await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.Description, property.StringValue, language.Id);
                        break;
                    case "MetaKeywords":
                        await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.MetaKeywords, property.StringValue, language.Id);
                        break;
                    case "MetaDescription":
                        await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.MetaDescription, property.StringValue, language.Id);
                        break;
                    case "MetaTitle":
                        await _localizedEntityService.SaveLocalizedValueAsync(category, m => m.MetaTitle, property.StringValue, language.Id);
                        break;
                    case "SeName":
                        //search engine name
                        if (setSeName)
                        {
                            var lSeName = await _urlRecordService.ValidateSeNameAsync(category, property.StringValue, localizedName, false);
                            await _urlRecordService.SaveSlugAsync(category, lSeName, language.Id);
                        }
                        break;
                }
            }
        }
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportManufaturerLocalizedAsync(Manufacturer manufacturer, WorkbookMetadata<Manufacturer> metadata, PropertyManager<Manufacturer, Language> manager, int iRow, IList<Language> languages)
    {
        if (!metadata.LocalizedWorksheets.Any())
            return;

        var setSeName = metadata.LocalizedProperties.Any(p => p.PropertyName == "SeName");
        foreach (var language in languages)
        {
            var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
            if (lWorksheet == null)
                continue;

            manager.CurrentLanguage = language;
            manager.ReadLocalizedFromXlsx(lWorksheet, iRow);

            foreach (var property in manager.GetLocalizedProperties)
            {
                string localizedName = null;

                switch (property.PropertyName)
                {
                    case "Name":
                        localizedName = property.StringValue;
                        await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.Name, localizedName, language.Id);
                        break;
                    case "Description":
                        await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.Description, property.StringValue, language.Id);
                        break;
                    case "MetaKeywords":
                        await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.MetaKeywords, property.StringValue, language.Id);
                        break;
                    case "MetaDescription":
                        await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.MetaDescription, property.StringValue, language.Id);
                        break;
                    case "MetaTitle":
                        await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.MetaTitle, property.StringValue, language.Id);
                        break;
                    case "SeName":
                        //search engine name
                        if (setSeName)
                        {
                            var localizedSeName = await _urlRecordService.ValidateSeNameAsync(manufacturer, property.StringValue, localizedName, false);
                            await _urlRecordService.SaveSlugAsync(manufacturer, localizedSeName, language.Id);
                        }
                        break;
                }
            }
        }
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SetOutLineForProductAttributeRowAsync(object cellValue, IXLWorksheet worksheet, int endRow)
    {
        try
        {
            var aid = Convert.ToInt32(cellValue ?? -1);

            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(aid);

            if (productAttribute != null)
                worksheet.Row(endRow).OutlineLevel = 1;
        }
        catch (FormatException)
        {
            if ((cellValue ?? string.Empty).ToString() == "AttributeId")
                worksheet.Row(endRow).OutlineLevel = 1;
        }
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportProductAttributeAsync(ImportProductMetadata metadata, Product lastLoadedProduct, IList<Language> languages, int iRow)
    {
        var productAttributeManager = metadata.ProductAttributeManager;
        if (!_catalogSettings.ExportImportProductAttributes || lastLoadedProduct == null || productAttributeManager.IsCaption)
            return;

        var productAttributeId = productAttributeManager.GetDefaultProperty("AttributeId").IntValue;
        var attributeControlTypeId = productAttributeManager.GetDefaultProperty("AttributeControlType").IntValue;

        var productAttributeValueId = productAttributeManager.GetDefaultProperty("ProductAttributeValueId").IntValue;
        var associatedProductId = productAttributeManager.GetDefaultProperty("AssociatedProductId").IntValue;
        var valueName = productAttributeManager.GetDefaultProperty("ValueName").StringValue;
        var attributeValueTypeId = productAttributeManager.GetDefaultProperty("AttributeValueType").IntValue;
        var colorSquaresRgb = productAttributeManager.GetDefaultProperty("ColorSquaresRgb").StringValue;
        var imageSquaresPictureId = productAttributeManager.GetDefaultProperty("ImageSquaresPictureId").IntValue;
        var priceAdjustment = productAttributeManager.GetDefaultProperty("PriceAdjustment").DecimalValue;
        var priceAdjustmentUsePercentage = productAttributeManager.GetDefaultProperty("PriceAdjustmentUsePercentage").BooleanValue;
        var weightAdjustment = productAttributeManager.GetDefaultProperty("WeightAdjustment").DecimalValue;
        var cost = productAttributeManager.GetDefaultProperty("Cost").DecimalValue;
        var customerEntersQty = productAttributeManager.GetDefaultProperty("CustomerEntersQty").BooleanValue;
        var quantity = productAttributeManager.GetDefaultProperty("Quantity").IntValue;
        var isPreSelected = productAttributeManager.GetDefaultProperty("IsPreSelected").BooleanValue;
        var displayOrder = productAttributeManager.GetDefaultProperty("DisplayOrder").IntValue;
        var pictureIdsStr = productAttributeManager.GetDefaultProperty("PictureIds").StringValue;
        var textPrompt = productAttributeManager.GetDefaultProperty("AttributeTextPrompt").StringValue;
        var isRequired = productAttributeManager.GetDefaultProperty("AttributeIsRequired").BooleanValue;
        var attributeDisplayOrder = productAttributeManager.GetDefaultProperty("AttributeDisplayOrder").IntValue;

        var productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(lastLoadedProduct.Id))
            .FirstOrDefault(pam => pam.ProductAttributeId == productAttributeId);
        var pictureIds = new List<int>();
        if (!string.IsNullOrWhiteSpace(pictureIdsStr))
            pictureIds = Array.ConvertAll(pictureIdsStr.Split(new[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries), int.Parse).ToList();

        if (productAttributeMapping == null)
        {
            //insert mapping
            productAttributeMapping = new ProductAttributeMapping
            {
                ProductId = lastLoadedProduct.Id,
                ProductAttributeId = productAttributeId,
                TextPrompt = textPrompt,
                IsRequired = isRequired,
                AttributeControlTypeId = attributeControlTypeId,
                DisplayOrder = attributeDisplayOrder
            };
            await _productAttributeService.InsertProductAttributeMappingAsync(productAttributeMapping);
        }
        else
        {
            productAttributeMapping.AttributeControlTypeId = attributeControlTypeId;
            productAttributeMapping.TextPrompt = textPrompt;
            productAttributeMapping.IsRequired = isRequired;
            productAttributeMapping.DisplayOrder = attributeDisplayOrder;
            await _productAttributeService.UpdateProductAttributeMappingAsync(productAttributeMapping);
        }

        var pav = (await _productAttributeService.GetProductAttributeValuesAsync(productAttributeMapping.Id))
            .FirstOrDefault(p => p.Id == productAttributeValueId);

        //var pav = await _productAttributeService.GetProductAttributeValueByIdAsync(productAttributeValueId);

        var attributeControlType = (AttributeControlType)attributeControlTypeId;

        async Task saveAttributeValuePicturesAsync(Product product, ProductAttributeValue value)
        {
            var existingValuePictures =
                await _productAttributeService.GetProductAttributeValuePicturesAsync(value.Id);
            var productPictureIds = (await _pictureService.GetPicturesByProductIdAsync(product.Id))
                .Select(p => p.Id).ToList();

            //delete manufacturers
            foreach (var existingValuePicture in existingValuePictures)
                if (pictureIds.Contains(existingValuePicture.PictureId) ||
                    !productPictureIds.Contains(existingValuePicture.PictureId))
                    await _productAttributeService.DeleteProductAttributeValuePictureAsync(existingValuePicture);

            //add manufacturers
            foreach (var pictureId in pictureIds)
            {
                if (!productPictureIds.Contains(pictureId))
                    continue;

                if (_productAttributeService.FindProductAttributeValuePicture(existingValuePictures, value.Id,
                        pictureId) == null)
                {
                    await _productAttributeService.InsertProductAttributeValuePictureAsync(
                        new ProductAttributeValuePicture
                        {
                            ProductAttributeValueId = value.Id,
                            PictureId = pictureId
                        });
                }
            }

            if (!metadata.LocalizedWorksheets.Any())
                return;

            foreach (var language in languages)
            {
                var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws =>
                    ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                if (lWorksheet == null)
                    continue;

                productAttributeManager.CurrentLanguage = language;
                productAttributeManager.ReadLocalizedFromXlsx(lWorksheet, iRow,
                    ExportProductAttribute.ProductAttributeCellOffset);

                valueName = productAttributeManager.GetLocalizedProperty("ValueName").StringValue;
                textPrompt = productAttributeManager.GetLocalizedProperty("AttributeTextPrompt").StringValue;

                await _localizedEntityService.SaveLocalizedValueAsync(pav, p => p.Name, valueName, language.Id);
                await _localizedEntityService.SaveLocalizedValueAsync(productAttributeMapping, p => p.TextPrompt,
                    textPrompt, language.Id);

                switch (attributeControlType)
                {
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    case AttributeControlType.MultilineTextbox:
                    case AttributeControlType.TextBox:
                        if (productAttributeMapping.ValidationRulesAllowed())
                        {
                            var defaultValue = productAttributeManager.GetLocalizedProperty("DefaultValue")
                                ?.StringValue;
                            await _localizedEntityService.SaveLocalizedValueAsync(productAttributeMapping,
                                p => p.DefaultValue, defaultValue, language.Id);
                        }

                        return;
                }
            }
        }

        if (pav == null)
        {
            switch (attributeControlType)
            {
                case AttributeControlType.Datepicker:
                case AttributeControlType.FileUpload:
                case AttributeControlType.MultilineTextbox:
                case AttributeControlType.TextBox:
                    if (productAttributeMapping.ValidationRulesAllowed())
                    {
                        productAttributeMapping.ValidationMinLength = productAttributeManager.GetDefaultProperty("ValidationMinLength")?.IntValueNullable;
                        productAttributeMapping.ValidationMaxLength = productAttributeManager.GetDefaultProperty("ValidationMaxLength")?.IntValueNullable;
                        productAttributeMapping.ValidationFileMaximumSize = productAttributeManager.GetDefaultProperty("ValidationFileMaximumSize")?.IntValueNullable;
                        productAttributeMapping.ValidationFileAllowedExtensions = productAttributeManager.GetDefaultProperty("ValidationFileAllowedExtensions")?.StringValue;
                        productAttributeMapping.DefaultValue = productAttributeManager.GetDefaultProperty("DefaultValue")?.StringValue;

                        await _productAttributeService.UpdateProductAttributeMappingAsync(productAttributeMapping);
                    }

                    return;
            }

            pav = new ProductAttributeValue
            {
                ProductAttributeMappingId = productAttributeMapping.Id,
                AttributeValueType = (AttributeValueType)attributeValueTypeId,
                AssociatedProductId = associatedProductId,
                Name = valueName,
                PriceAdjustment = priceAdjustment,
                PriceAdjustmentUsePercentage = priceAdjustmentUsePercentage,
                WeightAdjustment = weightAdjustment,
                Cost = cost,
                IsPreSelected = isPreSelected,
                DisplayOrder = displayOrder,
                ColorSquaresRgb = colorSquaresRgb,
                ImageSquaresPictureId = imageSquaresPictureId,
                CustomerEntersQty = customerEntersQty,
                Quantity = quantity
            };

            await _productAttributeService.InsertProductAttributeValueAsync(pav);
            await saveAttributeValuePicturesAsync(lastLoadedProduct, pav);
        }
        else
        {
            pav.AttributeValueTypeId = attributeValueTypeId;
            pav.AssociatedProductId = associatedProductId;
            pav.Name = valueName;
            pav.ColorSquaresRgb = colorSquaresRgb;
            pav.ImageSquaresPictureId = imageSquaresPictureId;
            pav.PriceAdjustment = priceAdjustment;
            pav.PriceAdjustmentUsePercentage = priceAdjustmentUsePercentage;
            pav.WeightAdjustment = weightAdjustment;
            pav.Cost = cost;
            pav.CustomerEntersQty = customerEntersQty;
            pav.Quantity = quantity;
            pav.IsPreSelected = isPreSelected;
            pav.DisplayOrder = displayOrder;

            await _productAttributeService.UpdateProductAttributeValueAsync(pav);
            await saveAttributeValuePicturesAsync(lastLoadedProduct, pav);
        }
    }

    /// <returns>A task that represents the asynchronous operation</returns>


    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportSpecificationAttributeAsync(ImportProductMetadata metadata, Product lastLoadedProduct, IList<Language> languages, int iRow)
    {
        var specificationAttributeManager = metadata.SpecificationAttributeManager;
        if (!_catalogSettings.ExportImportProductSpecificationAttributes || lastLoadedProduct == null || specificationAttributeManager.IsCaption)
            return;

        var attributeTypeId = specificationAttributeManager.GetDefaultProperty("AttributeType").IntValue;
        var allowFiltering = specificationAttributeManager.GetDefaultProperty("AllowFiltering").BooleanValue;
        var specificationAttributeOptionId = specificationAttributeManager.GetDefaultProperty("SpecificationAttributeOptionId").IntValue;
        var productId = lastLoadedProduct.Id;
        var customValue = specificationAttributeManager.GetDefaultProperty("CustomValue").StringValue;
        var displayOrder = specificationAttributeManager.GetDefaultProperty("DisplayOrder").IntValue;
        var showOnProductPage = specificationAttributeManager.GetDefaultProperty("ShowOnProductPage").BooleanValue;

        //if specification attribute option isn't set, try to get first of possible specification attribute option for current specification attribute
        if (specificationAttributeOptionId == 0)
        {
            var specificationAttribute = specificationAttributeManager.GetDefaultProperty("SpecificationAttribute").IntValue;
            specificationAttributeOptionId =
                (await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(
                    specificationAttribute))
                .FirstOrDefault()?.Id ?? specificationAttributeOptionId;
        }

        var productSpecificationAttribute = specificationAttributeOptionId == 0
            ? null
            : (await _specificationAttributeService.GetProductSpecificationAttributesAsync(productId, specificationAttributeOptionId)).FirstOrDefault();

        var isNew = productSpecificationAttribute == null;

        if (isNew)
            productSpecificationAttribute = new ProductSpecificationAttribute();

        if (attributeTypeId != (int)SpecificationAttributeType.Option)
            //we allow filtering only for "Option" attribute type
            allowFiltering = false;

        //we don't allow CustomValue for "Option" attribute type
        if (attributeTypeId == (int)SpecificationAttributeType.Option)
            customValue = null;

        productSpecificationAttribute.AttributeTypeId = attributeTypeId;
        productSpecificationAttribute.SpecificationAttributeOptionId = specificationAttributeOptionId;
        productSpecificationAttribute.ProductId = productId;
        productSpecificationAttribute.CustomValue = customValue;
        productSpecificationAttribute.AllowFiltering = allowFiltering;
        productSpecificationAttribute.ShowOnProductPage = showOnProductPage;
        productSpecificationAttribute.DisplayOrder = displayOrder;

        if (isNew)
            await _specificationAttributeService.InsertProductSpecificationAttributeAsync(productSpecificationAttribute);
        else
            await _specificationAttributeService.UpdateProductSpecificationAttributeAsync(productSpecificationAttribute);

        if (!metadata.LocalizedWorksheets.Any())
            return;

        foreach (var language in languages)
        {
            var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
            if (lWorksheet == null)
                continue;

            specificationAttributeManager.CurrentLanguage = language;
            specificationAttributeManager.ReadLocalizedFromXlsx(lWorksheet, iRow, ExportProductAttribute.ProductAttributeCellOffset);

            customValue = specificationAttributeManager.GetLocalizedProperty("CustomValue").StringValue;
            await _localizedEntityService.SaveLocalizedValueAsync(productSpecificationAttribute, p => p.CustomValue, customValue, language.Id);
        }
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<string> DownloadFileAsync(string urlString, IList<string> downloadedFiles)
    {
        if (string.IsNullOrEmpty(urlString))
            return string.Empty;

        if (!Uri.IsWellFormedUriString(urlString, UriKind.Absolute))
            return urlString;

        if (!_catalogSettings.ExportImportAllowDownloadImages)
            return string.Empty;

        //ensure that temp directory is created
        var tempDirectory = _fileProvider.MapPath(ExportImportDefaults.UploadsTempPath);
        _fileProvider.CreateDirectory(tempDirectory);

        var fileName = _fileProvider.GetFileName(urlString);
        if (string.IsNullOrEmpty(fileName))
            return string.Empty;

        var filePath = _fileProvider.Combine(tempDirectory, fileName);
        try
        {
            var client = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
            var fileData = await client.GetByteArrayAsync(urlString);
            await using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                fs.Write(fileData, 0, fileData.Length);

            downloadedFiles?.Add(filePath);
            return filePath;
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("Download image failed", ex);
        }

        return string.Empty;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<ImportProductMetadata> PrepareImportProductDataAsync(IXLWorkbook workbook, IList<Language> languages)
    {
        //the columns
        var metadata = GetWorkbookMetadata<Product>(workbook, languages);
        var defaultWorksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;
        var localizedProperties = metadata.LocalizedProperties;

        var manager = new PropertyManager<Product, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

        var productAttributeProperties = new[]
        {
            new PropertyByName<ExportProductAttribute, Language>("AttributeId"),
            new PropertyByName<ExportProductAttribute, Language>("AttributeName"),
            new PropertyByName<ExportProductAttribute, Language>("DefaultValue"),
            new PropertyByName<ExportProductAttribute, Language>("ValidationMinLength"),
            new PropertyByName<ExportProductAttribute, Language>("ValidationMaxLength"),
            new PropertyByName<ExportProductAttribute, Language>("ValidationFileAllowedExtensions"),
            new PropertyByName<ExportProductAttribute, Language>("ValidationFileMaximumSize"),
            new PropertyByName<ExportProductAttribute, Language>("AttributeTextPrompt"),
            new PropertyByName<ExportProductAttribute, Language>("AttributeIsRequired"),
            new PropertyByName<ExportProductAttribute, Language>("AttributeControlType"),
            new PropertyByName<ExportProductAttribute, Language>("AttributeDisplayOrder"),
            new PropertyByName<ExportProductAttribute, Language>("ProductAttributeValueId"),
            new PropertyByName<ExportProductAttribute, Language>("ValueName"),
            new PropertyByName<ExportProductAttribute, Language>("AttributeValueType"),
            new PropertyByName<ExportProductAttribute, Language>("AssociatedProductId"),
            new PropertyByName<ExportProductAttribute, Language>("ColorSquaresRgb"),
            new PropertyByName<ExportProductAttribute, Language>("ImageSquaresPictureId"),
            new PropertyByName<ExportProductAttribute, Language>("PriceAdjustment"),
            new PropertyByName<ExportProductAttribute, Language>("PriceAdjustmentUsePercentage"),
            new PropertyByName<ExportProductAttribute, Language>("WeightAdjustment"),
            new PropertyByName<ExportProductAttribute, Language>("Cost"),
            new PropertyByName<ExportProductAttribute, Language>("CustomerEntersQty"),
            new PropertyByName<ExportProductAttribute, Language>("Quantity"),
            new PropertyByName<ExportProductAttribute, Language>("IsPreSelected"),
            new PropertyByName<ExportProductAttribute, Language>("DisplayOrder"),
            new PropertyByName<ExportProductAttribute, Language>("PictureId")
        };

        var productAttributeLocalizedProperties = new[]
        {
            new PropertyByName<ExportProductAttribute, Language>("DefaultValue"),
            new PropertyByName<ExportProductAttribute, Language>("AttributeTextPrompt"),
            new PropertyByName<ExportProductAttribute, Language>("ValueName")
        };

        var productAttributeManager = new PropertyManager<ExportProductAttribute, Language>(productAttributeProperties, _catalogSettings, productAttributeLocalizedProperties, languages);

        var specificationAttributeProperties = new[]
        {
            new PropertyByName<ExportSpecificationAttribute, Language>("AttributeType", (p, l) => p.AttributeTypeId),
            new PropertyByName<ExportSpecificationAttribute, Language>("SpecificationAttribute", (p, l) => p.SpecificationAttributeId),
            new PropertyByName<ExportSpecificationAttribute, Language>("CustomValue", (p, l) => p.CustomValue),
            new PropertyByName<ExportSpecificationAttribute, Language>("SpecificationAttributeOptionId", (p, l) => p.SpecificationAttributeOptionId),
            new PropertyByName<ExportSpecificationAttribute, Language>("AllowFiltering", (p, l) => p.AllowFiltering),
            new PropertyByName<ExportSpecificationAttribute, Language>("ShowOnProductPage", (p, l) => p.ShowOnProductPage),
            new PropertyByName<ExportSpecificationAttribute, Language>("DisplayOrder", (p, l) => p.DisplayOrder)
        };

        var specificationAttributeLocalizedProperties = new[]
        {
            new PropertyByName<ExportSpecificationAttribute, Language>("CustomValue")
        };

        var specificationAttributeManager = new PropertyManager<ExportSpecificationAttribute, Language>(specificationAttributeProperties, _catalogSettings, specificationAttributeLocalizedProperties, languages);

        var endRow = 2;
        var allCategories = new List<string>();
        var allSku = new List<string>();

        var tempProperty = manager.GetDefaultProperty("Categories");
        var categoryCellNum = tempProperty?.PropertyOrderPosition ?? -1;

        tempProperty = manager.GetDefaultProperty("SKU");
        var skuCellNum = tempProperty?.PropertyOrderPosition ?? -1;

        var allManufacturers = new List<string>();
        tempProperty = manager.GetDefaultProperty("Manufacturers");
        var manufacturerCellNum = tempProperty?.PropertyOrderPosition ?? -1;

        var allStores = new List<string>();
        tempProperty = manager.GetDefaultProperty("LimitedToStores");
        var limitedToStoresCellNum = tempProperty?.PropertyOrderPosition ?? -1;

        if (_catalogSettings.ExportImportUseDropdownlistsForAssociatedEntities)
        {
            productAttributeManager.SetSelectList("AttributeControlType", await AttributeControlType.TextBox.ToSelectListAsync(useLocalization: false));
            productAttributeManager.SetSelectList("AttributeValueType", await AttributeValueType.Simple.ToSelectListAsync(useLocalization: false));

            specificationAttributeManager.SetSelectList("AttributeType", await SpecificationAttributeType.Option.ToSelectListAsync(useLocalization: false));
            specificationAttributeManager.SetSelectList("SpecificationAttribute", (await _specificationAttributeService
                    .GetSpecificationAttributesAsync())
                .Select(sa => sa as BaseEntity)
                .ToSelectList(p => (p as SpecificationAttribute)?.Name ?? string.Empty));

            manager.SetSelectList("ProductType", await ProductType.SimpleProduct.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("GiftCardType", await GiftCardType.Virtual.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("DownloadActivationType",
                await DownloadActivationType.Manually.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("ManageInventoryMethod",
                await ManageInventoryMethod.DontManageStock.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("LowStockActivity",
                await LowStockActivity.Nothing.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("BackorderMode", await BackorderMode.NoBackorders.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("RecurringCyclePeriod",
                await RecurringProductCyclePeriod.Days.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("RentalPricePeriod", await RentalPricePeriod.Days.ToSelectListAsync(useLocalization: false));

            manager.SetSelectList("Vendor",
                (await _vendorService.GetAllVendorsAsync(showHidden: true)).Select(v => v as BaseEntity)
                .ToSelectList(p => (p as Vendor)?.Name ?? string.Empty));
            manager.SetSelectList("ProductTemplate",
                (await _productTemplateService.GetAllProductTemplatesAsync()).Select(pt => pt as BaseEntity)
                .ToSelectList(p => (p as ProductTemplate)?.Name ?? string.Empty));
            manager.SetSelectList("DeliveryDate",
                (await _dateRangeService.GetAllDeliveryDatesAsync()).Select(dd => dd as BaseEntity)
                .ToSelectList(p => (p as DeliveryDate)?.Name ?? string.Empty));
            manager.SetSelectList("ProductAvailabilityRange",
                (await _dateRangeService.GetAllProductAvailabilityRangesAsync()).Select(range => range as BaseEntity)
                .ToSelectList(p => (p as ProductAvailabilityRange)?.Name ?? string.Empty));
            manager.SetSelectList("TaxCategory",
                (await _taxCategoryService.GetAllTaxCategoriesAsync()).Select(tc => tc as BaseEntity)
                .ToSelectList(p => (p as TaxCategory)?.Name ?? string.Empty));
            manager.SetSelectList("BasepriceUnit",
                (await _measureService.GetAllMeasureWeightsAsync()).Select(mw => mw as BaseEntity)
                .ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));
            manager.SetSelectList("BasepriceBaseUnit",
                (await _measureService.GetAllMeasureWeightsAsync()).Select(mw => mw as BaseEntity)
                .ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));
        }

        var allAttributeIds = new List<int>();
        var allSpecificationAttributeOptionIds = new List<int>();

        var attributeIdCellNum = 1 + ExportProductAttribute.ProductAttributeCellOffset;
        var specificationAttributeOptionIdCellNum =
            specificationAttributeManager.GetIndex("SpecificationAttributeOptionId") +
            ExportProductAttribute.ProductAttributeCellOffset;

        var productsInFile = new List<int>();

        //find end of data
        var typeOfExportedAttribute = ExportedAttributeType.NotSpecified;
        while (true)
        {
            var allColumnsAreEmpty = manager.GetDefaultProperties
                .Select(property => defaultWorksheet.Row(endRow).Cell(property.PropertyOrderPosition))
                .All(cell => string.IsNullOrEmpty(cell?.Value.ToString()));

            if (allColumnsAreEmpty)
                break;

            if (new[] { 1, 2 }.Select(cellNum => defaultWorksheet.Row(endRow).Cell(cellNum))
                    .All(cell => string.IsNullOrEmpty(cell?.Value.ToString())) &&
                defaultWorksheet.Row(endRow).OutlineLevel == 0)
            {
                var cellValue = defaultWorksheet.Row(endRow).Cell(attributeIdCellNum).Value;
                await SetOutLineForProductAttributeRowAsync(cellValue, defaultWorksheet, endRow);
                await SetOutLineForSpecificationAttributeRowAsync(cellValue, defaultWorksheet, endRow);
            }

            if (defaultWorksheet.Row(endRow).OutlineLevel != 0)
            {
                var newTypeOfExportedAttribute = GetTypeOfExportedAttribute(defaultWorksheet, metadata.LocalizedWorksheets, productAttributeManager, specificationAttributeManager, endRow);

                //skip caption row
                if (newTypeOfExportedAttribute != ExportedAttributeType.NotSpecified && newTypeOfExportedAttribute != typeOfExportedAttribute)
                {
                    typeOfExportedAttribute = newTypeOfExportedAttribute;
                    endRow++;
                    continue;
                }

                switch (typeOfExportedAttribute)
                {
                    case ExportedAttributeType.ProductAttribute:
                        productAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, endRow,
                            ExportProductAttribute.ProductAttributeCellOffset);
                        if (int.TryParse(defaultWorksheet.Row(endRow).Cell(attributeIdCellNum).Value.ToString(), out var aid))
                        {
                            allAttributeIds.Add(aid);
                        }

                        break;
                    case ExportedAttributeType.SpecificationAttribute:
                        specificationAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, endRow, ExportProductAttribute.ProductAttributeCellOffset);

                        if (int.TryParse(defaultWorksheet.Row(endRow).Cell(specificationAttributeOptionIdCellNum).Value.ToString(), out var saoid))
                        {
                            allSpecificationAttributeOptionIds.Add(saoid);
                        }

                        break;
                }

                endRow++;
                continue;
            }

            if (categoryCellNum > 0)
            {
                var categoryIds = defaultWorksheet.Row(endRow).Cell(categoryCellNum).Value.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(categoryIds))
                    allCategories.AddRange(categoryIds
                        .Split(new[] { ";", ">>" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                        .Distinct());
            }

            if (skuCellNum > 0)
            {
                var sku = defaultWorksheet.Row(endRow).Cell(skuCellNum).Value.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(sku))
                    allSku.Add(sku);
            }

            if (manufacturerCellNum > 0)
            {
                var manufacturerIds = defaultWorksheet.Row(endRow).Cell(manufacturerCellNum).Value.ToString() ??
                                      string.Empty;
                if (!string.IsNullOrEmpty(manufacturerIds))
                    allManufacturers.AddRange(manufacturerIds
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
            }

            if (limitedToStoresCellNum > 0)
            {
                var storeIds = defaultWorksheet.Row(endRow).Cell(limitedToStoresCellNum).Value.ToString() ??
                               string.Empty;
                if (!string.IsNullOrEmpty(storeIds))
                    allStores.AddRange(storeIds
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
            }

            //counting the number of products
            productsInFile.Add(endRow);

            endRow++;
        }

        //performance optimization, the check for the existence of the categories in one SQL request
        var notExistingCategories = await _categoryService.GetNotExistingCategoriesAsync(allCategories.ToArray());
        if (notExistingCategories.Any())
        {
            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.CategoriesDontExist"), string.Join(", ", notExistingCategories)));
        }

        //performance optimization, the check for the existence of the manufacturers in one SQL request
        var notExistingManufacturers = await _manufacturerService.GetNotExistingManufacturersAsync(allManufacturers.ToArray());
        if (notExistingManufacturers.Any())
        {
            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.ManufacturersDontExist"), string.Join(", ", notExistingManufacturers)));
        }

        //performance optimization, the check for the existence of the product attributes in one SQL request
        var notExistingProductAttributes = await _productAttributeService.GetNotExistingAttributesAsync(allAttributeIds.ToArray());
        if (notExistingProductAttributes.Any())
        {
            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.ProductAttributesDontExist"), string.Join(", ", notExistingProductAttributes)));
        }

        //performance optimization, the check for the existence of the specification attribute options in one SQL request
        var notExistingSpecificationAttributeOptions = await _specificationAttributeService.GetNotExistingSpecificationAttributeOptionsAsync(allSpecificationAttributeOptionIds.Where(saoId => saoId != 0).ToArray());
        if (notExistingSpecificationAttributeOptions.Any())
        {
            throw new ArgumentException($"The following specification attribute option ID(s) don't exist - {string.Join(", ", notExistingSpecificationAttributeOptions)}");
        }

        //performance optimization, the check for the existence of the stores in one SQL request
        var notExistingStores = await _storeService.GetNotExistingStoresAsync(allStores.ToArray());
        if (notExistingStores.Any())
        {
            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.StoresDontExist"), string.Join(", ", notExistingStores)));
        }

        return new ImportProductMetadata
        {
            EndRow = endRow,
            Manager = manager,
            Properties = defaultProperties,
            ProductsInFile = productsInFile,
            ProductAttributeManager = productAttributeManager,
            DefaultWorksheet = defaultWorksheet,
            LocalizedWorksheets = metadata.LocalizedWorksheets,
            SpecificationAttributeManager = specificationAttributeManager,
            SkuCellNum = skuCellNum,
            AllSku = allSku
        };
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportProductsFromSplitedXlsxAsync(IXLWorksheet worksheet, ImportProductMetadata metadata)
    {
        foreach (var path in SplitProductFile(worksheet, metadata))
        {
            using var scope = _serviceScopeFactory.CreateScope();
            // Resolve
            var importManager = EngineContext.Current.Resolve<IImportManager>(scope);

            using var sr = new StreamReader(path);
            await importManager.ImportProductsFromXlsxAsync(sr.BaseStream);

            try
            {
                _fileProvider.DeleteFile(path);
            }
            catch
            {
                // ignored
            }
        }
    }

    protected virtual IList<string> SplitProductFile(IXLWorksheet worksheet, ImportProductMetadata metadata)
    {
        var fileIndex = 1;
        var fileName = Guid.NewGuid().ToString();
        var endCell = metadata.Properties.Max(p => p.PropertyOrderPosition);

        var filePaths = new List<string>();

        while (true)
        {
            var curIndex = fileIndex * _catalogSettings.ExportImportProductsCountInOneFile;

            var startRow = metadata.ProductsInFile[(fileIndex - 1) * _catalogSettings.ExportImportProductsCountInOneFile];

            var endRow = metadata.CountProductsInFile > curIndex + 1
                ? metadata.ProductsInFile[curIndex - 1]
                : metadata.EndRow;

            var filePath = $"{_fileProvider.MapPath(ExportImportDefaults.UploadsTempPath)}/{fileName}_part_{fileIndex}.xlsx";

            CopyDataToNewFile(metadata, worksheet, filePath, startRow, endRow, endCell);

            filePaths.Add(filePath);
            fileIndex += 1;

            if (endRow == metadata.EndRow)
                break;
        }

        return filePaths;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<(ImportOrderMetadata, IXLWorksheet)> PrepareImportOrderDataAsync(IXLWorkbook workbook)
    {
        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        //the columns
        var metadata = GetWorkbookMetadata<Order>(workbook, languages);
        var worksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;

        var manager = new PropertyManager<Order, Language>(defaultProperties, _catalogSettings);

        var orderItemProperties = new[]
        {
            new PropertyByName<OrderItem, Language>("OrderItemGuid"),
            new PropertyByName<OrderItem, Language>("Name"),
            new PropertyByName<OrderItem, Language>("Sku"),
            new PropertyByName<OrderItem, Language>("PriceExclTax"),
            new PropertyByName<OrderItem, Language>("PriceInclTax"),
            new PropertyByName<OrderItem, Language>("Quantity"),
            new PropertyByName<OrderItem, Language>("DiscountExclTax"),
            new PropertyByName<OrderItem, Language>("DiscountInclTax"),
            new PropertyByName<OrderItem, Language>("TotalExclTax"),
            new PropertyByName<OrderItem, Language>("TotalInclTax")
        };

        var orderItemManager = new PropertyManager<OrderItem, Language>(orderItemProperties, _catalogSettings);

        var endRow = 2;
        var allOrderGuids = new List<Guid>();

        var tempProperty = manager.GetDefaultProperty("OrderGuid");
        var orderGuidCellNum = tempProperty?.PropertyOrderPosition ?? -1;

        tempProperty = manager.GetDefaultProperty("CustomerGuid");
        var customerGuidCellNum = tempProperty?.PropertyOrderPosition ?? -1;

        manager.SetSelectList("OrderStatus", await OrderStatus.Cancelled.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("ShippingStatus", await ShippingStatus.Delivered.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("PaymentStatus", await PaymentStatus.Authorized.ToSelectListAsync(useLocalization: false));

        var allCustomerGuids = new List<Guid>();

        var allOrderItemSkus = new List<string>();

        var countOrdersInFile = 0;

        //find end of data
        while (true)
        {
            var allColumnsAreEmpty = manager.GetDefaultProperties
                .Select(property => worksheet.Row(endRow).Cell(property.PropertyOrderPosition))
                .All(cell => string.IsNullOrEmpty(cell?.Value.ToString()));

            if (allColumnsAreEmpty)
                break;

            if (worksheet.Row(endRow).OutlineLevel != 0)
            {
                orderItemManager.ReadDefaultFromXlsx(worksheet, endRow, 2);

                //skip caption row
                if (!orderItemManager.IsCaption)
                {
                    allOrderItemSkus.Add(orderItemManager.GetDefaultProperty("Sku").StringValue);
                }

                endRow++;
                continue;
            }

            if (orderGuidCellNum > 0)
            {
                var orderGuidString = worksheet.Row(endRow).Cell(orderGuidCellNum).Value.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(orderGuidString) && Guid.TryParse(orderGuidString, out var orderGuid))
                    allOrderGuids.Add(orderGuid);
            }

            if (customerGuidCellNum > 0)
            {
                var customerGuidString = worksheet.Row(endRow).Cell(customerGuidCellNum).Value.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(customerGuidString) && Guid.TryParse(customerGuidString, out var customerGuid))
                    allCustomerGuids.Add(customerGuid);
            }

            //counting the number of orders
            countOrdersInFile++;

            endRow++;
        }

        //performance optimization, the check for the existence of the customers in one SQL request
        var notExistingCustomerGuids = await _customerService.GetNotExistingCustomersAsync(allCustomerGuids.ToArray());
        if (notExistingCustomerGuids.Any())
        {
            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Orders.Import.CustomersDontExist"), string.Join(", ", notExistingCustomerGuids)));
        }

        //performance optimization, the check for the existence of the order items in one SQL request
        var notExistingProductSkus = await _productService.GetNotExistingProductsAsync(allOrderItemSkus.ToArray());
        if (notExistingProductSkus.Any())
        {
            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Orders.Import.ProductsDontExist"), string.Join(", ", notExistingProductSkus)));
        }

        return (new ImportOrderMetadata
        {
            EndRow = endRow,
            Manager = manager,
            Properties = defaultProperties,
            CountOrdersInFile = countOrdersInFile,
            OrderItemManager = orderItemManager,
            AllOrderGuids = allOrderGuids,
            AllCustomerGuids = allCustomerGuids
        }, worksheet);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportOrderItemAsync(PropertyManager<OrderItem, Language> orderItemManager, Order lastLoadedOrder)
    {
        if (lastLoadedOrder == null || orderItemManager.IsCaption)
            return;

        var orderItemGuid = Guid.TryParse(orderItemManager.GetDefaultProperty("OrderItemGuid").StringValue, out var guidValue) ? guidValue : Guid.NewGuid();
        var sku = orderItemManager.GetDefaultProperty("Sku").StringValue;
        var priceExclTax = orderItemManager.GetDefaultProperty("PriceExclTax").DecimalValue;
        var priceInclTax = orderItemManager.GetDefaultProperty("PriceInclTax").DecimalValue;
        var quantity = orderItemManager.GetDefaultProperty("Quantity").IntValue;
        var discountExclTax = orderItemManager.GetDefaultProperty("DiscountExclTax").DecimalValue;
        var discountInclTax = orderItemManager.GetDefaultProperty("DiscountInclTax").DecimalValue;
        var totalExclTax = orderItemManager.GetDefaultProperty("TotalExclTax").DecimalValue;
        var totalInclTax = orderItemManager.GetDefaultProperty("TotalInclTax").DecimalValue;

        var orderItemProduct = await _productService.GetProductBySkuAsync(sku);
        var orderItem = (await _orderService.GetOrderItemsAsync(lastLoadedOrder.Id)).FirstOrDefault(f => f.OrderItemGuid == orderItemGuid);

        if (orderItem == null)
        {
            //insert order item
            orderItem = new OrderItem
            {
                DiscountAmountExclTax = discountExclTax,
                DiscountAmountInclTax = discountInclTax,
                OrderId = lastLoadedOrder.Id,
                OrderItemGuid = orderItemGuid,
                PriceExclTax = totalExclTax,
                PriceInclTax = totalInclTax,
                ProductId = orderItemProduct.Id,
                Quantity = quantity,
                OriginalProductCost = orderItemProduct.ProductCost,
                UnitPriceExclTax = priceExclTax,
                UnitPriceInclTax = priceInclTax
            };
            await _orderService.InsertOrderItemAsync(orderItem);
        }
        else
        {
            //update order item
            orderItem.DiscountAmountExclTax = discountExclTax;
            orderItem.DiscountAmountInclTax = discountInclTax;
            orderItem.OrderId = lastLoadedOrder.Id;
            orderItem.PriceExclTax = totalExclTax;
            orderItem.PriceInclTax = totalInclTax;
            orderItem.Quantity = quantity;
            orderItem.UnitPriceExclTax = priceExclTax;
            orderItem.UnitPriceInclTax = priceInclTax;
            await _orderService.UpdateOrderItemAsync(orderItem);
        }
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportProductLocalizedAsync(Product product, ImportProductMetadata metadata, int iRow, IList<Language> languages)
    {
        if (metadata.LocalizedWorksheets.Any())
        {
            var manager = metadata.Manager;
            foreach (var language in languages)
            {
                var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                if (lWorksheet == null)
                    continue;

                manager.CurrentLanguage = language;
                manager.ReadLocalizedFromXlsx(lWorksheet, iRow);

                foreach (var property in manager.GetLocalizedProperties)
                {
                    string localizedName = null;

                    switch (property.PropertyName)
                    {
                        case "Name":
                            localizedName = property.StringValue;
                            await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.Name, localizedName, language.Id);
                            break;
                        case "ShortDescription":
                            await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.ShortDescription, property.StringValue, language.Id);
                            break;
                        case "FullDescription":
                            await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.FullDescription, property.StringValue, language.Id);
                            break;
                        case "MetaKeywords":
                            await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.MetaKeywords, property.StringValue, language.Id);
                            break;
                        case "MetaDescription":
                            await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.MetaDescription, property.StringValue, language.Id);
                            break;
                        case "MetaTitle":
                            await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.MetaTitle, property.StringValue, language.Id);
                            break;
                        case "SeName":
                            //search engine name
                            var localizedSeName = await _urlRecordService.ValidateSeNameAsync(product, property.StringValue, localizedName, false);
                            await _urlRecordService.SaveSlugAsync(product, localizedSeName, language.Id);
                            break;
                    }
                }
            }
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get excel workbook metadata
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="workbook">Excel workbook</param>
    /// <param name="languages">Languages</param>
    /// <returns>Workbook metadata</returns>
    public static WorkbookMetadata<T> GetWorkbookMetadata<T>(IXLWorkbook workbook, IList<Language> languages)
    {
        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        var properties = new List<PropertyByName<T, Language>>();
        var localizedProperties = new List<PropertyByName<T, Language>>();
        var localizedWorksheets = new List<IXLWorksheet>();

        var poz = 1;
        while (true)
        {
            try
            {
                var cell = worksheet.Row(1).Cell(poz);

                if (string.IsNullOrEmpty(cell?.Value.ToString()))
                    break;

                poz += 1;
                properties.Add(new PropertyByName<T, Language>(cell.Value.ToString()));
            }
            catch
            {
                break;
            }
        }

        foreach (var ws in workbook.Worksheets.Skip(1))
            if (languages.Any(l => l.UniqueSeoCode.Equals(ws.Name, StringComparison.InvariantCultureIgnoreCase)))
                localizedWorksheets.Add(ws);

        if (localizedWorksheets.Any())
        {
            // get the first worksheet in the workbook
            var localizedWorksheet = localizedWorksheets.First();

            poz = 1;
            while (true)
            {
                try
                {
                    var cell = localizedWorksheet.Row(1).Cell(poz);

                    if (string.IsNullOrEmpty(cell?.Value.ToString()))
                        break;

                    poz += 1;
                    localizedProperties.Add(new PropertyByName<T, Language>(cell.Value.ToString()));
                }
                catch
                {
                    break;
                }
            }
        }

        return new WorkbookMetadata<T>()
        {
            DefaultProperties = properties,
            LocalizedProperties = localizedProperties,
            DefaultWorksheet = worksheet,
            LocalizedWorksheets = localizedWorksheets
        };
    }

    /// <summary>
    /// Import customers from XLSX file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ImportCustomersFromXlsxAsync(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);

        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        //the columns
        var metadata = GetWorkbookMetadata<Customer>(workbook, languages);
        var defaultWorksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;

        var manager = new PropertyManager<Customer, Language>(defaultProperties, _catalogSettings);

        if (_catalogSettings.ExportImportUseDropdownlistsForAssociatedEntities)
        {
            manager.SetSelectList("VatNumberStatus",
                await VatNumberStatus.Unknown.ToSelectListAsync(useLocalization: false));
        }

        var iRow = 2;
        var allRoles = await _customerService.GetAllCustomerRolesAsync();
        var countries = await _countryService.GetAllCountriesAsync();
        var states = await _stateProvinceService.GetStateProvincesAsync();

        while (true)
        {
            var allColumnsAreEmpty = manager.GetDefaultProperties
                .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

            if (allColumnsAreEmpty)
                break;

            manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

            var customerGuid = manager.GetDefaultProperty("CustomerGuid").GuidValue;
            var customer = await _customerService.GetCustomerByGuidAsync(customerGuid) ??
                           await _customerService.GetCustomerByEmailAsync(manager.GetDefaultProperty("Email").StringValue);

            int? avatarPictureId = null;
            string signature = null;
            string password = null;
            string passwordSalt = null;

            var isNew = customer == null;

            if (isNew)
                customer = new Customer
                {
                    CustomerGuid = Guid.Empty.Equals(customerGuid) ? Guid.NewGuid() : customerGuid,
                    CreatedOnUtc = DateTime.UtcNow
                };

            var rolesToSave = new List<int>();

            foreach (var property in manager.GetDefaultProperties)
            {
                switch (property.PropertyName)
                {
                    case "Email":
                        customer.Email = property.StringValue;
                        break;
                    case "Username":
                        customer.Username = property.StringValue;
                        break;
                    case "IsTaxExempt":
                        customer.IsTaxExempt = property.BooleanValue;
                        break;
                    case "AffiliateId":
                        customer.AffiliateId = property.IntValue;
                        break;
                    case "Vendor":
                        if (!string.IsNullOrEmpty(property.StringValue))
                            if (int.TryParse(property.StringValue, out var vendorId))
                                customer.VendorId = vendorId;
                            else
                            {
                                var vendors = await _vendorService.GetAllVendorsAsync(property.StringValue);
                                customer.VendorId = vendors.FirstOrDefault()?.Id ?? 0;
                            }
                        else
                            customer.VendorId = 0;
                        break;
                    case "Active":
                        customer.Active = property.BooleanValue;
                        break;
                    case "CustomerRoles":
                        var roles = property.StringValue.Split(", ");

                        foreach (var role in roles)
                            if (int.TryParse(role, out var roleId))
                                rolesToSave.Add(roleId);
                            else
                            {
                                var currentRole = allRoles.FirstOrDefault(r =>
                                    r.Name.Equals(role, StringComparison.InvariantCultureIgnoreCase));

                                if (currentRole != null)
                                    rolesToSave.Add(currentRole.Id);
                            }
                        break;
                    case "CreatedOnUtc":
                        if (DateTime.TryParse(property.StringValue, out var date))
                            customer.CreatedOnUtc = date;
                        break;
                    case "FirstName":
                        customer.FirstName = property.StringValue;
                        break;
                    case "LastName":
                        customer.LastName = property.StringValue;
                        break;
                    case "Gender":
                        customer.Gender = property.StringValue;
                        break;
                    case "Company":
                        customer.Company = property.StringValue;
                        break;
                    case "StreetAddress":
                        customer.StreetAddress = property.StringValue;
                        break;
                    case "StreetAddress2":
                        customer.StreetAddress2 = property.StringValue;
                        break;
                    case "ZipPostalCode":
                        customer.ZipPostalCode = property.StringValue;
                        break;
                    case "City":
                        customer.City = property.StringValue;
                        break;
                    case "County":
                        customer.County = property.StringValue;
                        break;
                    case "Country":
                        if (int.TryParse(property.StringValue, out var countryId))
                            customer.CountryId = countryId;
                        else
                        {
                            var country = countries.FirstOrDefault(c =>
                                c.Name.Equals(property.StringValue, StringComparison.InvariantCultureIgnoreCase));

                            if (country != null)
                                customer.CountryId = country.Id;
                        }
                        break;
                    case "StateProvince":
                        if (int.TryParse(property.StringValue, out var stateId))
                            customer.StateProvinceId = stateId;
                        else
                        {
                            var state = states.FirstOrDefault(s =>
                                s.Name.Equals(property.StringValue, StringComparison.InvariantCultureIgnoreCase));

                            if (state != null)
                                customer.StateProvinceId = state.Id;
                        }
                        break;
                    case "Phone":
                        customer.Phone = property.StringValue;
                        break;
                    case "Fax":
                        customer.Fax = property.StringValue;
                        break;
                    case "VatNumber":
                        customer.VatNumber = property.StringValue;
                        break;
                    case "VatNumberStatus":
                        customer.VatNumberStatusId = property.IntValue;
                        break;
                    case "TimeZone":
                        customer.TimeZoneId = property.StringValue;
                        break;
                    case "AvatarPictureId":
                        avatarPictureId = property.IntValueNullable;
                        break;
                    case "Signature":
                        signature = property.StringValue;
                        break;
                    case "CustomCustomerAttributesXML":
                        customer.CustomCustomerAttributesXML = property.StringValue;
                        break;
                    case "Password":
                        password = property.StringValue;
                        break;
                    case "PasswordSalt":
                        passwordSalt = property.StringValue;
                        break;
                }
            }

            if (isNew)
                await _customerService.InsertCustomerAsync(customer);
            else
                await _customerService.UpdateCustomerAsync(customer);

            var customerRoles = await _customerService.GetCustomerRolesAsync(customer);

            foreach (var roleId in rolesToSave)
            {
                var role = allRoles.FirstOrDefault(r => r.Id == roleId);

                if (role == null || customerRoles.Any(cr => cr.Id == roleId))
                    continue;

                await _customerService.AddCustomerRoleMappingAsync(
                    new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = roleId });
            }

            if (!isNew && rolesToSave.Any())
                foreach (var customerRole in customerRoles.Where(cr=>!rolesToSave.Contains(cr.Id)).ToList())
                    await _customerService.RemoveCustomerRoleMappingAsync(customer, customerRole);

            if (avatarPictureId.HasValue)
                await _genericAttributeService.SaveAttributeAsync(customer,
                    NopCustomerDefaults.AvatarPictureIdAttribute, avatarPictureId.Value);

            if (!string.IsNullOrEmpty(signature))
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SignatureAttribute,
                    signature);

            if (_securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword &&
                !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(passwordSalt))
            {
                var lastPassword = isNew ? null : await _customerService.GetCurrentPasswordAsync(customer.Id);
                
                if (lastPassword == null || !(lastPassword.Password.Equals(password) && lastPassword.PasswordSalt.Equals(passwordSalt)))
                    await _customerService.InsertCustomerPasswordAsync(new CustomerPassword
                    {
                        CustomerId = customer.Id,
                        Password = password,
                        PasswordSalt = passwordSalt,
                        PasswordFormat = PasswordFormat.Hashed,
                        CreatedOnUtc = DateTime.UtcNow
                    });
            }

            iRow++;
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("ImportCustomers",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportCustomers"), iRow - 2));
    }

    /// <summary>
    /// Import products from XLSX file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ImportProductsFromXlsxAsync(Stream stream)
    {
        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        using var workbook = new XLWorkbook(stream);
        var downloadedFiles = new List<string>();

        var metadata = await PrepareImportProductDataAsync(workbook, languages);
        var defaultWorksheet = metadata.DefaultWorksheet;

        if (_catalogSettings.ExportImportSplitProductsFile && metadata.CountProductsInFile > _catalogSettings.ExportImportProductsCountInOneFile)
        {
            await ImportProductsFromSplitedXlsxAsync(defaultWorksheet, metadata);
            return;
        }

        //performance optimization, load all products by SKU in one SQL request
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        var allProductsBySku = await _productService.GetProductsBySkuAsync(metadata.AllSku.ToArray(), currentVendor?.Id ?? 0);

        //validate maximum number of products per vendor
        if (_vendorSettings.MaximumProductNumber > 0 &&
            currentVendor != null)
        {
            var newProductsCount = metadata.CountProductsInFile - allProductsBySku.Count;
            if (await _productService.GetNumberOfProductsByVendorIdAsync(currentVendor.Id) + newProductsCount > _vendorSettings.MaximumProductNumber)
                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ExceededMaximumNumber"), _vendorSettings.MaximumProductNumber));
        }

        //performance optimization, load all categories IDs for products in one SQL request
        var allProductsCategoryIds = await _categoryService.GetProductCategoryIdsAsync(allProductsBySku.Select(p => p.Id).ToArray());

        //performance optimization, load all categories in one SQL request
        Dictionary<CategoryKey, Category> allCategories;
        try
        {
            var allCategoryList = await _categoryService.GetAllCategoriesAsync(showHidden: true);

            allCategories = await allCategoryList
                .ToDictionaryAwaitAsync(async c => await CategoryKey.CreateCategoryKeyAsync(c, _categoryService, allCategoryList, _storeMappingService), c => new ValueTask<Category>(c));
        }
        catch (ArgumentException)
        {
            //categories with the same name are not supported in the same category level
            throw new ArgumentException(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.CategoriesWithSameNameNotSupported"));
        }

        //performance optimization, load all manufacturers IDs for products in one SQL request
        var allProductsManufacturerIds = await _manufacturerService.GetProductManufacturerIdsAsync(allProductsBySku.Select(p => p.Id).ToArray());

        //performance optimization, load all manufacturers in one SQL request
        var allManufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true);

        //performance optimization, load all stores in one SQL request
        var allStores = await _storeService.GetAllStoresAsync();

        //product to import images
        var productPictureMetadata = new List<ProductPictureMetadata>();

        Product lastLoadedProduct = null;
        var typeOfExportedAttribute = ExportedAttributeType.NotSpecified;

        for (var iRow = 2; iRow < metadata.EndRow; iRow++)
        {
            if (defaultWorksheet.Row(iRow).OutlineLevel != 0)
            {
                if (lastLoadedProduct == null)
                    continue;

                var newTypeOfExportedAttribute = GetTypeOfExportedAttribute(defaultWorksheet, metadata.LocalizedWorksheets, metadata.ProductAttributeManager, metadata.SpecificationAttributeManager, iRow);

                //skip caption row
                if (newTypeOfExportedAttribute != ExportedAttributeType.NotSpecified &&
                    newTypeOfExportedAttribute != typeOfExportedAttribute)
                {
                    typeOfExportedAttribute = newTypeOfExportedAttribute;
                    continue;
                }

                switch (typeOfExportedAttribute)
                {
                    case ExportedAttributeType.ProductAttribute:
                        await ImportProductAttributeAsync(metadata, lastLoadedProduct, languages, iRow);
                        break;
                    case ExportedAttributeType.SpecificationAttribute:
                        await ImportSpecificationAttributeAsync(metadata, lastLoadedProduct, languages, iRow);
                        break;
                    case ExportedAttributeType.NotSpecified:
                    default:
                        continue;
                }

                continue;
            }

            metadata.Manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

            var product = metadata.SkuCellNum > 0 ? allProductsBySku.FirstOrDefault(p => p.Sku == metadata.Manager.GetDefaultProperty("SKU").StringValue) : null;

            var isNew = product == null;

            product ??= new Product();

            //some of previous values
            var previousStockQuantity = product.StockQuantity;
            var previousWarehouseId = product.WarehouseId;
            var prevTotalStockQuantity = await _productService.GetTotalStockQuantityAsync(product);

            if (isNew)
                product.CreatedOnUtc = DateTime.UtcNow;

            foreach (var property in metadata.Manager.GetDefaultProperties)
            {
                switch (property.PropertyName)
                {
                    case "ProductType":
                        product.ProductTypeId = property.IntValue;
                        break;
                    case "ParentGroupedProductId":
                        product.ParentGroupedProductId = property.IntValue;
                        break;
                    case "VisibleIndividually":
                        product.VisibleIndividually = property.BooleanValue;
                        break;
                    case "Name":
                        product.Name = property.StringValue;
                        break;
                    case "ShortDescription":
                        product.ShortDescription = property.StringValue;
                        break;
                    case "FullDescription":
                        product.FullDescription = property.StringValue;
                        break;
                    case "Vendor":
                        //vendor can't change this field
                        if (currentVendor == null)
                            product.VendorId = property.IntValue;
                        break;
                    case "ProductTemplate":
                        product.ProductTemplateId = property.IntValue;
                        break;
                    case "ShowOnHomepage":
                        //vendor can't change this field
                        if (currentVendor == null)
                            product.ShowOnHomepage = property.BooleanValue;
                        break;
                    case "DisplayOrder":
                        //vendor can't change this field
                        if (currentVendor == null)
                            product.DisplayOrder = property.IntValue;
                        break;
                    case "MetaKeywords":
                        product.MetaKeywords = property.StringValue;
                        break;
                    case "MetaDescription":
                        product.MetaDescription = property.StringValue;
                        break;
                    case "MetaTitle":
                        product.MetaTitle = property.StringValue;
                        break;
                    case "AllowCustomerReviews":
                        product.AllowCustomerReviews = property.BooleanValue;
                        break;
                    case "Published":
                        product.Published = property.BooleanValue;
                        break;
                    case "SKU":
                        product.Sku = property.StringValue;
                        break;
                    case "ManufacturerPartNumber":
                        product.ManufacturerPartNumber = property.StringValue;
                        break;
                    case "Gtin":
                        product.Gtin = property.StringValue;
                        break;
                    case "IsGiftCard":
                        product.IsGiftCard = property.BooleanValue;
                        break;
                    case "GiftCardType":
                        product.GiftCardTypeId = property.IntValue;
                        break;
                    case "OverriddenGiftCardAmount":
                        product.OverriddenGiftCardAmount = property.DecimalValue;
                        break;
                    case "RequireOtherProducts":
                        product.RequireOtherProducts = property.BooleanValue;
                        break;
                    case "RequiredProductIds":
                        product.RequiredProductIds = property.StringValue;
                        break;
                    case "AutomaticallyAddRequiredProducts":
                        product.AutomaticallyAddRequiredProducts = property.BooleanValue;
                        break;
                    case "IsDownload":
                        product.IsDownload = property.BooleanValue;
                        break;
                    case "DownloadId":
                        product.DownloadId = property.IntValue;
                        break;
                    case "UnlimitedDownloads":
                        product.UnlimitedDownloads = property.BooleanValue;
                        break;
                    case "MaxNumberOfDownloads":
                        product.MaxNumberOfDownloads = property.IntValue;
                        break;
                    case "DownloadActivationType":
                        product.DownloadActivationTypeId = property.IntValue;
                        break;
                    case "HasSampleDownload":
                        product.HasSampleDownload = property.BooleanValue;
                        break;
                    case "SampleDownloadId":
                        product.SampleDownloadId = property.IntValue;
                        break;
                    case "HasUserAgreement":
                        product.HasUserAgreement = property.BooleanValue;
                        break;
                    case "UserAgreementText":
                        product.UserAgreementText = property.StringValue;
                        break;
                    case "IsRecurring":
                        product.IsRecurring = property.BooleanValue;
                        break;
                    case "RecurringCycleLength":
                        product.RecurringCycleLength = property.IntValue;
                        break;
                    case "RecurringCyclePeriod":
                        product.RecurringCyclePeriodId = property.IntValue;
                        break;
                    case "RecurringTotalCycles":
                        product.RecurringTotalCycles = property.IntValue;
                        break;
                    case "IsRental":
                        product.IsRental = property.BooleanValue;
                        break;
                    case "RentalPriceLength":
                        product.RentalPriceLength = property.IntValue;
                        break;
                    case "RentalPricePeriod":
                        product.RentalPricePeriodId = property.IntValue;
                        break;
                    case "IsShipEnabled":
                        product.IsShipEnabled = property.BooleanValue;
                        break;
                    case "IsFreeShipping":
                        product.IsFreeShipping = property.BooleanValue;
                        break;
                    case "ShipSeparately":
                        product.ShipSeparately = property.BooleanValue;
                        break;
                    case "AdditionalShippingCharge":
                        product.AdditionalShippingCharge = property.DecimalValue;
                        break;
                    case "DeliveryDate":
                        product.DeliveryDateId = property.IntValue;
                        break;
                    case "IsTaxExempt":
                        product.IsTaxExempt = property.BooleanValue;
                        break;
                    case "TaxCategory":
                        product.TaxCategoryId = property.IntValue;
                        break;
                    case "ManageInventoryMethod":
                        product.ManageInventoryMethodId = property.IntValue;
                        break;
                    case "ProductAvailabilityRange":
                        product.ProductAvailabilityRangeId = property.IntValue;
                        break;
                    case "UseMultipleWarehouses":
                        product.UseMultipleWarehouses = property.BooleanValue;
                        break;
                    case "WarehouseId":
                        product.WarehouseId = property.IntValue;
                        break;
                    case "StockQuantity":
                        product.StockQuantity = property.IntValue;
                        break;
                    case "DisplayStockAvailability":
                        product.DisplayStockAvailability = property.BooleanValue;
                        break;
                    case "DisplayStockQuantity":
                        product.DisplayStockQuantity = property.BooleanValue;
                        break;
                    case "MinStockQuantity":
                        product.MinStockQuantity = property.IntValue;
                        break;
                    case "LowStockActivity":
                        product.LowStockActivityId = property.IntValue;
                        break;
                    case "NotifyAdminForQuantityBelow":
                        product.NotifyAdminForQuantityBelow = property.IntValue;
                        break;
                    case "BackorderMode":
                        product.BackorderModeId = property.IntValue;
                        break;
                    case "AllowBackInStockSubscriptions":
                        product.AllowBackInStockSubscriptions = property.BooleanValue;
                        break;
                    case "OrderMinimumQuantity":
                        product.OrderMinimumQuantity = property.IntValue;
                        break;
                    case "OrderMaximumQuantity":
                        product.OrderMaximumQuantity = property.IntValue;
                        break;
                    case "AllowedQuantities":
                        product.AllowedQuantities = property.StringValue;
                        break;
                    case "AllowAddingOnlyExistingAttributeCombinations":
                        product.AllowAddingOnlyExistingAttributeCombinations = property.BooleanValue;
                        break;
                    case "NotReturnable":
                        product.NotReturnable = property.BooleanValue;
                        break;
                    case "DisableBuyButton":
                        product.DisableBuyButton = property.BooleanValue;
                        break;
                    case "DisableWishlistButton":
                        product.DisableWishlistButton = property.BooleanValue;
                        break;
                    case "AvailableForPreOrder":
                        product.AvailableForPreOrder = property.BooleanValue;
                        break;
                    case "PreOrderAvailabilityStartDateTimeUtc":
                        product.PreOrderAvailabilityStartDateTimeUtc = property.DateTimeNullable;
                        break;
                    case "CallForPrice":
                        product.CallForPrice = property.BooleanValue;
                        break;
                    case "Price":
                        product.Price = property.DecimalValue;
                        break;
                    case "OldPrice":
                        product.OldPrice = property.DecimalValue;
                        break;
                    case "ProductCost":
                        product.ProductCost = property.DecimalValue;
                        break;
                    case "CustomerEntersPrice":
                        product.CustomerEntersPrice = property.BooleanValue;
                        break;
                    case "MinimumCustomerEnteredPrice":
                        product.MinimumCustomerEnteredPrice = property.DecimalValue;
                        break;
                    case "MaximumCustomerEnteredPrice":
                        product.MaximumCustomerEnteredPrice = property.DecimalValue;
                        break;
                    case "BasepriceEnabled":
                        product.BasepriceEnabled = property.BooleanValue;
                        break;
                    case "BasepriceAmount":
                        product.BasepriceAmount = property.DecimalValue;
                        break;
                    case "BasepriceUnit":
                        product.BasepriceUnitId = property.IntValue;
                        break;
                    case "BasepriceBaseAmount":
                        product.BasepriceBaseAmount = property.DecimalValue;
                        break;
                    case "BasepriceBaseUnit":
                        product.BasepriceBaseUnitId = property.IntValue;
                        break;
                    case "MarkAsNew":
                        product.MarkAsNew = property.BooleanValue;
                        break;
                    case "MarkAsNewStartDateTimeUtc":
                        product.MarkAsNewStartDateTimeUtc = property.DateTimeNullable;
                        break;
                    case "MarkAsNewEndDateTimeUtc":
                        product.MarkAsNewEndDateTimeUtc = property.DateTimeNullable;
                        break;
                    case "Weight":
                        product.Weight = property.DecimalValue;
                        break;
                    case "Length":
                        product.Length = property.DecimalValue;
                        break;
                    case "Width":
                        product.Width = property.DecimalValue;
                        break;
                    case "Height":
                        product.Height = property.DecimalValue;
                        break;
                    case "IsLimitedToStores":
                        product.LimitedToStores = property.BooleanValue;
                        break;
                    case "DisplayAttributeCombinationImagesOnly":
                        product.DisplayAttributeCombinationImagesOnly = property.BooleanValue;
                        break;
                }
            }

            //set some default values if not specified
            if (isNew && metadata.Properties.All(p => p.PropertyName != "ProductType"))
                product.ProductType = ProductType.SimpleProduct;
            if (isNew && metadata.Properties.All(p => p.PropertyName != "VisibleIndividually"))
                product.VisibleIndividually = true;
            if (isNew && metadata.Properties.All(p => p.PropertyName != "Published"))
                product.Published = true;

            //sets the current vendor for the new product
            if (isNew && currentVendor != null)
                product.VendorId = currentVendor.Id;

            product.UpdatedOnUtc = DateTime.UtcNow;

            if (isNew)
                await _productService.InsertProductAsync(product);
            else
                await _productService.UpdateProductAsync(product);

            //quantity change history
            if (isNew || previousWarehouseId == product.WarehouseId)
            {
                await _productService.AddStockQuantityHistoryEntryAsync(product, product.StockQuantity - previousStockQuantity, product.StockQuantity,
                    product.WarehouseId, await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.ImportProduct.Edit"));
            }
            //warehouse is changed 
            else
            {
                //compose a message
                var oldWarehouseMessage = string.Empty;
                if (previousWarehouseId > 0)
                {
                    var oldWarehouse = await _shippingService.GetWarehouseByIdAsync(previousWarehouseId);
                    if (oldWarehouse != null)
                        oldWarehouseMessage = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse.Old"), oldWarehouse.Name);
                }

                var newWarehouseMessage = string.Empty;
                if (product.WarehouseId > 0)
                {
                    var newWarehouse = await _shippingService.GetWarehouseByIdAsync(product.WarehouseId);
                    if (newWarehouse != null)
                        newWarehouseMessage = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse.New"), newWarehouse.Name);
                }

                var message = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.ImportProduct.EditWarehouse"), oldWarehouseMessage, newWarehouseMessage);

                //record history
                await _productService.AddStockQuantityHistoryEntryAsync(product, -previousStockQuantity, 0, previousWarehouseId, message);
                await _productService.AddStockQuantityHistoryEntryAsync(product, product.StockQuantity, product.StockQuantity, product.WarehouseId, message);
            }

            if (!isNew &&
                product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                await _productService.GetTotalStockQuantityAsync(product) > 0 &&
                prevTotalStockQuantity <= 0 &&
                product.Published &&
                !product.Deleted)
            {
                await _backInStockSubscriptionService.SendNotificationsToSubscribersAsync(product);
            }

            var tempProperty = metadata.Manager.GetDefaultProperty("SeName");

            //search engine name
            var seName = tempProperty?.StringValue ?? (isNew ? string.Empty : await _urlRecordService.GetSeNameAsync(product, 0));
            await _urlRecordService.SaveSlugAsync(product, await _urlRecordService.ValidateSeNameAsync(product, seName, product.Name, true), 0);

            //save product localized data
            await ImportProductLocalizedAsync(product, metadata, iRow, languages);

            tempProperty = metadata.Manager.GetDefaultProperty("Categories");

            if (tempProperty != null)
            {
                var categoryList = tempProperty.StringValue;

                //category mappings
                var categories = isNew || !allProductsCategoryIds.ContainsKey(product.Id) ? Array.Empty<int>() : allProductsCategoryIds[product.Id];

                var storesIds = product.LimitedToStores
                    ? (await _storeMappingService.GetStoresIdsWithAccessAsync(product)).ToList()
                    : new List<int>();

                var importedCategories = await categoryList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(categoryName => new CategoryKey(categoryName, storesIds))
                    .SelectAwait(async categoryKey =>
                    {
                        var rez = (allCategories.TryGetValue(categoryKey, out var value) ? value.Id : allCategories.Values.FirstOrDefault(c => c.Name == categoryKey.Key)?.Id) ??
                                  allCategories.FirstOrDefault(p =>
                                          p.Key.Key.Equals(categoryKey.Key, StringComparison.InvariantCultureIgnoreCase))
                                      .Value?.Id;

                        if (!rez.HasValue && int.TryParse(categoryKey.Key, out var id))
                            rez = id;

                        if (!rez.HasValue)
                            //database doesn't contain the imported category
                            //this can happen if the category was deleted during the import process
                            await _logger.WarningAsync(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.DatabaseNotContainCategory"), product.Name, categoryKey.Key));

                        return rez;
                    }).Where(id => id != null).ToListAsync();

                foreach (var categoryId in importedCategories)
                {
                    if (categories.Any(c => c == categoryId))
                        continue;

                    var productCategory = new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId.Value,
                        IsFeaturedProduct = false,
                        DisplayOrder = 1
                    };
                    await _categoryService.InsertProductCategoryAsync(productCategory);
                }

                //delete product categories
                var deletedProductCategories = await categories.Where(categoryId => !importedCategories.Contains(categoryId))
                    .SelectAwait(async categoryId => (await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, true)).FirstOrDefault(pc => pc.CategoryId == categoryId)).Where(pc => pc != null).ToListAsync();

                foreach (var deletedProductCategory in deletedProductCategories)
                    await _categoryService.DeleteProductCategoryAsync(deletedProductCategory);
            }

            tempProperty = metadata.Manager.GetDefaultProperty("Manufacturers");
            if (tempProperty != null)
            {
                var manufacturerList = tempProperty.StringValue;

                //manufacturer mappings
                var manufacturers = isNew || !allProductsManufacturerIds.ContainsKey(product.Id) ? Array.Empty<int>() : allProductsManufacturerIds[product.Id];

                var importedManufacturers = await manufacturerList
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .SelectAwait(async x =>
                    {
                        var id = allManufacturers.FirstOrDefault(m => m.Name == x.Trim())?.Id;

                        if (id != null)
                            return id;

                        id = int.TryParse(x, out var parsedId) ? parsedId : null;

                        if (!id.HasValue)
                            //database doesn't contain the imported manufacturer
                            //this can happen if the manufacturer was deleted during the import process
                            await _logger.WarningAsync(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.DatabaseNotContainManufacturer"), product.Name, x));

                        return id;
                    }).Where(id => id.HasValue).ToListAsync();

                foreach (var manufacturerId in importedManufacturers)
                {
                    if (manufacturers.Any(c => c == manufacturerId))
                        continue;

                    var productManufacturer = new ProductManufacturer
                    {
                        ProductId = product.Id,
                        ManufacturerId = manufacturerId.Value,
                        IsFeaturedProduct = false,
                        DisplayOrder = 1
                    };
                    await _manufacturerService.InsertProductManufacturerAsync(productManufacturer);
                }

                //delete product manufacturers
                var deletedProductsManufacturers = await manufacturers.Where(manufacturerId => !importedManufacturers.Contains(manufacturerId))
                    .SelectAwait(async manufacturerId => (await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id)).FirstOrDefault(pc => pc.ManufacturerId == manufacturerId)).ToListAsync();
                foreach (var deletedProductManufacturer in deletedProductsManufacturers.Where(m => m != null))
                    await _manufacturerService.DeleteProductManufacturerAsync(deletedProductManufacturer);
            }

            tempProperty = metadata.Manager.GetDefaultProperty("ProductTags");
            if (tempProperty != null)
            {
                var productTags = tempProperty.StringValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();

                //searching existing product tags by their id
                var productTagIds = productTags.Where(pt => int.TryParse(pt, out var _)).Select(int.Parse);

                var productTagsByIds = (await _productTagService.GetAllProductTagsByProductIdAsync(product.Id)).Where(pt => productTagIds.Contains(pt.Id)).ToList();

                productTags.AddRange(productTagsByIds.Select(pt => pt.Name));
                var filter = productTagsByIds.Select(pt => pt.Id.ToString()).ToList();

                //product tag mappings
                await _productTagService.UpdateProductTagsAsync(product, productTags.Where(pt => !filter.Contains(pt)).ToArray());
            }

            tempProperty = metadata.Manager.GetDefaultProperty("LimitedToStores");
            if (tempProperty != null)
            {
                var limitedToStoresList = tempProperty.StringValue;

                var importedStores = product.LimitedToStores ? limitedToStoresList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => allStores.FirstOrDefault(store => store.Name == x.Trim())?.Id ?? int.Parse(x.Trim())).ToList() : new List<int>();

                await _productService.UpdateProductStoreMappingsAsync(product, importedStores);
            }

            var picture1 = await DownloadFileAsync(metadata.Manager.GetDefaultProperty("Picture1")?.StringValue, downloadedFiles);
            var picture2 = await DownloadFileAsync(metadata.Manager.GetDefaultProperty("Picture2")?.StringValue, downloadedFiles);
            var picture3 = await DownloadFileAsync(metadata.Manager.GetDefaultProperty("Picture3")?.StringValue, downloadedFiles);

            productPictureMetadata.Add(new ProductPictureMetadata
            {
                ProductItem = product,
                Picture1Path = picture1,
                Picture2Path = picture2,
                Picture3Path = picture3,
                IsNew = isNew
            });

            lastLoadedProduct = product;
        }

        if (_mediaSettings.ImportProductImagesUsingHash && await _pictureService.IsStoreInDbAsync())
            await ImportProductImagesUsingHashAsync(productPictureMetadata, allProductsBySku);
        else
            await ImportProductImagesUsingServicesAsync(productPictureMetadata);

        foreach (var downloadedFile in downloadedFiles)
        {
            if (!_fileProvider.FileExists(downloadedFile))
                continue;

            try
            {
                _fileProvider.DeleteFile(downloadedFile);
            }
            catch
            {
                // ignored
            }
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("ImportProducts", string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportProducts"), metadata.CountProductsInFile));
    }

    /// <summary>
    /// Import newsletter subscribers from TXT file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of imported subscribers
    /// </returns>
    public virtual async Task<int> ImportNewsletterSubscribersFromTxtAsync(Stream stream)
    {
        var count = 0;
        using (var reader = new StreamReader(stream))
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                var tmp = line.Split(',');

                if (tmp.Length > 3)
                    throw new NopException("Wrong file format");

                var isActive = true;

                var store = await _storeContext.GetCurrentStoreAsync();
                var storeId = store.Id;

                //"email" field specified
                var email = tmp[0].Trim();

                if (!CommonHelper.IsValidEmail(email))
                    continue;

                //"active" field specified
                if (tmp.Length >= 2)
                    isActive = bool.Parse(tmp[1].Trim());

                //"storeId" field specified
                if (tmp.Length == 3)
                    storeId = int.Parse(tmp[2].Trim());

                //import
                var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(email, storeId);
                if (subscription != null)
                {
                    subscription.Email = email;
                    subscription.Active = isActive;
                    await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
                }
                else
                {
                    var customer = await _customerService.GetCustomerByEmailAsync(email);
                    subscription = new NewsLetterSubscription
                    {
                        Active = isActive,
                        CreatedOnUtc = DateTime.UtcNow,
                        Email = email,
                        StoreId = storeId,
                        LanguageId = customer?.LanguageId ?? store.DefaultLanguageId,
                        NewsLetterSubscriptionGuid = Guid.NewGuid()
                    };
                    await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
                }

                count++;
            }

        await _customerActivityService.InsertActivityAsync("ImportNewsLetterSubscriptions",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportNewsLetterSubscriptions"), count));

        return count;
    }

    /// <summary>
    /// Import states from TXT file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="writeLog">Indicates whether to add logging</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of imported states
    /// </returns>
    public virtual async Task<int> ImportStatesFromTxtAsync(Stream stream, bool writeLog = true)
    {
        var count = 0;
        using (var reader = new StreamReader(stream))
        {
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                var tmp = line.Split(',');

                if (tmp.Length != 5)
                    throw new NopException("Wrong file format");

                //parse
                var countryTwoLetterIsoCode = tmp[0].Trim();
                var name = tmp[1].Trim();
                var abbreviation = tmp[2].Trim();
                var published = bool.Parse(tmp[3].Trim());
                var displayOrder = int.Parse(tmp[4].Trim());

                var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(countryTwoLetterIsoCode);
                if (country == null)
                {
                    //country cannot be loaded. skip
                    continue;
                }

                //import
                var states = await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id, showHidden: true);
                var state = states.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                if (state != null)
                {
                    state.Abbreviation = abbreviation;
                    state.Published = published;
                    state.DisplayOrder = displayOrder;
                    await _stateProvinceService.UpdateStateProvinceAsync(state);
                }
                else
                {
                    state = new StateProvince
                    {
                        CountryId = country.Id,
                        Name = name,
                        Abbreviation = abbreviation,
                        Published = published,
                        DisplayOrder = displayOrder
                    };
                    await _stateProvinceService.InsertStateProvinceAsync(state);
                }

                count++;
            }
        }

        //activity log
        if (writeLog)
        {
            await _customerActivityService.InsertActivityAsync("ImportStates",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportStates"), count));
        }

        return count;
    }

    /// <summary>
    /// Import manufacturers from XLSX file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ImportManufacturersFromXlsxAsync(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);

        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        //the columns
        var metadata = GetWorkbookMetadata<Manufacturer>(workbook, languages);
        var defaultWorksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;
        var localizedProperties = metadata.LocalizedProperties;

        var manager = new PropertyManager<Manufacturer, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

        var iRow = 2;
        var setSeName = defaultProperties.Any(p => p.PropertyName == "SeName");

        while (true)
        {
            var allColumnsAreEmpty = manager.GetDefaultProperties
                .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

            if (allColumnsAreEmpty)
                break;

            manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manager.GetDefaultProperty("Id").IntValue);

            var isNew = manufacturer == null;

            manufacturer ??= new Manufacturer();

            if (isNew)
            {
                manufacturer.CreatedOnUtc = DateTime.UtcNow;

                //default values
                manufacturer.PageSize = _catalogSettings.DefaultManufacturerPageSize;
                manufacturer.PageSizeOptions = _catalogSettings.DefaultManufacturerPageSizeOptions;
                manufacturer.Published = true;
                manufacturer.AllowCustomersToSelectPageSize = true;
            }

            var seName = string.Empty;

            foreach (var property in manager.GetDefaultProperties)
            {
                switch (property.PropertyName)
                {
                    case "Name":
                        manufacturer.Name = property.StringValue;
                        break;
                    case "Description":
                        manufacturer.Description = property.StringValue;
                        break;
                    case "ManufacturerTemplateId":
                        manufacturer.ManufacturerTemplateId = property.IntValue;
                        break;
                    case "MetaKeywords":
                        manufacturer.MetaKeywords = property.StringValue;
                        break;
                    case "MetaDescription":
                        manufacturer.MetaDescription = property.StringValue;
                        break;
                    case "MetaTitle":
                        manufacturer.MetaTitle = property.StringValue;
                        break;
                    case "Picture":
                        var picture = await LoadPictureAsync(manager.GetDefaultProperty("Picture").StringValue, manufacturer.Name, isNew ? null : (int?)manufacturer.PictureId);

                        if (picture != null)
                            manufacturer.PictureId = picture.Id;

                        break;
                    case "PageSize":
                        manufacturer.PageSize = property.IntValue;
                        break;
                    case "AllowCustomersToSelectPageSize":
                        manufacturer.AllowCustomersToSelectPageSize = property.BooleanValue;
                        break;
                    case "PageSizeOptions":
                        manufacturer.PageSizeOptions = property.StringValue;
                        break;
                    case "PriceRangeFiltering":
                        manufacturer.PriceRangeFiltering = property.BooleanValue;
                        break;
                    case "PriceFrom":
                        manufacturer.PriceFrom = property.DecimalValue;
                        break;
                    case "PriceTo":
                        manufacturer.PriceTo = property.DecimalValue;
                        break;
                    case "AutomaticallyCalculatePriceRange":
                        manufacturer.ManuallyPriceRange = property.BooleanValue;
                        break;
                    case "Published":
                        manufacturer.Published = property.BooleanValue;
                        break;
                    case "DisplayOrder":
                        manufacturer.DisplayOrder = property.IntValue;
                        break;
                    case "SeName":
                        seName = property.StringValue;
                        break;
                }
            }

            manufacturer.UpdatedOnUtc = DateTime.UtcNow;

            if (isNew)
                await _manufacturerService.InsertManufacturerAsync(manufacturer);
            else
                await _manufacturerService.UpdateManufacturerAsync(manufacturer);

            //search engine name
            if (setSeName)
                await _urlRecordService.SaveSlugAsync(manufacturer, await _urlRecordService.ValidateSeNameAsync(manufacturer, seName, manufacturer.Name, true), 0);

            //save manufacturer localized data
            await ImportManufaturerLocalizedAsync(manufacturer, metadata, manager, iRow, languages);

            iRow++;
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("ImportManufacturers",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportManufacturers"), iRow - 2));
    }

    /// <summary>
    /// Import categories from XLSX file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ImportCategoriesFromXlsxAsync(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);

        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        //the columns
        var metadata = GetWorkbookMetadata<Category>(workbook, languages);
        var defaultWorksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;
        var localizedProperties = metadata.LocalizedProperties;

        var manager = new PropertyManager<Category, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

        var iRow = 2;
        var setSeName = defaultProperties.Any(p => p.PropertyName == "SeName");

        //performance optimization, load all categories in one SQL request
        var allCategories = await (await _categoryService
                .GetAllCategoriesAsync(showHidden: true))
            .GroupByAwait(async c => await _categoryService.GetFormattedBreadCrumbAsync(c))
            .ToDictionaryAsync(c => c.Key, c => c.FirstAsync());

        var saveNextTime = new List<int>();

        while (true)
        {
            var allColumnsAreEmpty = manager.GetDefaultProperties
                .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                .All(cell => string.IsNullOrEmpty(cell?.Value.ToString()));

            if (allColumnsAreEmpty)
                break;

            //get category by data in xlsx file if it possible, or create new category
            var (category, isNew, currentCategoryBreadCrumb) = await GetCategoryFromXlsxAsync(manager, defaultWorksheet, iRow, allCategories);

            //update category by data in xlsx file
            var (seName, isParentCategoryExists) = await UpdateCategoryByXlsxAsync(category, manager, allCategories, isNew);

            if (isParentCategoryExists)
            {
                //if parent category exists in database then save category into database
                await SaveCategoryAsync(isNew, category, allCategories, currentCategoryBreadCrumb, setSeName, seName);

                //save category localized data
                await ImportCategoryLocalizedAsync(category, metadata, manager, iRow, languages);
            }
            else
            {
                //if parent category doesn't exists in database then try save category into database next time
                saveNextTime.Add(iRow);
            }

            iRow++;
        }

        var needSave = saveNextTime.Any();

        while (needSave)
        {
            var remove = new List<int>();

            //try to save unsaved categories
            foreach (var rowId in saveNextTime)
            {
                //get category by data in xlsx file if it possible, or create new category
                var (category, isNew, currentCategoryBreadCrumb) = await GetCategoryFromXlsxAsync(manager, defaultWorksheet, rowId, allCategories);
                //update category by data in xlsx file
                var (seName, isParentCategoryExists) = await UpdateCategoryByXlsxAsync(category, manager, allCategories, isNew);

                if (!isParentCategoryExists)
                    continue;

                //if parent category exists in database then save category into database
                await SaveCategoryAsync(isNew, category, allCategories, currentCategoryBreadCrumb, setSeName, seName);

                //save category localized data
                await ImportCategoryLocalizedAsync(category, metadata, manager, rowId, languages);

                remove.Add(rowId);
            }

            saveNextTime.RemoveAll(remove.Contains);

            needSave = remove.Any() && saveNextTime.Any();
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("ImportCategories",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportCategories"), iRow - 2 - saveNextTime.Count));

        if (!saveNextTime.Any())
            return;

        var categoriesName = new List<string>();

        foreach (var rowId in saveNextTime)
        {
            manager.ReadDefaultFromXlsx(defaultWorksheet, rowId);
            categoriesName.Add(manager.GetDefaultProperty("Name").StringValue);
        }

        throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Import.CategoriesArentImported"), string.Join(", ", categoriesName)));
    }

    /// <summary>
    /// Import orders from XLSX file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ImportOrdersFromXlsxAsync(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);

        var (metadata, worksheet) = await PrepareImportOrderDataAsync(workbook);

        //performance optimization, load all orders by guid in one SQL request
        var allOrdersByGuids = await _orderService.GetOrdersByGuidsAsync(metadata.AllOrderGuids.ToArray());

        //performance optimization, load all customers by guid in one SQL request
        var allCustomersByGuids = await _customerService.GetCustomersByGuidsAsync(metadata.AllCustomerGuids.ToArray());

        Order lastLoadedOrder = null;

        for (var iRow = 2; iRow < metadata.EndRow; iRow++)
        {
            //imports product attributes
            if (worksheet.Row(iRow).OutlineLevel != 0)
            {
                if (lastLoadedOrder == null)
                    continue;

                metadata.OrderItemManager.ReadDefaultFromXlsx(worksheet, iRow, 2);

                //skip caption row
                if (!metadata.OrderItemManager.IsCaption) 
                    await ImportOrderItemAsync(metadata.OrderItemManager, lastLoadedOrder);

                continue;
            }

            metadata.Manager.ReadDefaultFromXlsx(worksheet, iRow);

            var order = allOrdersByGuids.FirstOrDefault(p => p.OrderGuid == metadata.Manager.GetDefaultProperty("OrderGuid").GuidValue);

            var isNew = order == null;

            order ??= new Order();

            Address orderBillingAddress = null;
            Address orderAddress = null;

            if (isNew)
                order.CreatedOnUtc = DateTime.UtcNow;
            else
            {
                orderBillingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
                orderAddress = await _addressService.GetAddressByIdAsync((order.PickupInStore ? order.PickupAddressId : order.ShippingAddressId) ?? 0);
            }

            orderBillingAddress ??= new Address();
            orderAddress ??= new Address();

            var customer = allCustomersByGuids.FirstOrDefault(p => p.CustomerGuid.ToString() == metadata.Manager.GetDefaultProperty("CustomerGuid").StringValue);

            var billingStateProvinceAbbreviation = string.Empty;
            var shippingStateProvinceAbbreviation = string.Empty;

            foreach (var property in metadata.Manager.GetDefaultProperties)
                switch (property.PropertyName)
                {
                    case "StoreId":
                        if (await _storeService.GetStoreByIdAsync(property.IntValue) is { })
                            order.StoreId = property.IntValue;
                        else
                            order.StoreId = (await _storeContext.GetCurrentStoreAsync())?.Id ?? 0;
                        break;
                    case "OrderGuid":
                        order.OrderGuid = property.GuidValue;
                        break;
                    case "CustomerId":
                        order.CustomerId = customer?.Id ?? 0;
                        break;
                    case "OrderStatus":
                        order.OrderStatus = (OrderStatus)property.PropertyValue;
                        break;
                    case "PaymentStatus":
                        order.PaymentStatus = (PaymentStatus)property.PropertyValue;
                        break;
                    case "ShippingStatus":
                        order.ShippingStatus = (ShippingStatus)property.PropertyValue;
                        break;
                    case "OrderSubtotalInclTax":
                        order.OrderSubtotalInclTax = property.DecimalValue;
                        break;
                    case "OrderSubtotalExclTax":
                        order.OrderSubtotalExclTax = property.DecimalValue;
                        break;
                    case "OrderSubTotalDiscountInclTax":
                        order.OrderSubTotalDiscountInclTax = property.DecimalValue;
                        break;
                    case "OrderSubTotalDiscountExclTax":
                        order.OrderSubTotalDiscountExclTax = property.DecimalValue;
                        break;
                    case "OrderShippingInclTax":
                        order.OrderShippingInclTax = property.DecimalValue;
                        break;
                    case "OrderShippingExclTax":
                        order.OrderShippingExclTax = property.DecimalValue;
                        break;
                    case "PaymentMethodAdditionalFeeInclTax":
                        order.PaymentMethodAdditionalFeeInclTax = property.DecimalValue;
                        break;
                    case "PaymentMethodAdditionalFeeExclTax":
                        order.PaymentMethodAdditionalFeeExclTax = property.DecimalValue;
                        break;
                    case "TaxRates":
                        order.TaxRates = property.StringValue;
                        break;
                    case "OrderTax":
                        order.OrderTax = property.DecimalValue;
                        break;
                    case "OrderTotal":
                        order.OrderTotal = property.DecimalValue;
                        break;
                    case "RefundedAmount":
                        order.RefundedAmount = property.DecimalValue;
                        break;
                    case "OrderDiscount":
                        order.OrderDiscount = property.DecimalValue;
                        break;
                    case "CurrencyRate":
                        order.CurrencyRate = property.DecimalValue;
                        break;
                    case "CustomerCurrencyCode":
                        order.CustomerCurrencyCode = property.StringValue;
                        break;
                    case "AffiliateId":
                        order.AffiliateId = property.IntValue;
                        break;
                    case "PaymentMethodSystemName":
                        order.PaymentMethodSystemName = property.StringValue;
                        break;
                    case "ShippingPickupInStore":
                        order.PickupInStore = property.BooleanValue;
                        break;
                    case "ShippingMethod":
                        order.ShippingMethod = property.StringValue;
                        break;
                    case "ShippingRateComputationMethodSystemName":
                        order.ShippingRateComputationMethodSystemName = property.StringValue;
                        break;
                    case "CustomValuesXml":
                        order.CustomValuesXml = property.StringValue;
                        break;
                    case "VatNumber":
                        order.VatNumber = property.StringValue;
                        break;
                    case "CreatedOnUtc":
                        order.CreatedOnUtc = DateTime.TryParse(property.StringValue, out var createdOnUtc) ? createdOnUtc : DateTime.UtcNow;
                        break;
                    case "BillingFirstName":
                        orderBillingAddress.FirstName = property.StringValue;
                        break;
                    case "BillingLastName":
                        orderBillingAddress.LastName = property.StringValue;
                        break;
                    case "BillingPhoneNumber":
                        orderBillingAddress.PhoneNumber = property.StringValue;
                        break;
                    case "BillingEmail":
                        orderBillingAddress.Email = property.StringValue;
                        break;
                    case "BillingFaxNumber":
                        orderBillingAddress.FaxNumber = property.StringValue;
                        break;
                    case "BillingCompany":
                        orderBillingAddress.Company = property.StringValue;
                        break;
                    case "BillingAddress1":
                        orderBillingAddress.Address1 = property.StringValue;
                        break;
                    case "BillingAddress2":
                        orderBillingAddress.Address2 = property.StringValue;
                        break;
                    case "BillingCity":
                        orderBillingAddress.City = property.StringValue;
                        break;
                    case "BillingCounty":
                        orderBillingAddress.County = property.StringValue;
                        break;
                    case "BillingStateProvinceAbbreviation":
                        billingStateProvinceAbbreviation = property.StringValue;
                        
                        break;
                    case "BillingZipPostalCode":
                        orderBillingAddress.ZipPostalCode = property.StringValue;
                        break;
                    case "BillingCountryCode":
                        if (await _countryService.GetCountryByTwoLetterIsoCodeAsync(property.StringValue) is { } billingCountry)
                            orderBillingAddress.CountryId = billingCountry.Id;
                        break;
                    case "ShippingFirstName":
                        orderAddress.FirstName = property.StringValue;
                        break;
                    case "ShippingLastName":
                        orderAddress.LastName = property.StringValue;
                        break;
                    case "ShippingPhoneNumber":
                        orderAddress.PhoneNumber = property.StringValue;
                        break;
                    case "ShippingEmail":
                        orderAddress.Email = property.StringValue;
                        break;
                    case "ShippingFaxNumber":
                        orderAddress.FaxNumber = property.StringValue;
                        break;
                    case "ShippingCompany":
                        orderAddress.Company = property.StringValue;
                        break;
                    case "ShippingAddress1":
                        orderAddress.Address1 = property.StringValue;
                        break;
                    case "ShippingAddress2":
                        orderAddress.Address2 = property.StringValue;
                        break;
                    case "ShippingCity":
                        orderAddress.City = property.StringValue;
                        break;
                    case "ShippingCounty":
                        orderAddress.County = property.StringValue;
                        break;
                    case "ShippingStateProvinceAbbreviation":
                        shippingStateProvinceAbbreviation = property.StringValue;
                        break;
                    case "ShippingZipPostalCode":
                        orderAddress.ZipPostalCode = property.StringValue;
                        break;
                    case "ShippingCountryCode":
                        if (await _countryService.GetCountryByTwoLetterIsoCodeAsync(property.StringValue) is { } shippingCountry)
                            orderAddress.CountryId = shippingCountry.Id;
                        break;
                }

            if (await _stateProvinceService.GetStateProvinceByAbbreviationAsync(billingStateProvinceAbbreviation, orderBillingAddress.CountryId) is { } billingState)
                orderBillingAddress.StateProvinceId = billingState.Id;

            if (await _stateProvinceService.GetStateProvinceByAbbreviationAsync(shippingStateProvinceAbbreviation, orderAddress.CountryId) is { } shippingState)
                orderAddress.StateProvinceId = shippingState.Id;

            //check order address field values from excel
            if (string.IsNullOrWhiteSpace(orderAddress.FirstName) && string.IsNullOrWhiteSpace(orderAddress.LastName) && string.IsNullOrWhiteSpace(orderAddress.Email))
                orderAddress = null;

            //insert or update billing address
            if (orderBillingAddress.Id == 0)
            {
                await _addressService.InsertAddressAsync(orderBillingAddress);
                order.BillingAddressId = orderBillingAddress.Id;
            }
            else
                await _addressService.UpdateAddressAsync(orderBillingAddress);

            //insert or update shipping/pickup address
            if (orderAddress != null)
            {
                if (orderAddress.Id == 0)
                {
                    await _addressService.InsertAddressAsync(orderAddress);

                    if (order.PickupInStore)
                        order.PickupAddressId = orderAddress.Id;
                    else
                        order.ShippingAddressId = orderAddress.Id;
                }
                else
                    await _addressService.UpdateAddressAsync(orderAddress);
            }
            else
                order.ShippingAddressId = null;

            //set some default values if not specified
            if (isNew)
            {
                //customer language
                var customerLanguage = await _languageService.GetLanguageByIdAsync(customer?.LanguageId ?? 0);
                if (customerLanguage == null || !customerLanguage.Published)
                    customerLanguage = await _workContext.GetWorkingLanguageAsync();
                order.CustomerLanguageId = customerLanguage.Id;

                //tax display type
                order.CustomerTaxDisplayType = _taxSettings.TaxDisplayType;

                //set other default values
                order.AllowStoringCreditCardNumber = false;
                order.AuthorizationTransactionCode = string.Empty;
                order.AuthorizationTransactionId = string.Empty;
                order.AuthorizationTransactionResult = string.Empty;
                order.CaptureTransactionId = string.Empty;
                order.CaptureTransactionResult = string.Empty;
                order.CardCvv2 = string.Empty;
                order.CardExpirationMonth = string.Empty;
                order.CardExpirationYear = string.Empty;
                order.CardName = string.Empty;
                order.CardNumber = string.Empty;
                order.CardType = string.Empty;
                order.CustomerIp = string.Empty;
                order.CustomOrderNumber = string.Empty;
                order.MaskedCreditCardNumber = string.Empty;
                order.RefundedAmount = decimal.Zero;
                order.SubscriptionTransactionId = string.Empty;

                await _orderService.InsertOrderAsync(order);

                //generate and set custom order number
                order.CustomOrderNumber = _customNumberFormatter.GenerateOrderCustomNumber(order);
                await _orderService.UpdateOrderAsync(order);
            }
            else
                await _orderService.UpdateOrderAsync(order);

            lastLoadedOrder = order;
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("ImportOrders", string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportOrders"), metadata.CountOrdersInFile));
    }

    #endregion

    #region Nested classes

    protected partial class ProductPictureMetadata
    {
        public Product ProductItem { get; set; }

        public string Picture1Path { get; set; }

        public string Picture2Path { get; set; }

        public string Picture3Path { get; set; }

        public bool IsNew { get; set; }
    }

    public partial class CategoryKey
    {
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task<CategoryKey> CreateCategoryKeyAsync(Category category, ICategoryService categoryService, IList<Category> allCategories, IStoreMappingService storeMappingService)
        {
            return new CategoryKey(await categoryService.GetFormattedBreadCrumbAsync(category, allCategories), category.LimitedToStores ? (await storeMappingService.GetStoresIdsWithAccessAsync(category)).ToList() : new List<int>())
            {
                Category = category
            };
        }

        public CategoryKey(string key, List<int> storesIds = null)
        {
            Key = key.Trim();
            StoresIds = storesIds ?? new List<int>();
        }

        public List<int> StoresIds { get; }

        public Category Category { get; protected set; }

        public string Key { get; }

        public bool Equals(CategoryKey y)
        {
            if (y == null)
                return false;

            if (Category != null && y.Category != null)
                return Category.Id == y.Category.Id;

            if ((StoresIds.Any() || y.StoresIds.Any())
                && (StoresIds.All(id => !y.StoresIds.Contains(id)) || y.StoresIds.All(id => !StoresIds.Contains(id))))
                return false;

            return Key.Equals(y.Key);
        }

        public override int GetHashCode()
        {
            if (!StoresIds.Any())
                return Key.GetHashCode();

            var storesIds = StoresIds.Select(id => id.ToString())
                .Aggregate(string.Empty, (all, current) => all + current);

            return $"{storesIds}_{Key}".GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is CategoryKey other && other.Equals(this);
        }
    }

    #endregion
}