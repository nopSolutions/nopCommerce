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
        #region Fields

        //private string _connectionString = "";
        //private int _cookieExpires = 128;
        private bool _dynamicDiscovery;
        private string _engineType;
        private XmlNode _scheduleTasks;
        private string _themeBasePath;

        #endregion

        #region Methods

        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            NopConfig config = new NopConfig();
            //var sqlServerNode = section.SelectSingleNode("SqlServer");
            //if (sqlServerNode != null)
            //{
            //    XmlAttribute attribute = sqlServerNode.Attributes["ConnectionStringName"];
            //    if ((attribute != null) && (WebConfigurationManager.ConnectionStrings[attribute.Value] != null))
            //        config.ConnectionString = WebConfigurationManager.ConnectionStrings[attribute.Value].ConnectionString;
            //}

            var dynamicDiscoveryNode = section.SelectSingleNode("DynamicDiscovery");
            if (dynamicDiscoveryNode != null)
            {
                var attribute = dynamicDiscoveryNode.Attributes["Enabled"];
                if (attribute != null)
                    config.DynamicDiscovery = Convert.ToBoolean(attribute.Value);
            }

            var engineNode = section.SelectSingleNode("Engine");
            if (engineNode != null)
            {
                var attribute = engineNode.Attributes["Type"];
                if (attribute != null)
                    config.EngineType = attribute.Value;
            }

            var themeNode = section.SelectSingleNode("Themes");
            if (themeNode != null)
            {
                var attribute = themeNode.Attributes["basePath"];
                if (attribute != null)
                    config.ThemeBasePath = attribute.Value;
            }

            config.ScheduleTasks = section.SelectSingleNode("ScheduleTasks");

            return config;
        }
        
            #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the connection string that is used to connect to the storage
        /// </summary>
        //public string ConnectionString
        //{
        //    get
        //    {
        //        return _connectionString;
        //    }
        //    set
        //    {
        //        _connectionString = value;
        //    }
        //}

        /// <summary>
        /// Gets or sets the expiration date and time for the Cookie in hours
        /// </summary>
        //public int CookieExpires
        //{
        //    get
        //    {
        //        return _cookieExpires;
        //    }
        //    set
        //    {
        //        _cookieExpires = value;
        //    }
        //}

        /// <summary>
        /// In addition to configured assemblies examine and load assemblies in the bin directory.
        /// </summary>
        public bool DynamicDiscovery
        {
            get
            {
                return _dynamicDiscovery;
            }
            set
            {
                _dynamicDiscovery = value;
            }
        }

        /// <summary>
        /// A custom <see cref="IEngine"/> to manage the application instead of the default.
        /// </summary>
        public string EngineType
        {
            get
            {
                return _engineType;
            }
            set
            {
                _engineType = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a schedule tasks section
        /// </summary>
        public XmlNode ScheduleTasks
        {
            get
            {
                return _scheduleTasks;
            }
            set
            {
                _scheduleTasks = value;
            }
        }

        /// <summary>
        /// Specifices where the themes will be stored (~/Themes/)
        /// </summary>
        public string ThemeBasePath
        {
            get
            {
                return _themeBasePath;
            }
            set
            {
                _themeBasePath = value;
            }
        }

        #endregion
    }
}
