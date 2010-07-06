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
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class AdminLanguageSelectorControl : BaseNopAdministrationUserControl
    {
        private void BindLanguages()
        {
            var languages = LanguageManager.GetAllLanguages(false);
            if (languages.Count > 1)
            {
                this.Visible = true;
                this.ddlLanguages.Items.Clear();
                Language customerLanguage = NopContext.Current.WorkingLanguage;
                foreach (Language language in languages)
                {
                    ListItem item = new ListItem(language.Name, language.LanguageId.ToString());
                    this.ddlLanguages.Items.Add(item);
                }
                if (customerLanguage != null)
                    CommonHelper.SelectListItem(this.ddlLanguages, customerLanguage.LanguageId);
            }
            else
                this.Visible = false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindLanguages();
            }
        }

        protected void ddlLanguages_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            int languagesId = int.Parse(this.ddlLanguages.SelectedItem.Value);
            Language language = LanguageManager.GetLanguageById(languagesId);
            if (language != null && language.Published)
            {
                NopContext.Current.WorkingLanguage = language;
                CommonHelper.ReloadCurrentPage();
            }
        }
    }
}