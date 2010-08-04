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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class HeaderControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        
        protected void lFinishImpersonate_Click(object sender, EventArgs e)
        {
            if (NopContext.Current != null &&
                NopContext.Current.IsCurrentCustomerImpersonated &&
                NopContext.Current.OriginalUser != null)
            {
                NopContext.Current.OriginalUser.ImpersonatedCustomerGuid = Guid.Empty;
                Response.Redirect(CommonHelper.GetStoreLocation());
            }
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            Literal lUnreadPrivateMessages = topLoginView.FindControl("lUnreadPrivateMessages") as Literal;
            if (lUnreadPrivateMessages != null)
            {
                lUnreadPrivateMessages.Text = GetUnreadPrivateMessages();
            }
            base.OnPreRender(e);
        }

        protected string GetUnreadPrivateMessages()
        {
            string result = string.Empty;
            if (ForumManager.AllowPrivateMessages &&
                NopContext.Current.User != null && !NopContext.Current.User.IsGuest)
            {
                int totalRecords = 0;
                var privateMessages = ForumManager.GetAllPrivateMessages(0,
                    NopContext.Current.User.CustomerId, false, null, false, string.Empty, 1, 0, out totalRecords);

                if (totalRecords > 0)
                {
                    result = string.Format(GetLocaleResourceString("PrivateMessages.TotalUnread"), totalRecords);

                    //notifications here
                    if (SettingManager.GetSettingValueBoolean("Common.ShowAlertForPM") &&
                        !NopContext.Current.User.NotifiedAboutNewPrivateMessages)
                    {
                        this.DisplayAlertMessage(string.Format(GetLocaleResourceString("PrivateMessages.YouHaveUnreadPM", totalRecords)));
                        NopContext.Current.User.NotifiedAboutNewPrivateMessages = true;
                    }
                }
            }
            return result;
        }
    }
}