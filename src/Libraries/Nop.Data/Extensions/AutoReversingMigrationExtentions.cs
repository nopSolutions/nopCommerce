using System;
using System.Data;
using System.Linq;
using System.Text;
using FluentMigrator;
using FluentMigrator.Builders.Create.Index;

namespace Nop.Data.Extensions
{
    /// <summary>
    /// MigrationBase extensions
    /// </summary>
    public static class AutoReversingMigrationExtensions
    {
        #region Utilities

        /// <summary>
        /// Generates the foreign key name
        /// </summary>
        /// <param name="foreignTable">Name of the foreign table</param>
        /// <param name="foreignColumn">Name of the foreign column</param>
        /// <param name="primaryTable">Name of the primary table</param>
        /// <param name="primaryColumn">Name of the primary column</param>
        /// <param name="isShort">Specifies whether to use the short version of the key; by default set to true</param>
        /// <returns></returns>
        private static string GetForeignKeyName(string foreignTable, string foreignColumn, string primaryTable, string primaryColumn, bool isShort = true)
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

        #endregion

        #region Methods

        /// <summary>
        /// Creates a foreign key
        /// </summary>
        /// <param name="migration">Migration class</param>
        /// <param name="foreignTable">Name of the foreign table</param>
        /// <param name="foreignColumn">Name of the foreign column</param>
        /// <param name="primaryTable">Name of the primary table</param>
        /// <param name="primaryColumn">Name of the primary column</param>
        /// <param name="onDeleteRule">The rule to apply to DELETE operations</param>
        /// <param name="addIndex">Indicates whether to add an index foreign key column; by default set to true</param>
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

            if (migration.Schema.Table(foreignTable).Index(indexName).Exists())
                return;

            migration.AddIndex(indexName, foreignTable, i => i.Ascending(), foreignColumn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="migration">Migration class</param>
        /// <param name="indexName">Name of the index</param>
        /// <param name="tableName">Name of the table</param>
        /// <param name="options">Options to index</param>
        /// <param name="columnNames">Name of the columns</param>
        /// <returns></returns>
        public static ICreateIndexOnColumnSyntax AddIndex(this MigrationBase migration, string indexName, string tableName, Func<ICreateIndexColumnOptionsSyntax, ICreateIndexOnColumnSyntax> options, params string[] columnNames)
        {
            if (!columnNames.Any())
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

        #endregion
    }
}
