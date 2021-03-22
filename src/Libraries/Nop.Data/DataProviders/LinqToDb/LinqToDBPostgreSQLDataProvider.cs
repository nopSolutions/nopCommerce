using System.Data;
using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;

namespace Nop.Data.DataProviders.LinqToDB
{
    /// <summary>
    /// Represents a data provider for PostgreSQL
    /// </summary>
    public class LinqToDBPostgreSQLDataProvider : PostgreSQLDataProvider
    {
        public LinqToDBPostgreSQLDataProvider() : base(ProviderName.PostgreSQL, PostgreSQLVersion.v95) { }

        public override void SetParameter(DataConnection dataConnection, IDbDataParameter parameter, string name, DbDataType dataType, object value)
        {
            if (value is string && dataType.DataType == DataType.NVarChar)
            {
                dataType = dataType.WithDbType("citext");
            }

            base.SetParameter(dataConnection, parameter, name, dataType, value);
        }
    }
}
