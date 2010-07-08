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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Campaigns;
using NopSolutions.NopCommerce.Common.Utils; 

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CampaignInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Campaign campaign = CampaignManager.GetCampaignById(this.CampaignId);
            if (campaign != null)
            {
                this.pnlSendCampaign.Visible = true;
                StringBuilder allowedTokensString = new StringBuilder();
                string[] allowedTokens = MessageManager.GetListOfCampaignAllowedTokens();
                for (int i = 0; i < allowedTokens.Length; i++)
                {
                    string token = allowedTokens[i];
                    allowedTokensString.Append(token);
                    if (i != allowedTokens.Length - 1)
                        allowedTokensString.Append(", ");
                }
                this.lblAllowedTokens.Text = allowedTokensString.ToString();

                this.txtName.Text = campaign.Name;
                this.txtSubject.Text = campaign.Subject;
                this.txtBody.Content = campaign.Body;

                this.pnlCreatedOn.Visible = true;
                this.lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(campaign.CreatedOn, DateTimeKind.Utc).ToString();
            }
            else
            {
                this.pnlSendCampaign.Visible = false;

                this.pnlCreatedOn.Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        public Campaign SaveInfo()
        {
            Campaign campaign = CampaignManager.GetCampaignById(this.CampaignId);

            if (campaign != null)
            {
                campaign = CampaignManager.UpdateCampaign(campaign.CampaignId,
                    txtName.Text, txtSubject.Text, txtBody.Content, campaign.CreatedOn);
            }
            else
            {
                campaign = CampaignManager.InsertCampaign(txtName.Text,
                       txtSubject.Text, txtBody.Content, DateTime.UtcNow);
            }

            return campaign;
        }

        protected void btnSendTestEmail_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    CampaignManager.SendCampaign(this.CampaignId, txtSendTestEmailTo.Text);
                    lblSendEmailResult.Text = GetLocaleResourceString("Admin.CampaignInfo.EmailSent");
                }
                catch (Exception exc)
                {
                    lblSendEmailResult.Text = exc.Message;
                }
            }
        }

        protected void btnSendMassEmail_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int scriptTimeout = base.Server.ScriptTimeout;
                try
                {
                    Server.ScriptTimeout = 300;
                    var subscriptions = MessageManager.GetAllNewsLetterSubscriptions(string.Empty, false);
                    int totalEmailsSent = CampaignManager.SendCampaign(this.CampaignId, subscriptions);
                    lblSendEmailResult.Text = string.Format(GetLocaleResourceString("Admin.CampaignInfo.EmailSentToCustomers"), totalEmailsSent);
                }
                catch (Exception exc)
                {
                    lblSendEmailResult.Text = exc.ToString();
                }
                finally
                {
                    Server.ScriptTimeout = scriptTimeout;
                }
            }
        }

        public int CampaignId
        {
            get
            {
                return CommonHelper.QueryStringInt("CampaignId");
            }
        }
    }
}