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
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Product product = ProductManager.GetProductById(this.ProductId);

            if (this.HasLocalizableContent)
            {
                var languages = this.GetLocalizableLanguagesSupported();
                rptrLanguageTabs.DataSource = languages;
                rptrLanguageTabs.DataBind();
                rptrLanguageDivs.DataSource = languages;
                rptrLanguageDivs.DataBind();
            }

            if (product != null)
            {
                this.txtName.Text = product.Name;
                this.txtShortDescription.Text = product.ShortDescription;
                this.txtFullDescription.Content = product.FullDescription;
                this.txtAdminComment.Text = product.AdminComment;
                CommonHelper.SelectListItem(this.ddlTemplate, product.TemplateId);
                this.cbShowOnHomePage.Checked = product.ShowOnHomePage;

                this.cbAllowCustomerReviews.Checked = product.AllowCustomerReviews;
                this.cbAllowCustomerRatings.Checked = product.AllowCustomerRatings;
                this.cbPublished.Checked = product.Published;

                var productReviews = product.ProductReviews;
                if (productReviews.Count > 0)
                {
                    hlViewReviews.Visible = true;
                    hlViewReviews.Text = string.Format(GetLocaleResourceString("Admin.ProductInfo.AllowCustomerReviewsView"), productReviews.Count);
                    hlViewReviews.NavigateUrl = CommonHelper.GetStoreAdminLocation() + "ProductReviews.aspx?ProductID=" + product.ProductId.ToString();
                }
                else
                    hlViewReviews.Visible = false;

                this.txtProductTags.Text = GenerateListOfProductTagss(ProductManager.GetAllProductTags(product.ProductId, string.Empty));
            }
        }

        private void FillDropDowns()
        {
            this.ddlTemplate.Items.Clear();
            var productTemplateCollection = TemplateManager.GetAllProductTemplates();
            foreach (ProductTemplate productTemplate in productTemplateCollection)
            {
                ListItem item2 = new ListItem(productTemplate.Name, productTemplate.ProductTemplateId.ToString());
                this.ddlTemplate.Items.Add(item2);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.FillDropDowns();
                this.BindData();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();
            BindJQueryIdTabs();

            base.OnPreRender(e);
        }

        private string GenerateListOfProductTagss(List<ProductTag> productTags)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < productTags.Count; i++)
            {
                ProductTag pt = productTags[i];
                result.Append(pt.Name.ToString());
                if (i != productTags.Count - 1)
                {
                    result.Append(", ");
                }
            }
            return result.ToString();
        }

        private string[] ParseProductTags(string productTags)
        {
            List<string> result = new List<string>();
            string[] values = productTags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string val1 in values)
            {
                if (!String.IsNullOrEmpty(val1.Trim()))
                {
                    result.Add(val1);
                }
            }
            return result.ToArray();
        }

        public Product SaveInfo()
        {
            Product product = ProductManager.GetProductById(this.ProductId);
            if (product != null)
            {
                product = ProductManager.UpdateProduct(product.ProductId, txtName.Text, txtShortDescription.Text, txtFullDescription.Content, txtAdminComment.Text,
                     int.Parse(this.ddlTemplate.SelectedItem.Value),
                     cbShowOnHomePage.Checked, product.MetaKeywords, product.MetaDescription,
                     product.MetaTitle, product.SEName, cbAllowCustomerReviews.Checked,
                     cbAllowCustomerRatings.Checked, product.RatingSum, product.TotalRatingVotes, cbPublished.Checked,
                     product.Deleted, product.CreatedOn, DateTime.UtcNow);

                SaveLocalizableContent(product);

                //product tags
                var productTags1 = ProductManager.GetAllProductTags(product.ProductId, string.Empty);
                foreach (var productTag in productTags1)
                {
                    ProductManager.RemoveProductTagMapping(product.ProductId, productTag.ProductTagId);
                }
                string[] productTagNames = ParseProductTags(txtProductTags.Text);
                foreach (string productTagName in productTagNames)
                {
                    ProductTag productTag = null;
                    var productTags2 = ProductManager.GetAllProductTags(0,
                        productTagName);
                    if (productTags2.Count == 0)
                    {
                        productTag = ProductManager.InsertProductTag(productTagName, 0);
                    }
                    else
                    {
                        productTag = productTags2[0];
                    }
                    ProductManager.AddProductTagMapping(product.ProductId, productTag.ProductTagId);
                }
            }

            return product;
        }

        protected void SaveLocalizableContent(Product product)
        {
            if (product == null)
                return;

            if (!this.HasLocalizableContent)
                return;

            foreach (RepeaterItem item in rptrLanguageDivs.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var txtLocalizedName = (TextBox)item.FindControl("txtLocalizedName");
                    var txtLocalizedShortDescription = (TextBox)item.FindControl("txtLocalizedShortDescription");
                    var txtLocalizedFullDescription = (AjaxControlToolkit.HTMLEditor.Editor)item.FindControl("txtLocalizedFullDescription");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string name = txtLocalizedName.Text;
                    string shortDescription = txtLocalizedShortDescription.Text;
                    string fullDescription = txtLocalizedFullDescription.Content;

                    bool allFieldsAreEmpty = (string.IsNullOrEmpty(name) &&
                        string.IsNullOrEmpty(shortDescription) &&
                        string.IsNullOrEmpty(fullDescription));

                    var content = ProductManager.GetProductLocalizedByProductIdAndLanguageId(product.ProductId, languageId);
                    if (content == null)
                    {
                        if (!allFieldsAreEmpty && languageId > 0)
                        {
                            //only insert if one of the fields are filled out (avoid too many empty records in db...)
                            content = ProductManager.InsertProductLocalized(product.ProductId,
                                   languageId, name, shortDescription, fullDescription,
                                   string.Empty, string.Empty, string.Empty, string.Empty);
                        }
                    }
                    else
                    {
                        if (languageId > 0)
                        {
                            content = ProductManager.UpdateProductLocalized(content.ProductLocalizedId, content.ProductId,
                                languageId, name, shortDescription, fullDescription,
                                content.MetaKeywords, content.MetaDescription,
                                content.MetaTitle, content.SEName);
                        }
                    }
                }
            }
        }

        protected void rptrLanguageDivs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var txtLocalizedName = (TextBox)e.Item.FindControl("txtLocalizedName");
                var txtLocalizedShortDescription = (TextBox)e.Item.FindControl("txtLocalizedShortDescription");
                var txtLocalizedFullDescription = (AjaxControlToolkit.HTMLEditor.Editor)e.Item.FindControl("txtLocalizedFullDescription");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");

                int languageId = int.Parse(lblLanguageId.Text);

                var content = ProductManager.GetProductLocalizedByProductIdAndLanguageId(this.ProductId, languageId);
                if (content != null)
                {
                    txtLocalizedName.Text = content.Name;
                    txtLocalizedShortDescription.Text = content.ShortDescription;
                    txtLocalizedFullDescription.Content = content.FullDescription;
                }
            }
        }

        public int ProductId
        {
            get
            {
                return CommonHelper.QueryStringInt("ProductId");
            }
        }
    }
}