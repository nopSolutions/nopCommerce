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
using FredCK.FCKeditorV2;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Product product = IoC.Resolve<IProductService>().GetProductById(this.ProductId);

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
                this.txtFullDescription.Value = product.FullDescription;
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

                this.txtProductTags.Text = GenerateListOfProductTagss(IoC.Resolve<IProductService>().GetAllProductTags(product.ProductId, string.Empty));
            }
        }

        private void FillDropDowns()
        {
            this.ddlTemplate.Items.Clear();
            var productTemplateCollection = IoC.Resolve<ITemplateService>().GetAllProductTemplates();
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
            Product product = IoC.Resolve<IProductService>().GetProductById(this.ProductId);
            if (product != null)
            {
                product.Name = txtName.Text;
                product.ShortDescription = txtShortDescription.Text;
                product.FullDescription = txtFullDescription.Value;
                product.AdminComment = txtAdminComment.Text;
                product.TemplateId = int.Parse(this.ddlTemplate.SelectedItem.Value);
                product.ShowOnHomePage = cbShowOnHomePage.Checked;
                product.AllowCustomerReviews = cbAllowCustomerReviews.Checked;
                product.AllowCustomerRatings = cbAllowCustomerRatings.Checked;
                product.Published = cbPublished.Checked;
                product.UpdatedOn = DateTime.UtcNow;

                IoC.Resolve<IProductService>().UpdateProduct(product);

                SaveLocalizableContent(product);

                //product tags
                var productTags1 = IoC.Resolve<IProductService>().GetAllProductTags(product.ProductId, string.Empty);
                foreach (var productTag in productTags1)
                {
                    IoC.Resolve<IProductService>().RemoveProductTagMapping(product.ProductId, productTag.ProductTagId);
                }
                string[] productTagNames = ParseProductTags(txtProductTags.Text);
                foreach (string productTagName in productTagNames)
                {
                    ProductTag productTag = null;
                    var productTags2 = IoC.Resolve<IProductService>().GetAllProductTags(0,
                        productTagName);
                    if (productTags2.Count == 0)
                    {
                        productTag = new ProductTag()
                        {
                            Name = productTagName,
                            ProductCount = 0
                        };
                        IoC.Resolve<IProductService>().InsertProductTag(productTag);
                    }
                    else
                    {
                        productTag = productTags2[0];
                    }
                    IoC.Resolve<IProductService>().AddProductTagMapping(product.ProductId, productTag.ProductTagId);
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
                    var txtLocalizedFullDescription = (FCKeditor)item.FindControl("txtLocalizedFullDescription");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string name = txtLocalizedName.Text;
                    string shortDescription = txtLocalizedShortDescription.Text;
                    string fullDescription = txtLocalizedFullDescription.Value;

                    bool allFieldsAreEmpty = (string.IsNullOrEmpty(name) &&
                        string.IsNullOrEmpty(shortDescription) &&
                        string.IsNullOrEmpty(fullDescription));

                    var content = IoC.Resolve<IProductService>().GetProductLocalizedByProductIdAndLanguageId(product.ProductId, languageId);
                    if (content == null)
                    {
                        if (!allFieldsAreEmpty && languageId > 0)
                        {
                            //only insert if one of the fields are filled out (avoid too many empty records in db...)
                            content = new ProductLocalized()
                            {
                                ProductId = product.ProductId,
                                LanguageId = languageId,
                                Name = name,
                                ShortDescription = shortDescription,
                                FullDescription = fullDescription
                            };
                            IoC.Resolve<IProductService>().InsertProductLocalized(content);
                        }
                    }
                    else
                    {
                        if (languageId > 0)
                        {
                            content.LanguageId = languageId;
                            content.Name = name;
                            content.ShortDescription = shortDescription;
                            content.FullDescription = fullDescription;
                            IoC.Resolve<IProductService>().UpdateProductLocalized(content);
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
                var txtLocalizedFullDescription = (FCKeditor)e.Item.FindControl("txtLocalizedFullDescription");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");

                int languageId = int.Parse(lblLanguageId.Text);

                var content = IoC.Resolve<IProductService>().GetProductLocalizedByProductIdAndLanguageId(this.ProductId, languageId);
                if (content != null)
                {
                    txtLocalizedName.Text = content.Name;
                    txtLocalizedShortDescription.Text = content.ShortDescription;
                    txtLocalizedFullDescription.Value = content.FullDescription;
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