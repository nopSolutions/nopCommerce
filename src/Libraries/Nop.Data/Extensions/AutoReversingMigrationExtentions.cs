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
            var keyName = GetForeignKeyName(foreignTable, foreignColumn, primaryTable, primaryColumn);

            if (migration.Schema.Table(foreignTable).Constraint(keyName).Exists())
                keyName = GetForeignKeyName(foreignTable, foreignColumn, primaryTable, primaryColumn, false);

            migration.Create.ForeignKey(keyName).FromTable(foreignTable)
                .ForeignColumn(foreignColumn)
                .ToTable(primaryTable)
                .PrimaryColumn(primaryColumn)
                .OnDelete(onDeleteRule);

            if (!addIndex)
                return;

            var indexName = $"IX_{foreignTable}_{foreignColumn}";

            if(migration.Schema.Table(foreignTable).Index(indexName).Exists())
                return;

            migration.AddIndex(indexName, foreignTable, i=>i.Ascending(), foreignColumn);
        }

        public static ICreateIndexOnColumnSyntax AddIndex(this MigrationBase migration, string indexName, string tableName, Func<ICreateIndexColumnOptionsSyntax, ICreateIndexOnColumnSyntax> options, params string [] columnNames)
        {
            if(!columnNames.Any())
                return null;

            if (migration.Schema.Table(tableName).Index(indexName).Exists())
                return null;

            var index = migration.Create.Index(indexName).OnTable(tableName);

            foreach (var column in columnNames)
            {
                options(index.OnColumn(column));
            }

            return index.WithOptions().NonClustered();
        }

        private static string GetForeignKeyName(string foreignTable, string foreignColumn, string primaryTable, string primaryColumn, bool isShort=true)
        {
            var sb = new StringBuilder();

            sb.Append("FK_");
            sb.Append(foreignTable);
            sb.Append("_");

            sb.Append(isShort
                ? $"{primaryTable}_{primaryTable}{primaryColumn}"
                : $"{foreignColumn}_{primaryTable}_{primaryColumn}");


            return sb.ToString();
        }
    }
}
