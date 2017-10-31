using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ProductAttributeMap : NopEntityTypeConfiguration<ProductAttribute>
    {
        public override void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProductAttribute");
            builder.HasKey(pa => pa.Id);
            builder.Property(pa => pa.Name).IsRequired();
        }
    }
}