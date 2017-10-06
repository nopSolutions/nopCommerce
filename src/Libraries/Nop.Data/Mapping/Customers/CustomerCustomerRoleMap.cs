using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public class CustomerCustomerRoleMap : NopEntityTypeConfiguration<Customer_CustomerRole_Mapping>
    {
        public override void Configure(EntityTypeBuilder<Customer_CustomerRole_Mapping> builder)
        {
            base.Configure(builder);
            builder.HasKey(t => new { t.CustomerId, t.CustomerRoleId });
            builder.HasOne(ca => ca.Customer)
                .WithMany(ca => ca.CustomerCustomerRoles)
                .HasForeignKey(ca => ca.CustomerId);

            builder.HasOne(ca => ca.CustomerRole)
                .WithMany(ca => ca.Customers)
                .HasForeignKey(ca => ca.CustomerRoleId);
        }
    }
}
