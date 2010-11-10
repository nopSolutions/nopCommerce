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
using System.Data.EntityClient;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Xml;

namespace NopSolutions.NopCommerce.BusinessLogic.IoC
{
    /// <summary>
    /// Dependency resolver factory
    /// </summary>
    public class DependencyResolverFactory : IDependencyResolverFactory
    {
        /// <summary>
        /// Resolver type
        /// </summary>
        private readonly Type _resolverType;


        /// <summary>
        /// Ctor
        /// </summary>
        public DependencyResolverFactory()
            : this(ConfigurationManager.AppSettings["dependencyResolverTypeName"])
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="resolverTypeName">Resolver type name</param>
        public DependencyResolverFactory(string resolverTypeName)
        {
            if (String.IsNullOrEmpty(resolverTypeName))
                throw new ArgumentNullException("resolverTypeName");

            _resolverType = Type.GetType(resolverTypeName, true, true);
        }

        /// <summary>
        /// Create dependency resolver
        /// </summary>
        /// <returns>Dependency resolver</returns>
        public IDependencyResolver CreateInstance()
        {
            return Activator.CreateInstance(_resolverType) as IDependencyResolver;
        }
    }
}
