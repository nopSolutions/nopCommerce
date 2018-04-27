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
                var providerName = DataSettingsManager.LoadSettings()?.DataProvider;
                if (string.IsNullOrEmpty(providerName))
                    throw new NopException("Data settings doesn't contain a provider name");

                switch (providerName.ToLowerInvariant())
                {
                    case "sqlserver":
                        return new SqlServerDataProvider();

                    //case "sqlce":
                    //    return new SqlCeDataProvider();

                    default:
                        throw new NopException($"Not supported data provider name: '{providerName}'");
                }
            }
        }

        #endregion
    }
}