using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.SQLite;
using LinqToDB.Mapping;
using LinqToDB.Tools;
using Microsoft.Data.Sqlite;
using Nop.Core;
using Nop.Core.ComponentModel;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Mapping;
using Nop.Data.Migrations;

namespace Nop.Tests
{
    /// <summary>
    /// Represents the SQLite data provider
    /// </summary>
    public partial class SqLiteNopDataProvider : INopDataProvider
    {
        #region Consts

        //it's quite fast hash (to cheaply distinguish between objects)
        private const string HASH_ALGORITHM = "SHA1";
        private static IDbConnection _connection;
        private static DataConnection _dataContext;
        private static readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        #endregion

        #region Utils

        private void UpdateOutputParameters(DataConnection dataConnection, DataParameter[] dataParameters)
        {
            if (dataParameters is null || dataParameters.Length == 0)
                return;

            foreach (var dataParam in dataParameters.Where(p => p.Direction == ParameterDirection.Output))
                UpdateParameterValue(dataConnection, dataParam);
        }

        private void UpdateParameterValue(DataConnection dataConnection, DataParameter parameter)
        {
            if (dataConnection is null)
                throw new ArgumentNullException(nameof(dataConnection));

            if (parameter is null)
                throw new ArgumentNullException(nameof(parameter));

            if (dataConnection.Command is IDbCommand command &&
                command.Parameters.Count > 0 &&
                command.Parameters.Contains(parameter.Name) &&
                command.Parameters[parameter.Name] is IDbDataParameter param)
                parameter.Value = param.Value;
        }

        #endregion

        #region Methods

        public void CreateDatabase(string collation, int triesToConnect = 10)
        {
            Task.FromResult(ExecuteNonQueryAsync("PRAGMA journal_mode=WAL;"));
        }

        /// <summary>
        /// Creates a connection to a database
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Connection to a database</returns>
        public virtual IDbConnection CreateDbConnection(string connectionString = null)
        {
            _connection ??= new SqliteConnection(string.IsNullOrEmpty(connectionString)
                ? DataSettingsManager.LoadSettings().ConnectionString
                : connectionString);

            return _connection;
        }

        /// <summary>
        /// Initialize database
        /// </summary>
        public void InitializeDatabase()
        {
            var migrationManager = EngineContext.Current.Resolve<IMigrationManager>();
            migrationManager.ApplyUpMigrations(typeof(NopDbStartup).Assembly);
        }

        /// <summary>
        /// Inserts record into table. Returns inserted entity with identity
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns>Inserted entity</returns>
        public TEntity InsertEntity<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            using (new ReaderWriteLockDisposable(_locker))
            {
                entity.Id = DataContext.InsertWithInt32Identity(entity);
                return entity;
            }
        }

        /// <summary>
        /// Insert a new entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        public Task<TEntity> InsertEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            InsertEntity(entity);

