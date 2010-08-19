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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;



namespace NopSolutions.NopCommerce.Web.Templates.Categories
{
    public partial class ProductsInLines1 : BaseNopUserControl
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
            if (SettingManager.GetSettingValueBoolean("Common.AllowProductSorting"))
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
            var category = CategoryManager.GetCategoryById(this.CategoryId);

            //breadcrumb
            rptrCategoryBreadcrumb.DataSource = CategoryManager.GetBreadCrumb(this.CategoryId);
            rptrCategoryBreadcrumb.DataBind();

            lDescription.Text = category.LocalizedDescription;

            //subcategories
            var subCategories = CategoryManager.GetAllCategoriesByParentCategoryId(category.CategoryId);
            if (subCategories.Count > 0)
            {
                dlSubCategories.DataSource = subCategories;
                dlSubCategories.DataBind();
            }
            else
                dlSubCategories.Visible = false;

            //featured products
            var featuredProducts = category.FeaturedProducts;
            if (featuredProducts.Count > 0)
            {
                dlFeaturedProducts.DataSource = featuredProducts;
                dlFeaturedProducts.DataBind();
            }
            else
            {
                pnlFeaturedProducts.Visible = false;
            }

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
                    minPriceConverted = CurrencyManager.ConvertCurrency(minPrice.Value, NopContext.Current.WorkingCurrency, CurrencyManager.PrimaryStoreCurrency);
                }

                maxPrice = ctrlPriceRangeFilter.SelectedPriceRange.To;
                if (maxPrice.HasValue)
                {
                    maxPriceConverted = CurrencyManager.ConvertCurrency(maxPrice.Value, NopContext.Current.WorkingCurrency, CurrencyManager.PrimaryStoreCurrency);
                }
            }

            //specification filter
            var psoFilterOption = ctrlProductSpecificationFilter.GetAlreadyFilteredSpecOptionIds();

            //sorting
            ProductSortingEnum orderBy = ProductSortingEnum.Position;
            if (SettingManager.GetSettingValueBoolean("Common.AllowProductSorting"))
            {
                CommonHelper.SelectListItem(this.ddlSorting, CommonHelper.QueryStringInt("orderby"));
                orderBy = (ProductSortingEnum)Enum.ToObject(typeof(ProductSortingEnum), int.Parse(ddlSorting.SelectedItem.Value));
            }

            var productCollection = ProductManager.GetAllProducts(this.CategoryId,
                0, 0, false, minPriceConverted, maxPriceConverted,
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

        protected void dlSubCategories_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var category = e.Item.DataItem as Category;
                string categoryURL = SEOHelper.GetCategoryUrl(category);

                var hlImageLink = e.Item.FindControl("hlImageLink") as HyperLink;
                if (hlImageLink != null)
                {
                    hlImageLink.ImageUrl = PictureManager.GetPictureUrl(category.PictureId, SettingManager.GetSettingValueInteger("Media.Category.ThumbnailImageSize", 125), true);
                    hlImageLink.NavigateUrl = categoryURL;
                    hlImageLink.ToolTip = String.Format(GetLocaleResourceString("Media.Category.ImageLinkTitleFormat"), category.LocalizedName);
                    hlImageLink.Text = String.Format(GetLocaleResourceString("Media.Category.ImageAlternateTextFormat"), category.LocalizedName);
                }

                var hlCategory = e.Item.FindControl("hlCategory") as HyperLink;
                if (hlCategory != null)
                {
                    hlCategory.NavigateUrl = categoryURL;
                    hlCategory.ToolTip = String.Format(GetLocaleResourceString("Media.Category.ImageLinkTitleFormat"), category.LocalizedName);
                    hlCategory.Text = Server.HtmlEncode(category.LocalizedName);
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