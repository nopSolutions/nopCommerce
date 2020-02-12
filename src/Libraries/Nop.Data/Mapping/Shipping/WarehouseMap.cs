using LinqToDB.Mapping;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Represents a warehouse mapping configuration
    /// </summary>
    public partial class WarehouseMap : NopEntityTypeConfiguration<Warehouse>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Warehouse> builder)
        {
            builder.HasTableName(nameof(Warehouse));

            builder.Property(warehouse => warehouse.Name).HasLength(400).IsNullable(false);
            builder.Property(warehouse => warehouse.AdminComment);
            builder.Property(warehouse => warehouse.AddressId);
        }

        #endregion
    }
}