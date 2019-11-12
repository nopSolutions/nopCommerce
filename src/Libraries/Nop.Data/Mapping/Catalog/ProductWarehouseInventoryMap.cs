using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product warehouse inventory mapping configuration
    /// </summary>
    public partial class ProductWarehouseInventoryMap : NopEntityTypeConfiguration<ProductWarehouseInventory>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductWarehouseInventory> builder)
        {
            builder.HasTableName(nameof(ProductWarehouseInventory));

            builder.Property(productWarehouseInventory => productWarehouseInventory.ProductId);
            builder.Property(productWarehouseInventory => productWarehouseInventory.WarehouseId);
            builder.Property(productWarehouseInventory => productWarehouseInventory.StockQuantity);
            builder.Property(productWarehouseInventory => productWarehouseInventory.ReservedQuantity);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(productWarehouseInventory => productWarehouseInventory.Product)
            //    .WithMany(product => product.ProductWarehouseInventory)
            //    .HasForeignKey(productWarehouseInventory => productWarehouseInventory.ProductId)
            //    .IsColumnRequired();

            //builder.HasOne(productWarehouseInventory => productWarehouseInventory.Warehouse)
            //    .WithMany()
            //    .HasForeignKey(productWarehouseInventory => productWarehouseInventory.WarehouseId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}