using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Core.Data
{
    /// <summary>
    /// Represents the data settings
    /// </summary>
    public partial class DataSettings
    {
        #region Ctor

        public DataSettings()
        {
            RawDataSettings = new Dictionary<string, string>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a data provider
        /// </summary>
        public string DataProvider { get; set; }

        /// <summary>
        /// Gets or sets a connection string
        /// </summary>
        public string DataConnectionString { get; set; }

        /// <summary>
        /// Gets or sets a raw settings
        /// </summary>
        public IDictionary<string, string> RawDataSettings { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the information is entered
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public bool IsValid => !string.IsNullOrEmpty(this.DataProvider) && !string.IsNullOrEmpty(this.DataConnectionString);

        #endregion
    }
}