using LinqToDB.Mapping;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Represents a product availability range mapping configuration
    /// </summary>
    public partial class ProductAvailabilityRangeMap : NopEntityTypeConfiguration<ProductAvailabilityRange>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductAvailabilityRange> builder)
        {
            builder.HasTableName(nameof(ProductAvailabilityRange));

            builder.Property(range => range.Name).HasLength(400);
            builder.HasColumn(range => range.Name).IsColumnRequired();

            builder.Property(range => range.DisplayOrder);
        }

        #endregion
    }
}