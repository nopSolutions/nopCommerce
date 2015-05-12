using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    public partial class DiscountMap : NopEntityTypeConfiguration<Discount>
    {
        public DiscountMap()
        {
            this.ToTable("Discount");
            this.HasKey(d => d.Id);
            this.Property(d => d.Name).IsRequired().HasMaxLength(200);
            this.Property(d => d.CouponCode).HasMaxLength(100);
            this.Property(d => d.DiscountPercentage).HasPrecision(18, 4);
            this.Property(d => d.DiscountAmount).HasPrecision(18, 4);
            this.Property(d => d.MaximumDiscountAmount).HasPrecision(18, 4);

            this.Ignore(d => d.DiscountType);
            this.Ignore(d => d.DiscountLimitation);

            this.HasMany(dr => dr.DiscountRequirements)
                .WithRequired(d => d.Discount)
                .HasForeignKey(dr => dr.DiscountId);

            this.HasMany(dr => dr.AppliedToCategories)
                .WithMany(c => c.AppliedDiscounts)
                .Map(m => m.ToTable("Discount_AppliedToCategories"));

            this.HasMany(dr => dr.AppliedToManufacturers)
                .WithMany(c => c.AppliedDiscounts)
                .Map(m => m.ToTable("Discount_AppliedToManufacturers"));
            
            this.HasMany(dr => dr.AppliedToProducts)
                .WithMany(p => p.AppliedDiscounts)
                .Map(m => m.ToTable("Discount_AppliedToProducts"));
        }
    }
}