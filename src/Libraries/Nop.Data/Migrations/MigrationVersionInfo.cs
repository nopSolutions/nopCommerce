using FluentMigrator.Runner.VersionTableInfo;
using Nop.Core;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// The migration version table
    /// </summary>
    public partial class MigrationVersionInfo : BaseEntity, IVersionTableMetaData
    {
        #region Ctor

        public MigrationVersionInfo()
        {
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

        /// <summary>
        /// Provides access to <code>ApplicationContext</code> object.
        /// </summary>
        /// <remarks>
        /// ApplicationContext value is set by FluentMigrator immediately after instantiation of a class
        /// implementing <code>IVersionTableMetaData</code> and before any of properties of <code>IVersionTableMetaData</code>
        /// is called. Properties can use <code>ApplicationContext</code> value to implement context-depending logic.
        /// </remarks>
        public object ApplicationContext { get; set; }

        /// <summary>
        /// Owns schema
        /// </summary>
        public bool OwnsSchema { get; } = true;

        /// <summary>
        /// Schema name
        /// </summary>
        public string SchemaName { get; } = string.Empty;

        /// <summary>
        /// Table name
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// "Version" column name
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// "Description" column name
        /// </summary>
        public string DescriptionColumnName { get; }

        /// <summary>
        /// Unique index name
        /// </summary>
        public string UniqueIndexName { get; } = "UC_Version";

        /// <summary>
        /// "Applied on" column name
        /// </summary>
        public string AppliedOnColumnName { get; }

        #endregion

        #endregion
    }
}
