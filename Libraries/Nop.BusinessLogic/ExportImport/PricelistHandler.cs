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
using System.Linq;
using System.Text;

using System.Web;

using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.ExportImport
{
    /// <summary>
    /// Pricelist handler
    /// </summary>
    public partial class PricelistHandler : IHttpHandler
    {
        #region IHttpHandler Member

        /// <summary>
        /// Gets a value indicating whether another request can use the IHttpHandler instance
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the IHttpHandler interface.
        /// </summary>
        /// <param name="context">An HttpContext object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            if (PricelistGuid.Length == 0)
            {
                context.Response.Write("No pricelist specified, please use PricelistGuid parameter");
                return;
            }

            Pricelist pl = null;

            if (PricelistGuid.Length != 0)
                pl = ProductManager.GetPricelistByGuid(PricelistGuid);

            else
                throw new MissingFieldException();

            if (pl == null)
            {
                context.Response.Write("no pricelist found");
                return;
            }

            string cachePath = string.Format("{0}files\\ExportImport\\", HttpContext.Current.Request.PhysicalApplicationPath);
            context.Response.Write(pl.CreatePricelist(cachePath));
        }

        /// <summary>
        /// Pricelist GUID
        /// </summary>
        public string PricelistGuid
        {
            get
            {
                return CommonHelper.QueryString("PricelistGuid");
            }
        }

        #endregion
    }
}
