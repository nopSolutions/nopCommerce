using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product mapping configuration
    /// </summary>
    public partial class ProductMap : NopEntityTypeConfiguration<Product>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Product> builder)
        {
            builder.HasTableName(nameof(Product));

            builder.Property(product => product.Name).HasLength(400).IsNullable(false);
            builder.Property(product => product.MetaKeywords).HasLength(400);
            builder.Property(product => product.MetaTitle).HasLength(400);
            builder.Property(product => product.Sku).HasLength(400);
            builder.Property(product => product.ManufacturerPartNumber).HasLength(400);
            builder.Property(product => product.Gtin).HasLength(400);
            builder.Property(product => product.AdditionalShippingCharge).HasDecimal();
            builder.Property(product => product.Price).HasDecimal();
            builder.Property(product => product.OldPrice).HasDecimal();
            builder.Property(product => product.ProductCost).HasDecimal();
            builder.Property(product => product.MinimumCustomerEnteredPrice).HasDecimal();
            builder.Property(product => product.MaximumCustomerEnteredPrice).HasDecimal();
            builder.Property(product => product.Weight).HasDecimal();
            builder.Property(product => product.Length).HasDecimal();
            builder.Property(product => product.Width).HasDecimal();
            builder.Property(product => product.Height).HasDecimal();
            builder.Property(product => product.RequiredProductIds).HasLength(1000);
            builder.Property(product => product.AllowedQuantities).HasLength(1000);
            builder.Property(product => product.BasepriceAmount).HasDecimal();
            builder.Property(product => product.BasepriceBaseAmount).HasDecimal();
            builder.Property(product => product.OverriddenGiftCardAmount).HasDecimal();
            builder.Property(product => product.ProductTypeId);
            builder.Property(product => product.ParentGroupedProductId);
            builder.Property(product => product.VisibleIndividually);
            builder.Property(product => product.ShortDescription);
            builder.Property(product => product.FullDescription);
            builder.Property(product => product.AdminComment);
            builder.Property(product => product.ProductTemplateId);
            builder.Property(product => product.VendorId);
            builder.Property(product => product.ShowOnHomepage);
            builder.Property(product => product.MetaDescription);
            builder.Property(product => product.AllowCustomerReviews);
            builder.Property(product => product.ApprovedRatingSum);
            builder.Property(product => product.NotApprovedRatingSum);
            builder.Property(product => product.ApprovedTotalReviews);
            builder.Property(product => product.NotApprovedTotalReviews);
            builder.Property(product => product.SubjectToAcl);
            builder.Property(product => product.LimitedToStores);
            builder.Property(product => product.IsGiftCard);
            builder.Property(product => product.GiftCardTypeId);
            builder.Property(product => product.RequireOtherProducts);
            builder.Property(product => product.AutomaticallyAddRequiredProducts);
            builder.Property(product => product.IsDownload);
            builder.Property(product => product.DownloadId);
            builder.Property(product => product.UnlimitedDownloads);
            builder.Property(product => product.MaxNumberOfDownloads);
            builder.Property(product => product.DownloadExpirationDays);
            builder.Property(product => product.DownloadActivationTypeId);
            builder.Property(product => product.HasSampleDownload);
            builder.Property(product => product.SampleDownloadId);
            builder.Property(product => product.HasUserAgreement);
            builder.Property(product => product.UserAgreementText);
            builder.Property(product => product.IsRecurring);
            builder.Property(product => product.RecurringCycleLength);
            builder.Property(product => product.RecurringCyclePeriodId);
            builder.Property(product => product.RecurringTotalCycles);
            builder.Property(product => product.IsRental);
            builder.Property(product => product.RentalPriceLength);
            builder.Property(product => product.RentalPricePeriodId);
            builder.Property(product => product.IsShipEnabled);
            builder.Property(product => product.IsFreeShipping);
            builder.Property(product => product.ShipSeparately);
            builder.Property(product => product.DeliveryDateId);
            builder.Property(product => product.IsTaxExempt);
            builder.Property(product => product.TaxCategoryId);
            builder.Property(product => product.IsTelecommunicationsOrBroadcastingOrElectronicServices);
            builder.Property(product => product.ManageInventoryMethodId);
            builder.Property(product => product.ProductAvailabilityRangeId);
            builder.Property(product => product.UseMultipleWarehouses);
            builder.Property(product => product.WarehouseId);
            builder.Property(product => product.StockQuantity);
            builder.Property(product => product.DisplayStockAvailability);
            builder.Property(product => product.DisplayStockQuantity);
            builder.Property(product => product.MinStockQuantity);
            builder.Property(product => product.LowStockActivityId);
            builder.Property(product => product.NotifyAdminForQuantityBelow);
            builder.Property(product => product.BackorderModeId);
            builder.Property(product => product.AllowBackInStockSubscriptions);
            builder.Property(product => product.OrderMinimumQuantity);
            builder.Property(product => product.OrderMaximumQuantity);
            builder.Property(product => product.AllowAddingOnlyExistingAttributeCombinations);
            builder.Property(product => product.NotReturnable);
            builder.Property(product => product.DisableBuyButton);
            builder.Property(product => product.DisableWishlistButton);
            builder.Property(product => product.AvailableForPreOrder);
            builder.Property(product => product.PreOrderAvailabilityStartDateTimeUtc);
            builder.Property(product => product.CallForPrice);
            builder.Property(product => product.CustomerEntersPrice);
            builder.Property(product => product.BasepriceEnabled);
            builder.Property(product => product.BasepriceUnitId);
            builder.Property(product => product.BasepriceBaseUnitId);
            builder.Property(product => product.MarkAsNew);
            builder.Property(product => product.MarkAsNewStartDateTimeUtc);
            builder.Property(product => product.MarkAsNewEndDateTimeUtc);
            builder.Property(product => product.HasTierPrices);
            builder.Property(product => product.HasDiscountsApplied);
            builder.Property(product => product.AvailableStartDateTimeUtc);
            builder.Property(product => product.AvailableEndDateTimeUtc);
            builder.Property(product => product.DisplayOrder);
            builder.Property(product => product.Published);
            builder.Property(product => product.Deleted);
            builder.Property(product => product.CreatedOnUtc);
            builder.Property(product => product.UpdatedOnUtc);

            builder.Ignore(product => product.ProductType);
            builder.Ignore(product => product.BackorderMode);
            builder.Ignore(product => product.DownloadActivationType);
            builder.Ignore(product => product.GiftCardType);
            builder.Ignore(product => product.LowStockActivity);
            builder.Ignore(product => product.ManageInventoryMethod);
            builder.Ignore(product => product.RecurringCyclePeriod);
            builder.Ignore(product => product.RentalPricePeriod);
        }

        #endregion
    }
}