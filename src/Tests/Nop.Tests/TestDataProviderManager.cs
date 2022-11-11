using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Configuration;
using Nop.Data.DataProviders;

namespace Nop.Tests
{
    /// <summary>
    /// Represents the data provider manager
    /// </summary>
    public partial class TestDataProviderManager : IDataProviderManager
    {
        #region Properties

        /// <summary>
        /// Gets data provider
        /// </summary>
        public INopDataProvider DataProvider
        {
            get
            {
                return Singleton<DataConfig>.Instance.DataProvider switch
                {
                    DataProviderType.SqlServer => new MsSqlNopDataProvider(),
                    DataProviderType.MySql => new MySqlNopDataProvider(),
                    DataProviderType.PostgreSQL => new PostgreSqlDataProvider(),
                    DataProviderType.Unknown => new SqLiteNopDataProvider(),
                    _ => throw new NopException($"Unknown [{Singleton<DataConfig>.Instance.DataProvider}] DataProvider")
                };
            }
        }

        #endregion
    }
}
