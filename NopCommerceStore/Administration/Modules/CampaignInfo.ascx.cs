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
using NopSolutions.NopCommerce.BusinessLogic.IoC; 

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CampaignInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Campaign campaign = IoCFactory.Resolve<ICampaignService>().GetCampaignById(this.CampaignId);
            if (campaign != null)
            {
                this.pnlSendCampaign.Visible = true;
                StringBuilder allowedTokensString = new StringBuilder();
                string[] allowedTokens = IoCFactory.Resolve<IMessageService>().GetListOfCampaignAllowedTokens();
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
                this.txtBody.Value = campaign.Body;

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
            Campaign campaign = IoCFactory.Resolve<ICampaignService>().GetCampaignById(this.CampaignId);

            if (campaign != null)
            {
                campaign.Name = txtName.Text;
                campaign.Subject = txtSubject.Text;
                campaign.Body = txtBody.Value;
                IoCFactory.Resolve<ICampaignService>().UpdateCampaign(campaign);
            }
            else
            {
                campaign = new Campaign()
                {
                    Name = txtName.Text,
                    Subject = txtSubject.Text,
                    Body = txtBody.Value,
                    CreatedOn = DateTime.UtcNow
                };
                IoCFactory.Resolve<ICampaignService>().InsertCampaign(campaign);
            }

            return campaign;
        }

        protected void btnSendTestEmail_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    IoCFactory.Resolve<ICampaignService>().SendCampaign(this.CampaignId, txtSendTestEmailTo.Text);
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
                    var subscriptions = IoCFactory.Resolve<IMessageService>().GetAllNewsLetterSubscriptions(string.Empty, false);
                    int totalEmailsSent = IoCFactory.Resolve<ICampaignService>().SendCampaign(this.CampaignId, subscriptions);
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