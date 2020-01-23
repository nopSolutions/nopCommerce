using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
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

        public NopDataConnection(INopDataProvider nopDataProvider) : base(nopDataProvider, nopDataProvider.CreateDbConnection())
        {
            AddMappingSchema(AdditionalSchema);
        }

        #endregion

        #region Utils

        private DataParameter GetParameterForCurrentCommand(string parameterName, ParameterDirection direction = ParameterDirection.Output)
        {
            if(string.IsNullOrWhiteSpace(parameterName))
                throw new ArgumentNullException(nameof(parameterName));
            
            if(Command == null || Command.Parameters.Count == 0 || !Command.Parameters.Contains(parameterName))
                return null;

            var dataParams = Command.Parameters.Cast<DataParameter>();

            return dataParams.FirstOrDefault(p => parameterName.Equals(p.Name, StringComparison.OrdinalIgnoreCase) ||
                p.Direction == direction);

        }

        private void UpdateOutputParameters(DataParameter[] dataParameters)
        {
            if(dataParameters is null|| dataParameters.Length == 0)
                return;

            foreach (var dataParam in dataParameters)
            {
                var currentParam = GetParameterForCurrentCommand(dataParam.Name);
                
                if(currentParam != null)
                {
                    dataParam.Value = currentParam.Value;
                }
            }
        }

        #endregion

        #region Methods

        public void ExecuteNonQuery(string sqlStatement, params DataParameter[] dataParameters)
        {
            var command = new CommandInfo(this, sqlStatement, dataParameters);
            command.Execute();

            UpdateOutputParameters(dataParameters);
        }

        /// <summary>
        /// Executes command using System.Data.CommandType.StoredProcedure command type and
        /// returns results as collection of values of specified type
        /// </summary>
        /// <typeparam name="TEntity">Result record type</typeparam>
        /// <param name="storeProcedureName">Procedure name</param>
        /// <param name="dataParameters">Command parameters</param>
        /// <returns>Returns collection of query result records</returns>
        public IList<TEntity> ExecuteStoredProcedure<TEntity>(string storeProcedureName, params DataParameter[] dataParameters)
        {
            var command = new CommandInfo(this, storeProcedureName, dataParameters);

            var rez = command.QueryProc<TEntity>().ToList();

            UpdateOutputParameters(dataParameters);

            return rez;
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

        public IList<T> QueryProc<T>(string procedureName, params DataParameter[] parameters)
        {
            return ExecuteStoredProcedure<T>(procedureName, parameters) ?? new List<T>();
        }

        public IList<T> Query<T>(string sql, params DataParameter[] parameters)
        {
            return Query<T>(sql, parameters)?.ToList() ?? new List<T>();
        }

        #endregion

        #region Properties

        public static MappingSchema AdditionalSchema { get; } = new MappingSchema();

        #endregion
    }
}
