using System;
using Newtonsoft.Json;

namespace Nop.Services.Plugins
{
    /// <summary>
    /// Represents base info of plugin descriptor
    /// </summary>
    public partial class PluginDescriptorBaseInfo: IComparable<PluginDescriptorBaseInfo>
    {
        /// <summary>
        /// Gets or sets the plugin system name
        /// </summary>
        [JsonProperty(PropertyName = "SystemName")]
        public virtual string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the version
        /// </summary>
        [JsonProperty(PropertyName = "Version")]
        public virtual string Version { get; set; }

        /// <summary>
        /// Compares this instance with a specified PluginDescriptorBaseInfo object
        /// </summary>
        /// <param name="other">The PluginDescriptorBaseInfo to compare with this instance</param>
        /// <returns>An integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the specified parameter</returns>
        public int CompareTo(PluginDescriptorBaseInfo other)
        {
            return string.Compare(SystemName, other.SystemName, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Determines whether this instance and another specified PluginDescriptor object have the same SystemName
        /// </summary>
        /// <param name="value">The PluginDescriptor to compare to this instance</param>
        /// <returns>True if the SystemName of the value parameter is the same as the SystemName of this instance; otherwise, false</returns>
        public override bool Equals(object value)
        {
            return SystemName?.Equals((value as PluginDescriptorBaseInfo)?.SystemName) ?? false;
        }

        /// <summary>
        /// Returns the hash code for this plugin descriptor
        /// </summary>
        /// <returns>A 32-bit signed integer hash code</returns>
        public override int GetHashCode()
        {
            return SystemName.GetHashCode();
        }

        /// <summary>
        /// Gets a copy of base info of plugin descriptor
        /// </summary>
        [JsonIgnore]
        public virtual PluginDescriptorBaseInfo GetBaseInfoCopy =>
            new()
            { SystemName = SystemName, Version = Version };
    }
}