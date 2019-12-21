using Nop.Core;
using Nop.Core.Data;

namespace Nop.Data
{
    /// <summary>
    /// Represents the Entity Framework data provider manager
    /// </summary>
    public partial class EfDataProviderManager : IDataProviderManager
    {
        #region Properties

        /// <summary>
        /// Gets data provider
        /// </summary>
        public IDataProvider DataProvider
        {
            get
            {
                if (!DataSettingsManager.DatabaseIsInstalled)
                    return new InMemoryDataProvider();

                var providerName = DataSettingsManager.LoadSettings()?.DataProvider;
                switch (providerName)
                {
                    case DataProviderType.SqlServer:
                        return new SqlServerDataProvider();
                    default:
                        throw new NopException($"Not supported data provider name: '{providerName}'");
                }
            }
        }

        #endregion
    }
}