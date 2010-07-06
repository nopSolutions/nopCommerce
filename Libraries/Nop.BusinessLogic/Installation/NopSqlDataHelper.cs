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
using System.Data;
using System.Text;
using System.Web.Configuration;
using System.Configuration;
using System.Data.SqlClient;

namespace NopSolutions.NopCommerce.BusinessLogic.Installation
{
    /// <summary>
    /// Data helper class
    /// </summary>
    public partial class NopSqlDataHelper
    {
        #region Methods

        internal static string GetConnectionString(string ConnectionStringName)
        {
            string connectionString = null;

            ConnectionStringSettings settings = WebConfigurationManager.ConnectionStrings[ConnectionStringName];
            if (settings != null)
            {
                connectionString = settings.ConnectionString;
            }

            return connectionString;
        }

        /// <summary>
        /// Gets connection string to master database
        /// </summary>
        /// <param name="connetionString">A connection string</param>
        /// <returns></returns>
        public static string GetMasterConnectionString(string connetionString)
        {
            var builder = new SqlConnectionStringBuilder(connetionString);
            builder.InitialCatalog = "master";
            return builder.ToString();
        }

        /// <summary>
        /// Gets database name from connection string
        /// </summary>
        /// <param name="connetionString">A connection string</param>
        /// <returns></returns>
        public static string GetDatabaseName(string connetionString)
        {
            var builder = new SqlConnectionStringBuilder(connetionString);
            return builder.InitialCatalog;
        }
        
        #endregion
    }
}
