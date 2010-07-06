//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web.Configuration;
using System.Xml;

namespace NopSolutions.NopCommerce.BusinessLogic.Configuration
{
    /// <summary>
    /// Represents a NopConfig
    /// </summary>
    public partial class NopConfig : IConfigurationSectionHandler
    {
        #region Fields
        private static string _connectionString = "";
        private static bool _initialized = false;
        private static int _cookieExpires = 128;
        private static XmlNode _scheduleTasks;
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
            XmlNode sqlServerNode = section.SelectSingleNode("SqlServer");
            if (sqlServerNode != null)
            {
                XmlAttribute attribute = sqlServerNode.Attributes["ConnectionStringName"];
                if ((attribute != null) && (WebConfigurationManager.ConnectionStrings[attribute.Value] != null))
                    _connectionString = WebConfigurationManager.ConnectionStrings[attribute.Value].ConnectionString;
            }
            
            _scheduleTasks = section.SelectSingleNode("ScheduleTasks");

            return null;
        }

        /// <summary>
        /// Initializes the NopConfig object
        /// </summary>
        public static void Init()
        {
            if (!_initialized)
            {
                ConfigurationManager.GetSection("NopConfig");
                _initialized = true;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the connection string that is used to connect to the storage
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        /// <summary>
        /// Gets or sets the expiration date and time for the Cookie in hours
        /// </summary>
        public static int CookieExpires
        {
            get
            {
                return _cookieExpires;
            }
            set
            {
                _cookieExpires = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a schedule tasks section
        /// </summary>
        public static XmlNode ScheduleTasks
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
        #endregion
    }
}
