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
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;


namespace NopSolutions.NopCommerce.Web.Templates.Categories
{
    public partial class ProductsInLines2: BaseNopFrontendUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillDropDowns();
                BindData();
            }
        }

        protected void FillDropDowns()
        {
            if (this.SettingManager.GetSettingValueBoolean("Common.AllowProductSorting"))
            {
                ddlSorting.Items.Clear();

                var ddlSortPositionItem = new ListItem(GetLocaleResourceString("ProductSorting.Position"), ((int)ProductSortingEnum.Position).ToString());
                ddlSorting.Items.Add(ddlSortPositionItem);

                var ddlSortNameItem = new ListItem(GetLocaleResourceString("ProductSorting.Name"), ((int)ProductSortingEnum.Name).ToString());
                ddlSorting.Items.Add(ddlSortNameItem);

                var ddlSortPriceItem = new ListItem(GetLocaleResourceString("ProductSorting.Price"), ((int)ProductSortingEnum.Price).ToString());
                ddlSorting.Items.Add(ddlSortPriceItem);

            }
            else
            {
                pnlSorting.Visible = false;
            }
        }

        protected void BindData()
        {
            var category = this.CategoryService.GetCategoryById(this.CategoryId);

            lName.Text = Server.HtmlEncode(category.LocalizedName);
            lDescription.Text = category.LocalizedDescription;

            //subcategories
            var subCategories = this.CategoryService.GetAllCategoriesByParentCategoryId(this.CategoryId);
            if (subCategories.Count > 0)
            {
                rptrSubCategories.DataSource = subCategories;
                rptrSubCategories.DataBind();
            }
            else
                rptrSubCategories.Visible = false;

            //price ranges
            this.ctrlPriceRangeFilter.PriceRanges = category.PriceRanges;

            //page size
            int totalRecords = 0;
            int pageSize = 10;
            if (category.PageSize > 0)
            {
                pageSize = category.PageSize;
            }

            //price ranges
            decimal? minPrice = null;
            decimal? maxPrice = null;
            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;
            if (ctrlPriceRangeFilter.SelectedPriceRange != null)
            {
                minPrice = ctrlPriceRangeFilter.SelectedPriceRange.From;
                if (minPrice.HasValue)
                {
                    minPriceConverted = this.CurrencyService.ConvertCurrency(minPrice.Value, NopContext.Current.WorkingCurrency, this.CurrencyService.PrimaryStoreCurrency);
                }

                maxPrice = ctrlPriceRangeFilter.SelectedPriceRange.To;
                if (maxPrice.HasValue)
                {
                    maxPriceConverted = this.CurrencyService.ConvertCurrency(maxPrice.Value, NopContext.Current.WorkingCurrency, this.CurrencyService.PrimaryStoreCurrency);
                }
            }

            //specification filter
            var psoFilterOption = ctrlProductSpecificationFilter.GetAlreadyFilteredSpecOptionIds();

            //sorting
            ProductSortingEnum orderBy = ProductSortingEnum.Position;
            if (this.SettingManager.GetSettingValueBoolean("Common.AllowProductSorting"))
            {
                CommonHelper.SelectListItem(this.ddlSorting, CommonHelper.QueryStringInt("orderby"));
                orderBy = (ProductSortingEnum)Enum.ToObject(typeof(ProductSortingEnum), int.Parse(ddlSorting.SelectedItem.Value));
            }

            //featured products are not supported by this template
            //that's hwhy we load all featured and non-featured products
            var productCollection = this.ProductService.GetAllProducts(this.CategoryId,
                0, 0, null, minPriceConverted, maxPriceConverted,
                string.Empty, false, pageSize, this.CurrentPageIndex,
                psoFilterOption, orderBy, out totalRecords);

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
                this.pnlSorting.Visible = false;
            }
        }

        protected void ddlSorting_SelectedIndexChanged(object sender, EventArgs e)
        {
            string url = CommonHelper.GetThisPageUrl(true);
            url = CommonHelper.ModifyQueryString(url, "orderby=" + ddlSorting.SelectedItem.Value, null);
            Response.Redirect(url);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ctrlPriceRangeFilter.ExcludedQueryStringParams = catalogPager.QueryStringProperty;

            ctrlProductSpecificationFilter.ExcludedQueryStringParams = catalogPager.QueryStringProperty;
            ctrlProductSpecificationFilter.CategoryId = this.CategoryId;

            ctrlProductSpecificationFilter.ReservedQueryStringParams = "CategoryId,";
            ctrlProductSpecificationFilter.ReservedQueryStringParams += "orderby,";
            ctrlProductSpecificationFilter.ReservedQueryStringParams += ctrlPriceRangeFilter.QueryStringProperty;
            ctrlProductSpecificationFilter.ReservedQueryStringParams += ",";
            ctrlProductSpecificationFilter.ReservedQueryStringParams += catalogPager.QueryStringProperty;
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.pnlFilters.Visible = ctrlPriceRangeFilter.Visible || ctrlProductSpecificationFilter.Visible;
            base.OnPreRender(e);
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
