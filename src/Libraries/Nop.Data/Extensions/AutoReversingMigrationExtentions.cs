using System.Data;
using System.Text;
using FluentMigrator;

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

            migration.Create.Index(indexName).OnTable(foreignTable).OnColumn(foreignColumn).Ascending().WithOptions().NonClustered();
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
