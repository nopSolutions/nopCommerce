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
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class BlacklistNetworkInfoControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        /// <summary>
        /// Bind controls on the form
        /// </summary>
        private void BindData()
        {
            BannedIpNetwork ipNetwork = IpBlacklistManager.GetBannedIpNetworkById(this.BannedIpNetworkId);
            if (ipNetwork != null)
            {
                txtBannedIP.Text = ipNetwork.ToString();
                txtComment.Text = ipNetwork.Comment;
                txtIpException.Text = ipNetwork.IpException;
                this.pnlCreatedOn.Visible = true;
                this.pnlUpdatedOn.Visible = true;
                lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(ipNetwork.CreatedOn, DateTimeKind.Utc).ToString();
                lblUpdatedOn.Text = DateTimeHelper.ConvertToUserTime(ipNetwork.UpdatedOn, DateTimeKind.Utc).ToString();
            }
            else
            {
                this.pnlCreatedOn.Visible = false;
                this.pnlUpdatedOn.Visible = false;
            }
        }

        /// <summary>
        /// Saves a BannedIpNetwork
        /// </summary>
        /// <returns>BannedIpNetwork</returns>
        public BannedIpNetwork SaveBannedIpNetworkInfo()
        {
            DateTime nowDT = DateTime.UtcNow;
            //split the text in the BannedIP to get the current IPs
            string[] rangeItems = txtBannedIP.Text.ToString().Split("-".ToCharArray());

            // Check if the 1st IP is valid
            if (!IpBlacklistManager.IsValidIp(rangeItems[0].Trim()))
                throw new NopException("The following isn't a valid IP address: " + rangeItems[0]);

            // Check if the 2nd IP is valid
            if (!IpBlacklistManager.IsValidIp(rangeItems[1].Trim()))
                throw new NopException("The following isn't a valid IP address: " + rangeItems[1]);

            BannedIpNetwork ipNetwork = IpBlacklistManager.GetBannedIpNetworkById(this.BannedIpNetworkId);
            //if ip network is not null update
            if (ipNetwork != null)
            {
                ipNetwork = IpBlacklistManager.UpdateBannedIpNetwork(this.BannedIpNetworkId, rangeItems[0], rangeItems[1],
                    txtComment.Text, txtIpException.Text, ipNetwork.CreatedOn, nowDT);
            }
            else //insert
            {
                ipNetwork = IpBlacklistManager.InsertBannedIpNetwork(rangeItems[0], rangeItems[1],
                    txtComment.Text, txtIpException.Text, nowDT, nowDT);
            }

            return ipNetwork;
        }

        /// <summary>
        /// Gets BannedIpNetworkID from query string
        /// </summary>
        public int BannedIpNetworkId
        {
            get
            {
                return CommonHelper.QueryStringInt("BannedIpNetworkId");
            }
        }

    }
}