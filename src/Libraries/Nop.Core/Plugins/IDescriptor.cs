namespace Nop.Core.Plugins
{
    /// <summary>
    /// Represents descriptor of the application extension (plugin or theme)
    /// </summary>
    public interface IDescriptor
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