namespace Nop.Services.Plugins
{
    /// <summary>
    /// Represents descriptor of the application extension (plugin or theme)
    /// </summary>
    public partial interface IDescriptor
    {
        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
         string FriendlyName { get; set; }
    }
}