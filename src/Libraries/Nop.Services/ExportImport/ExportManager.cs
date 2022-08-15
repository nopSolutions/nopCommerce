using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ClosedXML.Excel;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
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
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager : IExportManager
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly CustomerSettings _customerSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IAddressService _addressService;
        private readonly ICategoryService _categoryService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerAttributeFormatter _customerAttributeFormatter;
        private readonly ICustomerService _customerService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDiscountService _discountService;
        private readonly IForumService _forumService;
        private readonly IGdprService _gdprService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IMeasureService _measureService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IShipmentService _shipmentService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly ProductEditorSettings _productEditorSettings;

        #endregion

        #region Ctor

        public ExportManager(AddressSettings addressSettings,
            CatalogSettings catalogSettings,
            ICustomerActivityService customerActivityService,
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            ForumSettings forumSettings,
            IAddressService addressService,
            ICategoryService categoryService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerAttributeFormatter customerAttributeFormatter,
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IDiscountService discountService,
            IForumService forumService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IMeasureService measureService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
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
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            ProductEditorSettings productEditorSettings)
        {
            _addressSettings = addressSettings;
            _catalogSettings = catalogSettings;
            _customerActivityService = customerActivityService;
            _customerSettings = customerSettings;
            _dateTimeSettings = dateTimeSettings;
            _addressService = addressService;
            _forumSettings = forumSettings;
            _categoryService = categoryService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerAttributeFormatter = customerAttributeFormatter;
            _customerService = customerService;
            _dateRangeService = dateRangeService;
            _dateTimeHelper = dateTimeHelper;
            _discountService = discountService;
            _forumService = forumService;
            _gdprService = gdprService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _measureService = measureService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
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

            foreach (var category in categories)
            {
                await xmlWriter.WriteStartElementAsync("Category");

                await xmlWriter.WriteStringAsync("Id", category.Id);

                await xmlWriter.WriteStringAsync("Name", category.Name);
                await xmlWriter.WriteStringAsync("Description", category.Description);
                await xmlWriter.WriteStringAsync("CategoryTemplateId", category.CategoryTemplateId);
                await xmlWriter.WriteStringAsync("MetaKeywords", category.MetaKeywords, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("MetaDescription", category.MetaDescription, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("MetaTitle", category.MetaTitle, await IgnoreExportCategoryPropertyAsync());
                await xmlWriter.WriteStringAsync("SeName", await _urlRecordService.GetSeNameAsync(category, 0), await IgnoreExportCategoryPropertyAsync());
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
                await xmlWriter.WriteStringAsync("IncludeInTopMenu", category.IncludeInTopMenu, await IgnoreExportCategoryPropertyAsync());
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
                    await xmlWriter.WriteStringAsync("ProductName", product.Name);
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

            return await _pictureService.GetThumbLocalPathAsync(picture);
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
        /// Returns the list of limited to stores for a product separated by a ";"
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of store
        /// </returns>
        protected virtual async Task<object> GetLimitedToStoresAsync(Product product)
        {
            string limitedToStores = null;
            foreach (var storeMapping in await _storeMappingService.GetStoreMappingsAsync(product))
            {
                var store = await _storeService.GetStoreByIdAsync(storeMapping.StoreId);

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
            
            return pictures.Count > pictureIndex ? await _pictureService.GetThumbLocalPathAsync(pictures[pictureIndex]) : null;
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
        protected virtual async Task<bool> IgnoreExportLimitedToStoreAsync()
        {
            return _catalogSettings.IgnoreStoreLimitations ||
                   !_catalogSettings.ExportImportProductUseLimitedToStores ||
                   (await _storeService.GetAllStoresAsync()).Count == 1;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<PropertyManager<ExportProductAttribute>> GetProductAttributeManagerAsync()
        {
            var attributeProperties = new[]
            {
                new PropertyByName<ExportProductAttribute>("AttributeId", p => p.AttributeId),
                new PropertyByName<ExportProductAttribute>("AttributeName", p => p.AttributeName),
                new PropertyByName<ExportProductAttribute>("DefaultValue", p => p.DefaultValue),
                new PropertyByName<ExportProductAttribute>("ValidationMinLength", p => p.ValidationMinLength),
                new PropertyByName<ExportProductAttribute>("ValidationMaxLength", p => p.ValidationMaxLength),
                new PropertyByName<ExportProductAttribute>("ValidationFileAllowedExtensions", p => p.ValidationFileAllowedExtensions),
                new PropertyByName<ExportProductAttribute>("ValidationFileMaximumSize", p => p.ValidationFileMaximumSize),
                new PropertyByName<ExportProductAttribute>("AttributeTextPrompt", p => p.AttributeTextPrompt),
                new PropertyByName<ExportProductAttribute>("AttributeIsRequired", p => p.AttributeIsRequired),
                new PropertyByName<ExportProductAttribute>("AttributeControlType", p => p.AttributeControlTypeId)
                {
                    DropDownElements = await AttributeControlType.TextBox.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<ExportProductAttribute>("AttributeDisplayOrder", p => p.AttributeDisplayOrder),
                new PropertyByName<ExportProductAttribute>("ProductAttributeValueId", p => p.Id),
                new PropertyByName<ExportProductAttribute>("ValueName", p => p.Name),
                new PropertyByName<ExportProductAttribute>("AttributeValueType", p => p.AttributeValueTypeId)
                {
                    DropDownElements = await AttributeValueType.Simple.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<ExportProductAttribute>("AssociatedProductId", p => p.AssociatedProductId),
                new PropertyByName<ExportProductAttribute>("ColorSquaresRgb", p => p.ColorSquaresRgb),
                new PropertyByName<ExportProductAttribute>("ImageSquaresPictureId", p => p.ImageSquaresPictureId),
                new PropertyByName<ExportProductAttribute>("PriceAdjustment", p => p.PriceAdjustment),
                new PropertyByName<ExportProductAttribute>("PriceAdjustmentUsePercentage", p => p.PriceAdjustmentUsePercentage),
                new PropertyByName<ExportProductAttribute>("WeightAdjustment", p => p.WeightAdjustment),
                new PropertyByName<ExportProductAttribute>("Cost", p => p.Cost),
                new PropertyByName<ExportProductAttribute>("CustomerEntersQty", p => p.CustomerEntersQty),
                new PropertyByName<ExportProductAttribute>("Quantity", p => p.Quantity),
                new PropertyByName<ExportProductAttribute>("IsPreSelected", p => p.IsPreSelected),
                new PropertyByName<ExportProductAttribute>("DisplayOrder", p => p.DisplayOrder),
                new PropertyByName<ExportProductAttribute>("PictureId", p => p.PictureId)
            };

            return new PropertyManager<ExportProductAttribute>(attributeProperties, _catalogSettings);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<PropertyManager<ExportSpecificationAttribute>> GetSpecificationAttributeManagerAsync()
        {
            var attributeProperties = new[]
            {
                new PropertyByName<ExportSpecificationAttribute>("AttributeType", p => p.AttributeTypeId)
                {
                    DropDownElements = await SpecificationAttributeType.Option.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<ExportSpecificationAttribute>("SpecificationAttribute", p => p.SpecificationAttributeId)
                {
                    DropDownElements = (await _specificationAttributeService.GetSpecificationAttributesAsync()).Select(sa => sa as BaseEntity).ToSelectList(p => (p as SpecificationAttribute)?.Name ?? string.Empty)
                },
                new PropertyByName<ExportSpecificationAttribute>("CustomValue", p => p.CustomValue),
                new PropertyByName<ExportSpecificationAttribute>("SpecificationAttributeOptionId", p => p.SpecificationAttributeOptionId),
                new PropertyByName<ExportSpecificationAttribute>("AllowFiltering", p => p.AllowFiltering),
                new PropertyByName<ExportSpecificationAttribute>("ShowOnProductPage", p => p.ShowOnProductPage),
                new PropertyByName<ExportSpecificationAttribute>("DisplayOrder", p => p.DisplayOrder)
            };

            return new PropertyManager<ExportSpecificationAttribute>(attributeProperties, _catalogSettings);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<byte[]> ExportProductsToXlsxWithAttributesAsync(PropertyByName<Product>[] properties, IEnumerable<Product> itemsToExport)
        {
            var productAttributeManager = await GetProductAttributeManagerAsync();
            var specificationAttributeManager = await GetSpecificationAttributeManagerAsync();

            await using var stream = new MemoryStream();
            // ok, we can run the real code of the sample now
            using (var workbook = new XLWorkbook())
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handles to the worksheets
                // Worksheet names cannot be more than 31 characters
                var worksheet = workbook.Worksheets.Add(typeof(Product).Name);
                var fpWorksheet = workbook.Worksheets.Add("ProductsFilters");
                fpWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;
                var fbaWorksheet = workbook.Worksheets.Add("ProductAttributesFilters");
                fbaWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;
                var fsaWorksheet = workbook.Worksheets.Add("SpecificationAttributesFilters");
                fsaWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;

                //create Headers and format them 
                var manager = new PropertyManager<Product>(properties, _catalogSettings);
                manager.WriteCaption(worksheet);

                var row = 2;
                foreach (var item in itemsToExport)
                {
                    manager.CurrentObject = item;
                    await manager.WriteToXlsxAsync(worksheet, row++, fWorksheet: fpWorksheet);

                    if (_catalogSettings.ExportImportProductAttributes) 
                        row = await ExportProductAttributesAsync(item, productAttributeManager, worksheet, row, fbaWorksheet);

                    if (_catalogSettings.ExportImportProductSpecificationAttributes) 
                        row = await ExportSpecificationAttributesAsync(item, specificationAttributeManager, worksheet, row, fsaWorksheet);
                }

                workbook.SaveAs(stream);
            }

            return stream.ToArray();
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<int> ExportProductAttributesAsync(Product item, PropertyManager<ExportProductAttribute> attributeManager, IXLWorksheet worksheet, int row, IXLWorksheet faWorksheet)
        {
            var attributes = await (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(item.Id))
                .SelectManyAwait(async pam =>
                {
                    var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);

                    var values = await _productAttributeService.GetProductAttributeValuesAsync(pam.Id);

                    if (values?.Any() ?? false)
                        return values.Select(pav =>
                            new ExportProductAttribute
                            {
                                AttributeId = productAttribute.Id,
                                AttributeName = productAttribute.Name,
                                AttributeTextPrompt = pam.TextPrompt,
                                AttributeIsRequired = pam.IsRequired,
                                AttributeControlTypeId = pam.AttributeControlTypeId,
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
                                PictureId = pav.PictureId
                            });

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
                        return new List<ExportProductAttribute> {attribute};

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

            attributeManager.WriteCaption(worksheet, row, ExportProductAttribute.ProductAttributeCellOffset);
            worksheet.Row(row).OutlineLevel = 1;
            worksheet.Row(row).Collapse();

            foreach (var exportProductAttribute in attributes)
            {
                row++;
                attributeManager.CurrentObject = exportProductAttribute;
                await attributeManager.WriteToXlsxAsync(worksheet, row, ExportProductAttribute.ProductAttributeCellOffset, faWorksheet);
                worksheet.Row(row).OutlineLevel = 1;
                worksheet.Row(row).Collapse();
            }

            return row + 1;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<int> ExportSpecificationAttributesAsync(Product item, PropertyManager<ExportSpecificationAttribute> attributeManager, IXLWorksheet worksheet, int row, IXLWorksheet faWorksheet)
        {
            var attributes = await (await _specificationAttributeService
                .GetProductSpecificationAttributesAsync(item.Id)).SelectAwait(
                async psa => new ExportSpecificationAttribute
                {
                    AttributeTypeId = psa.AttributeTypeId,
                    CustomValue = psa.CustomValue,
                    AllowFiltering = psa.AllowFiltering,
                    ShowOnProductPage = psa.ShowOnProductPage,
                    DisplayOrder = psa.DisplayOrder,
                    SpecificationAttributeOptionId = psa.SpecificationAttributeOptionId,
                    SpecificationAttributeId = (await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(psa.SpecificationAttributeOptionId)).SpecificationAttributeId
                }).ToListAsync();

            if (!attributes.Any())
                return row;

            attributeManager.WriteCaption(worksheet, row, ExportProductAttribute.ProductAttributeCellOffset);
            worksheet.Row(row).OutlineLevel = 1;
            worksheet.Row(row).Collapse();

            foreach (var exportProductAttribute in attributes)
            {
                row++;
                attributeManager.CurrentObject = exportProductAttribute;
                await attributeManager.WriteToXlsxAsync(worksheet, row, ExportProductAttribute.ProductAttributeCellOffset, faWorksheet);
                worksheet.Row(row).OutlineLevel = 1;
                worksheet.Row(row).Collapse();
            }

            return row + 1;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<byte[]> ExportOrderToXlsxWithProductsAsync(PropertyByName<Order>[] properties, IEnumerable<Order> itemsToExport)
        {
            var orderItemProperties = new[]
            {
                new PropertyByName<OrderItem>("OrderItemGuid", oi => oi.OrderItemGuid),
                new PropertyByName<OrderItem>("Name", async oi => (await _productService.GetProductByIdAsync(oi.ProductId)).Name),
                new PropertyByName<OrderItem>("Sku", async oi => await _productService.FormatSkuAsync(await _productService.GetProductByIdAsync(oi.ProductId), oi.AttributesXml)),
                new PropertyByName<OrderItem>("PriceExclTax", oi => oi.UnitPriceExclTax),
                new PropertyByName<OrderItem>("PriceInclTax", oi => oi.UnitPriceInclTax),
                new PropertyByName<OrderItem>("Quantity", oi => oi.Quantity),
                new PropertyByName<OrderItem>("DiscountExclTax", oi => oi.DiscountAmountExclTax),
                new PropertyByName<OrderItem>("DiscountInclTax", oi => oi.DiscountAmountInclTax),
                new PropertyByName<OrderItem>("TotalExclTax", oi => oi.PriceExclTax),
                new PropertyByName<OrderItem>("TotalInclTax", oi => oi.PriceInclTax)
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
                manager.WriteCaption(worksheet);

                var row = 2;
                foreach (var order in itemsToExport)
                {
                    manager.CurrentObject = order;
                    await manager.WriteToXlsxAsync(worksheet, row++);

                    //a vendor should have access only to his products
                    var vendor = await _workContext.GetCurrentVendorAsync();
                    var orderItems = await _orderService.GetOrderItemsAsync(order.Id, vendorId: vendor?.Id ?? 0);

                    if (!orderItems.Any())
                        continue;

                    orderItemsManager.WriteCaption(worksheet, row, 2);
                    worksheet.Row(row).OutlineLevel = 1;
                    worksheet.Row(row).Collapse();

                    foreach (var orderItem in orderItems)
                    {
                        row++;
                        orderItemsManager.CurrentObject = orderItem;
                        await orderItemsManager.WriteToXlsxAsync(worksheet, row, 2, fpWorksheet);
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
        private async Task<object> GetCustomCustomerAttributesAsync(Customer customer)
        {
            return await _customerAttributeFormatter.FormatAttributesAsync(customer.CustomCustomerAttributesXML, ";");
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

            foreach (var manufacturer in manufacturers)
            {
                await xmlWriter.WriteStartElementAsync("Manufacturer");

                await xmlWriter.WriteStringAsync("ManufacturerId", manufacturer.Id.ToString());
                await xmlWriter.WriteStringAsync("Name", manufacturer.Name);
                await xmlWriter.WriteStringAsync("Description", manufacturer.Description);
                await xmlWriter.WriteStringAsync("ManufacturerTemplateId", manufacturer.ManufacturerTemplateId);
                await xmlWriter.WriteStringAsync("MetaKeywords", manufacturer.MetaKeywords, await IgnoreExportManufacturerPropertyAsync());
                await xmlWriter.WriteStringAsync("MetaDescription", manufacturer.MetaDescription, await IgnoreExportManufacturerPropertyAsync());
                await xmlWriter.WriteStringAsync("MetaTitle", manufacturer.MetaTitle, await IgnoreExportManufacturerPropertyAsync());
                await xmlWriter.WriteStringAsync("SEName", await _urlRecordService.GetSeNameAsync(manufacturer, 0), await IgnoreExportManufacturerPropertyAsync());
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
                        await xmlWriter.WriteStringAsync("ProductName", product.Name);
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
        public virtual async Task<byte[]> ExportManufacturersToXlsxAsync(IEnumerable<Manufacturer> manufacturers)
        {
            //property manager 
            var manager = new PropertyManager<Manufacturer>(new[]
            {
                new PropertyByName<Manufacturer>("Id", p => p.Id),
                new PropertyByName<Manufacturer>("Name", p => p.Name),
                new PropertyByName<Manufacturer>("Description", p => p.Description),
                new PropertyByName<Manufacturer>("ManufacturerTemplateId", p => p.ManufacturerTemplateId),
                new PropertyByName<Manufacturer>("MetaKeywords", p => p.MetaKeywords, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer>("MetaDescription", p => p.MetaDescription, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer>("MetaTitle", p => p.MetaTitle, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer>("SeName", async p => await _urlRecordService.GetSeNameAsync(p, 0), await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer>("Picture", async p => await GetPicturesAsync(p.PictureId)),
                new PropertyByName<Manufacturer>("PageSize", p => p.PageSize, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer>("AllowCustomersToSelectPageSize", p => p.AllowCustomersToSelectPageSize, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer>("PageSizeOptions", p => p.PageSizeOptions, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer>("PriceRangeFiltering", p => p.PriceRangeFiltering, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer>("PriceFrom", p => p.PriceFrom, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer>("PriceTo", p => p.PriceTo, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer>("ManuallyPriceRange", p => p.ManuallyPriceRange, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer>("Published", p => p.Published, await IgnoreExportManufacturerPropertyAsync()),
                new PropertyByName<Manufacturer>("DisplayOrder", p => p.DisplayOrder)
            }, _catalogSettings);

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

            //property manager 
            var manager = new PropertyManager<Category>(new[]
            {
                new PropertyByName<Category>("Id", p => p.Id),
                new PropertyByName<Category>("Name", p => p.Name),
                new PropertyByName<Category>("Description", p => p.Description),
                new PropertyByName<Category>("CategoryTemplateId", p => p.CategoryTemplateId),
                new PropertyByName<Category>("MetaKeywords", p => p.MetaKeywords, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("MetaDescription", p => p.MetaDescription, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("MetaTitle", p => p.MetaTitle, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("SeName", async p => await _urlRecordService.GetSeNameAsync(p, 0), await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("ParentCategoryId", p => p.ParentCategoryId),
                new PropertyByName<Category>("ParentCategoryName", async p =>
                {
                    var category = parentCategories.FirstOrDefault(c => c.Id == p.ParentCategoryId);
                    return category != null ? await _categoryService.GetFormattedBreadCrumbAsync(category) : null;

                }, !_catalogSettings.ExportImportCategoriesUsingCategoryName),
                new PropertyByName<Category>("Picture", async p => await GetPicturesAsync(p.PictureId)),
                new PropertyByName<Category>("PageSize", p => p.PageSize, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("PriceRangeFiltering", p => p.PriceRangeFiltering, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("PriceFrom", p => p.PriceFrom, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("PriceTo", p => p.PriceTo, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("ManuallyPriceRange", p => p.ManuallyPriceRange, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("AllowCustomersToSelectPageSize", p => p.AllowCustomersToSelectPageSize, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("PageSizeOptions", p => p.PageSizeOptions, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("ShowOnHomepage", p => p.ShowOnHomepage, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("IncludeInTopMenu", p => p.IncludeInTopMenu, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("Published", p => p.Published, await IgnoreExportCategoryPropertyAsync()),
                new PropertyByName<Category>("DisplayOrder", p => p.DisplayOrder)
            }, _catalogSettings);

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

            foreach (var product in products)
            {
                await xmlWriter.WriteStartElementAsync("Product");

                await xmlWriter.WriteStringAsync("ProductId", product.Id);
                await xmlWriter.WriteStringAsync("ProductTypeId", product.ProductTypeId, await IgnoreExportProductPropertyAsync(p => p.ProductType));
                await xmlWriter.WriteStringAsync("ParentGroupedProductId", product.ParentGroupedProductId, await IgnoreExportProductPropertyAsync(p => p.ProductType));
                await xmlWriter.WriteStringAsync("VisibleIndividually", product.VisibleIndividually, await IgnoreExportProductPropertyAsync(p => p.VisibleIndividually));
                await xmlWriter.WriteStringAsync("Name", product.Name);
                await xmlWriter.WriteStringAsync("ShortDescription", product.ShortDescription);
                await xmlWriter.WriteStringAsync("FullDescription", product.FullDescription);
                await xmlWriter.WriteStringAsync("AdminComment", product.AdminComment, await IgnoreExportProductPropertyAsync(p => p.AdminComment));
                //vendor can't change this field
                await xmlWriter.WriteStringAsync("VendorId", product.VendorId, await IgnoreExportProductPropertyAsync(p => p.Vendor) || currentVendor != null);
                await xmlWriter.WriteStringAsync("ProductTemplateId", product.ProductTemplateId, await IgnoreExportProductPropertyAsync(p => p.ProductTemplate));
                //vendor can't change this field
                await xmlWriter.WriteStringAsync("ShowOnHomepage", product.ShowOnHomepage, await IgnoreExportProductPropertyAsync(p => p.ShowOnHomepage) || currentVendor != null);
                //vendor can't change this field
                await xmlWriter.WriteStringAsync("DisplayOrder", product.DisplayOrder, await IgnoreExportProductPropertyAsync(p => p.ShowOnHomepage) || currentVendor != null);
                await xmlWriter.WriteStringAsync("MetaKeywords", product.MetaKeywords, await IgnoreExportProductPropertyAsync(p => p.Seo));
                await xmlWriter.WriteStringAsync("MetaDescription", product.MetaDescription, await IgnoreExportProductPropertyAsync(p => p.Seo));
                await xmlWriter.WriteStringAsync("MetaTitle", product.MetaTitle, await IgnoreExportProductPropertyAsync(p => p.Seo));
                await xmlWriter.WriteStringAsync("SEName", await _urlRecordService.GetSeNameAsync(product, 0), await IgnoreExportProductPropertyAsync(p => p.Seo));
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
                await xmlWriter.WriteStringAsync("IsTelecommunicationsOrBroadcastingOrElectronicServices", product.IsTelecommunicationsOrBroadcastingOrElectronicServices, await IgnoreExportProductPropertyAsync(p => p.TelecommunicationsBroadcastingElectronicServices));
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
                    await  xmlWriter.WriteStartElementAsync("TierPrices");
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
                    await  xmlWriter.WriteStartElementAsync("ProductAttributes");
                    var productAttributMappings =
                        await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
                    foreach (var productAttributeMapping in productAttributMappings)
                    {
                        var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId);

                        await xmlWriter.WriteStartElementAsync("ProductAttributeMapping");
                        await xmlWriter.WriteStringAsync("ProductAttributeMappingId", productAttributeMapping.Id);
                        await xmlWriter.WriteStringAsync("ProductAttributeId", productAttributeMapping.ProductAttributeId);
                        await xmlWriter.WriteStringAsync("ProductAttributeName", productAttribute.Name);
                        await xmlWriter.WriteStringAsync("TextPrompt", productAttributeMapping.TextPrompt);
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

                            await xmlWriter.WriteStringAsync("DefaultValue", productAttributeMapping.DefaultValue);
                        }
                        //conditions
                        await xmlWriter.WriteElementStringAsync("ConditionAttributeXml", productAttributeMapping.ConditionAttributeXml);

                        await xmlWriter.WriteStartElementAsync("ProductAttributeValues");
                        var productAttributeValues = await _productAttributeService.GetProductAttributeValuesAsync(productAttributeMapping.Id);
                        foreach (var productAttributeValue in productAttributeValues)
                        {
                            await xmlWriter.WriteStartElementAsync("ProductAttributeValue");
                            await xmlWriter.WriteStringAsync("ProductAttributeValueId", productAttributeValue.Id);
                            await xmlWriter.WriteStringAsync("Name", productAttributeValue.Name);
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
                            await xmlWriter.WriteStringAsync("PictureId", productAttributeValue.PictureId);
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
            var properties = new[]
            {
                new PropertyByName<Product>("ProductId", p => p.Id),
                new PropertyByName<Product>("ProductType", p => p.ProductTypeId, await IgnoreExportProductPropertyAsync(p => p.ProductType))
                {
                    DropDownElements = await ProductType.SimpleProduct.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Product>("ParentGroupedProductId", p => p.ParentGroupedProductId, await IgnoreExportProductPropertyAsync(p => p.ProductType)),
                new PropertyByName<Product>("VisibleIndividually", p => p.VisibleIndividually, await IgnoreExportProductPropertyAsync(p => p.VisibleIndividually)),
                new PropertyByName<Product>("Name", p => p.Name),
                new PropertyByName<Product>("ShortDescription", p => p.ShortDescription),
                new PropertyByName<Product>("FullDescription", p => p.FullDescription),
                //vendor can't change this field
                new PropertyByName<Product>("Vendor", p => p.VendorId, await IgnoreExportProductPropertyAsync(p => p.Vendor) || currentVendor != null)
                {
                    DropDownElements = (await _vendorService.GetAllVendorsAsync(showHidden: true)).Select(v => v as BaseEntity).ToSelectList(p => (p as Vendor)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("ProductTemplate", p => p.ProductTemplateId, await IgnoreExportProductPropertyAsync(p => p.ProductTemplate))
                {
                    DropDownElements = (await _productTemplateService.GetAllProductTemplatesAsync()).Select(pt => pt as BaseEntity).ToSelectList(p => (p as ProductTemplate)?.Name ?? string.Empty)
                },
                //vendor can't change this field
                new PropertyByName<Product>("ShowOnHomepage", p => p.ShowOnHomepage, await IgnoreExportProductPropertyAsync(p => p.ShowOnHomepage) || currentVendor != null),
                //vendor can't change this field
                new PropertyByName<Product>("DisplayOrder", p => p.DisplayOrder, await IgnoreExportProductPropertyAsync(p => p.ShowOnHomepage) || currentVendor != null),
                new PropertyByName<Product>("MetaKeywords", p => p.MetaKeywords, await IgnoreExportProductPropertyAsync(p => p.Seo)),
                new PropertyByName<Product>("MetaDescription", p => p.MetaDescription, await IgnoreExportProductPropertyAsync(p => p.Seo)),
                new PropertyByName<Product>("MetaTitle", p => p.MetaTitle, await IgnoreExportProductPropertyAsync(p => p.Seo)),
                new PropertyByName<Product>("SeName", async p => await _urlRecordService.GetSeNameAsync(p, 0), await IgnoreExportProductPropertyAsync(p => p.Seo)),
                new PropertyByName<Product>("AllowCustomerReviews", p => p.AllowCustomerReviews, await IgnoreExportProductPropertyAsync(p => p.AllowCustomerReviews)),
                new PropertyByName<Product>("Published", p => p.Published, await IgnoreExportProductPropertyAsync(p => p.Published)),
                new PropertyByName<Product>("SKU", p => p.Sku),
                new PropertyByName<Product>("ManufacturerPartNumber", p => p.ManufacturerPartNumber, await IgnoreExportProductPropertyAsync(p => p.ManufacturerPartNumber)),
                new PropertyByName<Product>("Gtin", p => p.Gtin, await IgnoreExportProductPropertyAsync(p => p.GTIN)),
                new PropertyByName<Product>("IsGiftCard", p => p.IsGiftCard, await IgnoreExportProductPropertyAsync(p => p.IsGiftCard)),
                new PropertyByName<Product>("GiftCardType", p => p.GiftCardTypeId, await IgnoreExportProductPropertyAsync(p => p.IsGiftCard))
                {
                    DropDownElements = await GiftCardType.Virtual.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Product>("OverriddenGiftCardAmount", p => p.OverriddenGiftCardAmount, await IgnoreExportProductPropertyAsync(p => p.IsGiftCard)),
                new PropertyByName<Product>("RequireOtherProducts", p => p.RequireOtherProducts, await IgnoreExportProductPropertyAsync(p => p.RequireOtherProductsAddedToCart)),
                new PropertyByName<Product>("RequiredProductIds", p => p.RequiredProductIds, await IgnoreExportProductPropertyAsync(p => p.RequireOtherProductsAddedToCart)),
                new PropertyByName<Product>("AutomaticallyAddRequiredProducts", p => p.AutomaticallyAddRequiredProducts, await IgnoreExportProductPropertyAsync(p => p.RequireOtherProductsAddedToCart)),
                new PropertyByName<Product>("IsDownload", p => p.IsDownload, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
                new PropertyByName<Product>("DownloadId", p => p.DownloadId, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
                new PropertyByName<Product>("UnlimitedDownloads", p => p.UnlimitedDownloads, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
                new PropertyByName<Product>("MaxNumberOfDownloads", p => p.MaxNumberOfDownloads, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
                new PropertyByName<Product>("DownloadActivationType", p => p.DownloadActivationTypeId, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct))
                {
                    DropDownElements = await DownloadActivationType.Manually.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Product>("HasSampleDownload", p => p.HasSampleDownload, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
                new PropertyByName<Product>("SampleDownloadId", p => p.SampleDownloadId, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
                new PropertyByName<Product>("HasUserAgreement", p => p.HasUserAgreement, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
                new PropertyByName<Product>("UserAgreementText", p => p.UserAgreementText, await IgnoreExportProductPropertyAsync(p => p.DownloadableProduct)),
                new PropertyByName<Product>("IsRecurring", p => p.IsRecurring, await IgnoreExportProductPropertyAsync(p => p.RecurringProduct)),
                new PropertyByName<Product>("RecurringCycleLength", p => p.RecurringCycleLength, await IgnoreExportProductPropertyAsync(p => p.RecurringProduct)),
                new PropertyByName<Product>("RecurringCyclePeriod", p => p.RecurringCyclePeriodId, await IgnoreExportProductPropertyAsync(p => p.RecurringProduct))
                {
                    DropDownElements = await RecurringProductCyclePeriod.Days.ToSelectListAsync(useLocalization: false),
                    AllowBlank = true
                },
                new PropertyByName<Product>("RecurringTotalCycles", p => p.RecurringTotalCycles, await IgnoreExportProductPropertyAsync(p => p.RecurringProduct)),
                new PropertyByName<Product>("IsRental", p => p.IsRental, await IgnoreExportProductPropertyAsync(p => p.IsRental)),
                new PropertyByName<Product>("RentalPriceLength", p => p.RentalPriceLength, await IgnoreExportProductPropertyAsync(p => p.IsRental)),
                new PropertyByName<Product>("RentalPricePeriod", p => p.RentalPricePeriodId, await IgnoreExportProductPropertyAsync(p => p.IsRental))
                {
                    DropDownElements = await RentalPricePeriod.Days.ToSelectListAsync(useLocalization: false),
                    AllowBlank = true
                },
                new PropertyByName<Product>("IsShipEnabled", p => p.IsShipEnabled),
                new PropertyByName<Product>("IsFreeShipping", p => p.IsFreeShipping, await IgnoreExportProductPropertyAsync(p => p.FreeShipping)),
                new PropertyByName<Product>("ShipSeparately", p => p.ShipSeparately, await IgnoreExportProductPropertyAsync(p => p.ShipSeparately)),
                new PropertyByName<Product>("AdditionalShippingCharge", p => p.AdditionalShippingCharge, await IgnoreExportProductPropertyAsync(p => p.AdditionalShippingCharge)),
                new PropertyByName<Product>("DeliveryDate", p => p.DeliveryDateId, await IgnoreExportProductPropertyAsync(p => p.DeliveryDate))
                {
                    DropDownElements = (await _dateRangeService.GetAllDeliveryDatesAsync()).Select(dd => dd as BaseEntity).ToSelectList(p => (p as DeliveryDate)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("IsTaxExempt", p => p.IsTaxExempt),
                new PropertyByName<Product>("TaxCategory", p => p.TaxCategoryId)
                {
                    DropDownElements = (await _taxCategoryService.GetAllTaxCategoriesAsync()).Select(tc => tc as BaseEntity).ToSelectList(p => (p as TaxCategory)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("IsTelecommunicationsOrBroadcastingOrElectronicServices", p => p.IsTelecommunicationsOrBroadcastingOrElectronicServices, await IgnoreExportProductPropertyAsync(p => p.TelecommunicationsBroadcastingElectronicServices)),
                new PropertyByName<Product>("ManageInventoryMethod", p => p.ManageInventoryMethodId)
                {
                    DropDownElements = await ManageInventoryMethod.DontManageStock.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Product>("ProductAvailabilityRange", p => p.ProductAvailabilityRangeId, await IgnoreExportProductPropertyAsync(p => p.ProductAvailabilityRange))
                {
                    DropDownElements = (await _dateRangeService.GetAllProductAvailabilityRangesAsync()).Select(range => range as BaseEntity).ToSelectList(p => (p as ProductAvailabilityRange)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("UseMultipleWarehouses", p => p.UseMultipleWarehouses, await IgnoreExportProductPropertyAsync(p => p.UseMultipleWarehouses)),
                new PropertyByName<Product>("WarehouseId", p => p.WarehouseId, await IgnoreExportProductPropertyAsync(p => p.Warehouse)),
                new PropertyByName<Product>("StockQuantity", p => p.StockQuantity),
                new PropertyByName<Product>("DisplayStockAvailability", p => p.DisplayStockAvailability, await IgnoreExportProductPropertyAsync(p => p.DisplayStockAvailability)),
                new PropertyByName<Product>("DisplayStockQuantity", p => p.DisplayStockQuantity, await IgnoreExportProductPropertyAsync(p => p.DisplayStockAvailability)),
                new PropertyByName<Product>("MinStockQuantity", p => p.MinStockQuantity, await IgnoreExportProductPropertyAsync(p => p.MinimumStockQuantity)),
                new PropertyByName<Product>("LowStockActivity", p => p.LowStockActivityId, await IgnoreExportProductPropertyAsync(p => p.LowStockActivity))
                {
                    DropDownElements = await LowStockActivity.Nothing.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Product>("NotifyAdminForQuantityBelow", p => p.NotifyAdminForQuantityBelow, await IgnoreExportProductPropertyAsync(p => p.NotifyAdminForQuantityBelow)),
                new PropertyByName<Product>("BackorderMode", p => p.BackorderModeId, await IgnoreExportProductPropertyAsync(p => p.Backorders))
                {
                    DropDownElements = await BackorderMode.NoBackorders.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Product>("AllowBackInStockSubscriptions", p => p.AllowBackInStockSubscriptions, await IgnoreExportProductPropertyAsync(p => p.AllowBackInStockSubscriptions)),
                new PropertyByName<Product>("OrderMinimumQuantity", p => p.OrderMinimumQuantity, await IgnoreExportProductPropertyAsync(p => p.MinimumCartQuantity)),
                new PropertyByName<Product>("OrderMaximumQuantity", p => p.OrderMaximumQuantity, await IgnoreExportProductPropertyAsync(p => p.MaximumCartQuantity)),
                new PropertyByName<Product>("AllowedQuantities", p => p.AllowedQuantities, await IgnoreExportProductPropertyAsync(p => p.AllowedQuantities)),
                new PropertyByName<Product>("AllowAddingOnlyExistingAttributeCombinations", p => p.AllowAddingOnlyExistingAttributeCombinations, await IgnoreExportProductPropertyAsync(p => p.AllowAddingOnlyExistingAttributeCombinations)),
                new PropertyByName<Product>("NotReturnable", p => p.NotReturnable, await IgnoreExportProductPropertyAsync(p => p.NotReturnable)),
                new PropertyByName<Product>("DisableBuyButton", p => p.DisableBuyButton, await IgnoreExportProductPropertyAsync(p => p.DisableBuyButton)),
                new PropertyByName<Product>("DisableWishlistButton", p => p.DisableWishlistButton, await IgnoreExportProductPropertyAsync(p => p.DisableWishlistButton)),
                new PropertyByName<Product>("AvailableForPreOrder", p => p.AvailableForPreOrder, await IgnoreExportProductPropertyAsync(p => p.AvailableForPreOrder)),
                new PropertyByName<Product>("PreOrderAvailabilityStartDateTimeUtc", p => p.PreOrderAvailabilityStartDateTimeUtc, await IgnoreExportProductPropertyAsync(p => p.AvailableForPreOrder)),
                new PropertyByName<Product>("CallForPrice", p => p.CallForPrice, await IgnoreExportProductPropertyAsync(p => p.CallForPrice)),
                new PropertyByName<Product>("Price", p => p.Price),
                new PropertyByName<Product>("OldPrice", p => p.OldPrice, await IgnoreExportProductPropertyAsync(p => p.OldPrice)),
                new PropertyByName<Product>("ProductCost", p => p.ProductCost, await IgnoreExportProductPropertyAsync(p => p.ProductCost)),
                new PropertyByName<Product>("CustomerEntersPrice", p => p.CustomerEntersPrice, await IgnoreExportProductPropertyAsync(p => p.CustomerEntersPrice)),
                new PropertyByName<Product>("MinimumCustomerEnteredPrice", p => p.MinimumCustomerEnteredPrice, await IgnoreExportProductPropertyAsync(p => p.CustomerEntersPrice)),
                new PropertyByName<Product>("MaximumCustomerEnteredPrice", p => p.MaximumCustomerEnteredPrice, await IgnoreExportProductPropertyAsync(p => p.CustomerEntersPrice)),
                new PropertyByName<Product>("BasepriceEnabled", p => p.BasepriceEnabled, await IgnoreExportProductPropertyAsync(p => p.PAngV)),
                new PropertyByName<Product>("BasepriceAmount", p => p.BasepriceAmount, await IgnoreExportProductPropertyAsync(p => p.PAngV)),
                new PropertyByName<Product>("BasepriceUnit", p => p.BasepriceUnitId, await IgnoreExportProductPropertyAsync(p => p.PAngV))
                {
                    DropDownElements = (await _measureService.GetAllMeasureWeightsAsync()).Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("BasepriceBaseAmount", p => p.BasepriceBaseAmount, await IgnoreExportProductPropertyAsync(p => p.PAngV)),
                new PropertyByName<Product>("BasepriceBaseUnit", p => p.BasepriceBaseUnitId, await IgnoreExportProductPropertyAsync(p => p.PAngV))
                {
                    DropDownElements = (await _measureService.GetAllMeasureWeightsAsync()).Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("MarkAsNew", p => p.MarkAsNew, await IgnoreExportProductPropertyAsync(p => p.MarkAsNew)),
                new PropertyByName<Product>("MarkAsNewStartDateTimeUtc", p => p.MarkAsNewStartDateTimeUtc, await IgnoreExportProductPropertyAsync(p => p.MarkAsNew)),
                new PropertyByName<Product>("MarkAsNewEndDateTimeUtc", p => p.MarkAsNewEndDateTimeUtc, await IgnoreExportProductPropertyAsync(p => p.MarkAsNew)),
                new PropertyByName<Product>("Weight", p => p.Weight, await IgnoreExportProductPropertyAsync(p => p.Weight)),
                new PropertyByName<Product>("Length", p => p.Length, await IgnoreExportProductPropertyAsync(p => p.Dimensions)),
                new PropertyByName<Product>("Width", p => p.Width, await IgnoreExportProductPropertyAsync(p => p.Dimensions)),
                new PropertyByName<Product>("Height", p => p.Height, await IgnoreExportProductPropertyAsync(p => p.Dimensions)),
                new PropertyByName<Product>("Categories", GetCategoriesAsync),
                new PropertyByName<Product>("Manufacturers", GetManufacturersAsync, await IgnoreExportProductPropertyAsync(p => p.Manufacturers)),
                new PropertyByName<Product>("ProductTags", GetProductTagsAsync, await IgnoreExportProductPropertyAsync(p => p.ProductTags)),
                new PropertyByName<Product>("IsLimitedToStores", p => p.LimitedToStores, await IgnoreExportLimitedToStoreAsync()),
                new PropertyByName<Product>("LimitedToStores", GetLimitedToStoresAsync, await IgnoreExportLimitedToStoreAsync()),
                new PropertyByName<Product>("Picture1", async p => await GetPictureAsync(p, 0)),
                new PropertyByName<Product>("Picture2", async p => await GetPictureAsync(p, 1)),
                new PropertyByName<Product>("Picture3", async p => await GetPictureAsync(p, 2))
            };

            var productList = products.ToList();

            var productAdvancedMode = true;
            try
            {
                productAdvancedMode = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), "product-advanced-mode");
            }
            catch (ArgumentNullException)
            {
            }

            if (!_catalogSettings.ExportImportProductAttributes && !_catalogSettings.ExportImportProductSpecificationAttributes)
                return await new PropertyManager<Product>(properties, _catalogSettings).ExportToXlsxAsync(productList);

            //activity log
            await _customerActivityService.InsertActivityAsync("ExportProducts",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportProducts"), productList.Count));

            if (productAdvancedMode || _productEditorSettings.ProductAttributes)
                return await ExportProductsToXlsxWithAttributesAsync(properties, productList);

            return await new PropertyManager<Product>(properties, _catalogSettings).ExportToXlsxAsync(productList);
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
                new PropertyByName<Order>("OrderId", p => p.Id),
                new PropertyByName<Order>("StoreId", p => p.StoreId),
                new PropertyByName<Order>("OrderGuid", p => p.OrderGuid, ignore),
                new PropertyByName<Order>("CustomerId", p => p.CustomerId, ignore),
                new PropertyByName<Order>("CustomerGuid", async p => (await _customerService.GetCustomerByIdAsync(p.CustomerId))?.CustomerGuid, ignore),
                new PropertyByName<Order>("OrderStatus", p => p.OrderStatusId, ignore)
                {
                    DropDownElements = await OrderStatus.Pending.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Order>("PaymentStatus", p => p.PaymentStatusId, ignore)
                {
                    DropDownElements = await PaymentStatus.Pending.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Order>("ShippingStatus", p => p.ShippingStatusId, ignore)
                {
                    DropDownElements = await ShippingStatus.ShippingNotRequired.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Order>("OrderSubtotalInclTax", p => p.OrderSubtotalInclTax, ignore),
                new PropertyByName<Order>("OrderSubtotalExclTax", p => p.OrderSubtotalExclTax, ignore),
                new PropertyByName<Order>("OrderSubTotalDiscountInclTax", p => p.OrderSubTotalDiscountInclTax, ignore),
                new PropertyByName<Order>("OrderSubTotalDiscountExclTax", p => p.OrderSubTotalDiscountExclTax, ignore),
                new PropertyByName<Order>("OrderShippingInclTax", p => p.OrderShippingInclTax, ignore),
                new PropertyByName<Order>("OrderShippingExclTax", p => p.OrderShippingExclTax, ignore),
                new PropertyByName<Order>("PaymentMethodAdditionalFeeInclTax", p => p.PaymentMethodAdditionalFeeInclTax, ignore),
                new PropertyByName<Order>("PaymentMethodAdditionalFeeExclTax", p => p.PaymentMethodAdditionalFeeExclTax, ignore),
                new PropertyByName<Order>("TaxRates", p => p.TaxRates, ignore),
                new PropertyByName<Order>("OrderTax", p => p.OrderTax, ignore),
                new PropertyByName<Order>("OrderTotal", p => p.OrderTotal, ignore),
                new PropertyByName<Order>("RefundedAmount", p => p.RefundedAmount, ignore),
                new PropertyByName<Order>("OrderDiscount", p => p.OrderDiscount, ignore),
                new PropertyByName<Order>("CurrencyRate", p => p.CurrencyRate),
                new PropertyByName<Order>("CustomerCurrencyCode", p => p.CustomerCurrencyCode),
                new PropertyByName<Order>("AffiliateId", p => p.AffiliateId, ignore),
                new PropertyByName<Order>("PaymentMethodSystemName", p => p.PaymentMethodSystemName, ignore),
                new PropertyByName<Order>("ShippingPickupInStore", p => p.PickupInStore, ignore),
                new PropertyByName<Order>("ShippingMethod", p => p.ShippingMethod),
                new PropertyByName<Order>("ShippingRateComputationMethodSystemName", p => p.ShippingRateComputationMethodSystemName, ignore),
                new PropertyByName<Order>("CustomValuesXml", p => p.CustomValuesXml, ignore),
                new PropertyByName<Order>("VatNumber", p => p.VatNumber, ignore),
                new PropertyByName<Order>("CreatedOnUtc", p => p.CreatedOnUtc),
                new PropertyByName<Order>("BillingFirstName", async p => (await orderBillingAddress(p))?.FirstName ?? string.Empty),
                new PropertyByName<Order>("BillingLastName", async p => (await orderBillingAddress(p))?.LastName ?? string.Empty),
                new PropertyByName<Order>("BillingEmail", async p => (await orderBillingAddress(p))?.Email ?? string.Empty),
                new PropertyByName<Order>("BillingCompany", async p => (await orderBillingAddress(p))?.Company ?? string.Empty),
                new PropertyByName<Order>("BillingCountry", async p => (await _countryService.GetCountryByAddressAsync(await orderBillingAddress(p)))?.Name ?? string.Empty),
                new PropertyByName<Order>("BillingCountryCode", async p => (await _countryService.GetCountryByAddressAsync(await orderBillingAddress(p)))?.TwoLetterIsoCode, ignore),
                new PropertyByName<Order>("BillingStateProvince", async p => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderBillingAddress(p)))?.Name ?? string.Empty),
                new PropertyByName<Order>("BillingStateProvinceAbbreviation", async p => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderBillingAddress(p)))?.Abbreviation, ignore),
                new PropertyByName<Order>("BillingCounty", async p => (await orderBillingAddress(p))?.County ?? string.Empty),
                new PropertyByName<Order>("BillingCity", async p => (await orderBillingAddress(p))?.City ?? string.Empty),
                new PropertyByName<Order>("BillingAddress1", async p => (await orderBillingAddress(p))?.Address1 ?? string.Empty),
                new PropertyByName<Order>("BillingAddress2", async p => (await orderBillingAddress(p))?.Address2 ?? string.Empty),
                new PropertyByName<Order>("BillingZipPostalCode", async p => (await orderBillingAddress(p))?.ZipPostalCode ?? string.Empty),
                new PropertyByName<Order>("BillingPhoneNumber", async p => (await orderBillingAddress(p))?.PhoneNumber ?? string.Empty),
                new PropertyByName<Order>("BillingFaxNumber", async p => (await orderBillingAddress(p))?.FaxNumber ?? string.Empty),
                new PropertyByName<Order>("ShippingFirstName", async p => (await orderAddress(p))?.FirstName ?? string.Empty),
                new PropertyByName<Order>("ShippingLastName", async p => (await orderAddress(p))?.LastName ?? string.Empty),
                new PropertyByName<Order>("ShippingEmail", async p => (await orderAddress(p))?.Email ?? string.Empty),
                new PropertyByName<Order>("ShippingCompany", async p => (await orderAddress(p))?.Company ?? string.Empty),
                new PropertyByName<Order>("ShippingCountry", async p => (await _countryService.GetCountryByAddressAsync(await orderAddress(p)))?.Name ?? string.Empty),
                new PropertyByName<Order>("ShippingCountryCode", async p => (await _countryService.GetCountryByAddressAsync(await orderAddress(p)))?.TwoLetterIsoCode, ignore),
                new PropertyByName<Order>("ShippingStateProvince", async p => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderAddress(p)))?.Name ?? string.Empty),
                new PropertyByName<Order>("ShippingStateProvinceAbbreviation", async p => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderAddress(p)))?.Abbreviation, ignore),
                new PropertyByName<Order>("ShippingCounty", async p => (await orderAddress(p))?.County ?? string.Empty),
                new PropertyByName<Order>("ShippingCity", async p => (await orderAddress(p))?.City ?? string.Empty),
                new PropertyByName<Order>("ShippingAddress1", async p => (await orderAddress(p))?.Address1 ?? string.Empty),
                new PropertyByName<Order>("ShippingAddress2", async p => (await orderAddress(p))?.Address2 ?? string.Empty),
                new PropertyByName<Order>("ShippingZipPostalCode", async p => (await orderAddress(p))?.ZipPostalCode ?? string.Empty),
                new PropertyByName<Order>("ShippingPhoneNumber", async p => (await orderAddress(p))?.PhoneNumber ?? string.Empty),
                new PropertyByName<Order>("ShippingFaxNumber", async p => (await orderAddress(p))?.FaxNumber ?? string.Empty)
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
                new PropertyByName<Customer>("CustomerId", p => p.Id),
                new PropertyByName<Customer>("CustomerGuid", p => p.CustomerGuid),
                new PropertyByName<Customer>("Email", p => p.Email),
                new PropertyByName<Customer>("Username", p => p.Username),
                new PropertyByName<Customer>("IsTaxExempt", p => p.IsTaxExempt),
                new PropertyByName<Customer>("AffiliateId", p => p.AffiliateId),
                new PropertyByName<Customer>("Vendor", getVendor),
                new PropertyByName<Customer>("Active", p => p.Active),
                new PropertyByName<Customer>("CustomerRoles", async p=>string.Join(", ",
                    (await _customerService.GetCustomerRolesAsync(p)).Select(role => _catalogSettings.ExportImportRelatedEntitiesByName ? role.Name : role.Id.ToString()))), 
                new PropertyByName<Customer>("IsGuest", async p => await _customerService.IsGuestAsync(p)),
                new PropertyByName<Customer>("IsRegistered", async p => await _customerService.IsRegisteredAsync(p)),
                new PropertyByName<Customer>("IsAdministrator", async p => await _customerService.IsAdminAsync(p)),
                new PropertyByName<Customer>("IsForumModerator", async p => await _customerService.IsForumModeratorAsync(p)),
                new PropertyByName<Customer>("IsVendor", async p => await _customerService.IsVendorAsync(p)),
                new PropertyByName<Customer>("CreatedOnUtc", p => p.CreatedOnUtc),
                //attributes
                new PropertyByName<Customer>("FirstName", p => p.FirstName, !_customerSettings.FirstNameEnabled),
                new PropertyByName<Customer>("LastName", p => p.LastName, !_customerSettings.LastNameEnabled),
                new PropertyByName<Customer>("Gender", p => p.Gender, !_customerSettings.GenderEnabled),
                new PropertyByName<Customer>("Company", p => p.Company, !_customerSettings.CompanyEnabled),
                new PropertyByName<Customer>("StreetAddress", p => p.StreetAddress, !_customerSettings.StreetAddressEnabled),
                new PropertyByName<Customer>("StreetAddress2", p => p.StreetAddress2, !_customerSettings.StreetAddress2Enabled),
                new PropertyByName<Customer>("ZipPostalCode", p => p.ZipPostalCode, !_customerSettings.ZipPostalCodeEnabled),
                new PropertyByName<Customer>("City", p => p.City, !_customerSettings.CityEnabled),
                new PropertyByName<Customer>("County", p => p.County, !_customerSettings.CountyEnabled),
                new PropertyByName<Customer>("Country", getCountry, !_customerSettings.CountryEnabled),
                new PropertyByName<Customer>("StateProvince", getStateProvince, !_customerSettings.StateProvinceEnabled),
                new PropertyByName<Customer>("Phone", p => p.Phone, !_customerSettings.PhoneEnabled),
                new PropertyByName<Customer>("Fax", p => p.Fax, !_customerSettings.FaxEnabled),
                new PropertyByName<Customer>("VatNumber", p => p.VatNumber),
                new PropertyByName<Customer>("VatNumberStatusId", p => p.VatNumberStatusId),
                new PropertyByName<Customer>("VatNumberStatus", p => p.VatNumberStatusId)
                {
                    DropDownElements = await VatNumberStatus.Unknown.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Customer>("TimeZone", p => p.TimeZoneId, !_dateTimeSettings.AllowCustomersToSetTimeZone),
                new PropertyByName<Customer>("AvatarPictureId", async p => await _genericAttributeService.GetAttributeAsync<int>(p, NopCustomerDefaults.AvatarPictureIdAttribute), !_customerSettings.AllowCustomersToUploadAvatars),
                new PropertyByName<Customer>("ForumPostCount", async p => await _genericAttributeService.GetAttributeAsync<int>(p, NopCustomerDefaults.ForumPostCountAttribute)),
                new PropertyByName<Customer>("Signature", async p => await _genericAttributeService.GetAttributeAsync<string>(p, NopCustomerDefaults.SignatureAttribute)),
                new PropertyByName<Customer>("CustomCustomerAttributes",  GetCustomCustomerAttributesAsync)
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

                foreach (var store in await _storeService.GetAllStoresAsync())
                {
                    var newsletter = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
                    var subscribedToNewsletters = newsletter != null && newsletter.Active;
                    await xmlWriter.WriteElementStringAsync($"Newsletter-in-store-{store.Id}", null, subscribedToNewsletters.ToString());
                }

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
        public virtual async Task<string> ExportNewsletterSubscribersToTxtAsync(IList<NewsLetterSubscription> subscriptions)
        {
            if (subscriptions == null)
                throw new ArgumentNullException(nameof(subscriptions));

            const char separator = ',';
            var sb = new StringBuilder();

            sb.Append(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.Fields.Email"));
            sb.Append(separator);
            sb.Append(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.Fields.Active"));
            sb.Append(separator);
            sb.Append(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.Fields.Store"));
            sb.Append(Environment.NewLine);

            foreach (var subscription in subscriptions)
            {
                sb.Append(subscription.Email);
                sb.Append(separator);
                sb.Append(subscription.Active);
                sb.Append(separator);
                sb.Append(subscription.StoreId);
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
            if (states == null)
                throw new ArgumentNullException(nameof(states));

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
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //lambda expressions for choosing correct order address
            async Task<Address> orderAddress(Order o) => await _addressService.GetAddressByIdAsync((o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) ?? 0);
            async Task<Address> orderBillingAddress(Order o) => await _addressService.GetAddressByIdAsync(o.BillingAddressId);

            //customer info and customer attributes
            var customerManager = new PropertyManager<Customer>(new[]
            {
                new PropertyByName<Customer>("Email", p => p.Email),
                new PropertyByName<Customer>("Username", p => p.Username, !_customerSettings.UsernamesEnabled), 
                //attributes
                new PropertyByName<Customer>("First name", p => p.FirstName, !_customerSettings.FirstNameEnabled),
                new PropertyByName<Customer>("Last name", p => p.LastName, !_customerSettings.LastNameEnabled),
                new PropertyByName<Customer>("Gender", p => p.Gender, !_customerSettings.GenderEnabled),
                new PropertyByName<Customer>("Date of birth", p => p.DateOfBirth, !_customerSettings.DateOfBirthEnabled),
                new PropertyByName<Customer>("Company", p => p.Company, !_customerSettings.CompanyEnabled),
                new PropertyByName<Customer>("Street address", p => p.StreetAddress, !_customerSettings.StreetAddressEnabled),
                new PropertyByName<Customer>("Street address 2", p => p.StreetAddress2, !_customerSettings.StreetAddress2Enabled),
                new PropertyByName<Customer>("Zip / postal code", p => p.ZipPostalCode, !_customerSettings.ZipPostalCodeEnabled),
                new PropertyByName<Customer>("City", p => p.City, !_customerSettings.CityEnabled),
                new PropertyByName<Customer>("County", p => p.County, !_customerSettings.CountyEnabled),
                new PropertyByName<Customer>("Country", async p => (await _countryService.GetCountryByIdAsync(p.CountryId))?.Name ?? string.Empty, !_customerSettings.CountryEnabled),
                new PropertyByName<Customer>("State province", async p => (await _stateProvinceService.GetStateProvinceByIdAsync(p.StateProvinceId))?.Name ?? string.Empty, !(_customerSettings.StateProvinceEnabled && _customerSettings.CountryEnabled)),
                new PropertyByName<Customer>("Phone", p => p.Phone, !_customerSettings.PhoneEnabled),
                new PropertyByName<Customer>("Fax", p => p.Fax, !_customerSettings.FaxEnabled),
                new PropertyByName<Customer>("Customer attributes",  GetCustomCustomerAttributesAsync)
            }, _catalogSettings);

            //customer orders
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            var orderManager = new PropertyManager<Order>(new[]
            {
                new PropertyByName<Order>("Order Number", p => p.CustomOrderNumber),
                new PropertyByName<Order>("Order status", async p => await _localizationService.GetLocalizedEnumAsync(p.OrderStatus)),
                new PropertyByName<Order>("Order total", async p => await _priceFormatter.FormatPriceAsync(_currencyService.ConvertCurrency(p.OrderTotal, p.CurrencyRate), true, p.CustomerCurrencyCode, false, currentLanguage.Id)),
                new PropertyByName<Order>("Shipping method", p => p.ShippingMethod),
                new PropertyByName<Order>("Created on", async p => (await _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc)).ToString("D")),
                new PropertyByName<Order>("Billing first name", async p => (await orderBillingAddress(p))?.FirstName ?? string.Empty),
                new PropertyByName<Order>("Billing last name", async p => (await orderBillingAddress(p))?.LastName ?? string.Empty),
                new PropertyByName<Order>("Billing email", async p => (await orderBillingAddress(p))?.Email ?? string.Empty),
                new PropertyByName<Order>("Billing company", async p => (await orderBillingAddress(p))?.Company ?? string.Empty, !_addressSettings.CompanyEnabled),
                new PropertyByName<Order>("Billing country", async p => await _countryService.GetCountryByAddressAsync(await orderBillingAddress(p)) is Country country ? await _localizationService.GetLocalizedAsync(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
                new PropertyByName<Order>("Billing state province", async p => await _stateProvinceService.GetStateProvinceByAddressAsync(await orderBillingAddress(p)) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
                new PropertyByName<Order>("Billing county", async p => (await orderBillingAddress(p))?.County ?? string.Empty, !_addressSettings.CountyEnabled),
                new PropertyByName<Order>("Billing city", async p => (await orderBillingAddress(p))?.City ?? string.Empty, !_addressSettings.CityEnabled),
                new PropertyByName<Order>("Billing address 1", async p => (await orderBillingAddress(p))?.Address1 ?? string.Empty, !_addressSettings.StreetAddressEnabled),
                new PropertyByName<Order>("Billing address 2", async p => (await orderBillingAddress(p))?.Address2 ?? string.Empty, !_addressSettings.StreetAddress2Enabled),
                new PropertyByName<Order>("Billing zip postal code", async p => (await orderBillingAddress(p))?.ZipPostalCode ?? string.Empty, !_addressSettings.ZipPostalCodeEnabled),
                new PropertyByName<Order>("Billing phone number", async p => (await orderBillingAddress(p))?.PhoneNumber ?? string.Empty, !_addressSettings.PhoneEnabled),
                new PropertyByName<Order>("Billing fax number", async p => (await orderBillingAddress(p))?.FaxNumber ?? string.Empty, !_addressSettings.FaxEnabled),
                new PropertyByName<Order>("Shipping first name", async p => (await orderAddress(p))?.FirstName ?? string.Empty),
                new PropertyByName<Order>("Shipping last name", async p => (await orderAddress(p))?.LastName ?? string.Empty),
                new PropertyByName<Order>("Shipping email", async p => (await orderAddress(p))?.Email ?? string.Empty),
                new PropertyByName<Order>("Shipping company", async p => (await orderAddress(p))?.Company ?? string.Empty, !_addressSettings.CompanyEnabled),
                new PropertyByName<Order>("Shipping country", async p => await _countryService.GetCountryByAddressAsync(await orderAddress(p)) is Country country ? await _localizationService.GetLocalizedAsync(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
                new PropertyByName<Order>("Shipping state province", async p => await _stateProvinceService.GetStateProvinceByAddressAsync(await orderAddress(p)) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
                new PropertyByName<Order>("Shipping county", async p => (await orderAddress(p))?.County ?? string.Empty, !_addressSettings.CountyEnabled),
                new PropertyByName<Order>("Shipping city", async p => (await orderAddress(p))?.City ?? string.Empty, !_addressSettings.CityEnabled),
                new PropertyByName<Order>("Shipping address 1", async p => (await orderAddress(p))?.Address1 ?? string.Empty, !_addressSettings.StreetAddressEnabled),
                new PropertyByName<Order>("Shipping address 2", async p => (await orderAddress(p))?.Address2 ?? string.Empty, !_addressSettings.StreetAddress2Enabled),
                new PropertyByName<Order>("Shipping zip postal code",
                    async p => (await orderAddress(p))?.ZipPostalCode ?? string.Empty, !_addressSettings.ZipPostalCodeEnabled),
                new PropertyByName<Order>("Shipping phone number", async p => (await orderAddress(p))?.PhoneNumber ?? string.Empty, !_addressSettings.PhoneEnabled),
                new PropertyByName<Order>("Shipping fax number", async p => (await orderAddress(p))?.FaxNumber ?? string.Empty, !_addressSettings.FaxEnabled)
            }, _catalogSettings);

            var orderItemsManager = new PropertyManager<OrderItem>(new[]
            { 
                new PropertyByName<OrderItem>("SKU", async oi => await _productService.FormatSkuAsync(await _productService.GetProductByIdAsync(oi.ProductId), oi.AttributesXml)),
                new PropertyByName<OrderItem>("Name", async oi => await _localizationService.GetLocalizedAsync(await _productService.GetProductByIdAsync(oi.ProductId), p => p.Name)),
                new PropertyByName<OrderItem>("Price", async oi => await _priceFormatter.FormatPriceAsync(_currencyService.ConvertCurrency((await _orderService.GetOrderByIdAsync(oi.OrderId)).CustomerTaxDisplayType == TaxDisplayType.IncludingTax ? oi.UnitPriceInclTax : oi.UnitPriceExclTax, (await _orderService.GetOrderByIdAsync(oi.OrderId)).CurrencyRate), true, (await _orderService.GetOrderByIdAsync(oi.OrderId)).CustomerCurrencyCode, false, currentLanguage.Id)),
                new PropertyByName<OrderItem>("Quantity", oi => oi.Quantity),
                new PropertyByName<OrderItem>("Total", async oi => await _priceFormatter.FormatPriceAsync((await _orderService.GetOrderByIdAsync(oi.OrderId)).CustomerTaxDisplayType == TaxDisplayType.IncludingTax ? oi.PriceInclTax : oi.PriceExclTax))
            }, _catalogSettings);

            var orders = await _orderService.SearchOrdersAsync(customerId: customer.Id);

            //customer addresses
            var addressManager = new PropertyManager<Address>(new[]
            {
                new PropertyByName<Address>("First name", p => p.FirstName),
                new PropertyByName<Address>("Last name", p => p.LastName),
                new PropertyByName<Address>("Email", p => p.Email),
                new PropertyByName<Address>("Company", p => p.Company, !_addressSettings.CompanyEnabled),
                new PropertyByName<Address>("Country", async p => await _countryService.GetCountryByAddressAsync(p) is Country country ? await _localizationService.GetLocalizedAsync(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
                new PropertyByName<Address>("State province", async p => await _stateProvinceService.GetStateProvinceByAddressAsync(p) is StateProvince stateProvince ? await _localizationService.GetLocalizedAsync(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
                new PropertyByName<Address>("County", p => p.County, !_addressSettings.CountyEnabled),
                new PropertyByName<Address>("City", p => p.City, !_addressSettings.CityEnabled),
                new PropertyByName<Address>("Address 1", p => p.Address1, !_addressSettings.StreetAddressEnabled),
                new PropertyByName<Address>("Address 2", p => p.Address2, !_addressSettings.StreetAddress2Enabled),
                new PropertyByName<Address>("Zip / postal code", p => p.ZipPostalCode, !_addressSettings.ZipPostalCodeEnabled),
                new PropertyByName<Address>("Phone number", p => p.PhoneNumber, !_addressSettings.PhoneEnabled),
                new PropertyByName<Address>("Fax number", p => p.FaxNumber, !_addressSettings.FaxEnabled),
                new PropertyByName<Address>("Custom attributes", async p => await _customerAttributeFormatter.FormatAttributesAsync(p.CustomAttributes, ";"))
            }, _catalogSettings);

            //customer private messages
            var privateMessageManager = new PropertyManager<PrivateMessage>(new[]
            {
                new PropertyByName<PrivateMessage>("From", async pm => await _customerService.GetCustomerByIdAsync(pm.FromCustomerId) is Customer cFrom ? (_customerSettings.UsernamesEnabled ? cFrom.Username : cFrom.Email) : string.Empty),
                new PropertyByName<PrivateMessage>("To", async pm => await _customerService.GetCustomerByIdAsync(pm.ToCustomerId) is Customer cTo ? (_customerSettings.UsernamesEnabled ? cTo.Username : cTo.Email) : string.Empty),
                new PropertyByName<PrivateMessage>("Subject", pm => pm.Subject),
                new PropertyByName<PrivateMessage>("Text", pm => pm.Text),
                new PropertyByName<PrivateMessage>("Created on", async pm => (await _dateTimeHelper.ConvertToUserTimeAsync(pm.CreatedOnUtc, DateTimeKind.Utc)).ToString("D"))
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
                new PropertyByName<GdprLog>("Request type", async log => await _localizationService.GetLocalizedEnumAsync(log.RequestType)),
                new PropertyByName<GdprLog>("Request details", log => log.RequestDetails),
                new PropertyByName<GdprLog>("Created on", async log => (await _dateTimeHelper.ConvertToUserTimeAsync(log.CreatedOnUtc, DateTimeKind.Utc)).ToString("D"))
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
                customerManager.WriteCaption(customerInfoWorksheet);
                await customerManager.WriteToXlsxAsync(customerInfoWorksheet, customerInfoRow);

                //customer addresses
                if (await _customerService.GetAddressesByCustomerIdAsync(customer.Id) is IList<Address> addresses && addresses.Any())
                {
                    customerInfoRow += 2;

                    var cell = customerInfoWorksheet.Row(customerInfoRow).Cell(1);
                    cell.Value = "Address List";
                    customerInfoRow += 1;
                    addressManager.SetCaptionStyle(cell);
                    addressManager.WriteCaption(customerInfoWorksheet, customerInfoRow);

                    foreach (var customerAddress in addresses)
                    {
                        customerInfoRow += 1;
                        addressManager.CurrentObject = customerAddress;
                        await addressManager.WriteToXlsxAsync(customerInfoWorksheet, customerInfoRow);
                    }
                }

                //customer orders
                if (orders.Any())
                {
                    var ordersWorksheet = workbook.Worksheets.Add("Orders");

                    orderManager.WriteCaption(ordersWorksheet);

                    var orderRow = 1;

                    foreach (var order in orders)
                    {
                        orderRow += 1;
                        orderManager.CurrentObject = order;
                        await orderManager.WriteToXlsxAsync(ordersWorksheet, orderRow);

                        //products
                        var orederItems = await _orderService.GetOrderItemsAsync(order.Id);

                        if (!orederItems.Any())
                            continue;

                        orderRow += 1;

                        orderItemsManager.WriteCaption(ordersWorksheet, orderRow, 2);
                        ordersWorksheet.Row(orderRow).OutlineLevel = 1;
                        ordersWorksheet.Row(orderRow).Collapse();

                        foreach (var orederItem in orederItems)
                        {
                            orderRow++;
                            orderItemsManager.CurrentObject = orederItem;
                            await orderItemsManager.WriteToXlsxAsync(ordersWorksheet, orderRow, 2, fWorksheet);
                            ordersWorksheet.Row(orderRow).OutlineLevel = 1;
                            ordersWorksheet.Row(orderRow).Collapse();
                        }
                    }
                }

                //customer private messages
                if (pmList?.Any() ?? false)
                {
                    var privateMessageWorksheet = workbook.Worksheets.Add("Private messages");
                    privateMessageManager.WriteCaption(privateMessageWorksheet);

                    var privateMessageRow = 1;

                    foreach (var privateMessage in pmList)
                    {
                        privateMessageRow += 1;

                        privateMessageManager.CurrentObject = privateMessage;
                        await privateMessageManager.WriteToXlsxAsync(privateMessageWorksheet, privateMessageRow);
                    }
                }

                //customer GDPR logs
                if (gdprLog.Any())
                {
                    var gdprLogWorksheet = workbook.Worksheets.Add("GDPR requests (log)");
                    gdprLogManager.WriteCaption(gdprLogWorksheet);

                    var gdprLogRow = 1;

                    foreach (var log in gdprLog)
                    {
                        gdprLogRow += 1;

                        gdprLogManager.CurrentObject = log;
                        await gdprLogManager.WriteToXlsxAsync(gdprLogWorksheet, gdprLogRow);
                    }
                }

                workbook.SaveAs(stream);
            }

            return stream.ToArray();
        }

        #endregion
    }
}
