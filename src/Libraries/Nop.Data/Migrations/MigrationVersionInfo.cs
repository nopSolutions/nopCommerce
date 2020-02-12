using System;
using FluentMigrator.Runner.VersionTableInfo;
using Nop.Core;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// The migration version table
    /// </summary>
    public class MigrationVersionInfo : BaseEntity, IVersionTableMetaData
    {
        #region Ctor

        public MigrationVersionInfo()
        {
            var dataConnection = new NopDataConnection();

            var table = dataConnection.GetTable<MigrationVersionInfo>();
            SchemaName = table.SchemaName;
            TableName = nameof(MigrationVersionInfo);
            ColumnName = nameof(Version);
            DescriptionColumnName = nameof(Description);
            AppliedOnColumnName = nameof(AppliedOn);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Version
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Applied on date time
        /// </summary>
        public DateTime AppliedOn { get; set; }

        #region IVersionTableMetaData

        public object ApplicationContext { get; set; }

        public bool OwnsSchema { get; } = true;

        public string SchemaName { get; }

        public string TableName { get; }

        public string ColumnName { get; }

        public string DescriptionColumnName { get; }

        public string UniqueIndexName { get; } = "UC_Version";

        public string AppliedOnColumnName { get; }

        #endregion

        #endregion
    }
}
