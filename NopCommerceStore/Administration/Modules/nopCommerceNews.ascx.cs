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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;
 

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class nopCommerceNewsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        public string FormatDateTime(string pubDateParsed)
        {
            DateTime dt = DateTime.Parse(pubDateParsed);
            dt = DateTimeHelper.ConvertToUserTime(dt, TimeZoneInfo.Utc);
            return dt.ToString("d");
        }

        protected void BindData()
        {
            try
            {
                dsNopCommerceNews.Url = string.Format("http://www.nopCommerce.com/NewsRSS.aspx?Version={0}&Localhost={1}&HideAdvertisements={2}&StoreURL={3}", SiteHelper.GetCurrentVersion(), HttpContext.Current.Request.Url.IsLoopback, SettingManager.GetSettingValueBoolean("Common.HideAdvertisementsOnAdminArea"), SettingManager.StoreUrl);
                lvNopCommerceNews.DataBind();
            }
            catch (Exception)
            {
                //ShowError("No internet connection. nopCommerce news could not be loaded", exc.Message);
                this.Visible = false;
            }
        }
    }
}