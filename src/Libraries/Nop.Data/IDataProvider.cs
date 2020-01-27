using System.Collections.Generic;
using System.Reflection;
using LinqToDB.Data;
using LinqToDB.Mapping;
using Nop.Core;

namespace Nop.Data
{
    /// <summary>
    /// Represents a data provider
    /// </summary>
    public partial interface IDataProvider
    {
        #region Methods

        /// <summary>
        /// Create the database
        /// </summary>
        /// <param name="collation">Collation</param>
        /// <param name="triesToConnect">Count of tries to connect to the database after creating; set 0 if no need to connect after creating</param>
        void CreateDatabase(string collation, int triesToConnect = 10);

        /// <summary>
        /// Initialize database
        /// </summary>
        void InitializeDatabase();

        /// <summary>
        /// Executes all found (and unapplied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration;
        /// leave null to search migration on the whole application pull</param>
        void ApplyUpMigrations(Assembly assembly = null);

        /// <summary>
        /// Executes an Down migration
        /// </summary>
        /// <param name="assembly">Assembly to find the migration;
        /// leave null to search migration on the whole application pull</param>
        void ApplyDownMigrations(Assembly assembly);

        /// <summary>
        /// Create database schema if it not exists
        /// </summary>
        /// <param name="assembly">Assembly to find the mapping configurations classes;
        /// leave null to search mapping configurations classes on the whole application pull</param>
        void CreateDatabaseSchemaIfNotExists(Assembly assembly);

        /// <summary>
        /// Delete database schema if it exists
        /// </summary>
        /// <param name="assembly">Assembly to find the mapping configurations classes;
        /// leave null to search mapping configurations classes on the whole application pull</param>
        void DeleteDatabaseSchemaIfExists(Assembly assembly);

        /// <summary>
        /// Checks if the specified database exists, returns true if database exists
        /// </summary>
        /// <returns>Returns true if the database exists.</returns>
        bool IsDatabaseExists();
        
        /// <summary>
        /// Get the current identity value
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <returns>Integer identity; null if cannot get the result</returns>
        int? GetTableIdent<T>() where T : BaseEntity;

        /// <summary>
        /// Set table identity (is supported)
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="ident">Identity value</param>
        void SetTableIdent<T>(int ident) where T : BaseEntity;

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
        /// Loads the original copy of the entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Copy of the passed entity</returns>
        TEntity LoadOriginalCopy<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Insert a new entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        TEntity InsertEntity<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Returns mapped entity descriptor
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <returns>Mapped entity descriptor</returns>
        EntityDescriptor GetEntityDescriptor<TEntity>() where TEntity : BaseEntity;

        /// <summary>
        /// Build the connection string
        /// </summary>
        /// <param name="nopConnectionString">Connection string info</param>
        /// <returns>Connection string</returns>
        string BuildConnectionString(INopConnectionStringInfo nopConnectionString);

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

        /// <summary>
        /// Executes command using System.Data.CommandType.StoredProcedure command type and
        /// returns results as collection of values of specified type
        /// </summary>
        /// <typeparam name="T">Result record type</typeparam>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Returns collection of query result records</returns>
        IList<T> QueryProc<T>(string procedureName, params DataParameter[] parameters);

        /// <summary>
        /// Executes command and returns results as collection of values of specified type
        /// </summary>
        /// <typeparam name="T">Result record type</typeparam>
        /// <param name="sql">Command text</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Returns collection of query result records</returns>
        IList<T> Query<T>(string sql, params DataParameter[] parameters);

        #endregion
    }
}