using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Services.Messages;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager : IExportManager
    {
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;

        public ExportManager(ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IProductService productService,
            IPictureService pictureService,
            INewsLetterSubscriptionService newsLetterSubscriptionService)
        {
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._pictureService = pictureService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
        }

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
                    xmlWriter.WriteElementString("PriceRanges", null, category.PriceRanges);
                    xmlWriter.WriteElementString("ShowOnHomePage", null, category.ShowOnHomePage.ToString());
                    xmlWriter.WriteElementString("Published", null, category.Published.ToString());
                    xmlWriter.WriteElementString("Deleted", null, category.Deleted.ToString());
                    xmlWriter.WriteElementString("DisplayOrder", null, category.DisplayOrder.ToString());
                    xmlWriter.WriteElementString("CreatedOnUtc", null, category.CreatedOnUtc.ToString());
                    xmlWriter.WriteElementString("UpdatedOnUtc", null, category.UpdatedOnUtc.ToString());


                    xmlWriter.WriteStartElement("Products");
                    var productCategories = _categoryService.GetProductCategoriesByCategoryId(category.Id);
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
                xmlWriter.WriteElementString("PriceRanges", null, manufacturer.PriceRanges);
                xmlWriter.WriteElementString("Published", null, manufacturer.Published.ToString());
                xmlWriter.WriteElementString("Deleted", null, manufacturer.Deleted.ToString());
                xmlWriter.WriteElementString("DisplayOrder", null, manufacturer.DisplayOrder.ToString());
                xmlWriter.WriteElementString("CreatedOnUtc", null, manufacturer.CreatedOnUtc.ToString());
                xmlWriter.WriteElementString("UpdatedOnUtc", null, manufacturer.UpdatedOnUtc.ToString());

                xmlWriter.WriteStartElement("Products");
                var productManufacturers = _manufacturerService.GetProductManufacturersByManufacturerId(manufacturer.Id);
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
                var productVariants = _productService.GetProductVariantsByProductId(product.Id);
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
                        xmlWriter.WriteElementString("OrderMinimumQuantity", null, productVariant.OrderMinimumQuantity.ToString());
                        xmlWriter.WriteElementString("OrderMaximumQuantity", null, productVariant.OrderMaximumQuantity.ToString());
                        xmlWriter.WriteElementString("DisableBuyButton", null, productVariant.DisableBuyButton.ToString());
                        xmlWriter.WriteElementString("DisableWishlistButton", null, productVariant.DisableWishlistButton.ToString());
                        xmlWriter.WriteElementString("CallForPrice", null, productVariant.CallForPrice.ToString());
                        xmlWriter.WriteElementString("Price", null, productVariant.Price.ToString());
                        xmlWriter.WriteElementString("OldPrice", null, productVariant.OldPrice.ToString());
                        xmlWriter.WriteElementString("ProductCost", null, productVariant.ProductCost.ToString());
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
        /// Export products to XLS
        /// </summary>
        /// <param name="filePath">File path to use</param>
        /// <param name="products">Products</param>
        public virtual void ExportProductsToXls(string filePath, IList<Product> products)
        {
            using (var excelHelper = new ExcelHelper(filePath))
            {
                excelHelper.Hdr = "YES";
                excelHelper.Imex = "0";
                var tableDefinition = new Dictionary<string, string>();
                tableDefinition.Add("Name", "ntext");
                tableDefinition.Add("ShortDescription", "ntext");
                tableDefinition.Add("FullDescription", "ntext");
                tableDefinition.Add("ProductTemplateId", "int");
                tableDefinition.Add("ShowOnHomePage", "nvarchar(5)");
                tableDefinition.Add("MetaKeywords", "ntext");
                tableDefinition.Add("MetaDescription", "ntext");
                tableDefinition.Add("MetaTitle", "ntext");
                tableDefinition.Add("AllowCustomerReviews", "nvarchar(5)");
                tableDefinition.Add("Published", "nvarchar(5)");
                tableDefinition.Add("SKU", "ntext");
                tableDefinition.Add("ManufacturerPartNumber", "ntext");
                tableDefinition.Add("IsGiftCard", "nvarchar(5)");
                tableDefinition.Add("GiftCardTypeId", "int");
                tableDefinition.Add("RequireOtherProducts", "nvarchar(5)");
                tableDefinition.Add("RequiredProductVariantIds", "ntext");
                tableDefinition.Add("AutomaticallyAddRequiredProductVariants", "nvarchar(5)");
                tableDefinition.Add("IsDownload", "nvarchar(5)");
                tableDefinition.Add("DownloadId", "int");
                tableDefinition.Add("UnlimitedDownloads", "nvarchar(5)");
                tableDefinition.Add("MaxNumberOfDownloads", "int");
                tableDefinition.Add("DownloadActivationTypeId", "int");
                tableDefinition.Add("HasSampleDownload", "nvarchar(5)");
                tableDefinition.Add("SampleDownloadId", "int");
                tableDefinition.Add("HasUserAgreement", "nvarchar(5)");
                tableDefinition.Add("UserAgreementText", "ntext");
                tableDefinition.Add("IsRecurring", "nvarchar(5)");
                tableDefinition.Add("RecurringCycleLength", "int");
                tableDefinition.Add("RecurringCyclePeriodId", "int");
                tableDefinition.Add("RecurringTotalCycles", "int");
                tableDefinition.Add("IsShipEnabled", "nvarchar(5)");
                tableDefinition.Add("IsFreeShipping", "nvarchar(5)");
                tableDefinition.Add("AdditionalShippingCharge", "decimal");
                tableDefinition.Add("IsTaxExempt", "nvarchar(5)");
                tableDefinition.Add("TaxCategoryId", "int");
                tableDefinition.Add("ManageInventoryMethodId", "int");
                tableDefinition.Add("StockQuantity", "int");
                tableDefinition.Add("DisplayStockAvailability", "nvarchar(5)");
                tableDefinition.Add("DisplayStockQuantity", "nvarchar(5)");
                tableDefinition.Add("MinStockQuantity", "int");
                tableDefinition.Add("LowStockActivityId", "int");
                tableDefinition.Add("NotifyAdminForQuantityBelow", "int");
                tableDefinition.Add("BackorderModeId", "int");
                tableDefinition.Add("OrderMinimumQuantity", "int");
                tableDefinition.Add("OrderMaximumQuantity", "int");
                tableDefinition.Add("DisableBuyButton", "nvarchar(5)");
                tableDefinition.Add("DisableWishlistButton", "nvarchar(5)");
                tableDefinition.Add("CallForPrice", "nvarchar(5)");
                tableDefinition.Add("Price", "decimal");
                tableDefinition.Add("OldPrice", "decimal");
                tableDefinition.Add("ProductCost", "decimal");
                tableDefinition.Add("CustomerEntersPrice", "nvarchar(5)");
                tableDefinition.Add("MinimumCustomerEnteredPrice", "decimal");
                tableDefinition.Add("MaximumCustomerEnteredPrice", "decimal");
                tableDefinition.Add("Weight", "decimal");
                tableDefinition.Add("Length", "decimal");
                tableDefinition.Add("Width", "decimal");
                tableDefinition.Add("Height", "decimal");
                tableDefinition.Add("CreatedOnUtc", "decimal");
                tableDefinition.Add("CategoryIds", "nvarchar(255)");
                tableDefinition.Add("ManufacturerIds", "nvarchar(255)");
                tableDefinition.Add("Picture1", "nvarchar(255)");
                tableDefinition.Add("Picture2", "nvarchar(255)");
                tableDefinition.Add("Picture3", "nvarchar(255)");
                excelHelper.WriteTable("Products", tableDefinition);

                string decimalQuoter = (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(",") ? "\"" : String.Empty);

                foreach (var p in products)
                {
                    var productVariants = _productService.GetProductVariantsByProductId(p.Id);

                    foreach (var pv in productVariants)
                    {
                        var sb = new StringBuilder();
                        sb.Append("INSERT INTO [Products] (Name, ShortDescription,FullDescription,ProductTemplateId,ShowOnHomePage,MetaKeywords,MetaDescription,MetaTitle,AllowCustomerReviews,Published,SKU,ManufacturerPartNumber,IsGiftCard,GiftCardTypeId,RequireOtherProducts,RequiredProductVariantIds,AutomaticallyAddRequiredProductVariants,IsDownload,DownloadId,UnlimitedDownloads,MaxNumberOfDownloads,DownloadActivationTypeId,HasSampleDownload,SampleDownloadId,HasUserAgreement,UserAgreementText,IsRecurring,RecurringCycleLength,RecurringCyclePeriodId,RecurringTotalCycles,IsShipEnabled,IsFreeShipping,AdditionalShippingCharge,IsTaxExempt,TaxCategoryId,ManageInventoryMethodId,StockQuantity,DisplayStockAvailability,DisplayStockQuantity,MinStockQuantity,LowStockActivityId,NotifyAdminForQuantityBelow,BackorderModeId,OrderMinimumQuantity,OrderMaximumQuantity,DisableBuyButton,DisableWishlistButton,CallForPrice,Price,OldPrice,ProductCost,CustomerEntersPrice,MinimumCustomerEnteredPrice,MaximumCustomerEnteredPrice,Weight, Length, Width, Height, CreatedOnUtc,CategoryIds,ManufacturerIds,Picture1,Picture2,Picture3) VALUES (");
                        sb.Append('"'); sb.Append(p.Name != null ? p.Name.Replace('"', '\'') : ""); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.ShortDescription != null ? p.ShortDescription.Replace('"', '\''): ""); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.FullDescription != null ? p.FullDescription.Replace('"', '\'') : ""); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.ProductTemplateId); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.ShowOnHomePage); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.MetaKeywords != null ? p.MetaKeywords.Replace('"', '\'') : ""); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.MetaDescription != null ? p.MetaDescription.Replace('"', '\'') : ""); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.MetaTitle != null ? p.MetaTitle.Replace('"', '\'') : ""); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.AllowCustomerReviews); sb.Append("\",");
                        sb.Append('"'); sb.Append(p.Published); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.Sku != null ? pv.Sku.Replace('"', '\''): ""); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.ManufacturerPartNumber != null ? pv.ManufacturerPartNumber.Replace('"', '\'') : ""); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.IsGiftCard); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.GiftCardTypeId); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.RequireOtherProducts); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.RequiredProductVariantIds != null ? pv.RequiredProductVariantIds.Replace('"', '\'') : ""); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.AutomaticallyAddRequiredProductVariants); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.IsDownload); sb.Append("\",");
                        sb.Append(pv.DownloadId); sb.Append(",");
                        sb.Append('"'); sb.Append(pv.UnlimitedDownloads); sb.Append("\",");
                        sb.Append(pv.MaxNumberOfDownloads); sb.Append(",");
                        sb.Append(pv.DownloadActivationTypeId); sb.Append(",");
                        sb.Append('"'); sb.Append(pv.HasSampleDownload); sb.Append("\",");
                        sb.Append(pv.SampleDownloadId); sb.Append(",");
                        sb.Append('"'); sb.Append(pv.HasUserAgreement); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.UserAgreementText != null ? pv.UserAgreementText.Replace('"', '\'') : ""); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.IsRecurring); sb.Append("\",");
                        sb.Append(pv.RecurringCycleLength); sb.Append(",");
                        sb.Append(pv.RecurringCyclePeriodId); sb.Append(",");
                        sb.Append(pv.RecurringTotalCycles); sb.Append(",");
                        sb.Append('"'); sb.Append(pv.IsShipEnabled); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.IsFreeShipping); sb.Append("\",");
                        sb.Append(decimalQuoter); sb.Append(pv.AdditionalShippingCharge); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append('"'); sb.Append(pv.IsTaxExempt); sb.Append("\",");
                        sb.Append(pv.TaxCategoryId); sb.Append(",");
                        sb.Append(pv.ManageInventoryMethodId); sb.Append(",");
                        sb.Append(pv.StockQuantity); sb.Append(",");
                        sb.Append('"'); sb.Append(pv.DisplayStockAvailability); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.DisplayStockQuantity); sb.Append("\",");
                        sb.Append(pv.MinStockQuantity); sb.Append(",");
                        sb.Append(pv.LowStockActivityId); sb.Append(",");
                        sb.Append(pv.NotifyAdminForQuantityBelow); sb.Append(",");
                        sb.Append(pv.BackorderModeId); sb.Append(",");
                        sb.Append(pv.OrderMinimumQuantity); sb.Append(",");
                        sb.Append(pv.OrderMaximumQuantity); sb.Append(",");
                        sb.Append('"'); sb.Append(pv.DisableBuyButton); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.DisableWishlistButton); sb.Append("\",");
                        sb.Append('"'); sb.Append(pv.CallForPrice); sb.Append("\",");
                        sb.Append(decimalQuoter); sb.Append(pv.Price); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append(decimalQuoter); sb.Append(pv.OldPrice); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append(decimalQuoter); sb.Append(pv.ProductCost); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append('"'); sb.Append(pv.CustomerEntersPrice); sb.Append("\",");
                        sb.Append(decimalQuoter); sb.Append(pv.MinimumCustomerEnteredPrice); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append(decimalQuoter); sb.Append(pv.MaximumCustomerEnteredPrice); sb.Append(decimalQuoter); sb.Append(',');//decimal                        
                        sb.Append(decimalQuoter); sb.Append(pv.Weight); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append(decimalQuoter); sb.Append(pv.Length); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append(decimalQuoter); sb.Append(pv.Width); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append(decimalQuoter); sb.Append(pv.Height); sb.Append(decimalQuoter); sb.Append(',');//decimal
                        sb.Append(decimalQuoter); sb.Append(pv.CreatedOnUtc.ToOADate()); sb.Append(decimalQuoter); sb.Append(',');
                        //category identifiers
                        string categoryIds = null;
                        foreach (var pc in _categoryService.GetProductCategoriesByProductId(p.Id))
                        {
                            categoryIds += pc.CategoryId;
                            categoryIds += ";";
                        }
                        sb.Append('"'); sb.Append(categoryIds != null ? categoryIds.Replace('"', '\'') : ""); sb.Append("\",");
                        //manufacturer identifiers
                        string manufacturerIds = null;
                        foreach (var pm in _manufacturerService.GetProductManufacturersByProductId(p.Id))
                        {
                            manufacturerIds += pm.ManufacturerId;
                            manufacturerIds += ";";
                        }
                        sb.Append('"'); sb.Append(manufacturerIds != null ? manufacturerIds.Replace('"', '\'') : ""); sb.Append("\",");
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
                        sb.Append('"'); sb.Append(picture1 != null ? picture1.Replace('"', '\'') : ""); sb.Append("\",");
                        sb.Append('"'); sb.Append(picture2 != null ? picture2.Replace('"', '\'') : ""); sb.Append("\",");
                        sb.Append('"'); sb.Append(picture3 != null ? picture3.Replace('"', '\'') : ""); sb.Append("\"");
                        
                        sb.Append(")");

                        excelHelper.ExecuteCommand(sb.ToString());
                    }
                }
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
                xmlWriter.WriteElementString("OrderWeight", null, order.OrderWeight.ToString());
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
                xmlWriter.WriteElementString("ShippedDateUtc", null, (order.ShippedDateUtc == null) ? string.Empty : order.ShippedDateUtc.Value.ToString());
                xmlWriter.WriteElementString("TrackingNumber", null, order.TrackingNumber);
                xmlWriter.WriteElementString("VatNumber", null, order.VatNumber);
                xmlWriter.WriteElementString("Deleted", null, order.Deleted.ToString());
                xmlWriter.WriteElementString("CreatedOnUtc", null, order.CreatedOnUtc.ToString());

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

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Export orders to XLS
        /// </summary>
        /// <param name="filePath">File path to use</param>
        /// <param name="orders">Orders</param>
        public virtual void ExportOrdersToXls(string filePath, IList<Order> orders)
        {
            using (var excelHelper = new ExcelHelper(filePath))
            {
                excelHelper.Hdr = "YES";
                excelHelper.Imex = "0";
                var tableDefinition = new Dictionary<string, string>();
                tableDefinition.Add("OrderId", "int");
                tableDefinition.Add("OrderGuid", "uniqueidentifier");
                tableDefinition.Add("CustomerId", "int");
                tableDefinition.Add("OrderSubtotalInclTax", "decimal");
                tableDefinition.Add("OrderSubtotalExclTax", "decimal");
                tableDefinition.Add("OrderSubTotalDiscountInclTax", "decimal");
                tableDefinition.Add("OrderSubTotalDiscountExclTax", "decimal");
                tableDefinition.Add("OrderShippingInclTax", "decimal");
                tableDefinition.Add("OrderShippingExclTax", "decimal");
                tableDefinition.Add("PaymentMethodAdditionalFeeInclTax", "decimal");
                tableDefinition.Add("PaymentMethodAdditionalFeeExclTax", "decimal");
                tableDefinition.Add("TaxRates", "nvarchar(255)");
                tableDefinition.Add("OrderTax", "decimal");
                tableDefinition.Add("OrderTotal", "decimal");
                tableDefinition.Add("RefundedAmount", "decimal");
                tableDefinition.Add("OrderDiscount", "decimal");
                tableDefinition.Add("CurrencyRate", "decimal");
                tableDefinition.Add("CustomerCurrencyCode", "nvarchar(5)");
                tableDefinition.Add("OrderWeight", "decimal");
                tableDefinition.Add("AffiliateId", "int");
                tableDefinition.Add("OrderStatusId", "int");
                tableDefinition.Add("PaymentMethodSystemName", "nvarchar(100)");
                tableDefinition.Add("PurchaseOrderNumber", "nvarchar(100)");
                tableDefinition.Add("PaymentStatusId", "int");
                tableDefinition.Add("ShippingStatusId", "int");
                tableDefinition.Add("ShippingMethod", "nvarchar(100)");
                tableDefinition.Add("ShippingRateComputationMethodSystemName", "nvarchar(100)");
                tableDefinition.Add("VatNumber", "nvarchar(100)");
                tableDefinition.Add("CreatedOnUtc", "decimal");
                excelHelper.WriteTable("Orders", tableDefinition);

                string decimalQuoter = (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(",") ? "\"" : String.Empty);

                foreach (var order in orders)
                {
                    var sb = new StringBuilder();
                    sb.Append("INSERT INTO [Orders] (OrderId, OrderGuid, CustomerId, OrderSubtotalInclTax, OrderSubtotalExclTax, OrderSubTotalDiscountInclTax, OrderSubTotalDiscountExclTax, OrderShippingInclTax, OrderShippingExclTax, PaymentMethodAdditionalFeeInclTax, PaymentMethodAdditionalFeeExclTax, TaxRates, OrderTax, OrderTotal, RefundedAmount, OrderDiscount, CurrencyRate, CustomerCurrencyCode, OrderWeight, AffiliateId, OrderStatusId, PaymentMethodSystemName, PurchaseOrderNumber, PaymentStatusId, ShippingStatusId,  ShippingMethod, ShippingRateComputationMethodSystemName, VatNumber, CreatedOnUtc) VALUES (");


                    sb.Append(order.Id); sb.Append(",");
                    sb.Append('"'); sb.Append(order.OrderGuid); sb.Append("\",");
                    sb.Append(order.CustomerId); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderSubtotalInclTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderSubtotalExclTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderSubTotalDiscountInclTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderSubTotalDiscountExclTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderShippingInclTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderShippingExclTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.PaymentMethodAdditionalFeeInclTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.PaymentMethodAdditionalFeeExclTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append('"'); sb.Append(order.TaxRates != null ? order.TaxRates.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderTax); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderTotal); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.RefundedAmount); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderDiscount); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(decimalQuoter); sb.Append(order.CurrencyRate); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append('"'); sb.Append(order.CustomerCurrencyCode != null ? order.CustomerCurrencyCode.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append(decimalQuoter); sb.Append(order.OrderWeight); sb.Append(decimalQuoter); sb.Append(",");
                    sb.Append(order.AffiliateId.HasValue ? order.AffiliateId.Value : 0); sb.Append(",");
                    sb.Append(order.OrderStatusId); sb.Append(",");
                    sb.Append('"'); sb.Append(order.PaymentMethodSystemName != null ? order.PaymentMethodSystemName.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.PurchaseOrderNumber != null ? order.PurchaseOrderNumber.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append(order.PaymentStatusId); sb.Append(",");
                    sb.Append(order.ShippingStatusId); sb.Append(",");
                    sb.Append('"'); sb.Append(order.ShippingMethod != null ? order.ShippingMethod.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.ShippingRateComputationMethodSystemName != null ? order.ShippingRateComputationMethodSystemName.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append('"'); sb.Append(order.VatNumber != null ? order.VatNumber.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append(decimalQuoter); sb.Append(order.CreatedOnUtc.ToOADate()); sb.Append(decimalQuoter);
                    sb.Append(")");

                    excelHelper.ExecuteCommand(sb.ToString());
                }
            }
        }

        /// <summary>
        /// Export customer list to XLS
        /// </summary>
        /// <param name="filePath">File path to use</param>
        /// <param name="customers">Customers</param>
        public virtual void ExportCustomersToXls(string filePath, IList<Customer> customers)
        {
            using (var excelHelper = new ExcelHelper(filePath))
            {
                excelHelper.Hdr = "YES";
                excelHelper.Imex = "0";
                var tableDefinition = new Dictionary<string, string>();
                //standard properties
                tableDefinition.Add("CustomerId", "int");
                tableDefinition.Add("CustomerGuid", "uniqueidentifier");
                tableDefinition.Add("Email", "nvarchar(255)");
                tableDefinition.Add("Username", "nvarchar(255)");
                tableDefinition.Add("PasswordStr", "nvarchar(255)"); //why can't we use 'Password' name?
                tableDefinition.Add("PasswordFormatId", "int");
                tableDefinition.Add("PasswordSalt", "nvarchar(255)");
                tableDefinition.Add("LanguageId", "int");
                tableDefinition.Add("CurrencyId", "int");
                tableDefinition.Add("TaxDisplayTypeId", "int");
                tableDefinition.Add("IsTaxExempt", "nvarchar(5)");
                tableDefinition.Add("VatNumber", "nvarchar(100)");
                tableDefinition.Add("VatNumberStatusId", "int");
                tableDefinition.Add("TimeZoneId", "nvarchar(200)");
                tableDefinition.Add("AffiliateId", "int");
                tableDefinition.Add("Active", "nvarchar(5)");
                tableDefinition.Add("Deleted", "nvarchar(5)");

                //roles
                tableDefinition.Add("IsGuest", "nvarchar(5)");
                tableDefinition.Add("IsRegistered", "nvarchar(5)");
                tableDefinition.Add("IsAdministrator", "nvarchar(5)");
                tableDefinition.Add("IsForumModerator", "nvarchar(5)");

                //attributes
                tableDefinition.Add("FirstName", "nvarchar(255)");
                tableDefinition.Add("LastName", "nvarchar(255)");
                tableDefinition.Add("Gender", "nvarchar(255)");
                tableDefinition.Add("Company", "nvarchar(255)");
                tableDefinition.Add("StreetAddress", "nvarchar(255)");
                tableDefinition.Add("StreetAddress2", "nvarchar(255)");
                tableDefinition.Add("ZipPostalCode", "nvarchar(255)");
                tableDefinition.Add("City", "nvarchar(255)");
                tableDefinition.Add("CountryId", "int");
                tableDefinition.Add("StateProvinceId", "int");
                tableDefinition.Add("Phone", "nvarchar(255)");
                tableDefinition.Add("Fax", "nvarchar(255)");

                tableDefinition.Add("AvatarPictureId", "int");
                tableDefinition.Add("ForumPostCount", "int");
                tableDefinition.Add("Signature", "nvarchar(255)");
                excelHelper.WriteTable("Customers", tableDefinition);

                //string decimalQuoter = (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(",") ? "\"" : String.Empty);

                foreach (var customer in customers)
                {
                    var sb = new StringBuilder();
                    sb.Append("INSERT INTO [Customers] (CustomerId,CustomerGuid,Email,Username,PasswordStr,PasswordFormatId,PasswordSalt,LanguageId,CurrencyId,TaxDisplayTypeId,IsTaxExempt,VatNumber,VatNumberStatusId,TimeZoneId,AffiliateId,Active,Deleted,IsGuest,IsRegistered,IsAdministrator,IsForumModerator,FirstName,LastName,Gender,Company,StreetAddress,StreetAddress2,ZipPostalCode,City,CountryId,StateProvinceId,Phone,Fax,AvatarPictureId,ForumPostCount,Signature) VALUES (");

                    sb.Append(customer.Id); sb.Append(",");
                    sb.Append('"'); sb.Append(customer.CustomerGuid); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.Email != null ? customer.Email.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.Username != null ? customer.Username.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.Password != null ? customer.Password.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append(customer.PasswordFormatId); sb.Append(",");
                    sb.Append('"'); sb.Append(customer.PasswordSalt != null ? customer.PasswordSalt.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append(customer.LanguageId.HasValue ? customer.LanguageId.Value : 0); sb.Append(",");
                    sb.Append(customer.CurrencyId.HasValue ? customer.CurrencyId.Value : 0); sb.Append(",");
                    sb.Append(customer.TaxDisplayTypeId); sb.Append(',');
                    sb.Append('"'); sb.Append(customer.IsTaxExempt); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.VatNumber != null ? customer.VatNumber.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append(customer.VatNumberStatusId); sb.Append(',');
                    sb.Append('"'); sb.Append(customer.TimeZoneId != null ? customer.TimeZoneId.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append(customer.AffiliateId.HasValue ? customer.AffiliateId.Value : 0); sb.Append(",");
                    sb.Append('"'); sb.Append(customer.Active); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.Deleted); sb.Append("\",");

                    //roles
                    sb.Append('"'); sb.Append(customer.IsGuest()); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.IsRegistered()); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.IsAdmin()); sb.Append("\",");
                    sb.Append('"'); sb.Append(customer.IsForumModerator()); sb.Append("\",");


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
                    sb.Append('"'); sb.Append(firstName != null ? firstName.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append('"'); sb.Append(lastName != null ? lastName.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append('"'); sb.Append(gender != null ? gender.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append('"'); sb.Append(company != null ? company.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append('"'); sb.Append(streetAddress != null ? streetAddress.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append('"'); sb.Append(streetAddress2 != null ? streetAddress2.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append('"'); sb.Append(zipPostalCode != null ? zipPostalCode.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append('"'); sb.Append(city != null ? city.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append(countryId); sb.Append(',');
                    sb.Append(stateProvinceId); sb.Append(',');
                    sb.Append('"'); sb.Append(phone != null ? phone.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append('"'); sb.Append(fax != null ? fax.Replace('"', '\'') : ""); sb.Append("\",");
                    sb.Append('"'); sb.Append(avatarPictureId); sb.Append("\",");
                    sb.Append(forumPostCount); sb.Append(",");
                    sb.Append('"'); sb.Append(signature != null ? signature.Replace('"', '\'') : ""); sb.Append("\"");
                    sb.Append(")");

                    excelHelper.ExecuteCommand(sb.ToString());
                }
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

        /// <summary>
        /// Export language resources to xml
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>Result in XML format</returns>
        public virtual string ExportLanguageToXml(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Language");
            xmlWriter.WriteAttributeString("Name", language.Name);

            var resources = language.LocaleStringResources.OrderBy(x => x.ResourceName).ToList();
            foreach (var resource in resources)
            {
                xmlWriter.WriteStartElement("LocaleResource");
                xmlWriter.WriteAttributeString("Name", resource.ResourceName);
                xmlWriter.WriteElementString("Value", null, resource.ResourceValue);
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
