using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ProductSpecificationAttributeMap : NopEntityTypeConfiguration<ProductSpecificationAttribute>
    {
        public override void Configure(EntityTypeBuilder<ProductSpecificationAttribute> builder)
        {
            base.Configure(builder);
            builder.ToTable("Product_SpecificationAttribute_Mapping");
            builder.HasKey(psa => psa.Id);

            builder.Property(psa => psa.CustomValue).HasMaxLength(4000);

            builder.Ignore(psa => psa.AttributeType);

            builder.HasOne(psa => psa.SpecificationAttributeOption)
                .WithMany()
                .HasForeignKey(psa => psa.SpecificationAttributeOptionId)
                .IsRequired(true);


            builder.HasOne(psa => psa.Product)
                .WithMany(p => p.ProductSpecificationAttributes)
                .HasForeignKey(psa => psa.ProductId)
                .IsRequired(true);
        }
    }
}