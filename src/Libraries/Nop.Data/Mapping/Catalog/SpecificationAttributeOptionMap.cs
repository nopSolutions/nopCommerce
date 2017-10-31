using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class SpecificationAttributeOptionMap : NopEntityTypeConfiguration<SpecificationAttributeOption>
    {
        public override void Configure(EntityTypeBuilder<SpecificationAttributeOption> builder)
        {
            base.Configure(builder);
            builder.ToTable("SpecificationAttributeOption");
            builder.HasKey(sao => sao.Id);
            builder.Property(sao => sao.Name).IsRequired();
            builder.Property(sao => sao.ColorSquaresRgb).HasMaxLength(100);

            builder.HasOne(sao => sao.SpecificationAttribute)
                .WithMany(sa => sa.SpecificationAttributeOptions)
                .HasForeignKey(sao => sao.SpecificationAttributeId)
                .IsRequired(true);
        }
    }
}