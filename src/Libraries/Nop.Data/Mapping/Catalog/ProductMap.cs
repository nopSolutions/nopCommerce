using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductMap : NopEntityTypeConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);
            builder.ToTable("Product");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(400);
            builder.Property(p => p.MetaKeywords).HasMaxLength(400);
            builder.Property(p => p.MetaTitle).HasMaxLength(400);
            builder.Property(p => p.Sku).HasMaxLength(400);
            builder.Property(p => p.ManufacturerPartNumber).HasMaxLength(400);
            builder.Property(p => p.Gtin).HasMaxLength(400);
            builder.Property(p => p.AdditionalShippingCharge);
            builder.Property(p => p.Price);
            builder.Property(p => p.OldPrice);
            builder.Property(p => p.ProductCost);
            builder.Property(p => p.MinimumCustomerEnteredPrice);
            builder.Property(p => p.MaximumCustomerEnteredPrice);
            builder.Property(p => p.Weight);
            builder.Property(p => p.Length);
            builder.Property(p => p.Width);
            builder.Property(p => p.Height);
            builder.Property(p => p.RequiredProductIds).HasMaxLength(1000);
            builder.Property(p => p.AllowedQuantities).HasMaxLength(1000);
            builder.Property(p => p.BasepriceAmount);
            builder.Property(p => p.BasepriceBaseAmount);
            builder.Ignore(p => p.ProductType);
            builder.Ignore(p => p.BackorderMode);
            builder.Ignore(p => p.DownloadActivationType);
            builder.Ignore(p => p.GiftCardType);
            builder.Ignore(p => p.LowStockActivity);
            builder.Ignore(p => p.ManageInventoryMethod);
            builder.Ignore(p => p.RecurringCyclePeriod);
            builder.Ignore(p => p.RentalPricePeriod);
        }
    }
}