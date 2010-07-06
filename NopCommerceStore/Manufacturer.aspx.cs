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
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common;
namespace NopSolutions.NopCommerce.Web
{
    public partial class ManufacturerPage : BaseNopPage
    {
        Manufacturer manufacturer = null;

        private void CreateChildControlsTree()
        {
            manufacturer = ManufacturerManager.GetManufacturerById(this.ManufacturerId);
            if (manufacturer != null)
            {
                Control child = null;

                var manufacturerTemplate = manufacturer.ManufacturerTemplate;
                if (manufacturerTemplate == null)
                    throw new NopException(string.Format("Manufacturer template path can not be empty. ManufacturerID={0}", manufacturer.ManufacturerId));

                child = base.LoadControl(manufacturerTemplate.TemplatePath);
                this.ManufacturerPlaceHolder.Controls.Add(child);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.CreateChildControlsTree();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (manufacturer == null || manufacturer.Deleted || !manufacturer.Published)
                Response.Redirect(CommonHelper.GetStoreLocation());
            
            string title = string.Empty;
            if (!string.IsNullOrEmpty(manufacturer.LocalizedMetaTitle))
                title = manufacturer.LocalizedMetaTitle;
            else
                title = manufacturer.LocalizedName;            
            SEOHelper.RenderTitle(this, title, true);
            SEOHelper.RenderMetaTag(this, "description", manufacturer.LocalizedMetaDescription, true);
            SEOHelper.RenderMetaTag(this, "keywords", manufacturer.LocalizedMetaKeywords, true);

            if (!Page.IsPostBack)
            {
                NopContext.Current.LastContinueShoppingPage = CommonHelper.GetThisPageUrl(true);
            }
        }

        public int ManufacturerId
        {
            get
            {
                return CommonHelper.QueryStringInt("ManufacturerId");
            }
        }

        public override PageSslProtectionEnum SslProtected
        {
            get
            {
                return PageSslProtectionEnum.No;
            }
        }
    }
}