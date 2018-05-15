using System.Collections.Generic;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Plugins uploaded event
    /// </summary>
    public class PluginsUploadedEvent
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="uploadedPlugins">Uploaded plugins</param>
        public PluginsUploadedEvent(IList<PluginDescriptor> uploadedPlugins)
        {
            this.UploadedPlugins = uploadedPlugins;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Uploaded plugins
        /// </summary>
        public IList<PluginDescriptor> UploadedPlugins { get; private set; }

        #endregion
    }

    /// <summary>
    /// Represents the plugin updated event
    /// </summary>
    public class PluginUpdatedEvent
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="plugin">Updated plugin</param>
        public PluginUpdatedEvent(PluginDescriptor plugin)
        {
            this.Plugin = plugin;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Updated plugin
        /// </summary>
        public PluginDescriptor Plugin { get; private set; }

        #endregion
    }
}