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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class StoreThemeSelectorControl: BaseNopFrontendUserControl
    {
        private void BindThemes()
        {
            if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.AllowCustomerSelectTheme"))
            {
                this.Visible = true;
                this.ddlTheme.Items.Clear();

                string[] systemThemes = IoC.Resolve<ISettingManager>().GetSettingValue("Display.SystemThemes").Split(',');

                var themes = from f in Directory.GetDirectories(Server.MapPath("~/App_Themes"))
                             where (!systemThemes.Contains(Path.GetFileName(f).ToLower()))
                             select Path.GetFileName(f);

                foreach (string theme in themes)
                {
                    var item = new ListItem(theme, theme);
                    ddlTheme.Items.Add(item);
                }

                CommonHelper.SelectListItem(this.ddlTheme, NopContext.Current.WorkingTheme);
            }
            else
                this.Visible = false;
        }

        protected override void OnInit(EventArgs e)
        {
            BindThemes();
            base.OnInit(e);
        }

        protected void ddlTheme_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var theme = this.ddlTheme.SelectedItem.Value;
            NopContext.Current.WorkingTheme = theme;
            CommonHelper.ReloadCurrentPage();
        }
    }
}
