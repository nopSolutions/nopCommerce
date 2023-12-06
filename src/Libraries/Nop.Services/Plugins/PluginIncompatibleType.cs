namespace Nop.Services.Plugins
{
    public enum PluginIncompatibleType
    {
        /// <summary>
        /// The plugin isn't compatible with the current version
        /// </summary>
        NotCompatibleWithCurrentVersion,
        /// <summary>
        /// The main assembly isn't found
        /// </summary>
        MainAssemblyNotFound,
        /// <summary>
        /// Another version of the same library is already loaded in memory
        /// </summary>
        HasCollisions
    }
}
