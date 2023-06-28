using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using Nop.Services.Logging;
using Nop.Core.Domain.Logging;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    /// <summary>
    /// Service to connect & do CRUD operations on ISAM
    /// </summary>
    public class BaseService : IBaseService
    {
        private string _connectionString = "DSN=abc-erp";
        private List<OdbcCommand> _batchCommands = new List<OdbcCommand>();

        private readonly CoreSettings _settings;
        private readonly ILogger _logger;

        public BaseService(
            CoreSettings settings,
            ILogger logger
        )
        {
            _settings = settings;
            _logger = logger;
        }

        /// <summary>
        /// Inserts into ISAM database given information
        /// </summary>
        /// <param name="table_name">insert table</param>
        /// <param name="column_names">columns inside table</param>
        /// <param name="parameters">values to be inserted</param>
        /// <param name="batch">choose whether to execute command now, or batch commands all at once</param>
        public void Insert(string table_name, IList<string> column_names, IList<OdbcParameter> parameters, bool batch = false)
        {
            IList<string> names = new List<string>();
            IList<string> vals = new List<string>();

            OdbcCommand dbCommand = new OdbcCommand();


            string query = "INSERT INTO " + table_name + " (" + string.Join(",", column_names) + ") "
                + "VALUES (" + BuildQuestionMarkStrings(parameters.Count) + ")";
            dbCommand.CommandText = query;
            SetParameters(dbCommand, parameters);

            if (batch)
            {
                _batchCommands.Add(dbCommand);
            }
            else
            {
                ExecuteSingleNonQuery(dbCommand);
            }
        }

        /// <summary>
        /// update a table row
        /// </summary>
        /// <param name="table_name">table to be written</param>
        /// <param name="column_names">columns to be changed</param>
        /// <param name="parameters">values to be changed to</param>
        /// <param name="where_clause">WHERE statement that chooses the row</param>
        /// <param name="where_params">parameters of where statement</param>
        /// <param name="batch">choose whether to execute command now, or batch commands all at once</param>
        public void Update(string table_name, IList<string> column_names, IList<OdbcParameter> parameters, string where_clause, IList<OdbcParameter> where_params, bool batch = false)
        {
            OdbcCommand dbCommand = new OdbcCommand();

            string query = "UPDATE " + table_name + " SET ";
            for (int i = 0; i < column_names.Count; i++)
            {
                query += column_names[i] + " = ?";

                // handle trailing comma
                if (i < column_names.Count - 1)
                {
                    query += ", ";
                }
                else
                {
                    query += " ";
                }

            }

            dbCommand.CommandText = query + where_clause;
            SetParameters(dbCommand, parameters);
            SetParameters(dbCommand, where_params);

            if (batch)
            {
                _batchCommands.Add(dbCommand);
            }
            else
            {
                ExecuteSingleNonQuery(dbCommand);
            }
        }

        /// <summary>
        /// delete a table row
        /// </summary>
        /// <param name="table_name">table to be deleted</param>
        /// <param name="where_clause">WHERE statement that chooses the row</param>
        /// <param name="where_params">parameters of where statement</param>
        /// <param name="batch">choose whether to execute command now, or batch commands all at once</param>
        public void Delete(string table_name, string where_clause, IList<OdbcParameter> where_params, bool batch = false)
        {
            OdbcCommand dbCommand = new OdbcCommand();
            dbCommand.CommandText = "DELETE FROM " + table_name + " WHERE " + where_clause;
            SetParameters(dbCommand, where_params);

            if (batch)
            {
                _batchCommands.Add(dbCommand);
            }
            else
            {
                ExecuteSingleNonQuery(dbCommand);
            }
        }

        /// <summary>
        /// select a table row
        /// </summary>
        /// <param name="table_name">table to be read</param>
        /// <param name="column_names">columns to be read</param>
        /// <param name="where_clause">WHERE statement chooses rows</param>
        /// <param name="where_params">parameters of where statement</param>
        public DataSet Get(string table_name, IList<string> columns, string where_clause, IList<OdbcParameter> where_params)
        {
            OdbcCommand dbCommand = new OdbcCommand();
            dbCommand.CommandText = "SELECT " + string.Join(",", columns) + " FROM " + table_name + " " + where_clause;
            SetParameters(dbCommand, where_params);

            return ExecuteQueries(dbCommand);
        }

        /// <summary>
        /// Allows the user to build up a batch of commands, then executes them all at once
        /// </summary>
        public void ExecuteBatch()
        {
            using (OdbcConnection dbConnection = new OdbcConnection(_connectionString))
            {
                // skip opening the connection if local
                if (!_settings.AreExternalCallsSkipped)
                {
                    OpenConnection(dbConnection);
                }

                try
                {
                    foreach (OdbcCommand dbCommand in _batchCommands)
                    {
                        if (!IsLocalMode(dbCommand))
                        {
                            dbCommand.Connection = dbConnection;
                            try
                            {
                                dbCommand.ExecuteNonQuery();
                            }
                            catch (OdbcException e)
                            {
                                // allow for "duplicate"
                                if (e.Message.Contains("add a duplicate value"))
                                {
                                    _logger.InsertLogAsync(
                                        LogLevel.Warning,
                                        "Attempt made to insert duplicate value into ODBC, skipping.",
                                        e.Message ?? string.Empty);
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }
                        dbCommand.Dispose();
                    }
                    _batchCommands.Clear();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The OdbcCommand returned the following message");
                    Console.WriteLine(ex.Message);
                    _batchCommands.Clear();
                    throw;
                }
            }
        }

        /// <summary>
        /// private utility to execute a single command
        /// </summary>
        /// <param name="dbCommand">to be executed</param>
        private void ExecuteSingleNonQuery(OdbcCommand dbCommand)
        {
            if (!IsLocalMode(dbCommand)) { return; }

            using (OdbcConnection dbConnection = new OdbcConnection(_connectionString))
            {
                dbCommand.Connection = dbConnection;
                OpenConnection(dbConnection);

                try
                {
                    dbCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Executing the query '" + dbCommand.CommandText + "' failed.");
                    Console.WriteLine("The OdbcCommand returned the following message");
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }


        /// <summary>
        /// Executes a Get command
        /// </summary>
        /// <param name="comm">command to be executed</param>
        /// <returns>Dataset that contains information</returns>
        private DataSet ExecuteQueries(OdbcCommand comm)
        {
            if (IsLocalMode(comm)) { return new DataSet(); }

            using (OdbcConnection dbConnection = new OdbcConnection(_connectionString))
            {
                OpenConnection(dbConnection);

                comm.Connection = dbConnection;
                OdbcDataAdapter adapter = new OdbcDataAdapter(comm);

                var dataSet = new DataSet();

                try
                {
                    adapter.Fill(dataSet);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Executing the query '" + comm.CommandText + "' failed.");
                    Console.WriteLine("The OdbcCommand returned the following message");
                    Console.WriteLine(ex.Message);
                    comm.Dispose();
                    throw;
                }
                comm.Dispose();

                return dataSet;
            }
        }

        /// <summary>
        /// boilerplate open connection
        /// </summary>
        /// <param name="conn"></param>
        private void OpenConnection(IDbConnection conn)
        {
            try
            {
                conn.Open();
            }
            catch (OdbcException ex)
            {
                Console.WriteLine("connection to the DSN '" + _connectionString + "' failed.");
                Console.WriteLine("The OdbcConnection returned the following message");
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// gives question marks separated by ", "
        /// </summary>
        /// <param name="nRepeats"># of question marks to be repeated</param>
        /// <returns></returns>
        private string BuildQuestionMarkStrings(int nRepeats)
        {
            string qMarks = "";
            for (int i = 0; i < nRepeats - 1; ++i)
            {
                qMarks += "?, ";
            }
            qMarks += "? ";
            return qMarks;
        }

        /// <summary>
        /// sets 
        /// </summary>
        /// <param name="dbCommand"></param>
        /// <param name="parameters"></param>
        private void SetParameters(OdbcCommand dbCommand, IList<OdbcParameter> parameters)
        {
            foreach (var param in parameters)
            {
                dbCommand.Parameters.Add(param.ParameterName, param.OdbcType).Value = param.Value;
            }
        }

        // Adds a log statement if in local mode.
        private bool IsLocalMode(OdbcCommand dbCommand)
        {
            if (_settings.AreExternalCallsSkipped)
            {
                var fullMessage = $"Query: {dbCommand.CommandText}";
                _logger.InsertLogAsync(
                    LogLevel.Error,
                    "ISAM query skipped.",
                    fullMessage
                );

                return true;
            }

            return false;
        }
    }
}
