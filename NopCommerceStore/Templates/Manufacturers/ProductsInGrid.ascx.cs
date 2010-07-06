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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Templates.Manufacturers
{
    public partial class ProductsInGrid : BaseNopUserControl
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
            var manufacturer = ManufacturerManager.GetManufacturerById(this.ManufacturerId);
            lName.Text = Server.HtmlEncode(manufacturer.LocalizedName);
            lDescription.Text = manufacturer.LocalizedDescription;

            //featured products
            var featuredProducts = manufacturer.FeaturedProducts;
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
            this.ctrlPriceRangeFilter.PriceRanges = manufacturer.PriceRanges;

            if (string.IsNullOrEmpty(ctrlPriceRangeFilter.PriceRanges))
                this.pnlFilters.Visible = false;

            //page size
            int totalRecords = 0;
            int pageSize = 10;
            if (manufacturer.PageSize > 0)
            {
                pageSize = manufacturer.PageSize;
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
            
            //sorting
            ProductSortingEnum orderBy = ProductSortingEnum.Position;
            if (SettingManager.GetSettingValueBoolean("Common.AllowProductSorting"))
            {
                CommonHelper.SelectListItem(this.ddlSorting, CommonHelper.QueryStringInt("orderby"));
                orderBy = (ProductSortingEnum)Enum.ToObject(typeof(ProductSortingEnum), int.Parse(ddlSorting.SelectedItem.Value));
            }

            var productCollection = ProductManager.GetAllProducts(0,
                this.ManufacturerId, 0, false, minPriceConverted, maxPriceConverted,
                string.Empty, false, pageSize, this.CurrentPageIndex,
                null, orderBy, out totalRecords);

            if (productCollection.Count > 0)
            {
                this.productsPager.PageSize = pageSize;
                this.productsPager.TotalRecords = totalRecords;
                this.productsPager.PageIndex = this.CurrentPageIndex;

                this.dlProducts.DataSource = productCollection;
                this.dlProducts.DataBind();
            }
            else
            {
                this.dlProducts.Visible = false;
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
            ctrlPriceRangeFilter.ExcludedQueryStringParams = productsPager.QueryStringProperty;
        }

        public int CurrentPageIndex
        {
            get
            {
                int _pageIndex = CommonHelper.QueryStringInt(productsPager.QueryStringProperty);
                _pageIndex--;
                if (_pageIndex < 0)
                    _pageIndex = 0;
                return _pageIndex;
            }
        }

        public int ManufacturerId
        {
            get
            {
                return CommonHelper.QueryStringInt("ManufacturerId");
            }
        }
    }
}
