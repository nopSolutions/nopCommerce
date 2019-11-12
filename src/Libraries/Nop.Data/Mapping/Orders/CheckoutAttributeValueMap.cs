using LinqToDB.Mapping;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a checkout attribute value mapping configuration
    /// </summary>
    public partial class CheckoutAttributeValueMap : NopEntityTypeConfiguration<CheckoutAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<CheckoutAttributeValue> builder)
        {
            builder.HasTableName(nameof(CheckoutAttributeValue));

            builder.Property(value => value.Name).HasLength(400);
            builder.HasColumn(value => value.Name).IsColumnRequired();
            builder.Property(value => value.ColorSquaresRgb).HasLength(100);
            builder.Property(value => value.PriceAdjustment).HasDbType("decimal(18, 4)");
            builder.Property(value => value.WeightAdjustment).HasDbType("decimal(18, 4)");

            builder.Property(value => value.CheckoutAttributeId);
            builder.Property(value => value.IsPreSelected);
            builder.Property(value => value.DisplayOrder);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(value => value.CheckoutAttribute)
            //    .WithMany(attribute => attribute.CheckoutAttributeValues)
            //    .HasForeignKey(value => value.CheckoutAttributeId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}