using System;
using System.Collections.Generic;
using LinqToDB.Data;
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
        /// Initialize database
        /// </summary>
        void InitializeDatabase();

        /// <summary>
        /// Get string parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        DataParameter GetStringParameter(string parameterName, string parameterValue);

        /// <summary>
        /// Get output string parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Parameter</returns>
        DataParameter GetOutputStringParameter(string parameterName);

        /// <summary>
        /// Get int parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        DataParameter GetInt32Parameter(string parameterName, int? parameterValue);

        /// <summary>
        /// Get output int32 parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Parameter</returns>
        DataParameter GetOutputInt32Parameter(string parameterName);

        /// <summary>
        /// Get boolean parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        DataParameter GetBooleanParameter(string parameterName, bool? parameterValue);

        /// <summary>
        /// Get decimal parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        DataParameter GetDecimalParameter(string parameterName, decimal? parameterValue);

        /// <summary>
        /// Get datetime parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        DataParameter GetDateTimeParameter(string parameterName, DateTime? parameterValue);

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
        /// <param name="sql">Command text. This is caller's responsibility to properly escape procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Returns collection of query result records</returns>
        IEnumerable<T> QueryProc<T>(string sql, params DataParameter[] parameters);

        /// <summary>
        /// Executes command and returns results as collection of values of specified type
        /// </summary>
        /// <typeparam name="T">Result record type</typeparam>
        /// <param name="sql">Command text</param>
        /// <returns>Returns collection of query result records</returns>
        IEnumerable<T> Query<T>(string sql);

        /// <summary>
        /// Executes command and returns number of affected records
        /// </summary>
        /// <param name="sql">Command text</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Number of records, affected by command execution</returns>
        int Execute(string sql, params DataParameter[] parameters);

        #endregion
    }
}