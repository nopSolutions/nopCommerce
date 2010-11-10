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
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Installation;
using NopSolutions.NopCommerce.BusinessLogic.IoC;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Security
{
    public partial class BlacklistHttpModule : IHttpModule
    {
        #region Constructor
        /// <summary>
        /// Creates a new instance of BlacklistHttpModule
        /// </summary>
        public BlacklistHttpModule() { }
        #endregion

        #region IHttpModule Members

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(Context_BeginRequest);
        }

        #endregion

        #region Handlers

        /// <summary>
        /// Event handler for BeginRequest.
        /// </summary>
        /// <param name="sender">Sender object instance.</param>
        /// <param name="e">Event arguments.</param>
        void Context_BeginRequest(object sender, EventArgs e)
        {
            if (InstallerHelper.ConnectionStringIsSet())
            {
                try
                {
                    //exit if a request for a .net mapping that isn't a content page is made i.e. axd
                    if (!CommonHelper.IsContentPageRequested())
                        return;
                    //exit if a request for a .net mapping that isn't a content page is made i.e. axd
                    if (!CommonHelper.IsContentPageRequested())
                        return;

                    if (HttpContext.Current != null && !HttpContext.Current.Request.Url.IsLoopback)
                    {
                        HttpApplication application = sender as HttpApplication;
                        var clientIP = new BannedIpAddress();
                        clientIP.Address = application.Request.UserHostAddress;
                        // On any unexpected error we let visitor to visit website
                        if (IoCFactory.Resolve<IBlacklistService>().IsIpAddressBanned(clientIP))
                        {
                            // Blocking process

                            // for now just show error 404 - Forbidden
                            // later let the user know that his ip address/network 
                            // was banned and a reason why... this means we need an error page (aspx)
                            application.Response.StatusCode = 403;
                            application.Server.Transfer("~/BannedAddress.htm");
                            application.Response.StatusDescription = "Access is denied";
                            application.Response.End();
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion

    }
}
