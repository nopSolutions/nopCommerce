using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nop.Core.Configuration
{
    /// <summary>
    /// Represents the app settings
    /// </summary>
    public partial class AppSettings
    {
        #region Fields

        private readonly Dictionary<Type, IConfig> _configurations = new();

        #endregion

        #region Ctor

        public AppSettings(IList<IConfig> configurations)
        {
            _configurations = configurations
                ?.ToDictionary(config => config.GetType(), config => config)
                ?? new Dictionary<Type, IConfig>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets raw configuration parameters
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, JToken> Configuration { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get configuration parameters by type
        /// </summary>
        /// <typeparam name="TConfig">Configuration type</typeparam>
        /// <returns>Configuration parameters</returns>
        public TConfig Get<TConfig>() where TConfig : class, IConfig
        {
            if (_configurations[typeof(TConfig)] is not TConfig config)
                throw new NopException($"No configuration with type '{typeof(TConfig)}' found");

            return config;
        }

        #endregion
    }
}