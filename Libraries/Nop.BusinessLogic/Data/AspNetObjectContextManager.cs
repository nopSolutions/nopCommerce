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
using System.Web;
using System.Diagnostics;


namespace NopSolutions.NopCommerce.BusinessLogic.Data
{
    /// <summary>
    /// Creates one ObjectContext instance per HTTP request. This instance is then
    /// shared during the lifespan of the HTTP request.
    /// </summary>
    public sealed class AspNetObjectContextManager<T> : ObjectContextManager<T> where T : ObjectContext, new()
    {
        private object _lockObject;

        /// <summary>
        /// Returns a shared ObjectContext instance.
        /// </summary>
        public override T ObjectContext
        {
            get
            {
                string connectionString = ObjectContextHelper.GetEntityConnectionString();
                if (HttpContext.Current == null)
                {
                    T context = Activator.CreateInstance(typeof(T), new object[] { connectionString }) as T;
                    return context;
                }

                string ocKey = "nopocm_" + HttpContext.Current.GetHashCode().ToString("x");

                //lock (_lockObject)
                {
                    if (!HttpContext.Current.Items.Contains(ocKey))
                    {
                        T context = Activator.CreateInstance(typeof(T), new object[] { connectionString }) as T;
                        HttpContext.Current.Items.Add(ocKey, context);
                        Debug.WriteLine("AspNetObjectContextManager: Created new ObjectContext");
                    }
                }
                return HttpContext.Current.Items[ocKey] as T;
            }
        }
        
        /// <summary>
        /// Ctor
        /// </summary>
        public AspNetObjectContextManager()
        {
            _lockObject = new object();
        }
    }
}
