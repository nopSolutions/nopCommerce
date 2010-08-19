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
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;



namespace NopSolutions.NopCommerce.Web.Templates.Categories
{
    public partial class ProductsInLines2 : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected void BindData()
        {
            var category = CategoryManager.GetCategoryById(this.CategoryId);
            lName.Text = Server.HtmlEncode(category.LocalizedName);
            lDescription.Text = category.LocalizedDescription;

            var subCategories = CategoryManager.GetAllCategoriesByParentCategoryId(this.CategoryId);
            if (subCategories.Count > 0)
            {
                rptrSubCategories.DataSource = subCategories;
                rptrSubCategories.DataBind();
            }
            else
                rptrSubCategories.Visible = false;

            int totalRecords = 0;
            int pageSize = 10;
            if (category.PageSize > 0)
            {
                pageSize = category.PageSize;
            }
            var productCollection = ProductManager.GetAllProducts(this.CategoryId,
                0, 0, null, pageSize, this.CurrentPageIndex, out totalRecords);
            if (productCollection.Count > 0)
            {
                this.catalogPager.PageSize = pageSize;
                this.catalogPager.TotalRecords = totalRecords;
                this.catalogPager.PageIndex = this.CurrentPageIndex;

                this.lvCatalog.DataSource = productCollection;
                this.lvCatalog.DataBind();
            }
            else
            {
                this.lvCatalog.Visible = false;
            }
        }

        protected void rptrSubCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var category = e.Item.DataItem as Category;
                var hlCategory = e.Item.FindControl("hlCategory") as HyperLink;
                if (hlCategory != null)
                {
                    hlCategory.NavigateUrl = SEOHelper.GetCategoryUrl(category);
                }
            }
        }

        protected void lvCatalog_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var dataItem = e.Item as ListViewDataItem;
                if (dataItem != null)
                {
                    var product = dataItem.DataItem as Product;
                    if (product != null)
                    {
                        var hlProduct = dataItem.FindControl("hlProduct") as HyperLink;
                        hlProduct.NavigateUrl = SEOHelper.GetProductUrl(product);
                    }
                }
            }
        }

        public int CurrentPageIndex
        {
            get
            {
                int _pageIndex = CommonHelper.QueryStringInt(catalogPager.QueryStringProperty);
                _pageIndex--;
                if (_pageIndex < 0)
                    _pageIndex = 0;
                return _pageIndex;
            }
        }

        public int CategoryId
        {
            get
            {
                return CommonHelper.QueryStringInt("CategoryId");
            }
        }
    }
}
