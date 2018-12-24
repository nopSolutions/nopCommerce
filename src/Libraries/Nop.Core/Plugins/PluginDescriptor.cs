using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Represents a plugin descriptor
    /// </summary>
    public class PluginDescriptor : IDescriptor, IComparable<PluginDescriptor>
    {
        #region Fields

        private static readonly INopFileProvider _fileProvider;

        #endregion

        #region Ctor

        public PluginDescriptor()
        {
            this.SupportedVersions = new List<string>();
            this.LimitedToStores = new List<int>();
            this.LimitedToCustomerRoles = new List<int>();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="referencedAssembly">Referenced assembly</param>
        public PluginDescriptor(Assembly referencedAssembly) : this()
        {
            this.ReferencedAssembly = referencedAssembly;
        }

        static PluginDescriptor()
        {
            //we use the default file provider, since the DI isn't initialized yet
            _fileProvider = CommonHelper.DefaultFileProvider;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the instance of the plugin
        /// </summary>
        /// <returns>Plugin instance</returns>
        public IPlugin Instance()
        {
            return Instance<IPlugin>();
        }

        /// <summary>
        /// Get the instance of the plugin
        /// </summary>
        /// <typeparam name="T">Type of the plugin</typeparam>
        /// <returns>Plugin instance</returns>
        public virtual T Instance<T>() where T : class, IPlugin
        {
            object instance = null;
            try
            {
                instance = EngineContext.Current.Resolve(PluginType);
            }
            catch
            {
                //try resolve
            }

            if (instance == null)
            {
                //not resolved
                instance = EngineContext.Current.ResolveUnregistered(PluginType);
            }

            var typedInstance = instance as T;
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

            return string.Compare(FriendlyName, other.FriendlyName, StringComparison.InvariantCultureIgnoreCase);
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
        /// Determines whether this instance and another specified PluginDescriptor object have the same SystemName
        /// </summary>
        /// <param name="value">The PluginDescriptor to compare to this instance</param>
        /// <returns>True if the SystemName of the value parameter is the same as the SystemName of this instance; otherwise, false</returns>
        public override bool Equals(object value)
        {
            return SystemName?.Equals((value as PluginDescriptor)?.SystemName) ?? false;
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
        /// Delete plugin directory from disk storage
        /// </summary>
        /// <returns>True if plugin directory is deleted, false if not</returns>
        public bool DeletePlugin()
        {
           //check whether plugin is installed
            if (Installed)
                return false;

            var directoryName = _fileProvider.GetDirectoryName(OriginalAssemblyFile);

            if (_fileProvider.DirectoryExists(directoryName))
                _fileProvider.DeleteDirectory(directoryName);

            return true;
        }

        /// <summary>
        /// Save plugin descriptor to the plugin description file
        /// </summary>
        public void Save()
        {
            //get the description file path
            if (OriginalAssemblyFile == null)
                throw new Exception($"Cannot load original assembly path for {SystemName} plugin.");

            var filePath = _fileProvider.Combine(_fileProvider.GetDirectoryName(OriginalAssemblyFile), NopPluginDefaults.DescriptionFileName);
            if (!_fileProvider.FileExists(filePath))
                throw new Exception($"Description file for {SystemName} plugin does not exist. {filePath}");

            //save the file
            var text = JsonConvert.SerializeObject(this, Formatting.Indented);
            _fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
        }

        /// <summary>
        /// Get plugin logo URL
        /// </summary>
        /// <returns>Logo URL</returns>
        public string GetLogoUrl()
        {
            var pluginDirectory = _fileProvider.GetDirectoryName(OriginalAssemblyFile);
            if (string.IsNullOrEmpty(pluginDirectory))
                return null;

            var logoExtension = NopPluginDefaults.SupportedLogoImageExtensions
                .FirstOrDefault(ext => _fileProvider.FileExists(_fileProvider.Combine(pluginDirectory, $"{NopPluginDefaults.LogoFileName}.{ext}")));
            if (string.IsNullOrWhiteSpace(logoExtension))
                return null; //No logo file was found with any of the supported extensions.

            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            var logoUrl = $"{webHelper.GetStoreLocation()}plugins/{_fileProvider.GetDirectoryNameOnly(pluginDirectory)}/{NopPluginDefaults.LogoFileName}.{logoExtension}";
            return logoUrl;
        }

        /// <summary>
        /// Get plugin descriptor from the plugin description file
        /// </summary>
        /// <param name="filePath">Path to the description file</param>
        /// <returns>Plugin descriptor</returns>
        public static PluginDescriptor GetPluginDescriptorFromFile(string filePath)
        {
            var text = _fileProvider.ReadAllText(filePath, Encoding.UTF8);

            return GetPluginDescriptorFromText(text);
        }

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
        public virtual string OriginalAssemblyFile { get; internal set; }

        /// <summary>
        /// Gets or sets the assembly that has been shadow copied that is active in the application
        /// </summary>
        [JsonIgnore]
        public virtual Assembly ReferencedAssembly { get; internal set; }

        /// <summary>
        /// Gets or sets the value indicating whether need to show the plugin on plugins page
        /// </summary>
        [JsonIgnore]
        public virtual bool ShowInPluginsList { get; set; } = true;

        /// <summary>
        /// Gets or sets the last error
        /// </summary>
        [JsonIgnore]
        public virtual Exception Error { get; set; }

        #endregion
    }
}