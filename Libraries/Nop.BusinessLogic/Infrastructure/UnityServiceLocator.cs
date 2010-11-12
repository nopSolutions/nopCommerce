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
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace NopSolutions.NopCommerce.BusinessLogic.Infrastructure
{
    /// <summary>
    /// Unity implemenation of the <see cref="ServiceLocatorImplBase" />
    /// </summary>
    public class UnityServiceLocator : ServiceLocatorImplBase
    {
        readonly IUnityContainer unityContainer;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="unityContainer">Unity container</param>
        public UnityServiceLocator(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            return unityContainer.Resolve(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return unityContainer.ResolveAll(serviceType);
        }
    }
}
