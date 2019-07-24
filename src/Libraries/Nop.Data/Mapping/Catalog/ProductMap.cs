using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

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
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable(nameof(Product));
            builder.HasKey(product => product.Id);

            builder.Property(product => product.Name).HasMaxLength(400).IsRequired();
            builder.Property(product => product.MetaKeywords).HasMaxLength(400);
            builder.Property(product => product.MetaTitle).HasMaxLength(400);
            builder.Property(product => product.Sku).HasMaxLength(400);
            builder.Property(product => product.ManufacturerPartNumber).HasMaxLength(400);
            builder.Property(product => product.Gtin).HasMaxLength(400);
            builder.Property(product => product.AdditionalShippingCharge).HasColumnType("decimal(18, 4)");
            builder.Property(product => product.Price).HasColumnType("decimal(18, 4)");
            builder.Property(product => product.OldPrice).HasColumnType("decimal(18, 4)");
            builder.Property(product => product.ProductCost).HasColumnType("decimal(18, 4)");
            builder.Property(product => product.MinimumCustomerEnteredPrice).HasColumnType("decimal(18, 4)");
            builder.Property(product => product.MaximumCustomerEnteredPrice).HasColumnType("decimal(18, 4)");
            builder.Property(product => product.Weight).HasColumnType("decimal(18, 4)");
            builder.Property(product => product.Length).HasColumnType("decimal(18, 4)");
            builder.Property(product => product.Width).HasColumnType("decimal(18, 4)");
            builder.Property(product => product.Height).HasColumnType("decimal(18, 4)");
            builder.Property(product => product.RequiredProductIds).HasMaxLength(1000);
            builder.Property(product => product.AllowedQuantities).HasMaxLength(1000);
            builder.Property(product => product.BasepriceAmount).HasColumnType("decimal(18, 4)");
            builder.Property(product => product.BasepriceBaseAmount).HasColumnType("decimal(18, 4)");
            builder.Property(product => product.OverriddenGiftCardAmount).HasColumnType("decimal(18, 4)");

            builder.Ignore(product => product.ProductType);
            builder.Ignore(product => product.BackorderMode);
            builder.Ignore(product => product.DownloadActivationType);
            builder.Ignore(product => product.GiftCardType);
            builder.Ignore(product => product.LowStockActivity);
            builder.Ignore(product => product.ManageInventoryMethod);
            builder.Ignore(product => product.RecurringCyclePeriod);
            builder.Ignore(product => product.RentalPricePeriod);
            builder.Ignore(product => product.AppliedDiscounts);

            base.Configure(builder);
        }

        #endregion
    }
}