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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class SettingDetailsControl : BaseNopAdministrationUserControl
    {
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    Setting setting = ctrlSettingInfo.SaveInfo();

                    CustomerActivityManager.InsertActivity(
                        "EditSetting",
                        GetLocaleResourceString("ActivityLog.EditSetting"),
                        setting.Name);

                    Response.Redirect("SettingDetails.aspx?SettingID=" + setting.SettingId.ToString());
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                Setting setting = SettingManager.GetSettingById(this.SettingId);
                if (setting != null)
                {
                    SettingManager.DeleteSetting(setting.SettingId);

                    CustomerActivityManager.InsertActivity(
                        "DeleteSetting",
                        GetLocaleResourceString("ActivityLog.DeleteSetting"),
                        setting.Name);
                }
                Response.Redirect("Settings.aspx");
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        public int SettingId
        {
            get
            {
                return CommonHelper.QueryStringInt("SettingId");
            }
        }
    }
}