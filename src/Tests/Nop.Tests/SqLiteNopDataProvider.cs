using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.SQLite;
using LinqToDB.Tools;
using Microsoft.Data.Sqlite;
using Nop.Core;
using Nop.Core.ComponentModel;
using Nop.Data;
using Nop.Data.DataProviders;
namespace Nop.Tests
{
    /// <summary>
    /// Represents the SQLite data provider
    /// </summary>
    public partial class SqLiteNopDataProvider : BaseDataProvider, INopDataProvider
    {
        #region Consts

        //it's quite fast hash (to cheaply distinguish between objects)
        private const string HASH_ALGORITHM = "SHA1";
        private static DataConnection _dataContext;

        private static readonly ReaderWriterLockSlim _locker = new();

        #endregion

        #region Methods

        public void CreateDatabase(string collation, int triesToConnect = 10)
        {
            ExecuteNonQueryAsync("PRAGMA journal_mode=WAL;").Wait();
        }

        /// <summary>
        /// Gets a connection to the database for a current data provider
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Connection to a database</returns>
        protected override DbConnection GetInternalDbConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            return new SqliteConnection(string.IsNullOrEmpty(connectionString)
                ? DataSettingsManager.LoadSettings().ConnectionString
                : connectionString);
        }

        /// <summary>
        /// Inserts record into table. Returns inserted entity with identity
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns>Inserted entity</returns>
        public override TEntity InsertEntity<TEntity>(TEntity entity)
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
        public override Task<TEntity> InsertEntityAsync<TEntity>(TEntity entity)
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
        public override Task UpdateEntityAsync<TEntity>(TEntity entity)
        {
            using (new ReaderWriteLockDisposable(_locker))
                DataContext.Update(entity);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates records in table, using values from entity parameter.
        /// Records to update are identified by match on primary key value from obj value.
        /// </summary>
        /// <param name="entities">Entities with data to update</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UpdateEntitiesAsync<TEntity>(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                await UpdateEntityAsync(entity);
        }

        /// <summary>
        /// Updates records in table, using values from entity parameter.
        /// Records to update are identified by match on primary key value from obj value.
        /// </summary>
        /// <param name="entities">Entities with data to update</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public override void UpdateEntities<TEntity>(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                UpdateEntity(entity);
        }

        /// <summary>
        /// Deletes record in table. Record to delete identified
        /// by match on primary key value from obj value.
        /// </summary>
        /// <param name="entity">Entity for delete operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public override Task DeleteEntityAsync<TEntity>(TEntity entity)
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
        public override Task BulkDeleteEntitiesAsync<TEntity>(IList<TEntity> entities)
        {
            using (new ReaderWriteLockDisposable(_locker))
            {
                foreach (var entity in entities)
                    DataContext.Delete(entity);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Performs delete records in a table by a condition
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public override Task<int> BulkDeleteEntitiesAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(DataContext.GetTable<TEntity>()
                .Where(predicate).Delete());
        }

        /// <summary>
        /// Performs bulk insert operation for entity collection.
        /// </summary>
        /// <param name="entities">Entities for insert operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public override Task BulkInsertEntitiesAsync<TEntity>(IEnumerable<TEntity> entities)
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
        public override IQueryable<TEntity> GetTable<TEntity>()
        {
            using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.Read))
                return DataContext.GetTable<TEntity>();
        }

        /// <summary>
        /// Get the current identity value
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <returns>Integer identity; null if cannot get the result</returns>
        public Task<int?> GetTableIdentAsync<TEntity>() where TEntity : BaseEntity
        {
            using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.Read))
            {
                var tableName = DataContext.GetTable<TEntity>().TableName;

                var result = DataContext.Query<int?>($"select seq from sqlite_sequence where name = \"{tableName}\"")
                    .FirstOrDefault();

                return Task.FromResult<int?>(result ?? 1);
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

        /// <summary>
        /// Build the connection string
        /// </summary>
        /// <param name="nopConnectionString">Connection string info</param>
        /// <returns>Connection string</returns>
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
        /// Executes command using System.Data.CommandType.StoredProcedure command type and
        /// returns results as collection of values of specified type
        /// </summary>
        /// <typeparam name="T">Result record type</typeparam>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Returns collection of query result records</returns>
        public override Task<IList<T>> QueryProcAsync<T>(string procedureName, params DataParameter[] parameters)
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
        public override Task<IList<T>> QueryAsync<T>(string sql, params DataParameter[] parameters)
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
        public override Task<int> ExecuteNonQueryAsync(string sql, params DataParameter[] dataParameters)
        {
            using (new ReaderWriteLockDisposable(_locker, ReaderWriteLockType.Read))
            {
                var command = CreateDbCommand(sql, dataParameters);
                return command.ExecuteAsync();
            }
        }

        /// <summary>
        /// Creates a new temporary storage and populate it using data from provided query
        /// </summary>
        /// <param name="storeKey">Name of temporary storage</param>
        /// <param name="query">Query to get records to populate created storage with initial data</param>
        /// <typeparam name="TItem">Storage record mapping class</typeparam>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the iQueryable instance of temporary storage
        /// </returns>
        public override Task<ITempDataStorage<TItem>> CreateTempDataStorageAsync<TItem>(string storeKey, IQueryable<TItem> query)
        {
            return Task.FromResult<ITempDataStorage<TItem>>(new TempSqlDataStorage<TItem>(storeKey, query, DataContext));
        }

        public Task<bool> DatabaseExistsAsync()
        {
            return Task.FromResult(DatabaseExists());
        }

        /// <summary>
        /// Truncates database table
        /// </summary>
        /// <param name="resetIdentity">Performs reset identity column</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public override Task TruncateAsync<TEntity>(bool resetIdentity = false)
        {
            using (new ReaderWriteLockDisposable(_locker))
                DataContext.GetTable<TEntity>().Truncate(resetIdentity);

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        protected DataConnection DataContext => _dataContext ??= CreateDataConnection();

        /// <summary>
        /// Linq2Db data provider
        /// </summary>
        protected override IDataProvider LinqToDbDataProvider { get; } = SQLiteTools.GetDataProvider(ProviderName.SQLiteMS);

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
