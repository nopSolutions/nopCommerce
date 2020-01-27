using System.Collections.Generic;
using System.Data;  
using System.Reflection;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.Mapping;
using Nop.Core;

namespace Nop.Data
{
    /// <summary>
    /// Represents a data provider
    /// </summary>
    public partial interface INopDataProvider
    {
        #region Methods

        /// <summary>
        /// Create the database
        /// </summary>
        /// <param name="collation">Collation</param>
        /// <param name="triesToConnect">Count of tries to connect to the database after creating; set 0 if no need to connect after creating</param>
        void CreateDatabase(string collation, int triesToConnect = 10);
        
        NopDataConnection CreateDataContext();

        IDbConnection CreateDbConnection();
        
        /// <summary>
        /// Initialize database
        /// </summary>
        void InitializeDatabase();

        string GetForeignKeyName(string foreignTable, string foreignColumn, string primaryTable, string primaryColumn, bool isShort = true);

        string GetIndexName(string targetTable, string targetColumn, bool isShort = true);   

        ITable<TEntity> GetTable<TEntity>() where TEntity : BaseEntity;
        
        /// <summary>
        /// Get the current identity value
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <returns>Integer identity; null if cannot get the result</returns>
        int? GetTableIdent<TEntity>() where TEntity : BaseEntity;

        /// <summary>
        /// Checks if the specified database exists, returns true if database exists
        /// </summary>
        /// <returns>Returns true if the database exists.</returns>
        bool IsDatabaseExists();
        
        /// <summary>
        /// Creates a backup of the database
        /// </summary>
        void BackupDatabase(string fileName);

        /// <summary>
        /// Restores the database from a backup
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        void RestoreDatabase(string backupFileName);

        /// <summary>
        /// Re-index database tables
        /// </summary>
        void ReIndexTables();

        /// <summary>
        /// Build the connection string
        /// </summary>
        /// <param name="nopConnectionString">Connection string info</param>
        /// <returns>Connection string</returns>
        string BuildConnectionString(INopConnectionStringInfo nopConnectionString);

        /// <summary>
        /// Set table identity (is supported)
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="ident">Identity value</param>
        void SetTableIdent<T>(int ident) where T : BaseEntity;     

        /// <summary>
        /// Returns mapped entity descriptor
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <returns>Mapped entity descriptor</returns>
        EntityDescriptor GetEntityDescriptor<TEntity>() where TEntity : BaseEntity;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this data provider supports backup
        /// </summary>
        bool BackupSupported { get; }

        /// <summary>
        /// Gets a maximum length of the data for HASHBYTES functions, returns 0 if HASHBYTES function is not supported
        /// </summary>
        int SupportedLengthOfBinaryHash { get; }

        #endregion
    }
}