using System;
using System.IO;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Media;
using OfficeOpenXml;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager : IImportManager
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;

        public ImportManager(IProductService productService, ICategoryService categoryService,
            IManufacturerService manufacturerService, IPictureService pictureService)
        {
            this._productService = productService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._pictureService = pictureService;
        }

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

        /// <summary>
        /// Import products from XLSX file
        /// </summary>
        /// <param name="filePath">Excel file path</param>
        public virtual void ImportProductsFromXlsx(string filePath)
        {
            
            var newFile = new FileInfo(filePath);
            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(newFile))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");
                
                //the columns
                var properties = new string[]
                {
                    "Name",
                    "ShortDescription",
                    "FullDescription",
                    "ProductTemplateId",
                    "ShowOnHomePage",
                    "MetaKeywords",
                    "MetaDescription",
                    "MetaTitle",
                    "SeName",
                    "AllowCustomerReviews",
                    "Published",
                    "ProductVariantName",
                    "SKU",
                    "ManufacturerPartNumber",
                    "Gtin",
                    "IsGiftCard",
                    "GiftCardTypeId",
                    "RequireOtherProducts",
                    "RequiredProductVariantIds",
                    "AutomaticallyAddRequiredProductVariants",
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

                    string name = worksheet.Cells[iRow, GetColumnIndex(properties, "Name")].Value as string;
                    string shortDescription = worksheet.Cells[iRow, GetColumnIndex(properties, "ShortDescription")].Value as string;
                    string fullDescription = worksheet.Cells[iRow, GetColumnIndex(properties, "FullDescription")].Value as string;
                    int productTemplateId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "ProductTemplateId")].Value);
                    bool showOnHomePage = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "ShowOnHomePage")].Value);
                    string metaKeywords = worksheet.Cells[iRow, GetColumnIndex(properties, "MetaKeywords")].Value as string;
                    string metaDescription = worksheet.Cells[iRow, GetColumnIndex(properties, "MetaDescription")].Value as string;
                    string metaTitle = worksheet.Cells[iRow, GetColumnIndex(properties, "MetaTitle")].Value as string;
                    string seName = worksheet.Cells[iRow, GetColumnIndex(properties, "SeName")].Value as string;
                    bool allowCustomerReviews = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AllowCustomerReviews")].Value);
                    bool published = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "Published")].Value);
                    string productVariantName = worksheet.Cells[iRow, GetColumnIndex(properties, "ProductVariantName")].Value as string;
                    string sku = worksheet.Cells[iRow, GetColumnIndex(properties, "SKU")].Value as string;
                    string manufacturerPartNumber = worksheet.Cells[iRow, GetColumnIndex(properties, "ManufacturerPartNumber")].Value as string;
                    string gtin = worksheet.Cells[iRow, GetColumnIndex(properties, "Gtin")].Value as string;
                    bool isGiftCard = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "IsGiftCard")].Value);
                    int giftCardTypeId = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "GiftCardTypeId")].Value);
                    bool requireOtherProducts = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "RequireOtherProducts")].Value);
                    string requiredProductVariantIds = worksheet.Cells[iRow, GetColumnIndex(properties, "RequiredProductVariantIds")].Value as string;
                    bool automaticallyAddRequiredProductVariants = Convert.ToBoolean(worksheet.Cells[iRow, GetColumnIndex(properties, "AutomaticallyAddRequiredProductVariants")].Value);
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



                    var productVariant = _productService.GetProductVariantBySku(sku);
                    if (productVariant != null)
                    {
                        var product = productVariant.Product;
                        product.Name = name;
                        product.ShortDescription = shortDescription;
                        product.FullDescription = fullDescription;
                        product.ProductTemplateId = productTemplateId;
                        product.ShowOnHomePage = showOnHomePage;
                        product.MetaKeywords = metaKeywords;
                        product.MetaDescription = metaDescription;
                        product.MetaTitle = metaTitle;
                        product.SeName = seName;
                        product.AllowCustomerReviews = allowCustomerReviews;
                        product.Published = published;
                        product.CreatedOnUtc = createdOnUtc;
                        product.UpdatedOnUtc = DateTime.UtcNow;

                        _productService.UpdateProduct(product);

                        productVariant.Name = productVariantName;
                        productVariant.Sku = sku;
                        productVariant.ManufacturerPartNumber = manufacturerPartNumber;
                        productVariant.Gtin = gtin;
                        productVariant.IsGiftCard = isGiftCard;
                        productVariant.GiftCardTypeId = giftCardTypeId;
                        productVariant.RequireOtherProducts = requireOtherProducts;
                        productVariant.RequiredProductVariantIds = requiredProductVariantIds;
                        productVariant.AutomaticallyAddRequiredProductVariants = automaticallyAddRequiredProductVariants;
                        productVariant.IsDownload = isDownload;
                        productVariant.DownloadId = downloadId;
                        productVariant.UnlimitedDownloads = unlimitedDownloads;
                        productVariant.MaxNumberOfDownloads = maxNumberOfDownloads;
                        productVariant.DownloadActivationTypeId = downloadActivationTypeId;
                        productVariant.HasSampleDownload = hasSampleDownload;
                        productVariant.SampleDownloadId = sampleDownloadId;
                        productVariant.HasUserAgreement = hasUserAgreement;
                        productVariant.UserAgreementText = userAgreementText;
                        productVariant.IsRecurring = isRecurring;
                        productVariant.RecurringCycleLength = recurringCycleLength;
                        productVariant.RecurringCyclePeriodId = recurringCyclePeriodId;
                        productVariant.RecurringTotalCycles = recurringTotalCycles;
                        productVariant.IsShipEnabled = isShipEnabled;
                        productVariant.IsFreeShipping = isFreeShipping;
                        productVariant.AdditionalShippingCharge = additionalShippingCharge;
                        productVariant.IsTaxExempt = isTaxExempt;
                        productVariant.TaxCategoryId = taxCategoryId;
                        productVariant.ManageInventoryMethodId = manageInventoryMethodId;
                        productVariant.StockQuantity = stockQuantity;
                        productVariant.DisplayStockAvailability = displayStockAvailability;
                        productVariant.DisplayStockQuantity = displayStockQuantity;
                        productVariant.MinStockQuantity = minStockQuantity;
                        productVariant.LowStockActivityId = lowStockActivityId;
                        productVariant.NotifyAdminForQuantityBelow = notifyAdminForQuantityBelow;
                        productVariant.BackorderModeId = backorderModeId;
                        productVariant.AllowBackInStockSubscriptions = allowBackInStockSubscriptions;
                        productVariant.OrderMinimumQuantity = orderMinimumQuantity;
                        productVariant.OrderMaximumQuantity = orderMaximumQuantity;
                        productVariant.AllowedQuantities = allowedQuantities;
                        productVariant.DisableBuyButton = disableBuyButton;
                        productVariant.DisableWishlistButton = disableWishlistButton;
                        productVariant.CallForPrice = callForPrice;
                        productVariant.Price = price;
                        productVariant.OldPrice = oldPrice;
                        productVariant.ProductCost = productCost;
                        productVariant.SpecialPrice = specialPrice;
                        productVariant.SpecialPriceStartDateTimeUtc = specialPriceStartDateTimeUtc;
                        productVariant.SpecialPriceEndDateTimeUtc = specialPriceEndDateTimeUtc;
                        productVariant.CustomerEntersPrice = customerEntersPrice;
                        productVariant.MinimumCustomerEnteredPrice = minimumCustomerEnteredPrice;
                        productVariant.MaximumCustomerEnteredPrice = maximumCustomerEnteredPrice;
                        productVariant.Weight = weight;
                        productVariant.Length = length;
                        productVariant.Width = width;
                        productVariant.Height = height;
                        productVariant.Published = published;
                        productVariant.CreatedOnUtc = createdOnUtc;
                        productVariant.UpdatedOnUtc = DateTime.UtcNow;

                        _productService.UpdateProductVariant(productVariant);
                    }
                    else
                    {
                        var product = new Product()
                        {
                            Name = name,
                            ShortDescription = shortDescription,
                            FullDescription = fullDescription,
                            ShowOnHomePage = showOnHomePage,
                            MetaKeywords = metaKeywords,
                            MetaDescription = metaDescription,
                            MetaTitle = metaTitle,
                            SeName = seName,
                            AllowCustomerReviews = allowCustomerReviews,
                            Published = published,
                            CreatedOnUtc = createdOnUtc,
                            UpdatedOnUtc = DateTime.UtcNow
                        };
                        _productService.InsertProduct(product);

                        productVariant = new ProductVariant()
                        {
                            ProductId = product.Id,
                            Name = productVariantName,
                            Sku = sku,
                            ManufacturerPartNumber = manufacturerPartNumber,
                            Gtin = gtin,
                            IsGiftCard = isGiftCard,
                            GiftCardTypeId = giftCardTypeId,
                            RequireOtherProducts = requireOtherProducts,
                            RequiredProductVariantIds = requiredProductVariantIds,
                            AutomaticallyAddRequiredProductVariants = automaticallyAddRequiredProductVariants,
                            IsDownload = isDownload,
                            DownloadId = downloadId,
                            UnlimitedDownloads = unlimitedDownloads,
                            MaxNumberOfDownloads = maxNumberOfDownloads,
                            DownloadActivationTypeId = downloadActivationTypeId,
                            HasSampleDownload = hasSampleDownload,
                            SampleDownloadId = sampleDownloadId,
                            HasUserAgreement = hasUserAgreement,
                            UserAgreementText = userAgreementText,
                            IsRecurring = isRecurring,
                            RecurringCycleLength = recurringCycleLength,
                            RecurringCyclePeriodId = recurringCyclePeriodId,
                            RecurringTotalCycles = recurringTotalCycles,
                            IsShipEnabled = isShipEnabled,
                            IsFreeShipping = isFreeShipping,
                            AdditionalShippingCharge = additionalShippingCharge,
                            IsTaxExempt = isTaxExempt,
                            TaxCategoryId = taxCategoryId,
                            ManageInventoryMethodId = manageInventoryMethodId,
                            StockQuantity = stockQuantity,
                            DisplayStockAvailability = displayStockAvailability,
                            DisplayStockQuantity = displayStockQuantity,
                            MinStockQuantity = minStockQuantity,
                            LowStockActivityId = lowStockActivityId,
                            NotifyAdminForQuantityBelow = notifyAdminForQuantityBelow,
                            BackorderModeId = backorderModeId,
                            AllowBackInStockSubscriptions = allowBackInStockSubscriptions,
                            OrderMinimumQuantity = orderMinimumQuantity,
                            OrderMaximumQuantity = orderMaximumQuantity,
                            AllowedQuantities = allowedQuantities,
                            DisableBuyButton = disableBuyButton,
                            CallForPrice = callForPrice,
                            Price = price,
                            OldPrice = oldPrice,
                            ProductCost = productCost,
                            SpecialPrice = specialPrice,
                            SpecialPriceStartDateTimeUtc = specialPriceStartDateTimeUtc,
                            SpecialPriceEndDateTimeUtc = specialPriceEndDateTimeUtc,
                            CustomerEntersPrice = customerEntersPrice,
                            MinimumCustomerEnteredPrice = minimumCustomerEnteredPrice,
                            MaximumCustomerEnteredPrice = maximumCustomerEnteredPrice,
                            Weight = weight,
                            Length = length,
                            Width = width,
                            Height = height,
                            Published = published,
                            CreatedOnUtc = createdOnUtc,
                            UpdatedOnUtc = DateTime.UtcNow
                        };

                        _productService.InsertProductVariant(productVariant);
                    }

                    //category mappings
                    if (!String.IsNullOrEmpty(categoryIds))
                    {
                        foreach (var id in categoryIds.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x.Trim())))
                        {
                            if (productVariant.Product.ProductCategories.Where(x => x.CategoryId == id).FirstOrDefault() == null)
                            {
                                //ensure that category exists
                                var category = _categoryService.GetCategoryById(id);
                                if (category != null)
                                {
                                    var productCategory = new ProductCategory()
                                    {
                                        ProductId = productVariant.Product.Id,
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
                            if (productVariant.Product.ProductManufacturers.Where(x => x.ManufacturerId == id).FirstOrDefault() == null)
                            {
                                //ensure that manufacturer exists
                                var manufacturer = _manufacturerService.GetManufacturerById(id);
                                if (manufacturer != null)
                                {
                                    var productManufacturer = new ProductManufacturer()
                                    {
                                        ProductId = productVariant.Product.Id,
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
                    foreach (var picture in new string[] { picture1, picture2, picture3 })
                    {
                        if (String.IsNullOrEmpty(picture))
                            continue;

                        productVariant.Product.ProductPictures.Add(new ProductPicture()
                        {
                            Picture = _pictureService.InsertPicture(File.ReadAllBytes(picture), "image/jpeg", _pictureService.GetPictureSeName(name), true),
                            DisplayOrder = 1,
                        });
                        _productService.UpdateProduct(productVariant.Product);
                    }

                    //update "HasTierPrices" and "HasDiscountsApplied" properties
                    _productService.UpdateHasTierPricesProperty(productVariant);
                    _productService.UpdateHasDiscountsApplied(productVariant);



                    //next product
                    iRow++;
                }
            }
        }
    }
}
