using System;
using System.IO;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Media;
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

        #endregion

        #region Ctor

        public ImportManager(IProductService productService, ICategoryService categoryService,
            IManufacturerService manufacturerService, IPictureService pictureService,
            IUrlRecordService urlRecordService)
        {
            this._productService = productService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._pictureService = pictureService;
            this._urlRecordService = urlRecordService;
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
                var properties = new string[]
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
                    "IsShipEnabled",
                    "IsFreeShipping",
                    "AdditionalShippingCharge",
                    "DeliveryDateId",
                    "WarehouseId",
                    "IsTaxExempt",
                    "TaxCategoryId",
                    "ManageInventoryMethodId",
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
                    "Weight",
                    "Length",
                    "Width",
                    "Height",
                    "CreatedOnUtc",
                    "CategoryIds",
                    "ManufacturerIds",
                    "Picture1",
                    "Picture2",
                    "Picture3",
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
                    string name = worksheet.Cells[iRow, GetColumnIndex(properties, "Name")].Value as string;
                    string shortDescription = worksheet.Cells[iRow, GetColumnIndex(properties, "ShortDescription")].Value as string;
                    string fullDescription = worksheet.Cells[iRow, GetColumnIndex(properties, "FullDescription")].Value as string;
                    int vendorId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "VendorId")].Value);
                    int productTemplateId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "ProductTemplateId")].Value);
                    bool showOnHomePage = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "ShowOnHomePage")].Value);
                    string metaKeywords = worksheet.Cells[iRow, GetColumnIndex(properties, "MetaKeywords")].Value as string;
                    string metaDescription = worksheet.Cells[iRow, GetColumnIndex(properties, "MetaDescription")].Value as string;
                    string metaTitle = worksheet.Cells[iRow, GetColumnIndex(properties, "MetaTitle")].Value as string;
                    string seName = worksheet.Cells[iRow, GetColumnIndex(properties, "SeName")].Value as string;
                    bool allowCustomerReviews = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AllowCustomerReviews")].Value);
                    bool published = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "Published")].Value);
                    string sku = worksheet.Cells[iRow, GetColumnIndex(properties, "SKU")].Value as string;
                    string manufacturerPartNumber = worksheet.Cells[iRow, GetColumnIndex(properties, "ManufacturerPartNumber")].Value as string;
                    string gtin = worksheet.Cells[iRow, GetColumnIndex(properties, "Gtin")].Value as string;
                    bool isGiftCard = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsGiftCard")].Value);
                    int giftCardTypeId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "GiftCardTypeId")].Value);
                    bool requireOtherProducts = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "RequireOtherProducts")].Value);
                    string requiredProductIds = worksheet.Cells[iRow, GetColumnIndex(properties, "RequiredProductIds")].Value as string;
                    bool automaticallyAddRequiredProducts = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AutomaticallyAddRequiredProducts")].Value);
                    bool isDownload = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsDownload")].Value);
                    int downloadId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "DownloadId")].Value);
                    bool unlimitedDownloads = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "UnlimitedDownloads")].Value);
                    int maxNumberOfDownloads = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "MaxNumberOfDownloads")].Value);
                    int downloadActivationTypeId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "DownloadActivationTypeId")].Value);
                    bool hasSampleDownload = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "HasSampleDownload")].Value);
                    int sampleDownloadId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "SampleDownloadId")].Value);
                    bool hasUserAgreement = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "HasUserAgreement")].Value);
                    string userAgreementText = worksheet.Cells[iRow, GetColumnIndex(properties, "UserAgreementText")].Value as string;
                    bool isRecurring = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsRecurring")].Value);
                    int recurringCycleLength = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "RecurringCycleLength")].Value);
                    int recurringCyclePeriodId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "RecurringCyclePeriodId")].Value);
                    int recurringTotalCycles = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "RecurringTotalCycles")].Value);
                    bool isShipEnabled = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsShipEnabled")].Value);
                    bool isFreeShipping = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsFreeShipping")].Value);
                    decimal additionalShippingCharge = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "AdditionalShippingCharge")].Value);
                    int deliveryDateId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "DeliveryDateId")].Value);
                    int warehouseId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "WarehouseId")].Value);
                    bool isTaxExempt = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsTaxExempt")].Value);
                    int taxCategoryId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "TaxCategoryId")].Value);
                    int manageInventoryMethodId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "ManageInventoryMethodId")].Value);
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
                    string allowedQuantities = worksheet.Cells[iRow, GetColumnIndex(properties, "AllowedQuantities")].Value as string;
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
                    decimal weight = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Weight")].Value);
                    decimal length = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Length")].Value);
                    decimal width = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Width")].Value);
                    decimal height = Convert.ToDecimal(worksheet.Cells[iRow, GetColumnIndex(properties, "Height")].Value);
                    DateTime createdOnUtc = DateTime.FromOADate(Convert.ToDouble(worksheet.Cells[iRow, GetColumnIndex(properties, "CreatedOnUtc")].Value));
                    string categoryIds = worksheet.Cells[iRow, GetColumnIndex(properties, "CategoryIds")].Value as string;
                    string manufacturerIds = worksheet.Cells[iRow, GetColumnIndex(properties, "ManufacturerIds")].Value as string;
                    string picture1 = worksheet.Cells[iRow, GetColumnIndex(properties, "Picture1")].Value as string;
                    string picture2 = worksheet.Cells[iRow, GetColumnIndex(properties, "Picture2")].Value as string;
                    string picture3 = worksheet.Cells[iRow, GetColumnIndex(properties, "Picture3")].Value as string;



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
                    product.IsShipEnabled = isShipEnabled;
                    product.IsFreeShipping = isFreeShipping;
                    product.AdditionalShippingCharge = additionalShippingCharge;
                    product.DeliveryDateId = deliveryDateId;
                    product.WarehouseId = warehouseId;
                    product.IsTaxExempt = isTaxExempt;
                    product.TaxCategoryId = taxCategoryId;
                    product.ManageInventoryMethodId = manageInventoryMethodId;
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
                    product.Weight = weight;
                    product.Length = length;
                    product.Width = width;
                    product.Height = height;
                    product.Published = published;
                    product.CreatedOnUtc = createdOnUtc;
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
                        foreach (var id in categoryIds.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x.Trim())))
                        {
                            if (product.ProductCategories.FirstOrDefault(x => x.CategoryId == id) == null)
                            {
                                //ensure that category exists
                                var category = _categoryService.GetCategoryById(id);
                                if (category != null)
                                {
                                    var productCategory = new ProductCategory()
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
                        foreach (var id in manufacturerIds.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x.Trim())))
                        {
                            if (product.ProductManufacturers.FirstOrDefault(x => x.ManufacturerId == id) == null)
                            {
                                //ensure that manufacturer exists
                                var manufacturer = _manufacturerService.GetManufacturerById(id);
                                if (manufacturer != null)
                                {
                                    var productManufacturer = new ProductManufacturer()
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
                    foreach (var picturePath in new string[] { picture1, picture2, picture3 })
                    {
                        if (String.IsNullOrEmpty(picturePath))
                            continue;

                        var newPictureBinary = File.ReadAllBytes(picturePath);
                        var pictureAlreadyExists = false;
                        if (!newProduct)
                        {
                            //compare with existing product pictures
                            var existingPictures = _pictureService.GetPicturesByProductId(product.Id);
                            foreach (var existingPicture in existingPictures)
                            {
                                var existingBinary = _pictureService.LoadPictureBinary(existingPicture);
                                if (existingBinary.SequenceEqual(newPictureBinary))
                                {
                                    //the same picture content
                                    pictureAlreadyExists = true;
                                    break;
                                }
                            }
                        }

                        if (!pictureAlreadyExists)
                        {
                            product.ProductPictures.Add(new ProductPicture()
                            {
                                Picture = _pictureService.InsertPicture(newPictureBinary, "image/jpeg", _pictureService.GetPictureSeName(name), true),
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

        #endregion
    }
}
