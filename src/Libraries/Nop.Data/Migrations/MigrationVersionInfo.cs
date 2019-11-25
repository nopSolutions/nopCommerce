using System;
using FluentMigrator.Runner.VersionTableInfo;
using Nop.Core;

namespace Nop.Data.Migrations
{
    public class MigrationVersionInfo : BaseEntity, IVersionTableMetaData
    {
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

        public long Version { get; set; }
        public string Description { get; set; }
        public DateTime AppliedOn { get; set; }

        public object ApplicationContext { get; set; }
        public bool OwnsSchema { get; } = true;
        public string SchemaName { get; }
        public string TableName { get; }
        public string ColumnName { get; }
        public string DescriptionColumnName { get; }
        public string UniqueIndexName { get; } = "UC_Version";
        public string AppliedOnColumnName { get; }
    }
}
