using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductVariantAttributeCombinationMap : EntityTypeConfiguration<ProductVariantAttributeCombination>
    {
        public ProductVariantAttributeCombinationMap()
        {
            this.ToTable("ProductVariantAttributeCombination");
            this.HasKey(pvac => pvac.Id);

            this.Property(pvac => pvac.Sku).HasMaxLength(400);
            this.Property(pvac => pvac.ManufacturerPartNumber).HasMaxLength(400);
            this.Property(pvac => pvac.Gtin).HasMaxLength(400);
            this.Property(pvac => pvac.OverriddenPrice).HasPrecision(18, 4);

            this.HasRequired(pvac => pvac.Product)
                .WithMany(p => p.ProductVariantAttributeCombinations)
                .HasForeignKey(pvac => pvac.ProductId);
        }
    }
}