using LinqToDB.Mapping;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    /// <summary>
    /// Represents a discount-category mapping configuration
    /// </summary>
    public partial class DiscountCategoryMap : NopEntityTypeConfiguration<DiscountCategoryMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<DiscountCategoryMapping> builder)
        {
            builder.HasTableName(nameof(DiscountCategoryMapping));
            builder.HasPrimaryKey(mapping => new { mapping.DiscountId, mapping.EntityId });

            builder.Property(mapping => mapping.DiscountId);
            builder.Property(mapping => mapping.EntityId);

            builder.Ignore(mapping => mapping.Id);
        }

        #endregion
    }
}