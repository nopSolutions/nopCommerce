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
    /// Dependency resolver
    /// </summary>
    public interface IDependencyResolver
    {
        void Register<T>(T instance);

        void Inject<T>(T existing);

        T Resolve<T>(Type type);

        T Resolve<T>(Type type, string name);

        T Resolve<T>();

        T Resolve<T>(string name);

        IEnumerable<T> ResolveAll<T>();
    }
}
