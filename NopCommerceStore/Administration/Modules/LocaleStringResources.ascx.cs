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
using System.Linq;
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
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class LocaleStringResourcesControl : BaseNopAdministrationUserControl
    {
        private void FillDropDowns()
        {
            this.ddlLanguage.Items.Clear();
            var languageCollection = IoCFactory.Resolve<ILanguageService>().GetAllLanguages();
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

        private void BindGrid()
        {
            if (this.ddlLanguage.SelectedItem == null)
                return;

            Language language = IoCFactory.Resolve<ILanguageService>().GetLanguageById(int.Parse(this.ddlLanguage.SelectedItem.Value));
            if (language != null)
            {
                var allResources = language.LocaleStringResources;
                var filteredResources = new List<LocaleStringResource>();

                string filterByResourceName = txtResourceName.Text.Trim();
                string filterByResourceValue = txtResourceValue.Text.Trim();
                foreach (KeyValuePair<string, LocaleStringResource> kvp in allResources)
                {
                    if (kvp.Value != null)
                    {
                        //filter by resource name
                        bool resourceNameOK = false;
                        
                        if (String.IsNullOrWhiteSpace(filterByResourceName) ||
                            kvp.Value.ResourceName.IndexOf(filterByResourceName, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            resourceNameOK = true;
                        }
                        //filter by resource value
                        bool resourceValueOK = false;
                        if (String.IsNullOrWhiteSpace(filterByResourceValue) ||
                            kvp.Value.ResourceValue.IndexOf(filterByResourceValue, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            resourceValueOK = true;
                        }

                        if (resourceNameOK && resourceValueOK)
                        {
                            filteredResources.Add(kvp.Value);
                        }
                    }
                }
                gvLocaleStringResources.DataSource = filteredResources;
                gvLocaleStringResources.DataBind();
            }

            //btnAddNew.Visible = this.ddlLanguage.SelectedIndex > 0;
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    int languageId = 0;
                    if (this.ddlLanguage.SelectedItem != null)
                    {
                        languageId = int.Parse(this.ddlLanguage.SelectedItem.Value);
                    }
                    else
                    {
                        languageId = IoCFactory.Resolve<ILanguageService>().GetAllLanguages().FirstOrDefault().LanguageId;
                    }
                    Response.Redirect("LocaleStringResourceAdd.aspx?LanguageID=" + languageId);
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    BindGrid();
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