using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    public class DiscountAppliedToCategoriesMap : NopEntityTypeConfiguration<Discount_AppliedToCategories>
    {
        public override void Configure(EntityTypeBuilder<Discount_AppliedToCategories> builder)
        {
            base.Configure(builder);
            builder.HasKey(t => new {t.CategoryId, t.DiscountId});
            builder.HasOne(ca => ca.Discount)
                .WithMany(ca => ca.AppliedToCategories)
                .HasForeignKey(ca => ca.DiscountId);

            builder.HasOne(ca => ca.Category)
                .WithMany(ca => ca.AppliedDiscounts)
                .HasForeignKey(ca => ca.CategoryId);
        }
    }
}
