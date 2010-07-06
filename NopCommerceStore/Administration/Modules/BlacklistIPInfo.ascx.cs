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
    public partial class BlacklistIPInfoControl : BaseNopAdministrationUserControl
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
            BannedIpAddress ipAddress = IpBlacklistManager.GetBannedIpAddressById(this.BannedIpAddressId);
            if (ipAddress != null)
            {
                txtBannedIP.Text = ipAddress.Address;
                txtComment.Text = ipAddress.Comment;
                this.pnlCreatedOn.Visible = true;
                this.pnlUpdatedOn.Visible = true;
                lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(ipAddress.CreatedOn, DateTimeKind.Utc).ToString();
                lblUpdatedOn.Text = DateTimeHelper.ConvertToUserTime(ipAddress.UpdatedOn, DateTimeKind.Utc).ToString();
            }
            else
            {
                this.pnlCreatedOn.Visible = false;
                this.pnlUpdatedOn.Visible = false;
            }
        }

        /// <summary>
        /// Saves a BannedIpAddress
        /// </summary>
        /// <returns>BannedIpAddress</returns>
        public BannedIpAddress SaveBannedIpAddressInfo()
        {
            DateTime nowDT = DateTime.UtcNow;
            BannedIpAddress ipAddress = IpBlacklistManager.GetBannedIpAddressById(this.BannedIpAddressId);

            // Check if the IP is valid
            if (!IpBlacklistManager.IsValidIp(txtBannedIP.Text.Trim()))
                throw new NopException("The following isn't a valid IP address: " + txtBannedIP.Text);

            //if ip address is not null update
            if (ipAddress != null)
            {
                ipAddress = IpBlacklistManager.UpdateBannedIpAddress(this.BannedIpAddressId, txtBannedIP.Text,
                    txtComment.Text, ipAddress.CreatedOn, nowDT);
            }
            else //insert
            {
                ipAddress = IpBlacklistManager.InsertBannedIpAddress(txtBannedIP.Text, txtComment.Text, nowDT, nowDT);
            }

            return ipAddress;
        }

        /// <summary>
        /// Gets BannedIpAddressID from query string
        /// </summary>
        public int BannedIpAddressId
        {
            get
            {
                return CommonHelper.QueryStringInt("BannedIpAddressId");
            }
        }
    }
}