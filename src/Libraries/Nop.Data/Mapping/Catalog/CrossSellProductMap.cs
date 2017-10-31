using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class CrossSellProductMap : NopEntityTypeConfiguration<CrossSellProduct>
    {
        public override void Configure(EntityTypeBuilder<CrossSellProduct> builder)
        {
            base.Configure(builder);
            builder.ToTable("CrossSellProduct");
            builder.HasKey(c => c.Id);
        }
    }
}