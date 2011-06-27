using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductVariantAttributeValueMap : EntityTypeConfiguration<ProductVariantAttributeValue>
    {
        public ProductVariantAttributeValueMap()
        {
            this.ToTable("ProductVariantAttributeValue");
            this.HasKey(pvav => pvav.Id);
            this.Property(pvav => pvav.Name);

            this.Property(pvav => pvav.PriceAdjustment).HasPrecision(18, 4);
            this.Property(pvav => pvav.WeightAdjustment).HasPrecision(18, 4);

            this.HasRequired(pvav => pvav.ProductVariantAttribute)
                .WithMany(pva => pva.ProductVariantAttributeValues)
                .HasForeignKey(pvav => pvav.ProductVariantAttributeId);
        }
    }
}