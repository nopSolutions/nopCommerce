using LinqToDB.Mapping;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    /// <summary>
    /// Represents a discount-product mapping configuration
    /// </summary>
    public partial class DiscountProductMap : NopEntityTypeConfiguration<DiscountProductMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<DiscountProductMapping> builder)
        {
            builder.HasTableName(nameof(DiscountProductMapping));
            builder.HasPrimaryKey(mapping => new
            {
                mapping.DiscountId,
                mapping.EntityId
            });

            builder.Property(mapping => mapping.DiscountId);
            builder.Property(mapping => mapping.EntityId);

            builder.Ignore(mapping => mapping.Id);
        }

        #endregion
    }
}