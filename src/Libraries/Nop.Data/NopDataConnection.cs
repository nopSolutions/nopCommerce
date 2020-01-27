using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.Mapping;
using Nop.Core;

namespace Nop.Data
{
    /// <summary>
    /// Implements database connection abstraction.
    /// </summary>
    public class NopDataConnection : DataConnection, INopDataConnection
    {
        #region Ctor

        public NopDataConnection(IDataProvider dataProvider, IDbConnection connection) : base (dataProvider, connection)
        {
            
        }

        #endregion

        #region Utils

        private void UpdateParameterValue(DataParameter parameter)
        {
            if(parameter is null)
                throw new ArgumentNullException(nameof(parameter));
            
            if(Command == null || Command.Parameters.Count == 0 || !Command.Parameters.Contains(parameter.Name))
                return;;

            if(Command.Parameters[parameter.Name] is IDbDataParameter param)
            {
                parameter.Value = param.Value;
            }
        }

        private void UpdateOutputParameters(DataParameter[] dataParameters)
        {
            if(dataParameters is null|| dataParameters.Length == 0)
                return;

            foreach (var dataParam in dataParameters.Where(p => p.Direction == ParameterDirection.Output))
            {
                UpdateParameterValue(dataParam);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes command asynchronously and returns number of affected records.
        /// </summary>
        /// <param name="sqlStatement">Command text</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Number of records, affected by command execution.</returns>
        public int ExecuteNonQuery(string sqlStatement, params DataParameter[] parameters)
        {
            var command = new CommandInfo(this, sqlStatement, parameters);
            var affectedRecords = command.Execute();

            UpdateOutputParameters(parameters);

            return affectedRecords;
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
            var command = new CommandInfo(this, procedureName, parameters);

            var result = command.ExecuteProc<T>();
            UpdateOutputParameters(parameters);

            return result;
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
            var command = new CommandInfo(this, procedureName, parameters);

            var affectedRecords = command.ExecuteProc();
            UpdateOutputParameters(parameters);

            return affectedRecords;
        }

        public TEntity InsertEntity<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            entity.Id = this.InsertWithInt32Identity(entity);
            return entity;
        }

        public void BulkInsertEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : BaseEntity
        {
            this.BulkCopy<TEntity>(entities);
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
            var command = new CommandInfo(this, procedureName, parameters);

            var rez = command.QueryProc<TEntity>().ToList();

            UpdateOutputParameters(parameters);

            return rez ?? new List<TEntity>();
        }

        public IList<T> Query<T>(string sql, params DataParameter[] parameters)
        {
            return Query<T>(sql, parameters)?.ToList() ?? new List<T>();
        }

        #endregion
    }
}
