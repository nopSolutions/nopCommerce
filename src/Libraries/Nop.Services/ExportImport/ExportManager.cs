using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Services.Messages;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
public partial class ExportManager : IExportManager
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly StoreInformationSettings _storeInformationSettings;

        #endregion

        #region Ctor

        public ExportManager(ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IProductService productService,
            IPictureService pictureService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            StoreInformationSettings storeInformationSettings)
        {
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._pictureService = pictureService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._storeInformationSettings = storeInformationSettings;
        }

        #endregion

        #region Utilities

        protected virtual void WriteCategories(XmlWriter xmlWriter, int parentCategoryId)
        {
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId, true);
            if (categories != null && categories.Count > 0)
            {
                foreach (var category in categories)
                {
                    xmlWriter.WriteStartElement("Category");
                    xmlWriter.WriteElementString("Id", null, category.Id.ToString());
                    xmlWriter.WriteElementString("Name", null, category.Name);
                    xmlWriter.WriteElementString("Description", null, category.Description);
                    xmlWriter.WriteElementString("CategoryTemplateId", null, category.CategoryTemplateId.ToString());
                    xmlWriter.WriteElementString("MetaKeywords", null, category.MetaKeywords);
                    xmlWriter.WriteElementString("MetaDescription", null, category.MetaDescription);
                    xmlWriter.WriteElementString("MetaTitle", null, category.MetaTitle);
                    xmlWriter.WriteElementString("SeName", null, category.SeName);
                    xmlWriter.WriteElementString("ParentCategoryId", null, category.ParentCategoryId.ToString());
                    xmlWriter.WriteElementString("PictureId", null, category.PictureId.ToString());
                    xmlWriter.WriteElementString("PageSize", null, category.PageSize.ToString());
                    xmlWriter.WriteElementString("AllowCustomersToSelectPageSize", null, category.AllowCustomersToSelectPageSize.ToString());
                    xmlWriter.WriteElementString("PageSizeOptions", null, category.PageSizeOptions);
                    xmlWriter.WriteElementString("PriceRanges", null, category.PriceRanges);
                    xmlWriter.WriteElementString("ShowOnHomePage", null, category.ShowOnHomePage.ToString());
                    xmlWriter.WriteElementString("Published", null, category.Published.ToString());
                    xmlWriter.WriteElementString("Deleted", null, category.Deleted.ToString());
                    xmlWriter.WriteElementString("DisplayOrder", null, category.DisplayOrder.ToString());
                    xmlWriter.WriteElementString("CreatedOnUtc", null, category.CreatedOnUtc.ToString());
                    xmlWriter.WriteElementString("UpdatedOnUtc", null, category.UpdatedOnUtc.ToString());


                    xmlWriter.WriteStartElement("Products");
                    var productCategories = _categoryService.GetProductCategoriesByCategoryId(category.Id, 0, int.MaxValue, true);
                    foreach (var productCategory in productCategories)
                    {
                        var product = productCategory.Product;
                        if (product != null && !product.Deleted)
                        {
                            xmlWriter.WriteStartElement("ProductCategory");
                            xmlWriter.WriteElementString("ProductCategoryId", null, productCategory.Id.ToString());
                            xmlWriter.WriteElementString("ProductId", null, productCategory.ProductId.ToString());
                            xmlWriter.WriteElementString("IsFeaturedProduct", null, productCategory.IsFeaturedProduct.ToString());
                            xmlWriter.WriteElementString("DisplayOrder", null, productCategory.DisplayOrder.ToString());
                            xmlWriter.WriteEndElement();
                        }
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("SubCategories");
                    WriteCategories(xmlWriter, category.Id);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Export manufacturer list to xml
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

                xmlWriter.WriteElementString("ManufacturerId", null, manufacturer.Id.ToString());
                xmlWriter.WriteElementString("Name", null, manufacturer.Name);
                xmlWriter.WriteElementString("Description", null, manufacturer.Description);
                xmlWriter.WriteElementString("ManufacturerTemplateId", null, manufacturer.ManufacturerTemplateId.ToString());
                xmlWriter.WriteElementString("MetaKeywords", null, manufacturer.MetaKeywords);
                xmlWriter.WriteElementString("MetaDescription", null, manufacturer.MetaDescription);
                xmlWriter.WriteElementString("MetaTitle", null, manufacturer.MetaTitle);
                xmlWriter.WriteElementString("SEName", null, manufacturer.SeName);
                xmlWriter.WriteElementString("PictureId", null, manufacturer.PictureId.ToString());
                xmlWriter.WriteElementString("PageSize", null, manufacturer.PageSize.ToString());
                xmlWriter.WriteElementString("AllowCustomersToSelectPageSize", null, manufacturer.AllowCustomersToSelectPageSize.ToString());
                xmlWriter.WriteElementString("PageSizeOptions", null, manufacturer.PageSizeOptions);
                xmlWriter.WriteElementString("PriceRanges", null, manufacturer.PriceRanges);
                xmlWriter.WriteElementString("Published", null, manufacturer.Published.ToString());
                xmlWriter.WriteElementString("Deleted", null, manufacturer.Deleted.ToString());
                xmlWriter.WriteElementString("DisplayOrder", null, manufacturer.DisplayOrder.ToString());
                xmlWriter.WriteElementString("CreatedOnUtc", null, manufacturer.CreatedOnUtc.ToString());
                xmlWriter.WriteElementString("UpdatedOnUtc", null, manufacturer.UpdatedOnUtc.ToString());

                xmlWriter.WriteStartElement("Products");
                var productManufacturers = _manufacturerService.GetProductManufacturersByManufacturerId(manufacturer.Id, 0, int.MaxValue, true);
                if (productManufacturers != null)
                {
                    foreach (var productManufacturer in productManufacturers)
                    {
                        var product = productManufacturer.Product;
                        if (product != null && !product.Deleted)
                        {
                            xmlWriter.WriteStartElement("ProductManufacturer");
                            xmlWriter.WriteElementString("ProductManufacturerId", null, productManufacturer.Id.ToString());
                            xmlWriter.WriteElementString("ProductId", null, productManufacturer.ProductId.ToString());
                            xmlWriter.WriteElementString("IsFeaturedProduct", null, productManufacturer.IsFeaturedProduct.ToString());
                            xmlWriter.WriteElementString("DisplayOrder", null, productManufacturer.DisplayOrder.ToString());
                            xmlWriter.WriteEndElement();
                        }
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
        /// Export category list to xml
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
        /// Export product list to xml
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

                xmlWriter.WriteElementString("ProductId", null, product.Id.ToString());
                xmlWriter.WriteElementString("Name", null, product.Name);
                xmlWriter.WriteElementString("ShortDescription", null, product.ShortDescription);
                xmlWriter.WriteElementString("FullDescription", null, product.FullDescription);
                xmlWriter.WriteElementString("AdminComment", null, product.AdminComment);
                xmlWriter.WriteElementString("ProductTemplateId", null, product.ProductTemplateId.ToString());
                xmlWriter.WriteElementString("ShowOnHomePage", null, product.ShowOnHomePage.ToString());
                xmlWriter.WriteElementString("MetaKeywords", null, product.MetaKeywords);
                xmlWriter.WriteElementString("MetaDescription", null, product.MetaDescription);
                xmlWriter.WriteElementString("MetaTitle", null, product.MetaTitle);
                xmlWriter.WriteElementString("SEName", null, product.SeName);
                xmlWriter.WriteElementString("AllowCustomerReviews", null, product.AllowCustomerReviews.ToString());
                xmlWriter.WriteElementString("Published", null, product.Published.ToString());
                xmlWriter.WriteElementString("CreatedOnUtc", null, product.CreatedOnUtc.ToString());
                xmlWriter.WriteElementString("UpdatedOnUtc", null, product.UpdatedOnUtc.ToString());

                xmlWriter.WriteStartElement("ProductVariants");
                var productVariants = _productService.GetProductVariantsByProductId(product.Id, true);
                if (productVariants != null)
                {
                    foreach (var productVariant in productVariants)
                    {
                        xmlWriter.WriteStartElement("ProductVariant");
                        xmlWriter.WriteElementString("ProductVariantId", null, productVariant.Id.ToString());
                        xmlWriter.WriteElementString("ProductId", null, productVariant.ProductId.ToString());
                        xmlWriter.WriteElementString("Name", null, productVariant.Name);
                        xmlWriter.WriteElementString("SKU", null, productVariant.Sku);
                        xmlWriter.WriteElementString("Description", null, productVariant.Description);
                        xmlWriter.WriteElementString("AdminComment", null, productVariant.AdminComment);
                        xmlWriter.WriteElementString("ManufacturerPartNumber", null, productVariant.ManufacturerPartNumber);
                        xmlWriter.WriteElementString("Gtin", null, productVariant.Gtin);
                        xmlWriter.WriteElementString("IsGiftCard", null, productVariant.IsGiftCard.ToString());
                        xmlWriter.WriteElementString("GiftCardType", null, productVariant.GiftCardType.ToString());
                        xmlWriter.WriteElementString("RequireOtherProducts", null, productVariant.RequireOtherProducts.ToString());
                        xmlWriter.WriteElementString("RequiredProductVariantIds", null, productVariant.RequiredProductVariantIds);
                        xmlWriter.WriteElementString("AutomaticallyAddRequiredProductVariants", null, productVariant.AutomaticallyAddRequiredProductVariants.ToString());
                        xmlWriter.WriteElementString("IsDownload", null, productVariant.IsDownload.ToString());
                        xmlWriter.WriteElementString("DownloadId", null, productVariant.DownloadId.ToString());
                        xmlWriter.WriteElementString("UnlimitedDownloads", null, productVariant.UnlimitedDownloads.ToString());
                        xmlWriter.WriteElementString("MaxNumberOfDownloads", null, productVariant.MaxNumberOfDownloads.ToString());
                        if (productVariant.DownloadExpirationDays.HasValue)
                            xmlWriter.WriteElementString("DownloadExpirationDays", null, productVariant.DownloadExpirationDays.ToString());
                        else
                            xmlWriter.WriteElementString("DownloadExpirationDays", null, string.Empty);
                        xmlWriter.WriteElementString("DownloadActivationType", null, productVariant.DownloadActivationType.ToString());
                        xmlWriter.WriteElementString("HasSampleDownload", null, productVariant.HasSampleDownload.ToString());
                        xmlWriter.WriteElementString("SampleDownloadId", null, productVariant.SampleDownloadId.ToString());
                        xmlWriter.WriteElementString("HasUserAgreement", null, productVariant.HasUserAgreement.ToString());
                        xmlWriter.WriteElementString("UserAgreementText", null, productVariant.UserAgreementText);
                        xmlWriter.WriteElementString("IsRecurring", null, productVariant.IsRecurring.ToString());
                        xmlWriter.WriteElementString("RecurringCycleLength", null, productVariant.RecurringCycleLength.ToString());
                        xmlWriter.WriteElementString("RecurringCyclePeriodId", null, productVariant.RecurringCyclePeriodId.ToString());
                        xmlWriter.WriteElementString("RecurringTotalCycles", null, productVariant.RecurringTotalCycles.ToString());
                        xmlWriter.WriteElementString("IsShipEnabled", null, productVariant.IsShipEnabled.ToString());
                        xmlWriter.WriteElementString("IsFreeShipping", null, productVariant.IsFreeShipping.ToString());
                        xmlWriter.WriteElementString("AdditionalShippingCharge", null, productVariant.AdditionalShippingCharge.ToString());
                        xmlWriter.WriteElementString("IsTaxExempt", null, productVariant.IsTaxExempt.ToString());
                        xmlWriter.WriteElementString("TaxCategoryId", null, productVariant.TaxCategoryId.ToString());
                        xmlWriter.WriteElementString("ManageInventoryMethodId", null, productVariant.ManageInventoryMethodId.ToString());
                        xmlWriter.WriteElementString("StockQuantity", null, productVariant.StockQuantity.ToString());
                        xmlWriter.WriteElementString("DisplayStockAvailability", null, productVariant.DisplayStockAvailability.ToString());
                        xmlWriter.WriteElementString("DisplayStockQuantity", null, productVariant.DisplayStockQuantity.ToString());
                        xmlWriter.WriteElementString("MinStockQuantity", null, productVariant.MinStockQuantity.ToString());
                        xmlWriter.WriteElementString("LowStockActivityId", null, productVariant.LowStockActivityId.ToString());
                        xmlWriter.WriteElementString("NotifyAdminForQuantityBelow", null, productVariant.NotifyAdminForQuantityBelow.ToString());
                        xmlWriter.WriteElementString("BackorderModeId", null, productVariant.BackorderModeId.ToString());
                        xmlWriter.WriteElementString("AllowBackInStockSubscriptions", null, productVariant.AllowBackInStockSubscriptions.ToString());
                        xmlWriter.WriteElementString("OrderMinimumQuantity", null, productVariant.OrderMinimumQuantity.ToString());
                        xmlWriter.WriteElementString("OrderMaximumQuantity", null, productVariant.OrderMaximumQuantity.ToString());
                        xmlWriter.WriteElementString("AllowedQuantities", null, productVariant.AllowedQuantities);
                        xmlWriter.WriteElementString("DisableBuyButton", null, productVariant.DisableBuyButton.ToString());
                        xmlWriter.WriteElementString("DisableWishlistButton", null, productVariant.DisableWishlistButton.ToString());
                        xmlWriter.WriteElementString("CallForPrice", null, productVariant.CallForPrice.ToString());
                        xmlWriter.WriteElementString("Price", null, productVariant.Price.ToString());
                        xmlWriter.WriteElementString("OldPrice", null, productVariant.OldPrice.ToString());
                        xmlWriter.WriteElementString("ProductCost", null, productVariant.ProductCost.ToString());
                        xmlWriter.WriteElementString("SpecialPrice", null, productVariant.SpecialPrice.HasValue ? productVariant.SpecialPrice.ToString() : "");
                        xmlWriter.WriteElementString("SpecialPriceStartDateTimeUtc", null, productVariant.SpecialPriceStartDateTimeUtc.HasValue ? productVariant.SpecialPriceStartDateTimeUtc.ToString() : "");
                        xmlWriter.WriteElementString("SpecialPriceEndDateTimeUtc", null, productVariant.SpecialPriceEndDateTimeUtc.HasValue ? productVariant.SpecialPriceEndDateTimeUtc.ToString() : "");
                        xmlWriter.WriteElementString("CustomerEntersPrice", null, productVariant.CustomerEntersPrice.ToString());
                        xmlWriter.WriteElementString("MinimumCustomerEnteredPrice", null, productVariant.MinimumCustomerEnteredPrice.ToString());
                        xmlWriter.WriteElementString("MaximumCustomerEnteredPrice", null, productVariant.MaximumCustomerEnteredPrice.ToString());
                        xmlWriter.WriteElementString("Weight", null, productVariant.Weight.ToString());
                        xmlWriter.WriteElementString("Length", null, productVariant.Length.ToString());
                        xmlWriter.WriteElementString("Width", null, productVariant.Width.ToString());
                        xmlWriter.WriteElementString("Height", null, productVariant.Height.ToString());
                        xmlWriter.WriteElementString("PictureId", null, productVariant.PictureId.ToString());
                        xmlWriter.WriteElementString("Published", null, productVariant.Published.ToString());
                        xmlWriter.WriteElementString("Deleted", null, productVariant.Deleted.ToString());
                        xmlWriter.WriteElementString("DisplayOrder", null, productVariant.DisplayOrder.ToString());
                        xmlWriter.WriteElementString("CreatedOnUtc", null, productVariant.CreatedOnUtc.ToString());
                        xmlWriter.WriteElementString("UpdatedOnUtc", null, productVariant.UpdatedOnUtc.ToString());

                        xmlWriter.WriteStartElement("ProductDiscounts");
                        var discounts = productVariant.AppliedDiscounts;
                        foreach (var discount in discounts)
                        {
                            xmlWriter.WriteElementString("DiscountId", null, discount.Id.ToString());
                        }
                        xmlWriter.WriteEndElement();


                        xmlWriter.WriteStartElement("TierPrices");
                        var tierPrices = productVariant.TierPrices;
                        foreach (var tierPrice in tierPrices)
                        {
                            xmlWriter.WriteElementString("TierPriceId", null, tierPrice.Id.ToString());
                            xmlWriter.WriteElementString("CustomerRoleId", null, tierPrice.CustomerRoleId.HasValue ? tierPrice.CustomerRoleId.ToString() : "0");
                            xmlWriter.WriteElementString("Quantity", null, tierPrice.Quantity.ToString());
                            xmlWriter.WriteElementString("Price", null, tierPrice.Price.ToString());
                        }
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ProductAttributes");
                        var productVariantAttributes = productVariant.ProductVariantAttributes;
                        foreach (var productVariantAttribute in productVariantAttributes)
                        {
                            xmlWriter.WriteStartElement("ProductVariantAttribute");
                            xmlWriter.WriteElementString("ProductVariantAttributeId", null, productVariantAttribute.Id.ToString());
                            xmlWriter.WriteElementString("ProductAttributeId", null, productVariantAttribute.ProductAttributeId.ToString());
                            xmlWriter.WriteElementString("TextPrompt", null, productVariantAttribute.TextPrompt);
                            xmlWriter.WriteElementString("IsRequired", null, productVariantAttribute.IsRequired.ToString());
                            xmlWriter.WriteElementString("AttributeControlTypeId", null, productVariantAttribute.AttributeControlTypeId.ToString());
                            xmlWriter.WriteElementString("DisplayOrder", null, productVariantAttribute.DisplayOrder.ToString());



                            xmlWriter.WriteStartElement("ProductVariantAttributeValues");
                            var productVariantAttributeValues = productVariantAttribute.ProductVariantAttributeValues;
                            foreach (var productVariantAttributeValue in productVariantAttributeValues)
                            {
                                xmlWriter.WriteElementString("ProductVariantAttributeValueId", null, productVariantAttributeValue.Id.ToString());
                                xmlWriter.WriteElementString("Name", null, productVariantAttributeValue.Name);
                                xmlWriter.WriteElementString("PriceAdjustment", null, productVariantAttributeValue.PriceAdjustment.ToString());
                                xmlWriter.WriteElementString("WeightAdjustment", null, productVariantAttributeValue.WeightAdjustment.ToString());
                                xmlWriter.WriteElementString("IsPreSelected", null, productVariantAttributeValue.IsPreSelected.ToString());
                                xmlWriter.WriteElementString("DisplayOrder", null, productVariantAttributeValue.DisplayOrder.ToString());
                            }
                            xmlWriter.WriteEndElement();


                            xmlWriter.WriteEndElement();
                        }
                        xmlWriter.WriteEndElement();
                        
                        xmlWriter.WriteEndElement();
                    }
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ProductPictures");
                var productPictures = product.ProductPictures;
                foreach (var productPicture in productPictures)
                {
                    xmlWriter.WriteStartElement("ProductPicture");
                    xmlWriter.WriteElementString("ProductPictureId", null, productPicture.Id.ToString());
                    xmlWriter.WriteElementString("PictureId", null, productPicture.PictureId.ToString());
                    xmlWriter.WriteElementString("DisplayOrder", null, productPicture.DisplayOrder.ToString());
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
                        xmlWriter.WriteElementString("ProductCategoryId", null, productCategory.Id.ToString());
                        xmlWriter.WriteElementString("CategoryId", null, productCategory.CategoryId.ToString());
                        xmlWriter.WriteElementString("IsFeaturedProduct", null, productCategory.IsFeaturedProduct.ToString());
                        xmlWriter.WriteElementString("DisplayOrder", null, productCategory.DisplayOrder.ToString());
                        xmlWriter.WriteEndElement();
                    }
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ProductManufacturers");
                var productManufacturers = _manufacturerService.GetProductManufacturersByProductId(product.Id);
                if (productManufacturers != null)
                {
                    foreach (var productManufacturer in productManufacturers)
                    {
                        xmlWriter.WriteStartElement("ProductManufacturer");
                        xmlWriter.WriteElementString("ProductManufacturerId", null, productManufacturer.Id.ToString());
                        xmlWriter.WriteElementString("ManufacturerId", null, productManufacturer.ManufacturerId.ToString());
                        xmlWriter.WriteElementString("IsFeaturedProduct", null, productManufacturer.IsFeaturedProduct.ToString());
                        xmlWriter.WriteElementString("DisplayOrder", null, productManufacturer.DisplayOrder.ToString());
                        xmlWriter.WriteEndElement();
                    }
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ProductSpecificationAttributes");
                var productSpecificationAttributes = product.ProductSpecificationAttributes;
                foreach (var productSpecificationAttribute in productSpecificationAttributes)
                {
                    xmlWriter.WriteStartElement("ProductSpecificationAttribute");
                    xmlWriter.WriteElementString("ProductSpecificationAttributeId", null, productSpecificationAttribute.Id.ToString());
                    xmlWriter.WriteElementString("SpecificationAttributeOptionId", null, productSpecificationAttribute.SpecificationAttributeOptionId.ToString());
                    xmlWriter.WriteElementString("AllowFiltering", null, productSpecificationAttribute.AllowFiltering.ToString());
                    xmlWriter.WriteElementString("ShowOnProductPage", null, productSpecificationAttribute.ShowOnProductPage.ToString());
                    xmlWriter.WriteElementString("DisplayOrder", null, productSpecificationAttribute.DisplayOrder.ToString());
                    xmlWriter.WriteEndElement();
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
        /// Export products to XLSX
        /// </summary>
        /// <param name="filePath">File path to use</param>
        /// <param name="products">Products</param>
        public virtual void ExportProductsToXlsx(string filePath, IList<Product> products)
        {
            var newFile = new FileInfo(filePath);
            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(newFile))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Products");
                //Create Headers and format them 
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
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }


                int row = 2;
                foreach (var p in products)
                {
                    var productVariants = _productService.GetProductVariantsByProductId(p.Id, true);
                    foreach (var pv in productVariants)
                    {
                        int col = 1;

                        worksheet.Cells[row, col].Value = p.Name;
                        col++;

                        worksheet.Cells[row, col].Value = p.ShortDescription;
                        col++;

                        worksheet.Cells[row, col].Value = p.FullDescription;
                        col++;

                        worksheet.Cells[row, col].Value = p.ProductTemplateId;
                        col++;

                        worksheet.Cells[row, col].Value = p.ShowOnHomePage;
                        col++;

                        worksheet.Cells[row, col].Value = p.MetaKeywords;
                        col++;

                        worksheet.Cells[row, col].Value = p.MetaDescription;
                        col++;

                        worksheet.Cells[row, col].Value = p.MetaTitle;
                        col++;

                        worksheet.Cells[row, col].Value = p.SeName;
                        col++;

                        worksheet.Cells[row, col].Value = p.AllowCustomerReviews;
                        col++;

                        worksheet.Cells[row, col].Value = p.Published;
                        col++;
                        
                        worksheet.Cells[row, col].Value = pv.Name;
                        col++;

                        worksheet.Cells[row, col].Value = pv.Sku;
                        col++;

                        worksheet.Cells[row, col].Value = pv.ManufacturerPartNumber;
                        col++;

                        worksheet.Cells[row, col].Value = pv.Gtin;
                        col++;

                        worksheet.Cells[row, col].Value = pv.IsGiftCard;
                        col++;

                        worksheet.Cells[row, col].Value = pv.GiftCardTypeId;
                        col++;

                        worksheet.Cells[row, col].Value = pv.RequireOtherProducts;
                        col++;

                        worksheet.Cells[row, col].Value = pv.RequiredProductVariantIds;
                        col++;

                        worksheet.Cells[row, col].Value = pv.AutomaticallyAddRequiredProductVariants;
                        col++;

                        worksheet.Cells[row, col].Value = pv.IsDownload;
                        col++;

                        worksheet.Cells[row, col].Value = pv.DownloadId;
                        col++;

                        worksheet.Cells[row, col].Value = pv.UnlimitedDownloads;
                        col++;

                        worksheet.Cells[row, col].Value = pv.MaxNumberOfDownloads;
                        col++;

                        worksheet.Cells[row, col].Value = pv.DownloadActivationTypeId;
                        col++;

                        worksheet.Cells[row, col].Value = pv.HasSampleDownload;
                        col++;

                        worksheet.Cells[row, col].Value = pv.SampleDownloadId;
                        col++;

                        worksheet.Cells[row, col].Value = pv.HasUserAgreement;
                        col++;

                        worksheet.Cells[row, col].Value = pv.UserAgreementText;
                        col++;

                        worksheet.Cells[row, col].Value = pv.IsRecurring;
                        col++;

                        worksheet.Cells[row, col].Value = pv.RecurringCycleLength;
                        col++;

                        worksheet.Cells[row, col].Value = pv.RecurringCyclePeriodId;
                        col++;

                        worksheet.Cells[row, col].Value = pv.RecurringTotalCycles;
                        col++;

                        worksheet.Cells[row, col].Value = pv.IsShipEnabled;
                        col++;

                        worksheet.Cells[row, col].Value = pv.IsFreeShipping;
                        col++;

                        worksheet.Cells[row, col].Value = pv.AdditionalShippingCharge;
                        col++;

                        worksheet.Cells[row, col].Value = pv.IsTaxExempt;
                        col++;

                        worksheet.Cells[row, col].Value = pv.TaxCategoryId;
                        col++;

                        worksheet.Cells[row, col].Value = pv.ManageInventoryMethodId;
                        col++;

                        worksheet.Cells[row, col].Value = pv.StockQuantity;
                        col++;

                        worksheet.Cells[row, col].Value = pv.DisplayStockAvailability;
                        col++;

                        worksheet.Cells[row, col].Value = pv.DisplayStockQuantity;
                        col++;

                        worksheet.Cells[row, col].Value = pv.MinStockQuantity;
                        col++;

                        worksheet.Cells[row, col].Value = pv.LowStockActivityId;
                        col++;

                        worksheet.Cells[row, col].Value = pv.NotifyAdminForQuantityBelow;
                        col++;

                        worksheet.Cells[row, col].Value = pv.BackorderModeId;
                        col++;

                        worksheet.Cells[row, col].Value = pv.AllowBackInStockSubscriptions;
                        col++;

                        worksheet.Cells[row, col].Value = pv.OrderMinimumQuantity;
                        col++;

                        worksheet.Cells[row, col].Value = pv.OrderMaximumQuantity;
                        col++;

                        worksheet.Cells[row, col].Value = pv.AllowedQuantities;
                        col++;

                        worksheet.Cells[row, col].Value = pv.DisableBuyButton;
                        col++;

                        worksheet.Cells[row, col].Value = pv.DisableWishlistButton;
                        col++;

                        worksheet.Cells[row, col].Value = pv.CallForPrice;
                        col++;

                        worksheet.Cells[row, col].Value = pv.Price;
                        col++;

                        worksheet.Cells[row, col].Value = pv.OldPrice;
                        col++;

                        worksheet.Cells[row, col].Value = pv.ProductCost;
                        col++;

                        worksheet.Cells[row, col].Value = pv.SpecialPrice;
                        col++;

                        worksheet.Cells[row, col].Value = pv.SpecialPriceStartDateTimeUtc;
                        col++;

                        worksheet.Cells[row, col].Value = pv.SpecialPriceEndDateTimeUtc;
                        col++;

                        worksheet.Cells[row, col].Value = pv.CustomerEntersPrice;
                        col++;

                        worksheet.Cells[row, col].Value = pv.MinimumCustomerEnteredPrice;
                        col++;

                        worksheet.Cells[row, col].Value = pv.MaximumCustomerEnteredPrice;
                        col++;

                        worksheet.Cells[row, col].Value = pv.Weight;
                        col++;

                        worksheet.Cells[row, col].Value = pv.Length;
                        col++;

                        worksheet.Cells[row, col].Value = pv.Width;
                        col++;

                        worksheet.Cells[row, col].Value = pv.Height;
                        col++;

                        worksheet.Cells[row, col].Value = pv.CreatedOnUtc.ToOADate();
                        col++;

                        //category identifiers
                        string categoryIds = null;
                        foreach (var pc in _categoryService.GetProductCategoriesByProductId(p.Id))
                        {
                            categoryIds += pc.CategoryId;
                            categoryIds += ";";
                        }
                        worksheet.Cells[row, col].Value = categoryIds;
                        col++;

                        //manufacturer identifiers
                        string manufacturerIds = null;
                        foreach (var pm in _manufacturerService.GetProductManufacturersByProductId(p.Id))
                        {
                            manufacturerIds += pm.ManufacturerId;
                            manufacturerIds += ";";
                        }
                        worksheet.Cells[row, col].Value = manufacturerIds;
                        col++;

                        //pictures (up to 3 pictures)
                        string picture1 = null;
                        string picture2 = null;
                        string picture3 = null;
                        var pictures = _pictureService.GetPicturesByProductId(p.Id, 3);
                        for (int i = 0; i < pictures.Count; i++)
                        {
                            string pictureLocalPath = _pictureService.GetPictureLocalPath(pictures[i]);
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
                        worksheet.Cells[row, col].Value = picture1;
                        col++;
                        worksheet.Cells[row, col].Value = picture2;
                        col++;
                        worksheet.Cells[row, col].Value = picture3;
                        col++;

                        row++;
                    }
                }








                // we had better add some document properties to the spreadsheet 

                // set some core property values
                xlPackage.Workbook.Properties.Title = string.Format("{0} products", _storeInformationSettings.StoreName);
                xlPackage.Workbook.Properties.Author = _storeInformationSettings.StoreName;
                xlPackage.Workbook.Properties.Subject = string.Format("{0} products", _storeInformationSettings.StoreName);
                xlPackage.Workbook.Properties.Keywords = string.Format("{0} products", _storeInformationSettings.StoreName);
                xlPackage.Workbook.Properties.Category = "Products";
                xlPackage.Workbook.Properties.Comments = string.Format("{0} products", _storeInformationSettings.StoreName);

                // set some extended property values
                xlPackage.Workbook.Properties.Company = _storeInformationSettings.StoreName;
                xlPackage.Workbook.Properties.HyperlinkBase = new Uri(_storeInformationSettings.StoreUrl);

                // save the new spreadsheet
                xlPackage.Save();
            }
        }

        /// <summary>
        /// Export order list to xml
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <returns>Result in XML format</returns>
        public virtual string ExportOrdersToXml(IList<Order> orders)
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Orders");
            xmlWriter.WriteAttributeString("Version", NopVersion.CurrentVersion);


            foreach (var order in orders)
            {
                xmlWriter.WriteStartElement("Order");
                
                xmlWriter.WriteElementString("OrderId", null, order.Id.ToString());
                xmlWriter.WriteElementString("OrderGuid", null, order.OrderGuid.ToString());
                xmlWriter.WriteElementString("CustomerId", null, order.CustomerId.ToString());
                xmlWriter.WriteElementString("CustomerLanguageId", null, order.CustomerLanguageId.ToString());
                xmlWriter.WriteElementString("CustomerTaxDisplayTypeId", null, order.CustomerTaxDisplayTypeId.ToString());
                xmlWriter.WriteElementString("CustomerIp", null, order.CustomerIp);
                xmlWriter.WriteElementString("OrderSubtotalInclTax", null, order.OrderSubtotalInclTax.ToString());
                xmlWriter.WriteElementString("OrderSubtotalExclTax", null, order.OrderSubtotalExclTax.ToString());
                xmlWriter.WriteElementString("OrderSubTotalDiscountInclTax", null, order.OrderSubTotalDiscountInclTax.ToString());
                xmlWriter.WriteElementString("OrderSubTotalDiscountExclTax", null, order.OrderSubTotalDiscountExclTax.ToString());
                xmlWriter.WriteElementString("OrderShippingInclTax", null, order.OrderShippingInclTax.ToString());
                xmlWriter.WriteElementString("OrderShippingExclTax", null, order.OrderShippingExclTax.ToString());
                xmlWriter.WriteElementString("PaymentMethodAdditionalFeeInclTax", null, order.PaymentMethodAdditionalFeeInclTax.ToString());
                xmlWriter.WriteElementString("PaymentMethodAdditionalFeeExclTax", null, order.PaymentMethodAdditionalFeeExclTax.ToString());
                xmlWriter.WriteElementString("TaxRates", null, order.TaxRates);
                xmlWriter.WriteElementString("OrderTax", null, order.OrderTax.ToString());
                xmlWriter.WriteElementString("OrderTotal", null, order.OrderTotal.ToString());
                xmlWriter.WriteElementString("RefundedAmount", null, order.RefundedAmount.ToString());
                xmlWriter.WriteElementString("OrderDiscount", null, order.OrderDiscount.ToString());
                xmlWriter.WriteElementString("CurrencyRate", null, order.CurrencyRate.ToString());
                xmlWriter.WriteElementString("CustomerCurrencyCode", null, order.CustomerCurrencyCode);
                xmlWriter.WriteElementString("AffiliateId", null, order.AffiliateId.ToString());
                xmlWriter.WriteElementString("OrderStatusId", null, order.OrderStatusId.ToString());
                xmlWriter.WriteElementString("AllowStoringCreditCardNumber", null, order.AllowStoringCreditCardNumber.ToString());
                xmlWriter.WriteElementString("CardType", null, order.CardType);
                xmlWriter.WriteElementString("CardName", null, order.CardName);
                xmlWriter.WriteElementString("CardNumber", null, order.CardNumber);
                xmlWriter.WriteElementString("MaskedCreditCardNumber", null, order.MaskedCreditCardNumber);
                xmlWriter.WriteElementString("CardCvv2", null, order.CardCvv2);
                xmlWriter.WriteElementString("CardExpirationMonth", null, order.CardExpirationMonth);
                xmlWriter.WriteElementString("CardExpirationYear", null, order.CardExpirationYear);
                xmlWriter.WriteElementString("PaymentMethodSystemName", null, order.PaymentMethodSystemName);
                xmlWriter.WriteElementString("AuthorizationTransactionId", null, order.AuthorizationTransactionId);
                xmlWriter.WriteElementString("AuthorizationTransactionCode", null, order.AuthorizationTransactionCode);
                xmlWriter.WriteElementString("AuthorizationTransactionResult", null, order.AuthorizationTransactionResult);
                xmlWriter.WriteElementString("CaptureTransactionId", null, order.CaptureTransactionId);
                xmlWriter.WriteElementString("CaptureTransactionResult", null, order.CaptureTransactionResult);
                xmlWriter.WriteElementString("SubscriptionTransactionId", null, order.SubscriptionTransactionId);
                xmlWriter.WriteElementString("PurchaseOrderNumber", null, order.PurchaseOrderNumber);
                xmlWriter.WriteElementString("PaymentStatusId", null, order.PaymentStatusId.ToString());
                xmlWriter.WriteElementString("PaidDateUtc", null, (order.PaidDateUtc == null) ? string.Empty : order.PaidDateUtc.Value.ToString());
                xmlWriter.WriteElementString("ShippingStatusId", null, order.ShippingStatusId.ToString());
                xmlWriter.WriteElementString("ShippingMethod", null, order.ShippingMethod);
                xmlWriter.WriteElementString("ShippingRateComputationMethodSystemName", null, order.ShippingRateComputationMethodSystemName);
                xmlWriter.WriteElementString("VatNumber", null, order.VatNumber);
                xmlWriter.WriteElementString("Deleted", null, order.Deleted.ToString());
                xmlWriter.WriteElementString("CreatedOnUtc", null, order.CreatedOnUtc.ToString());

                //products
                var orderProductVariants = order.OrderProductVariants;
                if (orderProductVariants.Count > 0)
                {
                    xmlWriter.WriteStartElement("OrderProductVariants");
                    foreach (var orderProductVariant in orderProductVariants)
                    {
                        xmlWriter.WriteStartElement("OrderProductVariant");
                        xmlWriter.WriteElementString("OrderProductVariantId", null, orderProductVariant.Id.ToString());
                        xmlWriter.WriteElementString("ProductVariantId", null, orderProductVariant.ProductVariantId.ToString());

                        var productVariant = orderProductVariant.ProductVariant;
                        if (productVariant != null)
                            xmlWriter.WriteElementString("ProductVariantName", null, productVariant.FullProductName);


                        xmlWriter.WriteElementString("UnitPriceInclTax", null, orderProductVariant.UnitPriceInclTax.ToString());
                        xmlWriter.WriteElementString("UnitPriceExclTax", null, orderProductVariant.UnitPriceExclTax.ToString());
                        xmlWriter.WriteElementString("PriceInclTax", null, orderProductVariant.PriceInclTax.ToString());
                        xmlWriter.WriteElementString("PriceExclTax", null, orderProductVariant.PriceExclTax.ToString());
                        xmlWriter.WriteElementString("AttributeDescription", null, orderProductVariant.AttributeDescription);
                        xmlWriter.WriteElementString("AttributesXml", null, orderProductVariant.AttributesXml);
                        xmlWriter.WriteElementString("Quantity", null, orderProductVariant.Quantity.ToString());
                        xmlWriter.WriteElementString("DiscountAmountInclTax", null, orderProductVariant.DiscountAmountInclTax.ToString());
                        xmlWriter.WriteElementString("DiscountAmountExclTax", null, orderProductVariant.DiscountAmountExclTax.ToString());
                        xmlWriter.WriteElementString("DownloadCount", null, orderProductVariant.DownloadCount.ToString());
                        xmlWriter.WriteElementString("IsDownloadActivated", null, orderProductVariant.IsDownloadActivated.ToString());
                        xmlWriter.WriteElementString("LicenseDownloadId", null, orderProductVariant.LicenseDownloadId.ToString());
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }

                //shipments
                var shipments = order.Shipments.OrderBy(x => x.CreatedOnUtc).ToList();
                if (shipments.Count > 0)
                {
                    xmlWriter.WriteStartElement("Shipments");
                    foreach (var shipment in shipments)
                    {
                        xmlWriter.WriteStartElement("Shipment");
                        xmlWriter.WriteElementString("ShipmentId", null, shipment.Id.ToString());
                        xmlWriter.WriteElementString("TrackingNumber", null, shipment.TrackingNumber);
                        xmlWriter.WriteElementString("TotalWeight", null, shipment.TotalWeight.HasValue ? shipment.TotalWeight.Value.ToString() : "");

                        xmlWriter.WriteElementString("ShippedDateUtc", null,shipment.ShippedDateUtc.HasValue ? 
                            shipment.ShippedDateUtc.ToString() : "");
                        xmlWriter.WriteElementString("DeliveryDateUtc", null, shipment.DeliveryDateUtc.HasValue ?
                            shipment.DeliveryDateUtc.Value.ToString() : "");
                        xmlWriter.WriteElementString("CreatedOnUtc", null, shipment.CreatedOnUtc.ToString());
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
        /// <param name="filePath">File path to use</param>
        /// <param name="orders">Orders</param>
        public virtual void ExportOrdersToXlsx(string filePath, IList<Order> orders)
        {
            var newFile = new FileInfo(filePath);
            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(newFile))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Orders");
                //Create Headers and format them
                var properties = new string[]
                    {
                        //order properties
                        "OrderId",
                        "OrderGuid",
                        "CustomerId",
                        "OrderSubtotalInclTax",
                        "OrderSubtotalExclTax",
                        "OrderSubTotalDiscountInclTax",
                        "OrderSubTotalDiscountExclTax",
                        "OrderShippingInclTax",
                        "OrderShippingExclTax",
                        "PaymentMethodAdditionalFeeInclTax",
                        "PaymentMethodAdditionalFeeExclTax",
                        "TaxRates",
                        "OrderTax",
                        "OrderTotal",
                        "RefundedAmount",
                        "OrderDiscount",
                        "CurrencyRate",
                        "CustomerCurrencyCode",
                        "AffiliateId",
                        "OrderStatusId",
                        "PaymentMethodSystemName",
                        "PurchaseOrderNumber",
                        "PaymentStatusId",
                        "ShippingStatusId",
                        "ShippingMethod",
                        "ShippingRateComputationMethodSystemName",
                        "VatNumber",
                        "CreatedOnUtc",
                        //billing address
                        "BillingFirstName",
                        "BillingLastName",
                        "BillingEmail",
                        "BillingCompany",
                        "BillingCountry",
                        "BillingStateProvince",
                        "BillingCity",
                        "BillingAddress1",
                        "BillingAddress2",
                        "BillingZipPostalCode",
                        "BillingPhoneNumber",
                        "BillingFaxNumber",
                        //shipping address
                        "ShippingFirstName",
                        "ShippingLastName",
                        "ShippingEmail",
                        "ShippingCompany",
                        "ShippingCountry",
                        "ShippingStateProvince",
                        "ShippingCity",
                        "ShippingAddress1",
                        "ShippingAddress2",
                        "ShippingZipPostalCode",
                        "ShippingPhoneNumber",
                        "ShippingFaxNumber",
                    };
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }


                int row = 2;
                    foreach (var order in orders)
                    {
                        int col = 1;

                        //order properties
                        worksheet.Cells[row, col].Value = order.Id;
                        col++;

                        worksheet.Cells[row, col].Value = order.OrderGuid;
                        col++;

                        worksheet.Cells[row, col].Value = order.CustomerId;
                        col++;

                        worksheet.Cells[row, col].Value = order.OrderSubtotalInclTax;
                        col++;

                        worksheet.Cells[row, col].Value = order.OrderSubtotalExclTax;
                        col++;

                        worksheet.Cells[row, col].Value = order.OrderSubTotalDiscountInclTax;
                        col++;

                        worksheet.Cells[row, col].Value = order.OrderSubTotalDiscountExclTax;
                        col++;

                        worksheet.Cells[row, col].Value = order.OrderShippingInclTax;
                        col++;

                        worksheet.Cells[row, col].Value = order.OrderShippingExclTax;
                        col++;

                        worksheet.Cells[row, col].Value = order.PaymentMethodAdditionalFeeInclTax;
                        col++;

                        worksheet.Cells[row, col].Value = order.PaymentMethodAdditionalFeeExclTax;
                        col++;

                        worksheet.Cells[row, col].Value = order.TaxRates;
                        col++;

                        worksheet.Cells[row, col].Value = order.OrderTax;
                        col++;

                        worksheet.Cells[row, col].Value = order.OrderTotal;
                        col++;

                        worksheet.Cells[row, col].Value = order.RefundedAmount;
                        col++;

                        worksheet.Cells[row, col].Value = order.OrderDiscount;
                        col++;

                        worksheet.Cells[row, col].Value = order.CurrencyRate;
                        col++;

                        worksheet.Cells[row, col].Value = order.CustomerCurrencyCode;
                        col++;

                        worksheet.Cells[row, col].Value = order.AffiliateId.HasValue ? order.AffiliateId.Value : 0;
                        col++;

                        worksheet.Cells[row, col].Value = order.OrderStatusId;
                        col++;

                        worksheet.Cells[row, col].Value = order.PaymentMethodSystemName;
                        col++;

                        worksheet.Cells[row, col].Value = order.PurchaseOrderNumber;
                        col++;

                        worksheet.Cells[row, col].Value =order.PaymentStatusId;
                        col++;

                        worksheet.Cells[row, col].Value = order.ShippingStatusId;
                        col++;

                        worksheet.Cells[row, col].Value = order.ShippingMethod;
                        col++;

                        worksheet.Cells[row, col].Value = order.ShippingRateComputationMethodSystemName;
                        col++;

                        worksheet.Cells[row, col].Value = order.VatNumber;
                        col++;

                        worksheet.Cells[row, col].Value = order.CreatedOnUtc.ToOADate();
                        col++;

                        
                        //billing address
                        worksheet.Cells[row, col].Value = order.BillingAddress != null ? order.BillingAddress.FirstName : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.BillingAddress != null ? order.BillingAddress.LastName : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.BillingAddress != null ? order.BillingAddress.Email : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.BillingAddress != null ? order.BillingAddress.Company : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.BillingAddress != null && order.BillingAddress.Country != null ? order.BillingAddress.Country.Name : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.BillingAddress != null && order.BillingAddress.StateProvince != null ? order.BillingAddress.StateProvince.Name : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.BillingAddress != null ? order.BillingAddress.City : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.BillingAddress != null ? order.BillingAddress.Address1 : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.BillingAddress != null ? order.BillingAddress.Address2 : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.BillingAddress != null ? order.BillingAddress.ZipPostalCode : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.BillingAddress != null ? order.BillingAddress.PhoneNumber : "";
                        col++;

                        worksheet.Cells[row, col].Value = order.BillingAddress != null ? order.BillingAddress.FaxNumber : "";
                        col++;

                        //shipping address
                        worksheet.Cells[row, col].Value = order.ShippingAddress != null ? order.ShippingAddress.FirstName : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.ShippingAddress != null ? order.ShippingAddress.LastName : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.ShippingAddress != null ? order.ShippingAddress.Email : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.ShippingAddress != null ? order.ShippingAddress.Company : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.ShippingAddress != null && order.ShippingAddress.Country != null ? order.ShippingAddress.Country.Name : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.ShippingAddress != null && order.ShippingAddress.StateProvince != null ? order.ShippingAddress.StateProvince.Name : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.ShippingAddress != null ? order.ShippingAddress.City : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.ShippingAddress != null ? order.ShippingAddress.Address1 : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.ShippingAddress != null ? order.ShippingAddress.Address2 : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.ShippingAddress != null ? order.ShippingAddress.ZipPostalCode : "";
                        col++;
                        
                        worksheet.Cells[row, col].Value = order.ShippingAddress != null ? order.ShippingAddress.PhoneNumber : "";
                        col++;

                        worksheet.Cells[row, col].Value = order.ShippingAddress != null ? order.ShippingAddress.FaxNumber : "";
                        col++;
                        
                        //next row
                        row++;
                    }








                // we had better add some document properties to the spreadsheet 

                // set some core property values
                xlPackage.Workbook.Properties.Title = string.Format("{0} orders", _storeInformationSettings.StoreName);
                xlPackage.Workbook.Properties.Author = _storeInformationSettings.StoreName;
                xlPackage.Workbook.Properties.Subject = string.Format("{0} orders", _storeInformationSettings.StoreName);
                xlPackage.Workbook.Properties.Keywords = string.Format("{0} orders", _storeInformationSettings.StoreName);
                xlPackage.Workbook.Properties.Category = "Orders";
                xlPackage.Workbook.Properties.Comments = string.Format("{0} orders", _storeInformationSettings.StoreName);

                // set some extended property values
                xlPackage.Workbook.Properties.Company = _storeInformationSettings.StoreName;
                xlPackage.Workbook.Properties.HyperlinkBase = new Uri(_storeInformationSettings.StoreUrl);

                // save the new spreadsheet
                xlPackage.Save();
            }
        }

        /// <summary>
        /// Export customer list to XLSX
        /// </summary>
        /// <param name="filePath">File path to use</param>
        /// <param name="customers">Customers</param>
        public virtual void ExportCustomersToXlsx(string filePath, IList<Customer> customers)
        {
            var newFile = new FileInfo(filePath);
            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(newFile))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Customers");
                //Create Headers and format them
                var properties = new string[]
                    {
                        "CustomerId",
                        "CustomerGuid",
                        "Email",
                        "Username",
                        "PasswordStr",//why can't we use 'Password' name?
                        "PasswordFormatId",
                        "PasswordSalt",
                        "LanguageId",
                        "CurrencyId",
                        "TaxDisplayTypeId",
                        "IsTaxExempt",
                        "VatNumber",
                        "VatNumberStatusId",
                        "TimeZoneId",
                        "AffiliateId",
                        "Active",
                        "IsGuest",
                        "IsRegistered",
                        "IsAdministrator",
                        "IsForumModerator",
                        "FirstName",
                        "LastName",
                        "Gender",
                        "Company",
                        "StreetAddress",
                        "StreetAddress2",
                        "ZipPostalCode",
                        "City",
                        "CountryId",
                        "StateProvinceId",
                        "Phone",
                        "Fax",
                        "AvatarPictureId",
                        "ForumPostCount",
                        "Signature",
                    };
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }


                int row = 2;
                foreach (var customer in customers)
                {
                    int col = 1;

                    worksheet.Cells[row, col].Value = customer.Id;
                    col++;

                    worksheet.Cells[row, col].Value = customer.CustomerGuid;
                    col++;

                    worksheet.Cells[row, col].Value = customer.Email;
                    col++;

                    worksheet.Cells[row, col].Value = customer.Username;
                    col++;

                    worksheet.Cells[row, col].Value = customer.Password;
                    col++;

                    worksheet.Cells[row, col].Value = customer.PasswordFormatId;
                    col++;

                    worksheet.Cells[row, col].Value = customer.PasswordSalt;
                    col++;

                    worksheet.Cells[row, col].Value = customer.LanguageId.HasValue ? customer.LanguageId.Value : 0;
                    col++;

                    worksheet.Cells[row, col].Value = customer.CurrencyId.HasValue ? customer.CurrencyId.Value : 0;
                    col++;

                    worksheet.Cells[row, col].Value = customer.TaxDisplayTypeId;
                    col++;

                    worksheet.Cells[row, col].Value = customer.IsTaxExempt;
                    col++;

                    worksheet.Cells[row, col].Value = customer.VatNumber;
                    col++;

                    worksheet.Cells[row, col].Value = customer.VatNumberStatusId;
                    col++;

                    worksheet.Cells[row, col].Value = customer.TimeZoneId;
                    col++;

                    worksheet.Cells[row, col].Value = customer.AffiliateId.HasValue ? customer.AffiliateId.Value : 0;
                    col++;

                    worksheet.Cells[row, col].Value = customer.Active;
                    col++;

                    //roles
                    worksheet.Cells[row, col].Value = customer.IsGuest();
                    col++;

                    worksheet.Cells[row, col].Value = customer.IsRegistered();
                    col++;

                    worksheet.Cells[row, col].Value = customer.IsAdmin();
                    col++;

                    worksheet.Cells[row, col].Value = customer.IsForumModerator();
                    col++;
                    
                    //attributes
                    var firstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
                    var lastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
                    var gender = customer.GetAttribute<string>(SystemCustomerAttributeNames.Gender);
                    var company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company);
                    var streetAddress = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress);
                    var streetAddress2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2);
                    var zipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode);
                    var city = customer.GetAttribute<string>(SystemCustomerAttributeNames.City);
                    var countryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId);
                    var stateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId);
                    var phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                    var fax = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax);

                    var avatarPictureId = customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId);
                    var forumPostCount = customer.GetAttribute<int>(SystemCustomerAttributeNames.ForumPostCount);
                    var signature = customer.GetAttribute<string>(SystemCustomerAttributeNames.Signature);

                    worksheet.Cells[row, col].Value = firstName;
                    col++;

                    worksheet.Cells[row, col].Value = lastName;
                    col++;

                    worksheet.Cells[row, col].Value = gender;
                    col++;

                    worksheet.Cells[row, col].Value = company;
                    col++;

                    worksheet.Cells[row, col].Value = streetAddress;
                    col++;

                    worksheet.Cells[row, col].Value = streetAddress2;
                    col++;

                    worksheet.Cells[row, col].Value = zipPostalCode;
                    col++;

                    worksheet.Cells[row, col].Value = city;
                    col++;

                    worksheet.Cells[row, col].Value = countryId;
                    col++;

                    worksheet.Cells[row, col].Value = stateProvinceId;
                    col++;

                    worksheet.Cells[row, col].Value = phone;
                    col++;

                    worksheet.Cells[row, col].Value = fax;
                    col++;

                    worksheet.Cells[row, col].Value = avatarPictureId;
                    col++;

                    worksheet.Cells[row, col].Value = forumPostCount;
                    col++;

                    worksheet.Cells[row, col].Value = signature;
                    col++;

                    row++;
                }








                // we had better add some document properties to the spreadsheet 

                // set some core property values
                xlPackage.Workbook.Properties.Title = string.Format("{0} customers", _storeInformationSettings.StoreName);
                xlPackage.Workbook.Properties.Author = _storeInformationSettings.StoreName;
                xlPackage.Workbook.Properties.Subject = string.Format("{0} customers", _storeInformationSettings.StoreName);
                xlPackage.Workbook.Properties.Keywords = string.Format("{0} customers", _storeInformationSettings.StoreName);
                xlPackage.Workbook.Properties.Category = "Customers";
                xlPackage.Workbook.Properties.Comments = string.Format("{0} customers", _storeInformationSettings.StoreName);

                // set some extended property values
                xlPackage.Workbook.Properties.Company = _storeInformationSettings.StoreName;
                xlPackage.Workbook.Properties.HyperlinkBase = new Uri(_storeInformationSettings.StoreUrl);

                // save the new spreadsheet
                xlPackage.Save();
            }
        }

        /// <summary>
        /// Export customer list to xml
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
                xmlWriter.WriteElementString("Password", null, customer.Password);
                xmlWriter.WriteElementString("PasswordFormatId", null, customer.PasswordFormatId.ToString());
                xmlWriter.WriteElementString("PasswordSalt", null, customer.PasswordSalt);
                xmlWriter.WriteElementString("LanguageId", null, customer.LanguageId.HasValue ? customer.LanguageId.ToString() : "0");
                xmlWriter.WriteElementString("CurrencyId", null, customer.CurrencyId.HasValue ? customer.CurrencyId.ToString() : "0");
                xmlWriter.WriteElementString("TaxDisplayTypeId", null, customer.TaxDisplayTypeId.ToString());
                xmlWriter.WriteElementString("IsTaxExempt", null, customer.IsTaxExempt.ToString());
                xmlWriter.WriteElementString("VatNumber", null, customer.VatNumber);
                xmlWriter.WriteElementString("VatNumberStatusId", null, customer.VatNumberStatusId.ToString());
                xmlWriter.WriteElementString("TimeZoneId", null, customer.TimeZoneId);
                xmlWriter.WriteElementString("AffiliateId", null, customer.AffiliateId.HasValue ? customer.AffiliateId.ToString() : "0");
                xmlWriter.WriteElementString("Active", null, customer.Active.ToString());


                xmlWriter.WriteElementString("IsGuest", null, customer.IsGuest().ToString());
                xmlWriter.WriteElementString("IsRegistered", null, customer.IsRegistered().ToString());
                xmlWriter.WriteElementString("IsAdministrator", null, customer.IsAdmin().ToString());
                xmlWriter.WriteElementString("IsForumModerator", null, customer.IsForumModerator().ToString());

                xmlWriter.WriteElementString("FirstName", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName));
                xmlWriter.WriteElementString("LastName", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName));
                xmlWriter.WriteElementString("Gender", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.Gender));
                xmlWriter.WriteElementString("Company", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.Company));

                xmlWriter.WriteElementString("CountryId", null, customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId).ToString());
                xmlWriter.WriteElementString("StreetAddress", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress));
                xmlWriter.WriteElementString("StreetAddress2", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2));
                xmlWriter.WriteElementString("ZipPostalCode", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode));
                xmlWriter.WriteElementString("City", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.City));
                xmlWriter.WriteElementString("CountryId", null, customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId).ToString());
                xmlWriter.WriteElementString("StateProvinceId", null, customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId).ToString());
                xmlWriter.WriteElementString("Phone", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone));
                xmlWriter.WriteElementString("Fax", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax));

                var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmail(customer.Email);
                bool subscribedToNewsletters = newsletter != null && newsletter.Active;
                xmlWriter.WriteElementString("Newsletter", null, subscribedToNewsletters.ToString());

                xmlWriter.WriteElementString("AvatarPictureId", null, customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId).ToString());
                xmlWriter.WriteElementString("ForumPostCount", null, customer.GetAttribute<int>(SystemCustomerAttributeNames.ForumPostCount).ToString());
                xmlWriter.WriteElementString("Signature", null, customer.GetAttribute<string>(SystemCustomerAttributeNames.Signature));
                
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }


        #endregion
    }
}
