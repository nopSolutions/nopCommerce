using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.ExportImport.Help;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Seo;
using OfficeOpenXml;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager : IImportManager
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreContext _storeContext;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;

        #endregion

        #region Ctor

        public ImportManager(IProductService productService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IStoreContext storeContext,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService)
        {
            this._productService = productService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._pictureService = pictureService;
            this._urlRecordService = urlRecordService;
            this._storeContext = storeContext;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
        }

        #endregion

        #region Utilities

        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            if (columnName == null)
                throw new ArgumentNullException("columnName");

            for (int i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i + 1; //excel indexes start from 1
            return 0;
        }

        protected virtual string ConvertColumnToString(object columnValue)
        {
            if (columnValue == null)
                return null;

            return Convert.ToString(columnValue);
        }

        protected virtual string GetMimeTypeFromFilePath(string filePath)
        {
            var mimeType = MimeMapping.GetMimeMapping(filePath);

            //little hack here because MimeMapping does not contain all mappings (e.g. PNG)
            if (mimeType == "application/octet-stream")
                mimeType = "image/jpeg";

            return mimeType;
        }

        /// <summary>
        /// Creates or loads the image
        /// </summary>
        /// <param name="picturePath">The path to the image file</param>
        /// <param name="name">The name of the object</param>
        /// <param name="picId">Image identifier, may be null</param>
        /// <returns>The image or null if the image has not changed</returns>
        protected virtual Picture LoadPicture(string picturePath, string name, int? picId = null)
        {
            if (String.IsNullOrEmpty(picturePath) || !File.Exists(picturePath))
                return null;

            var mimeType = GetMimeTypeFromFilePath(picturePath);
            var newPictureBinary = File.ReadAllBytes(picturePath);
            var pictureAlreadyExists = false;
            if (picId != null)
            {
                //compare with existing product pictures
                var existingPicture = _pictureService.GetPictureById(picId.Value);

                var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                //picture binary after validation (like in database)
                var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                if (existingBinary.SequenceEqual(validatedPictureBinary) ||
                    existingBinary.SequenceEqual(newPictureBinary))
                {
                    pictureAlreadyExists = true;
                }
            }

            if (pictureAlreadyExists) return null;

            var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType,
                _pictureService.GetPictureSeName(name));
            return newPicture;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Import products from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void ImportProductsFromXlsx(Stream stream)
        {
            //var start = DateTime.Now;
                //the columns
                var properties = new []
                {
                    new PropertyByName<Product>("ProductTypeId"),
                    new PropertyByName<Product>("ParentGroupedProductId"),
                    new PropertyByName<Product>("VisibleIndividually"),
                    new PropertyByName<Product>("Name"),
                    new PropertyByName<Product>("ShortDescription"),
                    new PropertyByName<Product>("FullDescription"),
                    new PropertyByName<Product>("VendorId"),
                    new PropertyByName<Product>("ProductTemplateId"),
                    new PropertyByName<Product>("ShowOnHomePage"),
                    new PropertyByName<Product>("MetaKeywords"),
                    new PropertyByName<Product>("MetaDescription"),
                    new PropertyByName<Product>("MetaTitle"),
                    new PropertyByName<Product>("SeName"),
                    new PropertyByName<Product>("AllowCustomerReviews"),
                    new PropertyByName<Product>("Published"),
                    new PropertyByName<Product>("SKU"),
                    new PropertyByName<Product>("ManufacturerPartNumber"),
                    new PropertyByName<Product>("Gtin"),
                    new PropertyByName<Product>("IsGiftCard"),
                    new PropertyByName<Product>("GiftCardTypeId"),
                    new PropertyByName<Product>("OverriddenGiftCardAmount"),
                    new PropertyByName<Product>("RequireOtherProducts"),
                    new PropertyByName<Product>("RequiredProductIds"),
                    new PropertyByName<Product>("AutomaticallyAddRequiredProducts"),
                    new PropertyByName<Product>("IsDownload"),
                    new PropertyByName<Product>("DownloadId"),
                    new PropertyByName<Product>("UnlimitedDownloads"),
                    new PropertyByName<Product>("MaxNumberOfDownloads"),
                    new PropertyByName<Product>("DownloadActivationTypeId"),
                    new PropertyByName<Product>("HasSampleDownload"),
                    new PropertyByName<Product>("SampleDownloadId"),
                    new PropertyByName<Product>("HasUserAgreement"),
                    new PropertyByName<Product>("UserAgreementText"),
                    new PropertyByName<Product>("IsRecurring"),
                    new PropertyByName<Product>("RecurringCycleLength"),
                    new PropertyByName<Product>("RecurringCyclePeriodId"),
                    new PropertyByName<Product>("RecurringTotalCycles"),
                    new PropertyByName<Product>("IsRental"),
                    new PropertyByName<Product>("RentalPriceLength"),
                    new PropertyByName<Product>("RentalPricePeriodId"),
                    new PropertyByName<Product>("IsShipEnabled"),
                    new PropertyByName<Product>("IsFreeShipping"),
                    new PropertyByName<Product>("ShipSeparately"),
                    new PropertyByName<Product>("AdditionalShippingCharge"),
                    new PropertyByName<Product>("DeliveryDateId"),
                    new PropertyByName<Product>("IsTaxExempt"),
                    new PropertyByName<Product>("TaxCategoryId"),
                    new PropertyByName<Product>("IsTelecommunicationsOrBroadcastingOrElectronicServices"),
                    new PropertyByName<Product>("ManageInventoryMethodId"),
                    new PropertyByName<Product>("UseMultipleWarehouses"),
                    new PropertyByName<Product>("WarehouseId"),
                    new PropertyByName<Product>("StockQuantity"),
                    new PropertyByName<Product>("DisplayStockAvailability"),
                    new PropertyByName<Product>("DisplayStockQuantity"),
                    new PropertyByName<Product>("MinStockQuantity"),
                    new PropertyByName<Product>("LowStockActivityId"),
                    new PropertyByName<Product>("NotifyAdminForQuantityBelow"),
                    new PropertyByName<Product>("BackorderModeId"),
                    new PropertyByName<Product>("AllowBackInStockSubscriptions"),
                    new PropertyByName<Product>("OrderMinimumQuantity"),
                    new PropertyByName<Product>("OrderMaximumQuantity"),
                    new PropertyByName<Product>("AllowedQuantities"),
                    new PropertyByName<Product>("AllowAddingOnlyExistingAttributeCombinations"),
                    new PropertyByName<Product>("DisableBuyButton"),
                    new PropertyByName<Product>("DisableWishlistButton"),
                    new PropertyByName<Product>("AvailableForPreOrder"),
                    new PropertyByName<Product>("PreOrderAvailabilityStartDateTimeUtc"),
                    new PropertyByName<Product>("CallForPrice"),
                    new PropertyByName<Product>("Price"),
                    new PropertyByName<Product>("OldPrice"),
                    new PropertyByName<Product>("ProductCost"),
                    new PropertyByName<Product>("SpecialPrice"),
                    new PropertyByName<Product>("SpecialPriceStartDateTimeUtc"),
                    new PropertyByName<Product>("SpecialPriceEndDateTimeUtc"),
                    new PropertyByName<Product>("CustomerEntersPrice"),
                    new PropertyByName<Product>("MinimumCustomerEnteredPrice"),
                    new PropertyByName<Product>("MaximumCustomerEnteredPrice"),
                    new PropertyByName<Product>("BasepriceEnabled"),
                    new PropertyByName<Product>("BasepriceAmount"),
                    new PropertyByName<Product>("BasepriceUnitId"),
                    new PropertyByName<Product>("BasepriceBaseAmount"),
                    new PropertyByName<Product>("BasepriceBaseUnitId"),
                    new PropertyByName<Product>("MarkAsNew"),
                    new PropertyByName<Product>("MarkAsNewStartDateTimeUtc"),
                    new PropertyByName<Product>("MarkAsNewEndDateTimeUtc"),
                    new PropertyByName<Product>("Weight"),
                    new PropertyByName<Product>("Length"),
                    new PropertyByName<Product>("Width"),
                    new PropertyByName<Product>("Height"),
                    new PropertyByName<Product>("CategoryIds"),
                    new PropertyByName<Product>("ManufacturerIds"),
                    new PropertyByName<Product>("Picture1"),
                    new PropertyByName<Product>("Picture2"),
                    new PropertyByName<Product>("Picture3")
                };

            var manager = new PropertyManager<Product>(properties);

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                var endRow = 2;
                var allCategoriesIds = new List<int>();
                var allSku = new List<string>();

                var categoryCellNum = manager.GetProperty("CategoryIds").PropertyOrderPosition;
                var skuCellNum = manager.GetProperty("SKU").PropertyOrderPosition;

                var allManufacturersIds = new List<int>();
                var manufacturerCellNum = manager.GetProperty("ManufacturerIds").PropertyOrderPosition;

                //find end of data
                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[endRow, property.PropertyOrderPosition])
                        .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    var categoryIds = worksheet.Cells[endRow, categoryCellNum].Value.Return(p => p.ToString(), string.Empty);
                    if(!categoryIds.IsEmpty())
                        allCategoriesIds.AddRange(categoryIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x.Trim())));

                    var sku = worksheet.Cells[endRow, skuCellNum].Value.Return(p => p.ToString(), string.Empty);
                    if (!sku.IsEmpty())
                        allSku.Add(sku);

                    var manufacturerIds = worksheet.Cells[endRow, manufacturerCellNum].Value.Return(p => p.ToString(), string.Empty);
                    if (!manufacturerIds.IsEmpty())
                        allManufacturersIds.AddRange(manufacturerIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x.Trim())));

                    endRow++;
                }

                //performance optimization, the check for the existence of the categories in one SQL request
                var notExistingCategories = _categoryService.GetNotExistingCategories(allCategoriesIds.ToArray());
                if (notExistingCategories.Any())
                {
                    throw (new ArgumentException(string.Format("The following category ID(s) don't exist - {0}", string.Join(", ", notExistingCategories))));
                }

                //performance optimization, the check for the existence of the manufacturers in one SQL request
                var notExistingManufacturers = _manufacturerService.GetNotExistingManufacturers(allManufacturersIds.ToArray());
                if (notExistingManufacturers.Any())
                {
                    throw new ArgumentException(string.Format("The following manufacturer ID(s) don't exist - {0}", string.Join(", ", notExistingManufacturers)));
                }

                //performance optimization, load all products by SKU in one SQL request
                var allProductsBySku = _productService.GetProductsBySku(allSku.ToArray());

                //performance optimization, load all categories IDs for products in one SQL request
                var allProductsCategoryIds = _categoryService.GetProductCategoryIds(allProductsBySku.Select(p => p.Id).ToArray());

                //performance optimization, load all manufacturers IDs for products in one SQL request
                var allProductsManufacturerIds = _manufacturerService.GetProductManufacturerIds(allProductsBySku.Select(p => p.Id).ToArray());

                for (var iRow = 2; iRow < endRow; iRow++)
                {
                    manager.ReadFromXlsx(worksheet, iRow);

                    var product = allProductsBySku.FirstOrDefault(p=>p.Sku == manager.GetProperty("SKU").StringValue);

                    var isNew = product == null;

                    product = product ?? new Product();

                    if (isNew)
                        product.CreatedOnUtc = DateTime.UtcNow;

                    product.ProductTypeId = manager.GetProperty("ProductTypeId").IntValue;
                    product.ParentGroupedProductId = manager.GetProperty("ParentGroupedProductId").IntValue;
                    product.VisibleIndividually = manager.GetProperty("VisibleIndividually").BooleanValue;
                    product.Name = manager.GetProperty("Name").StringValue;
                    product.ShortDescription = manager.GetProperty("ShortDescription").StringValue;
                    product.FullDescription = manager.GetProperty("FullDescription").StringValue;
                    product.VendorId = manager.GetProperty("VendorId").IntValue;
                    product.ProductTemplateId = manager.GetProperty("ProductTemplateId").IntValue;
                    product.ShowOnHomePage = manager.GetProperty("ShowOnHomePage").BooleanValue;
                    product.MetaKeywords = manager.GetProperty("MetaKeywords").StringValue;
                    product.MetaDescription = manager.GetProperty("MetaDescription").StringValue;
                    product.MetaTitle = manager.GetProperty("MetaTitle").StringValue;
                    var seName = manager.GetProperty("SeName").StringValue;
                    product.AllowCustomerReviews = manager.GetProperty("AllowCustomerReviews").BooleanValue;
                    product.Published = manager.GetProperty("Published").BooleanValue;
                    product.Sku = manager.GetProperty("SKU").StringValue;
                    product.ManufacturerPartNumber = manager.GetProperty("ManufacturerPartNumber").StringValue;
                    product.Gtin = manager.GetProperty("Gtin").StringValue;
                    product.IsGiftCard = manager.GetProperty("IsGiftCard").BooleanValue;
                    product.GiftCardTypeId = manager.GetProperty("GiftCardTypeId").IntValue;
                    product.OverriddenGiftCardAmount = manager.GetProperty("OverriddenGiftCardAmount").DecimalValue;
                    product.RequireOtherProducts = manager.GetProperty("RequireOtherProducts").BooleanValue;
                    product.RequiredProductIds = manager.GetProperty("RequiredProductIds").StringValue;
                    product.AutomaticallyAddRequiredProducts = manager.GetProperty("AutomaticallyAddRequiredProducts").BooleanValue;
                    product.IsDownload = manager.GetProperty("IsDownload").BooleanValue;
                    product.DownloadId = manager.GetProperty("DownloadId").IntValue;
                    product.UnlimitedDownloads = manager.GetProperty("UnlimitedDownloads").BooleanValue;
                    product.MaxNumberOfDownloads = manager.GetProperty("MaxNumberOfDownloads").IntValue;
                    product.DownloadActivationTypeId = manager.GetProperty("DownloadActivationTypeId").IntValue;
                    product.HasSampleDownload = manager.GetProperty("HasSampleDownload").BooleanValue;
                    product.SampleDownloadId = manager.GetProperty("SampleDownloadId").IntValue;
                    product.HasUserAgreement = manager.GetProperty("HasUserAgreement").BooleanValue;
                    product.UserAgreementText = manager.GetProperty("UserAgreementText").StringValue;
                    product.IsRecurring = manager.GetProperty("IsRecurring").BooleanValue;
                    product.RecurringCycleLength = manager.GetProperty("RecurringCycleLength").IntValue;
                    product.RecurringCyclePeriodId = manager.GetProperty("RecurringCyclePeriodId").IntValue;
                    product.RecurringTotalCycles = manager.GetProperty("RecurringTotalCycles").IntValue;
                    product.IsRental = manager.GetProperty("IsRental").BooleanValue;
                    product.RentalPriceLength = manager.GetProperty("RentalPriceLength").IntValue;
                    product.RentalPricePeriodId = manager.GetProperty("RentalPricePeriodId").IntValue;
                    product.IsShipEnabled = manager.GetProperty("IsShipEnabled").BooleanValue;
                    product.IsFreeShipping = manager.GetProperty("IsFreeShipping").BooleanValue;
                    product.ShipSeparately = manager.GetProperty("ShipSeparately").BooleanValue;
                    product.AdditionalShippingCharge = manager.GetProperty("AdditionalShippingCharge").DecimalValue;
                    product.DeliveryDateId = manager.GetProperty("DeliveryDateId").IntValue;
                    product.IsTaxExempt = manager.GetProperty("IsTaxExempt").BooleanValue;
                    product.TaxCategoryId = manager.GetProperty("TaxCategoryId").IntValue;
                    product.IsTelecommunicationsOrBroadcastingOrElectronicServices = manager.GetProperty("IsTelecommunicationsOrBroadcastingOrElectronicServices").BooleanValue;
                    product.ManageInventoryMethodId = manager.GetProperty("ManageInventoryMethodId").IntValue;
                    product.UseMultipleWarehouses = manager.GetProperty("UseMultipleWarehouses").BooleanValue;
                    product.WarehouseId = manager.GetProperty("WarehouseId").IntValue;
                    product.StockQuantity = manager.GetProperty("StockQuantity").IntValue;
                    product.DisplayStockAvailability = manager.GetProperty("DisplayStockAvailability").BooleanValue;
                    product.DisplayStockQuantity = manager.GetProperty("DisplayStockQuantity").BooleanValue;
                    product.MinStockQuantity = manager.GetProperty("MinStockQuantity").IntValue;
                    product.LowStockActivityId = manager.GetProperty("LowStockActivityId").IntValue;
                    product.NotifyAdminForQuantityBelow = manager.GetProperty("NotifyAdminForQuantityBelow").IntValue;
                    product.BackorderModeId = manager.GetProperty("BackorderModeId").IntValue;
                    product.AllowBackInStockSubscriptions = manager.GetProperty("AllowBackInStockSubscriptions").BooleanValue;
                    product.OrderMinimumQuantity = manager.GetProperty("OrderMinimumQuantity").IntValue;
                    product.OrderMaximumQuantity = manager.GetProperty("OrderMaximumQuantity").IntValue;
                    product.AllowedQuantities = manager.GetProperty("AllowedQuantities").StringValue;
                    product.AllowAddingOnlyExistingAttributeCombinations = manager.GetProperty("AllowAddingOnlyExistingAttributeCombinations").BooleanValue;
                    product.DisableBuyButton = manager.GetProperty("DisableBuyButton").BooleanValue;
                    product.DisableWishlistButton = manager.GetProperty("DisableWishlistButton").BooleanValue;
                    product.AvailableForPreOrder = manager.GetProperty("AvailableForPreOrder").BooleanValue;
                    product.PreOrderAvailabilityStartDateTimeUtc = manager.GetProperty("PreOrderAvailabilityStartDateTimeUtc").DateTimeNullable;
                    product.CallForPrice = manager.GetProperty("CallForPrice").BooleanValue;
                    product.Price = manager.GetProperty("Price").DecimalValue;
                    product.OldPrice = manager.GetProperty("OldPrice").DecimalValue;
                    product.ProductCost = manager.GetProperty("ProductCost").DecimalValue;
                    product.SpecialPrice = manager.GetProperty("SpecialPrice").DecimalValueNullable;
                    product.SpecialPriceStartDateTimeUtc = manager.GetProperty("SpecialPriceStartDateTimeUtc").DateTimeNullable;
                    product.SpecialPriceEndDateTimeUtc = manager.GetProperty("SpecialPriceEndDateTimeUtc").DateTimeNullable;
                    product.CustomerEntersPrice = manager.GetProperty("CustomerEntersPrice").BooleanValue;
                    product.MinimumCustomerEnteredPrice = manager.GetProperty("MinimumCustomerEnteredPrice").DecimalValue;
                    product.MaximumCustomerEnteredPrice = manager.GetProperty("MaximumCustomerEnteredPrice").DecimalValue;
                    product.BasepriceEnabled = manager.GetProperty("BasepriceEnabled").BooleanValue;
                    product.BasepriceAmount = manager.GetProperty("BasepriceAmount").DecimalValue;
                    product.BasepriceUnitId = manager.GetProperty("BasepriceUnitId").IntValue;
                    product.BasepriceBaseAmount = manager.GetProperty("BasepriceBaseAmount").DecimalValue;
                    product.BasepriceBaseUnitId = manager.GetProperty("BasepriceBaseUnitId").IntValue;
                    product.MarkAsNew = manager.GetProperty("MarkAsNew").BooleanValue;
                    product.MarkAsNewStartDateTimeUtc = manager.GetProperty("MarkAsNewStartDateTimeUtc").DateTimeNullable;
                    product.MarkAsNewEndDateTimeUtc = manager.GetProperty("MarkAsNewEndDateTimeUtc").DateTimeNullable;
                    product.Weight = manager.GetProperty("Weight").DecimalValue;
                    product.Length = manager.GetProperty("Length").DecimalValue;
                    product.Width = manager.GetProperty("Width").DecimalValue;
                    product.Height = manager.GetProperty("Height").DecimalValue;

                    var categoryIds = manager.GetProperty("CategoryIds").StringValue;
                    var manufacturerIds = manager.GetProperty("ManufacturerIds").StringValue;

                    var picture1 = manager.GetProperty("Picture1").StringValue;
                    var picture2 = manager.GetProperty("Picture2").StringValue;
                    var picture3 = manager.GetProperty("Picture3").StringValue;

                    product.UpdatedOnUtc = DateTime.UtcNow;

                    if (isNew)
                    {
                        _productService.InsertProduct(product);
                    }
                    else
                    {
                        _productService.UpdateProduct(product);
                    }

                    //search engine name
                    _urlRecordService.SaveSlug(product, product.ValidateSeName(seName, product.Name, true), 0);

                    //category mappings
                    var categories = isNew || !allProductsCategoryIds.ContainsKey(product.Id) ? new int[0] : allProductsCategoryIds[product.Id];
                    foreach (var categoryId in categoryIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x.Trim())))
                    {
                        if (categories.Any(c => c == categoryId))
                            continue;
                       
                        var productCategory = new ProductCategory
                        {
                            ProductId = product.Id,
                            CategoryId = categoryId,
                            IsFeaturedProduct = false,
                            DisplayOrder = 1
                        };
                        _categoryService.InsertProductCategory(productCategory);
                    }

                    //manufacturer mappings
                    var manufacturers = isNew || !allProductsManufacturerIds.ContainsKey(product.Id) ? new int[0] : allProductsManufacturerIds[product.Id];
                    foreach (var manufacturerId in manufacturerIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x.Trim())))
                    {
                        if (manufacturers.Any(c => c == manufacturerId))
                            continue;

                        var productManufacturer = new ProductManufacturer
                        {
                            ProductId = product.Id,
                            ManufacturerId = manufacturerId,
                            IsFeaturedProduct = false,
                            DisplayOrder = 1
                        };
                        _manufacturerService.InsertProductManufacturer(productManufacturer);
                    }

                    //pictures
                    foreach (var picturePath in new[] { picture1, picture2, picture3 })
                    {
                        if (String.IsNullOrEmpty(picturePath))
                            continue;

                        var mimeType = GetMimeTypeFromFilePath(picturePath);
                        var newPictureBinary = File.ReadAllBytes(picturePath);
                        var pictureAlreadyExists = false;
                        if (!isNew)
                        {
                            //compare with existing product pictures
                            var existingPictures = _pictureService.GetPicturesByProductId(product.Id);
                            foreach (var existingPicture in existingPictures)
                            {
                                var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                                //picture binary after validation (like in database)
                                var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                                if (!existingBinary.SequenceEqual(validatedPictureBinary) && !existingBinary.SequenceEqual(newPictureBinary))
                                    continue;
                                //the same picture content
                                pictureAlreadyExists = true;
                                break;
                            }
                        }

                        if (pictureAlreadyExists)
                            continue;
                        var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType, _pictureService.GetPictureSeName(product.Name));
                        product.ProductPictures.Add(new ProductPicture
                        {
                            //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                            //pictures are duplicated
                            //maybe because entity size is too large
                            PictureId = newPicture.Id,
                            DisplayOrder = 1,
                        });
                        _productService.UpdateProduct(product);
                    }

                    //update "HasTierPrices" and "HasDiscountsApplied" properties
                    //_productService.UpdateHasTierPricesProperty(product);
                    //_productService.UpdateHasDiscountsApplied(product);
                }
            }
            //Trace.WriteLine(DateTime.Now-start);
        }

        /// <summary>
        /// Import newsletter subscribers from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Number of imported subscribers</returns>
        public virtual int ImportNewsletterSubscribersFromTxt(Stream stream)
        {
            int count = 0;
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(line))
                        continue;
                    string[] tmp = line.Split(',');

                    string email;
                    bool isActive = true;
                    int storeId = _storeContext.CurrentStore.Id;
                    //parse
                    if (tmp.Length == 1)
                    {
                        //"email" only
                        email = tmp[0].Trim();
                    }
                    else if (tmp.Length == 2)
                    {
                        //"email" and "active" fields specified
                        email = tmp[0].Trim();
                        isActive = Boolean.Parse(tmp[1].Trim());
                    }
                    else if (tmp.Length == 3)
                    {
                        //"email" and "active" and "storeId" fields specified
                        email = tmp[0].Trim();
                        isActive = Boolean.Parse(tmp[1].Trim());
                        storeId = Int32.Parse(tmp[2].Trim());
                    }
                    else
                        throw new NopException("Wrong file format");

                    //import
                    var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(email, storeId);
                    if (subscription != null)
                    {
                        subscription.Email = email;
                        subscription.Active = isActive;
                        _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);
                    }
                    else
                    {
                        subscription = new NewsLetterSubscription
                        {
                            Active = isActive,
                            CreatedOnUtc = DateTime.UtcNow,
                            Email = email,
                            StoreId = storeId,
                            NewsLetterSubscriptionGuid = Guid.NewGuid()
                        };
                        _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                    }
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Import states from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Number of imported states</returns>
        public virtual int ImportStatesFromTxt(Stream stream)
        {
            int count = 0;
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(line))
                        continue;
                    string[] tmp = line.Split(',');

                    if (tmp.Length != 5)
                        throw new NopException("Wrong file format");

                    //parse
                    var countryTwoLetterIsoCode = tmp[0].Trim();
                    var name = tmp[1].Trim();
                    var abbreviation = tmp[2].Trim();
                    bool published = Boolean.Parse(tmp[3].Trim());
                    int displayOrder = Int32.Parse(tmp[4].Trim());

                    var country = _countryService.GetCountryByTwoLetterIsoCode(countryTwoLetterIsoCode);
                    if (country == null)
                    {
                        //country cannot be loaded. skip
                        continue;
                    }

                    //import
                    var states = _stateProvinceService.GetStateProvincesByCountryId(country.Id, showHidden: true);
                    var state = states.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                    if (state != null)
                    {
                        state.Abbreviation = abbreviation;
                        state.Published = published;
                        state.DisplayOrder = displayOrder;
                        _stateProvinceService.UpdateStateProvince(state);
                    }
                    else
                    {
                        state = new StateProvince
                        {
                            CountryId = country.Id,
                            Name = name,
                            Abbreviation = abbreviation,
                            Published = published,
                            DisplayOrder = displayOrder,
                        };
                        _stateProvinceService.InsertStateProvince(state);
                    }
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Import manufacturers from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void ImportManufacturersFromXlsx(Stream stream)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<Manufacturer>("Id"),
                new PropertyByName<Manufacturer>("Name"),
                new PropertyByName<Manufacturer>("Description"),
                new PropertyByName<Manufacturer>("ManufacturerTemplateId"),
                new PropertyByName<Manufacturer>("MetaKeywords"),
                new PropertyByName<Manufacturer>("MetaDescription"),
                new PropertyByName<Manufacturer>("MetaTitle"),
                new PropertyByName<Manufacturer>("Picture"),
                new PropertyByName<Manufacturer>("PageSize"),
                new PropertyByName<Manufacturer>("AllowCustomersToSelectPageSize"),
                new PropertyByName<Manufacturer>("PageSizeOptions"),
                new PropertyByName<Manufacturer>("PriceRanges"),
                new PropertyByName<Manufacturer>("Published"),
                new PropertyByName<Manufacturer>("DisplayOrder")
            };

            var manager = new PropertyManager<Manufacturer>(properties);

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                var iRow = 2;

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[iRow, property.PropertyOrderPosition])
                        .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadFromXlsx(worksheet, iRow);

                    var manufacturer = _manufacturerService.GetManufacturerById(manager.GetProperty("Id").IntValue);

                    var isNew = manufacturer == null;

                    manufacturer = manufacturer ?? new Manufacturer();

                    if (isNew)
                        manufacturer.CreatedOnUtc = DateTime.UtcNow;

                    manufacturer.Name = manager.GetProperty("Name").StringValue;
                    manufacturer.Description = manager.GetProperty("Description").StringValue;
                    manufacturer.ManufacturerTemplateId = manager.GetProperty("ManufacturerTemplateId").IntValue;
                    manufacturer.MetaKeywords = manager.GetProperty("MetaKeywords").StringValue;
                    manufacturer.MetaDescription = manager.GetProperty("MetaDescription").StringValue;
                    manufacturer.MetaTitle = manager.GetProperty("MetaTitle").StringValue;
                    var picture = LoadPicture(manager.GetProperty("Picture").StringValue, manufacturer.Name,
                        isNew ? null : (int?) manufacturer.PictureId);
                    manufacturer.PageSize = manager.GetProperty("PageSize").IntValue;
                    manufacturer.AllowCustomersToSelectPageSize = manager.GetProperty("AllowCustomersToSelectPageSize").BooleanValue;
                    manufacturer.PageSizeOptions = manager.GetProperty("PageSizeOptions").StringValue;
                    manufacturer.PriceRanges = manager.GetProperty("PriceRanges").StringValue;
                    manufacturer.Published = manager.GetProperty("Published").BooleanValue;
                    manufacturer.DisplayOrder = manager.GetProperty("DisplayOrder").IntValue;

                    if (picture != null)
                        manufacturer.PictureId = picture.Id;

                    manufacturer.UpdatedOnUtc = DateTime.UtcNow;

                    if (isNew)
                        _manufacturerService.InsertManufacturer(manufacturer);
                    else
                        _manufacturerService.UpdateManufacturer(manufacturer);

                    iRow++;
                }
            }
        }

        /// <summary>
        /// Import categories from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void ImportCategoriesFromXlsx(Stream stream)
        {
            var properties = new[]
            {
                new PropertyByName<Category>("Id"),
                new PropertyByName<Category>("Name"),
                new PropertyByName<Category>("Description"),
                new PropertyByName<Category>("CategoryTemplateId"),
                new PropertyByName<Category>("MetaKeywords"),
                new PropertyByName<Category>("MetaDescription"),
                new PropertyByName<Category>("MetaTitle"),
                new PropertyByName<Category>("ParentCategoryId"),
                new PropertyByName<Category>("Picture"),
                new PropertyByName<Category>("PageSize"),
                new PropertyByName<Category>("AllowCustomersToSelectPageSize"),
                new PropertyByName<Category>("PageSizeOptions"),
                new PropertyByName<Category>("PriceRanges"),
                new PropertyByName<Category>("ShowOnHomePage"),
                new PropertyByName<Category>("IncludeInTopMenu"),
                new PropertyByName<Category>("Published"),
                new PropertyByName<Category>("DisplayOrder")
            };

            var manager = new PropertyManager<Category>(properties);

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                var iRow = 2;

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[iRow, property.PropertyOrderPosition])
                        .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadFromXlsx(worksheet, iRow);


                    var category = _categoryService.GetCategoryById(manager.GetProperty("Id").IntValue);

                    var isNew = category == null;

                    category = category ?? new Category();

                    if (isNew)
                        category.CreatedOnUtc = DateTime.UtcNow;

                    category.Name = manager.GetProperty("Name").StringValue;
                    category.Description = manager.GetProperty("Description").StringValue;

                    category.CategoryTemplateId = manager.GetProperty("CategoryTemplateId").IntValue;
                    category.MetaKeywords = manager.GetProperty("MetaKeywords").StringValue;
                    category.MetaDescription = manager.GetProperty("MetaDescription").StringValue;
                    category.MetaTitle = manager.GetProperty("MetaTitle").StringValue;
                    category.ParentCategoryId = manager.GetProperty("ParentCategoryId").IntValue;
                    var picture = LoadPicture(manager.GetProperty("Picture").StringValue, category.Name, isNew ? null : (int?) category.PictureId);
                    category.PageSize = manager.GetProperty("PageSize").IntValue;
                    category.AllowCustomersToSelectPageSize = manager.GetProperty("AllowCustomersToSelectPageSize").BooleanValue;
                    category.PageSizeOptions = manager.GetProperty("PageSizeOptions").StringValue;
                    category.PriceRanges = manager.GetProperty("PriceRanges").StringValue;
                    category.ShowOnHomePage = manager.GetProperty("ShowOnHomePage").BooleanValue;
                    category.IncludeInTopMenu = manager.GetProperty("IncludeInTopMenu").BooleanValue;
                    category.Published = manager.GetProperty("Published").BooleanValue;
                    category.DisplayOrder = manager.GetProperty("DisplayOrder").IntValue;

                    if (picture != null)
                        category.PictureId = picture.Id;

                    category.UpdatedOnUtc = DateTime.UtcNow;

                    if (isNew)
                        _categoryService.InsertCategory(category);
                    else
                        _categoryService.UpdateCategory(category);

                    iRow++;
                }
            }
        }

        #endregion
    }
}
