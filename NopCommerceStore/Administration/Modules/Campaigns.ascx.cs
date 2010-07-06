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
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.ExportImport;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Campaigns;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CampaignsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected void BindData()
        {
            var campaignCollection = CampaignManager.GetAllCampaigns();
            gvCampaigns.DataSource = campaignCollection;
            gvCampaigns.DataBind();
        }

        protected void btnExportCVS_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    string fileName = String.Format("newsletter_emails_{0}_{1}.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));

                    StringBuilder sb = new StringBuilder();
                    var subscriptions = MessageManager.GetAllNewsLetterSubscriptions(false);
                    if (subscriptions.Count == 0)
                    {
                        throw new NopException("No emails to export");
                    }
                    for (int i = 0; i < subscriptions.Count; i++)
                    {
                        var subscription = subscriptions[i];
                        sb.Append(subscription.Email);
                        if (i != subscriptions.Count - 1)
                            sb.AppendLine(",");
                    }
                    string result = sb.ToString();
                    CommonHelper.WriteResponseTxt(result, fileName);
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }        
    }
}