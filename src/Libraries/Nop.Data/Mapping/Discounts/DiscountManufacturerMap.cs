using LinqToDB.Mapping;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    /// <summary>
    /// Represents a discount-manufacturer mapping configuration
    /// </summary>
    public partial class DiscountManufacturerMap : NopEntityTypeConfiguration<DiscountManufacturerMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<DiscountManufacturerMapping> builder)
        {
            builder.HasTableName(nameof(DiscountManufacturerMapping));
            builder.HasPrimaryKey(mapping => new { mapping.DiscountId, mapping.EntityId });

            builder.Property(mapping => mapping.DiscountId);
            builder.Property(mapping => mapping.EntityId);

            builder.Ignore(mapping => mapping.Id);
        }

        #endregion
    }
}