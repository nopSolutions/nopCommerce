using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    class ShippingMethodRestrictionsMap : NopEntityTypeConfiguration<ShippingMethodCountry>
    {
        public override void Configure(EntityTypeBuilder<ShippingMethodCountry> builder)
        {
            base.Configure(builder);
            builder.HasKey(t => new { t.CountryId, t.ShippingMethodId });
            builder.HasOne(ca => ca.Country)
                .WithMany(ca => ca.RestrictedShippingMethods)
                .HasForeignKey(ca => ca.CountryId);

            builder.HasOne(ca => ca.ShippingMethod)
                .WithMany(ca => ca.RestrictedCountries)
                .HasForeignKey(ca => ca.ShippingMethodId);
        }
    }
}
