using LinqToDB.Mapping;
using Nop.Data.Migrations;

namespace Nop.Data.Mapping.Migrations
{
    /// <summary>
    /// Represents a migration version configuration
    /// </summary>
    public partial class MigrationVersionInfoMap : NopEntityTypeConfiguration<MigrationVersionInfo>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<MigrationVersionInfo> builder)
        {
            builder.HasTableName(nameof(MigrationVersionInfo));

            builder.Property(versionInfo => versionInfo.Version);
            builder.Property(versionInfo => versionInfo.Description);
            builder.Property(versionInfo => versionInfo.AppliedOn);

            builder.Ignore(versionInfo => versionInfo.Id);
            builder.Ignore(versionInfo => versionInfo.ApplicationContext);
            builder.Ignore(versionInfo => versionInfo.OwnsSchema);
            builder.Ignore(versionInfo => versionInfo.SchemaName);
            builder.Ignore(versionInfo => versionInfo.TableName);
            builder.Ignore(versionInfo => versionInfo.ColumnName);
            builder.Ignore(versionInfo => versionInfo.DescriptionColumnName);
            builder.Ignore(versionInfo => versionInfo.UniqueIndexName);
            builder.Ignore(versionInfo => versionInfo.AppliedOnColumnName);
        }

        #endregion
    }
}