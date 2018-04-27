using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nop.Core;

namespace Nop.Data.Extensions
{
    /// <summary>
    /// Represents DB context extensions
    /// </summary>
    public static class DbContextExtensions
    {
        #region Utilities
#if EF6

        private static T InnerGetCopy<T>(IDbContext context, T currentCopy, Func<DbEntityEntry<T>, DbPropertyValues> func) where T : BaseEntity
        {
            //Get the database context
            var dbContext = CastOrThrow(context);

            //Get the entity tracking object
            var entry = GetEntityOrReturnNull(currentCopy, dbContext);

            //The output 
            T output = null;

            //Try and get the values
            if (entry != null)
            {
                var dbPropertyValues = func(entry);
                if (dbPropertyValues != null)
                {
                    output = dbPropertyValues.ToObject() as T;
                }
            }

            return output;
        }

        /// <summary>
        /// Gets the entity or return null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentCopy">The current copy.</param>
        /// <param name="dbContext">The db context.</param>
        /// <returns></returns>
        private static DbEntityEntry<T> GetEntityOrReturnNull<T>(T currentCopy, DbContext dbContext) where T : BaseEntity
        {
            return dbContext.ChangeTracker.Entries<T>().FirstOrDefault(e => e.Entity == currentCopy);
        }

        private static DbContext CastOrThrow(IDbContext context)
        {
            var output = context as DbContext;

            if (output == null)
            {
                throw new InvalidOperationException("Context does not support operation.");
            }

            return output;
        }
#endif

