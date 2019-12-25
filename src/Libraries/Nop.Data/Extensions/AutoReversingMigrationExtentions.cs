using System;
using System.Data;
using System.Linq;
using System.Text;
using FluentMigrator;
using FluentMigrator.Builders.Create.Index;

namespace Nop.Data.Extensions
{
    public static class AutoReversingMigrationExtentions
    {
        public static void AddForeignKey(this MigrationBase migration, string foreignTable, string foreignColumn, string primaryTable, string primaryColumn, Rule onDeleteRule = Rule.None, bool addIndex = true)
        {

            migration.Create.ForeignKey().FromTable(foreignTable)
                .ForeignColumn(foreignColumn)
                .ToTable(primaryTable)
                .PrimaryColumn(primaryColumn)
                .OnDelete(onDeleteRule);

            if (!addIndex)
                return;

            migration.AddIndex(string.Empty, foreignTable, i=>i.Ascending(), foreignColumn);
        }

        public static ICreateIndexOnColumnSyntax AddIndex(this MigrationBase migration, string indexName, string tableName, Func<ICreateIndexColumnOptionsSyntax, ICreateIndexOnColumnSyntax> options, params string [] columnNames)
        {
            if(!columnNames.Any())
                return null;

            if (!string.IsNullOrEmpty(indexName) && migration.Schema.Table(tableName).Index(indexName).Exists())
                return null;

            var index = migration.Create.Index(indexName).OnTable(tableName);

            foreach (var column in columnNames)
            {
                options(index.OnColumn(column));
            }

            return index.WithOptions().NonClustered();
        }
    }
}
