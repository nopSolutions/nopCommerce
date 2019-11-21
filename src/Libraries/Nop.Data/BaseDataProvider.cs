using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using Nop.Core;

namespace Nop.Data
{
    /// <summary>
    /// Represents a base data provider
    /// </summary>
    public abstract partial class BaseDataProvider
    {
        #region Ctor

        protected readonly DataConnection _dataConnection;

        #endregion

        #region Ctor

        protected BaseDataProvider()
        {
            _dataConnection = new NopDataConnection();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get DB parameter
        /// </summary>
        /// <param name="dbType">Data type</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        protected virtual DataParameter GetParameter(DataType dbType, string parameterName, object parameterValue)
        {
            var parameter = new DataParameter
            {
                Name = parameterName,
                Value = parameterValue,
                DataType = dbType
            };

            return parameter;
        }

        /// <summary>
        /// Get output DB parameter
        /// </summary>
        /// <param name="dbType">Data type</param>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Parameter</returns>
        protected virtual DataParameter GetOutputParameter(DataType dbType, string parameterName)
        {
            var parameter = new DataParameter
            {
                Name = parameterName,
                DataType = dbType,
                Direction = ParameterDirection.Output
            };

            return parameter;
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Get string parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public virtual DataParameter GetStringParameter(string parameterName, string parameterValue)
        {
            return GetParameter(DataType.NVarChar, parameterName, (object)parameterValue ?? DBNull.Value);
        }

        /// <summary>
        /// Get output string parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Parameter</returns>
        public virtual DataParameter GetOutputStringParameter(string parameterName)
        {
            return GetOutputParameter(DataType.NVarChar, parameterName);
        }

        /// <summary>
        /// Get int parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public virtual DataParameter GetInt32Parameter(string parameterName, int? parameterValue)
        {
            return GetParameter(DataType.Int32, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
        }

        /// <summary>
        /// Get output int32 parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Parameter</returns>
        public virtual DataParameter GetOutputInt32Parameter(string parameterName)
        {
            return GetOutputParameter(DataType.Int32, parameterName);
        }

        /// <summary>
        /// Get boolean parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public virtual DataParameter GetBooleanParameter(string parameterName, bool? parameterValue)
        {
            return GetParameter(DataType.Boolean, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
        }

        /// <summary>
        /// Get decimal parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public virtual DataParameter GetDecimalParameter(string parameterName, decimal? parameterValue)
        {
            return GetParameter(DataType.Decimal, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
        }

        /// <summary>
        /// Get datetime parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public virtual DataParameter GetDateTimeParameter(string parameterName, DateTime? parameterValue)
        {
            return GetParameter(DataType.DateTime, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
        }

        /// <summary>
        /// Loads the original copy of the entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Copy of the passed entity</returns>
        public virtual TEntity LoadOriginalCopy<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            var entities = _dataConnection.GetTable<TEntity>();
            return entities.FirstOrDefault(e => e.Id == Convert.ToInt32(entity.Id));
        }

        public virtual IEnumerable<T> QueryProc<T>(string sql, params DataParameter[] parameters)
        {
            return _dataConnection.QueryProc<T>(sql, parameters);
        }

        public virtual IEnumerable<T> Query<T>(string sql)
        {
            return _dataConnection.Query<T>(sql);
        }

        public virtual int Execute(string sql, params DataParameter[] parameters)
        {
            return _dataConnection.Execute(sql, parameters);
        }

        #endregion
    }
}