        /// <summary>
        /// Get SQL commands from the script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <returns>List of commands</returns>
        private static IList<string> GetCommandsFromScript(string sql)
        {
            var commands = new List<string>();

            //origin from the Microsoft.EntityFrameworkCore.Migrations.SqlServerMigrationsSqlGenerator.Generate method
            sql = Regex.Replace(sql, @"\\\r?\n", string.Empty);
            var batches = Regex.Split(sql, @"^\s*(GO[ \t]+[0-9]+|GO)(?:\s+|$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            for (var i = 0; i < batches.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(batches[i]) || batches[i].StartsWith("GO", StringComparison.OrdinalIgnoreCase))
                    continue;

                var count = 1;
                if (i != batches.Length - 1 && batches[i + 1].StartsWith("GO", StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(batches[i + 1], "([0-9]+)");
                    if (match.Success)
                        count = int.Parse(match.Value);
                }

                var builder = new StringBuilder();
                for (var j = 0; j < count; j++)
                {
                    builder.Append(batches[i]);
                    if (i == batches.Length - 1)
                        builder.AppendLine();
                }

                commands.Add(builder.ToString());
            }

            return commands;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the original copy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="currentCopy">The current copy.</param>
        /// <returns></returns>
        public static T LoadOriginalCopy<T>(this IDbContext context, T currentCopy) where T : BaseEntity
        {
#if EF6
            return InnerGetCopy(context, currentCopy, e => e.OriginalValues);
#else
            return null;
#endif
        }

        /// <summary>
        /// Loads the database copy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="currentCopy">The current copy.</param>
        /// <returns></returns>
        public static T LoadDatabaseCopy<T>(this IDbContext context, T currentCopy) where T : BaseEntity
        {
#if EF6
            return InnerGetCopy(context, currentCopy, e => e.GetDatabaseValues());
#else
            return null;
#endif
        }

        /// <summary>
        /// Drop a plugin table
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="tableName">Table name</param>
        public static void DropPluginTable(this IDbContext context, string tableName)
        {
#if EF6
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            //drop the table
            if (context.Database.SqlQuery<int>("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}", tableName).Any<int>())
            {
                var dbScript = "DROP TABLE [" + tableName + "]";
                context.Database.ExecuteSqlCommand(dbScript);
            }
            context.SaveChanges();
#endif
        }

        /// <summary>
        /// Get table name of entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="context">Context</param>
        /// <returns>Table name</returns>
        public static string GetTableName<T>(this IDbContext context) where T : BaseEntity
        {
#if EF6
            //var tableName = typeof(T).Name;
            //return tableName;

            //this code works only with Entity Framework.
            //If you want to support other database, then use the code above (commented)

            var adapter = ((IObjectContextAdapter)context).ObjectContext;
            var storageModel = (StoreItemCollection)adapter.MetadataWorkspace.GetItemCollection(DataSpace.SSpace);
            var containers = storageModel.GetItems<EntityContainer>();
            var entitySetBase = containers.SelectMany(c => c.BaseEntitySets.Where(bes => bes.Name == typeof(T).Name)).First();

            // Here are variables that will hold table and schema name
            var tableName = entitySetBase.MetadataProperties.First(p => p.Name == "Table").Value.ToString();
            //string schemaName = productEntitySetBase.MetadataProperties.First(p => p.Name == "Schema").Value.ToString();
            return tableName;
#else
            return null;
#endif
        }

        /// <summary>
        /// Get column maximum length
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="entityTypeName">Entity type name</param>
        /// <param name="columnName">Column name</param>
        /// <returns>Maximum length. Null if such rule does not exist</returns>
        public static int? GetColumnMaxLength(this IDbContext context, string entityTypeName, string columnName)
        {
#if EF6
            var rez = GetColumnsMaxLength(context, entityTypeName, columnName);
            return rez.ContainsKey(columnName) ? rez[columnName] as int? : null;
#else
            return null;
#endif
        }

        /// <summary>
        /// Get columns maximum length
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="entityTypeName">Entity type name</param>
        /// <param name="columnNames">Column names</param>
        /// <returns></returns>
        public static IDictionary<string, int> GetColumnsMaxLength(this IDbContext context, string entityTypeName, params string[] columnNames)
        {
#if EF6
            var fieldFacets = GetFieldFacets(context, entityTypeName, "String", columnNames);

            var queryResult = fieldFacets
                .Select(f => new { Name = f.Key, MaxLength = f.Value["MaxLength"].Value })
                .Where(p => int.TryParse(p.MaxLength.ToString(), out int _))
                .ToDictionary(p => p.Name, p => Convert.ToInt32(p.MaxLength));

            return queryResult;
#else
            return null;
#endif
        }


        /// <summary>
        /// Get maximum decimal values
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="entityTypeName">Entity type name</param>
        /// <param name="columnNames">Column names</param>
        /// <returns></returns>
        public static IDictionary<string, decimal> GetDecimalMaxValue(this IDbContext context, string entityTypeName, params string[] columnNames)
        {
#if EF6
            var fieldFacets = GetFieldFacets(context, entityTypeName, "Decimal", columnNames);

            return fieldFacets.ToDictionary(p => p.Key, p => int.Parse(p.Value["Precision"].Value.ToString()) - int.Parse(p.Value["Scale"].Value.ToString()))
                .ToDictionary(p => p.Key, p => new decimal(Math.Pow(10, p.Value)));
#else
            return null;
#endif
        }

#if EF6
        private static Dictionary<string, ReadOnlyMetadataCollection<Facet>> GetFieldFacets(this IDbContext context,
            string entityTypeName, string edmTypeName, params string[] columnNames)
        {
            //original: http://stackoverflow.com/questions/5081109/entity-framework-4-0-automatically-truncate-trim-string-before-insert

            var entType = Type.GetType(entityTypeName);
            var adapter = ((IObjectContextAdapter)context).ObjectContext;
            var metadataWorkspace = adapter.MetadataWorkspace;
            var q = from meta in metadataWorkspace.GetItems(DataSpace.CSpace).Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                    from p in (meta as EntityType).Properties.Where(p => columnNames.Contains(p.Name) && p.TypeUsage.EdmType.Name == edmTypeName)
                    select p;

            var queryResult = q.Where(p =>
            {
                var match = p.DeclaringType.Name == entityTypeName;
                if (!match && entType != null)
                {
                    //Is a fully qualified name....
                    match = entType.Name == p.DeclaringType.Name;
                }

                return match;

            }).ToDictionary(p => p.Name, p => p.TypeUsage.Facets);

            return queryResult;
        }
#endif

        /// <summary>
        /// Get database name
        /// </summary>
        /// <param name="context">DB context</param>
        /// <returns>Database name</returns>
        public static string DbName(this IDbContext context)
        {
#if EF6
            var connection = ((IObjectContextAdapter)context).ObjectContext.Connection as EntityConnection;
            if (connection == null)
                return string.Empty;

            return connection.StoreConnection.Database;
#else
            return null;
#endif
        }

        /// <summary>
        /// Execute commands from the SQL script against the context database
        /// </summary>
        /// <param name="context">DB context</param>
        /// <param name="sql">SQL script</param>
        public static void ExecuteSqlScript(this IDbContext context, string sql)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var sqlCommands = GetCommandsFromScript(sql);
            foreach (var command in sqlCommands)
                context.ExecuteSqlCommand(command);
        }

        /// <summary>
        /// Execute commands from a file with SQL script against the context database
        /// </summary>
        /// <param name="context">DB context</param>
        /// <param name="filePath">Path to the file</param>
        public static void ExecuteSqlScriptFromFile(this IDbContext context, string filePath)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!File.Exists(filePath))
                return;

            context.ExecuteSqlScript(File.ReadAllText(filePath));
        }

        #endregion
    }
}