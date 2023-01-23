using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FluentMigrator;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Expressions;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.Mapping;
using LinqToDB.Tools;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data.DataProviders.Interceptors;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace Nop.Data.DataProviders
{
    public abstract partial class BaseDataProvider : IMappingEntityAccessor
    {
        #region Fields

        protected static ConcurrentDictionary<Type, NopEntityDescriptor> EntityDescriptors { get; } = new ConcurrentDictionary<Type, NopEntityDescriptor>();

        #endregion

        #region Utils

        /// <summary>
        /// Gets a connection to the database for a current data provider
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Connection to a database</returns>
        protected abstract DbConnection GetInternalDbConnection(string connectionString);

        /// <summary>
        /// Creates the database connection
        /// </summary>
        protected virtual DataConnection CreateDataConnection()
        {
            return CreateDataConnection(LinqToDbDataProvider);
        }

        /// <summary>
        /// Creates database command instance using provided command text and parameters.
        /// </summary>
        /// <param name="sql">Command text</param>
        /// <param name="dataParameters">Command parameters</param>
        protected virtual CommandInfo CreateDbCommand(string sql, DataParameter[] dataParameters)
        {
            if (dataParameters is null)
                throw new ArgumentNullException(nameof(dataParameters));

            var dataConnection = CreateDataConnection(LinqToDbDataProvider);

            return new CommandInfo(dataConnection, sql, dataParameters);
        }

        /// <summary>
        /// Creates the database connection
        /// </summary>
        /// <param name="dataProvider">Data provider</param>
        /// <returns>Database connection</returns>
        protected virtual DataConnection CreateDataConnection(IDataProvider dataProvider)
        {
            if (dataProvider is null)
                throw new ArgumentNullException(nameof(dataProvider));

            var dataConnection = new DataConnection(dataProvider, CreateDbConnection(), GetMappingSchema())
            {
                CommandTimeout = DataSettingsManager.GetSqlCommandTimeout()
            };

            if (MiniProfillerEnabled)
            {
                dataConnection.AddInterceptor(UnwrapProfilerInterceptor.Instance);
            }

            return dataConnection;
        }

        /// <summary>
        /// Creates a connection to a database
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Connection to a database</returns>
        protected virtual DbConnection CreateDbConnection(string connectionString = null)
        {
            var dbConnection = GetInternalDbConnection(!string.IsNullOrEmpty(connectionString) ? connectionString : GetCurrentConnectionString());

            return MiniProfillerEnabled ? new ProfiledDbConnection(dbConnection, MiniProfiler.Current) : dbConnection;
        }

        /// <summary>
        /// Gets a data hash from database side
        /// </summary>
        /// <param name="binaryData">Array for a hashing function</param>
        /// <returns>Data hash</returns>
        /// <remarks>
        /// For SQL Server 2014 (12.x) and earlier, allowed input values are limited to 8000 bytes.
        /// https://docs.microsoft.com/en-us/sql/t-sql/functions/hashbytes-transact-sql
        /// </remarks>
        [Sql.Expression("CONVERT(VARCHAR(128), HASHBYTES('SHA2_512', SUBSTRING({0}, 0, 8000)), 2)", ServerSideOnly = true, Configuration = ProviderName.SqlServer)]
        [Sql.Expression("SHA2({0}, 512)", ServerSideOnly = true, Configuration = ProviderName.MySql)]
        [Sql.Expression("encode(digest({0}, 'sha512'), 'hex')", ServerSideOnly = true, Configuration = ProviderName.PostgreSQL)]
        protected static string SqlSha2(object binaryData)
        {
            throw new InvalidOperationException("This function should be used only in database code");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize database
        /// </summary>
        public virtual void InitializeDatabase()
        {
            var migrationManager = EngineContext.Current.Resolve<IMigrationManager>();

            var targetAssembly = typeof(NopDbStartup).Assembly;
            migrationManager.ApplyUpMigrations(targetAssembly);

            var typeFinder = Singleton<ITypeFinder>.Instance;
            var mAssemblies = typeFinder.FindClassesOfType<MigrationBase>()
                .Select(t => t.Assembly)
                .Where(assembly => !assembly.FullName.Contains("FluentMigrator.Runner"))
                .Distinct()
                .ToArray();

            //mark update migrations as applied
            foreach (var assembly in mAssemblies)
            {
                migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update, true);
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
        public virtual Task<ITempDataStorage<TItem>> CreateTempDataStorageAsync<TItem>(string storeKey, IQueryable<TItem> query)
            where TItem : class
        {
            return Task.FromResult<ITempDataStorage<TItem>>(new TempSqlDataStorage<TItem>(storeKey, query, CreateDataConnection()));
        }

        /// <summary>
        /// Returns mapped entity descriptor
        /// </summary>
        /// <param name="entityType">Type of entity</param>
        /// <returns>Mapped entity descriptor</returns>
        public virtual NopEntityDescriptor GetEntityDescriptor(Type entityType)
        {
            return EntityDescriptors.GetOrAdd(entityType, t =>
            {
                var tableName = NameCompatibilityManager.GetTableName(t);
                var expression = new CreateTableExpression { TableName = tableName };
                var builder = new CreateTableExpressionBuilder(expression, new NullMigrationContext());
                builder.RetrieveTableExpressions(t);

                return new NopEntityDescriptor
                {
                    EntityName = tableName,
                    SchemaName = builder.Expression.SchemaName,
                    Fields = builder.Expression.Columns.Select(column => new NopEntityFieldDescriptor
                    {
                        Name = column.Name,
                        IsPrimaryKey = column.IsPrimaryKey,
                        IsNullable = column.IsNullable,
                        Size = column.Size,
                        Precision = column.Precision,
                        IsIdentity = column.IsIdentity,
                        Type = getPropertyTypeByColumnName(t, column.Name)
                    }).ToList()
                };
            });

            static Type getPropertyTypeByColumnName(Type targetType, string name)
            {
                var (mappedType, _) = Array.Find(targetType
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty), pi => name.Equals(NameCompatibilityManager.GetColumnName(targetType, pi.Name))).PropertyType.GetTypeToMap();

                return mappedType;
            }
        }

        /// <summary>
        /// Get hash values of a stored entity field
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="keySelector">A key selector which should project to a dictionary key</param>
        /// <param name="fieldSelector">A field selector to apply a transform to a hash value</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Dictionary</returns>
        public virtual async Task<IDictionary<int, string>> GetFieldHashesAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, int>> keySelector,
            Expression<Func<TEntity, object>> fieldSelector) where TEntity : BaseEntity
        {
            if (keySelector.Body is not MemberExpression keyMember ||
                keyMember.Member is not PropertyInfo keyPropInfo)
            {
                throw new ArgumentException($"Expression '{keySelector}' refers to method or field, not a property.");
            }

            if (fieldSelector.Body is not MemberExpression member ||
                member.Member is not PropertyInfo propInfo)
            {
                throw new ArgumentException($"Expression '{fieldSelector}' refers to a method or field, not a property.");
            }

            var hashes = GetTable<TEntity>()
                .Where(predicate)
                .Select(x => new
                {
                    Id = Sql.Property<int>(x, keyPropInfo.Name),
                    Hash = SqlSha2(Sql.Property<object>(x, propInfo.Name))
                });

            return await AsyncIQueryableExtensions.ToDictionaryAsync(hashes, p => p.Id, p => p.Hash);
        }

        /// <summary>
        /// Get or create mapping schema with specified configuration name (<see cref="ConfigurationName"/>) and base mapping schema
        /// </summary>
        public MappingSchema GetMappingSchema()
        {
            return Singleton<MappingSchema>.Instance ??= new MappingSchema(ConfigurationName, LinqToDbDataProvider.MappingSchema)
            {
                MetadataReader = new FluentMigratorMetadataReader(this)
            };
        }

        /// <summary>
        /// Returns queryable source for specified mapping class for current connection,
        /// mapped to database table or view.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Queryable source</returns>
        public virtual IQueryable<TEntity> GetTable<TEntity>() where TEntity : BaseEntity
        {
            return new DataContext(LinqToDbDataProvider, GetCurrentConnectionString())
            {
                MappingSchema = GetMappingSchema(),
                CommandTimeout = DataSettingsManager.GetSqlCommandTimeout()
            }
                .GetTable<TEntity>();
        }

        /// <summary>
        /// Inserts record into table. Returns inserted entity with identity
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the inserted entity
        /// </returns>
        public virtual async Task<TEntity> InsertEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            using var dataContext = CreateDataConnection();
            entity.Id = await dataContext.InsertWithInt32IdentityAsync(entity);
            return entity;
        }

        /// <summary>
        /// Inserts record into table. Returns inserted entity with identity
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns>Inserted entity</returns>
        public virtual TEntity InsertEntity<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            using var dataContext = CreateDataConnection();
            entity.Id = dataContext.InsertWithInt32Identity(entity);
            return entity;
        }

        /// <summary>
        /// Updates record in table, using values from entity parameter.
        /// Record to update identified by match on primary key value from obj value.
        /// </summary>
        /// <param name="entity">Entity with data to update</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            using var dataContext = CreateDataConnection();
            await dataContext.UpdateAsync(entity);
        }

        /// <summary>
        /// Updates record in table, using values from entity parameter.
        /// Record to update identified by match on primary key value from obj value.
        /// </summary>
        /// <param name="entity">Entity with data to update</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public virtual void UpdateEntity<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            using var dataContext = CreateDataConnection();
            dataContext.Update(entity);
        }

        /// <summary>
        /// Updates records in table, using values from entity parameter.
        /// Records to update are identified by match on primary key value from obj value.
        /// </summary>
        /// <param name="entities">Entities with data to update</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            //we don't use the Merge API on this level, because this API not support all databases.
            //you may see all supported databases by the following link: https://linq2db.github.io/articles/sql/merge/Merge-API.html#supported-databases
            foreach (var entity in entities)
                await UpdateEntityAsync(entity);
        }

        /// <summary>
        /// Updates records in table, using values from entity parameter.
        /// Records to update are identified by match on primary key value from obj value.
        /// </summary>
        /// <param name="entities">Entities with data to update</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public virtual void UpdateEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            //we don't use the Merge API on this level, because this API not support all databases.
            //you may see all supported databases by the following link: https://linq2db.github.io/articles/sql/merge/Merge-API.html#supported-databases
            foreach (var entity in entities)
                UpdateEntity(entity);
        }

        /// <summary>
        /// Deletes record in table. Record to delete identified
        /// by match on primary key value from obj value.
        /// </summary>
        /// <param name="entity">Entity for delete operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            using var dataContext = CreateDataConnection();
            await dataContext.DeleteAsync(entity);
        }

        /// <summary>
        /// Deletes record in table. Record to delete identified
        /// by match on primary key value from obj value.
        /// </summary>
        /// <param name="entity">Entity for delete operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public virtual void DeleteEntity<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            using var dataContext = CreateDataConnection();
            dataContext.Delete(entity);
        }

        /// <summary>
        /// Performs delete records in a table
        /// </summary>
        /// <param name="entities">Entities for delete operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task BulkDeleteEntitiesAsync<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity
        {
            using var dataContext = CreateDataConnection();
            if (entities.All(entity => entity.Id == 0))
            {
                foreach (var entity in entities)
                    await dataContext.DeleteAsync(entity);
            }
            else
            {
                await dataContext.GetTable<TEntity>()
                   .Where(e => e.Id.In(entities.Select(x => x.Id)))
                   .DeleteAsync();
            }
        }

        /// <summary>
        /// Performs delete records in a table
        /// </summary>
        /// <param name="entities">Entities for delete operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public virtual void BulkDeleteEntities<TEntity>(IList<TEntity> entities) where TEntity : BaseEntity
        {
            using var dataContext = CreateDataConnection();
            if (entities.All(entity => entity.Id == 0))
                foreach (var entity in entities)
                    dataContext.Delete(entity);
            else
                dataContext.GetTable<TEntity>()
                    .Where(e => e.Id.In(entities.Select(x => x.Id)))
                    .Delete();
        }

        /// <summary>
        /// Performs delete records in a table by a condition
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of deleted records
        /// </returns>
        public virtual async Task<int> BulkDeleteEntitiesAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : BaseEntity
        {
            using var dataContext = CreateDataConnection();
            return await dataContext.GetTable<TEntity>()
                .Where(predicate)
                .DeleteAsync();
        }

        /// <summary>
        /// Performs delete records in a table by a condition
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>
        /// The number of deleted records
        /// </returns>
        public virtual int BulkDeleteEntities<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : BaseEntity
        {
            using var dataContext = CreateDataConnection();
            return dataContext.GetTable<TEntity>()
                .Where(predicate)
                .Delete();
        }

        /// <summary>
        /// Performs bulk insert operation for entity collection.
        /// </summary>
        /// <param name="entities">Entities for insert operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task BulkInsertEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            using var dataContext = CreateDataConnection(LinqToDbDataProvider);
            await dataContext.BulkCopyAsync(new BulkCopyOptions(), entities.RetrieveIdentity(dataContext));
        }

        /// <summary>
        /// Performs bulk insert operation for entity collection.
        /// </summary>
        /// <param name="entities">Entities for insert operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public virtual void BulkInsertEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            using var dataContext = CreateDataConnection(LinqToDbDataProvider);
            dataContext.BulkCopy(new BulkCopyOptions(), entities.RetrieveIdentity(dataContext));
        }

        /// <summary>
        /// Executes command asynchronously and returns number of affected records
        /// </summary>
        /// <param name="sql">Command text</param>
        /// <param name="dataParameters">Command parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of records, affected by command execution.
        /// </returns>
        public virtual async Task<int> ExecuteNonQueryAsync(string sql, params DataParameter[] dataParameters)
        {
            var command = CreateDbCommand(sql, dataParameters);

            return await command.ExecuteAsync();
        }

        /// <summary>
        /// Executes command using System.Data.CommandType.StoredProcedure command type and
        /// returns results as collection of values of specified type
        /// </summary>
        /// <typeparam name="T">Result record type</typeparam>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the returns collection of query result records
        /// </returns>
        public virtual Task<IList<T>> QueryProcAsync<T>(string procedureName, params DataParameter[] parameters)
        {
            var command = CreateDbCommand(procedureName, parameters);
            var rez = command.QueryProc<T>()?.ToList();
            return Task.FromResult<IList<T>>(rez ?? new List<T>());
        }

        /// <summary>
        /// Executes SQL command and returns results as collection of values of specified type
        /// </summary>
        /// <typeparam name="T">Type of result items</typeparam>
        /// <param name="sql">SQL command text</param>
        /// <param name="parameters">Parameters to execute the SQL command</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the collection of values of specified type
        /// </returns>
        public virtual Task<IList<T>> QueryAsync<T>(string sql, params DataParameter[] parameters)
        {
            using var dataContext = CreateDataConnection();
            return Task.FromResult<IList<T>>(dataContext.Query<T>(sql, parameters)?.ToList() ?? new List<T>());
        }

        /// <summary>
        /// Truncates database table
        /// </summary>
        /// <param name="resetIdentity">Performs reset identity column</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public virtual async Task TruncateAsync<TEntity>(bool resetIdentity = false) where TEntity : BaseEntity
        {
            using var dataContext = CreateDataConnection(LinqToDbDataProvider);
            await dataContext.GetTable<TEntity>().TruncateAsync(resetIdentity);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Linq2Db data provider
        /// </summary>
        protected abstract IDataProvider LinqToDbDataProvider { get; }

        /// <summary>
        /// Gets or sets a value that indicates whether should use MiniProfiler for the current connection
        /// </summary>
        protected static bool MiniProfillerEnabled => Singleton<AppSettings>.Instance.Get<CommonConfig>().MiniProfilerEnabled;

        /// <summary>
        /// Database connection string
        /// </summary>
        protected static string GetCurrentConnectionString()
        {
            return DataSettingsManager.LoadSettings().ConnectionString;
        }

        /// <summary>
        /// Name of database provider
        /// </summary>
        public string ConfigurationName => LinqToDbDataProvider.Name;

        #endregion
    }
}