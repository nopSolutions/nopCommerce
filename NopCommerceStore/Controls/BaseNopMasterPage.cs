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

namespace NopSolutions.NopCommerce.Web
{
    public partial class BaseNopMasterPage : MasterPage
    {
        public BaseNopMasterPage() :
            base()
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SetFavIcon();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CheckAffiliate();
            }

            string defaulSEOTitle = SettingManager.GetSettingValue("SEO.DefaultTitle");
            string defaulSEODescription = SettingManager.GetSettingValue("SEO.DefaultMetaDescription");
            string defaulSEOKeywords = SettingManager.GetSettingValue("SEO.DefaultMetaKeywords");
            SEOHelper.RenderTitle(this.Page, defaulSEOTitle, false, false);
            SEOHelper.RenderMetaTag(this.Page, "description", defaulSEODescription, false);
            SEOHelper.RenderMetaTag(this.Page, "keywords", defaulSEOKeywords, false);

            if (SettingManager.GetSettingValueBoolean("Display.ShowNewsHeaderRssURL"))
            {
                SEOHelper.RenderHeaderRssLink(this.Page, defaulSEOTitle + ": News", SEOHelper.GetNewsRssUrl());
            }
            if (SettingManager.GetSettingValueBoolean("Display.ShowBlogHeaderRssURL"))
            {
                SEOHelper.RenderHeaderRssLink(this.Page, defaulSEOTitle + ": Blog", SEOHelper.GetBlogRssUrl());
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            AddPoweredBy();
        }

        protected void CheckAffiliate()
        {
            Affiliate affiliate = AffiliateManager.GetAffiliateById(CommonHelper.QueryStringInt("AffiliateId"));
            if (affiliate != null && affiliate.Active)
            {
                if (NopContext.Current.User == null)
                {
                    HttpCookie affiliateCookie = HttpContext.Current.Request.Cookies.Get("NopCommerce.AffiliateId");
                    if (affiliateCookie == null)
                        affiliateCookie = new HttpCookie("NopCommerce.AffiliateId");

                    affiliateCookie.Value = affiliate.AffiliateId.ToString();
                    affiliateCookie.Expires = DateTime.Now.AddDays(10.0);
                    HttpContext.Current.Response.Cookies.Set(affiliateCookie);
                }
                else if (NopContext.Current.User.AffiliateId != affiliate.AffiliateId)
                {
                    NopContext.Current.User = CustomerManager.SetAffiliate(NopContext.Current.User.CustomerId, affiliate.AffiliateId);
                }
            }
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
        
        protected void AddPoweredBy()
        {
            StringBuilder poweredBy = new StringBuilder();
            poweredBy.Append(Environment.NewLine);
            poweredBy.Append("<!--Powered by nopCommerce - http://www.nopCommerce.com-->");
            poweredBy.Append(Environment.NewLine);
            poweredBy.Append("<!--Copyright (c) 2008-2010-->");
            poweredBy.Append(Environment.NewLine);
            Page.Header.Controls.AddAt(Page.Header.Controls.Count, new LiteralControl(poweredBy.ToString()));
        }

        protected string GetLocaleResourceString(string ResourceName)
        {
            Language language = NopContext.Current.WorkingLanguage;
            return LocalizationManager.GetLocaleResourceString(ResourceName, language.LanguageId);
        }
    }
}