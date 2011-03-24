//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Data;
using Nop.Core.Domain.Tax;
using Nop.Services.Customers;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager : IImportManager
    {
        IProductService _productService;

        public ImportManager(IProductService productService)
        {
            this._productService = productService;
        }


        /// <summary>
        /// Import products from XLS file
        /// </summary>
        /// <param name="filePath">Excel file path</param>
        public void ImportProductsFromXls(string filePath)
        {
            using (var excelHelper = new ExcelHelper(filePath))
            {
                excelHelper.Hdr = "YES";
                excelHelper.Imex = "1";

                DataTable dt = excelHelper.ReadTable("Products");
                foreach (DataRow dr in dt.Rows)
                {
                    string name = dr["Name"].ToString();
                    string shortDescription = dr["ShortDescription"].ToString();
                    string fullDescription = dr["FullDescription"].ToString();
                    bool showOnHomePage = Convert.ToBoolean(dr["ShowOnHomePage"]);
                    string metaKeywords = dr["MetaKeywords"].ToString();
                    string metaDescription = dr["MetaDescription"].ToString();
                    string metaTitle = dr["MetaTitle"].ToString();
                    bool allowCustomerReviews = Convert.ToBoolean(dr["AllowCustomerReviews"]);
                    bool allowCustomerRatings = Convert.ToBoolean(dr["AllowCustomerRatings"]);
                    bool published = Convert.ToBoolean(dr["Published"]);
                    string sku = dr["SKU"].ToString();
                    string manufacturerPartNumber = dr["ManufacturerPartNumber"].ToString();
                    bool isGiftCard = Convert.ToBoolean(dr["IsGiftCard"]);
                    int giftCardTypeId = Convert.ToInt32(dr["GiftCardTypeId"]);
                    bool isDownload = Convert.ToBoolean(dr["IsDownload"]);
                    int downloadId = Convert.ToInt32(dr["DownloadId"]);
                    bool unlimitedDownloads = Convert.ToBoolean(dr["UnlimitedDownloads"]);
                    int maxNumberOfDownloads = Convert.ToInt32(dr["MaxNumberOfDownloads"]);
                    bool hasSampleDownload = Convert.ToBoolean(dr["HasSampleDownload"]);
                    int downloadActivationTypeId = Convert.ToInt32(dr["DownloadActivationTypeId"]);
                    int sampleDownloadId = Convert.ToInt32(dr["SampleDownloadId"]);
                    bool hasUserAgreement = Convert.ToBoolean(dr["HasUserAgreement"]);
                    string userAgreementText = dr["UserAgreementText"].ToString();
                    bool isRecurring = Convert.ToBoolean(dr["IsRecurring"]);
                    int recurringCycleLength = Convert.ToInt32(dr["RecurringCycleLength"]);
                    int recurringCyclePeriodId = Convert.ToInt32(dr["RecurringCyclePeriodId"]);
                    int recurringTotalCycles = Convert.ToInt32(dr["RecurringTotalCycles"]);
                    bool isShipEnabled = Convert.ToBoolean(dr["IsShipEnabled"]);
                    bool isFreeShipping = Convert.ToBoolean(dr["IsFreeShipping"]);
                    decimal additionalShippingCharge = Convert.ToDecimal(dr["AdditionalShippingCharge"]);
                    bool isTaxExempt = Convert.ToBoolean(dr["IsTaxExempt"]);
                    int taxCategoryId = Convert.ToInt32(dr["TaxCategoryId"]);
                    int manageInventoryMethodId = Convert.ToInt32(dr["ManageInventoryMethodId"]);
                    int stockQuantity = Convert.ToInt32(dr["StockQuantity"]);
                    bool displayStockAvailability = Convert.ToBoolean(dr["DisplayStockAvailability"]);
                    bool displayStockQuantity = Convert.ToBoolean(dr["DisplayStockQuantity"]);
                    int minStockQuantity = Convert.ToInt32(dr["MinStockQuantity"]);
                    int lowStockActivityId = Convert.ToInt32(dr["LowStockActivityId"]);
                    int notifyAdminForQuantityBelow = Convert.ToInt32(dr["NotifyAdminForQuantityBelow"]);
                    int backorderModeId = Convert.ToInt32(dr["BackorderModeId"]);
                    int orderMinimumQuantity = Convert.ToInt32(dr["OrderMinimumQuantity"]);
                    int orderMaximumQuantity = Convert.ToInt32(dr["OrderMaximumQuantity"]);
                    bool disableBuyButton = Convert.ToBoolean(dr["DisableBuyButton"]);
                    bool callForPrice = Convert.ToBoolean(dr["CallForPrice"]);
                    decimal price = Convert.ToDecimal(dr["Price"]);
                    decimal oldPrice = Convert.ToDecimal(dr["OldPrice"]);
                    decimal productCost = Convert.ToDecimal(dr["ProductCost"]);
                    bool customerEntersPrice = Convert.ToBoolean(dr["CustomerEntersPrice"]);
                    decimal minimumCustomerEnteredPrice = Convert.ToDecimal(dr["MinimumCustomerEnteredPrice"]);
                    decimal maximumCustomerEnteredPrice = Convert.ToDecimal(dr["MaximumCustomerEnteredPrice"]);
                    decimal weight = Convert.ToDecimal(dr["Weight"]);
                    decimal length = Convert.ToDecimal(dr["Length"]);
                    decimal width = Convert.ToDecimal(dr["Width"]);
                    decimal height = Convert.ToDecimal(dr["Height"]);
                    DateTime createdOnUtc = DateTime.FromOADate(Convert.ToDouble(dr["CreatedOnUtc"]));

                    var productVariant = _productService.GetProductVariantBySku(sku);
                    if (productVariant != null)
                    {
                        var product = _productService.GetProductById(productVariant.ProductId);
                        product.Name = name;
                        product.ShortDescription = shortDescription;
                        product.FullDescription = fullDescription;
                        product.ShowOnHomePage = showOnHomePage;
                        product.MetaKeywords = metaKeywords;
                        product.MetaDescription = metaDescription;
                        product.MetaTitle = metaTitle;
                        product.AllowCustomerReviews = allowCustomerReviews;
                        product.AllowCustomerRatings = allowCustomerRatings;
                        product.Published = published;
                        product.CreatedOnUtc = createdOnUtc;
                        product.UpdatedOnUtc = DateTime.UtcNow;

                        _productService.UpdateProduct(product);
                        
                        productVariant.Sku = sku;
                        productVariant.ManufacturerPartNumber = manufacturerPartNumber;
                        productVariant.IsGiftCard = isGiftCard;
                        productVariant.GiftCardTypeId = giftCardTypeId;
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
                        productVariant.OrderMinimumQuantity = orderMinimumQuantity;
                        productVariant.OrderMaximumQuantity = orderMaximumQuantity;
                        productVariant.DisableBuyButton = disableBuyButton;
                        productVariant.CallForPrice = callForPrice;
                        productVariant.Price = price;
                        productVariant.OldPrice = oldPrice;
                        productVariant.ProductCost = productCost;
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
                            AllowCustomerReviews = allowCustomerReviews,
                            AllowCustomerRatings = allowCustomerRatings,
                            Published = published,
                            CreatedOnUtc = createdOnUtc,
                            UpdatedOnUtc = DateTime.UtcNow
                        };
                        _productService.InsertProduct(product);

                        productVariant = new ProductVariant()
                        {
                            ProductId = product.Id,
                            Sku = sku,
                            ManufacturerPartNumber = manufacturerPartNumber,
                            IsGiftCard = isGiftCard,
                            GiftCardTypeId = giftCardTypeId,
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
                            OrderMinimumQuantity = orderMinimumQuantity,
                            OrderMaximumQuantity = orderMaximumQuantity,
                            DisableBuyButton = disableBuyButton,
                            CallForPrice = callForPrice,
                            Price = price,
                            OldPrice = oldPrice,
                            ProductCost = productCost,
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
                }
            }
        }
    }
}
