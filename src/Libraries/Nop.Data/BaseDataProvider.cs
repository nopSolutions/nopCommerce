using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.Mapping;
using LinqToDB.Tools;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data.Mapping;

namespace Nop.Data
{
    public abstract class BaseDataProvider
    {
        public abstract IDbConnection CreateDbConnection(string connectionString = null);

        #region Utils

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
            {
                parameter.Value = param.Value;
            }
        }

        private void UpdateOutputParameters(DataConnection dataConnection, DataParameter[] dataParameters)
        {
            if (dataParameters is null || dataParameters.Length == 0)
                return;

            foreach (var dataParam in dataParameters.Where(p => p.Direction == ParameterDirection.Output))
            {
                UpdateParameterValue(dataConnection, dataParam);
            }
        }

        #endregion

        #region Methods

        public virtual DataConnection CreateDataContext()
        {
            return CreateDataContext(LinqToDbDataProvider);
        }

        public DataConnection CreateDataContext(IDataProvider dataProvider)
        {
            if (dataProvider is null)
                throw new ArgumentNullException(nameof(dataProvider));

            var dataContext = new DataConnection(dataProvider, CreateDbConnection());
            dataContext.AddMappingSchema(AdditionalSchema);

            return dataContext;
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
        /// Returns queryable source for specified mapping class for current connection,
        /// mapped to database table or view.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Queryable source</returns>
        public virtual ITable<TEntity> GetTable<TEntity>() where TEntity : BaseEntity
        {
            return new DataContext(LinqToDbDataProvider, CurrentConnectionString) { MappingSchema = AdditionalSchema }.GetTable<TEntity>();
        }

        /// <summary>
        /// Inserts record into table. Returns inserted entity with identity
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns>Inserted entity</returns>
        public TEntity InsertEntity<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            using (var dataContext = CreateDataContext())
            {
                entity.Id = dataContext.InsertWithInt32Identity(entity);
                return entity;
            }
        }

        /// <summary>
        /// Updates record in table, using values from entity parameter. 
        /// Record to update identified by match on primary key value from obj value.
        /// </summary>
        /// <param name="entity">Entity with data to update</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public void UpdateEntity<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            using (var dataContext = CreateDataContext())
            {
                dataContext.Update(entity);
            }
        }

        /// <summary>
        /// Deletes record in table. Record to delete identified
        /// by match on primary key value from obj value.
        /// </summary>
        /// <param name="entity">Entity for delete operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public void DeleteEntity<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            using (var dataContext = CreateDataContext())
            {
                dataContext.Delete(entity);
            }
        }

        /// <summary>
        /// Performs bulk insert operation for entity colllection.
        /// </summary>
        /// <param name="entities"></param>
        /// <typeparam name="TEntity"></typeparam>
        public void BulkInsertEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            using (var dataContext = CreateDataContext(LinqToDbDataProvider))
            {
                dataContext.BulkCopy(new LinqToDB.Data.BulkCopyOptions(), entities.RetrieveIdentity(dataContext));
            }
        }

        /// <summary>
        /// Executes command asynchronously and returns number of affected records.
        /// </summary>
        /// <param name="sqlStatement">Command text</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Number of records, affected by command execution.</returns>
        public int ExecuteNonQuery(string sqlStatement, params DataParameter[] parameters)
        {
            using (var dataContext = CreateDataContext())
            {
                var command = new CommandInfo(dataContext, sqlStatement, parameters);
                var affectedRecords = command.Execute();

                UpdateOutputParameters(dataContext, parameters);

                return affectedRecords;
            }
        }

        /// <summary>
        /// Executes command using LinqToDB.Mapping.StoredProcedure command type and returns
        /// single value
        /// </summary>
        /// <typeparam name="TEntity">Result record type</typeparam>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Resulting value</returns>
        public T ExecuteStoredProcedure<T>(string procedureName, params DataParameter[] parameters)
        {
            using (var dataContext = CreateDataContext())
            {
                var command = new CommandInfo(dataContext, procedureName, parameters);

                var result = command.ExecuteProc<T>();
                UpdateOutputParameters(dataContext, parameters);

                return result;
            }
        }

        /// <summary>
        /// Executes command using LinqToDB.Mapping.StoredProcedure command type and returns
        /// number of affected records.
        /// </summary>
        /// <typeparam name="TEntity">Result record type</typeparam>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Number of records, affected by command execution.</returns>
        public int ExecuteStoredProcedure(string procedureName, params DataParameter[] parameters)
        {
            using (var dataContext = CreateDataContext())
            {
                var command = new CommandInfo(dataContext, procedureName, parameters);

                var affectedRecords = command.ExecuteProc();
                UpdateOutputParameters(dataContext, parameters);

                return affectedRecords;
            }
        }

        /// <summary>
        /// Executes command using System.Data.CommandType.StoredProcedure command type and
        /// returns results as collection of values of specified type
        /// </summary>
        /// <typeparam name="TEntity">Result record type</typeparam>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Returns collection of query result records</returns>
        public IList<TEntity> QueryProc<TEntity>(string procedureName, params DataParameter[] parameters) where TEntity : BaseEntity
        {
            using (var dataContext = CreateDataContext())
            {
                var command = new CommandInfo(dataContext, procedureName, parameters);
                var rez = command.QueryProc<TEntity>().ToList();
                UpdateOutputParameters(dataContext, parameters);
                return rez ?? new List<TEntity>();
            }
        }

        /// <summary>
        /// Executes SQL command and returns results as collection of values of specified type
        /// </summary>
        /// <typeparam name="T">Type of result items</typeparam>
        /// <param name="sql">SQL command text</param>
        /// <param name="parameters">Parameters to execute the SQL command</param>
        /// <returns>Collection of values of specified type</returns>
        public IList<T> Query<T>(string sql, params DataParameter[] parameters)
        {
            using (var dataContext = CreateDataContext())
            {
                return dataContext.Query<T>(sql, parameters)?.ToList() ?? new List<T>();
            }
        }

        #endregion

        protected abstract IDataProvider LinqToDbDataProvider { get; }

        protected static MappingSchema AdditionalSchema
        {
            get
            {
                if (Singleton<MappingSchema>.Instance is null)
                    Singleton<MappingSchema>.Instance = new MappingSchema { MetadataReader = new FluentMigratorMetadataReader() };

                return Singleton<MappingSchema>.Instance;
            }
        }

        protected string CurrentConnectionString => DataSettingsManager.LoadSettings().ConnectionString;

        /// <summary>
        /// Name of database provider
        /// </summary>
        public string DataProviderName => LinqToDbDataProvider.Name;
    }
}