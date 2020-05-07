using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using LinqToDB;
using LinqToDB.Data;
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

        /// <summary>
        /// Creates a connection to a database
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Connection to a database</returns>
        IDbConnection CreateDbConnection(string connectionString);

        /// <summary>
        /// Initialize database
        /// </summary>
        void InitializeDatabase();

        /// <summary>
        /// Insert a new entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        TEntity InsertEntity<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Updates record in table, using values from entity parameter. 
        /// Record to update identified by match on primary key value from obj value.
        /// </summary>
        /// <param name="entity">Entity with data to update</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        void UpdateEntity<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Deletes record in table. Record to delete identified
        /// by match on primary key value from obj value.
        /// </summary>
        /// <param name="entity">Entity for delete operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        void DeleteEntity<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Performs delete records in a table
        /// </summary>
        /// <param name="entities">Entities for delete operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        void BulkDeleteEntities<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity;

        /// <summary>
        /// Performs delete records in a table by a condition
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        void BulkDeleteEntities<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : BaseEntity;

        /// <summary>
        /// Performs bulk insert entities operation
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entities">Collection of Entities</param>
        void BulkInsertEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity;

        /// <summary>
        /// Gets the name of a foreign key
        /// </summary>
        /// <param name="foreignTable">Foreign key table</param>
        /// <param name="foreignColumn">Foreign key column name</param>
        /// <param name="primaryTable">Primary table</param>
        /// <param name="primaryColumn">Primary key column name</param>
        /// <returns>Name of a foreign key</returns>
        string CreateForeignKeyName(string foreignTable, string foreignColumn, string primaryTable, string primaryColumn);

        /// <summary>
        /// Gets the name of an index
        /// </summary>
        /// <param name="targetTable">Target table name</param>
        /// <param name="targetColumn">Target column name</param>
        /// <returns>Name of an index</returns>
        string GetIndexName(string targetTable, string targetColumn);

        /// <summary>
        /// Returns queryable source for specified mapping class for current connection,
        /// mapped to database table or view.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Queryable source</returns>
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

        /// <summary>
        /// Executes command and returns number of affected records.
        /// </summary>
        /// <param name="sqlStatement">Command text</param>
        /// <param name="dataParameters">Command parameters</param>
        /// <returns>Number of records, affected by command execution.</returns>
        int ExecuteNonQuery(string sqlStatement, params DataParameter[] dataParameters);

        /// <summary>
        /// Executes command using LinqToDB.Mapping.StoredProcedure command type and returns
        /// single value
        /// </summary>
        /// <typeparam name="T">Result record type</typeparam>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Resulting value</returns>
        T ExecuteStoredProcedure<T>(string procedureName, params DataParameter[] parameters);

        /// <summary>
        /// Executes command using LinqToDB.Mapping.StoredProcedure command type and returns
        /// number of affected records.
        /// </summary>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Returns collection of query result records</returns>
        int ExecuteStoredProcedure(string procedureName, params DataParameter[] parameters);

        #endregion

        #region Properties

        /// <summary>
        /// Name of database provider
        /// </summary>
        string ConfigurationName { get; }

        /// <summary>
        /// Gets allowed a limit input value of the data for hashing functions, returns 0 if not limited
        /// </summary>
        int SupportedLengthOfBinaryHash { get; }

        /// <summary>
        /// Gets a value indicating whether this data provider supports backup
        /// </summary>
        bool BackupSupported { get; }

        #endregion
    }
}