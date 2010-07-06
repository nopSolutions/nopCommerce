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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.ExportImport;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class LanguageInfoControl : BaseNopAdministrationUserControl
    {
        private int CompareCultures(CultureInfo x, CultureInfo y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {

                    return x.IetfLanguageTag.CompareTo(y.IetfLanguageTag);
                }
            }
        }

        private void FillDropDowns()
        {
            List<CultureInfo> cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToList();
            cultures.Sort(CompareCultures);
            this.ddlLanguageCulture.Items.Clear();
            foreach (CultureInfo ci in cultures)
            {
                string name = string.Format("{0}. {1}", ci.IetfLanguageTag, ci.EnglishName);
                ListItem item2 = new ListItem(name, ci.IetfLanguageTag);
                this.ddlLanguageCulture.Items.Add(item2);
            }
        }

        private void BindData()
        {
            Language language = LanguageManager.GetLanguageById(this.LanguageId);

            if (language != null)
            {
                this.txtName.Text = language.Name;

                ListItem ciItem = ddlLanguageCulture.Items.FindByValue(language.LanguageCulture);
                if (ciItem != null)
                    ciItem.Selected = true;

                this.txtFlagImageFileName.Text = language.FlagImageFileName;
                this.cbPublished.Checked = language.Published;
                this.txtDisplayOrder.Value = language.DisplayOrder;

                this.pnlImport.Visible = true;
            }
            else
            {
                this.pnlImport.Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.FillDropDowns();
                this.BindData();
            }
        }

        public Language SaveInfo()
        {
            Language language = LanguageManager.GetLanguageById(this.LanguageId);

            string name = txtName.Text;
            string languageCulture = ddlLanguageCulture.SelectedItem.Value;
            string flagImageFileName = txtFlagImageFileName.Text;
            bool published = cbPublished.Checked;
            int displayOrder = txtDisplayOrder.Value;

            if (language != null)
            {
                language = LanguageManager.UpdateLanguage(language.LanguageId, name,
                    languageCulture, flagImageFileName, published, displayOrder);

            }
            else
            {
                language = LanguageManager.InsertLanguage(name, languageCulture,
                    flagImageFileName, published, displayOrder);

            }

            return language;
        }

        protected void btnImportResources_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    Language language = LanguageManager.GetLanguageById(this.LanguageId);

                    if (language != null)
                    {
                        HttpPostedFile importResourcesFile = fuImportResources.PostedFile;
                        if ((importResourcesFile != null) && (!String.IsNullOrEmpty(importResourcesFile.FileName)))
                        {
                            using (StreamReader sr = new StreamReader(importResourcesFile.InputStream, Encoding.UTF8))
                            {
                                string content = sr.ReadToEnd();
                                ImportManager.ImportResources(this.LanguageId, content);
                                ShowMessage(GetLocaleResourceString("Admin.LanguageInfo.ResourcesImported"));
                            }
                        }
                    }
                    else
                        Response.Redirect("Languages.aspx");
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
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