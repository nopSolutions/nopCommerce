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
// Contributor(s): Jordan Van Gogh_______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Text;
using System.Threading;


namespace NopSolutions.NopCommerce.BusinessLogic.Data
{
    /// <summary>
    /// Maintains an ObjectContext instance in static field. This instance is then
    /// shared during the lifespan of the AppDomain.
    /// </summary>
    public sealed class StaticObjectContextManager<T> : ObjectContextManager<T> where T : ObjectContext, new()
    {
        private static T _objectContext;
        private static object _lockObject = new object();

        /// <summary>
        /// Returns a shared NorthwindObjectContext instance.
        /// </summary>
        public override T ObjectContext
        {
            get
            {
                lock (_lockObject)
                {
                    if (_objectContext == null)
                    {
                        string connectionString = ObjectContextHelper.GetEntityConnectionString();
                        _objectContext = Activator.CreateInstance(typeof(T), new object[] { connectionString }) as T;
                    }
                }

                return _objectContext;
            }
        }
    }
}
