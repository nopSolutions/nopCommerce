using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class RelatedProductMap : NopEntityTypeConfiguration<RelatedProduct>
    {
        public override void Configure(EntityTypeBuilder<RelatedProduct> builder)
        {
            base.Configure(builder);
            builder.ToTable("RelatedProduct");
            builder.HasKey(c => c.Id);
        }
    }
}