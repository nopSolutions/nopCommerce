using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public class WarehouseMap : NopEntityTypeConfiguration<Warehouse>
    {
        public override void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            base.Configure(builder);
            builder.ToTable("Warehouse");
            builder.HasKey(wh => wh.Id);
            builder.Property(wh => wh.Name).IsRequired().HasMaxLength(400);
        }
    }
}
