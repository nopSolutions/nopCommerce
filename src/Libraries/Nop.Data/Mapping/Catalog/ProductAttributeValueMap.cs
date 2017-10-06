using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductAttributeValueMap : NopEntityTypeConfiguration<ProductAttributeValue>
    {
        public override void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProductAttributeValue");
            builder.HasKey(pav => pav.Id);
            builder.Property(pav => pav.Name).IsRequired().HasMaxLength(400);
            builder.Property(pav => pav.ColorSquaresRgb).HasMaxLength(100);

            builder.Property(pav => pav.PriceAdjustment);
            builder.Property(pav => pav.WeightAdjustment);
            builder.Property(pav => pav.Cost);

            builder.Ignore(pav => pav.AttributeValueType);

            builder.HasOne(pav => pav.ProductAttributeMapping)
                .WithMany(pam => pam.ProductAttributeValues)
                .HasForeignKey(pav => pav.ProductAttributeMappingId);
        }
    }
}