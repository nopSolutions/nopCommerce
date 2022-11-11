namespace Nop.Services.Plugins
{
    /// <summary>
    /// Represents the plugin updated event
    /// </summary>
    public partial class PluginUpdatedEvent
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="plugin">Updated plugin</param>
        public PluginUpdatedEvent(PluginDescriptor plugin)
        {
            Plugin = plugin;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Updated plugin
        /// </summary>
        public PluginDescriptor Plugin { get; }

        #endregion
    }
}