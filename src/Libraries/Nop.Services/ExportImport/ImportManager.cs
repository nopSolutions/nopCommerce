using System;
using System.IO;
using System.Linq;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Messages;
using Nop.Services.Catalog;
using Nop.Services.Directory;
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

        #endregion

        #region Methods

        /// <summary>
        /// Import products from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void ImportProductsFromXlsx(Stream stream)
        {
            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                //the columns
                var properties = new []
                {
                    "ProductTypeId",
                    "ParentGroupedProductId",
                    "VisibleIndividually",
                    "Name",
                    "ShortDescription",
                    "FullDescription",
                    "VendorId",
                    "ProductTemplateId",
                    "ShowOnHomePage",
                    "MetaKeywords",
                    "MetaDescription",
                    "MetaTitle",
                    "SeName",
                    "AllowCustomerReviews",
                    "Published",
                    "SKU",
                    "ManufacturerPartNumber",
                    "Gtin",
                    "IsGiftCard",
                    "GiftCardTypeId",
                    "OverriddenGiftCardAmount",
                    "RequireOtherProducts",
                    "RequiredProductIds",
                    "AutomaticallyAddRequiredProducts",
                    "IsDownload",
                    "DownloadId",
                    "UnlimitedDownloads",
                    "MaxNumberOfDownloads",
                    "DownloadActivationTypeId",
                    "HasSampleDownload",
                    "SampleDownloadId",
                    "HasUserAgreement",
                    "UserAgreementText",
                    "IsRecurring",
                    "RecurringCycleLength",
                    "RecurringCyclePeriodId",
                    "RecurringTotalCycles",
                    "IsRental",
                    "RentalPriceLength",
                    "RentalPricePeriodId",
                    "IsShipEnabled",
                    "IsFreeShipping",
                    "ShipSeparately",
                    "AdditionalShippingCharge",
                    "DeliveryDateId",
                    "IsTaxExempt",
                    "TaxCategoryId",
                    "IsTelecommunicationsOrBroadcastingOrElectronicServices",
                    "ManageInventoryMethodId",
                    "UseMultipleWarehouses",
                    "WarehouseId",
                    "StockQuantity",
                    "DisplayStockAvailability",
                    "DisplayStockQuantity",
                    "MinStockQuantity",
                    "LowStockActivityId",
                    "NotifyAdminForQuantityBelow",
                    "BackorderModeId",
                    "AllowBackInStockSubscriptions",
                    "OrderMinimumQuantity",
                    "OrderMaximumQuantity",
                    "AllowedQuantities",
                    "AllowAddingOnlyExistingAttributeCombinations",
                    "DisableBuyButton",
                    "DisableWishlistButton",
                    "AvailableForPreOrder",
                    "PreOrderAvailabilityStartDateTimeUtc",
                    "CallForPrice",
                    "Price",
                    "OldPrice",
                    "ProductCost",
                    "SpecialPrice",
                    "SpecialPriceStartDateTimeUtc",
                    "SpecialPriceEndDateTimeUtc",
                    "CustomerEntersPrice",
                    "MinimumCustomerEnteredPrice",
                    "MaximumCustomerEnteredPrice",
                    "BasepriceEnabled",
                    "BasepriceAmount",
                    "BasepriceUnitId",
                    "BasepriceBaseAmount",
                    "BasepriceBaseUnitId",
                    "MarkAsNew",
                    "MarkAsNewStartDateTimeUtc",
                    "MarkAsNewEndDateTimeUtc",
                    "Weight",
                    "Length",
                    "Width",
                    "Height",
                    "CategoryIds",
                    "ManufacturerIds",
                    "Picture1",
                    "Picture2",
                    "Picture3"
                };


                int iRow = 2;
                while (true)
                {
                    bool allColumnsAreEmpty = true;
                    for (var i = 1; i <= properties.Length; i++)
                        if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                        {
                            allColumnsAreEmpty = false;
                            break;
                        }
                    if (allColumnsAreEmpty)
                        break;

                    int productTypeId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "ProductTypeId")].Value);
                    int parentGroupedProductId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "ParentGroupedProductId")].Value);
                    bool visibleIndividually = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "VisibleIndividually")].Value);
                    string name = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Name")].Value);
                    string shortDescription = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "ShortDescription")].Value);
                    string fullDescription = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "FullDescription")].Value);
                    int vendorId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "VendorId")].Value);
                    int productTemplateId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "ProductTemplateId")].Value);
                    bool showOnHomePage = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "ShowOnHomePage")].Value);
                    string metaKeywords = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MetaKeywords")].Value);
                    string metaDescription = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MetaDescription")].Value);
                    string metaTitle = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "MetaTitle")].Value);
                    string seName = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "SeName")].Value);
                    bool allowCustomerReviews = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AllowCustomerReviews")].Value);
                    bool published = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "Published")].Value);
                    string sku = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "SKU")].Value);
                    string manufacturerPartNumber = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "ManufacturerPartNumber")].Value);
                    string gtin = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Gtin")].Value);
                    bool isGiftCard = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsGiftCard")].Value);
                    int giftCardTypeId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "GiftCardTypeId")].Value);
                    decimal? overriddenGiftCardAmount = null;
                    var overriddenGiftCardAmountExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "OverriddenGiftCardAmount")].Value;
                    if (overriddenGiftCardAmountExcel != null)
                        overriddenGiftCardAmount = Convert.ToDecimal(overriddenGiftCardAmountExcel);
                    bool requireOtherProducts = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "RequireOtherProducts")].Value);
                    string requiredProductIds = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "RequiredProductIds")].Value);
                    bool automaticallyAddRequiredProducts = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AutomaticallyAddRequiredProducts")].Value);
                    bool isDownload = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsDownload")].Value);
                    int downloadId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "DownloadId")].Value);
                    bool unlimitedDownloads = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "UnlimitedDownloads")].Value);
                    int maxNumberOfDownloads = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "MaxNumberOfDownloads")].Value);
                    int downloadActivationTypeId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "DownloadActivationTypeId")].Value);
                    bool hasSampleDownload = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "HasSampleDownload")].Value);
                    int sampleDownloadId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "SampleDownloadId")].Value);
                    bool hasUserAgreement = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "HasUserAgreement")].Value);
                    string userAgreementText = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "UserAgreementText")].Value);
                    bool isRecurring = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsRecurring")].Value);
                    int recurringCycleLength = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "RecurringCycleLength")].Value);
                    int recurringCyclePeriodId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "RecurringCyclePeriodId")].Value);
                    int recurringTotalCycles = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "RecurringTotalCycles")].Value);
                    bool isRental = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsRental")].Value);
                    int rentalPriceLength = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "RentalPriceLength")].Value);
                    int rentalPricePeriodId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "RentalPricePeriodId")].Value);
                    bool isShipEnabled = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsShipEnabled")].Value);
                    bool isFreeShipping = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsFreeShipping")].Value);
                    bool shipSeparately = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "ShipSeparately")].Value);
                    decimal additionalShippingCharge = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "AdditionalShippingCharge")].Value);
                    int deliveryDateId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "DeliveryDateId")].Value);
                    bool isTaxExempt = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsTaxExempt")].Value);
                    int taxCategoryId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "TaxCategoryId")].Value);
                    bool isTelecommunicationsOrBroadcastingOrElectronicServices = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsTelecommunicationsOrBroadcastingOrElectronicServices")].Value);
                    int manageInventoryMethodId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "ManageInventoryMethodId")].Value);
                    bool useMultipleWarehouses = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "UseMultipleWarehouses")].Value);
                    int warehouseId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "WarehouseId")].Value);
                    int stockQuantity = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "StockQuantity")].Value);
                    bool displayStockAvailability = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "DisplayStockAvailability")].Value);
                    bool displayStockQuantity = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "DisplayStockQuantity")].Value);
                    int minStockQuantity = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "MinStockQuantity")].Value);
                    int lowStockActivityId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "LowStockActivityId")].Value);
                    int notifyAdminForQuantityBelow = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "NotifyAdminForQuantityBelow")].Value);
                    int backorderModeId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "BackorderModeId")].Value);
                    bool allowBackInStockSubscriptions = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AllowBackInStockSubscriptions")].Value);
                    int orderMinimumQuantity = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "OrderMinimumQuantity")].Value);
                    int orderMaximumQuantity = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "OrderMaximumQuantity")].Value);
                    string allowedQuantities = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "AllowedQuantities")].Value);
                    bool allowAddingOnlyExistingAttributeCombinations = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AllowAddingOnlyExistingAttributeCombinations")].Value);
                    bool disableBuyButton = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "DisableBuyButton")].Value);
                    bool disableWishlistButton = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "DisableWishlistButton")].Value);
                    bool availableForPreOrder = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AvailableForPreOrder")].Value);
                    DateTime? preOrderAvailabilityStartDateTimeUtc = null;
                    var preOrderAvailabilityStartDateTimeUtcExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "PreOrderAvailabilityStartDateTimeUtc")].Value;
                    if (preOrderAvailabilityStartDateTimeUtcExcel != null)
                        preOrderAvailabilityStartDateTimeUtc = DateTime.FromOADate(Convert.ToDouble(preOrderAvailabilityStartDateTimeUtcExcel));
                    bool callForPrice = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "CallForPrice")].Value);
                    decimal price = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Price")].Value);
                    decimal oldPrice = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "OldPrice")].Value);
                    decimal productCost = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "ProductCost")].Value);
                    decimal? specialPrice = null;
                    var specialPriceExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "SpecialPrice")].Value;
                    if (specialPriceExcel != null)
                        specialPrice = Convert.ToDecimal(specialPriceExcel);
                    DateTime? specialPriceStartDateTimeUtc = null;
                    var specialPriceStartDateTimeUtcExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "SpecialPriceStartDateTimeUtc")].Value;
                    if (specialPriceStartDateTimeUtcExcel != null)
                        specialPriceStartDateTimeUtc = DateTime.FromOADate(Convert.ToDouble(specialPriceStartDateTimeUtcExcel));
                    DateTime? specialPriceEndDateTimeUtc = null;
                    var specialPriceEndDateTimeUtcExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "SpecialPriceEndDateTimeUtc")].Value;
                    if (specialPriceEndDateTimeUtcExcel != null)
                        specialPriceEndDateTimeUtc = DateTime.FromOADate(Convert.ToDouble(specialPriceEndDateTimeUtcExcel));

                    bool customerEntersPrice = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "CustomerEntersPrice")].Value);
                    decimal minimumCustomerEnteredPrice = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "MinimumCustomerEnteredPrice")].Value);
                    decimal maximumCustomerEnteredPrice = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "MaximumCustomerEnteredPrice")].Value);
                    bool basepriceEnabled = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "BasepriceEnabled")].Value);
                    decimal basepriceAmount = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "BasepriceAmount")].Value);
                    int basepriceUnitId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "BasepriceUnitId")].Value);
                    decimal basepriceBaseAmount = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "BasepriceBaseAmount")].Value);
                    int basepriceBaseUnitId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "BasepriceBaseUnitId")].Value);
                    bool markAsNew = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "MarkAsNew")].Value);
                    DateTime? markAsNewStartDateTimeUtc = null;
                    var markAsNewStartDateTimeUtcExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "MarkAsNewStartDateTimeUtc")].Value;
                    if (markAsNewStartDateTimeUtcExcel != null)
                        markAsNewStartDateTimeUtc = DateTime.FromOADate(Convert.ToDouble(markAsNewStartDateTimeUtcExcel));
                    DateTime? markAsNewEndDateTimeUtc = null;
                    var markAsNewEndDateTimeUtcExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "MarkAsNewEndDateTimeUtc")].Value;
                    if (markAsNewEndDateTimeUtcExcel != null)
                        markAsNewEndDateTimeUtc = DateTime.FromOADate(Convert.ToDouble(markAsNewEndDateTimeUtcExcel));
                    decimal weight = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Weight")].Value);
                    decimal length = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Length")].Value);
                    decimal width = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Width")].Value);
                    decimal height = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Height")].Value);
                    string categoryIds = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "CategoryIds")].Value);
                    string manufacturerIds = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "ManufacturerIds")].Value);
                    string picture1 = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Picture1")].Value);
                    string picture2 = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Picture2")].Value);
                    string picture3 = ConvertColumnToString(worksheet.Cells[iRow, GetColumnIndex(properties, "Picture3")].Value);



                    var product = _productService.GetProductBySku(sku);
                    bool newProduct = false;
                    if (product == null)
                    {
                        product = new Product();
                        newProduct = true;
                    }
                    product.ProductTypeId = productTypeId;
                    product.ParentGroupedProductId = parentGroupedProductId;
                    product.VisibleIndividually = visibleIndividually;
                    product.Name = name;
                    product.ShortDescription = shortDescription;
                    product.FullDescription = fullDescription;
                    product.VendorId = vendorId;
                    product.ProductTemplateId = productTemplateId;
                    product.ShowOnHomePage = showOnHomePage;
                    product.MetaKeywords = metaKeywords;
                    product.MetaDescription = metaDescription;
                    product.MetaTitle = metaTitle;
                    product.AllowCustomerReviews = allowCustomerReviews;
                    product.Sku = sku;
                    product.ManufacturerPartNumber = manufacturerPartNumber;
                    product.Gtin = gtin;
                    product.IsGiftCard = isGiftCard;
                    product.GiftCardTypeId = giftCardTypeId;
                    product.OverriddenGiftCardAmount = overriddenGiftCardAmount;
                    product.RequireOtherProducts = requireOtherProducts;
                    product.RequiredProductIds = requiredProductIds;
                    product.AutomaticallyAddRequiredProducts = automaticallyAddRequiredProducts;
                    product.IsDownload = isDownload;
                    product.DownloadId = downloadId;
                    product.UnlimitedDownloads = unlimitedDownloads;
                    product.MaxNumberOfDownloads = maxNumberOfDownloads;
                    product.DownloadActivationTypeId = downloadActivationTypeId;
                    product.HasSampleDownload = hasSampleDownload;
                    product.SampleDownloadId = sampleDownloadId;
                    product.HasUserAgreement = hasUserAgreement;
                    product.UserAgreementText = userAgreementText;
                    product.IsRecurring = isRecurring;
                    product.RecurringCycleLength = recurringCycleLength;
                    product.RecurringCyclePeriodId = recurringCyclePeriodId;
                    product.RecurringTotalCycles = recurringTotalCycles;
                    product.IsRental = isRental;
                    product.RentalPriceLength = rentalPriceLength;
                    product.RentalPricePeriodId = rentalPricePeriodId;
                    product.IsShipEnabled = isShipEnabled;
                    product.IsFreeShipping = isFreeShipping;
                    product.ShipSeparately = shipSeparately;
                    product.AdditionalShippingCharge = additionalShippingCharge;
                    product.DeliveryDateId = deliveryDateId;
                    product.IsTaxExempt = isTaxExempt;
                    product.TaxCategoryId = taxCategoryId;
                    product.IsTelecommunicationsOrBroadcastingOrElectronicServices = isTelecommunicationsOrBroadcastingOrElectronicServices;
                    product.ManageInventoryMethodId = manageInventoryMethodId;
                    product.UseMultipleWarehouses = useMultipleWarehouses;
                    product.WarehouseId = warehouseId;
                    product.StockQuantity = stockQuantity;
                    product.DisplayStockAvailability = displayStockAvailability;
                    product.DisplayStockQuantity = displayStockQuantity;
                    product.MinStockQuantity = minStockQuantity;
                    product.LowStockActivityId = lowStockActivityId;
                    product.NotifyAdminForQuantityBelow = notifyAdminForQuantityBelow;
                    product.BackorderModeId = backorderModeId;
                    product.AllowBackInStockSubscriptions = allowBackInStockSubscriptions;
                    product.OrderMinimumQuantity = orderMinimumQuantity;
                    product.OrderMaximumQuantity = orderMaximumQuantity;
                    product.AllowedQuantities = allowedQuantities;
                    product.AllowAddingOnlyExistingAttributeCombinations = allowAddingOnlyExistingAttributeCombinations;
                    product.DisableBuyButton = disableBuyButton;
                    product.DisableWishlistButton = disableWishlistButton;
                    product.AvailableForPreOrder = availableForPreOrder;
                    product.PreOrderAvailabilityStartDateTimeUtc = preOrderAvailabilityStartDateTimeUtc;
                    product.CallForPrice = callForPrice;
                    product.Price = price;
                    product.OldPrice = oldPrice;
                    product.ProductCost = productCost;
                    product.SpecialPrice = specialPrice;
                    product.SpecialPriceStartDateTimeUtc = specialPriceStartDateTimeUtc;
                    product.SpecialPriceEndDateTimeUtc = specialPriceEndDateTimeUtc;
                    product.CustomerEntersPrice = customerEntersPrice;
                    product.MinimumCustomerEnteredPrice = minimumCustomerEnteredPrice;
                    product.MaximumCustomerEnteredPrice = maximumCustomerEnteredPrice;
                    product.BasepriceEnabled = basepriceEnabled;
                    product.BasepriceAmount = basepriceAmount;
                    product.BasepriceUnitId = basepriceUnitId;
                    product.BasepriceBaseAmount = basepriceBaseAmount;
                    product.BasepriceBaseUnitId = basepriceBaseUnitId;
                    product.MarkAsNew = markAsNew;
                    product.MarkAsNewStartDateTimeUtc = markAsNewStartDateTimeUtc;
                    product.MarkAsNewEndDateTimeUtc = markAsNewEndDateTimeUtc;
                    product.Weight = weight;
                    product.Length = length;
                    product.Width = width;
                    product.Height = height;
                    product.Published = published;

                    if(newProduct)
                        product.CreatedOnUtc = DateTime.UtcNow;

                    product.UpdatedOnUtc = DateTime.UtcNow;
                    if (newProduct)
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
                    if (!String.IsNullOrEmpty(categoryIds))
                    {
                        foreach (var id in categoryIds.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x.Trim())))
                        {
                            if (product.ProductCategories.FirstOrDefault(x => x.CategoryId == id) == null)
                            {
                                //ensure that category exists
                                var category = _categoryService.GetCategoryById(id);
                                if (category != null)
                                {
                                    var productCategory = new ProductCategory
                                    {
                                        ProductId = product.Id,
                                        CategoryId = category.Id,
                                        IsFeaturedProduct = false,
                                        DisplayOrder = 1
                                    };
                                    _categoryService.InsertProductCategory(productCategory);
                                }
                            }
                        }
                    }

                    //manufacturer mappings
                    if (!String.IsNullOrEmpty(manufacturerIds))
                    {
                        foreach (var id in manufacturerIds.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x.Trim())))
                        {
                            if (product.ProductManufacturers.FirstOrDefault(x => x.ManufacturerId == id) == null)
                            {
                                //ensure that manufacturer exists
                                var manufacturer = _manufacturerService.GetManufacturerById(id);
                                if (manufacturer != null)
                                {
                                    var productManufacturer = new ProductManufacturer
                                    {
                                        ProductId = product.Id,
                                        ManufacturerId = manufacturer.Id,
                                        IsFeaturedProduct = false,
                                        DisplayOrder = 1
                                    };
                                    _manufacturerService.InsertProductManufacturer(productManufacturer);
                                }
                            }
                        }
                    }

                    //pictures
                    foreach (var picturePath in new [] { picture1, picture2, picture3 })
                    {
                        if (String.IsNullOrEmpty(picturePath))
                            continue;

                        var mimeType = GetMimeTypeFromFilePath(picturePath);
                        var newPictureBinary = File.ReadAllBytes(picturePath);
                        var pictureAlreadyExists = false;
                        if (!newProduct)
                        {
                            //compare with existing product pictures
                            var existingPictures = _pictureService.GetPicturesByProductId(product.Id);
                            foreach (var existingPicture in existingPictures)
                            {
                                var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                                //picture binary after validation (like in database)
                                var validatedPictureBinary = _pictureService.ValidatePicture(newPictureBinary, mimeType);
                                if (existingBinary.SequenceEqual(validatedPictureBinary) || existingBinary.SequenceEqual(newPictureBinary))
                                {
                                    //the same picture content
                                    pictureAlreadyExists = true;
                                    break;
                                }
                            }
                        }

                        if (!pictureAlreadyExists)
                        {
                            var newPicture = _pictureService.InsertPicture(newPictureBinary, mimeType , _pictureService.GetPictureSeName(name));
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
                    }

                    //update "HasTierPrices" and "HasDiscountsApplied" properties
                    _productService.UpdateHasTierPricesProperty(product);
                    _productService.UpdateHasDiscountsApplied(product);



                    //next product
                    iRow++;
                }
            }
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

        #endregion
    }
}
