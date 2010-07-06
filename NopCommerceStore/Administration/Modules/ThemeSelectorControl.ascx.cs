using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ThemeSelectorControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            string[] systemThemes = SettingManager.GetSettingValue("Display.SystemThemes").Split(',');
      
            var themes = from f in Directory.GetDirectories(Server.MapPath("~/App_Themes"))
                       where  (!systemThemes.Contains(Path.GetFileName(f).ToLower()))
                       select Path.GetFileName(f);
            
            ddlThemes.DataSource = themes;
            ddlThemes.DataBind();

            CommonHelper.SelectListItem(this.ddlThemes, 
                SettingManager.GetSettingValue("Display.PublicStoreTheme"));
        }
        
        public string SelectedTheme
        {
            get 
            {
                return ddlThemes.SelectedItem.Value;
            }
        }
    }
}