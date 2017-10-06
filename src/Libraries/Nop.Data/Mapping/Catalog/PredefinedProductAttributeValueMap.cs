using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class PredefinedProductAttributeValueMap : NopEntityTypeConfiguration<PredefinedProductAttributeValue>
    {
        public override void Configure(EntityTypeBuilder<PredefinedProductAttributeValue> builder)
        {
            base.Configure(builder);
            builder.ToTable("PredefinedProductAttributeValue");
            builder.HasKey(pav => pav.Id);
            builder.Property(pav => pav.Name).IsRequired().HasMaxLength(400);
            builder.Property(pav => pav.PriceAdjustment);
            builder.Property(pav => pav.WeightAdjustment);
            builder.Property(pav => pav.Cost);
            builder.HasOne(pav => pav.ProductAttribute)
                .WithMany()
                .HasForeignKey(pav => pav.ProductAttributeId);
        }
    }
}