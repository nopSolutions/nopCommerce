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
// Contributor(s): Orchard Project_______. 
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Autofac;

namespace Nop.Web.Framework.IoC
{
    public class AutofacDependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public AutofacDependencyResolver(IContainer container)
        {
            this._container = container;
        }


        static bool TryResolveAtScope(ILifetimeScope scope, Type serviceType, out object value)
        {
            if (scope == null)
            {
                value = null;
                return false;
            }
            return scope.TryResolve(serviceType, out value);
        }

        bool TryResolve(Type serviceType, out object value)
        {
            return TryResolveAtScope(_container, serviceType, out value);
        }

        static object CreateInstance(Type t)
        {
            if (t.IsAbstract || t.IsInterface)
                return null;

            return Activator.CreateInstance(t);
        }

        TService Resolve<TService>(Type serviceType, TService defaultValue = default(TService))
        {
            object value;
            return TryResolve(serviceType, out value) ? (TService)value : defaultValue;
        }

        TService Resolve<TService>(Type serviceType, string key, TService defaultValue = default(TService))
        {
            object value;
            return TryResolve(serviceType, out value) ? (TService)value : defaultValue;
        }

        TService Resolve<TService>(Type serviceType, Func<Type, TService> defaultFactory)
        {
            object value;
            return TryResolve(serviceType, out value) ? (TService)value : defaultFactory(serviceType);
        }

        TService Resolve<TService>(Type serviceType, string key, Func<Type, TService> defaultFactory)
        {
            object value;
            return TryResolve(serviceType, out value) ? (TService)value : defaultFactory(serviceType);
        }

        TService Resolve<TService>()
        {
            // Resolve service, or null
            return Resolve(typeof(TService), default(TService));
        }

        object IDependencyResolver.GetService(Type serviceType)
        {
            // Resolve service, or null
            return Resolve(serviceType, default(object));
        }

        IEnumerable<object> IDependencyResolver.GetServices(Type serviceType)
        {
            return Resolve<IEnumerable>(typeof(IEnumerable<>).MakeGenericType(serviceType), Enumerable.Empty<object>()).Cast<object>();
        }
    }
}
