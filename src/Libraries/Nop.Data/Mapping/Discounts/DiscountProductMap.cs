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
            builder.HasTableName(NopMappingDefaults.DiscountAppliedToProductsTable);
            builder.HasPrimaryKey(mapping => new
            {
                mapping.DiscountId,
                mapping.EntityId
            });

            builder.Property(mapping => mapping.DiscountId).HasColumnName("Discount_Id");
            builder.Property(mapping => mapping.EntityId).HasColumnName("Product_Id");

            builder.Ignore(mapping => mapping.Id);
        }

        #endregion
    }
}