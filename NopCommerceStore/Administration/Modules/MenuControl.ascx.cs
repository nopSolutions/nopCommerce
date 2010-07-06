using System;
using System.Text;
using System.Web;
using System.Web.UI;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Utils.Html;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class MenuControl : BaseNopAdministrationUserControl
    {
        #region Handlers
        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();

            string superFishMenu = CommonHelper.GetStoreLocation() + "Scripts/jquery.superfishmenu.js";
            Page.ClientScript.RegisterClientScriptInclude(superFishMenu, superFishMenu);
            Page.ClientScript.RegisterClientScriptBlock(GetType(), String.Format("{0}_sfmenu", ClientID), String.Format("$(document).ready(function(){{$('#{0}').superfish({{autoArrows:false,speed:'fast',delay:200}});}});", ClientID), true); 
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                SiteMapProvider siteMapProvider = SiteMap.Providers[SiteMapProviderName];
                if(siteMapProvider == null)
                {
                    Visible = false;
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.AppendFormat("<ul id=\"{0}\" class=\"sf-menu\">", ClientID);
                    foreach(SiteMapNode node in siteMapProvider.RootNode.ChildNodes)
                    {
                        BuildMenuRecursive(sb, node);
                    }
                    sb.Append("</ul>");
                    lblMenuContent.Text = sb.ToString();
                }
            }
        }
        #endregion

        #region Utilities
        private void BuildMenuRecursive(StringBuilder sb, SiteMapNode node)
        {
            string imgUrl = node["IconUrl"];
            if (!String.IsNullOrEmpty(imgUrl) && imgUrl.StartsWith("~/"))
            {
                imgUrl = imgUrl.Substring(2, imgUrl.Length - 2);
                imgUrl = CommonHelper.GetStoreLocation() + imgUrl;
            }

            string title = HttpUtility.HtmlEncode(!String.IsNullOrEmpty(node["nopResourceTitle"]) ? GetLocaleResourceString(node["nopResourceTitle"]) : node.Title);
            string descr = HttpUtility.HtmlEncode(!String.IsNullOrEmpty(node["nopResourceDescription"]) ? GetLocaleResourceString(node["nopResourceDescription"]) : node.Description);

            sb.Append("<li>");
            sb.AppendFormat("<a href=\"{0}\" title=\"{2}\">{3}{1}</a>", (String.IsNullOrEmpty(node.Url) ? "#" : node.Url), title, descr, (!String.IsNullOrEmpty(imgUrl) ? String.Format("<img src=\"{0}\" alt=\"{1}\" /> ", imgUrl, title) : String.Empty));
            if(node.HasChildNodes)
            {
                sb.Append("<ul>");
                foreach(SiteMapNode childNode in node.ChildNodes)
                {
                    BuildMenuRecursive(sb, childNode);
                }
                sb.Append("</ul>");
            }
            sb.Append("</li>");
        }
        #endregion

        #region Properties
        public string SiteMapProviderName
        {
            get
            {
                return Convert.ToString(ViewState["SiteMapProviderName"]);
            }
            set
            {
                ViewState["SiteMapProviderName"] = value;
            }
        }
        #endregion
    }
}