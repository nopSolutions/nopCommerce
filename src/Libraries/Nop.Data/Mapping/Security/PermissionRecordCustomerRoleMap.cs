using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;

namespace Nop.Data.Mapping.Security
{
    /// <summary>
    /// Represents a permission record-customer role mapping configuration
    /// </summary>
    public partial class PermissionRecordCustomerRoleMap : NopEntityTypeConfiguration<PermissionRecordCustomerRoleMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<PermissionRecordCustomerRoleMapping> builder)
        {
            builder.ToTable(NopMappingDefaults.PermissionRecordRoleTable);
            builder.HasKey(mapping => new { mapping.PermissionRecordId, mapping.CustomerRoleId});

            builder.Property(mapping => mapping.PermissionRecordId).HasColumnName("PermissionRecord_Id");
            builder.Property(mapping => mapping.CustomerRoleId).HasColumnName("CustomerRole_Id");

            builder.HasOne<CustomerRole>().WithMany().HasForeignKey(mapping => mapping.CustomerRoleId).IsRequired();

            builder.HasOne<PermissionRecord>().WithMany().HasForeignKey(mapping => mapping.PermissionRecordId).IsRequired();

            builder.Ignore(mapping => mapping.Id);

            base.Configure(builder);
        }

        #endregion
    }
}