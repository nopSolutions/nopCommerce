using System.Collections.Generic;
using System.Data;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.Mapping;

namespace Nop.Data
{
    /// <summary>
    /// Implements database connection abstraction.
    /// </summary>
    internal class NopDataConnection : DataConnection
    {
        #region Ctor

        public NopDataConnection()
        {
            AddMappingSchema(AdditionalSchema);
        }

        #endregion

        #region Methods

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

            var outputParameters = dataParameters.Where(p => p.Direction == ParameterDirection.Output)
                .ToDictionary(p => p.Name, p => p);
            
            foreach (var outputParametersName in outputParameters.Keys.ToList())
            {
                outputParameters[outputParametersName].Value = (Command.Parameters[outputParametersName] as IDbDataParameter)?.Value;
            }

            return rez;
        }

        #endregion

        #region Properties

        public static MappingSchema AdditionalSchema { get; } = new MappingSchema();

        #endregion
    }
}
