using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public class WarehouseMap : NopEntityTypeConfiguration<Warehouse>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public WarehouseMap()
        {
            this.ToTable("Warehouse");
            this.HasKey(wh => wh.Id);
            this.Property(wh => wh.Name).IsRequired().HasMaxLength(400);
        }
    }
}
