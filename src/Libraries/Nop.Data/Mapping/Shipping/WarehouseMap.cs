using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        public override void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable(nameof(Warehouse));
            builder.HasKey(warehouse => warehouse.Id);

            builder.Property(warehouse => warehouse.Name).HasMaxLength(400).IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}