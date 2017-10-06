using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public class CustomerRole_PermissionRecordMap : NopEntityTypeConfiguration<CustomerRole_PermissionRecord>
    {
        public override void Configure(EntityTypeBuilder<CustomerRole_PermissionRecord> builder)
        {
            base.Configure(builder);
            builder.HasKey(t => new {t.CustomerRoleId, t.PermissionRecordId});
            builder.HasOne(ca => ca.CustomerRole)
                .WithMany(ca => ca.PermissionRecords)
                .HasForeignKey(ca => ca.CustomerRoleId);

            builder.HasOne(ca => ca.PermissionRecord)
                .WithMany(ca => ca.CustomerRoles)
                .HasForeignKey(ca => ca.PermissionRecordId);
        }
    }
}
