using System;
using System.Data;
using System.Data.Common;
using Nop.Core.Data;

namespace Nop.Core.Extensions
{
    public static class DataProviderExtensions
    {
        private static DbParameter GetParameter(this IDataProvider dataProvider, DbType dbType, string parameterName,
            object parameterValue)
        {
            var parameter = dataProvider.GetParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            parameter.DbType = dbType;

            return parameter;
        }

        private static DbParameter GetOutputParameter(this IDataProvider dataProvider, DbType dbType, string parameterName)
        {
            var parameter = dataProvider.GetParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = dbType;
            parameter.Direction = ParameterDirection.Output;

            return parameter;
        }

        public static DbParameter GetStringParameter(this IDataProvider dataProvider, string parameterName, string parameterValue)
        {
            return dataProvider.GetParameter(DbType.String, parameterName, (object)parameterValue ?? DBNull.Value);
        }

        public static DbParameter GetOutputStringParameter(this IDataProvider dataProvider, string parameterName)
        {
            return dataProvider.GetOutputParameter(DbType.String, parameterName);
        }

        public static DbParameter GetInt32Parameter(this IDataProvider dataProvider, string parameterName, int? parameterValue)
        {
            return dataProvider.GetParameter(DbType.Int32, parameterName, parameterValue.HasValue ? (object) parameterValue.Value : DBNull.Value);
        }

        public static DbParameter GetOutputInt32Parameter(this IDataProvider dataProvider, string parameterName)
        {
            return dataProvider.GetOutputParameter(DbType.Int32, parameterName);
        }

        public static DbParameter GetInt32Parameter(this IDataProvider dataProvider, string parameterName, bool? parameterValue)
        {
            return dataProvider.GetParameter(DbType.Int32, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
        }

        public static DbParameter GetBooleanParameter(this IDataProvider dataProvider, string parameterName, bool? parameterValue)
        {
            return dataProvider.GetParameter(DbType.Boolean, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
        }

        public static DbParameter GetDecimalParameter(this IDataProvider dataProvider, string parameterName, decimal? parameterValue)
        {
            return dataProvider.GetParameter(DbType.Decimal, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
        }

        public static DbParameter GetDateTimeParameter(this IDataProvider dataProvider, string parameterName, DateTime? parameterValue)
        {
            return dataProvider.GetParameter(DbType.DateTime, parameterName, parameterValue.HasValue ? (object)parameterValue.Value : DBNull.Value);
        }
    }
}
