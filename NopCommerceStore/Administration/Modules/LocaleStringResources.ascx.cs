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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.MobileControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.Common.Utils;


namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class LocaleStringResourcesControl : BaseNopAdministrationUserControl
    {
        private void FillDropDowns()
        {
            this.ddlLanguage.Items.Clear();
            ListItem ddlLanguageItem = new ListItem(GetLocaleResourceString("Admin.LocaleStringResources.SelectLanguage"), "0");
            this.ddlLanguage.Items.Add(ddlLanguageItem);
            var languageCollection = LanguageManager.GetAllLanguages();
            foreach (Language language in languageCollection)
            {
                ListItem ddlLanguageItem2 = new ListItem(language.Name, language.LanguageId.ToString());
                this.ddlLanguage.Items.Add(ddlLanguageItem2);
                if (this.LanguageId == language.LanguageId)
                    ddlLanguageItem2.Selected = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillDropDowns();
                BindGrid();
            }
        }

        void BindGrid()
        {
            Language language = LanguageManager.GetLanguageById(int.Parse(this.ddlLanguage.SelectedItem.Value));
            if (language != null)
            {
                var resourceDictionary = language.LocaleStringResources;
                List<LocaleStringResource> resources = new List<LocaleStringResource>();
                foreach (KeyValuePair<string, LocaleStringResource> kvp in resourceDictionary)
                {
                    if (kvp.Value != null)
                        resources.Add(kvp.Value);
                }
                gvLocaleStringResources.DataSource = resources;
                gvLocaleStringResources.DataBind();
            }

            btnAddNew.Visible = this.ddlLanguage.SelectedIndex > 0;
        }

        protected void ddlLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Language language = LanguageManager.GetLanguageById(int.Parse(this.ddlLanguage.SelectedItem.Value));
            if (language != null)
                Response.Redirect("LocaleStringResources.aspx?LanguageID=" + language.LanguageId);
            else
                Response.Redirect("LocaleStringResources.aspx");
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    Language language = LanguageManager.GetLanguageById(int.Parse(this.ddlLanguage.SelectedItem.Value));
                    if (language != null)
                        Response.Redirect("LocaleStringResourceAdd.aspx?LanguageID=" + language.LanguageId);
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void gvLocaleStringResources_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLocaleStringResources.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        public int LanguageId
        {
            get
            {
                return CommonHelper.QueryStringInt("LanguageId");
            }
        }
    }
}