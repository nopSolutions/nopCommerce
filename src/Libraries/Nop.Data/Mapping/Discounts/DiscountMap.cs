using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        public override void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.ToTable(nameof(Discount));
            builder.HasKey(discount => discount.Id);

            builder.Property(discount => discount.Name).HasMaxLength(200).IsRequired();
            builder.Property(discount => discount.CouponCode).HasMaxLength(100);
            builder.Property(discount => discount.DiscountPercentage).HasColumnType("decimal(18, 4)");
            builder.Property(discount => discount.DiscountAmount).HasColumnType("decimal(18, 4)");
            builder.Property(discount => discount.MaximumDiscountAmount).HasColumnType("decimal(18, 4)");

            builder.HasMany(discount => discount.DiscountRequirements)
                .WithOne(requirement => requirement.Discount)
                .HasForeignKey(requirement => requirement.DiscountId)
                .IsRequired();

#if EF6
            builder.HasMany(discount => discount.AppliedToCategories)
                .WithMany(category => category.AppliedDiscounts)
                .Map(mapping => mapping.ToTable("Discount_AppliedToCategories"));

            builder.HasMany(discount => discount.AppliedToManufacturers)
                .WithMany(manufacturer => manufacturer.AppliedDiscounts)
                .Map(mapping => mapping.ToTable("Discount_AppliedToManufacturers"));

            builder.HasMany(discount => discount.AppliedToProducts)
                .WithMany(product => product.AppliedDiscounts)
                .Map(mapping => mapping.ToTable("Discount_AppliedToProducts"));
#else
            builder.Ignore(discount => discount.AppliedToCategories);
            builder.Ignore(discount => discount.AppliedToManufacturers);
            builder.Ignore(discount => discount.AppliedToProducts);
#endif

            builder.Ignore(discount => discount.DiscountType);
            builder.Ignore(discount => discount.DiscountLimitation);

            //add custom configuration
            this.PostConfigure(builder);
        }

        #endregion
    }
}