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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Security;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class BlacklistControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            var ipAddressCollection = IpBlacklistManager.GetBannedIpAddressAll();
            gvBannedIpAddress.DataSource = ipAddressCollection;
            gvBannedIpAddress.DataBind();

            var ipNetworkCollection = IpBlacklistManager.GetBannedIpNetworkAll();
            gvBannedIpNetwork.DataSource = ipNetworkCollection;
            gvBannedIpNetwork.DataBind();
        }

        protected void btnAddBannedIP_Click(object sender, EventArgs e)
        {
            Response.Redirect("BlacklistIPAdd.aspx");
        }

        protected void btnAddBannedNetwork_Click(object sender, EventArgs e)
        {
            Response.Redirect("BlacklistNetworkAdd.aspx");
        }
    }
}