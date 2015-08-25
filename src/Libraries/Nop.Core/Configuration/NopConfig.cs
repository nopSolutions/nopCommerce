using System;
using System.Configuration;
using System.Xml;
using Nop.Core.Infrastructure;

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
            var dynamicDiscoveryNode = section.SelectSingleNode("DynamicDiscovery");
            if (dynamicDiscoveryNode != null && dynamicDiscoveryNode.Attributes != null)
            {
                var attribute = dynamicDiscoveryNode.Attributes["Enabled"];
                if (attribute != null)
                    config.DynamicDiscovery = Convert.ToBoolean(attribute.Value);
            }

            var engineNode = section.SelectSingleNode("Engine");
            if (engineNode != null && engineNode.Attributes != null)
            {
                var attribute = engineNode.Attributes["Type"];
                if (attribute != null)
                    config.EngineType = attribute.Value;
            }

            var startupNode = section.SelectSingleNode("Startup");
            if (startupNode != null && startupNode.Attributes != null)
            {
                var attribute = startupNode.Attributes["IgnoreStartupTasks"];
                if (attribute != null)
                    config.IgnoreStartupTasks = Convert.ToBoolean(attribute.Value);
            }

            var themeNode = section.SelectSingleNode("Themes");
            if (themeNode != null && themeNode.Attributes != null)
            {
                var attribute = themeNode.Attributes["basePath"];
                if (attribute != null)
                    config.ThemeBasePath = attribute.Value;
            }

            var userAgentStringsNode = section.SelectSingleNode("UserAgentStrings");
            if (userAgentStringsNode != null && userAgentStringsNode.Attributes != null)
            {
                var attribute = userAgentStringsNode.Attributes["databasePath"];
                if (attribute != null)
                    config.UserAgentStringsPath = attribute.Value;
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

            return config;
        }
        
        /// <summary>
        /// In addition to configured assemblies examine and load assemblies in the bin directory.
        /// </summary>
        public bool DynamicDiscovery { get; private set; }

        /// <summary>
        /// A custom <see cref="IEngine"/> to manage the application instead of the default.
        /// </summary>
        public string EngineType { get; private set; }

        /// <summary>
        /// Specifices where the themes will be stored (~/Themes/)
        /// </summary>
        public string ThemeBasePath { get; private set; }

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
    }
}
