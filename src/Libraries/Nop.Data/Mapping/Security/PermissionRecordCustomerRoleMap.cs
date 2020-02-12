using LinqToDB.Mapping;
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
        public override void Configure(EntityMappingBuilder<PermissionRecordCustomerRoleMapping> builder)
        {
            builder.HasTableName(NopMappingDefaults.PermissionRecordRoleTable);
            builder.HasPrimaryKey(mapping => new
            {
                mapping.PermissionRecordId,
                mapping.CustomerRoleId
            });

            builder.Property(mapping => mapping.PermissionRecordId).HasColumnName("PermissionRecord_Id");
            builder.Property(mapping => mapping.CustomerRoleId).HasColumnName("CustomerRole_Id");

            builder.Ignore(mapping => mapping.Id);
        }

        #endregion
    }
}