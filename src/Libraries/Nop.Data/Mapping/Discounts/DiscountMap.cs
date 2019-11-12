using LinqToDB.Mapping;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    /// <summary>
    /// Represents a discount mapping configuration
    /// </summary>
    public partial class DiscountMap : NopEntityTypeConfiguration<Discount>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Discount> builder)
        {
            builder.HasTableName(nameof(Discount));

            builder.Property(discount => discount.Name).HasLength(200);
            builder.HasColumn(discount => discount.Name).IsColumnRequired();
            builder.Property(discount => discount.CouponCode).HasLength(100);
            builder.Property(discount => discount.DiscountPercentage).HasDbType("decimal(18, 4)");
            builder.Property(discount => discount.DiscountAmount).HasDbType("decimal(18, 4)");
            builder.Property(discount => discount.MaximumDiscountAmount).HasDbType("decimal(18, 4)");

            builder.Property(discount => discount.DiscountTypeId);
            builder.Property(discount => discount.UsePercentage);
            builder.Property(discount => discount.StartDateUtc);
            builder.Property(discount => discount.EndDateUtc);
            builder.Property(discount => discount.RequiresCouponCode);
            builder.Property(discount => discount.IsCumulative);
            builder.Property(discount => discount.DiscountLimitationId);
            builder.Property(discount => discount.LimitationTimes);
            builder.Property(discount => discount.MaximumDiscountedQuantity);
            builder.Property(discount => discount.AppliedToSubCategories);

            builder.Ignore(discount => discount.DiscountType);
            builder.Ignore(discount => discount.DiscountLimitation);

            //TODO: 239 Try to add ForeignKey
            //builder.HasMany(discount => discount.DiscountRequirements)
            //    .WithOne(requirement => requirement.Discount)
            //    .HasForeignKey(requirement => requirement.DiscountId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}