using System;
using System.Data;
using System.Linq;
using LinqToDB.Data;
using Nop.Core;

namespace Nop.Data
{
    public partial class MsSqlDataProvider: IDataProvider
    {
        private readonly DataConnection _dataConnection;

        public MsSqlDataProvider(DataConnection dataConnection)
        {
            _dataConnection = dataConnection;
        }

        #region Methods

        /// <summary>
        /// Check whether backups are supported
        /// </summary>
        protected void CheckBackupSupported()
        {
            if (!BackupSupported)
                throw new DataException("This database does not support backup");
        }

        /// <summary>
        /// Initialize database
        /// </summary>
        public void InitializeDatabase()
        {
        }

        /// <summary>
        /// Get a support database parameter object (used by stored procedures)
        /// </summary>
        /// <returns>Parameter</returns>
        public DataParameter GetParameter()
        {
            return new DataParameter();
        }

        /// <summary>
        /// Get the current identity value
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <returns>Integer identity; null if cannot get the result</returns>
        public virtual int? GetTableIdent<T>() where T : BaseEntity
        {
            var tableName = _dataConnection.GetTable<T>().TableName;

            var result = _dataConnection.Query<decimal?>($"SELECT IDENT_CURRENT('[{tableName}]') as Value")
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

            _dataConnection.Execute($"DBCC CHECKIDENT([{tableName}], RESEED, {ident})");
        }

        /// <summary>
        /// Creates a backup of the database
        /// </summary>
        public virtual void BackupDatabase(string fileName)
        {
            CheckBackupSupported();
            //var fileName = _fileProvider.Combine(GetBackupDirectoryPath(), $"database_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(10)}.{NopCommonDefaults.DbBackupFileExtension}");

            var commandText = $"BACKUP DATABASE [{_dataConnection.Connection.Database}] TO DISK = '{fileName}' WITH FORMAT";
            _dataConnection.Execute(commandText);
        }

        /// <summary>
        /// Restores the database from a backup
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        public virtual void RestoreDatabase(string backupFileName)
        {
            CheckBackupSupported();

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
                _dataConnection.Connection.Database,
                backupFileName);

            _dataConnection.Execute(commandText);
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

        /// <summary>
        /// Loads the original copy of the entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Copy of the passed entity</returns>
        public TEntity LoadOriginalCopy<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            var entities = _dataConnection.GetTable<TEntity>();
            return entities.FirstOrDefault(e => e.Id == Convert.ToInt32(entity.Id));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this data provider supports backup
        /// </summary>
        public bool BackupSupported { get; } = true;

        /// <summary>
        /// Gets a maximum length of the data for HASHBYTES functions, returns 0 if HASHBYTES function is not supported
        /// </summary>
        public int SupportedLengthOfBinaryHash { get; } = 8000; //for SQL Server 2008 and above HASHBYTES function has a limit of 8000 characters.

        #endregion
    }
}
