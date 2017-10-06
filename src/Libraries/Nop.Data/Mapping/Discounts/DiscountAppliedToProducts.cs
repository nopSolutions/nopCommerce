using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    public class DiscountAppliedToProductsMap : NopEntityTypeConfiguration<Discount_AppliedToProducts>
    {
        public override void Configure(EntityTypeBuilder<Discount_AppliedToProducts> builder)
        {
            base.Configure(builder);
            builder.HasKey(t => new { t.ProductId, t.DiscountId });
            builder.HasOne(ca => ca.Discount)
                .WithMany(ca => ca.AppliedToProducts)
                .HasForeignKey(ca => ca.DiscountId);

            builder.HasOne(ca => ca.Product)
                .WithMany(ca => ca.AppliedDiscounts)
                .HasForeignKey(ca => ca.ProductId);
        }
    }
}
