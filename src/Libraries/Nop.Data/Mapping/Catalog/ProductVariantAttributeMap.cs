using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductVariantAttributeMap : EntityTypeConfiguration<ProductVariantAttribute>
    {
        public ProductVariantAttributeMap()
        {
            this.ToTable("ProductVariant_ProductAttribute_Mapping");
            this.HasKey(pva => pva.Id);
            this.Ignore(pva => pva.AttributeControlType);

            this.HasRequired(pva => pva.ProductVariant)
                .WithMany(pv => pv.ProductVariantAttributes)
                .HasForeignKey(pva => pva.ProductVariantId);
            
            this.HasRequired(pva => pva.ProductAttribute)
                .WithMany()
                .HasForeignKey(pva => pva.ProductAttributeId);
        }
    }
}