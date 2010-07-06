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
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common;
 
namespace NopSolutions.NopCommerce.Web
{
    public partial class CategoryPage : BaseNopPage
    {
        Category category = null;

        private void CreateChildControlsTree()
        {
            category = CategoryManager.GetCategoryById(this.CategoryId);
            if (category != null)
            {
                Control child = null;

                CategoryTemplate categoryTemplate = category.CategoryTemplate;
                if (categoryTemplate == null)
                    throw new NopException(string.Format("Category template path can not be empty. CategoryID={0}", category.CategoryId));

                child = base.LoadControl(categoryTemplate.TemplatePath);
                this.CategoryPlaceHolder.Controls.Add(child);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);            
            this.CreateChildControlsTree();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (category == null || category.Deleted || !category.Published)
                Response.Redirect(CommonHelper.GetStoreLocation());

            string title = string.Empty;
            if (!string.IsNullOrEmpty(category.LocalizedMetaTitle))
                title = category.LocalizedMetaTitle;
            else
                title = category.LocalizedName;
            SEOHelper.RenderTitle(this, title, true);
            SEOHelper.RenderMetaTag(this, "description", category.LocalizedMetaDescription, true);
            SEOHelper.RenderMetaTag(this, "keywords", category.LocalizedMetaKeywords, true);

            if (!Page.IsPostBack)
            {
                NopContext.Current.LastContinueShoppingPage = CommonHelper.GetThisPageUrl(true);
            }
        }

        public int CategoryId
        {
            get
            {
                return CommonHelper.QueryStringInt("CategoryId");
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