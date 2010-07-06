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
using System.Data.EntityClient;
using System.Data.Objects;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Configuration;


namespace NopSolutions.NopCommerce.BusinessLogic.Data
{
    /// <summary>
    /// ObjectContextHelper classes. 
    /// </summary>
    public class ObjectContextHelper
    {
        private static ObjectContextManager<NopObjectContext> _objectContextManager;
        private static string _connectionString;

        /// <summary>
        /// Returns the current ObjectContextManager instance. Encapsulated the 
        /// _objectContextManager field to show it as an association on the class diagram.
        /// </summary>
        private static ObjectContextManager<NopObjectContext> ObjectContextManager
        {
            get { return _objectContextManager; }
            set { _objectContextManager = value; }
        }

        /// <summary>
        /// Gets an object context
        /// </summary>
        public static NopObjectContext CurrentObjectContext
        {
            get
            {
                if (ObjectContextManager == null)
                    InstantiateObjectContextManager();

                return ObjectContextManager.ObjectContext;
            }
        }

        /// <summary>
        /// Instantiates a new ObjectContextManager
        /// </summary>
        private static void InstantiateObjectContextManager()
        {
            //create ASP.NET now
            ObjectContextManager = new AspNetObjectContextManager<NopObjectContext>();
        }

        /// <summary>
        /// Gets a connection string
        /// </summary>
        /// <returns>Connection string</returns>
        public static string GetEntityConnectionString()
        {
            if (_connectionString == null)
            {
                var ecsbuilder = new EntityConnectionStringBuilder();
                ecsbuilder.Provider = "System.Data.SqlClient";
                ecsbuilder.ProviderConnectionString = NopConfig.ConnectionString;
                ecsbuilder.Metadata = @"res://*/Data.NopModel.csdl|res://*/Data.NopModel.ssdl|res://*/Data.NopModel.msl";
                _connectionString = ecsbuilder.ToString();
            }
            return _connectionString;
        }
    }
}
