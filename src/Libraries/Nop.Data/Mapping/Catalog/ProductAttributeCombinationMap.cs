using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ProductAttributeCombinationMap : NopEntityTypeConfiguration<ProductAttributeCombination>
    {
        public override void Configure(EntityTypeBuilder<ProductAttributeCombination> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProductAttributeCombination");
            builder.HasKey(pac => pac.Id);

            builder.Property(pac => pac.Sku).HasMaxLength(400);
            builder.Property(pac => pac.ManufacturerPartNumber).HasMaxLength(400);
            builder.Property(pac => pac.Gtin).HasMaxLength(400);
            builder.Property(pac => pac.OverriddenPrice);

            builder.HasOne(pac => pac.Product)
                .WithMany(p => p.ProductAttributeCombinations)
                .HasForeignKey(pac => pac.ProductId);
        }
    }
}