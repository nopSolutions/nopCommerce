using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.MySql;
using LinqToDB.SqlQuery;
using MySql.Data.MySqlClient;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data.Migrations;

namespace Nop.Data
{
    public class MySqlNopDataProvider : BaseDataProvider, INopDataProvider
    {
        #region Utils

        /// <summary>
        /// Check whether backups are supported
        /// </summary>
        protected void CheckBackupSupported()
        {
            if (!BackupSupported)
                throw new DataException("This database does not support backup");
        }

        protected MySqlConnectionStringBuilder GetConnectionStringBuilder()
        {
            return new MySqlConnectionStringBuilder(CurrentConnectionString);
        }

        /// <summary>
        /// Get SQL commands from the script
        /// </summary>
        /// <param name="sql">SQL script</param>
        /// <returns>List of commands</returns>
        private static IList<string> GetCommandsFromScript(string sql)
        {
            var commands = new List<string>();

            var batches = Regex.Split(sql, @"DELIMITER \;", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            if (batches?.Length > 0)
            {
                commands.AddRange(
                    batches
                        .Where(b => !string.IsNullOrWhiteSpace(b))
                        .Select(b =>
                        {
                            b = Regex.Replace(b, @"(DELIMITER )?\$\$", string.Empty);
                            b = Regex.Replace(b, @"#(.*?)\r?\n", "/* $1 */");
                            b = Regex.Replace(b, @"(\r?\n)|(\t)", " ");
                            return b;
                        }
                    )

                );
            }
            return commands;
        }

        /// <summary>
        /// Execute commands from a file with SQL script against the context database
        /// </summary>
        /// <param name="fileProvider">File provider</param>
        /// <param name="filePath">Path to the file</param>
        protected void ExecuteSqlScriptFromFile(INopFileProvider fileProvider, string filePath)
        {
            filePath = fileProvider.MapPath(filePath);
            if (!fileProvider.FileExists(filePath))
                return;

            ExecuteSqlScript(fileProvider.ReadAllText(filePath, Encoding.Default));
        }

        #endregion

        #region Methods

        public virtual void ConfigureDataContext(IDataContext dataContext)
        {

            AdditionalSchema.SetDataType(
                typeof(Guid),
                new SqlDataType(DataType.NChar, typeof(Guid), 36));

            AdditionalSchema.SetConvertExpression<string, Guid>(strGuid => new Guid(strGuid));
        }

        public override DataConnection CreateDataContext()
        {
            var dataContext = CreateDataContext(LinqToDbDataProvider);

            ConfigureDataContext(dataContext);

            return dataContext;
        }

        public override IDbConnection CreateDbConnection(string connectionString = null)
        {
            return new MySqlConnection(!string.IsNullOrEmpty(connectionString) ? connectionString : CurrentConnectionString);
        }

        /// <summary>
        /// Creates the database by using the loaded connection string
        /// </summary>
        /// <param name="collation"></param>
        /// <param name="triesToConnect"></param>
        public void CreateDatabase(string collation, int triesToConnect = 10)
        {
            if (IsDatabaseExists())
                return;

            var builder = GetConnectionStringBuilder();

            //gets database name
            var databaseName = builder.Database;

            //now create connection string to 'master' database. It always exists.
            builder.Database = null;

            using (var connection = CreateDbConnection(builder.ConnectionString))
            {
                var query = $"CREATE DATABASE IF NOT EXISTS {databaseName};";
                if (!string.IsNullOrWhiteSpace(collation))
                    query = $"{query} COLLATE {collation}";

                var command = connection.CreateCommand(); //TODO
                command.CommandText = query;
                command.Connection.Open();

                command.ExecuteNonQuery();
            }

            //try connect
            if (triesToConnect <= 0)
                return;

            //sometimes on slow servers (hosting) there could be situations when database requires some time to be created.
            //but we have already started creation of tables and sample data.
            //as a result there is an exception thrown and the installation process cannot continue.
            //that's why we are in a cycle of "triesToConnect" times trying to connect to a database with a delay of one second.
            for (var i = 0; i <= triesToConnect; i++)
            {
                if (i == triesToConnect)
                    throw new Exception("Unable to connect to the new database. Please try one more time");

                if (!IsDatabaseExists())
                    Thread.Sleep(1000);
                else
                    break;
            }
        }

        /// <summary>
        /// Checks if the specified database exists, returns true if database exists
        /// </summary>
        /// <returns>Returns true if the database exists.</returns>
        public bool IsDatabaseExists()
        {
            try
            {
                using (var connection = CreateDbConnection())
                {
                    //just try to connect
                    connection.Open();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Execute commands from the SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        public void ExecuteSqlScript(string sql)
        {
            var sqlCommands = GetCommandsFromScript(sql);
            using (var currentConnection = CreateDataContext())
            {
                foreach (var command in sqlCommands)
                    currentConnection.Execute(command);
            }
        }

        /// <summary>
        /// Initialize database
        /// </summary>
        public void InitializeDatabase()
        {
            var migrationManager = EngineContext.Current.Resolve<IMigrationManager>();
            migrationManager.ApplyUpMigrations(null, NopMigrationTags.Schema);

            //create stored procedures 
            var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
            ExecuteSqlScriptFromFile(fileProvider, NopDataDefaults.MySQLStoredProceduresFilePath);
        }

        /// <summary>
        /// Get the current identity value
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <returns>Integer identity; null if cannot get the result</returns>
        public virtual int? GetTableIdent<T>() where T : BaseEntity
        {
            using (var currentConnection = CreateDataContext())
            {
                var tableName = currentConnection.GetTable<T>().TableName;
                var databaseName = currentConnection.Connection.Database;

                var result = currentConnection.Query<decimal?>($"SELECT AUTO_INCREMENT FROM information_schema.TABLES WHERE TABLE_SCHEMA = '{databaseName}' AND TABLE_NAME = '{tableName}'")
                    .FirstOrDefault();

                return result.HasValue ? Convert.ToInt32(result) : 1;
            }
        }

        /// <summary>
        /// Set table identity (is supported)
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="ident">Identity value</param>
        public virtual void SetTableIdent<T>(int ident) where T : BaseEntity
        {
            var currentIdent = GetTableIdent<T>();
            if (!currentIdent.HasValue || ident <= currentIdent.Value)
                return;

            using (var currentConnection = CreateDataContext())
            {
                var tableName = currentConnection.GetTable<T>().TableName;

                currentConnection.Execute($"ALTER TABLE '{tableName}' AUTO_INCREMENT = {ident}");
            }
        }

        /// <summary>
        /// Creates a backup of the database
        /// </summary>
        public virtual void BackupDatabase(string fileName)
        {
            //nothing
        }

        /// <summary>
        /// Restores the database from a backup
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        public virtual void RestoreDatabase(string backupFileName)
        {
            //nothing
        }

        /// <summary>
        /// Re-index database tables
        /// </summary>
        public virtual void ReIndexTables()
        {
            using (var currentConnection = CreateDataContext())
            {
                var tables = currentConnection.Query<string>($"SHOW TABLES FROM `{currentConnection.Connection.Database}`").ToList();

                if (tables?.Count > 0)
                {
                    currentConnection.Execute($"OPTIMIZE TABLE `{string.Join("`, `", tables)}`");
                }
            }
        }

        public virtual string BuildConnectionString(INopConnectionStringInfo nopConnectionString)
        {
            if (nopConnectionString is null)
                throw new ArgumentNullException(nameof(nopConnectionString));

            if (nopConnectionString.IntegratedSecurity)
                throw new NopException("Data provider supports connection only with login and password");

            var builder = new MySqlConnectionStringBuilder
            {
                Server = nopConnectionString.ServerName,
                Database = nopConnectionString.DatabaseName,
                AllowUserVariables = true,
                UserID = nopConnectionString.Username,
                Password = nopConnectionString.Password,
            };

            return builder.ConnectionString;
        }

        public virtual string GetForeignKeyName(string foreignTable, string foreignColumn, string primaryTable, string primaryColumn, bool isShort = true)
        {
            return $"FK_{Guid.NewGuid().ToString("D")}";
        }

        public virtual string GetIndexName(string targetTable, string targetColumn, bool isShort = true)
        {
            return $"IX_{Guid.NewGuid().ToString("D")}";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this data provider supports backup
        /// </summary>
        public bool BackupSupported { get; } = false;

        protected override IDataProvider LinqToDbDataProvider => new MySqlDataProvider();

        /// <summary>
        /// Gets allowed a limit input value of the data for hashing functions, returns 0 if not limited
        /// </summary>
        public int SupportedLengthOfBinaryHash { get; } = 0;

        #endregion
    }
}
