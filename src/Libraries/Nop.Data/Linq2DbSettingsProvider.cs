using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;
using Nop.Data.Data;

namespace Nop.Data
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
        public string DefaultConfiguration => "nopCommerce";

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
                    new ConnectionStringSettings
                    {
                        Name = "nopCommerce",
                        ProviderType = _dataSettings.DataProvider,
                        ConnectionString = _dataSettings.DataConnectionString
                    };
            }
        }
    }
}