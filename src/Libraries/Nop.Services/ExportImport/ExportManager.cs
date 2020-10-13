using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
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
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using OfficeOpenXml;

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
        private readonly CustomerSettings _customerSettings;
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
            CustomerSettings customerSettings,
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
            _customerSettings = customerSettings;
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

        protected virtual async Task WriteCategoriesAsync(XmlWriter xmlWriter, int parentCategoryId)
        {
            var categories = await _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId, true);
            if (categories == null || !categories.Any())
                return;

            foreach (var category in categories)
            {
                await xmlWriter.WriteStartElementAsync("Category");

                await xmlWriter.WriteStringAsync("Id", category.Id);

                await xmlWriter.WriteStringAsync("Name", category.Name);
                await xmlWriter.WriteStringAsync("Description", category.Description);
                await xmlWriter.WriteStringAsync("CategoryTemplateId", category.CategoryTemplateId);
                await xmlWriter.WriteStringAsync("MetaKeywords", category.MetaKeywords, await IgnoreExportCategoryProperty());
                await xmlWriter.WriteStringAsync("MetaDescription", category.MetaDescription, await IgnoreExportCategoryProperty());
                await xmlWriter.WriteStringAsync("MetaTitle", category.MetaTitle, await IgnoreExportCategoryProperty());
                await xmlWriter.WriteStringAsync("SeName", await _urlRecordService.GetSeName(category, 0), await IgnoreExportCategoryProperty());
                await xmlWriter.WriteStringAsync("ParentCategoryId", category.ParentCategoryId);
                await xmlWriter.WriteStringAsync("PictureId", category.PictureId);
                await xmlWriter.WriteStringAsync("PageSize", category.PageSize, await IgnoreExportCategoryProperty());
                await xmlWriter.WriteStringAsync("AllowCustomersToSelectPageSize", category.AllowCustomersToSelectPageSize, await IgnoreExportCategoryProperty());
                await xmlWriter.WriteStringAsync("PageSizeOptions", category.PageSizeOptions, await IgnoreExportCategoryProperty());
                await xmlWriter.WriteStringAsync("PriceRanges", category.PriceRanges, await IgnoreExportCategoryProperty());
                await xmlWriter.WriteStringAsync("ShowOnHomepage", category.ShowOnHomepage, await IgnoreExportCategoryProperty());
                await xmlWriter.WriteStringAsync("IncludeInTopMenu", category.IncludeInTopMenu, await IgnoreExportCategoryProperty());
                await xmlWriter.WriteStringAsync("Published", category.Published, await IgnoreExportCategoryProperty());
                await xmlWriter.WriteStringAsync("Deleted", category.Deleted, true);
                await xmlWriter.WriteStringAsync("DisplayOrder", category.DisplayOrder);
                await xmlWriter.WriteStringAsync("CreatedOnUtc", category.CreatedOnUtc, await IgnoreExportCategoryProperty());
                await xmlWriter.WriteStringAsync("UpdatedOnUtc", category.UpdatedOnUtc, await IgnoreExportCategoryProperty());

                await xmlWriter.WriteStartElementAsync("Products");
                var productCategories = await _categoryService.GetProductCategoriesByCategoryId(category.Id, showHidden: true);
                foreach (var productCategory in productCategories)
                {
                    var product = await _productService.GetProductById(productCategory.ProductId);
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
                await WriteCategoriesAsync(xmlWriter, category.Id);
                await xmlWriter.WriteEndElementAsync();
                await xmlWriter.WriteEndElementAsync();
            }
        }

        /// <summary>
        /// Returns the path to the image file by ID
        /// </summary>
        /// <param name="pictureId">Picture ID</param>
        /// <returns>Path to the image file</returns>
        protected virtual async Task<string> GetPictures(int pictureId)
        {
            var picture = await _pictureService.GetPictureById(pictureId);

            return await _pictureService.GetThumbLocalPath(picture);
        }

        /// <summary>
        /// Returns the list of categories for a product separated by a ";"
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of categories</returns>
        protected virtual async Task<object> GetCategories(Product product)
        {
            string categoryNames = null;
            foreach (var pc in await _categoryService.GetProductCategoriesByProductId(product.Id, true))
            {
                if (_catalogSettings.ExportImportRelatedEntitiesByName)
                {
                    var category = await _categoryService.GetCategoryById(pc.CategoryId);
                    categoryNames += _catalogSettings.ExportImportProductCategoryBreadcrumb
                        ? await _categoryService.GetFormattedBreadCrumb(category)
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
        /// <returns>List of manufacturer</returns>
        protected virtual async Task<object> GetManufacturers(Product product)
        {
            string manufacturerNames = null;
            foreach (var pm in await _manufacturerService.GetProductManufacturersByProductId(product.Id, true))
            {
                if (_catalogSettings.ExportImportRelatedEntitiesByName)
                {
                    var manufacturer = await _manufacturerService.GetManufacturerById(pm.ManufacturerId);
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
        /// <returns>List of store</returns>
        protected virtual async Task<object> GetLimitedToStores(Product product)
        {
            string limitedToStores = null;
            foreach (var storeMapping in await _storeMappingService.GetStoreMappings(product))
            {
                var store = await _storeService.GetStoreById(storeMapping.StoreId);

                limitedToStores += _catalogSettings.ExportImportRelatedEntitiesByName ? store.Name : store.Id.ToString();

                limitedToStores += ";";
            }

            return limitedToStores;
        }

        /// <summary>
        /// Returns the list of product tag for a product separated by a ";"
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of product tag</returns>
        protected virtual async Task<object> GetProductTags(Product product)
        {
            string productTagNames = null;

            var productTags = await _productTagService.GetAllProductTagsByProductId(product.Id);

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
        /// <returns>image thumb local path</returns>
        protected virtual async Task<string> GetPicture(Product product, short pictureIndex)
        {
            // we need only the picture at a specific index, no need to get more pictures than that
            var recordsToReturn = pictureIndex + 1;
            var pictures = await _pictureService.GetPicturesByProductId(product.Id, recordsToReturn);
            
            return pictures.Count > pictureIndex ? await _pictureService.GetThumbLocalPath(pictures[pictureIndex]) : null;
        }

        protected virtual async Task<bool> IgnoreExportProductProperty(Func<ProductEditorSettings, bool> func)
        {
            var productAdvancedMode = true;
            try
            {
                productAdvancedMode = await _genericAttributeService.GetAttribute<bool>(await _workContext.GetCurrentCustomer(), "product-advanced-mode");
            }
            catch (ArgumentNullException)
            {
            }

            return !productAdvancedMode && !func(_productEditorSettings);
        }

        protected virtual async Task<bool> IgnoreExportCategoryProperty()
        {
            try
            {
                return !await _genericAttributeService.GetAttribute<bool>(await _workContext.GetCurrentCustomer(), "category-advanced-mode");
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        protected virtual async Task<bool> IgnoreExportManufacturerProperty()
        {
            try
            {
                return !await _genericAttributeService.GetAttribute<bool>(await _workContext.GetCurrentCustomer(), "manufacturer-advanced-mode");
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        protected virtual async Task<bool> IgnoreExportLimitedToStore()
        {
            return _catalogSettings.IgnoreStoreLimitations ||
                   !_catalogSettings.ExportImportProductUseLimitedToStores ||
                   (await _storeService.GetAllStores()).Count == 1;
        }

        private PropertyManager<ExportProductAttribute> GetProductAttributeManager()
        {
            var attributeProperties = new[]
            {
                new PropertyByName<ExportProductAttribute>("AttributeId", p => p.AttributeId),
                new PropertyByName<ExportProductAttribute>("AttributeName", p => p.AttributeName),
                new PropertyByName<ExportProductAttribute>("AttributeTextPrompt", p => p.AttributeTextPrompt),
                new PropertyByName<ExportProductAttribute>("AttributeIsRequired", p => p.AttributeIsRequired),
                new PropertyByName<ExportProductAttribute>("AttributeControlType", p => p.AttributeControlTypeId)
                {
                    DropDownElements = AttributeControlType.TextBox.ToSelectList(useLocalization: false)
                },
                new PropertyByName<ExportProductAttribute>("AttributeDisplayOrder", p => p.AttributeDisplayOrder),
                new PropertyByName<ExportProductAttribute>("ProductAttributeValueId", p => p.Id),
                new PropertyByName<ExportProductAttribute>("ValueName", p => p.Name),
                new PropertyByName<ExportProductAttribute>("AttributeValueType", p => p.AttributeValueTypeId)
                {
                    DropDownElements = AttributeValueType.Simple.ToSelectList(useLocalization: false)
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

        private async Task<PropertyManager<ExportSpecificationAttribute>> GetSpecificationAttributeManager()
        {
            var attributeProperties = new[]
            {
                new PropertyByName<ExportSpecificationAttribute>("AttributeType", p => p.AttributeTypeId)
                {
                    DropDownElements = SpecificationAttributeType.Option.ToSelectList(useLocalization: false)
                },
                new PropertyByName<ExportSpecificationAttribute>("SpecificationAttribute", p => p.SpecificationAttributeId)
                {
                    DropDownElements = (await _specificationAttributeService.GetSpecificationAttributes()).Select(sa => sa as BaseEntity).ToSelectList(p => (p as SpecificationAttribute)?.Name ?? string.Empty)
                },
                new PropertyByName<ExportSpecificationAttribute>("CustomValue", p => p.CustomValue),
                new PropertyByName<ExportSpecificationAttribute>("SpecificationAttributeOptionId", p => p.SpecificationAttributeOptionId),
                new PropertyByName<ExportSpecificationAttribute>("AllowFiltering", p => p.AllowFiltering),
                new PropertyByName<ExportSpecificationAttribute>("ShowOnProductPage", p => p.ShowOnProductPage),
                new PropertyByName<ExportSpecificationAttribute>("DisplayOrder", p => p.DisplayOrder)
            };

            return new PropertyManager<ExportSpecificationAttribute>(attributeProperties, _catalogSettings);
        }

        private async Task<byte[]> ExportProductsToXlsxWithAttributes(PropertyByName<Product>[] properties, IEnumerable<Product> itemsToExport)
        {
            var productAttributeManager = GetProductAttributeManager();
            var specificationAttributeManager = await GetSpecificationAttributeManager();

            await using var stream = new MemoryStream();
            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(stream))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handles to the worksheets
                var worksheet = xlPackage.Workbook.Worksheets.Add(typeof(Product).Name);
                var fpWorksheet = xlPackage.Workbook.Worksheets.Add("DataForProductsFilters");
                fpWorksheet.Hidden = eWorkSheetHidden.VeryHidden;
                var fbaWorksheet = xlPackage.Workbook.Worksheets.Add("DataForProductAttributesFilters");
                fbaWorksheet.Hidden = eWorkSheetHidden.VeryHidden;
                var fsaWorksheet = xlPackage.Workbook.Worksheets.Add("DataForSpecificationAttributesFilters");
                fsaWorksheet.Hidden = eWorkSheetHidden.VeryHidden;

                //create Headers and format them 
                var manager = new PropertyManager<Product>(properties, _catalogSettings);
                manager.WriteCaption(worksheet);

                var row = 2;
                foreach (var item in itemsToExport)
                {
                    manager.CurrentObject = item;
                    await manager.WriteToXlsx(worksheet, row++, fWorksheet: fpWorksheet);

                    if (_catalogSettings.ExportImportProductAttributes) 
                        row = await ExportProductAttributes(item, productAttributeManager, worksheet, row, fbaWorksheet);

                    if (_catalogSettings.ExportImportProductSpecificationAttributes) 
                        row = await ExportSpecificationAttributes(item, specificationAttributeManager, worksheet, row, fsaWorksheet);
                }

                xlPackage.Save();
            }

            return stream.ToArray();
        }

        private async Task<int> ExportProductAttributes(Product item, PropertyManager<ExportProductAttribute> attributeManager, ExcelWorksheet worksheet, int row, ExcelWorksheet faWorksheet)
        {
            var attributes = (await _productAttributeService.GetProductAttributeMappingsByProductId(item.Id))
                .SelectMany(pam =>
                {
                    var productAttribute = _productAttributeService.GetProductAttributeById(pam.ProductAttributeId).Result;

                    if (_productAttributeService.GetProductAttributeValues(pam.Id).Result is IList<ProductAttributeValue> values)
                    {
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
                    }

                    return new List<ExportProductAttribute>
                    {
                        new ExportProductAttribute
                        {
                            AttributeId = productAttribute.Id,
                            AttributeName = productAttribute.Name,
                            AttributeTextPrompt = pam.TextPrompt,
                            AttributeIsRequired = pam.IsRequired,
                            AttributeControlTypeId = pam.AttributeControlTypeId
                        }
                    };
                }).ToList();

            //attributes.AddRange(item.ProductAttributeMappings.Where(pam => !pam.ProductAttributeValues.Any()).Select(
            //    pam => new ExportProductAttribute
            //    {
            //        AttributeId = pam.ProductAttribute.Id,
            //        AttributeName = pam.ProductAttribute.Name,
            //        AttributeTextPrompt = pam.TextPrompt,
            //        AttributeIsRequired = pam.IsRequired,
            //        AttributeControlTypeId = pam.AttributeControlTypeId
            //    }));

            if (!attributes.Any())
                return row;

            attributeManager.WriteCaption(worksheet, row, ExportProductAttribute.ProducAttributeCellOffset);
            worksheet.Row(row).OutlineLevel = 1;
            worksheet.Row(row).Collapsed = true;

            foreach (var exportProductAttribute in attributes)
            {
                row++;
                attributeManager.CurrentObject = exportProductAttribute;
                await attributeManager.WriteToXlsx(worksheet, row, ExportProductAttribute.ProducAttributeCellOffset, faWorksheet);
                worksheet.Row(row).OutlineLevel = 1;
                worksheet.Row(row).Collapsed = true;
            }

            return row + 1;
        }

        private async Task<int> ExportSpecificationAttributes(Product item, PropertyManager<ExportSpecificationAttribute> attributeManager, ExcelWorksheet worksheet, int row, ExcelWorksheet faWorksheet)
        {
            var attributes = (await _specificationAttributeService
                .GetProductSpecificationAttributes(item.Id)).Select(
                psa => new ExportSpecificationAttribute
                {
                    AttributeTypeId = psa.AttributeTypeId,
                    CustomValue = psa.CustomValue,
                    AllowFiltering = psa.AllowFiltering,
                    ShowOnProductPage = psa.ShowOnProductPage,
                    DisplayOrder = psa.DisplayOrder,
                    SpecificationAttributeOptionId = psa.SpecificationAttributeOptionId,
                    SpecificationAttributeId = _specificationAttributeService.GetSpecificationAttributeOptionById(psa.SpecificationAttributeOptionId).Result.SpecificationAttributeId
                }).ToList();

            if (!attributes.Any())
                return row;

            attributeManager.WriteCaption(worksheet, row, ExportProductAttribute.ProducAttributeCellOffset);
            worksheet.Row(row).OutlineLevel = 1;
            worksheet.Row(row).Collapsed = true;

            foreach (var exportProductAttribute in attributes)
            {
                row++;
                attributeManager.CurrentObject = exportProductAttribute;
                await attributeManager.WriteToXlsx(worksheet, row, ExportProductAttribute.ProducAttributeCellOffset, faWorksheet);
                worksheet.Row(row).OutlineLevel = 1;
                worksheet.Row(row).Collapsed = true;
            }

            return row + 1;
        }

        private async Task<byte[]> ExportOrderToXlsxWithProducts(PropertyByName<Order>[] properties, IEnumerable<Order> itemsToExport)
        {
            var orderItemProperties = new[]
            {
                new PropertyByName<OrderItem>("Name", async oi => (await _productService.GetProductById(oi.ProductId)).Name),
                new PropertyByName<OrderItem>("Sku", async oi => (await _productService.GetProductById(oi.ProductId)).Sku),
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
            using (var xlPackage = new ExcelPackage(stream))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handles to the worksheets
                var worksheet = xlPackage.Workbook.Worksheets.Add(typeof(Order).Name);
                var fpWorksheet = xlPackage.Workbook.Worksheets.Add("DataForProductsFilters");
                fpWorksheet.Hidden = eWorkSheetHidden.VeryHidden;

                //create Headers and format them 
                var manager = new PropertyManager<Order>(properties, _catalogSettings);
                manager.WriteCaption(worksheet);

                var row = 2;
                foreach (var order in itemsToExport)
                {
                    manager.CurrentObject = order;
                    await manager.WriteToXlsx(worksheet, row++);

                    //a vendor should have access only to his products
                    var orderItems = await _orderService.GetOrderItems(order.Id, vendorId: (await _workContext.GetCurrentVendor())?.Id ?? 0);

                    if (!orderItems.Any())
                        continue;

                    orderItemsManager.WriteCaption(worksheet, row, 2);
                    worksheet.Row(row).OutlineLevel = 1;
                    worksheet.Row(row).Collapsed = true;

                    foreach (var orderItem in orderItems)
                    {
                        row++;
                        orderItemsManager.CurrentObject = orderItem;
                        await orderItemsManager.WriteToXlsx(worksheet, row, 2, fpWorksheet);
                        worksheet.Row(row).OutlineLevel = 1;
                        worksheet.Row(row).Collapsed = true;
                    }

                    row++;
                }

                xlPackage.Save();
            }

            return stream.ToArray();
        }

        private async Task<object> GetCustomCustomerAttributes(Customer customer)
        {
            var selectedCustomerAttributes = await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CustomCustomerAttributes);
            
            return await _customerAttributeFormatter.FormatAttributes(selectedCustomerAttributes, ";");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Export manufacturer list to XML
        /// </summary>
        /// <param name="manufacturers">Manufacturers</param>
        /// <returns>Result in XML format</returns>
        public virtual async Task<string> ExportManufacturersToXml(IList<Manufacturer> manufacturers)
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

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
                await xmlWriter.WriteStringAsync("MetaKeywords", manufacturer.MetaKeywords, await IgnoreExportManufacturerProperty());
                await xmlWriter.WriteStringAsync("MetaDescription", manufacturer.MetaDescription, await IgnoreExportManufacturerProperty());
                await xmlWriter.WriteStringAsync("MetaTitle", manufacturer.MetaTitle, await IgnoreExportManufacturerProperty());
                await xmlWriter.WriteStringAsync("SEName", await _urlRecordService.GetSeName(manufacturer, 0), await IgnoreExportManufacturerProperty());
                await xmlWriter.WriteStringAsync("PictureId", manufacturer.PictureId);
                await xmlWriter.WriteStringAsync("PageSize", manufacturer.PageSize, await IgnoreExportManufacturerProperty());
                await xmlWriter.WriteStringAsync("AllowCustomersToSelectPageSize", manufacturer.AllowCustomersToSelectPageSize, await IgnoreExportManufacturerProperty());
                await xmlWriter.WriteStringAsync("PageSizeOptions", manufacturer.PageSizeOptions, await IgnoreExportManufacturerProperty());
                await xmlWriter.WriteStringAsync("PriceRanges", manufacturer.PriceRanges, await IgnoreExportManufacturerProperty());
                await xmlWriter.WriteStringAsync("Published", manufacturer.Published, await IgnoreExportManufacturerProperty());
                await xmlWriter.WriteStringAsync("Deleted", manufacturer.Deleted, true);
                await xmlWriter.WriteStringAsync("DisplayOrder", manufacturer.DisplayOrder);
                await xmlWriter.WriteStringAsync("CreatedOnUtc", manufacturer.CreatedOnUtc, await IgnoreExportManufacturerProperty());
                await xmlWriter.WriteStringAsync("UpdatedOnUtc", manufacturer.UpdatedOnUtc, await IgnoreExportManufacturerProperty());

                await xmlWriter.WriteStartElementAsync("Products");
                var productManufacturers = await _manufacturerService.GetProductManufacturersByManufacturerId(manufacturer.Id, showHidden: true);
                if (productManufacturers != null)
                {
                    foreach (var productManufacturer in productManufacturers)
                    {
                        var product = await _productService.GetProductById(productManufacturer.ProductId);
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

            return stringWriter.ToString();
        }

        /// <summary>
        /// Export manufacturers to XLSX
        /// </summary>
        /// <param name="manufacturers">Manufactures</param>
        public virtual async Task<byte[]> ExportManufacturersToXlsx(IEnumerable<Manufacturer> manufacturers)
        {
            //property manager 
            var manager = new PropertyManager<Manufacturer>(new[]
            {
                new PropertyByName<Manufacturer>("Id", p => p.Id),
                new PropertyByName<Manufacturer>("Name", p => p.Name),
                new PropertyByName<Manufacturer>("Description", p => p.Description),
                new PropertyByName<Manufacturer>("ManufacturerTemplateId", p => p.ManufacturerTemplateId),
                new PropertyByName<Manufacturer>("MetaKeywords", p => p.MetaKeywords, await IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("MetaDescription", p => p.MetaDescription, await IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("MetaTitle", p => p.MetaTitle, await IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("SeName", async p => await _urlRecordService.GetSeName(p, 0), await IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("Picture", async p => await GetPictures(p.PictureId)),
                new PropertyByName<Manufacturer>("PageSize", p => p.PageSize, await IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("AllowCustomersToSelectPageSize", p => p.AllowCustomersToSelectPageSize, await IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("PageSizeOptions", p => p.PageSizeOptions, await IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("PriceRanges", p => p.PriceRanges, await IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("Published", p => p.Published, await IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("DisplayOrder", p => p.DisplayOrder)
            }, _catalogSettings);

            return await manager.ExportToXlsx(manufacturers);
        }

        /// <summary>
        /// Export category list to XML
        /// </summary>
        /// <returns>Result in XML format</returns>
        public virtual async Task<string> ExportCategoriesToXml()
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync("Categories");
            await xmlWriter.WriteAttributeStringAsync("Version", NopVersion.CURRENT_VERSION);
            await WriteCategoriesAsync(xmlWriter, 0);
            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();

            return stringWriter.ToString();
        }

        /// <summary>
        /// Export categories to XLSX
        /// </summary>
        /// <param name="categories">Categories</param>
        public virtual async Task<byte[]> ExportCategoriesToXlsx(IList<Category> categories)
        {
            var parentCategories = new List<Category>();
            if (_catalogSettings.ExportImportCategoriesUsingCategoryName)
                //performance optimization, load all parent categories in one SQL request
                parentCategories.AddRange(await _categoryService.GetCategoriesByIds(categories.Select(c => c.ParentCategoryId).Where(id => id != 0).ToArray()));

            //property manager 
            var manager = new PropertyManager<Category>(new[]
            {
                new PropertyByName<Category>("Id", p => p.Id),
                new PropertyByName<Category>("Name", p => p.Name),
                new PropertyByName<Category>("Description", p => p.Description),
                new PropertyByName<Category>("CategoryTemplateId", p => p.CategoryTemplateId),
                new PropertyByName<Category>("MetaKeywords", p => p.MetaKeywords, await IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("MetaDescription", p => p.MetaDescription, await IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("MetaTitle", p => p.MetaTitle, await IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("SeName", async p => await _urlRecordService.GetSeName(p, 0), await IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("ParentCategoryId", p => p.ParentCategoryId),
                new PropertyByName<Category>("ParentCategoryName", async p =>
                {
                    var category = parentCategories.FirstOrDefault(c => c.Id == p.ParentCategoryId);
                    return category != null ? await _categoryService.GetFormattedBreadCrumb(category) : null;

                }, !_catalogSettings.ExportImportCategoriesUsingCategoryName),
                new PropertyByName<Category>("Picture", async p => await GetPictures(p.PictureId)),
                new PropertyByName<Category>("PageSize", p => p.PageSize, await IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("AllowCustomersToSelectPageSize", p => p.AllowCustomersToSelectPageSize, await IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("PageSizeOptions", p => p.PageSizeOptions, await IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("PriceRanges", p => p.PriceRanges, await IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("ShowOnHomepage", p => p.ShowOnHomepage, await IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("IncludeInTopMenu", p => p.IncludeInTopMenu, await IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("Published", p => p.Published, await IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("DisplayOrder", p => p.DisplayOrder)
            }, _catalogSettings);

            return await manager.ExportToXlsx(categories);
        }

        /// <summary>
        /// Export product list to XML
        /// </summary>
        /// <param name="products">Products</param>
        /// <returns>Result in XML format</returns>
        public virtual async Task<string> ExportProductsToXml(IList<Product> products)
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync("Products");
            await xmlWriter.WriteAttributeStringAsync("Version", NopVersion.CURRENT_VERSION);

            foreach (var product in products)
            {
                await xmlWriter.WriteStartElementAsync("Product");

                await xmlWriter.WriteStringAsync("ProductId", product.Id);
                await xmlWriter.WriteStringAsync("ProductTypeId", product.ProductTypeId, await IgnoreExportProductProperty(p => p.ProductType));
                await xmlWriter.WriteStringAsync("ParentGroupedProductId", product.ParentGroupedProductId, await IgnoreExportProductProperty(p => p.ProductType));
                await xmlWriter.WriteStringAsync("VisibleIndividually", product.VisibleIndividually, await IgnoreExportProductProperty(p => p.VisibleIndividually));
                await xmlWriter.WriteStringAsync("Name", product.Name);
                await xmlWriter.WriteStringAsync("ShortDescription", product.ShortDescription);
                await xmlWriter.WriteStringAsync("FullDescription", product.FullDescription);
                await xmlWriter.WriteStringAsync("AdminComment", product.AdminComment, await IgnoreExportProductProperty(p => p.AdminComment));
                //vendor can't change this field
                await xmlWriter.WriteStringAsync("VendorId", product.VendorId, await IgnoreExportProductProperty(p => p.Vendor) || await _workContext.GetCurrentVendor() != null);
                await xmlWriter.WriteStringAsync("ProductTemplateId", product.ProductTemplateId, await IgnoreExportProductProperty(p => p.ProductTemplate));
                //vendor can't change this field
                await xmlWriter.WriteStringAsync("ShowOnHomepage", product.ShowOnHomepage, await IgnoreExportProductProperty(p => p.ShowOnHomepage) || await _workContext.GetCurrentVendor() != null);
                //vendor can't change this field
                await xmlWriter.WriteStringAsync("DisplayOrder", product.DisplayOrder, await IgnoreExportProductProperty(p => p.ShowOnHomepage) || await _workContext.GetCurrentVendor() != null);
                await xmlWriter.WriteStringAsync("MetaKeywords", product.MetaKeywords, await IgnoreExportProductProperty(p => p.Seo));
                await xmlWriter.WriteStringAsync("MetaDescription", product.MetaDescription, await IgnoreExportProductProperty(p => p.Seo));
                await xmlWriter.WriteStringAsync("MetaTitle", product.MetaTitle, await IgnoreExportProductProperty(p => p.Seo));
                await xmlWriter.WriteStringAsync("SEName", await _urlRecordService.GetSeName(product, 0), await IgnoreExportProductProperty(p => p.Seo));
                await xmlWriter.WriteStringAsync("AllowCustomerReviews", product.AllowCustomerReviews, await IgnoreExportProductProperty(p => p.AllowCustomerReviews));
                await xmlWriter.WriteStringAsync("SKU", product.Sku);
                await xmlWriter.WriteStringAsync("ManufacturerPartNumber", product.ManufacturerPartNumber, await IgnoreExportProductProperty(p => p.ManufacturerPartNumber));
                await xmlWriter.WriteStringAsync("Gtin", product.Gtin, await IgnoreExportProductProperty(p => p.GTIN));
                await xmlWriter.WriteStringAsync("IsGiftCard", product.IsGiftCard, await IgnoreExportProductProperty(p => p.IsGiftCard));
                await xmlWriter.WriteStringAsync("GiftCardType", product.GiftCardType, await IgnoreExportProductProperty(p => p.IsGiftCard));
                await xmlWriter.WriteStringAsync("OverriddenGiftCardAmount", product.OverriddenGiftCardAmount, await IgnoreExportProductProperty(p => p.IsGiftCard));
                await xmlWriter.WriteStringAsync("RequireOtherProducts", product.RequireOtherProducts, await IgnoreExportProductProperty(p => p.RequireOtherProductsAddedToCart));
                await xmlWriter.WriteStringAsync("RequiredProductIds", product.RequiredProductIds, await IgnoreExportProductProperty(p => p.RequireOtherProductsAddedToCart));
                await xmlWriter.WriteStringAsync("AutomaticallyAddRequiredProducts", product.AutomaticallyAddRequiredProducts, await IgnoreExportProductProperty(p => p.RequireOtherProductsAddedToCart));
                await xmlWriter.WriteStringAsync("IsDownload", product.IsDownload, await IgnoreExportProductProperty(p => p.DownloadableProduct));
                await xmlWriter.WriteStringAsync("DownloadId", product.DownloadId, await IgnoreExportProductProperty(p => p.DownloadableProduct));
                await xmlWriter.WriteStringAsync("UnlimitedDownloads", product.UnlimitedDownloads, await IgnoreExportProductProperty(p => p.DownloadableProduct));
                await xmlWriter.WriteStringAsync("MaxNumberOfDownloads", product.MaxNumberOfDownloads, await IgnoreExportProductProperty(p => p.DownloadableProduct));
                await xmlWriter.WriteStringAsync("DownloadExpirationDays", product.DownloadExpirationDays, await IgnoreExportProductProperty(p => p.DownloadableProduct));
                await xmlWriter.WriteStringAsync("DownloadActivationType", product.DownloadActivationType, await IgnoreExportProductProperty(p => p.DownloadableProduct));
                await xmlWriter.WriteStringAsync("HasSampleDownload", product.HasSampleDownload, await IgnoreExportProductProperty(p => p.DownloadableProduct));
                await xmlWriter.WriteStringAsync("SampleDownloadId", product.SampleDownloadId, await IgnoreExportProductProperty(p => p.DownloadableProduct));
                await xmlWriter.WriteStringAsync("HasUserAgreement", product.HasUserAgreement, await IgnoreExportProductProperty(p => p.DownloadableProduct));
                await xmlWriter.WriteStringAsync("UserAgreementText", product.UserAgreementText, await IgnoreExportProductProperty(p => p.DownloadableProduct));
                await xmlWriter.WriteStringAsync("IsRecurring", product.IsRecurring, await IgnoreExportProductProperty(p => p.RecurringProduct));
                await xmlWriter.WriteStringAsync("RecurringCycleLength", product.RecurringCycleLength, await IgnoreExportProductProperty(p => p.RecurringProduct));
                await xmlWriter.WriteStringAsync("RecurringCyclePeriodId", product.RecurringCyclePeriodId, await IgnoreExportProductProperty(p => p.RecurringProduct));
                await xmlWriter.WriteStringAsync("RecurringTotalCycles", product.RecurringTotalCycles, await IgnoreExportProductProperty(p => p.RecurringProduct));
                await xmlWriter.WriteStringAsync("IsRental", product.IsRental, await IgnoreExportProductProperty(p => p.IsRental));
                await xmlWriter.WriteStringAsync("RentalPriceLength", product.RentalPriceLength, await IgnoreExportProductProperty(p => p.IsRental));
                await xmlWriter.WriteStringAsync("RentalPricePeriodId", product.RentalPricePeriodId, await IgnoreExportProductProperty(p => p.IsRental));
                await xmlWriter.WriteStringAsync("IsShipEnabled", product.IsShipEnabled);
                await xmlWriter.WriteStringAsync("IsFreeShipping", product.IsFreeShipping, await IgnoreExportProductProperty(p => p.FreeShipping));
                await xmlWriter.WriteStringAsync("ShipSeparately", product.ShipSeparately, await IgnoreExportProductProperty(p => p.ShipSeparately));
                await xmlWriter.WriteStringAsync("AdditionalShippingCharge", product.AdditionalShippingCharge, await IgnoreExportProductProperty(p => p.AdditionalShippingCharge));
                await xmlWriter.WriteStringAsync("DeliveryDateId", product.DeliveryDateId, await IgnoreExportProductProperty(p => p.DeliveryDate));
                await xmlWriter.WriteStringAsync("IsTaxExempt", product.IsTaxExempt);
                await xmlWriter.WriteStringAsync("TaxCategoryId", product.TaxCategoryId);
                await xmlWriter.WriteStringAsync("IsTelecommunicationsOrBroadcastingOrElectronicServices", product.IsTelecommunicationsOrBroadcastingOrElectronicServices, await IgnoreExportProductProperty(p => p.TelecommunicationsBroadcastingElectronicServices));
                await xmlWriter.WriteStringAsync("ManageInventoryMethodId", product.ManageInventoryMethodId);
                await xmlWriter.WriteStringAsync("ProductAvailabilityRangeId", product.ProductAvailabilityRangeId, await IgnoreExportProductProperty(p => p.ProductAvailabilityRange));
                await xmlWriter.WriteStringAsync("UseMultipleWarehouses", product.UseMultipleWarehouses, await IgnoreExportProductProperty(p => p.UseMultipleWarehouses));
                await xmlWriter.WriteStringAsync("WarehouseId", product.WarehouseId, await IgnoreExportProductProperty(p => p.Warehouse));
                await xmlWriter.WriteStringAsync("StockQuantity", product.StockQuantity);
                await xmlWriter.WriteStringAsync("DisplayStockAvailability", product.DisplayStockAvailability, await IgnoreExportProductProperty(p => p.DisplayStockAvailability));
                await xmlWriter.WriteStringAsync("DisplayStockQuantity", product.DisplayStockQuantity, await IgnoreExportProductProperty(p => p.DisplayStockAvailability));
                await xmlWriter.WriteStringAsync("MinStockQuantity", product.MinStockQuantity, await IgnoreExportProductProperty(p => p.MinimumStockQuantity));
                await xmlWriter.WriteStringAsync("LowStockActivityId", product.LowStockActivityId, await IgnoreExportProductProperty(p => p.LowStockActivity));
                await xmlWriter.WriteStringAsync("NotifyAdminForQuantityBelow", product.NotifyAdminForQuantityBelow, await IgnoreExportProductProperty(p => p.NotifyAdminForQuantityBelow));
                await xmlWriter.WriteStringAsync("BackorderModeId", product.BackorderModeId, await IgnoreExportProductProperty(p => p.Backorders));
                await xmlWriter.WriteStringAsync("AllowBackInStockSubscriptions", product.AllowBackInStockSubscriptions, await IgnoreExportProductProperty(p => p.AllowBackInStockSubscriptions));
                await xmlWriter.WriteStringAsync("OrderMinimumQuantity", product.OrderMinimumQuantity, await IgnoreExportProductProperty(p => p.MinimumCartQuantity));
                await xmlWriter.WriteStringAsync("OrderMaximumQuantity", product.OrderMaximumQuantity, await IgnoreExportProductProperty(p => p.MaximumCartQuantity));
                await xmlWriter.WriteStringAsync("AllowedQuantities", product.AllowedQuantities, await IgnoreExportProductProperty(p => p.AllowedQuantities));
                await xmlWriter.WriteStringAsync("AllowAddingOnlyExistingAttributeCombinations", product.AllowAddingOnlyExistingAttributeCombinations, await IgnoreExportProductProperty(p => p.AllowAddingOnlyExistingAttributeCombinations));
                await xmlWriter.WriteStringAsync("NotReturnable", product.NotReturnable, await IgnoreExportProductProperty(p => p.NotReturnable));
                await xmlWriter.WriteStringAsync("DisableBuyButton", product.DisableBuyButton, await IgnoreExportProductProperty(p => p.DisableBuyButton));
                await xmlWriter.WriteStringAsync("DisableWishlistButton", product.DisableWishlistButton, await IgnoreExportProductProperty(p => p.DisableWishlistButton));
                await xmlWriter.WriteStringAsync("AvailableForPreOrder", product.AvailableForPreOrder, await IgnoreExportProductProperty(p => p.AvailableForPreOrder));
                await xmlWriter.WriteStringAsync("PreOrderAvailabilityStartDateTimeUtc", product.PreOrderAvailabilityStartDateTimeUtc, await IgnoreExportProductProperty(p => p.AvailableForPreOrder));
                await xmlWriter.WriteStringAsync("CallForPrice", product.CallForPrice, await IgnoreExportProductProperty(p => p.CallForPrice));
                await xmlWriter.WriteStringAsync("Price", product.Price);
                await xmlWriter.WriteStringAsync("OldPrice", product.OldPrice, await IgnoreExportProductProperty(p => p.OldPrice));
                await xmlWriter.WriteStringAsync("ProductCost", product.ProductCost, await IgnoreExportProductProperty(p => p.ProductCost));
                await xmlWriter.WriteStringAsync("CustomerEntersPrice", product.CustomerEntersPrice, await IgnoreExportProductProperty(p => p.CustomerEntersPrice));
                await xmlWriter.WriteStringAsync("MinimumCustomerEnteredPrice", product.MinimumCustomerEnteredPrice, await IgnoreExportProductProperty(p => p.CustomerEntersPrice));
                await xmlWriter.WriteStringAsync("MaximumCustomerEnteredPrice", product.MaximumCustomerEnteredPrice, await IgnoreExportProductProperty(p => p.CustomerEntersPrice));
                await xmlWriter.WriteStringAsync("BasepriceEnabled", product.BasepriceEnabled, await IgnoreExportProductProperty(p => p.PAngV));
                await xmlWriter.WriteStringAsync("BasepriceAmount", product.BasepriceAmount, await IgnoreExportProductProperty(p => p.PAngV));
                await xmlWriter.WriteStringAsync("BasepriceUnitId", product.BasepriceUnitId, await IgnoreExportProductProperty(p => p.PAngV));
                await xmlWriter.WriteStringAsync("BasepriceBaseAmount", product.BasepriceBaseAmount, await IgnoreExportProductProperty(p => p.PAngV));
                await xmlWriter.WriteStringAsync("BasepriceBaseUnitId", product.BasepriceBaseUnitId, await IgnoreExportProductProperty(p => p.PAngV));
                await xmlWriter.WriteStringAsync("MarkAsNew", product.MarkAsNew, await IgnoreExportProductProperty(p => p.MarkAsNew));
                await xmlWriter.WriteStringAsync("MarkAsNewStartDateTimeUtc", product.MarkAsNewStartDateTimeUtc, await IgnoreExportProductProperty(p => p.MarkAsNew));
                await xmlWriter.WriteStringAsync("MarkAsNewEndDateTimeUtc", product.MarkAsNewEndDateTimeUtc, await IgnoreExportProductProperty(p => p.MarkAsNew));
                await xmlWriter.WriteStringAsync("Weight", product.Weight, await IgnoreExportProductProperty(p => p.Weight));
                await xmlWriter.WriteStringAsync("Length", product.Length, await IgnoreExportProductProperty(p => p.Dimensions));
                await xmlWriter.WriteStringAsync("Width", product.Width, await IgnoreExportProductProperty(p => p.Dimensions));
                await xmlWriter.WriteStringAsync("Height", product.Height, await IgnoreExportProductProperty(p => p.Dimensions));
                await xmlWriter.WriteStringAsync("Published", product.Published, await IgnoreExportProductProperty(p => p.Published));
                await xmlWriter.WriteStringAsync("CreatedOnUtc", product.CreatedOnUtc);
                await xmlWriter.WriteStringAsync("UpdatedOnUtc", product.UpdatedOnUtc);

                if (!await IgnoreExportProductProperty(p => p.Discounts))
                {
                    await xmlWriter.WriteStartElementAsync("ProductDiscounts");

                    foreach (var discount in await _discountService.GetAppliedDiscounts(product))
                    {
                        await xmlWriter.WriteStartElementAsync("Discount");
                        await xmlWriter.WriteStringAsync("DiscountId", discount.Id);
                        await xmlWriter.WriteStringAsync("Name", discount.Name);
                        await xmlWriter.WriteEndElementAsync();
                    }

                    await xmlWriter.WriteEndElementAsync();
                }

                if (!await IgnoreExportProductProperty(p => p.TierPrices))
                {
                    await  xmlWriter.WriteStartElementAsync("TierPrices");
                    var tierPrices = await _productService.GetTierPricesByProduct(product.Id);
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

                if (!await IgnoreExportProductProperty(p => p.ProductAttributes))
                {
                    await  xmlWriter.WriteStartElementAsync("ProductAttributes");
                    var productAttributMappings =
                        await _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                    foreach (var productAttributeMapping in productAttributMappings)
                    {
                        var productAttribute = await _productAttributeService.GetProductAttributeById(productAttributeMapping.ProductAttributeId);

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
                        var productAttributeValues = await _productAttributeService.GetProductAttributeValues(productAttributeMapping.Id);
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
                var productPictures = await _productService.GetProductPicturesByProductId(product.Id);
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
                var productCategories = await _categoryService.GetProductCategoriesByProductId(product.Id);
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

                if (!await IgnoreExportProductProperty(p => p.Manufacturers))
                {
                    await xmlWriter.WriteStartElementAsync("ProductManufacturers");
                    var productManufacturers = await _manufacturerService.GetProductManufacturersByProductId(product.Id);
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

                if (!await IgnoreExportProductProperty(p => p.SpecificationAttributes))
                {
                    await xmlWriter.WriteStartElementAsync("ProductSpecificationAttributes");
                    var productSpecificationAttributes = await _specificationAttributeService.GetProductSpecificationAttributes(product.Id);
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

                if (!await IgnoreExportProductProperty(p => p.ProductTags))
                {
                    await xmlWriter.WriteStartElementAsync("ProductTags");
                    var productTags = await _productTagService.GetAllProductTagsByProductId(product.Id);
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

            return stringWriter.ToString();
        }

        /// <summary>
        /// Export products to XLSX
        /// </summary>
        /// <param name="products">Products</param>
        public virtual async Task<byte[]> ExportProductsToXlsx(IEnumerable<Product> products)
        {
            var properties = new[]
            {
                new PropertyByName<Product>("ProductId", p => p.Id),
                new PropertyByName<Product>("ProductType", p => p.ProductTypeId, await IgnoreExportProductProperty(p => p.ProductType))
                {
                    DropDownElements = ProductType.SimpleProduct.ToSelectList(useLocalization: false)
                },
                new PropertyByName<Product>("ParentGroupedProductId", p => p.ParentGroupedProductId, await IgnoreExportProductProperty(p => p.ProductType)),
                new PropertyByName<Product>("VisibleIndividually", p => p.VisibleIndividually, await IgnoreExportProductProperty(p => p.VisibleIndividually)),
                new PropertyByName<Product>("Name", p => p.Name),
                new PropertyByName<Product>("ShortDescription", p => p.ShortDescription),
                new PropertyByName<Product>("FullDescription", p => p.FullDescription),
                //vendor can't change this field
                new PropertyByName<Product>("Vendor", p => p.VendorId, await IgnoreExportProductProperty(p => p.Vendor) || await _workContext.GetCurrentVendor() != null)
                {
                    DropDownElements = (await _vendorService.GetAllVendors(showHidden: true)).Select(v => v as BaseEntity).ToSelectList(p => (p as Vendor)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("ProductTemplate", p => p.ProductTemplateId, await IgnoreExportProductProperty(p => p.ProductTemplate))
                {
                    DropDownElements = (await _productTemplateService.GetAllProductTemplates()).Select(pt => pt as BaseEntity).ToSelectList(p => (p as ProductTemplate)?.Name ?? string.Empty)
                },
                //vendor can't change this field
                new PropertyByName<Product>("ShowOnHomepage", p => p.ShowOnHomepage, await IgnoreExportProductProperty(p => p.ShowOnHomepage) || await _workContext.GetCurrentVendor() != null),
                //vendor can't change this field
                new PropertyByName<Product>("DisplayOrder", p => p.DisplayOrder, await IgnoreExportProductProperty(p => p.ShowOnHomepage) || await _workContext.GetCurrentVendor() != null),
                new PropertyByName<Product>("MetaKeywords", p => p.MetaKeywords, await IgnoreExportProductProperty(p => p.Seo)),
                new PropertyByName<Product>("MetaDescription", p => p.MetaDescription, await IgnoreExportProductProperty(p => p.Seo)),
                new PropertyByName<Product>("MetaTitle", p => p.MetaTitle, await IgnoreExportProductProperty(p => p.Seo)),
                new PropertyByName<Product>("SeName", async p => await _urlRecordService.GetSeName(p, 0), await IgnoreExportProductProperty(p => p.Seo)),
                new PropertyByName<Product>("AllowCustomerReviews", p => p.AllowCustomerReviews, await IgnoreExportProductProperty(p => p.AllowCustomerReviews)),
                new PropertyByName<Product>("Published", p => p.Published, await IgnoreExportProductProperty(p => p.Published)),
                new PropertyByName<Product>("SKU", p => p.Sku),
                new PropertyByName<Product>("ManufacturerPartNumber", p => p.ManufacturerPartNumber, await IgnoreExportProductProperty(p => p.ManufacturerPartNumber)),
                new PropertyByName<Product>("Gtin", p => p.Gtin, await IgnoreExportProductProperty(p => p.GTIN)),
                new PropertyByName<Product>("IsGiftCard", p => p.IsGiftCard, await IgnoreExportProductProperty(p => p.IsGiftCard)),
                new PropertyByName<Product>("GiftCardType", p => p.GiftCardTypeId, await IgnoreExportProductProperty(p => p.IsGiftCard))
                {
                    DropDownElements = GiftCardType.Virtual.ToSelectList(useLocalization: false)
                },
                new PropertyByName<Product>("OverriddenGiftCardAmount", p => p.OverriddenGiftCardAmount, await IgnoreExportProductProperty(p => p.IsGiftCard)),
                new PropertyByName<Product>("RequireOtherProducts", p => p.RequireOtherProducts, await IgnoreExportProductProperty(p => p.RequireOtherProductsAddedToCart)),
                new PropertyByName<Product>("RequiredProductIds", p => p.RequiredProductIds, await IgnoreExportProductProperty(p => p.RequireOtherProductsAddedToCart)),
                new PropertyByName<Product>("AutomaticallyAddRequiredProducts", p => p.AutomaticallyAddRequiredProducts, await IgnoreExportProductProperty(p => p.RequireOtherProductsAddedToCart)),
                new PropertyByName<Product>("IsDownload", p => p.IsDownload, await IgnoreExportProductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("DownloadId", p => p.DownloadId, await IgnoreExportProductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("UnlimitedDownloads", p => p.UnlimitedDownloads, await IgnoreExportProductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("MaxNumberOfDownloads", p => p.MaxNumberOfDownloads, await IgnoreExportProductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("DownloadActivationType", p => p.DownloadActivationTypeId, await IgnoreExportProductProperty(p => p.DownloadableProduct))
                {
                    DropDownElements = DownloadActivationType.Manually.ToSelectList(useLocalization: false)
                },
                new PropertyByName<Product>("HasSampleDownload", p => p.HasSampleDownload, await IgnoreExportProductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("SampleDownloadId", p => p.SampleDownloadId, await IgnoreExportProductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("HasUserAgreement", p => p.HasUserAgreement, await IgnoreExportProductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("UserAgreementText", p => p.UserAgreementText, await IgnoreExportProductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("IsRecurring", p => p.IsRecurring, await IgnoreExportProductProperty(p => p.RecurringProduct)),
                new PropertyByName<Product>("RecurringCycleLength", p => p.RecurringCycleLength, await IgnoreExportProductProperty(p => p.RecurringProduct)),
                new PropertyByName<Product>("RecurringCyclePeriod", p => p.RecurringCyclePeriodId, await IgnoreExportProductProperty(p => p.RecurringProduct))
                {
                    DropDownElements = RecurringProductCyclePeriod.Days.ToSelectList(useLocalization: false),
                    AllowBlank = true
                },
                new PropertyByName<Product>("RecurringTotalCycles", p => p.RecurringTotalCycles, await IgnoreExportProductProperty(p => p.RecurringProduct)),
                new PropertyByName<Product>("IsRental", p => p.IsRental, await IgnoreExportProductProperty(p => p.IsRental)),
                new PropertyByName<Product>("RentalPriceLength", p => p.RentalPriceLength, await IgnoreExportProductProperty(p => p.IsRental)),
                new PropertyByName<Product>("RentalPricePeriod", p => p.RentalPricePeriodId, await IgnoreExportProductProperty(p => p.IsRental))
                {
                    DropDownElements = RentalPricePeriod.Days.ToSelectList(useLocalization: false),
                    AllowBlank = true
                },
                new PropertyByName<Product>("IsShipEnabled", p => p.IsShipEnabled),
                new PropertyByName<Product>("IsFreeShipping", p => p.IsFreeShipping, await IgnoreExportProductProperty(p => p.FreeShipping)),
                new PropertyByName<Product>("ShipSeparately", p => p.ShipSeparately, await IgnoreExportProductProperty(p => p.ShipSeparately)),
                new PropertyByName<Product>("AdditionalShippingCharge", p => p.AdditionalShippingCharge, await IgnoreExportProductProperty(p => p.AdditionalShippingCharge)),
                new PropertyByName<Product>("DeliveryDate", p => p.DeliveryDateId, await IgnoreExportProductProperty(p => p.DeliveryDate))
                {
                    DropDownElements = (await _dateRangeService.GetAllDeliveryDates()).Select(dd => dd as BaseEntity).ToSelectList(p => (p as DeliveryDate)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("IsTaxExempt", p => p.IsTaxExempt),
                new PropertyByName<Product>("TaxCategory", p => p.TaxCategoryId)
                {
                    DropDownElements = (await _taxCategoryService.GetAllTaxCategories()).Select(tc => tc as BaseEntity).ToSelectList(p => (p as TaxCategory)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("IsTelecommunicationsOrBroadcastingOrElectronicServices", p => p.IsTelecommunicationsOrBroadcastingOrElectronicServices, await IgnoreExportProductProperty(p => p.TelecommunicationsBroadcastingElectronicServices)),
                new PropertyByName<Product>("ManageInventoryMethod", p => p.ManageInventoryMethodId)
                {
                    DropDownElements = ManageInventoryMethod.DontManageStock.ToSelectList(useLocalization: false)
                },
                new PropertyByName<Product>("ProductAvailabilityRange", p => p.ProductAvailabilityRangeId, await IgnoreExportProductProperty(p => p.ProductAvailabilityRange))
                {
                    DropDownElements = (await _dateRangeService.GetAllProductAvailabilityRanges()).Select(range => range as BaseEntity).ToSelectList(p => (p as ProductAvailabilityRange)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("UseMultipleWarehouses", p => p.UseMultipleWarehouses, await IgnoreExportProductProperty(p => p.UseMultipleWarehouses)),
                new PropertyByName<Product>("WarehouseId", p => p.WarehouseId, await IgnoreExportProductProperty(p => p.Warehouse)),
                new PropertyByName<Product>("StockQuantity", p => p.StockQuantity),
                new PropertyByName<Product>("DisplayStockAvailability", p => p.DisplayStockAvailability, await IgnoreExportProductProperty(p => p.DisplayStockAvailability)),
                new PropertyByName<Product>("DisplayStockQuantity", p => p.DisplayStockQuantity, await IgnoreExportProductProperty(p => p.DisplayStockAvailability)),
                new PropertyByName<Product>("MinStockQuantity", p => p.MinStockQuantity, await IgnoreExportProductProperty(p => p.MinimumStockQuantity)),
                new PropertyByName<Product>("LowStockActivity", p => p.LowStockActivityId, await IgnoreExportProductProperty(p => p.LowStockActivity))
                {
                    DropDownElements = LowStockActivity.Nothing.ToSelectList(useLocalization: false)
                },
                new PropertyByName<Product>("NotifyAdminForQuantityBelow", p => p.NotifyAdminForQuantityBelow, await IgnoreExportProductProperty(p => p.NotifyAdminForQuantityBelow)),
                new PropertyByName<Product>("BackorderMode", p => p.BackorderModeId, await IgnoreExportProductProperty(p => p.Backorders))
                {
                    DropDownElements = BackorderMode.NoBackorders.ToSelectList(useLocalization: false)
                },
                new PropertyByName<Product>("AllowBackInStockSubscriptions", p => p.AllowBackInStockSubscriptions, await IgnoreExportProductProperty(p => p.AllowBackInStockSubscriptions)),
                new PropertyByName<Product>("OrderMinimumQuantity", p => p.OrderMinimumQuantity, await IgnoreExportProductProperty(p => p.MinimumCartQuantity)),
                new PropertyByName<Product>("OrderMaximumQuantity", p => p.OrderMaximumQuantity, await IgnoreExportProductProperty(p => p.MaximumCartQuantity)),
                new PropertyByName<Product>("AllowedQuantities", p => p.AllowedQuantities, await IgnoreExportProductProperty(p => p.AllowedQuantities)),
                new PropertyByName<Product>("AllowAddingOnlyExistingAttributeCombinations", p => p.AllowAddingOnlyExistingAttributeCombinations, await IgnoreExportProductProperty(p => p.AllowAddingOnlyExistingAttributeCombinations)),
                new PropertyByName<Product>("NotReturnable", p => p.NotReturnable, await IgnoreExportProductProperty(p => p.NotReturnable)),
                new PropertyByName<Product>("DisableBuyButton", p => p.DisableBuyButton, await IgnoreExportProductProperty(p => p.DisableBuyButton)),
                new PropertyByName<Product>("DisableWishlistButton", p => p.DisableWishlistButton, await IgnoreExportProductProperty(p => p.DisableWishlistButton)),
                new PropertyByName<Product>("AvailableForPreOrder", p => p.AvailableForPreOrder, await IgnoreExportProductProperty(p => p.AvailableForPreOrder)),
                new PropertyByName<Product>("PreOrderAvailabilityStartDateTimeUtc", p => p.PreOrderAvailabilityStartDateTimeUtc, await IgnoreExportProductProperty(p => p.AvailableForPreOrder)),
                new PropertyByName<Product>("CallForPrice", p => p.CallForPrice, await IgnoreExportProductProperty(p => p.CallForPrice)),
                new PropertyByName<Product>("Price", p => p.Price),
                new PropertyByName<Product>("OldPrice", p => p.OldPrice, await IgnoreExportProductProperty(p => p.OldPrice)),
                new PropertyByName<Product>("ProductCost", p => p.ProductCost, await IgnoreExportProductProperty(p => p.ProductCost)),
                new PropertyByName<Product>("CustomerEntersPrice", p => p.CustomerEntersPrice, await IgnoreExportProductProperty(p => p.CustomerEntersPrice)),
                new PropertyByName<Product>("MinimumCustomerEnteredPrice", p => p.MinimumCustomerEnteredPrice, await IgnoreExportProductProperty(p => p.CustomerEntersPrice)),
                new PropertyByName<Product>("MaximumCustomerEnteredPrice", p => p.MaximumCustomerEnteredPrice, await IgnoreExportProductProperty(p => p.CustomerEntersPrice)),
                new PropertyByName<Product>("BasepriceEnabled", p => p.BasepriceEnabled, await IgnoreExportProductProperty(p => p.PAngV)),
                new PropertyByName<Product>("BasepriceAmount", p => p.BasepriceAmount, await IgnoreExportProductProperty(p => p.PAngV)),
                new PropertyByName<Product>("BasepriceUnit", p => p.BasepriceUnitId, await IgnoreExportProductProperty(p => p.PAngV))
                {
                    DropDownElements = (await _measureService.GetAllMeasureWeights()).Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("BasepriceBaseAmount", p => p.BasepriceBaseAmount, await IgnoreExportProductProperty(p => p.PAngV)),
                new PropertyByName<Product>("BasepriceBaseUnit", p => p.BasepriceBaseUnitId, await IgnoreExportProductProperty(p => p.PAngV))
                {
                    DropDownElements = (await _measureService.GetAllMeasureWeights()).Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("MarkAsNew", p => p.MarkAsNew, await IgnoreExportProductProperty(p => p.MarkAsNew)),
                new PropertyByName<Product>("MarkAsNewStartDateTimeUtc", p => p.MarkAsNewStartDateTimeUtc, await IgnoreExportProductProperty(p => p.MarkAsNew)),
                new PropertyByName<Product>("MarkAsNewEndDateTimeUtc", p => p.MarkAsNewEndDateTimeUtc, await IgnoreExportProductProperty(p => p.MarkAsNew)),
                new PropertyByName<Product>("Weight", p => p.Weight, await IgnoreExportProductProperty(p => p.Weight)),
                new PropertyByName<Product>("Length", p => p.Length, await IgnoreExportProductProperty(p => p.Dimensions)),
                new PropertyByName<Product>("Width", p => p.Width, await IgnoreExportProductProperty(p => p.Dimensions)),
                new PropertyByName<Product>("Height", p => p.Height, await IgnoreExportProductProperty(p => p.Dimensions)),
                new PropertyByName<Product>("Categories", GetCategories),
                new PropertyByName<Product>("Manufacturers", GetManufacturers, await IgnoreExportProductProperty(p => p.Manufacturers)),
                new PropertyByName<Product>("ProductTags", GetProductTags, await IgnoreExportProductProperty(p => p.ProductTags)),
                new PropertyByName<Product>("IsLimitedToStores", p => p.LimitedToStores, await IgnoreExportLimitedToStore()),
                new PropertyByName<Product>("LimitedToStores", GetLimitedToStores, await IgnoreExportLimitedToStore()),
                new PropertyByName<Product>("Picture1", async p => await GetPicture(p, 0)),
                new PropertyByName<Product>("Picture2", async p => await GetPicture(p, 1)),
                new PropertyByName<Product>("Picture3", async p => await GetPicture(p, 2))
            };

            var productList = products.ToList();

            var productAdvancedMode = true;
            try
            {
                productAdvancedMode = await _genericAttributeService.GetAttribute<bool>(await _workContext.GetCurrentCustomer(), "product-advanced-mode");
            }
            catch (ArgumentNullException)
            {
            }

            if (!_catalogSettings.ExportImportProductAttributes && !_catalogSettings.ExportImportProductSpecificationAttributes)
                return await new PropertyManager<Product>(properties, _catalogSettings).ExportToXlsx(productList);

            if (productAdvancedMode || _productEditorSettings.ProductAttributes)
                return await ExportProductsToXlsxWithAttributes(properties, productList);

            return await new PropertyManager<Product>(properties, _catalogSettings).ExportToXlsx(productList);
        }

        /// <summary>
        /// Export order list to XML
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <returns>Result in XML format</returns>
        public virtual async Task<string> ExportOrdersToXml(IList<Order> orders)
        {
            //a vendor should have access only to part of order information
            var ignore = await _workContext.GetCurrentVendor() != null;

            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

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
                    var orderItems = await _orderService.GetOrderItems(order.Id, vendorId: (await _workContext.GetCurrentVendor())?.Id ?? 0);

                    if (orderItems.Any())
                    {
                        await xmlWriter.WriteStartElementAsync("OrderItems");
                        foreach (var orderItem in orderItems)
                        {
                            var product = await _productService.GetProductById(orderItem.ProductId);

                            await xmlWriter.WriteStartElementAsync("OrderItem");
                            await xmlWriter.WriteStringAsync("Id", orderItem.Id);
                            await xmlWriter.WriteStringAsync("OrderItemGuid", orderItem.OrderItemGuid);
                            await xmlWriter.WriteStringAsync("Name", product.Name);
                            await xmlWriter.WriteStringAsync("Sku", product.Sku);
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
                var shipments = (await _shipmentService.GetShipmentsByOrderId(order.Id)).OrderBy(x => x.CreatedOnUtc).ToList();
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

            return stringWriter.ToString();
        }

        /// <summary>
        /// Export orders to XLSX
        /// </summary>
        /// <param name="orders">Orders</param>
        public virtual async Task<byte[]> ExportOrdersToXlsx(IList<Order> orders)
        {
            //a vendor should have access only to part of order information
            var ignore = await _workContext.GetCurrentVendor() != null;

            //lambda expressions for choosing correct order address
            async Task<Address> orderAddress(Order o) => await _addressService.GetAddressById((o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) ?? 0);
            async Task<Address> orderBillingAddress(Order o) => await _addressService.GetAddressById(o.BillingAddressId);

            //property array
            var properties = new[]
            {
                new PropertyByName<Order>("OrderId", p => p.Id),
                new PropertyByName<Order>("StoreId", p => p.StoreId),
                new PropertyByName<Order>("OrderGuid", p => p.OrderGuid, ignore),
                new PropertyByName<Order>("CustomerId", p => p.CustomerId, ignore),
                new PropertyByName<Order>("OrderStatusId", p => p.OrderStatusId, ignore),
                new PropertyByName<Order>("PaymentStatusId", p => p.PaymentStatusId),
                new PropertyByName<Order>("ShippingStatusId", p => p.ShippingStatusId, ignore),
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
                new PropertyByName<Order>("CreatedOnUtc", p => p.CreatedOnUtc.ToOADate()),
                new PropertyByName<Order>("BillingFirstName", async p => (await orderBillingAddress(p))?.FirstName ?? string.Empty),
                new PropertyByName<Order>("BillingLastName", async p => (await orderBillingAddress(p))?.LastName ?? string.Empty),
                new PropertyByName<Order>("BillingEmail", async p => (await orderBillingAddress(p))?.Email ?? string.Empty),
                new PropertyByName<Order>("BillingCompany", async p => (await orderBillingAddress(p))?.Company ?? string.Empty),
                new PropertyByName<Order>("BillingCountry", async p => (await _countryService.GetCountryByAddress(await orderBillingAddress(p)))?.Name ?? string.Empty),
                new PropertyByName<Order>("BillingStateProvince", async p => (await _stateProvinceService.GetStateProvinceByAddress(await orderBillingAddress(p)))?.Name ?? string.Empty),
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
                new PropertyByName<Order>("ShippingCountry", async p => (await _countryService.GetCountryByAddress(await orderAddress(p)))?.Name ?? string.Empty),
                new PropertyByName<Order>("ShippingStateProvince", async p => (await _stateProvinceService.GetStateProvinceByAddress(await orderAddress(p)))?.Name ?? string.Empty),
                new PropertyByName<Order>("ShippingCounty", async p => (await orderAddress(p))?.County ?? string.Empty),
                new PropertyByName<Order>("ShippingCity", async p => (await orderAddress(p))?.City ?? string.Empty),
                new PropertyByName<Order>("ShippingAddress1", async p => (await orderAddress(p))?.Address1 ?? string.Empty),
                new PropertyByName<Order>("ShippingAddress2", async p => (await orderAddress(p))?.Address2 ?? string.Empty),
                new PropertyByName<Order>("ShippingZipPostalCode", async p => (await orderAddress(p))?.ZipPostalCode ?? string.Empty),
                new PropertyByName<Order>("ShippingPhoneNumber", async p => (await orderAddress(p))?.PhoneNumber ?? string.Empty),
                new PropertyByName<Order>("ShippingFaxNumber", async p => (await orderAddress(p))?.FaxNumber ?? string.Empty)
            };

            return _orderSettings.ExportWithProducts
                ? await ExportOrderToXlsxWithProducts(properties, orders)
                : await new PropertyManager<Order>(properties, _catalogSettings).ExportToXlsx(orders);
        }

        /// <summary>
        /// Export customer list to XLSX
        /// </summary>
        /// <param name="customers">Customers</param>
        public virtual async Task<byte[]> ExportCustomersToXlsx(IList<Customer> customers)
        {
            //property manager 
            var manager = new PropertyManager<Customer>(new[]
            {
                new PropertyByName<Customer>("CustomerId", p => p.Id),
                new PropertyByName<Customer>("CustomerGuid", p => p.CustomerGuid),
                new PropertyByName<Customer>("Email", p => p.Email),
                new PropertyByName<Customer>("Username", p => p.Username),
                new PropertyByName<Customer>("Password", async p => (await _customerService.GetCurrentPassword(p.Id))?.Password),
                new PropertyByName<Customer>("PasswordFormatId", async p => (await _customerService.GetCurrentPassword(p.Id))?.PasswordFormatId ?? 0),
                new PropertyByName<Customer>("PasswordSalt", async p => (await _customerService.GetCurrentPassword(p.Id))?.PasswordSalt),
                new PropertyByName<Customer>("IsTaxExempt", p => p.IsTaxExempt),
                new PropertyByName<Customer>("AffiliateId", p => p.AffiliateId),
                new PropertyByName<Customer>("VendorId", p => p.VendorId),
                new PropertyByName<Customer>("Active", p => p.Active),
                new PropertyByName<Customer>("IsGuest", async p => await _customerService.IsGuest(p)),
                new PropertyByName<Customer>("IsRegistered", async p => await _customerService.IsRegistered(p)),
                new PropertyByName<Customer>("IsAdministrator", async p => await _customerService.IsAdmin(p)),
                new PropertyByName<Customer>("IsForumModerator", async p => await _customerService.IsForumModerator(p)),
                new PropertyByName<Customer>("CreatedOnUtc", p => p.CreatedOnUtc),
                //attributes
                new PropertyByName<Customer>("FirstName", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.FirstNameAttribute)),
                new PropertyByName<Customer>("LastName", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.LastNameAttribute)),
                new PropertyByName<Customer>("Gender", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.GenderAttribute)),
                new PropertyByName<Customer>("Company", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.CompanyAttribute)),
                new PropertyByName<Customer>("StreetAddress", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.StreetAddressAttribute)),
                new PropertyByName<Customer>("StreetAddress2", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.StreetAddress2Attribute)),
                new PropertyByName<Customer>("ZipPostalCode", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.ZipPostalCodeAttribute)),
                new PropertyByName<Customer>("City", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.CityAttribute)),
                new PropertyByName<Customer>("County", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.CountyAttribute)),
                new PropertyByName<Customer>("CountryId", async p => await _genericAttributeService.GetAttribute<int>(p, NopCustomerDefaults.CountryIdAttribute)),
                new PropertyByName<Customer>("StateProvinceId", async p => await _genericAttributeService.GetAttribute<int>(p, NopCustomerDefaults.StateProvinceIdAttribute)),
                new PropertyByName<Customer>("Phone", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.PhoneAttribute)),
                new PropertyByName<Customer>("Fax", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.FaxAttribute)),
                new PropertyByName<Customer>("VatNumber", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.VatNumberAttribute)),
                new PropertyByName<Customer>("VatNumberStatusId", async p => await _genericAttributeService.GetAttribute<int>(p, NopCustomerDefaults.VatNumberStatusIdAttribute)),
                new PropertyByName<Customer>("TimeZoneId", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.TimeZoneIdAttribute)),
                new PropertyByName<Customer>("AvatarPictureId", async p => await _genericAttributeService.GetAttribute<int>(p, NopCustomerDefaults.AvatarPictureIdAttribute)),
                new PropertyByName<Customer>("ForumPostCount", async p => await _genericAttributeService.GetAttribute<int>(p, NopCustomerDefaults.ForumPostCountAttribute)),
                new PropertyByName<Customer>("Signature", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.SignatureAttribute)),
                new PropertyByName<Customer>("CustomCustomerAttributes",  GetCustomCustomerAttributes)
            }, _catalogSettings);

            return await manager.ExportToXlsx(customers);
        }

        /// <summary>
        /// Export customer list to XML
        /// </summary>
        /// <param name="customers">Customers</param>
        /// <returns>Result in XML format</returns>
        public virtual async Task<string> ExportCustomersToXml(IList<Customer> customers)
        {
            var settings = new XmlWriterSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

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

                var customerPassword = await _customerService.GetCurrentPassword(customer.Id);
                await xmlWriter.WriteElementStringAsync("Password", null, customerPassword?.Password);
                await xmlWriter.WriteElementStringAsync("PasswordFormatId", null, (customerPassword?.PasswordFormatId ?? 0).ToString());
                await xmlWriter.WriteElementStringAsync("PasswordSalt", null, customerPassword?.PasswordSalt);

                await xmlWriter.WriteElementStringAsync("IsTaxExempt", null, customer.IsTaxExempt.ToString());
                await xmlWriter.WriteElementStringAsync("AffiliateId", null, customer.AffiliateId.ToString());
                await xmlWriter.WriteElementStringAsync("VendorId", null, customer.VendorId.ToString());
                await xmlWriter.WriteElementStringAsync("Active", null, customer.Active.ToString());

                await xmlWriter.WriteElementStringAsync("IsGuest", null, (await _customerService.IsGuest(customer)).ToString());
                await xmlWriter.WriteElementStringAsync("IsRegistered", null, (await _customerService.IsRegistered(customer)).ToString());
                await xmlWriter.WriteElementStringAsync("IsAdministrator", null, (await _customerService.IsAdmin(customer)).ToString());
                await xmlWriter.WriteElementStringAsync("IsForumModerator", null, (await _customerService.IsForumModerator(customer)).ToString());
                await xmlWriter.WriteElementStringAsync("CreatedOnUtc", null, customer.CreatedOnUtc.ToString(CultureInfo.InvariantCulture));

                await xmlWriter.WriteElementStringAsync("FirstName", null, await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute));
                await xmlWriter.WriteElementStringAsync("LastName", null, await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute));
                await xmlWriter.WriteElementStringAsync("Gender", null, await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.GenderAttribute));
                await xmlWriter.WriteElementStringAsync("Company", null, await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute));

                await xmlWriter.WriteElementStringAsync("CountryId", null, (await _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute)).ToString());
                await xmlWriter.WriteElementStringAsync("StreetAddress", null, await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddressAttribute));
                await xmlWriter.WriteElementStringAsync("StreetAddress2", null, await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddress2Attribute));
                await xmlWriter.WriteElementStringAsync("ZipPostalCode", null, await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute));
                await xmlWriter.WriteElementStringAsync("City", null, await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CityAttribute));
                await xmlWriter.WriteElementStringAsync("County", null, await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CountyAttribute));
                await xmlWriter.WriteElementStringAsync("StateProvinceId", null, (await _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute)).ToString());
                await xmlWriter.WriteElementStringAsync("Phone", null, await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute));
                await xmlWriter.WriteElementStringAsync("Fax", null, await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FaxAttribute));
                await xmlWriter.WriteElementStringAsync("VatNumber", null, await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.VatNumberAttribute));
                await xmlWriter.WriteElementStringAsync("VatNumberStatusId", null, (await _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.VatNumberStatusIdAttribute)).ToString());
                await xmlWriter.WriteElementStringAsync("TimeZoneId", null, await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.TimeZoneIdAttribute));

                foreach (var store in await _storeService.GetAllStores())
                {
                    var newsletter = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id);
                    var subscribedToNewsletters = newsletter != null && newsletter.Active;
                    await xmlWriter.WriteElementStringAsync($"Newsletter-in-store-{store.Id}", null, subscribedToNewsletters.ToString());
                }

                await xmlWriter.WriteElementStringAsync("AvatarPictureId", null, (await _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute)).ToString());
                await xmlWriter.WriteElementStringAsync("ForumPostCount", null, (await _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.ForumPostCountAttribute)).ToString());
                await xmlWriter.WriteElementStringAsync("Signature", null, await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.SignatureAttribute));

                var selectedCustomerAttributesString = await _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CustomCustomerAttributes);

                if (!string.IsNullOrEmpty(selectedCustomerAttributesString))
                {
                    var selectedCustomerAttributes = new StringReader(selectedCustomerAttributesString);
                    var selectedCustomerAttributesXmlReader = XmlReader.Create(selectedCustomerAttributes);
                    await xmlWriter.WriteNodeAsync(selectedCustomerAttributesXmlReader, false);
                }

                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();

            return stringWriter.ToString();
        }

        /// <summary>
        /// Export newsletter subscribers to TXT
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>Result in TXT (string) format</returns>
        public virtual string ExportNewsletterSubscribersToTxt(IList<NewsLetterSubscription> subscriptions)
        {
            if (subscriptions == null)
                throw new ArgumentNullException(nameof(subscriptions));

            const string separator = ",";
            var sb = new StringBuilder();

            sb.Append(_localizationService.GetResource("Admin.Promotions.NewsLetterSubscriptions.Fields.Email"));
            sb.Append(separator);
            sb.Append(_localizationService.GetResource("Admin.Promotions.NewsLetterSubscriptions.Fields.Active"));
            sb.Append(separator);
            sb.Append(_localizationService.GetResource("Admin.Promotions.NewsLetterSubscriptions.Fields.Store"));
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

            return sb.ToString();
        }

        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>Result in TXT (string) format</returns>
        public virtual async Task<string> ExportStatesToTxt(IList<StateProvince> states)
        {
            if (states == null)
                throw new ArgumentNullException(nameof(states));

            const string separator = ",";
            var sb = new StringBuilder();
            foreach (var state in states)
            {
                sb.Append((await _countryService.GetCountryById(state.CountryId)).TwoLetterIsoCode);
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

            return sb.ToString();
        }

        /// <summary>
        /// Export customer info (GDPR request) to XLSX 
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Customer GDPR info</returns>
        public virtual async Task<byte[]> ExportCustomerGdprInfoToXlsx(Customer customer, int storeId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //lambda expressions for choosing correct order address
            async Task<Address> orderAddress(Order o) => await _addressService.GetAddressById((o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) ?? 0);
            async Task<Address> orderBillingAddress(Order o) => await _addressService.GetAddressById(o.BillingAddressId);

            //customer info and customer attributes
            var customerManager = new PropertyManager<Customer>(new[]
            {
                new PropertyByName<Customer>("Email", p => p.Email),
                new PropertyByName<Customer>("Username", p => p.Username, !_customerSettings.UsernamesEnabled), 
                //attributes
                new PropertyByName<Customer>("First name", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.FirstNameAttribute), !_customerSettings.FirstNameEnabled),
                new PropertyByName<Customer>("Last name", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.LastNameAttribute), !_customerSettings.LastNameEnabled),
                new PropertyByName<Customer>("Gender", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.GenderAttribute), !_customerSettings.GenderEnabled),
                new PropertyByName<Customer>("Date of birth", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.DateOfBirthAttribute), !_customerSettings.DateOfBirthEnabled),
                new PropertyByName<Customer>("Company", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.CompanyAttribute), !_customerSettings.CompanyEnabled),
                new PropertyByName<Customer>("Street address", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.StreetAddressAttribute), !_customerSettings.StreetAddressEnabled),
                new PropertyByName<Customer>("Street address 2", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.StreetAddress2Attribute), !_customerSettings.StreetAddress2Enabled),
                new PropertyByName<Customer>("Zip / postal code", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.ZipPostalCodeAttribute), !_customerSettings.ZipPostalCodeEnabled),
                new PropertyByName<Customer>("City", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.CityAttribute), !_customerSettings.CityEnabled),
                new PropertyByName<Customer>("County", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.CountyAttribute), !_customerSettings.CountyEnabled),
                new PropertyByName<Customer>("Country", async p => (await _countryService.GetCountryById(await _genericAttributeService.GetAttribute<int>(p, NopCustomerDefaults.CountryIdAttribute)))?.Name ?? string.Empty, !_customerSettings.CountryEnabled),
                new PropertyByName<Customer>("State province", async p => (await _stateProvinceService.GetStateProvinceById(await _genericAttributeService.GetAttribute<int>(p, NopCustomerDefaults.StateProvinceIdAttribute)))?.Name ?? string.Empty, !(_customerSettings.StateProvinceEnabled && _customerSettings.CountryEnabled)),
                new PropertyByName<Customer>("Phone", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.PhoneAttribute), !_customerSettings.PhoneEnabled),
                new PropertyByName<Customer>("Fax", async p => await _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.FaxAttribute), !_customerSettings.FaxEnabled),
                new PropertyByName<Customer>("Customer attributes",  GetCustomCustomerAttributes)
            }, _catalogSettings);

            //customer orders
            var orderManager = new PropertyManager<Order>(new[]
            {
                new PropertyByName<Order>("Order Number", p => p.CustomOrderNumber),
                new PropertyByName<Order>("Order status", async p => await _localizationService.GetLocalizedEnum(p.OrderStatus)),
                new PropertyByName<Order>("Order total", async p => await _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(p.OrderTotal, p.CurrencyRate), true, p.CustomerCurrencyCode, false, (await _workContext.GetWorkingLanguage()).Id)),
                new PropertyByName<Order>("Shipping method", p => p.ShippingMethod),
                new PropertyByName<Order>("Created on", p => _dateTimeHelper.ConvertToUserTime(p.CreatedOnUtc, DateTimeKind.Utc).ToString("D")),
                new PropertyByName<Order>("Billing first name", async p => (await orderBillingAddress(p))?.FirstName ?? string.Empty),
                new PropertyByName<Order>("Billing last name", async p => (await orderBillingAddress(p))?.LastName ?? string.Empty),
                new PropertyByName<Order>("Billing email", async p => (await orderBillingAddress(p))?.Email ?? string.Empty),
                new PropertyByName<Order>("Billing company", async p => (await orderBillingAddress(p))?.Company ?? string.Empty, !_addressSettings.CompanyEnabled),
                new PropertyByName<Order>("Billing country", async p => await _countryService.GetCountryByAddress(await orderBillingAddress(p)) is Country country ? await _localizationService.GetLocalized(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
                new PropertyByName<Order>("Billing state province", async p => await _stateProvinceService.GetStateProvinceByAddress(await orderBillingAddress(p)) is StateProvince stateProvince ? await _localizationService.GetLocalized(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
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
                new PropertyByName<Order>("Shipping country", async p => await _countryService.GetCountryByAddress(await orderAddress(p)) is Country country ? await _localizationService.GetLocalized(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
                new PropertyByName<Order>("Shipping state province", async p => await _stateProvinceService.GetStateProvinceByAddress(await orderAddress(p)) is StateProvince stateProvince ? await _localizationService.GetLocalized(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
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
                new PropertyByName<OrderItem>("SKU", async oi => (await _productService.GetProductById(oi.ProductId)).Sku),
                new PropertyByName<OrderItem>("Name", async oi => await _localizationService.GetLocalized(await _productService.GetProductById(oi.ProductId), p => p.Name)),
                new PropertyByName<OrderItem>("Price", async oi => await _priceFormatter.FormatPrice(_currencyService.ConvertCurrency((await _orderService.GetOrderById(oi.OrderId)).CustomerTaxDisplayType == TaxDisplayType.IncludingTax ? oi.UnitPriceInclTax : oi.UnitPriceExclTax, (await _orderService.GetOrderById(oi.OrderId)).CurrencyRate), true, (await _orderService.GetOrderById(oi.OrderId)).CustomerCurrencyCode, false, (await _workContext.GetWorkingLanguage()).Id)),
                new PropertyByName<OrderItem>("Quantity", oi => oi.Quantity),
                new PropertyByName<OrderItem>("Total", async oi => await _priceFormatter.FormatPrice((await _orderService.GetOrderById(oi.OrderId)).CustomerTaxDisplayType == TaxDisplayType.IncludingTax ? oi.PriceInclTax : oi.PriceExclTax))
            }, _catalogSettings);

            var orders = await _orderService.SearchOrders(customerId: customer.Id);

            //customer addresses
            var addressManager = new PropertyManager<Address>(new[]
            {
                new PropertyByName<Address>("First name", p => p.FirstName),
                new PropertyByName<Address>("Last name", p => p.LastName),
                new PropertyByName<Address>("Email", p => p.Email),
                new PropertyByName<Address>("Company", p => p.Company, !_addressSettings.CompanyEnabled),
                new PropertyByName<Address>("Country", async p => await _countryService.GetCountryByAddress(p) is Country country ? await _localizationService.GetLocalized(country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
                new PropertyByName<Address>("State province", async p => await _stateProvinceService.GetStateProvinceByAddress(p) is StateProvince stateProvince ? await _localizationService.GetLocalized(stateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
                new PropertyByName<Address>("County", p => p.County, !_addressSettings.CountyEnabled),
                new PropertyByName<Address>("City", p => p.City, !_addressSettings.CityEnabled),
                new PropertyByName<Address>("Address 1", p => p.Address1, !_addressSettings.StreetAddressEnabled),
                new PropertyByName<Address>("Address 2", p => p.Address2, !_addressSettings.StreetAddress2Enabled),
                new PropertyByName<Address>("Zip / postal code", p => p.ZipPostalCode, !_addressSettings.ZipPostalCodeEnabled),
                new PropertyByName<Address>("Phone number", p => p.PhoneNumber, !_addressSettings.PhoneEnabled),
                new PropertyByName<Address>("Fax number", p => p.FaxNumber, !_addressSettings.FaxEnabled),
                new PropertyByName<Address>("Custom attributes", async p => await _customerAttributeFormatter.FormatAttributes(p.CustomAttributes, ";"))
            }, _catalogSettings);

            //customer private messages
            var privateMessageManager = new PropertyManager<PrivateMessage>(new[]
            {
                new PropertyByName<PrivateMessage>("From", async pm => await _customerService.GetCustomerById(pm.FromCustomerId) is Customer cFrom ? (_customerSettings.UsernamesEnabled ? cFrom.Username : cFrom.Email) : string.Empty),
                new PropertyByName<PrivateMessage>("To", async pm => await _customerService.GetCustomerById(pm.ToCustomerId) is Customer cTo ? (_customerSettings.UsernamesEnabled ? cTo.Username : cTo.Email) : string.Empty),
                new PropertyByName<PrivateMessage>("Subject", pm => pm.Subject),
                new PropertyByName<PrivateMessage>("Text", pm => pm.Text),
                new PropertyByName<PrivateMessage>("Created on", pm => _dateTimeHelper.ConvertToUserTime(pm.CreatedOnUtc, DateTimeKind.Utc).ToString("D"))
            }, _catalogSettings);

            List<PrivateMessage> pmList = null;
            if (_forumSettings.AllowPrivateMessages)
            {
                pmList = (await _forumService.GetAllPrivateMessages(storeId, customer.Id, 0, null, null, null, null)).ToList();
                pmList.AddRange((await _forumService.GetAllPrivateMessages(storeId, 0, customer.Id, null, null, null, null)).ToList());
            }

            //customer GDPR logs
            var gdprLogManager = new PropertyManager<GdprLog>(new[]
            {
                new PropertyByName<GdprLog>("Request type", async log => await _localizationService.GetLocalizedEnum(log.RequestType)),
                new PropertyByName<GdprLog>("Request details", log => log.RequestDetails),
                new PropertyByName<GdprLog>("Created on", log => _dateTimeHelper.ConvertToUserTime(log.CreatedOnUtc, DateTimeKind.Utc).ToString("D"))
            }, _catalogSettings);

            var gdprLog = await _gdprService.GetAllLog(customer.Id);

            await using var stream = new MemoryStream();
            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(stream))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handles to the worksheets
                var customerInfoWorksheet = xlPackage.Workbook.Worksheets.Add("Customer info");
                var fWorksheet = xlPackage.Workbook.Worksheets.Add("DataForFilters");
                fWorksheet.Hidden = eWorkSheetHidden.VeryHidden;

                //customer info and customer attributes
                var customerInfoRow = 2;
                customerManager.CurrentObject = customer;
                customerManager.WriteCaption(customerInfoWorksheet);
                await customerManager.WriteToXlsx(customerInfoWorksheet, customerInfoRow);

                //customer addresses
                if (await _customerService.GetAddressesByCustomerId(customer.Id) is IList<Address> addresses && addresses.Any())
                {
                    customerInfoRow += 2;

                    var cell = customerInfoWorksheet.Cells[customerInfoRow, 1];
                    cell.Value = "Address List";
                    customerInfoRow += 1;
                    addressManager.SetCaptionStyle(cell);
                    addressManager.WriteCaption(customerInfoWorksheet, customerInfoRow);

                    foreach (var customerAddress in addresses)
                    {
                        customerInfoRow += 1;
                        addressManager.CurrentObject = customerAddress;
                        await addressManager.WriteToXlsx(customerInfoWorksheet, customerInfoRow);
                    }
                }

                //customer orders
                if (orders.Any())
                {
                    var ordersWorksheet = xlPackage.Workbook.Worksheets.Add("Orders");

                    orderManager.WriteCaption(ordersWorksheet);

                    var orderRow = 1;

                    foreach (var order in orders)
                    {
                        orderRow += 1;
                        orderManager.CurrentObject = order;
                        await orderManager.WriteToXlsx(ordersWorksheet, orderRow);

                        //products
                        var orederItems = await _orderService.GetOrderItems(order.Id);

                        if (!orederItems.Any())
                            continue;

                        orderRow += 1;

                        orderItemsManager.WriteCaption(ordersWorksheet, orderRow, 2);
                        ordersWorksheet.Row(orderRow).OutlineLevel = 1;
                        ordersWorksheet.Row(orderRow).Collapsed = true;

                        foreach (var orederItem in orederItems)
                        {
                            orderRow++;
                            orderItemsManager.CurrentObject = orederItem;
                            await orderItemsManager.WriteToXlsx(ordersWorksheet, orderRow, 2, fWorksheet);
                            ordersWorksheet.Row(orderRow).OutlineLevel = 1;
                            ordersWorksheet.Row(orderRow).Collapsed = true;
                        }
                    }
                }

                //customer private messages
                if (pmList?.Any() ?? false)
                {
                    var privateMessageWorksheet = xlPackage.Workbook.Worksheets.Add("Private messages");
                    privateMessageManager.WriteCaption(privateMessageWorksheet);

                    var privateMessageRow = 1;

                    foreach (var privateMessage in pmList)
                    {
                        privateMessageRow += 1;

                        privateMessageManager.CurrentObject = privateMessage;
                        await privateMessageManager.WriteToXlsx(privateMessageWorksheet, privateMessageRow);
                    }
                }

                //customer GDPR logs
                if (gdprLog.Any())
                {
                    var gdprLogWorksheet = xlPackage.Workbook.Worksheets.Add("GDPR requests (log)");
                    gdprLogManager.WriteCaption(gdprLogWorksheet);

                    var gdprLogRow = 1;

                    foreach (var log in gdprLog)
                    {
                        gdprLogRow += 1;

                        gdprLogManager.CurrentObject = log;
                        await gdprLogManager.WriteToXlsx(gdprLogWorksheet, gdprLogRow);
                    }
                }

                xlPackage.Save();
            }

            return stream.ToArray();
        }

        #endregion
    }
}
