using Nop.Core;
using Nop.Core.Data;
using Nop.Data;

namespace Nop.Plugin.Data.MySQL.Data
{
    /// <summary>
    /// Represents the Entity Framework data provider manager
    /// </summary>
    public class DataProviderManager : IDataProviderManager
    {
        #region Properties

        /// <summary>
        /// Gets data provider
        /// </summary>
        public IDataProvider DataProvider
        {
            get
            {
                var providerName = DataSettingsManager.LoadSettings()?.DataProvider;
                switch (providerName)
                {
                    case DataProviderType.SqlServer:
                        return new SqlServerDataProvider();
                    
                    case DataProviderType.MySQL:
                        return new MySQLDataProvider();

                    default:
                        throw new NopException($"Not supported data provider name: '{providerName}'");
                }
            }
        }

        #endregion
    }
}
