using System;
using System.Data;
using LinqToDB;
using LinqToDB.Data;

namespace Nop.Data
{
    /// <summary>
    /// Helper class to use SqlParameter
    /// </summary>
    public static partial class SqlParameterHelper
    {
        #region Utilities

        /// <summary>
        /// Get DB parameter
        /// </summary>
        /// <param name="dbType">Data type</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        private static DataParameter GetParameter(DataType dbType, string parameterName, object parameterValue)
        {
            var parameter = new DataParameter
            {
                Name = parameterName,
                Value = parameterValue ?? DBNull.Value,
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
        private static DataParameter GetOutputParameter(DataType dbType, string parameterName)
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
        public static DataParameter GetStringParameter(string parameterName, string parameterValue)
        {
            return GetParameter(DataType.NVarChar, parameterName, parameterValue);
        }

        /// <summary>
        /// Get output string parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Parameter</returns>
        public static DataParameter GetOutputStringParameter(string parameterName)
        {
            return GetOutputParameter(DataType.NVarChar, parameterName);
        }

        /// <summary>
        /// Get int parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public static DataParameter GetInt32Parameter(string parameterName, int? parameterValue)
        {
            return GetParameter(DataType.Int32, parameterName, parameterValue);
        }

        /// <summary>
        /// Get output int32 parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Parameter</returns>
        public static DataParameter GetOutputInt32Parameter(string parameterName)
        {
            return GetOutputParameter(DataType.Int32, parameterName);
        }

        /// <summary>
        /// Get boolean parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public static DataParameter GetBooleanParameter(string parameterName, bool? parameterValue)
        {
            return GetParameter(DataType.Boolean, parameterName, parameterValue);
        }

        /// <summary>
        /// Get decimal parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public static DataParameter GetDecimalParameter(string parameterName, decimal? parameterValue)
        {
            return GetParameter(DataType.Decimal, parameterName, parameterValue);
        }

        /// <summary>
        /// Get datetime parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public static DataParameter GetDateTimeParameter(string parameterName, DateTime? parameterValue)
        {
            return GetParameter(DataType.DateTime, parameterName, parameterValue);
        }

        #endregion
    }
}
