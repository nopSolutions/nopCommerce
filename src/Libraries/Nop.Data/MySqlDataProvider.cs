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
        #region Fields

        //it's quite fast hash (to cheaply distinguish between objects)
        private const string HASH_ALGORITHM = "SHA1";

        #endregion

        #region Utils

        /// <summary>
        /// Configures the data context
        /// </summary>
        /// <param name="dataContext">Data context to configure</param>
        private void ConfigureDataContext(IDataContext dataContext)
        {
            AdditionalSchema.SetDataType(
                typeof(Guid),
                new SqlDataType(DataType.NChar, typeof(Guid), 36));

            AdditionalSchema.SetConvertExpression<string, Guid>(strGuid => new Guid(strGuid));
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

            if (batches.Length > 0)
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
                        }));
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

        /// <summary>
        /// Creates the database connection
        /// </summary>
        protected override DataConnection CreateDataConnection()
        {
            var dataContext = CreateDataConnection(LinqToDbDataProvider);

            ConfigureDataContext(dataContext);

            return dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a connection to the database for a current data provider
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Connection to a database</returns>
        protected override IDbConnection GetInternalDbConnection(string connectionString)
        {
            if(string.IsNullOrEmpty(connectionString))
                throw new ArgumentException(nameof(connectionString));

            return new MySqlConnection(connectionString);
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

                var command = connection.CreateCommand();
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
            using (var currentConnection = CreateDataConnection())
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
            migrationManager.ApplyUpMigrations();

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
            using (var currentConnection = CreateDataConnection())
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

            using (var currentConnection = CreateDataConnection())
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
            throw new DataException("This database provider does not support backup");
        }

        /// <summary>
        /// Restores the database from a backup
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        public virtual void RestoreDatabase(string backupFileName)
        {
            throw new DataException("This database provider does not support backup");
        }

        /// <summary>
        /// Re-index database tables
        /// </summary>
        public virtual void ReIndexTables()
        {
            using (var currentConnection = CreateDataConnection())
            {
                var tables = currentConnection.Query<string>($"SHOW TABLES FROM `{currentConnection.Connection.Database}`").ToList();

                if (tables.Count > 0)
                {
                    currentConnection.Execute($"OPTIMIZE TABLE `{string.Join("`, `", tables)}`");
                }
            }
        }

        /// <summary>
        /// Build the connection string
        /// </summary>
        /// <param name="nopConnectionString">Connection string info</param>
        /// <returns>Connection string</returns>
        public virtual string BuildConnectionString(INopConnectionStringInfo nopConnectionString)
        {
            if (nopConnectionString is null)
                throw new ArgumentNullException(nameof(nopConnectionString));

            if (nopConnectionString.IntegratedSecurity)
                throw new NopException("Data provider supports connection only with login and password");

            var builder = new MySqlConnectionStringBuilder
            {
                Server = nopConnectionString.ServerName,
                //Cast DatabaseName to lowercase to avoid case-sensitivity problems
                Database = nopConnectionString.DatabaseName.ToLower(),
                AllowUserVariables = true,
                UserID = nopConnectionString.Username,
                Password = nopConnectionString.Password,
            };

            return builder.ConnectionString;
        }

        /// <summary>
        /// Gets the name of a foreign key
        /// </summary>
        /// <param name="foreignTable">Foreign key table</param>
        /// <param name="foreignColumn">Foreign key column name</param>
        /// <param name="primaryTable">Primary table</param>
        /// <param name="primaryColumn">Primary key column name</param>
        /// <returns>Name of a foreign key</returns>
        public virtual string CreateForeignKeyName(string foreignTable, string foreignColumn, string primaryTable, string primaryColumn)
        {
            //mySql support only 64 chars for constraint name
            //that is why we use hash function for create unique name
            //see details on this topic: https://dev.mysql.com/doc/refman/8.0/en/identifier-length.html
            return "FK_" + HashHelper.CreateHash(Encoding.UTF8.GetBytes($"{foreignTable}_{foreignColumn}_{primaryTable}_{primaryColumn}"), HASH_ALGORITHM);
        }

        /// <summary>
        /// Gets the name of an index
        /// </summary>
        /// <param name="targetTable">Target table name</param>
        /// <param name="targetColumn">Target column name</param>
        /// <returns>Name of an index</returns>
        public virtual string GetIndexName(string targetTable, string targetColumn)
        {
            return "IX_" + HashHelper.CreateHash(Encoding.UTF8.GetBytes($"{targetTable}_{targetColumn}"), HASH_ALGORITHM);
        }

        #endregion

        #region Properties

        /// <summary>
        /// MySql data provider
        /// </summary>
        protected override IDataProvider LinqToDbDataProvider => new MySqlDataProvider();

        /// <summary>
        /// Gets allowed a limit input value of the data for hashing functions, returns 0 if not limited
        /// </summary>
        public int SupportedLengthOfBinaryHash { get; } = 0;

        /// <summary>
        /// Gets a value indicating whether this data provider supports backup
        /// </summary>
        public virtual bool BackupSupported => false;

        #endregion
    }
}