            return Task.FromResult(entity);
        }

        /// <summary>
        /// Updates record in table, using values from entity parameter. 
        /// Record to update identified by match on primary key value from obj value.
        /// </summary>
        /// <param name="entity">Entity with data to update</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public Task UpdateEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            using (new ReaderWriteLockDisposable(_locker)) 
                DataContext.Update(entity);

            return Task.CompletedTask;
        }

        public async Task UpdateEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            foreach (var entity in entities) 
                await UpdateEntityAsync(entity);
        }

        /// <summary>
        /// Deletes record in table. Record to delete identified
        /// by match on primary key value from obj value.
        /// </summary>
        /// <param name="entity">Entity for delete operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public Task DeleteEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            using (new ReaderWriteLockDisposable(_locker))
                DataContext.Delete(entity);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Performs delete records in a table
        /// </summary>
        /// <param name="entities">Entities for delete operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public Task BulkDeleteEntitiesAsync<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity
        {
            using (new ReaderWriteLockDisposable(_locker))
                foreach (var entity in entities)
                    DataContext.Delete(entity);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Performs delete records in a table by a condition
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public Task<int> BulkDeleteEntitiesAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : BaseEntity
        {
            return Task.FromResult(DataContext.GetTable<TEntity>()
                .Where(predicate).Delete());
        }

        /// <summary>
        /// Performs bulk insert operation for entity collection.
        /// </summary>
        /// <param name="entities">Entities for insert operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public Task BulkInsertEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            using (new ReaderWriteLockDisposable(_locker))
                DataContext.BulkCopy(new BulkCopyOptions(), entities.RetrieveIdentity(DataContext));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the name of a foreign key
        /// </summary>
        /// <param name="foreignTable">Foreign key table</param>
        /// <param name="foreignColumn">Foreign key column name</param>
        /// <param name="primaryTable">Primary table</param>
        /// <param name="primaryColumn">Primary key column name</param>
        /// <returns>Name of a foreign key</returns>
        public string CreateForeignKeyName(string foreignTable, string foreignColumn, string primaryTable, string primaryColumn)
        {
            return "FK_" + HashHelper.CreateHash(Encoding.UTF8.GetBytes($"{foreignTable}_{foreignColumn}_{primaryTable}_{primaryColumn}"), HASH_ALGORITHM);
        }

        /// <summary>
        /// Gets the name of an index
        /// </summary>
        /// <param name="targetTable">Target table name</param>
        /// <param name="targetColumn">Target column name</param>
        /// <returns>Name of an index</returns>
        public string GetIndexName(string targetTable, string targetColumn)
        {
            return "IX_" + HashHelper.CreateHash(Encoding.UTF8.GetBytes($"{targetTable}_{targetColumn}"), HASH_ALGORITHM);
        }

        /// <summary>
        /// Returns queryable source for specified mapping class for current connection,
        /// mapped to database table or view.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Queryable source</returns>
        public ITable<TEntity> GetTable<TEntity>() where TEntity : BaseEntity
        {
            using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.Read))
                return DataContext.GetTable<TEntity>();
        }

        /// <summary>
        /// Get the current identity value
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <returns>Integer identity; null if cannot get the result</returns>
        public int? GetTableIdent<TEntity>() where TEntity : BaseEntity
        {
            using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.Read))
            {
                var tableName = DataContext.GetTable<TEntity>().TableName;

                var result = DataContext.Query<int?>($"select seq from sqlite_sequence where name = \"{tableName}\"")
                    .FirstOrDefault();

                return result ?? 1;
            }
        }

        /// <summary>
        /// Checks if the specified database exists, returns true if database exists
        /// </summary>
        /// <returns>Returns true if the database exists.</returns>
        public bool DatabaseExists()
        {
            return true;
        }

        /// <summary>
        /// Creates a backup of the database
        /// </summary>
        public virtual Task BackupDatabaseAsync(string fileName)
        {
            throw new DataException("This database provider does not support backup");
        }

        /// <summary>
        /// Restores the database from a backup
        /// </summary>
        /// <param name="backupFileName">The name of the backup file</param>
        public virtual Task RestoreDatabaseAsync(string backupFileName)
        {
            throw new DataException("This database provider does not support backup");
        }

        /// <summary>
        /// Re-index database tables
        /// </summary>
        public Task ReIndexTablesAsync()
        {
            using (new ReaderWriteLockDisposable(_locker))
                DataContext.Execute("VACUUM;");

            return Task.CompletedTask;
        }

        public string BuildConnectionString(INopConnectionStringInfo nopConnectionString)
        {
            if (nopConnectionString is null)
                throw new ArgumentNullException(nameof(nopConnectionString));

            if (nopConnectionString.IntegratedSecurity)
                throw new NopException("Data provider supports connection only with password");

            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = CommonHelper.DefaultFileProvider.MapPath($"~/App_Data/{nopConnectionString.DatabaseName}.sqlite"),
                Password = nopConnectionString.Password,
                Mode = SqliteOpenMode.ReadWrite,
                Cache = SqliteCacheMode.Shared
            };

            return builder.ConnectionString;
        }

        /// <summary>
        /// Set table identity (is supported)
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="ident">Identity value</param>
        public Task SetTableIdentAsync<TEntity>(int ident) where TEntity : BaseEntity
        {
            using (new ReaderWriteLockDisposable(_locker))
            {
                var tableName = DataContext.GetTable<TEntity>().TableName;

                DataContext.Execute($"update sqlite_sequence set seq = {ident} where name = \"{tableName}\"");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns mapped entity descriptor.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Mapping descriptor</returns>
        public EntityDescriptor GetEntityDescriptor<TEntity>() where TEntity : BaseEntity
        {
            return AdditionalSchema?.GetEntityDescriptor(typeof(TEntity));
        }

        /// <summary>
        /// Executes command using System.Data.CommandType.StoredProcedure command type and
        /// returns results as collection of values of specified type
        /// </summary>
        /// <typeparam name="T">Result record type</typeparam>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Returns collection of query result records</returns>
        public Task<IList<T>> QueryProcAsync<T>(string procedureName, params DataParameter[] parameters)
        {
            //stored procedure is not support by SqLite
            return Task.FromResult<IList<T>>(new List<T>());
        }

        /// <summary>
        /// Executes SQL command and returns results as collection of values of specified type
        /// </summary>
        /// <typeparam name="T">Type of result items</typeparam>
        /// <param name="sql">SQL command text</param>
        /// <param name="parameters">Parameters to execute the SQL command</param>
        /// <returns>Collection of values of specified type</returns>
        public Task<IList<T>> QueryAsync<T>(string sql, params DataParameter[] parameters)
        {
            using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.Read))
                return Task.FromResult<IList<T>>(DataContext.Query<T>(sql, parameters).ToList());
        }

        /// <summary>
        /// Executes command asynchronously and returns number of affected records
        /// </summary>
        /// <param name="sql">Command text</param>
        /// <param name="dataParameters">Command parameters</param>
        /// <returns>Number of records, affected by command execution.</returns>
        public Task<int> ExecuteNonQueryAsync(string sql, params DataParameter[] dataParameters)
        {
            using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.Read))
            {
                var command = new CommandInfo(DataContext, sql, dataParameters);
                var affectedRecords = command.Execute();

                UpdateOutputParameters(DataContext, dataParameters);

                return  Task.FromResult<int>(affectedRecords);
            }
        }

        public Task<ITempDataStorage<TItem>> CreateTempDataStorageAsync<TItem>(string storageKey, IQueryable<TItem> query) where TItem : class
        {
            return Task.FromResult<ITempDataStorage<TItem>>(new TempSqlDataStorage<TItem>(storageKey, query, DataContext));
        }

        public Task<ITable<TEntity>> GetTableAsync<TEntity>() where TEntity : BaseEntity
        {
            return Task.FromResult(GetTable<TEntity>());
        }

        public Task<int?> GetTableIdentAsync<TEntity>() where TEntity : BaseEntity
        {
            return Task.FromResult(GetTableIdent<TEntity>());
        }

        public Task<bool> DatabaseExistsAsync()
        {
            return Task.FromResult(DatabaseExists());
        }

        #endregion

        #region Properties

        protected DataConnection DataContext =>
            _dataContext ??= new DataConnection(LinqToDbDataProvider, CreateDbConnection(), AdditionalSchema)
            {
                CommandTimeout = DataSettingsManager.GetSqlCommandTimeout()
            };

        /// <summary>
        /// Name of database provider
        /// </summary>
        public string ConfigurationName => LinqToDbDataProvider.Name;

        /// <summary>
        /// Additional mapping schema
        /// </summary>
        protected MappingSchema AdditionalSchema
        {
            get
            {
                if (!(Singleton<MappingSchema>.Instance is null))
                    return Singleton<MappingSchema>.Instance;

                Singleton<MappingSchema>.Instance =
                    new MappingSchema(ConfigurationName) { MetadataReader = new FluentMigratorMetadataReader() };

                return Singleton<MappingSchema>.Instance;
            }
        }

        /// <summary>
        /// Linq2Db data provider
        /// </summary>
        protected IDataProvider LinqToDbDataProvider { get; } = SQLiteTools.GetDataProvider();

        /// <summary>
        /// Gets allowed a limit input value of the data for hashing functions, returns 0 if not limited
        /// </summary>
        public int SupportedLengthOfBinaryHash { get; } = 0;

        /// <summary>
        /// Gets a value indicating whether this data provider supports backup
        /// </summary>
        public bool BackupSupported { get; } = false;

        #endregion
    }
}
