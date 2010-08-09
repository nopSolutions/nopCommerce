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
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using System.Collections.Generic;
using AjaxControlToolkit;


namespace NopSolutions.NopCommerce.Web
{
    public partial class BaseNopAdministrationPage : Page
    {
        public BaseNopAdministrationPage()
        {

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

        protected override void OnPreInit(EventArgs e)
        {
            //admin user validation
            if (this.AdministratorSecurityValidationEnabled &&
                !ValidateAdministratorSecurity())
            {
                string url = SEOHelper.GetAdminAreaLoginPageUrl();
                Response.Redirect(url);
            }

            //page security validation
            if (!ValidatePageSecurity())
            {
                string url = SEOHelper.GetAdminAreaAccessDeniedUrl();
                Response.Redirect(url);
            }

            if(this.IPAddressValidationEnabled && !ValidateIP())
            {
                Response.Redirect(SEOHelper.GetAdminAreaAccessDeniedUrl());
            }

            base.OnPreInit(e);
        }

        /// <summary>
        /// Validates page security for current user
        /// </summary>
        /// <returns>true if action is allow; otherwise false</returns>
        protected virtual bool ValidatePageSecurity()
        {
            return true;
        }

        /// <summary>
        /// Validates admin security
        /// </summary>
        /// <returns>true if admin access is allow; otherwise false</returns>
        protected virtual bool ValidateAdministratorSecurity()
        {
            if (NopContext.Current == null ||
                NopContext.Current.User == null ||
                NopContext.Current.User.IsGuest)
                return false;

            return NopContext.Current.User.IsAdmin;
        }

        protected virtual bool ValidateIP()
        {
            string ipList = SettingManager.GetSettingValue("Security.AdminAreaAllowedIP").Trim();
            if(String.IsNullOrEmpty(ipList))
            {
                return true;
            }
            foreach (string s in ipList.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (s.Trim().Equals(NopContext.Current.UserHostAddress))
                {
                    return true;
                }
            }
            return false;
        }

        protected override void OnLoad(EventArgs e)
        {
            CommonHelper.EnsureSsl();
            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            string adminJS = CommonHelper.GetStoreLocation() + "Scripts/admin.js";
            Page.ClientScript.RegisterClientScriptInclude(adminJS, adminJS);
            
            base.OnPreRender(e);
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
            MasterPage masterPage = this.Master;
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
            MasterPage masterPage = this.Master;
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

        protected virtual bool AdministratorSecurityValidationEnabled
        {
            get
            {
                return true;
            }
        }

        protected virtual bool IPAddressValidationEnabled
        {
            get
            {
                return true;
            }
        }
    }
}
