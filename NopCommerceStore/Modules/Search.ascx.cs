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
using System.ComponentModel;
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
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using System.Collections.Generic;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class SearchControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!String.IsNullOrEmpty(this.SearchTerms))
                    txtSearchTerm.Text = this.SearchTerms;
                BindData();
                BindCategories();
                BindManufacturers();
            }
        }
       
        protected void BindData()
        {
            try
            {
                string keywords = txtSearchTerm.Text.Trim();

                if (!String.IsNullOrEmpty(keywords))
                {
                    int searchTermMinimumLength = SettingManager.GetSettingValueInteger("Search.ProductSearchTermMinimumLength", 3);
                    if (keywords.Length < searchTermMinimumLength)
                        throw new NopException(string.Format(LocalizationManager.GetLocaleResourceString("Search.SearchTermMinimumLengthIsNCharacters"), searchTermMinimumLength));

                    bool advSearch = cbAdvancedSearch.Checked;
                    int categoryId = 0;
                    int manufacturerId = 0;
                    decimal? minPriceConverted = null;
                    decimal? maxPriceConverted = null;
                    bool searchInProductDescriptions = false;
                    if (advSearch)
                    {
                        //categories
                        if (ddlCategories.Items.Count > 0)
                            categoryId = int.Parse(this.ddlCategories.SelectedItem.Value);

                        //manufacturers
                        if (ddlManufacturers.Items.Count > 0)
                            manufacturerId = int.Parse(this.ddlManufacturers.SelectedItem.Value);

                        //min price
                        decimal? minPrice = null;
                        try
                        {
                            if (!string.IsNullOrEmpty(txtPriceFrom.Text.Trim()))
                            {
                                minPrice = decimal.Parse(txtPriceFrom.Text.Trim());
                                if (minPrice.HasValue)
                                {
                                    minPriceConverted = CurrencyManager.ConvertCurrency(minPrice.Value, NopContext.Current.WorkingCurrency, CurrencyManager.PrimaryStoreCurrency);
                                }
                            }
                        }
                        catch
                        {
                            txtPriceFrom.Text = string.Empty;
                        }

                        //max price
                        decimal? maxPrice = null;
                        try
                        {
                            if (!string.IsNullOrEmpty(txtPriceTo.Text.Trim()))
                            {
                                maxPrice = decimal.Parse(txtPriceTo.Text.Trim());
                                if (maxPrice.HasValue)
                                {
                                    maxPriceConverted = CurrencyManager.ConvertCurrency(maxPrice.Value, NopContext.Current.WorkingCurrency, CurrencyManager.PrimaryStoreCurrency);
                                }
                            }
                        }
                        catch
                        {
                            txtPriceTo.Text = string.Empty;
                        }

                        //search in descriptions
                        searchInProductDescriptions = cbSearchInProductDescriptions.Checked;
                    }

                    int totalRecords = 0;
                    var products = ProductManager.GetAllProducts(categoryId,
                        manufacturerId, 0, null,
                        minPriceConverted, maxPriceConverted,
                        keywords, searchInProductDescriptions,
                        100, 0, new List<int>(), out totalRecords);

                    lvProducts.DataSource = products;
                    lvProducts.DataBind();
                    lvProducts.Visible = products.Count > 0;
                    pagerProducts.Visible = products.Count > pagerProducts.PageSize;
                    lblNoResults.Visible = !lvProducts.Visible;

                    int customerId = 0;
                    if (NopContext.Current.User != null)
                        customerId = NopContext.Current.User.CustomerId;
                    SearchLogManager.InsertSearchLog(txtSearchTerm.Text, customerId, DateTime.UtcNow);
                }
                else
                {
                    pagerProducts.Visible = false;
                    lvProducts.Visible = false;
                }
            }
            catch (Exception exc)
            {
                lvProducts.Visible = false;
                pagerProducts.Visible = false;
                lblError.Text = Server.HtmlEncode(exc.Message);
            }
        }

        protected void BindCategories()
        {
            var categories = CategoryManager.GetAllCategories();

            if (categories.Count > 0)
            {
                this.ddlCategories.Items.Clear();
                this.ddlCategories.Items.Add(new ListItem(GetLocaleResourceString("Search.AllCategories"), "0"));

                foreach (var category in categories)
                {
                    string catName = GetCategoryFullName(category);
                    ListItem item = new ListItem(catName, category.CategoryId.ToString());
                    this.ddlCategories.Items.Add(item);
                }

                this.ddlCategories.DataBind();
            }
            else
            {
                this.trCategories.Visible = false;
            }
        }
        
        protected string GetCategoryFullName(Category category)
        {
            string result = string.Empty;

            while (category != null && !category.Deleted)
            {
                if (String.IsNullOrEmpty(result))
                {
                    result = category.LocalizedName;
                }
                else
                {
                    result = "--" + result;
                }
                category = category.ParentCategory;
            }
            return result;
        }

        protected void BindManufacturers()
        {
            var manufacturers = ManufacturerManager.GetAllManufacturers();
            if (manufacturers.Count > 0)
            {
                this.ddlManufacturers.Items.Clear();
                this.ddlManufacturers.Items.Add(new ListItem(GetLocaleResourceString("Search.AllManufacturers"), "0"));

                foreach (var manufacturer in manufacturers)
                {
                    ListItem item2 = new ListItem(manufacturer.LocalizedName, manufacturer.ManufacturerId.ToString());
                    this.ddlManufacturers.Items.Add(item2);
                }
            }
            else
            {
                this.trManufacturers.Visible = false;
            }
        }

        protected void lvProducts_OnPagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            this.pagerProducts.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
            BindData();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();

            this.cbAdvancedSearch.Attributes.Add("onclick", "toggleAdvancedSearch();");
            this.txtSearchTerm.Attributes.Add("onkeydown", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + btnSearch.ClientID + "').click();return false;}} else {return true}; ");
            
            base.OnPreRender(e);
        }

        public string SearchTerms
        {
            get
            {
                return CommonHelper.QueryString("SearchTerms");
            }
        }
    }
}