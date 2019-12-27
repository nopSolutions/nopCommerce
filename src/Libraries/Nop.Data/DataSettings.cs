using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nop.Data
{
    /// <summary>
    /// Represents the data settings
    /// </summary>
    public partial class DataSettings : IConnectionStringSettings, ILinqToDBSettings
    {
        #region Ctor

        public DataSettings()
        {
            RawDataSettings = new Dictionary<string, string>();
        }

        #endregion

        #region Properties

        #region IConnectionStringSettings

        /// <summary>
        /// Gets or sets a connection string
        /// </summary>
        [JsonProperty(PropertyName = "DataConnectionString")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets connection configuration name
        /// </summary>
        [JsonIgnore]
        public string Name => DefaultConfiguration;

        /// <summary>
        /// Gets or sets data provider configuration name
        /// </summary>
        [JsonIgnore]
        public string ProviderName => DataProvider.ToString();

        /// <summary>
        /// Is this connection configuration defined on global level (machine.config) or on application level.
        /// </summary>
        [JsonIgnore]
        public bool IsGlobal => false;

        #endregion

        #region ILinqToDBSettings

        /// <summary>
        /// Gets list of data provider settings
        /// </summary>
        [JsonIgnore]
        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

        /// <summary>
        /// Gets name of default connection configuration
        /// </summary>
        [JsonIgnore]
        public string DefaultConfiguration => "nopCommerce";

        /// <summary>
        /// Gets name of default data provider configuration
        /// </summary>
        [JsonIgnore]
        public string DefaultDataProvider => Enum.GetName(typeof(DataProviderType), DataProvider);

        /// <summary>
        /// Gets list of connection configurations
        /// </summary>
        [JsonIgnore]
        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return this;
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets a data provider
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DataProviderType DataProvider { get; set; }

        /// <summary>
        /// Gets or sets a raw settings
        /// </summary>
        public IDictionary<string, string> RawDataSettings { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the information is entered
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public bool IsValid => DataProvider != DataProviderType.Unknown && !string.IsNullOrEmpty(ConnectionString);
        
        #endregion
    }
}