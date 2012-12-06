using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductSpecificationAttributeMap : EntityTypeConfiguration<ProductSpecificationAttribute>
    {
        public ProductSpecificationAttributeMap()
        {
            this.ToTable("Product_SpecificationAttribute_Mapping");
            this.HasKey(psa => psa.Id);

            this.Property(psa => psa.CustomValue).HasMaxLength(4000);

            this.HasRequired(psa => psa.SpecificationAttributeOption)
                .WithMany(sao => sao.ProductSpecificationAttributes)
                .HasForeignKey(psa => psa.SpecificationAttributeOptionId);


            this.HasRequired(psa => psa.Product)
                .WithMany(p => p.ProductSpecificationAttributes)
                .HasForeignKey(psa => psa.ProductId);
        }
    }
}