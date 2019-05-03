using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Extensions;

namespace Nop.Services.Common
{
    /// <summary>
    ///  Maintenance service
    /// </summary>
    public partial class MaintenanceService : IMaintenanceService
    {
        #region Fields

        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly INopFileProvider _fileProvider;

        #endregion

        #region Ctor

        public MaintenanceService(IDataProvider dataProvider,
            IDbContext dbContext,
            INopFileProvider fileProvider)
        {
            _dataProvider = dataProvider;
            _dbContext = dbContext;
            _fileProvider = fileProvider;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get directory path for backs
        /// </summary>
        /// <param name="ensureFolderCreated">A value indicating whether a directory should be created if it doesn't exist</param>
        /// <returns></returns>
        protected virtual string GetBackupDirectoryPath(bool ensureFolderCreated = true)
        {
            var path = _fileProvider.GetAbsolutePath(NopCommonDefaults.DbBackupsPath);
            if (ensureFolderCreated)
                _fileProvider.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Check whether backups are supported
        /// </summary>
        protected virtual void CheckBackupSupported()
        {
            if (!_dataProvider.BackupSupported)
                throw new DataException("This database does not support backup");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the current identity value
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <returns>Integer identity; null if cannot get the result</returns>
        public virtual int? GetTableIdent<T>() where T : BaseEntity
        {
            var tableName = _dbContext.GetTableName<T>();
            var result = _dbContext
                .QueryFromSql<DecimalQueryType>($"SELECT IDENT_CURRENT('[{tableName}]') as Value")
                .Select(decimalValue => decimalValue.Value).FirstOrDefault();
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

            var tableName = _dbContext.GetTableName<T>();
            _dbContext.ExecuteSqlCommand($"DBCC CHECKIDENT([{tableName}], RESEED, {ident})");
        }

        /// <summary>
        /// Gets all backup files
        /// </summary>
        /// <returns>Backup file collection</returns>
        public virtual IList<string> GetAllBackupFiles()
        {
            var path = GetBackupDirectoryPath();

            if (!_fileProvider.DirectoryExists(path))
            {
                throw new NopException("Backup directory not exists");
            }

            return _fileProvider.GetFiles(path, $"*.{NopCommonDefaults.DbBackupFileExtension}")
                .OrderByDescending(p => _fileProvider.GetLastWriteTime(p)).ToList();
        }

        /// <summary>
        /// Creates a backup of the database
        /// </summary>
        public virtual void BackupDatabase()
        {
            CheckBackupSupported();
            var fileName = _fileProvider.Combine(GetBackupDirectoryPath(), $"database_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(10)}.{NopCommonDefaults.DbBackupFileExtension}");

            var commandText = $"BACKUP DATABASE [{_dbContext.DbName()}] TO DISK = '{fileName}' WITH FORMAT";

            _dbContext.ExecuteSqlCommand(commandText, true);
        }

        /// <summary>
        /// Restores the database from a backup
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        public virtual void RestoreDatabase(string backupFileName)
        {
            CheckBackupSupported();

            var conn = new SqlConnectionStringBuilder(DataSettingsManager.LoadSettings(fileProvider: _fileProvider).DataConnectionString)
            {
                InitialCatalog = "master"
            };

            //this method (backups) works only with SQL Server database
            using (var sqlConnectiononn = new SqlConnection(conn.ToString()))
            {
                var commandText = string.Format(
                    "DECLARE @ErrorMessage NVARCHAR(4000)\n" +
                    "ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE\n" +
                    "BEGIN TRY\n" +
                        "RESTORE DATABASE [{0}] FROM DISK = '{1}' WITH REPLACE\n" +
                    "END TRY\n" +
                    "BEGIN CATCH\n" +
                        "SET @ErrorMessage = ERROR_MESSAGE()\n" +
                    "END CATCH\n" +
                    "ALTER DATABASE [{0}] SET MULTI_USER WITH ROLLBACK IMMEDIATE\n" +
                    "IF (@ErrorMessage is not NULL)\n" +
                    "BEGIN\n" +
                        "RAISERROR (@ErrorMessage, 16, 1)\n" +
                    "END",
                    _dbContext.DbName(),
                    backupFileName);

                DbCommand dbCommand = new SqlCommand(commandText, sqlConnectiononn);
                if (sqlConnectiononn.State != ConnectionState.Open)
                    sqlConnectiononn.Open();
                dbCommand.ExecuteNonQuery();
            }

            //clear all pools
            SqlConnection.ClearAllPools();
        }

        /// <summary>
        /// Returns the path to the backup file
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        /// <returns>The path to the backup file</returns>
        public virtual string GetBackupPath(string backupFileName)
        {
            return _fileProvider.Combine(GetBackupDirectoryPath(), backupFileName);
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
                FROM [{_dbContext.DbName()}].information_schema.tables
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

            _dbContext.ExecuteSqlCommand(commandText, true);
        }

        #endregion
    }
}