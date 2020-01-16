using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;
using LinqToDB.SqlQuery;
using MySql.Data.MySqlClient;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Data
{
    public class MySqlDataProvider : BaseDataProvider, IDataProvider
    {
        #region Utils

        protected MySqlConnectionStringBuilder GetConnectionStringBuilder()
        {
            var connectionString = DataSettingsManager.LoadSettings().ConnectionString;

            return new MySqlConnectionStringBuilder(connectionString);
        }

        protected override void ConfigureDataContext(IDataContext dataContext)
        {
            dataContext.MappingSchema.SetDataType(
                typeof(string),
                new SqlDataType(DataType.Text, typeof(string)));

            dataContext.MappingSchema.SetDataType(
                typeof(Guid),
                new SqlDataType(DataType.NChar, typeof(Guid), 36));

            dataContext.MappingSchema.SetDataType(
                typeof(byte[]),
                new SqlDataType(new DbDataType(typeof(byte[]), DataType.Blob)));
        }

        protected virtual void EnsureDatabaseAfterInit()
        {
            var builder = GetConnectionStringBuilder();
            using (var connection = new MySqlConnection(builder.ConnectionString))
            {
                var query = @"ALTER TABLE `picturebinary` 
                            CHANGE COLUMN `BinaryData` `BinaryData` LONGBLOB NULL DEFAULT NULL ;";
                var command = new MySqlCommand(query, connection);
                command.Connection.Open();

                command.ExecuteNonQuery();

            }
        }

        #endregion

        #region Methods

        public void CreateDatabase(string collation, int triesToConnect = 10)
        {
            if (IsDatabaseExists())
                return;

            var builder = GetConnectionStringBuilder();

            //gets database name
            var databaseName = builder.Database;

            //now create connection string to 'master' dabatase. It always exists.
            builder.Database = null;

            using (var connection = new MySqlConnection(builder.ConnectionString))
            {
                var query = $"CREATE DATABASE IF NOT EXISTS {databaseName};";
                if (!string.IsNullOrWhiteSpace(collation))
                    query = $"{query} COLLATE {collation}";

                var command = new MySqlCommand(query, connection);
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
                using (var connection = new MySqlConnection(GetConnectionStringBuilder().ConnectionString))
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
        /// Check whether backups are supported
        /// </summary>
        protected void CheckBackupSupported()
        {
            if (!BackupSupported)
                throw new DataException("This database does not support backup");
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

        /// <summary>
        /// Execute commands from the SQL script
        /// </summary>
        /// <param name="sql">SQL script</param>
        public void ExecuteSqlScript(string sql)
        {
            var sqlCommands = GetCommandsFromScript(sql);
            foreach (var command in sqlCommands)
                _dataConnection.Execute(command);
        }

        /// <summary>
        /// Initialize database
        /// </summary>
        public void InitializeDatabase()
        {
            CreateDatabaseSchemaIfNotExists();
            EnsureDatabaseAfterInit();
            var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();

            //create stored procedures 
            ExecuteSqlScriptFromFile(fileProvider, NopDataDefaults.MySQLStoredProceduresFilePath);
        }

        /// <summary>
        /// Get the current identity value
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <returns>Integer identity; null if cannot get the result</returns>
        public virtual int? GetTableIdent<T>() where T : BaseEntity
        {
            var tableName = _dataConnection.GetTable<T>().TableName;
            var databaseName = _dataConnection.Connection.Database;

            var result = _dataConnection.Query<decimal?>($"SELECT AUTO_INCREMENT FROM information_schema.TABLES WHERE TABLE_SCHEMA = '{databaseName}' AND TABLE_NAME = '{tableName}'")
                .FirstOrDefault();

            return result.HasValue ? Convert.ToInt32(result) : 1;
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

            var tableName = _dataConnection.GetTable<T>().TableName;

            _dataConnection.Execute($"ALTER TABLE '{tableName}' AUTO_INCREMENT = {ident}");
        }

        /// <summary>
        /// Creates a backup of the database
        /// </summary>
        public virtual void BackupDatabase(string fileName)
        {
            CheckBackupSupported();
            //var fileName = _fileProvider.Combine(GetBackupDirectoryPath(), $"database_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(10)}.{NopCommonDefaults.DbBackupFileExtension}");

            //var commandText = $"BACKUP DATABASE [{_dataConnection.Connection.Database}] TO DISK = '{fileName}' WITH FORMAT";
            //_dataConnection.Execute(commandText);
        }

        /// <summary>
        /// Restores the database from a backup
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        public virtual void RestoreDatabase(string backupFileName)
        {
            CheckBackupSupported();

            //var commandText = string.Format(
            //    "DECLARE @ErrorMessage NVARCHAR(4000)\n" +
            //    "ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE\n" +
            //    "BEGIN TRY\n" +
            //    "RESTORE DATABASE [{0}] FROM DISK = '{1}' WITH REPLACE\n" +
            //    "END TRY\n" +
            //    "BEGIN CATCH\n" +
            //    "SET @ErrorMessage = ERROR_MESSAGE()\n" +
            //    "END CATCH\n" +
            //    "ALTER DATABASE [{0}] SET MULTI_USER WITH ROLLBACK IMMEDIATE\n" +
            //    "IF (@ErrorMessage is not NULL)\n" +
            //    "BEGIN\n" +
            //    "RAISERROR (@ErrorMessage, 16, 1)\n" +
            //    "END",
            //    _dataConnection.Connection.Database,
            //    backupFileName);

            //_dataConnection.Execute(commandText);
        }

        /// <summary>
        /// Re-index database tables
        /// </summary>
        public virtual void ReIndexTables()
        {
            var commandText = $@"
                        DECLARE @TableName sysname 
                        DECLARE cur_reindex CURSOR FOR
                        SELECT table_name
                        FROM [{_dataConnection.Connection.Database}].information_schema.tables
                        WHERE table_type = 'base table'
                        OPEN cur_reindex
                        FETCH NEXT FROM cur_reindex INTO @TableName
                        WHILE @@FETCH_STATUS = 0
                            BEGIN
                                exec('ALTER INDEX ALL ON [' + @TableName + '] REBUILD')
                                        FETCH NEXT FROM cur_reindex INTO @TableName
                            END
                        CLOSE cur_reindex
                        DEALLOCATE cur_reindex";

            _dataConnection.Execute(commandText);
        }

        public virtual string BuildConnectionString(INopConnectionStringInfo nopConnectionString)
        {
            if (nopConnectionString is null)
                throw new ArgumentNullException(nameof(nopConnectionString));

            var builder = new MySqlConnectionStringBuilder
            {
                Server = nopConnectionString.ServerName,
                Database = nopConnectionString.DatabaseName,
                PersistSecurityInfo = false,
                AllowUserVariables = true,
                IntegratedSecurity = nopConnectionString.IntegratedSecurity
            };

            if (!nopConnectionString.IntegratedSecurity)
            {
                builder.UserID = nopConnectionString.Username;
                builder.Password = nopConnectionString.Password;
            }

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

        /// <summary>
        /// Gets a maximum length of the data for HASHBYTES functions, returns 0 if HASHBYTES function is not supported
        /// </summary>
        public int SupportedLengthOfBinaryHash { get; } = 8000; //for SQL Server 2008 and above HASHBYTES function has a limit of 8000 characters.

        #endregion
    }
}
