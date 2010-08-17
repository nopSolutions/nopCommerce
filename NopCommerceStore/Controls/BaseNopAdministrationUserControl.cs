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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using AjaxControlToolkit;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web
{
    public partial class BaseNopAdministrationUserControl : UserControl
    {
        public BaseNopAdministrationUserControl()
        {

        }

        protected virtual void BindJQuery()
        {
            CommonHelper.BindJQuery(this.Page);
        }

        protected virtual void BindJQueryIdTabs()
        {
            string jqueryTabs = CommonHelper.GetStoreLocation() + "Scripts/jquery.idTabs.min.js";
            Page.ClientScript.RegisterClientScriptInclude(jqueryTabs, jqueryTabs);
        }

        protected void SelectTab(TabContainer tabContainer, string tabId)
        {
            if (tabContainer == null)
                throw new ArgumentNullException("tabContainer");

            if (!String.IsNullOrEmpty(tabId))
            {
                AjaxControlToolkit.TabPanel tab = tabContainer.FindControl(tabId) as AjaxControlToolkit.TabPanel;
                if (tab != null)
                {
                    tabContainer.ActiveTab = tab;
                }
            }
        }

        protected string GetActiveTabId(TabContainer tabContainer)
        {
            if (tabContainer == null)
                throw new ArgumentNullException("tabContainer");

            if (tabContainer.ActiveTab != null)
                return tabContainer.ActiveTab.ID;

            return string.Empty;
        }

        protected void ProcessException(Exception exc)
        {
            ProcessException(exc, true);
        }

        protected void ProcessException(Exception exc, bool showError)
        {
            LogManager.InsertLog(LogTypeEnum.AdministrationArea, exc.Message, exc);
            if (showError)
            {
                if (SettingManager.GetSettingValueBoolean("Display.AdminArea.ShowFullErrors"))
                {
                    ShowError(exc.Message, exc.ToString());
                }
                else
                {
                    ShowError(exc.Message, string.Empty);
                }
            }
        }

        protected void ShowMessage(string message)
        {
            if (this.Page == null)
                return;

            MasterPage masterPage = this.Page.Master;
            if (masterPage == null)
                return;

            BaseNopAdministrationMasterPage nopAdministrationMasterPage = masterPage as BaseNopAdministrationMasterPage;
            if (nopAdministrationMasterPage != null)
                nopAdministrationMasterPage.ShowMessage(message);
        }

        protected void ShowError(string message)
        {
            ShowError(message, string.Empty);
        }

        protected void ShowError(string message, string completeMessage)
        {
            if (this.Page == null)
                return;

            MasterPage masterPage = this.Page.Master;
            if (masterPage == null)
                return;

            BaseNopAdministrationMasterPage nopAdministrationMasterPage = masterPage as BaseNopAdministrationMasterPage;
            if (nopAdministrationMasterPage != null)
                nopAdministrationMasterPage.ShowError(message, completeMessage);
        }

        protected string GetLocaleResourceString(string resourceName)
        {
            Language language = NopContext.Current.WorkingLanguage;
            return LocalizationManager.GetLocaleResourceString(resourceName, language.LanguageId);
        }

        protected virtual List<Language> GetLocalizableLanguagesSupported()
        {
            return LanguageManager.GetAllLanguages(true);
        }

        protected virtual bool HasLocalizableContent
        {
            get
            {
                var languages = GetLocalizableLanguagesSupported();
                return languages.Count > 1;
            }
        }
    }
}