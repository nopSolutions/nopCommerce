using LinqToDB.Mapping;
using Nop.Core.Domain.Security;

namespace Nop.Data.Mapping.Security
{
    /// <summary>
    /// Represents a permission record mapping configuration
    /// </summary>
    public partial class PermissionRecordMap : NopEntityTypeConfiguration<PermissionRecord>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<PermissionRecord> builder)
        {
            builder.HasTableName(nameof(PermissionRecord));

            builder.HasColumn(record => record.Name).IsColumnRequired();
            builder.Property(record => record.SystemName).HasLength(255);
            builder.Property(record => record.Category).HasLength(255);
            builder.HasColumn(record => record.SystemName).IsColumnRequired();
            builder.HasColumn(record => record.Category).IsColumnRequired();
        }

        #endregion
    }
}