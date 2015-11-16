using System;
using System.Configuration;
using System.Xml;

namespace Nop.Core.Configuration
{
    /// <summary>
    /// Represents a NopConfig
    /// </summary>
    public partial class NopConfig : IConfigurationSectionHandler
    {
        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new NopConfig();

            var startupNode = section.SelectSingleNode("Startup");
            if (startupNode != null && startupNode.Attributes != null)
            {
                var attribute = startupNode.Attributes["IgnoreStartupTasks"];
                if (attribute != null)
                    config.IgnoreStartupTasks = Convert.ToBoolean(attribute.Value);
            }

            var redisCachingNode = section.SelectSingleNode("RedisCaching");
            if (redisCachingNode != null && redisCachingNode.Attributes != null)
            {
                var enabledAttribute = redisCachingNode.Attributes["Enabled"];
                if (enabledAttribute != null)
                    config.RedisCachingEnabled = Convert.ToBoolean(enabledAttribute.Value);

                var connectionStringAttribute = redisCachingNode.Attributes["ConnectionString"];
                if (connectionStringAttribute != null)
                    config.RedisCachingConnectionString = connectionStringAttribute.Value;
            }

            var userAgentStringsNode = section.SelectSingleNode("UserAgentStrings");
            if (userAgentStringsNode != null && userAgentStringsNode.Attributes != null)
            {
                var attribute = userAgentStringsNode.Attributes["databasePath"];
                if (attribute != null)
                    config.UserAgentStringsPath = attribute.Value;
            }

            var supportPreviousNopcommerceVersionsNode = section.SelectSingleNode("SupportPreviousNopcommerceVersions");
            if (supportPreviousNopcommerceVersionsNode != null && supportPreviousNopcommerceVersionsNode.Attributes != null)
            {
                var attribute = supportPreviousNopcommerceVersionsNode.Attributes["Enabled"];
                if (attribute != null)
                    config.SupportPreviousNopcommerceVersions = Convert.ToBoolean(attribute.Value);
            }

            var webFarmsNode = section.SelectSingleNode("WebFarms");
            if (webFarmsNode != null && webFarmsNode.Attributes != null)
            {
                var multipleInstancesEnabledAttribute = webFarmsNode.Attributes["MultipleInstancesEnabled"];
                if (multipleInstancesEnabledAttribute != null)
                    config.MultipleInstancesEnabled = Convert.ToBoolean(multipleInstancesEnabledAttribute.Value);

                var runOnAzureWebsitesAttribute = webFarmsNode.Attributes["RunOnAzureWebsites"];
                if (runOnAzureWebsitesAttribute != null)
                    config.RunOnAzureWebsites = Convert.ToBoolean(runOnAzureWebsitesAttribute.Value);
            }

            var installationNode = section.SelectSingleNode("Installation");
            if (installationNode != null && installationNode.Attributes != null)
            {
                var disableSampleDataDuringInstallationAttribute = installationNode.Attributes["DisableSampleDataDuringInstallation"];
                if (disableSampleDataDuringInstallationAttribute != null)
                    config.DisableSampleDataDuringInstallation = Convert.ToBoolean(disableSampleDataDuringInstallationAttribute.Value);

                var useFastInstallationServiceAttribute = installationNode.Attributes["UseFastInstallationService"];
                if (useFastInstallationServiceAttribute != null)
                    config.UseFastInstallationService = Convert.ToBoolean(useFastInstallationServiceAttribute.Value);

                var pluginsIgnoredDuringInstallationAttribute = installationNode.Attributes["PluginsIgnoredDuringInstallation"];
                if (pluginsIgnoredDuringInstallationAttribute != null)
                    config.PluginsIgnoredDuringInstallation = pluginsIgnoredDuringInstallationAttribute.Value;
            }

            return config;
        }
        
        /// <summary>
        /// Indicates whether we should ignore startup tasks
        /// </summary>
        public bool IgnoreStartupTasks { get; private set; }

        /// <summary>
        /// Path to database with user agent strings
        /// </summary>
        public string UserAgentStringsPath { get; private set; }

        /// <summary>
        /// Indicates whether we should use Redis server for caching (instead of default in-memory caching)
        /// </summary>
        public bool RedisCachingEnabled { get; private set; }
        /// <summary>
        /// Redis connection string. Used when Redis caching is enabled
        /// </summary>
        public string RedisCachingConnectionString { get; private set; }

        /// <summary>
        /// Indicates whether we should support previous nopCommerce versions (it can slightly improve performance)
        /// </summary>
        public bool SupportPreviousNopcommerceVersions { get; private set; }

        /// <summary>
        /// A value indicating whether the site is run on multiple instances (e.g. web farm, Windows Azure with multiple instances, etc).
        /// Do not enable it if you run on Azure but use one instance only
        /// </summary>
        public bool MultipleInstancesEnabled { get; private set; }

        /// <summary>
        /// A value indicating whether the site is run on Windows Azure Websites
        /// </summary>
        public bool RunOnAzureWebsites { get; private set; }

        /// <summary>
        /// A value indicating whether a store owner can install sample data during installation
        /// </summary>
        public bool DisableSampleDataDuringInstallation { get; private set; }
        /// <summary>
        /// By default this setting should always be set to "False" (only for advanced users)
        /// </summary>
        public bool UseFastInstallationService { get; private set; }
        /// <summary>
        /// A list of plugins ignored during nopCommerce installation
        /// </summary>
        public string PluginsIgnoredDuringInstallation { get; private set; }
    }
}
