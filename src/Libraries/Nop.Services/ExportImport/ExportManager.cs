using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
using Nop.Services.ExportImport.Help;
using Nop.Services.Forums;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
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
        private readonly ICategoryService _categoryService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerAttributeFormatter _customerAttributeFormatter;
        private readonly ICustomerService _customerService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IDateTimeHelper _dateTimeHelper;
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
        private readonly IProductTagService _productTagService;
        private readonly IProductTemplateService _productTemplateService;
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
            ICategoryService categoryService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerAttributeFormatter customerAttributeFormatter,
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
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
            IProductTagService productTagService,
            IProductTemplateService productTemplateService,
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
            _forumSettings = forumSettings;
            _categoryService = categoryService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerAttributeFormatter = customerAttributeFormatter;
            _customerService = customerService;
            _dateRangeService = dateRangeService;
            _dateTimeHelper = dateTimeHelper;
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
            _productTagService = productTagService;
            _productTemplateService = productTemplateService;
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

        protected virtual void WriteCategories(XmlWriter xmlWriter, int parentCategoryId)
        {
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId, true);
            if (categories == null || !categories.Any())
                return;

            foreach (var category in categories)
            {
                xmlWriter.WriteStartElement("Category");

                xmlWriter.WriteString("Id", category.Id);

                xmlWriter.WriteString("Name", category.Name);
                xmlWriter.WriteString("Description", category.Description);
                xmlWriter.WriteString("CategoryTemplateId", category.CategoryTemplateId);
                xmlWriter.WriteString("MetaKeywords", category.MetaKeywords, IgnoreExportCategoryProperty());
                xmlWriter.WriteString("MetaDescription", category.MetaDescription, IgnoreExportCategoryProperty());
                xmlWriter.WriteString("MetaTitle", category.MetaTitle, IgnoreExportCategoryProperty());
                xmlWriter.WriteString("SeName", _urlRecordService.GetSeName(category, 0), IgnoreExportCategoryProperty());
                xmlWriter.WriteString("ParentCategoryId", category.ParentCategoryId);
                xmlWriter.WriteString("PictureId", category.PictureId);
                xmlWriter.WriteString("PageSize", category.PageSize, IgnoreExportCategoryProperty());
                xmlWriter.WriteString("AllowCustomersToSelectPageSize", category.AllowCustomersToSelectPageSize, IgnoreExportCategoryProperty());
                xmlWriter.WriteString("PageSizeOptions", category.PageSizeOptions, IgnoreExportCategoryProperty());
                xmlWriter.WriteString("PriceRanges", category.PriceRanges, IgnoreExportCategoryProperty());
                xmlWriter.WriteString("ShowOnHomepage", category.ShowOnHomepage, IgnoreExportCategoryProperty());
                xmlWriter.WriteString("IncludeInTopMenu", category.IncludeInTopMenu, IgnoreExportCategoryProperty());
                xmlWriter.WriteString("Published", category.Published, IgnoreExportCategoryProperty());
                xmlWriter.WriteString("Deleted", category.Deleted, true);
                xmlWriter.WriteString("DisplayOrder", category.DisplayOrder);
                xmlWriter.WriteString("CreatedOnUtc", category.CreatedOnUtc, IgnoreExportCategoryProperty());
                xmlWriter.WriteString("UpdatedOnUtc", category.UpdatedOnUtc, IgnoreExportCategoryProperty());

                xmlWriter.WriteStartElement("Products");
                var productCategories = _categoryService.GetProductCategoriesByCategoryId(category.Id, showHidden: true);
                foreach (var productCategory in productCategories)
                {
                    var product = productCategory.Product;
                    if (product == null || product.Deleted)
                        continue;

                    xmlWriter.WriteStartElement("ProductCategory");
                    xmlWriter.WriteString("ProductCategoryId", productCategory.Id);
                    xmlWriter.WriteString("ProductId", productCategory.ProductId);
                    xmlWriter.WriteString("ProductName", product.Name);
                    xmlWriter.WriteString("IsFeaturedProduct", productCategory.IsFeaturedProduct);
                    xmlWriter.WriteString("DisplayOrder", productCategory.DisplayOrder);
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("SubCategories");
                WriteCategories(xmlWriter, category.Id);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// Returns the path to the image file by ID
        /// </summary>
        /// <param name="pictureId">Picture ID</param>
        /// <returns>Path to the image file</returns>
        protected virtual string GetPictures(int pictureId)
        {
            var picture = _pictureService.GetPictureById(pictureId);
            return _pictureService.GetThumbLocalPath(picture);
        }

        /// <summary>
        /// Returns the list of categories for a product separated by a ";"
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of categories</returns>
        protected virtual string GetCategories(Product product)
        {
            string categoryNames = null;
            foreach (var pc in _categoryService.GetProductCategoriesByProductId(product.Id, true))
            {
                if (_catalogSettings.ExportImportRelatedEntitiesByName)
                {
                    categoryNames += _catalogSettings.ExportImportProductCategoryBreadcrumb
                        ? _categoryService.GetFormattedBreadCrumb(pc.Category)
                        : pc.Category.Name;
                }
                else
                {
                    categoryNames += pc.Category.Id.ToString();
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
        protected virtual string GetManufacturers(Product product)
        {
            string manufacturerNames = null;
            foreach (var pm in _manufacturerService.GetProductManufacturersByProductId(product.Id, true))
            {
                manufacturerNames += _catalogSettings.ExportImportRelatedEntitiesByName
                    ? pm.Manufacturer.Name
                    : pm.Manufacturer.Id.ToString();

                manufacturerNames += ";";
            }

            return manufacturerNames;
        }

        /// <summary>
        /// Returns the list of limited to stores for a product separated by a ";"
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of store</returns>
        protected virtual string GetLimitedToStores(Product product)
        {
            string limitedToStores = null;
            foreach (var storeMapping in _storeMappingService.GetStoreMappings(product))
            {
                limitedToStores += _catalogSettings.ExportImportRelatedEntitiesByName
                    ? storeMapping.Store.Name
                    : storeMapping.Store.Id.ToString();

                limitedToStores += ";";
            }

            return limitedToStores;
        }

        /// <summary>
        /// Returns the list of product tag for a product separated by a ";"
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of product tag</returns>
        protected virtual string GetProductTags(Product product)
        {
            string productTagNames = null;

            var productTags = _productTagService.GetAllProductTagsByProductId(product.Id);

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
        /// Returns the three first image associated with the product
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>three first image</returns>
        protected virtual string[] GetPictures(Product product)
        {
            //pictures (up to 3 pictures)
            string picture1 = null;
            string picture2 = null;
            string picture3 = null;
            var pictures = _pictureService.GetPicturesByProductId(product.Id, 3);
            for (var i = 0; i < pictures.Count; i++)
            {
                var pictureLocalPath = _pictureService.GetThumbLocalPath(pictures[i]);
                switch (i)
                {
                    case 0:
                        picture1 = pictureLocalPath;
                        break;
                    case 1:
                        picture2 = pictureLocalPath;
                        break;
                    case 2:
                        picture3 = pictureLocalPath;
                        break;
                }
            }

            return new[] { picture1, picture2, picture3 };
        }

        protected virtual bool IgnoreExportPoductProperty(Func<ProductEditorSettings, bool> func)
        {
            var productAdvancedMode = true;
            try
            {
                productAdvancedMode = _genericAttributeService.GetAttribute<bool>(_workContext.CurrentCustomer, "product-advanced-mode");
            }
            catch (ArgumentNullException)
            {
            }

            return !productAdvancedMode && !func(_productEditorSettings);
        }

        protected virtual bool IgnoreExportCategoryProperty()
        {
            try
            {
                return !_genericAttributeService.GetAttribute<bool>(_workContext.CurrentCustomer, "category-advanced-mode");
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        protected virtual bool IgnoreExportManufacturerProperty()
        {
            try
            {
                return !_genericAttributeService.GetAttribute<bool>(_workContext.CurrentCustomer, "manufacturer-advanced-mode");
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        protected virtual bool IgnoreExportLimitedToStore()
        {
            return _catalogSettings.IgnoreStoreLimitations || 
                   !_catalogSettings.ExportImportProductUseLimitedToStores ||
                   _storeService.GetAllStores().Count == 1;
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

        private PropertyManager<ExportSpecificationAttribute> GetSpecificationAttributeManager()
        {
            var attributeProperties = new[]
            {
                new PropertyByName<ExportSpecificationAttribute>("AttributeType", p => p.AttributeTypeId)
                {
                    DropDownElements = SpecificationAttributeType.Option.ToSelectList(useLocalization: false)
                },
                new PropertyByName<ExportSpecificationAttribute>("SpecificationAttribute", p => p.SpecificationAttributeId)
                {
                    DropDownElements = _specificationAttributeService.GetSpecificationAttributes().Select(sa => sa as BaseEntity).ToSelectList(p => (p as SpecificationAttribute)?.Name ?? string.Empty)
                },
                new PropertyByName<ExportSpecificationAttribute>("CustomValue", p => p.CustomValue),
                new PropertyByName<ExportSpecificationAttribute>("SpecificationAttributeOptionId", p => p.SpecificationAttributeOptionId),
                new PropertyByName<ExportSpecificationAttribute>("AllowFiltering", p => p.AllowFiltering),
                new PropertyByName<ExportSpecificationAttribute>("ShowOnProductPage", p => p.ShowOnProductPage),
                new PropertyByName<ExportSpecificationAttribute>("DisplayOrder", p => p.DisplayOrder)
            };

            return new PropertyManager<ExportSpecificationAttribute>(attributeProperties, _catalogSettings);
        }

        private byte[] ExportProductsToXlsxWithAttributes(PropertyByName<Product>[] properties, IEnumerable<Product> itemsToExport)
        {
            var productAttributeManager = GetProductAttributeManager();
            var specificationAttributeManager = GetSpecificationAttributeManager();

            using (var stream = new MemoryStream())
            {
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
                        manager.WriteToXlsx(worksheet, row++, fWorksheet: fpWorksheet);

                        if (_catalogSettings.ExportImportProductAttributes)
                        {
                            row = ExportProductAttributes(item, productAttributeManager, worksheet, row, fbaWorksheet);
                        }

                        if (_catalogSettings.ExportImportProductSpecificationAttributes)
                        {
                            row = ExportSpecificationAttributes(item, specificationAttributeManager, worksheet, row, fsaWorksheet);
                        }
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        private int ExportProductAttributes(Product item, PropertyManager<ExportProductAttribute> attributeManager, ExcelWorksheet worksheet, int row, ExcelWorksheet faWorksheet)
        {
            var attributes = item.ProductAttributeMappings.SelectMany(pam => pam.ProductAttributeValues.Select(
                pav => new ExportProductAttribute
                {
                    AttributeId = pam.ProductAttribute.Id,
                    AttributeName = pam.ProductAttribute.Name,
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
                })).ToList();

            attributes.AddRange(item.ProductAttributeMappings.Where(pam => !pam.ProductAttributeValues.Any()).Select(
                pam => new ExportProductAttribute
                {
                    AttributeId = pam.ProductAttribute.Id,
                    AttributeName = pam.ProductAttribute.Name,
                    AttributeTextPrompt = pam.TextPrompt,
                    AttributeIsRequired = pam.IsRequired,
                    AttributeControlTypeId = pam.AttributeControlTypeId
                }));

            if (!attributes.Any())
                return row;

            attributeManager.WriteCaption(worksheet, row, ExportProductAttribute.ProducAttributeCellOffset);
            worksheet.Row(row).OutlineLevel = 1;
            worksheet.Row(row).Collapsed = true;

            foreach (var exportProducAttribute in attributes)
            {
                row++;
                attributeManager.CurrentObject = exportProducAttribute;
                attributeManager.WriteToXlsx(worksheet, row, ExportProductAttribute.ProducAttributeCellOffset, faWorksheet);
                worksheet.Row(row).OutlineLevel = 1;
                worksheet.Row(row).Collapsed = true;
            }

            return row + 1;
        }

        private int ExportSpecificationAttributes(Product item, PropertyManager<ExportSpecificationAttribute> attributeManager, ExcelWorksheet worksheet, int row, ExcelWorksheet faWorksheet)
        {
            var attributes = item.ProductSpecificationAttributes.Select(
                psa => new ExportSpecificationAttribute
                {
                    AttributeTypeId = psa.AttributeTypeId,
                    CustomValue = psa.CustomValue,
                    AllowFiltering = psa.AllowFiltering,
                    ShowOnProductPage = psa.ShowOnProductPage,
                    DisplayOrder = psa.DisplayOrder,
                    SpecificationAttributeOptionId = psa.SpecificationAttributeOptionId,
                    SpecificationAttributeId = psa.SpecificationAttributeOption.SpecificationAttribute.Id
                }).ToList();

            if (!attributes.Any())
                return row;

            attributeManager.WriteCaption(worksheet, row, ExportProductAttribute.ProducAttributeCellOffset);
            worksheet.Row(row).OutlineLevel = 1;
            worksheet.Row(row).Collapsed = true;

            foreach (var exportProducAttribute in attributes)
            {
                row++;
                attributeManager.CurrentObject = exportProducAttribute;
                attributeManager.WriteToXlsx(worksheet, row, ExportProductAttribute.ProducAttributeCellOffset, faWorksheet);
                worksheet.Row(row).OutlineLevel = 1;
                worksheet.Row(row).Collapsed = true;
            }

            return row + 1;
        }

        private byte[] ExportOrderToXlsxWithProducts(PropertyByName<Order>[] properties, IEnumerable<Order> itemsToExport)
        {
            var orderItemProperties = new[]
            {
                new PropertyByName<OrderItem>("Name", oi => oi.Product.Name),
                new PropertyByName<OrderItem>("Sku", oi => oi.Product.Sku),
                new PropertyByName<OrderItem>("PriceExclTax", oi => oi.UnitPriceExclTax),
                new PropertyByName<OrderItem>("PriceInclTax", oi => oi.UnitPriceInclTax),
                new PropertyByName<OrderItem>("Quantity", oi => oi.Quantity),
                new PropertyByName<OrderItem>("DiscountExclTax", oi => oi.DiscountAmountExclTax),
                new PropertyByName<OrderItem>("DiscountInclTax", oi => oi.DiscountAmountInclTax),
                new PropertyByName<OrderItem>("TotalExclTax", oi => oi.PriceExclTax),
                new PropertyByName<OrderItem>("TotalInclTax", oi => oi.PriceInclTax)
            };

            var orderItemsManager = new PropertyManager<OrderItem>(orderItemProperties, _catalogSettings);

            using (var stream = new MemoryStream())
            {
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
                        manager.WriteToXlsx(worksheet, row++);

                        //products
                        var orederItems = order.OrderItems.ToList();

                        //a vendor should have access only to his products
                        if (_workContext.CurrentVendor != null)
                            orederItems = orederItems.Where(p => p.Product.VendorId == _workContext.CurrentVendor.Id).ToList();

                        if (!orederItems.Any())
                            continue;

                        orderItemsManager.WriteCaption(worksheet, row, 2);
                        worksheet.Row(row).OutlineLevel = 1;
                        worksheet.Row(row).Collapsed = true;

                        foreach (var orederItem in orederItems)
                        {
                            row++;
                            orderItemsManager.CurrentObject = orederItem;
                            orderItemsManager.WriteToXlsx(worksheet, row, 2, fpWorksheet);
                            worksheet.Row(row).OutlineLevel = 1;
                            worksheet.Row(row).Collapsed = true;
                        }

                        row++;
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        private string GetCustomCustomerAttributes(Customer customer)
        {
            var selectedCustomerAttributes = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CustomCustomerAttributes);
            return _customerAttributeFormatter.FormatAttributes(selectedCustomerAttributes, ";");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Export manufacturer list to XML
        /// </summary>
        /// <param name="manufacturers">Manufacturers</param>
        /// <returns>Result in XML format</returns>
        public virtual string ExportManufacturersToXml(IList<Manufacturer> manufacturers)
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Manufacturers");
            xmlWriter.WriteAttributeString("Version", NopVersion.CurrentVersion);

            foreach (var manufacturer in manufacturers)
            {
                xmlWriter.WriteStartElement("Manufacturer");

                xmlWriter.WriteString("ManufacturerId", manufacturer.Id);
                xmlWriter.WriteString("Name", manufacturer.Name);
                xmlWriter.WriteString("Description", manufacturer.Description);
                xmlWriter.WriteString("ManufacturerTemplateId", manufacturer.ManufacturerTemplateId);
                xmlWriter.WriteString("MetaKeywords", manufacturer.MetaKeywords, IgnoreExportManufacturerProperty());
                xmlWriter.WriteString("MetaDescription", manufacturer.MetaDescription, IgnoreExportManufacturerProperty());
                xmlWriter.WriteString("MetaTitle", manufacturer.MetaTitle, IgnoreExportManufacturerProperty());
                xmlWriter.WriteString("SEName", _urlRecordService.GetSeName(manufacturer, 0), IgnoreExportManufacturerProperty());
                xmlWriter.WriteString("PictureId", manufacturer.PictureId);
                xmlWriter.WriteString("PageSize", manufacturer.PageSize, IgnoreExportManufacturerProperty());
                xmlWriter.WriteString("AllowCustomersToSelectPageSize", manufacturer.AllowCustomersToSelectPageSize, IgnoreExportManufacturerProperty());
                xmlWriter.WriteString("PageSizeOptions", manufacturer.PageSizeOptions, IgnoreExportManufacturerProperty());
                xmlWriter.WriteString("PriceRanges", manufacturer.PriceRanges, IgnoreExportManufacturerProperty());
                xmlWriter.WriteString("Published", manufacturer.Published, IgnoreExportManufacturerProperty());
                xmlWriter.WriteString("Deleted", manufacturer.Deleted, true);
                xmlWriter.WriteString("DisplayOrder", manufacturer.DisplayOrder);
                xmlWriter.WriteString("CreatedOnUtc", manufacturer.CreatedOnUtc, IgnoreExportManufacturerProperty());
                xmlWriter.WriteString("UpdatedOnUtc", manufacturer.UpdatedOnUtc, IgnoreExportManufacturerProperty());

                xmlWriter.WriteStartElement("Products");
                var productManufacturers = _manufacturerService.GetProductManufacturersByManufacturerId(manufacturer.Id, showHidden: true);
                if (productManufacturers != null)
                {
                    foreach (var productManufacturer in productManufacturers)
                    {
                        var product = productManufacturer.Product;
                        if (product == null || product.Deleted) 
                            continue;

                        xmlWriter.WriteStartElement("ProductManufacturer");
                        xmlWriter.WriteString("ProductManufacturerId", productManufacturer.Id);
                        xmlWriter.WriteString("ProductId", productManufacturer.ProductId);
                        xmlWriter.WriteString("ProductName", product.Name);
                        xmlWriter.WriteString("IsFeaturedProduct", productManufacturer.IsFeaturedProduct);
                        xmlWriter.WriteString("DisplayOrder", productManufacturer.DisplayOrder);
                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export manufacturers to XLSX
        /// </summary>
        /// <param name="manufacturers">Manufactures</param>
        public virtual byte[] ExportManufacturersToXlsx(IEnumerable<Manufacturer> manufacturers)
        {
            //property manager 
            var manager = new PropertyManager<Manufacturer>(new[]
            {
                new PropertyByName<Manufacturer>("Id", p => p.Id),
                new PropertyByName<Manufacturer>("Name", p => p.Name),
                new PropertyByName<Manufacturer>("Description", p => p.Description),
                new PropertyByName<Manufacturer>("ManufacturerTemplateId", p => p.ManufacturerTemplateId),
                new PropertyByName<Manufacturer>("MetaKeywords", p => p.MetaKeywords, IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("MetaDescription", p => p.MetaDescription, IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("MetaTitle", p => p.MetaTitle, IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("SeName", p => _urlRecordService.GetSeName(p, 0), IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("Picture", p => GetPictures(p.PictureId)),
                new PropertyByName<Manufacturer>("PageSize", p => p.PageSize, IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("AllowCustomersToSelectPageSize", p => p.AllowCustomersToSelectPageSize, IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("PageSizeOptions", p => p.PageSizeOptions, IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("PriceRanges", p => p.PriceRanges, IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("Published", p => p.Published, IgnoreExportManufacturerProperty()),
                new PropertyByName<Manufacturer>("DisplayOrder", p => p.DisplayOrder)
            }, _catalogSettings);

            return manager.ExportToXlsx(manufacturers);
        }

        /// <summary>
        /// Export category list to XML
        /// </summary>
        /// <returns>Result in XML format</returns>
        public virtual string ExportCategoriesToXml()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Categories");
            xmlWriter.WriteAttributeString("Version", NopVersion.CurrentVersion);
            WriteCategories(xmlWriter, 0);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export categories to XLSX
        /// </summary>
        /// <param name="categories">Categories</param>
        public virtual byte[] ExportCategoriesToXlsx(IList<Category> categories)
        {
            var parentCatagories = new List<Category>();
            if (_catalogSettings.ExportImportCategoriesUsingCategoryName)
            {
                //performance optimization, load all parent categories in one SQL request
                parentCatagories = _categoryService.GetCategoriesByIds(categories.Select(c => c.ParentCategoryId).Where(id => id != 0).ToArray());
            }

            //property manager 
            var manager = new PropertyManager<Category>(new[]
            {
                new PropertyByName<Category>("Id", p => p.Id),
                new PropertyByName<Category>("Name", p => p.Name),
                new PropertyByName<Category>("Description", p => p.Description),
                new PropertyByName<Category>("CategoryTemplateId", p => p.CategoryTemplateId),
                new PropertyByName<Category>("MetaKeywords", p => p.MetaKeywords, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("MetaDescription", p => p.MetaDescription, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("MetaTitle", p => p.MetaTitle, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("SeName", p => _urlRecordService.GetSeName(p, 0), IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("ParentCategoryId", p => p.ParentCategoryId),
                new PropertyByName<Category>("ParentCategoryName", p =>
                {
                    var category = parentCatagories.FirstOrDefault(c => c.Id == p.ParentCategoryId);
                    return category != null ? _categoryService.GetFormattedBreadCrumb(category) : null;
                }, !_catalogSettings.ExportImportCategoriesUsingCategoryName),
                new PropertyByName<Category>("Picture", p => GetPictures(p.PictureId)),
                new PropertyByName<Category>("PageSize", p => p.PageSize, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("AllowCustomersToSelectPageSize", p => p.AllowCustomersToSelectPageSize, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("PageSizeOptions", p => p.PageSizeOptions, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("PriceRanges", p => p.PriceRanges, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("ShowOnHomepage", p => p.ShowOnHomepage, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("IncludeInTopMenu", p => p.IncludeInTopMenu, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("Published", p => p.Published, IgnoreExportCategoryProperty()),
                new PropertyByName<Category>("DisplayOrder", p => p.DisplayOrder)
            }, _catalogSettings);

            return manager.ExportToXlsx(categories);
        }

        /// <summary>
        /// Export product list to XML
        /// </summary>
        /// <param name="products">Products</param>
        /// <returns>Result in XML format</returns>
        public virtual string ExportProductsToXml(IList<Product> products)
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Products");
            xmlWriter.WriteAttributeString("Version", NopVersion.CurrentVersion);

            foreach (var product in products)
            {
                xmlWriter.WriteStartElement("Product");

                xmlWriter.WriteString("ProductId", product.Id);
                xmlWriter.WriteString("ProductTypeId", product.ProductTypeId, IgnoreExportPoductProperty(p => p.ProductType));
                xmlWriter.WriteString("ParentGroupedProductId", product.ParentGroupedProductId, IgnoreExportPoductProperty(p => p.ProductType));
                xmlWriter.WriteString("VisibleIndividually", product.VisibleIndividually, IgnoreExportPoductProperty(p => p.VisibleIndividually));
                xmlWriter.WriteString("Name", product.Name);
                xmlWriter.WriteString("ShortDescription", product.ShortDescription);
                xmlWriter.WriteString("FullDescription", product.FullDescription);
                xmlWriter.WriteString("AdminComment", product.AdminComment, IgnoreExportPoductProperty(p => p.AdminComment));
                //vendor can't change this field
                xmlWriter.WriteString("VendorId", product.VendorId, IgnoreExportPoductProperty(p => p.Vendor) || _workContext.CurrentVendor != null);
                xmlWriter.WriteString("ProductTemplateId", product.ProductTemplateId, IgnoreExportPoductProperty(p => p.ProductTemplate));
                xmlWriter.WriteString("ShowOnHomepage", product.ShowOnHomepage, IgnoreExportPoductProperty(p => p.ShowOnHomepage));
                xmlWriter.WriteString("MetaKeywords", product.MetaKeywords, IgnoreExportPoductProperty(p => p.Seo));
                xmlWriter.WriteString("MetaDescription", product.MetaDescription, IgnoreExportPoductProperty(p => p.Seo));
                xmlWriter.WriteString("MetaTitle", product.MetaTitle, IgnoreExportPoductProperty(p => p.Seo));
                xmlWriter.WriteString("SEName", _urlRecordService.GetSeName(product, 0), IgnoreExportPoductProperty(p => p.Seo));
                xmlWriter.WriteString("AllowCustomerReviews", product.AllowCustomerReviews, IgnoreExportPoductProperty(p => p.AllowCustomerReviews));
                xmlWriter.WriteString("SKU", product.Sku);
                xmlWriter.WriteString("ManufacturerPartNumber", product.ManufacturerPartNumber, IgnoreExportPoductProperty(p => p.ManufacturerPartNumber));
                xmlWriter.WriteString("Gtin", product.Gtin, IgnoreExportPoductProperty(p => p.GTIN));
                xmlWriter.WriteString("IsGiftCard", product.IsGiftCard, IgnoreExportPoductProperty(p => p.IsGiftCard));
                xmlWriter.WriteString("GiftCardType", product.GiftCardType, IgnoreExportPoductProperty(p => p.IsGiftCard));
                xmlWriter.WriteString("OverriddenGiftCardAmount", product.OverriddenGiftCardAmount, IgnoreExportPoductProperty(p => p.IsGiftCard));
                xmlWriter.WriteString("RequireOtherProducts", product.RequireOtherProducts, IgnoreExportPoductProperty(p => p.RequireOtherProductsAddedToTheCart));
                xmlWriter.WriteString("RequiredProductIds", product.RequiredProductIds, IgnoreExportPoductProperty(p => p.RequireOtherProductsAddedToTheCart));
                xmlWriter.WriteString("AutomaticallyAddRequiredProducts", product.AutomaticallyAddRequiredProducts, IgnoreExportPoductProperty(p => p.RequireOtherProductsAddedToTheCart));
                xmlWriter.WriteString("IsDownload", product.IsDownload, IgnoreExportPoductProperty(p => p.DownloadableProduct));
                xmlWriter.WriteString("DownloadId", product.DownloadId, IgnoreExportPoductProperty(p => p.DownloadableProduct));
                xmlWriter.WriteString("UnlimitedDownloads", product.UnlimitedDownloads, IgnoreExportPoductProperty(p => p.DownloadableProduct));
                xmlWriter.WriteString("MaxNumberOfDownloads", product.MaxNumberOfDownloads, IgnoreExportPoductProperty(p => p.DownloadableProduct));
                xmlWriter.WriteString("DownloadExpirationDays", product.DownloadExpirationDays, IgnoreExportPoductProperty(p => p.DownloadableProduct));
                xmlWriter.WriteString("DownloadActivationType", product.DownloadActivationType, IgnoreExportPoductProperty(p => p.DownloadableProduct));
                xmlWriter.WriteString("HasSampleDownload", product.HasSampleDownload, IgnoreExportPoductProperty(p => p.DownloadableProduct));
                xmlWriter.WriteString("SampleDownloadId", product.SampleDownloadId, IgnoreExportPoductProperty(p => p.DownloadableProduct));
                xmlWriter.WriteString("HasUserAgreement", product.HasUserAgreement, IgnoreExportPoductProperty(p => p.DownloadableProduct));
                xmlWriter.WriteString("UserAgreementText", product.UserAgreementText, IgnoreExportPoductProperty(p => p.DownloadableProduct));
                xmlWriter.WriteString("IsRecurring", product.IsRecurring, IgnoreExportPoductProperty(p => p.RecurringProduct));
                xmlWriter.WriteString("RecurringCycleLength", product.RecurringCycleLength, IgnoreExportPoductProperty(p => p.RecurringProduct));
                xmlWriter.WriteString("RecurringCyclePeriodId", product.RecurringCyclePeriodId, IgnoreExportPoductProperty(p => p.RecurringProduct));
                xmlWriter.WriteString("RecurringTotalCycles", product.RecurringTotalCycles, IgnoreExportPoductProperty(p => p.RecurringProduct));
                xmlWriter.WriteString("IsRental", product.IsRental, IgnoreExportPoductProperty(p => p.IsRental));
                xmlWriter.WriteString("RentalPriceLength", product.RentalPriceLength, IgnoreExportPoductProperty(p => p.IsRental));
                xmlWriter.WriteString("RentalPricePeriodId", product.RentalPricePeriodId, IgnoreExportPoductProperty(p => p.IsRental));
                xmlWriter.WriteString("IsShipEnabled", product.IsShipEnabled);
                xmlWriter.WriteString("IsFreeShipping", product.IsFreeShipping, IgnoreExportPoductProperty(p => p.FreeShipping));
                xmlWriter.WriteString("ShipSeparately", product.ShipSeparately, IgnoreExportPoductProperty(p => p.ShipSeparately));
                xmlWriter.WriteString("AdditionalShippingCharge", product.AdditionalShippingCharge, IgnoreExportPoductProperty(p => p.AdditionalShippingCharge));
                xmlWriter.WriteString("DeliveryDateId", product.DeliveryDateId, IgnoreExportPoductProperty(p => p.DeliveryDate));
                xmlWriter.WriteString("IsTaxExempt", product.IsTaxExempt);
                xmlWriter.WriteString("TaxCategoryId", product.TaxCategoryId);
                xmlWriter.WriteString("IsTelecommunicationsOrBroadcastingOrElectronicServices", product.IsTelecommunicationsOrBroadcastingOrElectronicServices, IgnoreExportPoductProperty(p => p.TelecommunicationsBroadcastingElectronicServices));
                xmlWriter.WriteString("ManageInventoryMethodId", product.ManageInventoryMethodId);
                xmlWriter.WriteString("ProductAvailabilityRangeId", product.ProductAvailabilityRangeId, IgnoreExportPoductProperty(p => p.ProductAvailabilityRange));
                xmlWriter.WriteString("UseMultipleWarehouses", product.UseMultipleWarehouses, IgnoreExportPoductProperty(p => p.UseMultipleWarehouses));
                xmlWriter.WriteString("WarehouseId", product.WarehouseId, IgnoreExportPoductProperty(p => p.Warehouse));
                xmlWriter.WriteString("StockQuantity", product.StockQuantity);
                xmlWriter.WriteString("DisplayStockAvailability", product.DisplayStockAvailability, IgnoreExportPoductProperty(p => p.DisplayStockAvailability));
                xmlWriter.WriteString("DisplayStockQuantity", product.DisplayStockQuantity, IgnoreExportPoductProperty(p => p.DisplayStockQuantity));
                xmlWriter.WriteString("MinStockQuantity", product.MinStockQuantity, IgnoreExportPoductProperty(p => p.MinimumStockQuantity));
                xmlWriter.WriteString("LowStockActivityId", product.LowStockActivityId, IgnoreExportPoductProperty(p => p.LowStockActivity));
                xmlWriter.WriteString("NotifyAdminForQuantityBelow", product.NotifyAdminForQuantityBelow, IgnoreExportPoductProperty(p => p.NotifyAdminForQuantityBelow));
                xmlWriter.WriteString("BackorderModeId", product.BackorderModeId, IgnoreExportPoductProperty(p => p.Backorders));
                xmlWriter.WriteString("AllowBackInStockSubscriptions", product.AllowBackInStockSubscriptions, IgnoreExportPoductProperty(p => p.AllowBackInStockSubscriptions));
                xmlWriter.WriteString("OrderMinimumQuantity", product.OrderMinimumQuantity, IgnoreExportPoductProperty(p => p.MinimumCartQuantity));
                xmlWriter.WriteString("OrderMaximumQuantity", product.OrderMaximumQuantity, IgnoreExportPoductProperty(p => p.MaximumCartQuantity));
                xmlWriter.WriteString("AllowedQuantities", product.AllowedQuantities, IgnoreExportPoductProperty(p => p.AllowedQuantities));
                xmlWriter.WriteString("AllowAddingOnlyExistingAttributeCombinations", product.AllowAddingOnlyExistingAttributeCombinations, IgnoreExportPoductProperty(p => p.AllowAddingOnlyExistingAttributeCombinations));
                xmlWriter.WriteString("NotReturnable", product.NotReturnable, IgnoreExportPoductProperty(p => p.NotReturnable));
                xmlWriter.WriteString("DisableBuyButton", product.DisableBuyButton, IgnoreExportPoductProperty(p => p.DisableBuyButton));
                xmlWriter.WriteString("DisableWishlistButton", product.DisableWishlistButton, IgnoreExportPoductProperty(p => p.DisableWishlistButton));
                xmlWriter.WriteString("AvailableForPreOrder", product.AvailableForPreOrder, IgnoreExportPoductProperty(p => p.AvailableForPreOrder));
                xmlWriter.WriteString("PreOrderAvailabilityStartDateTimeUtc", product.PreOrderAvailabilityStartDateTimeUtc, IgnoreExportPoductProperty(p => p.AvailableForPreOrder));
                xmlWriter.WriteString("CallForPrice", product.CallForPrice, IgnoreExportPoductProperty(p => p.CallForPrice));
                xmlWriter.WriteString("Price", product.Price);
                xmlWriter.WriteString("OldPrice", product.OldPrice, IgnoreExportPoductProperty(p => p.OldPrice));
                xmlWriter.WriteString("ProductCost", product.ProductCost, IgnoreExportPoductProperty(p => p.ProductCost));
                xmlWriter.WriteString("CustomerEntersPrice", product.CustomerEntersPrice, IgnoreExportPoductProperty(p => p.CustomerEntersPrice));
                xmlWriter.WriteString("MinimumCustomerEnteredPrice", product.MinimumCustomerEnteredPrice, IgnoreExportPoductProperty(p => p.CustomerEntersPrice));
                xmlWriter.WriteString("MaximumCustomerEnteredPrice", product.MaximumCustomerEnteredPrice, IgnoreExportPoductProperty(p => p.CustomerEntersPrice));
                xmlWriter.WriteString("BasepriceEnabled", product.BasepriceEnabled, IgnoreExportPoductProperty(p => p.PAngV));
                xmlWriter.WriteString("BasepriceAmount", product.BasepriceAmount, IgnoreExportPoductProperty(p => p.PAngV));
                xmlWriter.WriteString("BasepriceUnitId", product.BasepriceUnitId, IgnoreExportPoductProperty(p => p.PAngV));
                xmlWriter.WriteString("BasepriceBaseAmount", product.BasepriceBaseAmount, IgnoreExportPoductProperty(p => p.PAngV));
                xmlWriter.WriteString("BasepriceBaseUnitId", product.BasepriceBaseUnitId, IgnoreExportPoductProperty(p => p.PAngV));
                xmlWriter.WriteString("MarkAsNew", product.MarkAsNew, IgnoreExportPoductProperty(p => p.MarkAsNew));
                xmlWriter.WriteString("MarkAsNewStartDateTimeUtc", product.MarkAsNewStartDateTimeUtc, IgnoreExportPoductProperty(p => p.MarkAsNewStartDate));
                xmlWriter.WriteString("MarkAsNewEndDateTimeUtc", product.MarkAsNewEndDateTimeUtc, IgnoreExportPoductProperty(p => p.MarkAsNewEndDate));
                xmlWriter.WriteString("Weight", product.Weight, IgnoreExportPoductProperty(p => p.Weight));
                xmlWriter.WriteString("Length", product.Length, IgnoreExportPoductProperty(p => p.Dimensions));
                xmlWriter.WriteString("Width", product.Width, IgnoreExportPoductProperty(p => p.Dimensions));
                xmlWriter.WriteString("Height", product.Height, IgnoreExportPoductProperty(p => p.Dimensions));
                xmlWriter.WriteString("Published", product.Published, IgnoreExportPoductProperty(p => p.Published));
                xmlWriter.WriteString("CreatedOnUtc", product.CreatedOnUtc);
                xmlWriter.WriteString("UpdatedOnUtc", product.UpdatedOnUtc);

                if (!IgnoreExportPoductProperty(p => p.Discounts))
                {
                    xmlWriter.WriteStartElement("ProductDiscounts");
                    var discounts = product.AppliedDiscounts;
                    foreach (var discount in discounts)
                    {
                        xmlWriter.WriteStartElement("Discount");
                        xmlWriter.WriteString("DiscountId", discount.Id);
                        xmlWriter.WriteString("Name", discount.Name);
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                }

                if (!IgnoreExportPoductProperty(p => p.TierPrices))
                {
                    xmlWriter.WriteStartElement("TierPrices");
                    var tierPrices = product.TierPrices;
                    foreach (var tierPrice in tierPrices)
                    {
                        xmlWriter.WriteStartElement("TierPrice");
                        xmlWriter.WriteString("TierPriceId", tierPrice.Id);
                        xmlWriter.WriteString("StoreId", tierPrice.StoreId);
                        xmlWriter.WriteString("CustomerRoleId", tierPrice.CustomerRoleId, defaulValue: "0");
                        xmlWriter.WriteString("Quantity", tierPrice.Quantity);
                        xmlWriter.WriteString("Price", tierPrice.Price);
                        xmlWriter.WriteString("StartDateTimeUtc", tierPrice.StartDateTimeUtc);
                        xmlWriter.WriteString("EndDateTimeUtc", tierPrice.EndDateTimeUtc);
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                }

                if (!IgnoreExportPoductProperty(p => p.ProductAttributes))
                {
                    xmlWriter.WriteStartElement("ProductAttributes");
                    var productAttributMappings =
                        _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                    foreach (var productAttributeMapping in productAttributMappings)
                    {
                        xmlWriter.WriteStartElement("ProductAttributeMapping");
                        xmlWriter.WriteString("ProductAttributeMappingId", productAttributeMapping.Id);
                        xmlWriter.WriteString("ProductAttributeId", productAttributeMapping.ProductAttributeId);
                        xmlWriter.WriteString("ProductAttributeName", productAttributeMapping.ProductAttribute.Name);
                        xmlWriter.WriteString("TextPrompt", productAttributeMapping.TextPrompt);
                        xmlWriter.WriteString("IsRequired", productAttributeMapping.IsRequired);
                        xmlWriter.WriteString("AttributeControlTypeId", productAttributeMapping.AttributeControlTypeId);
                        xmlWriter.WriteString("DisplayOrder", productAttributeMapping.DisplayOrder);
                        //validation rules
                        if (productAttributeMapping.ValidationRulesAllowed())
                        {
                            if (productAttributeMapping.ValidationMinLength.HasValue)
                            {
                                xmlWriter.WriteString("ValidationMinLength",
                                    productAttributeMapping.ValidationMinLength.Value);
                            }

                            if (productAttributeMapping.ValidationMaxLength.HasValue)
                            {
                                xmlWriter.WriteString("ValidationMaxLength",
                                    productAttributeMapping.ValidationMaxLength.Value);
                            }

                            if (string.IsNullOrEmpty(productAttributeMapping.ValidationFileAllowedExtensions))
                            {
                                xmlWriter.WriteString("ValidationFileAllowedExtensions",
                                    productAttributeMapping.ValidationFileAllowedExtensions);
                            }

                            if (productAttributeMapping.ValidationFileMaximumSize.HasValue)
                            {
                                xmlWriter.WriteString("ValidationFileMaximumSize",
                                    productAttributeMapping.ValidationFileMaximumSize.Value);
                            }

                            xmlWriter.WriteString("DefaultValue", productAttributeMapping.DefaultValue);
                        }
                        //conditions
                        xmlWriter.WriteElementString("ConditionAttributeXml",
                            productAttributeMapping.ConditionAttributeXml);

                        xmlWriter.WriteStartElement("ProductAttributeValues");
                        var productAttributeValues = productAttributeMapping.ProductAttributeValues;
                        foreach (var productAttributeValue in productAttributeValues)
                        {
                            xmlWriter.WriteStartElement("ProductAttributeValue");
                            xmlWriter.WriteString("ProductAttributeValueId", productAttributeValue.Id);
                            xmlWriter.WriteString("Name", productAttributeValue.Name);
                            xmlWriter.WriteString("AttributeValueTypeId", productAttributeValue.AttributeValueTypeId);
                            xmlWriter.WriteString("AssociatedProductId", productAttributeValue.AssociatedProductId);
                            xmlWriter.WriteString("ColorSquaresRgb", productAttributeValue.ColorSquaresRgb);
                            xmlWriter.WriteString("ImageSquaresPictureId", productAttributeValue.ImageSquaresPictureId);
                            xmlWriter.WriteString("PriceAdjustment", productAttributeValue.PriceAdjustment);
                            xmlWriter.WriteString("PriceAdjustmentUsePercentage", productAttributeValue.PriceAdjustmentUsePercentage);
                            xmlWriter.WriteString("WeightAdjustment", productAttributeValue.WeightAdjustment);
                            xmlWriter.WriteString("Cost", productAttributeValue.Cost);
                            xmlWriter.WriteString("CustomerEntersQty", productAttributeValue.CustomerEntersQty);
                            xmlWriter.WriteString("Quantity", productAttributeValue.Quantity);
                            xmlWriter.WriteString("IsPreSelected", productAttributeValue.IsPreSelected);
                            xmlWriter.WriteString("DisplayOrder", productAttributeValue.DisplayOrder);
                            xmlWriter.WriteString("PictureId", productAttributeValue.PictureId);
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteStartElement("ProductPictures");
                var productPictures = product.ProductPictures;
                foreach (var productPicture in productPictures)
                {
                    xmlWriter.WriteStartElement("ProductPicture");
                    xmlWriter.WriteString("ProductPictureId", productPicture.Id);
                    xmlWriter.WriteString("PictureId", productPicture.PictureId);
                    xmlWriter.WriteString("DisplayOrder", productPicture.DisplayOrder);
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ProductCategories");
                var productCategories = _categoryService.GetProductCategoriesByProductId(product.Id);
                if (productCategories != null)
                {
                    foreach (var productCategory in productCategories)
                    {
                        xmlWriter.WriteStartElement("ProductCategory");
                        xmlWriter.WriteString("ProductCategoryId", productCategory.Id);
                        xmlWriter.WriteString("CategoryId", productCategory.CategoryId);
                        xmlWriter.WriteString("IsFeaturedProduct", productCategory.IsFeaturedProduct);
                        xmlWriter.WriteString("DisplayOrder", productCategory.DisplayOrder);
                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();

                if (!IgnoreExportPoductProperty(p => p.Manufacturers))
                {
                    xmlWriter.WriteStartElement("ProductManufacturers");
                    var productManufacturers = _manufacturerService.GetProductManufacturersByProductId(product.Id);
                    if (productManufacturers != null)
                    {
                        foreach (var productManufacturer in productManufacturers)
                        {
                            xmlWriter.WriteStartElement("ProductManufacturer");
                            xmlWriter.WriteString("ProductManufacturerId", productManufacturer.Id);
                            xmlWriter.WriteString("ManufacturerId", productManufacturer.ManufacturerId);
                            xmlWriter.WriteString("IsFeaturedProduct", productManufacturer.IsFeaturedProduct);
                            xmlWriter.WriteString("DisplayOrder", productManufacturer.DisplayOrder);
                            xmlWriter.WriteEndElement();
                        }
                    }

                    xmlWriter.WriteEndElement();
                }

                if (!IgnoreExportPoductProperty(p => p.SpecificationAttributes))
                {
                    xmlWriter.WriteStartElement("ProductSpecificationAttributes");
                    var productSpecificationAttributes = product.ProductSpecificationAttributes;
                    foreach (var productSpecificationAttribute in productSpecificationAttributes)
                    {
                        xmlWriter.WriteStartElement("ProductSpecificationAttribute");
                        xmlWriter.WriteString("ProductSpecificationAttributeId", productSpecificationAttribute.Id);
                        xmlWriter.WriteString("SpecificationAttributeOptionId", productSpecificationAttribute.SpecificationAttributeOptionId);
                        xmlWriter.WriteString("CustomValue", productSpecificationAttribute.CustomValue);
                        xmlWriter.WriteString("AllowFiltering", productSpecificationAttribute.AllowFiltering);
                        xmlWriter.WriteString("ShowOnProductPage", productSpecificationAttribute.ShowOnProductPage);
                        xmlWriter.WriteString("DisplayOrder", productSpecificationAttribute.DisplayOrder);
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                }

                if (!IgnoreExportPoductProperty(p => p.ProductTags))
                {
                    xmlWriter.WriteStartElement("ProductTags");
                    var productTags = _productTagService.GetAllProductTagsByProductId(product.Id);
                    foreach (var productTag in productTags)
                    {
                        xmlWriter.WriteStartElement("ProductTag");
                        xmlWriter.WriteString("Id", productTag.Id);
                        xmlWriter.WriteString("Name", productTag.Name);
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export products to XLSX
        /// </summary>
        /// <param name="products">Products</param>
        public virtual byte[] ExportProductsToXlsx(IEnumerable<Product> products)
        {
            var properties = new[]
            {
                new PropertyByName<Product>("ProductId", p => p.Id),
                new PropertyByName<Product>("ProductType", p => p.ProductTypeId, IgnoreExportPoductProperty(p => p.ProductType))
                {
                    DropDownElements = ProductType.SimpleProduct.ToSelectList(useLocalization: false)
                },
                new PropertyByName<Product>("ParentGroupedProductId", p => p.ParentGroupedProductId, IgnoreExportPoductProperty(p => p.ProductType)),
                new PropertyByName<Product>("VisibleIndividually", p => p.VisibleIndividually, IgnoreExportPoductProperty(p => p.VisibleIndividually)),
                new PropertyByName<Product>("Name", p => p.Name),
                new PropertyByName<Product>("ShortDescription", p => p.ShortDescription),
                new PropertyByName<Product>("FullDescription", p => p.FullDescription),
                //vendor can't change this field
                new PropertyByName<Product>("Vendor", p => p.VendorId, IgnoreExportPoductProperty(p => p.Vendor) || _workContext.CurrentVendor != null)
                {
                    DropDownElements = _vendorService.GetAllVendors(showHidden: true).Select(v => v as BaseEntity).ToSelectList(p => (p as Vendor)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("ProductTemplate", p => p.ProductTemplateId, IgnoreExportPoductProperty(p => p.ProductTemplate))
                {
                    DropDownElements = _productTemplateService.GetAllProductTemplates().Select(pt => pt as BaseEntity).ToSelectList(p => (p as ProductTemplate)?.Name ?? string.Empty)
                },
                //vendor can't change this field
                new PropertyByName<Product>("ShowOnHomepage", p => p.ShowOnHomepage, IgnoreExportPoductProperty(p => p.ShowOnHomepage) || _workContext.CurrentVendor != null),
                new PropertyByName<Product>("MetaKeywords", p => p.MetaKeywords, IgnoreExportPoductProperty(p => p.Seo)),
                new PropertyByName<Product>("MetaDescription", p => p.MetaDescription, IgnoreExportPoductProperty(p => p.Seo)),
                new PropertyByName<Product>("MetaTitle", p => p.MetaTitle, IgnoreExportPoductProperty(p => p.Seo)),
                new PropertyByName<Product>("SeName", p => _urlRecordService.GetSeName(p, 0), IgnoreExportPoductProperty(p => p.Seo)),
                new PropertyByName<Product>("AllowCustomerReviews", p => p.AllowCustomerReviews, IgnoreExportPoductProperty(p => p.AllowCustomerReviews)),
                new PropertyByName<Product>("Published", p => p.Published, IgnoreExportPoductProperty(p => p.Published)),
                new PropertyByName<Product>("SKU", p => p.Sku),
                new PropertyByName<Product>("ManufacturerPartNumber", p => p.ManufacturerPartNumber, IgnoreExportPoductProperty(p => p.ManufacturerPartNumber)),
                new PropertyByName<Product>("Gtin", p => p.Gtin, IgnoreExportPoductProperty(p => p.GTIN)),
                new PropertyByName<Product>("IsGiftCard", p => p.IsGiftCard, IgnoreExportPoductProperty(p => p.IsGiftCard)),
                new PropertyByName<Product>("GiftCardType", p => p.GiftCardTypeId, IgnoreExportPoductProperty(p => p.IsGiftCard))
                {
                    DropDownElements = GiftCardType.Virtual.ToSelectList(useLocalization: false)
                },
                new PropertyByName<Product>("OverriddenGiftCardAmount", p => p.OverriddenGiftCardAmount, IgnoreExportPoductProperty(p => p.IsGiftCard)),
                new PropertyByName<Product>("RequireOtherProducts", p => p.RequireOtherProducts, IgnoreExportPoductProperty(p => p.RequireOtherProductsAddedToTheCart)),
                new PropertyByName<Product>("RequiredProductIds", p => p.RequiredProductIds, IgnoreExportPoductProperty(p => p.RequireOtherProductsAddedToTheCart)),
                new PropertyByName<Product>("AutomaticallyAddRequiredProducts", p => p.AutomaticallyAddRequiredProducts, IgnoreExportPoductProperty(p => p.RequireOtherProductsAddedToTheCart)),
                new PropertyByName<Product>("IsDownload", p => p.IsDownload, IgnoreExportPoductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("DownloadId", p => p.DownloadId, IgnoreExportPoductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("UnlimitedDownloads", p => p.UnlimitedDownloads, IgnoreExportPoductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("MaxNumberOfDownloads", p => p.MaxNumberOfDownloads, IgnoreExportPoductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("DownloadActivationType", p => p.DownloadActivationTypeId, IgnoreExportPoductProperty(p => p.DownloadableProduct))
                {
                    DropDownElements = DownloadActivationType.Manually.ToSelectList(useLocalization: false)
                },
                new PropertyByName<Product>("HasSampleDownload", p => p.HasSampleDownload, IgnoreExportPoductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("SampleDownloadId", p => p.SampleDownloadId, IgnoreExportPoductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("HasUserAgreement", p => p.HasUserAgreement, IgnoreExportPoductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("UserAgreementText", p => p.UserAgreementText, IgnoreExportPoductProperty(p => p.DownloadableProduct)),
                new PropertyByName<Product>("IsRecurring", p => p.IsRecurring, IgnoreExportPoductProperty(p => p.RecurringProduct)),
                new PropertyByName<Product>("RecurringCycleLength", p => p.RecurringCycleLength, IgnoreExportPoductProperty(p => p.RecurringProduct)),
                new PropertyByName<Product>("RecurringCyclePeriod", p => p.RecurringCyclePeriodId, IgnoreExportPoductProperty(p => p.RecurringProduct))
                {
                    DropDownElements = RecurringProductCyclePeriod.Days.ToSelectList(useLocalization: false),
                    AllowBlank = true
                },
                new PropertyByName<Product>("RecurringTotalCycles", p => p.RecurringTotalCycles, IgnoreExportPoductProperty(p => p.RecurringProduct)),
                new PropertyByName<Product>("IsRental", p => p.IsRental, IgnoreExportPoductProperty(p => p.IsRental)),
                new PropertyByName<Product>("RentalPriceLength", p => p.RentalPriceLength, IgnoreExportPoductProperty(p => p.IsRental)),
                new PropertyByName<Product>("RentalPricePeriod", p => p.RentalPricePeriodId, IgnoreExportPoductProperty(p => p.IsRental))
                {
                    DropDownElements = RentalPricePeriod.Days.ToSelectList(useLocalization: false),
                    AllowBlank = true
                },
                new PropertyByName<Product>("IsShipEnabled", p => p.IsShipEnabled),
                new PropertyByName<Product>("IsFreeShipping", p => p.IsFreeShipping, IgnoreExportPoductProperty(p => p.FreeShipping)),
                new PropertyByName<Product>("ShipSeparately", p => p.ShipSeparately, IgnoreExportPoductProperty(p => p.ShipSeparately)),
                new PropertyByName<Product>("AdditionalShippingCharge", p => p.AdditionalShippingCharge, IgnoreExportPoductProperty(p => p.AdditionalShippingCharge)),
                new PropertyByName<Product>("DeliveryDate", p => p.DeliveryDateId, IgnoreExportPoductProperty(p => p.DeliveryDate))
                {
                    DropDownElements = _dateRangeService.GetAllDeliveryDates().Select(dd => dd as BaseEntity).ToSelectList(p => (p as DeliveryDate)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("IsTaxExempt", p => p.IsTaxExempt),
                new PropertyByName<Product>("TaxCategory", p => p.TaxCategoryId)
                {
                    DropDownElements = _taxCategoryService.GetAllTaxCategories().Select(tc => tc as BaseEntity).ToSelectList(p => (p as TaxCategory)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("IsTelecommunicationsOrBroadcastingOrElectronicServices", p => p.IsTelecommunicationsOrBroadcastingOrElectronicServices, IgnoreExportPoductProperty(p => p.TelecommunicationsBroadcastingElectronicServices)),
                new PropertyByName<Product>("ManageInventoryMethod", p => p.ManageInventoryMethodId)
                {
                    DropDownElements = ManageInventoryMethod.DontManageStock.ToSelectList(useLocalization: false)
                },
                new PropertyByName<Product>("ProductAvailabilityRange", p => p.ProductAvailabilityRangeId, IgnoreExportPoductProperty(p => p.ProductAvailabilityRange))
                {
                    DropDownElements = _dateRangeService.GetAllProductAvailabilityRanges().Select(range => range as BaseEntity).ToSelectList(p => (p as ProductAvailabilityRange)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("UseMultipleWarehouses", p => p.UseMultipleWarehouses, IgnoreExportPoductProperty(p => p.UseMultipleWarehouses)),
                new PropertyByName<Product>("WarehouseId", p => p.WarehouseId, IgnoreExportPoductProperty(p => p.Warehouse)),
                new PropertyByName<Product>("StockQuantity", p => p.StockQuantity),
                new PropertyByName<Product>("DisplayStockAvailability", p => p.DisplayStockAvailability, IgnoreExportPoductProperty(p => p.DisplayStockAvailability)),
                new PropertyByName<Product>("DisplayStockQuantity", p => p.DisplayStockQuantity, IgnoreExportPoductProperty(p => p.DisplayStockQuantity)),
                new PropertyByName<Product>("MinStockQuantity", p => p.MinStockQuantity, IgnoreExportPoductProperty(p => p.MinimumStockQuantity)),
                new PropertyByName<Product>("LowStockActivity", p => p.LowStockActivityId, IgnoreExportPoductProperty(p => p.LowStockActivity))
                {
                    DropDownElements = LowStockActivity.Nothing.ToSelectList(useLocalization: false)
                },
                new PropertyByName<Product>("NotifyAdminForQuantityBelow", p => p.NotifyAdminForQuantityBelow, IgnoreExportPoductProperty(p => p.NotifyAdminForQuantityBelow)),
                new PropertyByName<Product>("BackorderMode", p => p.BackorderModeId, IgnoreExportPoductProperty(p => p.Backorders))
                {
                    DropDownElements = BackorderMode.NoBackorders.ToSelectList(useLocalization: false)
                },
                new PropertyByName<Product>("AllowBackInStockSubscriptions", p => p.AllowBackInStockSubscriptions, IgnoreExportPoductProperty(p => p.AllowBackInStockSubscriptions)),
                new PropertyByName<Product>("OrderMinimumQuantity", p => p.OrderMinimumQuantity, IgnoreExportPoductProperty(p => p.MinimumCartQuantity)),
                new PropertyByName<Product>("OrderMaximumQuantity", p => p.OrderMaximumQuantity, IgnoreExportPoductProperty(p => p.MaximumCartQuantity)),
                new PropertyByName<Product>("AllowedQuantities", p => p.AllowedQuantities, IgnoreExportPoductProperty(p => p.AllowedQuantities)),
                new PropertyByName<Product>("AllowAddingOnlyExistingAttributeCombinations", p => p.AllowAddingOnlyExistingAttributeCombinations, IgnoreExportPoductProperty(p => p.AllowAddingOnlyExistingAttributeCombinations)),
                new PropertyByName<Product>("NotReturnable", p => p.NotReturnable, IgnoreExportPoductProperty(p => p.NotReturnable)),
                new PropertyByName<Product>("DisableBuyButton", p => p.DisableBuyButton, IgnoreExportPoductProperty(p => p.DisableBuyButton)),
                new PropertyByName<Product>("DisableWishlistButton", p => p.DisableWishlistButton, IgnoreExportPoductProperty(p => p.DisableWishlistButton)),
                new PropertyByName<Product>("AvailableForPreOrder", p => p.AvailableForPreOrder, IgnoreExportPoductProperty(p => p.AvailableForPreOrder)),
                new PropertyByName<Product>("PreOrderAvailabilityStartDateTimeUtc", p => p.PreOrderAvailabilityStartDateTimeUtc, IgnoreExportPoductProperty(p => p.AvailableForPreOrder)),
                new PropertyByName<Product>("CallForPrice", p => p.CallForPrice, IgnoreExportPoductProperty(p => p.CallForPrice)),
                new PropertyByName<Product>("Price", p => p.Price),
                new PropertyByName<Product>("OldPrice", p => p.OldPrice, IgnoreExportPoductProperty(p => p.OldPrice)),
                new PropertyByName<Product>("ProductCost", p => p.ProductCost, IgnoreExportPoductProperty(p => p.ProductCost)),
                new PropertyByName<Product>("CustomerEntersPrice", p => p.CustomerEntersPrice, IgnoreExportPoductProperty(p => p.CustomerEntersPrice)),
                new PropertyByName<Product>("MinimumCustomerEnteredPrice", p => p.MinimumCustomerEnteredPrice, IgnoreExportPoductProperty(p => p.CustomerEntersPrice)),
                new PropertyByName<Product>("MaximumCustomerEnteredPrice", p => p.MaximumCustomerEnteredPrice, IgnoreExportPoductProperty(p => p.CustomerEntersPrice)),
                new PropertyByName<Product>("BasepriceEnabled", p => p.BasepriceEnabled, IgnoreExportPoductProperty(p => p.PAngV)),
                new PropertyByName<Product>("BasepriceAmount", p => p.BasepriceAmount, IgnoreExportPoductProperty(p => p.PAngV)),
                new PropertyByName<Product>("BasepriceUnit", p => p.BasepriceUnitId, IgnoreExportPoductProperty(p => p.PAngV))
                {
                    DropDownElements = _measureService.GetAllMeasureWeights().Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("BasepriceBaseAmount", p => p.BasepriceBaseAmount, IgnoreExportPoductProperty(p => p.PAngV)),
                new PropertyByName<Product>("BasepriceBaseUnit", p => p.BasepriceBaseUnitId, IgnoreExportPoductProperty(p => p.PAngV))
                {
                    DropDownElements = _measureService.GetAllMeasureWeights().Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty),
                    AllowBlank = true
                },
                new PropertyByName<Product>("MarkAsNew", p => p.MarkAsNew, IgnoreExportPoductProperty(p => p.MarkAsNew)),
                new PropertyByName<Product>("MarkAsNewStartDateTimeUtc", p => p.MarkAsNewStartDateTimeUtc, IgnoreExportPoductProperty(p => p.MarkAsNewStartDate)),
                new PropertyByName<Product>("MarkAsNewEndDateTimeUtc", p => p.MarkAsNewEndDateTimeUtc, IgnoreExportPoductProperty(p => p.MarkAsNewEndDate)),
                new PropertyByName<Product>("Weight", p => p.Weight, IgnoreExportPoductProperty(p => p.Weight)),
                new PropertyByName<Product>("Length", p => p.Length, IgnoreExportPoductProperty(p => p.Dimensions)),
                new PropertyByName<Product>("Width", p => p.Width, IgnoreExportPoductProperty(p => p.Dimensions)),
                new PropertyByName<Product>("Height", p => p.Height, IgnoreExportPoductProperty(p => p.Dimensions)),
                new PropertyByName<Product>("Categories", GetCategories),
                new PropertyByName<Product>("Manufacturers", GetManufacturers, IgnoreExportPoductProperty(p => p.Manufacturers)),
                new PropertyByName<Product>("ProductTags", GetProductTags, IgnoreExportPoductProperty(p => p.ProductTags)),
                new PropertyByName<Product>("IsLimitedToStores", p=>p.LimitedToStores, IgnoreExportLimitedToStore()),
                new PropertyByName<Product>("LimitedToStores", GetLimitedToStores, IgnoreExportLimitedToStore()),
                new PropertyByName<Product>("Picture1", p => GetPictures(p)[0]),
                new PropertyByName<Product>("Picture2", p => GetPictures(p)[1]),
                new PropertyByName<Product>("Picture3", p => GetPictures(p)[2])
            };

            var productList = products.ToList();

            var productAdvancedMode = true;
            try
            {
                productAdvancedMode = _genericAttributeService.GetAttribute<bool>(_workContext.CurrentCustomer, "product-advanced-mode");
            }
            catch (ArgumentNullException)
            {
            }

            if (!_catalogSettings.ExportImportProductAttributes && !_catalogSettings.ExportImportProductSpecificationAttributes)
                return new PropertyManager<Product>(properties, _catalogSettings).ExportToXlsx(productList);

            if (productAdvancedMode || _productEditorSettings.ProductAttributes)
                return ExportProductsToXlsxWithAttributes(properties, productList);

            return new PropertyManager<Product>(properties, _catalogSettings).ExportToXlsx(productList);
        }

        /// <summary>
        /// Export order list to XML
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <returns>Result in XML format</returns>
        public virtual string ExportOrdersToXml(IList<Order> orders)
        {
            //a vendor should have access only to part of order information
            var ignore = _workContext.CurrentVendor != null;

            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Orders");
            xmlWriter.WriteAttributeString("Version", NopVersion.CurrentVersion);

            foreach (var order in orders)
            {
                xmlWriter.WriteStartElement("Order");

                xmlWriter.WriteString("OrderId", order.Id);
                xmlWriter.WriteString("OrderGuid", order.OrderGuid, ignore);
                xmlWriter.WriteString("StoreId", order.StoreId);
                xmlWriter.WriteString("CustomerId", order.CustomerId, ignore);
                xmlWriter.WriteString("OrderStatusId", order.OrderStatusId, ignore);
                xmlWriter.WriteString("PaymentStatusId", order.PaymentStatusId, ignore);
                xmlWriter.WriteString("ShippingStatusId", order.ShippingStatusId, ignore);
                xmlWriter.WriteString("CustomerLanguageId", order.CustomerLanguageId, ignore);
                xmlWriter.WriteString("CustomerTaxDisplayTypeId", order.CustomerTaxDisplayTypeId, ignore);
                xmlWriter.WriteString("CustomerIp", order.CustomerIp, ignore);
                xmlWriter.WriteString("OrderSubtotalInclTax", order.OrderSubtotalInclTax, ignore);
                xmlWriter.WriteString("OrderSubtotalExclTax", order.OrderSubtotalExclTax, ignore);
                xmlWriter.WriteString("OrderSubTotalDiscountInclTax", order.OrderSubTotalDiscountInclTax, ignore);
                xmlWriter.WriteString("OrderSubTotalDiscountExclTax", order.OrderSubTotalDiscountExclTax, ignore);
                xmlWriter.WriteString("OrderShippingInclTax", order.OrderShippingInclTax, ignore);
                xmlWriter.WriteString("OrderShippingExclTax", order.OrderShippingExclTax, ignore);
                xmlWriter.WriteString("PaymentMethodAdditionalFeeInclTax", order.PaymentMethodAdditionalFeeInclTax, ignore);
                xmlWriter.WriteString("PaymentMethodAdditionalFeeExclTax", order.PaymentMethodAdditionalFeeExclTax, ignore);
                xmlWriter.WriteString("TaxRates", order.TaxRates, ignore);
                xmlWriter.WriteString("OrderTax", order.OrderTax, ignore);
                xmlWriter.WriteString("OrderTotal", order.OrderTotal, ignore);
                xmlWriter.WriteString("RefundedAmount", order.RefundedAmount, ignore);
                xmlWriter.WriteString("OrderDiscount", order.OrderDiscount, ignore);
                xmlWriter.WriteString("CurrencyRate", order.CurrencyRate);
                xmlWriter.WriteString("CustomerCurrencyCode", order.CustomerCurrencyCode);
                xmlWriter.WriteString("AffiliateId", order.AffiliateId, ignore);
                xmlWriter.WriteString("AllowStoringCreditCardNumber", order.AllowStoringCreditCardNumber, ignore);
                xmlWriter.WriteString("CardType", order.CardType, ignore);
                xmlWriter.WriteString("CardName", order.CardName, ignore);
                xmlWriter.WriteString("CardNumber", order.CardNumber, ignore);
                xmlWriter.WriteString("MaskedCreditCardNumber", order.MaskedCreditCardNumber, ignore);
                xmlWriter.WriteString("CardCvv2", order.CardCvv2, ignore);
                xmlWriter.WriteString("CardExpirationMonth", order.CardExpirationMonth, ignore);
                xmlWriter.WriteString("CardExpirationYear", order.CardExpirationYear, ignore);
                xmlWriter.WriteString("PaymentMethodSystemName", order.PaymentMethodSystemName, ignore);
                xmlWriter.WriteString("AuthorizationTransactionId", order.AuthorizationTransactionId, ignore);
                xmlWriter.WriteString("AuthorizationTransactionCode", order.AuthorizationTransactionCode, ignore);
                xmlWriter.WriteString("AuthorizationTransactionResult", order.AuthorizationTransactionResult, ignore);
                xmlWriter.WriteString("CaptureTransactionId", order.CaptureTransactionId, ignore);
                xmlWriter.WriteString("CaptureTransactionResult", order.CaptureTransactionResult, ignore);
                xmlWriter.WriteString("SubscriptionTransactionId", order.SubscriptionTransactionId, ignore);
                xmlWriter.WriteString("PaidDateUtc", order.PaidDateUtc == null ? string.Empty : order.PaidDateUtc.Value.ToString(CultureInfo.InvariantCulture), ignore);
                xmlWriter.WriteString("ShippingMethod", order.ShippingMethod);
                xmlWriter.WriteString("ShippingRateComputationMethodSystemName", order.ShippingRateComputationMethodSystemName, ignore);
                xmlWriter.WriteString("CustomValuesXml", order.CustomValuesXml, ignore);
                xmlWriter.WriteString("VatNumber", order.VatNumber, ignore);
                xmlWriter.WriteString("Deleted", order.Deleted, ignore);
                xmlWriter.WriteString("CreatedOnUtc", order.CreatedOnUtc);

                if (_orderSettings.ExportWithProducts)
                {
                    //products
                    var orderItems = order.OrderItems;

                    //a vendor should have access only to his products
                    if (_workContext.CurrentVendor != null)
                        orderItems = orderItems.Where(oi => oi.Product.VendorId == _workContext.CurrentVendor.Id).ToList();

                    if (orderItems.Any())
                    {
                        xmlWriter.WriteStartElement("OrderItems");
                        foreach (var orderItem in orderItems)
                        {
                            xmlWriter.WriteStartElement("OrderItem");
                            xmlWriter.WriteString("Id", orderItem.Id);
                            xmlWriter.WriteString("OrderItemGuid", orderItem.OrderItemGuid);
                            xmlWriter.WriteString("Name", orderItem.Product.Name);
                            xmlWriter.WriteString("Sku", orderItem.Product.Sku);
                            xmlWriter.WriteString("PriceExclTax", orderItem.UnitPriceExclTax);
                            xmlWriter.WriteString("PriceInclTax", orderItem.UnitPriceInclTax);
                            xmlWriter.WriteString("Quantity", orderItem.Quantity);
                            xmlWriter.WriteString("DiscountExclTax", orderItem.DiscountAmountExclTax);
                            xmlWriter.WriteString("DiscountInclTax", orderItem.DiscountAmountInclTax);
                            xmlWriter.WriteString("TotalExclTax", orderItem.PriceExclTax);
                            xmlWriter.WriteString("TotalInclTax", orderItem.PriceInclTax);
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteEndElement();
                    }
                }

                //shipments
                var shipments = order.Shipments.OrderBy(x => x.CreatedOnUtc).ToList();
                if (shipments.Any())
                {
                    xmlWriter.WriteStartElement("Shipments");
                    foreach (var shipment in shipments)
                    {
                        xmlWriter.WriteStartElement("Shipment");
                        xmlWriter.WriteElementString("ShipmentId", null, shipment.Id.ToString());
                        xmlWriter.WriteElementString("TrackingNumber", null, shipment.TrackingNumber);
                        xmlWriter.WriteElementString("TotalWeight", null, shipment.TotalWeight?.ToString() ?? string.Empty);
                        xmlWriter.WriteElementString("ShippedDateUtc", null, shipment.ShippedDateUtc.HasValue ? shipment.ShippedDateUtc.ToString() : string.Empty);
                        xmlWriter.WriteElementString("DeliveryDateUtc", null, shipment.DeliveryDateUtc?.ToString() ?? string.Empty);
                        xmlWriter.WriteElementString("CreatedOnUtc", null, shipment.CreatedOnUtc.ToString(CultureInfo.InvariantCulture));
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export orders to XLSX
        /// </summary>
        /// <param name="orders">Orders</param>
        public virtual byte[] ExportOrdersToXlsx(IList<Order> orders)
        {
            //a vendor should have access only to part of order information
            var ignore = _workContext.CurrentVendor != null;

            //lambda expression for choosing correct order address
            Address orderAddress(Order o) => o.PickupInStore ? o.PickupAddress : o.ShippingAddress;

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
                new PropertyByName<Order>("BillingFirstName", p => p.BillingAddress?.FirstName ?? string.Empty),
                new PropertyByName<Order>("BillingLastName", p => p.BillingAddress?.LastName ?? string.Empty),
                new PropertyByName<Order>("BillingEmail", p => p.BillingAddress?.Email ?? string.Empty),
                new PropertyByName<Order>("BillingCompany", p => p.BillingAddress?.Company ?? string.Empty),
                new PropertyByName<Order>("BillingCountry", p => p.BillingAddress?.Country?.Name ?? string.Empty),
                new PropertyByName<Order>("BillingStateProvince", p => p.BillingAddress?.StateProvince?.Name ?? string.Empty),
                new PropertyByName<Order>("BillingCounty", p => p.BillingAddress?.County ?? string.Empty),
                new PropertyByName<Order>("BillingCity", p => p.BillingAddress?.City ?? string.Empty),
                new PropertyByName<Order>("BillingAddress1", p => p.BillingAddress?.Address1 ?? string.Empty),
                new PropertyByName<Order>("BillingAddress2", p => p.BillingAddress?.Address2 ?? string.Empty),
                new PropertyByName<Order>("BillingZipPostalCode", p => p.BillingAddress?.ZipPostalCode ?? string.Empty),
                new PropertyByName<Order>("BillingPhoneNumber", p => p.BillingAddress?.PhoneNumber ?? string.Empty),
                new PropertyByName<Order>("BillingFaxNumber", p => p.BillingAddress?.FaxNumber ?? string.Empty),
                new PropertyByName<Order>("ShippingFirstName", p => orderAddress(p)?.FirstName?? string.Empty),
                new PropertyByName<Order>("ShippingLastName", p =>orderAddress(p)?.LastName ?? string.Empty),
                new PropertyByName<Order>("ShippingEmail", p => orderAddress(p)?.Email ?? string.Empty),
                new PropertyByName<Order>("ShippingCompany", p => orderAddress(p)?.Company ?? string.Empty),
                new PropertyByName<Order>("ShippingCountry", p => orderAddress(p)?.Country?.Name ?? string.Empty),
                new PropertyByName<Order>("ShippingStateProvince", p => orderAddress(p)?.StateProvince?.Name ?? string.Empty),
                new PropertyByName<Order>("ShippingCounty", p => orderAddress(p)?.County ?? string.Empty),
                new PropertyByName<Order>("ShippingCity", p => orderAddress(p)?.City ?? string.Empty),
                new PropertyByName<Order>("ShippingAddress1", p => orderAddress(p)?.Address1 ?? string.Empty),
                new PropertyByName<Order>("ShippingAddress2", p => orderAddress(p)?.Address2 ?? string.Empty),
                new PropertyByName<Order>("ShippingZipPostalCode", p => orderAddress(p)?.ZipPostalCode ?? string.Empty),
                new PropertyByName<Order>("ShippingPhoneNumber", p => orderAddress(p)?.PhoneNumber ?? string.Empty),
                new PropertyByName<Order>("ShippingFaxNumber", p => orderAddress(p)?.FaxNumber ?? string.Empty)
            };

            return _orderSettings.ExportWithProducts
                ? ExportOrderToXlsxWithProducts(properties, orders)
                : new PropertyManager<Order>(properties, _catalogSettings).ExportToXlsx(orders);
        }

        /// <summary>
        /// Export customer list to XLSX
        /// </summary>
        /// <param name="customers">Customers</param>
        public virtual byte[] ExportCustomersToXlsx(IList<Customer> customers)
        {
            //property manager 
            var manager = new PropertyManager<Customer>(new[]
            {
                new PropertyByName<Customer>("CustomerId", p => p.Id),
                new PropertyByName<Customer>("CustomerGuid", p => p.CustomerGuid),
                new PropertyByName<Customer>("Email", p => p.Email),
                new PropertyByName<Customer>("Username", p => p.Username),
                new PropertyByName<Customer>("Password", p => _customerService.GetCurrentPassword(p.Id)?.Password),
                new PropertyByName<Customer>("PasswordFormatId", p => _customerService.GetCurrentPassword(p.Id)?.PasswordFormatId ?? 0),
                new PropertyByName<Customer>("PasswordSalt", p => _customerService.GetCurrentPassword(p.Id)?.PasswordSalt),
                new PropertyByName<Customer>("IsTaxExempt", p => p.IsTaxExempt),
                new PropertyByName<Customer>("AffiliateId", p => p.AffiliateId),
                new PropertyByName<Customer>("VendorId", p => p.VendorId),
                new PropertyByName<Customer>("Active", p => p.Active),
                new PropertyByName<Customer>("IsGuest", p => p.IsGuest()),
                new PropertyByName<Customer>("IsRegistered", p => p.IsRegistered()),
                new PropertyByName<Customer>("IsAdministrator", p => p.IsAdmin()),
                new PropertyByName<Customer>("IsForumModerator", p => p.IsForumModerator()),
                new PropertyByName<Customer>("CreatedOnUtc", p => p.CreatedOnUtc),
                //attributes
                new PropertyByName<Customer>("FirstName", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.FirstNameAttribute)),
                new PropertyByName<Customer>("LastName", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.LastNameAttribute)),
                new PropertyByName<Customer>("Gender", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.GenderAttribute)),
                new PropertyByName<Customer>("Company", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.CompanyAttribute)),
                new PropertyByName<Customer>("StreetAddress", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.StreetAddressAttribute)),
                new PropertyByName<Customer>("StreetAddress2", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.StreetAddress2Attribute)),
                new PropertyByName<Customer>("ZipPostalCode", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.ZipPostalCodeAttribute)),
                new PropertyByName<Customer>("City", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.CityAttribute)),
                new PropertyByName<Customer>("County", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.CountyAttribute)),
                new PropertyByName<Customer>("CountryId", p => _genericAttributeService.GetAttribute<int>(p, NopCustomerDefaults.CountryIdAttribute)),
                new PropertyByName<Customer>("StateProvinceId", p => _genericAttributeService.GetAttribute<int>(p, NopCustomerDefaults.StateProvinceIdAttribute)),
                new PropertyByName<Customer>("Phone", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.PhoneAttribute)),
                new PropertyByName<Customer>("Fax", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.FaxAttribute)),
                new PropertyByName<Customer>("VatNumber", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.VatNumberAttribute)),
                new PropertyByName<Customer>("VatNumberStatusId", p => _genericAttributeService.GetAttribute<int>(p, NopCustomerDefaults.VatNumberStatusIdAttribute)),
                new PropertyByName<Customer>("TimeZoneId", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.TimeZoneIdAttribute)),
                new PropertyByName<Customer>("AvatarPictureId", p => _genericAttributeService.GetAttribute<int>(p, NopCustomerDefaults.AvatarPictureIdAttribute)),
                new PropertyByName<Customer>("ForumPostCount", p => _genericAttributeService.GetAttribute<int>(p, NopCustomerDefaults.ForumPostCountAttribute)),
                new PropertyByName<Customer>("Signature", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.SignatureAttribute)),
                new PropertyByName<Customer>("CustomCustomerAttributes",  GetCustomCustomerAttributes)
            }, _catalogSettings);

            return manager.ExportToXlsx(customers);
        }

        /// <summary>
        /// Export customer list to XML
        /// </summary>
        /// <param name="customers">Customers</param>
        /// <returns>Result in XML format</returns>
        public virtual string ExportCustomersToXml(IList<Customer> customers)
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Customers");
            xmlWriter.WriteAttributeString("Version", NopVersion.CurrentVersion);

            foreach (var customer in customers)
            {
                xmlWriter.WriteStartElement("Customer");
                xmlWriter.WriteElementString("CustomerId", null, customer.Id.ToString());
                xmlWriter.WriteElementString("CustomerGuid", null, customer.CustomerGuid.ToString());
                xmlWriter.WriteElementString("Email", null, customer.Email);
                xmlWriter.WriteElementString("Username", null, customer.Username);

                var customerPassword = _customerService.GetCurrentPassword(customer.Id);
                xmlWriter.WriteElementString("Password", null, customerPassword?.Password);
                xmlWriter.WriteElementString("PasswordFormatId", null, (customerPassword?.PasswordFormatId ?? 0).ToString());
                xmlWriter.WriteElementString("PasswordSalt", null, customerPassword?.PasswordSalt);

                xmlWriter.WriteElementString("IsTaxExempt", null, customer.IsTaxExempt.ToString());
                xmlWriter.WriteElementString("AffiliateId", null, customer.AffiliateId.ToString());
                xmlWriter.WriteElementString("VendorId", null, customer.VendorId.ToString());
                xmlWriter.WriteElementString("Active", null, customer.Active.ToString());

                xmlWriter.WriteElementString("IsGuest", null, customer.IsGuest().ToString());
                xmlWriter.WriteElementString("IsRegistered", null, customer.IsRegistered().ToString());
                xmlWriter.WriteElementString("IsAdministrator", null, customer.IsAdmin().ToString());
                xmlWriter.WriteElementString("IsForumModerator", null, customer.IsForumModerator().ToString());
                xmlWriter.WriteElementString("CreatedOnUtc", null, customer.CreatedOnUtc.ToString(CultureInfo.InvariantCulture));

                xmlWriter.WriteElementString("FirstName", null, _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute));
                xmlWriter.WriteElementString("LastName", null, _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute));
                xmlWriter.WriteElementString("Gender", null, _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.GenderAttribute));
                xmlWriter.WriteElementString("Company", null, _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute));

                xmlWriter.WriteElementString("CountryId", null, _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute).ToString());
                xmlWriter.WriteElementString("StreetAddress", null, _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddressAttribute));
                xmlWriter.WriteElementString("StreetAddress2", null, _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddress2Attribute));
                xmlWriter.WriteElementString("ZipPostalCode", null, _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute));
                xmlWriter.WriteElementString("City", null, _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CityAttribute));
                xmlWriter.WriteElementString("County", null, _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CountyAttribute));
                xmlWriter.WriteElementString("StateProvinceId", null, _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute).ToString());
                xmlWriter.WriteElementString("Phone", null, _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute));
                xmlWriter.WriteElementString("Fax", null, _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FaxAttribute));
                xmlWriter.WriteElementString("VatNumber", null, _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.VatNumberAttribute));
                xmlWriter.WriteElementString("VatNumberStatusId", null, _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.VatNumberStatusIdAttribute).ToString());
                xmlWriter.WriteElementString("TimeZoneId", null, _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.TimeZoneIdAttribute));

                foreach (var store in _storeService.GetAllStores())
                {
                    var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id);
                    var subscribedToNewsletters = newsletter != null && newsletter.Active;
                    xmlWriter.WriteElementString($"Newsletter-in-store-{store.Id}", null, subscribedToNewsletters.ToString());
                }

                xmlWriter.WriteElementString("AvatarPictureId", null, _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute).ToString());
                xmlWriter.WriteElementString("ForumPostCount", null, _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.ForumPostCountAttribute).ToString());
                xmlWriter.WriteElementString("Signature", null, _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.SignatureAttribute));

                var selectedCustomerAttributesString = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CustomCustomerAttributes);

                if (!string.IsNullOrEmpty(selectedCustomerAttributesString))
                {
                    var selectedCustomerAttributes = new StringReader(selectedCustomerAttributesString);
                    var selectedCustomerAttributesXmlReader = XmlReader.Create(selectedCustomerAttributes);
                    xmlWriter.WriteNode(selectedCustomerAttributesXmlReader, false);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
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
            foreach (var subscription in subscriptions)
            {
                sb.Append(subscription.Email);
                sb.Append(separator);
                sb.Append(subscription.Active);
                sb.Append(separator);
                sb.Append(subscription.StoreId);
                sb.Append(Environment.NewLine); //new line
            }

            return sb.ToString();
        }

        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>Result in TXT (string) format</returns>
        public virtual string ExportStatesToTxt(IList<StateProvince> states)
        {
            if (states == null)
                throw new ArgumentNullException(nameof(states));

            const string separator = ",";
            var sb = new StringBuilder();
            foreach (var state in states)
            {
                sb.Append(state.Country.TwoLetterIsoCode);
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
        public virtual byte[] ExportCustomerGdprInfoToXlsx(Customer customer, int storeId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //lambda expression for choosing correct order address
            Address orderAddress(Order o) => o.PickupInStore ? o.PickupAddress : o.ShippingAddress;

            //customer info and customer attributes
            var customerManager = new PropertyManager<Customer>(new[]
            {
                new PropertyByName<Customer>("Email", p => p.Email, _customerSettings.UsernamesEnabled),
                new PropertyByName<Customer>("Username", p => p.Username, !_customerSettings.UsernamesEnabled), 
                //attributes
                new PropertyByName<Customer>("First name", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.FirstNameAttribute)),
                new PropertyByName<Customer>("Last name", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.LastNameAttribute)),
                new PropertyByName<Customer>("Gender", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.GenderAttribute), !_customerSettings.GenderEnabled),
                new PropertyByName<Customer>("Date of birth", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.DateOfBirthAttribute), !_customerSettings.DateOfBirthEnabled),
                new PropertyByName<Customer>("Company", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.CompanyAttribute), !_customerSettings.CompanyEnabled),
                new PropertyByName<Customer>("Street address", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.StreetAddressAttribute), !_customerSettings.StreetAddressEnabled),
                new PropertyByName<Customer>("Street address 2", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.StreetAddress2Attribute), !_customerSettings.StreetAddress2Enabled),
                new PropertyByName<Customer>("Zip / postal code", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.ZipPostalCodeAttribute), !_customerSettings.ZipPostalCodeEnabled),
                new PropertyByName<Customer>("City", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.CityAttribute), !_customerSettings.CityEnabled),
                new PropertyByName<Customer>("County", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.CountyAttribute), !_customerSettings.CountyEnabled),
                new PropertyByName<Customer>("Country", p => _countryService.GetCountryById(_genericAttributeService.GetAttribute<int>(p, NopCustomerDefaults.CountryIdAttribute))?.Name ?? string.Empty, !_customerSettings.CountryEnabled),
                new PropertyByName<Customer>("State province", p => _stateProvinceService.GetStateProvinceById(_genericAttributeService.GetAttribute<int>(p, NopCustomerDefaults.StateProvinceIdAttribute))?.Name ?? string.Empty, !(_customerSettings.StateProvinceEnabled && _customerSettings.CountryEnabled)),
                new PropertyByName<Customer>("Phone", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.PhoneAttribute), !_customerSettings.PhoneEnabled),
                new PropertyByName<Customer>("Fax", p => _genericAttributeService.GetAttribute<string>(p, NopCustomerDefaults.FaxAttribute), !_customerSettings.FaxEnabled),
                new PropertyByName<Customer>("Customer attributes",  GetCustomCustomerAttributes)
            }, _catalogSettings);

            //customer orders
            var orderManager = new PropertyManager<Order>(new[]
            {
                new PropertyByName<Order>("Order Number", p => p.CustomOrderNumber),
                new PropertyByName<Order>("Order status", p => _localizationService.GetLocalizedEnum(p.OrderStatus)),
                new PropertyByName<Order>("Order total", p => _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(p.OrderTotal, p.CurrencyRate), true, p.CustomerCurrencyCode, false, _workContext.WorkingLanguage)),
                new PropertyByName<Order>("Shipping method", p => p.ShippingMethod),
                new PropertyByName<Order>("Created on", p => _dateTimeHelper.ConvertToUserTime(p.CreatedOnUtc, DateTimeKind.Utc).ToString("D")),
                new PropertyByName<Order>("Billing first name", p => p.BillingAddress?.FirstName ?? string.Empty),
                new PropertyByName<Order>("Billing last name", p => p.BillingAddress?.LastName ?? string.Empty),
                new PropertyByName<Order>("Billing email", p => p.BillingAddress?.Email ?? string.Empty),
                new PropertyByName<Order>("Billing company", p => p.BillingAddress?.Company ?? string.Empty, !_addressSettings.CompanyEnabled),
                new PropertyByName<Order>("Billing country", p => p.BillingAddress?.Country != null ? _localizationService.GetLocalized(p.BillingAddress.Country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
                new PropertyByName<Order>("Billing state province", p => p.BillingAddress?.StateProvince != null ? _localizationService.GetLocalized(p.BillingAddress.StateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
                new PropertyByName<Order>("Billing county", p => p.BillingAddress?.County ?? string.Empty, !_addressSettings.CountyEnabled),
                new PropertyByName<Order>("Billing city", p => p.BillingAddress?.City ?? string.Empty, !_addressSettings.CityEnabled),
                new PropertyByName<Order>("Billing address 1", p => p.BillingAddress?.Address1 ?? string.Empty, !_addressSettings.StreetAddressEnabled),
                new PropertyByName<Order>("Billing address 2", p => p.BillingAddress?.Address2 ?? string.Empty, !_addressSettings.StreetAddress2Enabled),
                new PropertyByName<Order>("Billing zip postal code", p => p.BillingAddress?.ZipPostalCode ?? string.Empty, !_addressSettings.ZipPostalCodeEnabled),
                new PropertyByName<Order>("Billing phone number", p => p.BillingAddress?.PhoneNumber ?? string.Empty, !_addressSettings.PhoneEnabled),
                new PropertyByName<Order>("Billing fax number", p => p.BillingAddress?.FaxNumber ?? string.Empty, !_addressSettings.FaxEnabled),
                new PropertyByName<Order>("Shipping first name", p => orderAddress(p)?.FirstName ?? string.Empty),
                new PropertyByName<Order>("Shipping last name", p => orderAddress(p)?.LastName ?? string.Empty),
                new PropertyByName<Order>("Shipping email", p => orderAddress(p)?.Email ?? string.Empty),
                new PropertyByName<Order>("Shipping company", p => orderAddress(p)?.Company ?? string.Empty, !_addressSettings.CompanyEnabled),
                new PropertyByName<Order>("Shipping country", p => orderAddress(p)?.Country != null ? _localizationService.GetLocalized(orderAddress(p).Country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
                new PropertyByName<Order>("Shipping state province", p => orderAddress(p)?.StateProvince != null ? _localizationService.GetLocalized(orderAddress(p).StateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
                new PropertyByName<Order>("Shipping county", p => orderAddress(p)?.County ?? string.Empty, !_addressSettings.CountyEnabled),
                new PropertyByName<Order>("Shipping city", p => orderAddress(p)?.City ?? string.Empty, !_addressSettings.CityEnabled),
                new PropertyByName<Order>("Shipping address 1", p => orderAddress(p)?.Address1 ?? string.Empty, !_addressSettings.StreetAddressEnabled),
                new PropertyByName<Order>("Shipping address 2", p => orderAddress(p)?.Address2 ?? string.Empty, !_addressSettings.StreetAddress2Enabled),
                new PropertyByName<Order>("Shipping zip postal code",
                    p => orderAddress(p)?.ZipPostalCode ?? string.Empty, !_addressSettings.ZipPostalCodeEnabled),
                new PropertyByName<Order>("Shipping phone number", p => orderAddress(p)?.PhoneNumber ?? string.Empty, !_addressSettings.PhoneEnabled),
                new PropertyByName<Order>("Shipping fax number", p => orderAddress(p)?.FaxNumber ?? string.Empty, !_addressSettings.FaxEnabled)
            }, _catalogSettings);

            var orderItemsManager = new PropertyManager<OrderItem>(new[]
            {
                new PropertyByName<OrderItem>("SKU", oi => oi.Product.Sku),
                new PropertyByName<OrderItem>("Name", oi => _localizationService.GetLocalized(oi.Product, p => p.Name)),
                new PropertyByName<OrderItem>("Price", oi => _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(oi.Order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax ? oi.UnitPriceInclTax : oi.UnitPriceExclTax, oi.Order.CurrencyRate), true, oi.Order.CustomerCurrencyCode, false, _workContext.WorkingLanguage)),
                new PropertyByName<OrderItem>("Quantity", oi => oi.Quantity),
                new PropertyByName<OrderItem>("Total", oi => _priceFormatter.FormatPrice(oi.Order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax ? oi.PriceInclTax : oi.PriceExclTax))
            }, _catalogSettings);

            var orders = _orderService.SearchOrders(customerId: customer.Id);

            //customer addresses
            var addressManager = new PropertyManager<Address>(new[]
            {
                new PropertyByName<Address>("First name", p => p.FirstName),
                new PropertyByName<Address>("Last name", p => p.LastName),
                new PropertyByName<Address>("Email", p => p.Email),
                new PropertyByName<Address>("Company", p => p.Company, !_addressSettings.CompanyEnabled),
                new PropertyByName<Address>("Country", p => p.Country != null ? _localizationService.GetLocalized(p.Country, c => c.Name) : string.Empty, !_addressSettings.CountryEnabled),
                new PropertyByName<Address>("State province", p => p.StateProvince != null ? _localizationService.GetLocalized(p.StateProvince, sp => sp.Name) : string.Empty, !_addressSettings.StateProvinceEnabled),
                new PropertyByName<Address>("County", p => p.County, !_addressSettings.CountyEnabled),
                new PropertyByName<Address>("City", p => p.City, !_addressSettings.CityEnabled),
                new PropertyByName<Address>("Address 1", p => p.Address1, !_addressSettings.StreetAddressEnabled),
                new PropertyByName<Address>("Address 2", p => p.Address2, !_addressSettings.StreetAddress2Enabled),
                new PropertyByName<Address>("Zip / postal code", p => p.ZipPostalCode, !_addressSettings.ZipPostalCodeEnabled),
                new PropertyByName<Address>("Phone number", p => p.PhoneNumber, !_addressSettings.PhoneEnabled),
                new PropertyByName<Address>("Fax number", p => p.FaxNumber, !_addressSettings.FaxEnabled),
                new PropertyByName<Address>("Custom attributes", p => _customerAttributeFormatter.FormatAttributes(p.CustomAttributes, ";"))
            }, _catalogSettings);

            //customer private messages
            var privateMessageManager = new PropertyManager<PrivateMessage>(new[]
            {
                new PropertyByName<PrivateMessage>("From", pm => _customerSettings.UsernamesEnabled ? pm.FromCustomer.Username : pm.FromCustomer.Email),
                new PropertyByName<PrivateMessage>("To", pm => _customerSettings.UsernamesEnabled ? pm.ToCustomer.Username : pm.ToCustomer.Email),
                new PropertyByName<PrivateMessage>("Subject", pm => pm.Subject),
                new PropertyByName<PrivateMessage>("Text", pm => pm.Text),
                new PropertyByName<PrivateMessage>("Created on", pm => _dateTimeHelper.ConvertToUserTime(pm.CreatedOnUtc, DateTimeKind.Utc).ToString("D"))
            }, _catalogSettings);

            List<PrivateMessage> pmList = null;
            if (_forumSettings.AllowPrivateMessages)
            {
                pmList = _forumService.GetAllPrivateMessages(storeId, customer.Id, 0, null, null, null, null).ToList();
                pmList.AddRange(_forumService.GetAllPrivateMessages(storeId, 0, customer.Id, null, null, null, null).ToList());
            }

            //customer GDPR logs
            var gdprLogManager = new PropertyManager<GdprLog>(new[]
            {
                new PropertyByName<GdprLog>("Request type", log => _localizationService.GetLocalizedEnum(log.RequestType)),
                new PropertyByName<GdprLog>("Request details", log => log.RequestDetails),
                new PropertyByName<GdprLog>("Created on", log => _dateTimeHelper.ConvertToUserTime(log.CreatedOnUtc, DateTimeKind.Utc).ToString("D"))
            }, _catalogSettings);

            var gdprLog = _gdprService.GetAllLog(customer.Id);

            using (var stream = new MemoryStream())
            {
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
                    customerManager.WriteToXlsx(customerInfoWorksheet, customerInfoRow);

                    //customer addresses
                    if (customer.Addresses.Any())
                    {
                        customerInfoRow += 2;

                        var cell = customerInfoWorksheet.Cells[customerInfoRow, 1];
                        cell.Value = "Address List";
                        customerInfoRow += 1;
                        addressManager.SetCaptionStyle(cell);
                        addressManager.WriteCaption(customerInfoWorksheet, customerInfoRow);

                        foreach (var customerAddress in customer.Addresses)
                        {
                            customerInfoRow += 1;
                            addressManager.CurrentObject = customerAddress;
                            addressManager.WriteToXlsx(customerInfoWorksheet, customerInfoRow);
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
                            orderManager.WriteToXlsx(ordersWorksheet, orderRow);

                            //products
                            var orederItems = order.OrderItems.ToList();

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
                                orderItemsManager.WriteToXlsx(ordersWorksheet, orderRow, 2, fWorksheet);
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
                            privateMessageManager.WriteToXlsx(privateMessageWorksheet, privateMessageRow);
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
                            gdprLogManager.WriteToXlsx(gdprLogWorksheet, gdprLogRow);
                        }
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }

        #endregion
    }
}