using System.Linq;
using System.Collections.Generic;

namespace Nop.Core.Configuration
{
    /// <summary>
    /// Represents the event that is raised when App Settings are saving
    /// </summary>
    public class AppSettingsSavingEvent
    {
        #region Ctor

        public AppSettingsSavingEvent(IList<IConfig> configurations)
        {
            Configurations = configurations;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets configurations to save
        /// </summary>
        public IList<IConfig> Configurations { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Add configuration to save
        /// </summary>
        /// <param name="config">Configuration to save</param>
        public void AddConfig<TConfig>(TConfig config) where TConfig : class, IConfig
        {
            if (Configurations.OfType<TConfig>().FirstOrDefault() is TConfig currentConfig)
                Configurations[Configurations.IndexOf(currentConfig)] = config;
            else
                Configurations.Add(config);
        }

        #endregion
    }
}