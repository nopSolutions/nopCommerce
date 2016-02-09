using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Services.ExportImport.Help
{
    /// <summary>
    /// Class management product using the properties
    /// </summary>
    public class ProductGetProperties:BaseGetProperties, IGetProperties<Product>
    {
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;

        public ProductGetProperties(IPictureService pictureService, ICategoryService categoryService,
            IManufacturerService manufacturerService) : base(pictureService)
        {
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
        }

        /// <summary>
        /// Get property array
        /// </summary>
        public PropertyByName<Product>[] GetPropertys
        {
            get
            {
                return new[]
                {
                    new PropertyByName<Product>("ProductTypeId", p => p.ProductTypeId),
                    new PropertyByName<Product>("ParentGroupedProductId", p => p.ParentGroupedProductId),
                    new PropertyByName<Product>("VisibleIndividually", p => p.VisibleIndividually),
                    new PropertyByName<Product>("Name", p => p.Name),
                    new PropertyByName<Product>("ShortDescription", p => p.ShortDescription),
                    new PropertyByName<Product>("FullDescription", p => p.FullDescription),
                    new PropertyByName<Product>("VendorId", p => p.VendorId.ToString()),
                    new PropertyByName<Product>("ProductTemplateId", p => p.ProductTemplateId),
                    new PropertyByName<Product>("ShowOnHomePage", p => p.ShowOnHomePage),
                    new PropertyByName<Product>("MetaKeywords", p => p.MetaKeywords),
                    new PropertyByName<Product>("MetaDescription", p => p.MetaDescription),
                    new PropertyByName<Product>("MetaTitle", p => p.MetaTitle),
                    new PropertyByName<Product>("SeName", p => p.GetSeName(0)),
                    new PropertyByName<Product>("AllowCustomerReviews", p => p.AllowCustomerReviews),
                    new PropertyByName<Product>("Published", p => p.Published),
                    new PropertyByName<Product>("SKU", p => p.Sku),
                    new PropertyByName<Product>("ManufacturerPartNumber", p => p.ManufacturerPartNumber),
                    new PropertyByName<Product>("Gtin", p => p.Gtin),
                    new PropertyByName<Product>("IsGiftCard", p => p.IsGiftCard),
                    new PropertyByName<Product>("GiftCardTypeId", p => p.GiftCardTypeId),
                    new PropertyByName<Product>("OverriddenGiftCardAmount", p => p.OverriddenGiftCardAmount),
                    new PropertyByName<Product>("RequireOtherProducts", p => p.RequireOtherProducts),
                    new PropertyByName<Product>("RequiredProductIds", p => p.RequiredProductIds),
                    new PropertyByName<Product>("AutomaticallyAddRequiredProducts", p => p.AutomaticallyAddRequiredProducts),
                    new PropertyByName<Product>("IsDownload", p => p.IsDownload),
                    new PropertyByName<Product>("DownloadId", p => p.DownloadId),
                    new PropertyByName<Product>("UnlimitedDownloads", p => p.UnlimitedDownloads),
                    new PropertyByName<Product>("MaxNumberOfDownloads", p => p.MaxNumberOfDownloads),
                    new PropertyByName<Product>("DownloadActivationTypeId", p => p.DownloadActivationTypeId),
                    new PropertyByName<Product>("HasSampleDownload", p => p.HasSampleDownload),
                    new PropertyByName<Product>("SampleDownloadId", p => p.SampleDownloadId),
                    new PropertyByName<Product>("HasUserAgreement", p => p.HasUserAgreement),
                    new PropertyByName<Product>("UserAgreementText", p => p.UserAgreementText),
                    new PropertyByName<Product>("IsRecurring", p => p.IsRecurring),
                    new PropertyByName<Product>("RecurringCycleLength", p => p.RecurringCycleLength),
                    new PropertyByName<Product>("RecurringCyclePeriodId", p => p.RecurringCyclePeriodId),
                    new PropertyByName<Product>("RecurringTotalCycles", p => p.RecurringTotalCycles),
                    new PropertyByName<Product>("IsRental", p => p.IsRental),
                    new PropertyByName<Product>("RentalPriceLength", p => p.RentalPriceLength),
                    new PropertyByName<Product>("RentalPricePeriodId", p => p.RentalPricePeriodId),
                    new PropertyByName<Product>("IsShipEnabled", p => p.IsShipEnabled),
                    new PropertyByName<Product>("IsFreeShipping", p => p.IsFreeShipping),
                    new PropertyByName<Product>("ShipSeparately", p => p.ShipSeparately),
                    new PropertyByName<Product>("AdditionalShippingCharge", p => p.AdditionalShippingCharge),
                    new PropertyByName<Product>("DeliveryDateId", p => p.DeliveryDateId),
                    new PropertyByName<Product>("IsTaxExempt", p => p.IsTaxExempt),
                    new PropertyByName<Product>("TaxCategoryId", p => p.TaxCategoryId),
                    new PropertyByName<Product>("IsTelecommunicationsOrBroadcastingOrElectronicServices", p => p.IsTelecommunicationsOrBroadcastingOrElectronicServices),
                    new PropertyByName<Product>("ManageInventoryMethodId", p => p.ManageInventoryMethodId),
                    new PropertyByName<Product>("UseMultipleWarehouses", p => p.UseMultipleWarehouses),
                    new PropertyByName<Product>("WarehouseId", p => p.WarehouseId),
                    new PropertyByName<Product>("StockQuantity", p => p.StockQuantity),
                    new PropertyByName<Product>("DisplayStockAvailability", p => p.DisplayStockAvailability),
                    new PropertyByName<Product>("DisplayStockQuantity", p => p.DisplayStockQuantity),
                    new PropertyByName<Product>("MinStockQuantity", p => p.MinStockQuantity),
                    new PropertyByName<Product>("LowStockActivityId", p => p.LowStockActivityId),
                    new PropertyByName<Product>("NotifyAdminForQuantityBelow", p => p.NotifyAdminForQuantityBelow),
                    new PropertyByName<Product>("BackorderModeId", p => p.BackorderModeId),
                    new PropertyByName<Product>("AllowBackInStockSubscriptions", p => p.AllowBackInStockSubscriptions),
                    new PropertyByName<Product>("OrderMinimumQuantity", p => p.OrderMinimumQuantity),
                    new PropertyByName<Product>("OrderMaximumQuantity", p => p.OrderMaximumQuantity),
                    new PropertyByName<Product>("AllowedQuantities", p => p.AllowedQuantities),
                    new PropertyByName<Product>("AllowAddingOnlyExistingAttributeCombinations", p => p.AllowAddingOnlyExistingAttributeCombinations),
                    new PropertyByName<Product>("DisableBuyButton", p => p.DisableBuyButton),
                    new PropertyByName<Product>("DisableWishlistButton", p => p.DisableWishlistButton),
                    new PropertyByName<Product>("AvailableForPreOrder", p => p.AvailableForPreOrder),
                    new PropertyByName<Product>("PreOrderAvailabilityStartDateTimeUtc", p => p.PreOrderAvailabilityStartDateTimeUtc),
                    new PropertyByName<Product>("CallForPrice", p => p.CallForPrice),
                    new PropertyByName<Product>("Price", p => p.Price),
                    new PropertyByName<Product>("OldPrice", p => p.OldPrice),
                    new PropertyByName<Product>("ProductCost", p => p.ProductCost),
                    new PropertyByName<Product>("SpecialPrice", p => p.SpecialPrice),
                    new PropertyByName<Product>("SpecialPriceStartDateTimeUtc", p => p.SpecialPriceStartDateTimeUtc),
                    new PropertyByName<Product>("SpecialPriceEndDateTimeUtc", p => p.SpecialPriceEndDateTimeUtc),
                    new PropertyByName<Product>("CustomerEntersPrice", p => p.CustomerEntersPrice),
                    new PropertyByName<Product>("MinimumCustomerEnteredPrice", p => p.MinimumCustomerEnteredPrice),
                    new PropertyByName<Product>("MaximumCustomerEnteredPrice", p => p.MaximumCustomerEnteredPrice),
                    new PropertyByName<Product>("BasepriceEnabled", p => p.BasepriceEnabled),
                    new PropertyByName<Product>("BasepriceAmount", p => p.BasepriceAmount),
                    new PropertyByName<Product>("BasepriceUnitId", p => p.BasepriceUnitId),
                    new PropertyByName<Product>("BasepriceBaseAmount", p => p.BasepriceBaseAmount),
                    new PropertyByName<Product>("BasepriceBaseUnitId", p => p.BasepriceBaseUnitId),
                    new PropertyByName<Product>("MarkAsNew", p => p.MarkAsNew),
                    new PropertyByName<Product>("MarkAsNewStartDateTimeUtc", p => p.MarkAsNewStartDateTimeUtc),
                    new PropertyByName<Product>("MarkAsNewEndDateTimeUtc", p => p.MarkAsNewEndDateTimeUtc),
                    new PropertyByName<Product>("Weight", p => p.Weight),
                    new PropertyByName<Product>("Length", p => p.Length),
                    new PropertyByName<Product>("Width", p => p.Width),
                    new PropertyByName<Product>("Height", p => p.Height),
                    new PropertyByName<Product>("CreatedOnUtc", p => p.CreatedOnUtc),
                    new PropertyByName<Product>("CategoryIds", GetCategoryIds),
                    new PropertyByName<Product>("ManufacturerIds", GetManufacturerIds),
                    new PropertyByName<Product>("Picture1", p => GetPictures(p)[0]),
                    new PropertyByName<Product>("Picture2", p => GetPictures(p)[1]),
                    new PropertyByName<Product>("Picture3", p => GetPictures(p)[2])
                };
            }
        }

        /// <summary>
        /// Fills the specified object
        /// </summary>
        /// <param name="objectToFill">The object to fill</param>
        /// <param name="isNew">Is new object flag</param>
        /// <param name="manager">Property manager</param>
        public void FillObject(BaseEntity objectToFill, bool isNew, PropertyManager<Product> manager)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns the list of categories for a product separated by a ";"
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of categories</returns>
        private string GetCategoryIds(Product product)
        {
            string categoryIds = null;
            foreach (var pc in _categoryService.GetProductCategoriesByProductId(product.Id))
            {
                categoryIds += pc.CategoryId;
                categoryIds += ";";
            }
            return categoryIds;
        }

        /// <summary>
        /// Returns the list of manufacturer for a product separated by a ";"
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of manufacturer</returns>
        private string GetManufacturerIds(Product product)
        {
            string manufacturerIds = null;
            foreach (var pm in _manufacturerService.GetProductManufacturersByProductId(product.Id))
            {
                manufacturerIds += pm.ManufacturerId;
                manufacturerIds += ";";
            }
            return manufacturerIds;
        }

        /// <summary>
        /// Returns the three first image associated with the product
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>three first image</returns>
        private string[] GetPictures(Product product)
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
    }
}
