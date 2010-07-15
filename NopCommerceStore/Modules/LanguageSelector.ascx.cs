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
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using System.Collections.Generic;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class LanguageSelectorControl : BaseNopUserControl
    {
        private void BindLanguages()
        {
            var languages = LanguageManager.GetAllLanguages();
            if(languages.Count > 1)
            {
                var customerLanguage = NopContext.Current.WorkingLanguage;
                Visible = true;

                if(SettingManager.GetSettingValueBoolean("Common.UseImagesForLanguageSelection", false))
                {
                    rptLanguages.Visible = true;
                    ddlLanguages.Visible = false;
                    List<RptLanguageItem> itemList = new List<RptLanguageItem>();

                    foreach(var language in languages)
                    {
                        RptLanguageItem item = new RptLanguageItem();
                        item.Class = (language.LanguageId == customerLanguage.LanguageId ? "selected" : String.Empty);
                        item.ImageUrl = language.IconUrl;
                        item.LanguageID = language.LanguageId;
                        item.Name = language.Name;
                        itemList.Add(item);
                    }

                    rptLanguages.DataSource = itemList;
                    rptLanguages.DataBind();
                }
                else
                {
                    rptLanguages.Visible = false;
                    ddlLanguages.Visible = true;

                    ddlLanguages.Items.Clear();

                    foreach(var language in languages)
                    {
                        var item = new ListItem(language.Name, language.LanguageId.ToString());
                        ddlLanguages.Items.Add(item);
                    }
                    if(customerLanguage != null)
                    {
                        CommonHelper.SelectListItem(ddlLanguages, customerLanguage.LanguageId);
                    }
                }
            }
            else
            {
                Visible = false;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            BindLanguages();
            base.OnInit(e);
        }

        protected void ddlLanguages_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            int languagesId = int.Parse(this.ddlLanguages.SelectedItem.Value);
            var language = LanguageManager.GetLanguageById(languagesId);
            if (language != null && language.Published)
            {
                NopContext.Current.WorkingLanguage = language;
                CommonHelper.ReloadCurrentPage();
            }
        }

        protected void BtnSelectLanguage_OnCommand(object sender, CommandEventArgs e)
        {
            if(e.CommandName.Equals("SelectLanguage"))
            {
                int languageId = Int32.Parse(e.CommandArgument.ToString());
                var language = LanguageManager.GetLanguageById(languageId);
                if(language != null && language.Published)
                {
                    NopContext.Current.WorkingLanguage = language;
                    CommonHelper.ReloadCurrentPage();
                }
            }
        }

        private class RptLanguageItem
        {
            public string Class
            {
                get;
                set;
            }

            public string ImageUrl
            {
                get;
                set;
            }

            public int LanguageID
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }
        }
    }
}
