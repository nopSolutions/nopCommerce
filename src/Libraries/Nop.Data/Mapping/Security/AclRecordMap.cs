using LinqToDB.Mapping;
using Nop.Core.Domain.Security;

namespace Nop.Data.Mapping.Security
{
    /// <summary>
    /// Represents an ACL record mapping configuration
    /// </summary>
    public partial class AclRecordMap : NopEntityTypeConfiguration<AclRecord>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<AclRecord> builder)
        {
            builder.HasTableName(nameof(AclRecord));

            builder.Property(record => record.EntityName).HasLength(400).IsNullable(false);
            builder.Property(aclrecord => aclrecord.EntityId);
            builder.Property(aclrecord => aclrecord.CustomerRoleId);
        }

        #endregion
    }
}