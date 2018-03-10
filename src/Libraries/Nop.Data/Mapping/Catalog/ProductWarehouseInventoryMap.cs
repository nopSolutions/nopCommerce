using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ProductWarehouseInventoryMap : NopEntityTypeConfiguration<ProductWarehouseInventory>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ProductWarehouseInventoryMap()
        {
            this.ToTable("ProductWarehouseInventory");
            this.HasKey(x => x.Id);

            this.HasRequired(x => x.Product)
                .WithMany(p => p.ProductWarehouseInventory)
                .HasForeignKey(x => x.ProductId)
                .WillCascadeOnDelete(true);

            this.HasRequired(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .WillCascadeOnDelete(true);
        }
    }
}