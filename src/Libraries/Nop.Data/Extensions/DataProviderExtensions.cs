using System;
using System.Data;
using LinqToDB;
using LinqToDB.Data;
using Nop.Data.Data;

namespace Nop.Data.Extensions
{
    /// <summary>
    /// Represents data provider extensions
    /// </summary>
    public static class DataProviderExtensions
    {
        #region Utilities

        /// <summary>
        /// Get DB parameter
        /// </summary>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbType">Data type</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        private static DataParameter GetParameter(this IDataProvider dataProvider, DataType dbType, string parameterName, object parameterValue)
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
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbType">Data type</param>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Parameter</returns>
        private static DataParameter GetOutputParameter(this IDataProvider dataProvider, DataType dbType, string parameterName)
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
        /// <param name="dataProvider">Data provider</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public static DataParameter GetStringParameter(this IDataProvider dataProvider, string parameterName, string parameterValue)
        {
            return dataProvider.GetParameter(DataType.NVarChar, parameterName, (object)parameterValue ?? DBNull.Value);
        }

        /// <summary>
        /// Get output string parameter
        /// </summary>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Parameter</returns>
        public static DataParameter GetOutputStringParameter(this IDataProvider dataProvider, string parameterName)
        {
            return dataProvider.GetOutputParameter(DataType.NVarChar, parameterName);
        }

        /// <summary>
        /// Get int parameter
        /// </summary>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public static DataParameter GetInt32Parameter(this IDataProvider dataProvider, string parameterName, int? parameterValue)
        {
            return dataProvider.GetParameter(DataType.Int32, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
        }
        
        /// <summary>
        /// Get output int32 parameter
        /// </summary>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Parameter</returns>
        public static DataParameter GetOutputInt32Parameter(this IDataProvider dataProvider, string parameterName)
        {
            return dataProvider.GetOutputParameter(DataType.Int32, parameterName);
        }

        /// <summary>
        /// Get boolean parameter
        /// </summary>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public static DataParameter GetBooleanParameter(this IDataProvider dataProvider, string parameterName, bool? parameterValue)
        {
            return dataProvider.GetParameter(DataType.Boolean, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
        }
        
        /// <summary>
        /// Get decimal parameter
        /// </summary>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public static DataParameter GetDecimalParameter(this IDataProvider dataProvider, string parameterName, decimal? parameterValue)
        {
            return dataProvider.GetParameter(DataType.Decimal, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
        }

        /// <summary>
        /// Get datetime parameter
        /// </summary>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public static DataParameter GetDateTimeParameter(this IDataProvider dataProvider, string parameterName, DateTime? parameterValue)
        {
            return dataProvider.GetParameter(DataType.DateTime, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
        }

        #endregion
    }
}