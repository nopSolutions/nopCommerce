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
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web
{
    public partial class BaseNopMasterPage : MasterPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SetFavIcon();
        }

        protected void SetFavIcon()
        {
            string favIconPath = HttpContext.Current.Request.PhysicalApplicationPath + "favicon.ico";
            if(File.Exists(favIconPath))
            {
                string favIconUrl = CommonHelper.GetStoreLocation() + "favicon.ico";

                HtmlLink htmlLink1 = new HtmlLink();
                htmlLink1.Attributes["rel"] = "icon";
                htmlLink1.Attributes["href"] = favIconUrl;

                HtmlLink htmlLink2 = new HtmlLink();
                htmlLink2.Attributes["rel"] = "shortcut icon";
                htmlLink2.Attributes["href"] = favIconUrl;

                Page.Header.Controls.Add(htmlLink1);
                Page.Header.Controls.Add(htmlLink2);
            }
        }

        protected string GetLocaleResourceString(string ResourceName)
        {
            Language language = NopContext.Current.WorkingLanguage;
            return this.LocalizationManager.GetLocaleResourceString(ResourceName, language.LanguageId);
        }

        private ILocalizationManager _localizationManager;
        public ILocalizationManager LocalizationManager
        {
            get
            {
                if (_localizationManager == null)
                {
                    _localizationManager = IoC.Resolve<ILocalizationManager>();
                }
                return _localizationManager;
            }
        }
    }
}