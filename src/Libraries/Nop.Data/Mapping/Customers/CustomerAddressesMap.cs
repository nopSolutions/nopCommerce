using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public class CustomerAddressesMap : NopEntityTypeConfiguration<CustomerAddresses>
    {
        public override void Configure(EntityTypeBuilder<CustomerAddresses> builder)
        {
            base.Configure(builder);
            builder.HasKey(t => new { t.CustomerId, t.AddressId });
            builder.HasOne(ca => ca.Customer)
                .WithMany(ca => ca.CustomerAddresses)
                .HasForeignKey(ca => ca.CustomerId);

            builder.HasOne(ca => ca.Address)
                .WithMany()
                .HasForeignKey(ca => ca.AddressId);
        }
    }
}
