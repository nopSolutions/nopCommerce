using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductTemplateMap : NopEntityTypeConfiguration<ProductTemplate>
    {
        public override void Configure(EntityTypeBuilder<ProductTemplate> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProductTemplate");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(400);
            builder.Property(p => p.ViewPath).IsRequired().HasMaxLength(400);
        }
    }
}