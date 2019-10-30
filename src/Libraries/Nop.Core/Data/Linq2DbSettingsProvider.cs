using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;

namespace Nop.Core.Data
{
    /// <summary>
    /// Linq2db settings provider
    /// </summary>
    public class Linq2DbSettingsProvider : ILinqToDBSettings
    {
        private readonly DataSettings _dataSettings;

        public Linq2DbSettingsProvider(DataSettings dataSettings)
        {
            _dataSettings = dataSettings;
        }

        /// <summary>
        /// Gets list of data provider settings
        /// </summary>
        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

        /// <summary>
        /// Gets name of default connection configuration
        /// </summary>
        public string DefaultConfiguration => "SqlServer";

        /// <summary>
        /// Gets name of default data provider configuration
        /// </summary>
        public string DefaultDataProvider => DataProviderType.SqlServer.ToString();

        /// <summary>
        /// Gets list of connection configurations
        /// </summary>
        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    new ConnectionStringConfigurationProvider
                    {
                        Name = "nopCommerce",
                        ProviderName = _dataSettings.DataProvider.ToString(),
                        ConnectionString = _dataSettings.DataConnectionString
                    };
            }
        }
    }
}