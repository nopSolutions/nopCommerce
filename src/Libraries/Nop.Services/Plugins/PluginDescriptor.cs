using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Services.Plugins
{
    /// <summary>
    /// Represents a plugin descriptor
    /// </summary>
    public partial class PluginDescriptor : PluginDescriptorBaseInfo, IDescriptor, IComparable<PluginDescriptor>
    {
        #region Ctor

        public PluginDescriptor()
        {
            SupportedVersions = new List<string>();
            LimitedToStores = new List<int>();
            LimitedToCustomerRoles = new List<int>();
            DependsOn = new List<string>();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="referencedAssembly">Referenced assembly</param>
        public PluginDescriptor(Assembly referencedAssembly) : this()
        {
            ReferencedAssembly = referencedAssembly;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get plugin descriptor from the description text
        /// </summary>
        /// <param name="text">Description text</param>
        /// <returns>Plugin descriptor</returns>
        public static PluginDescriptor GetPluginDescriptorFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new PluginDescriptor();

            //get plugin descriptor from the JSON file
            var descriptor = JsonConvert.DeserializeObject<PluginDescriptor>(text);

            //nopCommerce 2.00 didn't have 'SupportedVersions' parameter, so let's set it to "2.00"
            if (!descriptor.SupportedVersions.Any())
                descriptor.SupportedVersions.Add("2.00");

            return descriptor;
        }

        /// <summary>
        /// Get the instance of the plugin
        /// </summary>
        /// <typeparam name="TPlugin">Type of the plugin</typeparam>
        /// <returns>Plugin instance</returns>
        public virtual TPlugin Instance<TPlugin>() where TPlugin : class, IPlugin
        {
            //try to resolve plugin as unregistered service
            var instance = EngineContext.Current.ResolveUnregistered(PluginType);

            //try to get typed instance
            var typedInstance = instance as TPlugin;
            if (typedInstance != null)
                typedInstance.PluginDescriptor = this;

            return typedInstance;
        }

        /// <summary>
        /// Compares this instance with a specified PluginDescriptor object
        /// </summary>
        /// <param name="other">The PluginDescriptor to compare with this instance</param>
        /// <returns>An integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the specified parameter</returns>
        public int CompareTo(PluginDescriptor other)
        {
            if (DisplayOrder != other.DisplayOrder)
                return DisplayOrder.CompareTo(other.DisplayOrder);

            return string.Compare(SystemName, other.SystemName, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns the plugin as a string
        /// </summary>
        /// <returns>Value of the FriendlyName</returns>
        public override string ToString()
        {
            return FriendlyName;
        }

        /// <summary>
        /// Save plugin descriptor to the plugin description file
        /// </summary>
        public virtual void Save()
        {
            //since plugins are loaded before IoC initialization using the default provider,
            //in order to avoid possible problems we use CommonHelper.DefaultFileProvider
            //instead of the main file provider
            var fileProvider = CommonHelper.DefaultFileProvider;

            //get the description file path
            if (OriginalAssemblyFile == null)
                throw new Exception($"Cannot load original assembly path for {SystemName} plugin.");

            var filePath = fileProvider.Combine(fileProvider.GetDirectoryName(OriginalAssemblyFile), NopPluginDefaults.DescriptionFileName);
            if (!fileProvider.FileExists(filePath))
                throw new Exception($"Description file for {SystemName} plugin does not exist. {filePath}");

            //save the file
            var text = JsonConvert.SerializeObject(this, Formatting.Indented);
            fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the plugin group
        /// </summary>
        [JsonProperty(PropertyName = "Group")]
        public virtual string Group { get; set; }

        /// <summary>
        /// Gets or sets the plugin friendly name
        /// </summary>
        [JsonProperty(PropertyName = "FriendlyName")]
        public virtual string FriendlyName { get; set; }
        
        /// <summary>
        /// Gets or sets the supported versions of nopCommerce
        /// </summary>
        [JsonProperty(PropertyName = "SupportedVersions")]
        public virtual IList<string> SupportedVersions { get; set; }

        /// <summary>
        /// Gets or sets the author
        /// </summary>
        [JsonProperty(PropertyName = "Author")]
        public virtual string Author { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [JsonProperty(PropertyName = "DisplayOrder")]
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the name of the assembly file
        /// </summary>
        [JsonProperty(PropertyName = "FileName")]
        public virtual string AssemblyFileName { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [JsonProperty(PropertyName = "Description")]
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the list of store identifiers in which this plugin is available. If empty, then this plugin is available in all stores
        /// </summary>
        [JsonProperty(PropertyName = "LimitedToStores")]
        public virtual IList<int> LimitedToStores { get; set; }

        /// <summary>
        /// Gets or sets the list of customer role identifiers for which this plugin is available. If empty, then this plugin is available for all ones.
        /// </summary>
        [JsonProperty(PropertyName = "LimitedToCustomerRoles")]
        public virtual IList<int> LimitedToCustomerRoles { get; set; }
        
        /// <summary>
        /// Gets or sets the list of plugins' system name that this plugin depends on
        /// </summary>
        [JsonProperty(PropertyName = "DependsOnSystemNames")]
        public virtual IList<string> DependsOn { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether plugin is installed
        /// </summary>
        [JsonIgnore]
        public virtual bool Installed { get; set; }

        /// <summary>
        /// Gets or sets the plugin type
        /// </summary>
        [JsonIgnore]
        public virtual Type PluginType { get; set; }

        /// <summary>
        /// Gets or sets the original assembly file that a shadow copy was made from it
        /// </summary>
        [JsonIgnore]
        public virtual string OriginalAssemblyFile { get; set; }

        /// <summary>
        /// Gets or sets the assembly that has been shadow copied that is active in the application
        /// </summary>
        [JsonIgnore]
        public virtual Assembly ReferencedAssembly { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether need to show the plugin on plugins page
        /// </summary>
        [JsonIgnore]
        public virtual bool ShowInPluginsList { get; set; } = true;

        #endregion
    }
}