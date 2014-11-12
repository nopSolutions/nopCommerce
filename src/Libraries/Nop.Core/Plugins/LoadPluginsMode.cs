namespace Nop.Core.Plugins
{
    /// <summary>
    /// Represents a mode to load plugins
    /// </summary>
    public enum LoadPluginsMode
    {
        /// <summary>
        /// All (Installed & Not installed)
        /// </summary>
        All = 0,
        /// <summary>
        /// Radio list
        /// </summary>
        InstalledOnly = 10,
        /// <summary>
        /// Checkboxes
        /// </summary>
        NotInstalledOnly = 20
    }
}