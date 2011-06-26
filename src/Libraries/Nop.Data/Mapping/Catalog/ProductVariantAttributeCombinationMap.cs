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
            this.Property(pvac => pvac.AttributesXml).IsMaxLength();

            this.HasRequired(pvac => pvac.ProductVariant)
                .WithMany(pv => pv.ProductVariantAttributeCombinations)
                .HasForeignKey(pvac => pvac.ProductVariantId);
        }
    }
}