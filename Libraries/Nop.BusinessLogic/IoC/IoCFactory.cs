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
using System.Diagnostics;
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
    /// Inversion of Control factory implementation.
    /// This is a simple factory built with Microsoft Unity    
    /// </summary>
    public static class IoCFactory
    {
        #region Fields

        private static IDependencyResolver _resolver;

        #endregion

        #region Methods

        public static void InitializeWith(IDependencyResolverFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            _resolver = factory.CreateInstance();
        }

        public static void Register<T>(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            _resolver.Register(instance);
        }

        public static void Inject<T>(T existing)
        {
            if (existing == null)
                throw new ArgumentNullException("existing");

            _resolver.Inject(existing);
        }

        public static T Resolve<T>(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return _resolver.Resolve<T>(type);
        }

        public static T Resolve<T>(Type type, string name)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (name == null)
                throw new ArgumentNullException("name");

            return _resolver.Resolve<T>(type, name);
        }

        public static T Resolve<T>()
        {
            return _resolver.Resolve<T>();
        }

        public static T Resolve<T>(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return _resolver.Resolve<T>(name);
        }

        public static IEnumerable<T> ResolveAll<T>()
        {
            return _resolver.ResolveAll<T>();
        }
        
        #endregion
    }
}
