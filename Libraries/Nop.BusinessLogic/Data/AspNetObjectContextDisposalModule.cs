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
using System.Web;


namespace NopSolutions.NopCommerce.BusinessLogic.Data
{

    /// <summary>
    /// Disposes an ObjectContext created by an AspNetObjectContextManager instance.
    /// </summary>
    public class AspNetObjectContextDisposalModule : IHttpModule
    {
        /// <summary>
        /// Initializes this HTTP module.
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.EndRequest += new EventHandler(EndOfHttpRequest);
        }

        /// <summary>
        /// Releases any resources held by this module. 
        /// </summary>
        public void Dispose()
        {
            /* No resources held... */
        }

        /// <summary>
        /// Is invoked at the end of a HTTP request. Disposes the shared ObjectContext instance. 
        /// </summary>
        private void EndOfHttpRequest(object sender, EventArgs e)
        {
            DisposeObjectContext();
        }

        /// <summary>
        /// Disposes the shared ObjectContext instance.
        /// </summary>
        public static void DisposeObjectContext()
        {
            if (HttpContext.Current == null)
                return;

            string ocKey = "nopocm_" + HttpContext.Current.GetHashCode().ToString("x");
            if (HttpContext.Current.Items.Contains(ocKey))
            {
                ObjectContext objectContext = HttpContext.Current.Items[ocKey] as ObjectContext;
                if (objectContext != null)
                    objectContext.Dispose();
                HttpContext.Current.Items.Remove(ocKey);

                System.Diagnostics.Debug.WriteLine("AspNetObjectContextManager: Disposed ObjectContext");
            }
        }
    }
}
