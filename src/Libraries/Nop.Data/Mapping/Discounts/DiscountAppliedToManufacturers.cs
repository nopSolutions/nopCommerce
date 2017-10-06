using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    public class DiscountAppliedToManufacturersMap : NopEntityTypeConfiguration<Discount_AppliedToManufacturers>
    {
        public override void Configure(EntityTypeBuilder<Discount_AppliedToManufacturers> builder)
        {
            base.Configure(builder);
            builder.HasKey(t => new { t.ManufacturerId, t.DiscountId });
            builder.HasOne(ca => ca.Discount)
                .WithMany(ca => ca.AppliedToManufacturers)
                .HasForeignKey(ca => ca.DiscountId);

            builder.HasOne(ca => ca.Manufacturer)
                .WithMany(ca => ca.AppliedDiscounts)
                .HasForeignKey(ca => ca.ManufacturerId);
        }
    }
}
