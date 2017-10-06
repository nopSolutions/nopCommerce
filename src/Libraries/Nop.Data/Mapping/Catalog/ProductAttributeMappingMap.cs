using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductAttributeMappingMap : NopEntityTypeConfiguration<ProductAttributeMapping>
    {
        public override void Configure(EntityTypeBuilder<ProductAttributeMapping> builder)
        {
            base.Configure(builder);
            builder.ToTable("Product_ProductAttribute_Mapping");
            builder.HasKey(pam => pam.Id);
            builder.Ignore(pam => pam.AttributeControlType);

            builder.HasOne(pam => pam.Product)
                .WithMany(p => p.ProductAttributeMappings)
                .HasForeignKey(pam => pam.ProductId);

            builder.HasOne(pam => pam.ProductAttribute)
                .WithMany()
                .HasForeignKey(pam => pam.ProductAttributeId);
        }
    }
}