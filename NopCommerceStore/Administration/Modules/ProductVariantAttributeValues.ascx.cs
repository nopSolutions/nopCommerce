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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ProductVariantAttributeValuesControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            ProductVariantAttribute productVariantAttribute = ProductAttributeManager.GetProductVariantAttributeById(this.ProductVariantAttributeId);
            if (productVariantAttribute != null)
            {
                if (this.HasLocalizableContent)
                {
                    var languages = this.GetLocalizableLanguagesSupported();
                    rptrLanguageTabs.DataSource = languages;
                    rptrLanguageTabs.DataBind();
                    rptrLanguageDivs.DataSource = languages;
                    rptrLanguageDivs.DataBind();
                }

                ProductVariant productVariant = productVariantAttribute.ProductVariant;
                if (productVariant == null)
                    Response.Redirect("Products.aspx");
                ProductAttribute productAttribute = productVariantAttribute.ProductAttribute;
                if (productAttribute == null)
                    Response.Redirect("Products.aspx");

                this.lblTitle.Text = string.Format(GetLocaleResourceString("Admin.ProductVariantAttributeValues.AddEdit"), Server.HtmlEncode(productAttribute.Name), Server.HtmlEncode(productVariant.FullProductName));
                this.hlProductURL.NavigateUrl = CommonHelper.GetStoreAdminLocation() + "ProductVariantDetails.aspx?ProductVariantID=" + productVariant.ProductVariantId;

                var productVariantAttributeValues = ProductAttributeManager.GetProductVariantAttributeValues(productVariantAttribute.ProductVariantAttributeId);
                if (productVariantAttributeValues.Count > 0)
                {
                    this.gvProductVariantAttributeValues.Visible = true;
                    this.gvProductVariantAttributeValues.DataSource = productVariantAttributeValues;
                    this.gvProductVariantAttributeValues.DataBind();
                }
                else
                    this.gvProductVariantAttributeValues.Visible = false;
            }
            else
                Response.Redirect("Products.aspx");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                gvProductVariantAttributeValues.Columns[1].HeaderText = string.Format("{0} [{1}]", GetLocaleResourceString("Admin.ProductVariantAttributeValues.PriceAdjustment"), CurrencyManager.PrimaryStoreCurrency.CurrencyCode);
                this.BindData();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();
            BindJQueryIdTabs();

            base.OnPreRender(e);
        }
        
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ProductVariantAttribute productVariantAttribute = ProductAttributeManager.GetProductVariantAttributeById(this.ProductVariantAttributeId);
                if (productVariantAttribute != null)
                {
                    ProductVariantAttributeValue productVariantAttributeValue = ProductAttributeManager.InsertProductVariantAttributeValue(productVariantAttribute.ProductVariantAttributeId,
                        txtNewName.Text, txtNewPriceAdjustment.Value, txtNewWeightAdjustment.Value,
                        cbNewIsPreSelected.Checked, txtNewDisplayOrder.Value);

                    SaveLocalizableContent(productVariantAttributeValue);

                    BindData();

                    txtNewName.Text = string.Empty;
                    txtNewPriceAdjustment.Value = 0;
                    txtNewWeightAdjustment.Value = 0;
                    txtNewDisplayOrder.Value = 1;
                }
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected void SaveLocalizableContent(ProductVariantAttributeValue pvav)
        {
            if (pvav == null)
                return;

            if (!this.HasLocalizableContent)
                return;

            foreach (RepeaterItem item in rptrLanguageDivs.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var txtNewLocalizedName = (TextBox)item.FindControl("txtNewLocalizedName");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string name = txtNewLocalizedName.Text;

                    bool allFieldsAreEmpty = string.IsNullOrEmpty(name);

                    var content = ProductAttributeManager.GetProductVariantAttributeValueLocalizedByProductVariantAttributeValueIdAndLanguageId(pvav.ProductVariantAttributeValueId, languageId);
                    if (content == null)
                    {
                        if (!allFieldsAreEmpty && languageId > 0)
                        {
                            //only insert if one of the fields are filled out (avoid too many empty records in db...)
                            content = ProductAttributeManager.InsertProductVariantAttributeValueLocalized(pvav.ProductVariantAttributeValueId,
                                   languageId, name);
                        }
                    }
                    else
                    {
                        if (languageId > 0)
                        {
                            content = ProductAttributeManager.UpdateProductVariantAttributeValueLocalized(content.ProductVariantAttributeValueLocalizedId, 
                                content.ProductVariantAttributeValueId, languageId, name);
                        }
                    }
                }
            }
        }

        protected void SaveLocalizableContentGrid(ProductVariantAttributeValue pvav)
        {
            if (pvav == null)
                return;

            if (!this.HasLocalizableContent)
                return;

            foreach (GridViewRow row in gvProductVariantAttributeValues.Rows)
            {
                Repeater rptrLanguageDivs2 = row.FindControl("rptrLanguageDivs2") as Repeater;
                if (rptrLanguageDivs2 != null)
                {
                    HiddenField hfProductVariantAttributeValueId = row.FindControl("hfProductVariantAttributeValueId") as HiddenField;
                    int pvavId = int.Parse(hfProductVariantAttributeValueId.Value);
                    if (pvavId == pvav.ProductVariantAttributeValueId)
                    {
                        foreach (RepeaterItem item in rptrLanguageDivs2.Items)
                        {
                            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                            {
                                var txtLocalizedName = (TextBox)item.FindControl("txtLocalizedName");
                                var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                                int languageId = int.Parse(lblLanguageId.Text);
                                string name = txtLocalizedName.Text;

                                bool allFieldsAreEmpty = string.IsNullOrEmpty(name);

                                var content = ProductAttributeManager.GetProductVariantAttributeValueLocalizedByProductVariantAttributeValueIdAndLanguageId(pvav.ProductVariantAttributeValueId, languageId);
                                if (content == null)
                                {
                                    if (!allFieldsAreEmpty && languageId > 0)
                                    {
                                        //only insert if one of the fields are filled out (avoid too many empty records in db...)
                                        content = ProductAttributeManager.InsertProductVariantAttributeValueLocalized(pvav.ProductVariantAttributeValueId,
                                            languageId, name);
                                    }
                                }
                                else
                                {
                                    if (languageId > 0)
                                    {
                                        content = ProductAttributeManager.UpdateProductVariantAttributeValueLocalized(content.ProductVariantAttributeValueLocalizedId,
                                            content.ProductVariantAttributeValueId, languageId, name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void rptrLanguageDivs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            
        }

        protected void gvProductVariantAttributeValues_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "UpdateProductVariantAttributeValue")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvProductVariantAttributeValues.Rows[index];

                HiddenField hfProductVariantAttributeValueId = row.FindControl("hfProductVariantAttributeValueId") as HiddenField;
                SimpleTextBox txtName = row.FindControl("txtName") as SimpleTextBox;
                DecimalTextBox txtPriceAdjustment = row.FindControl("txtPriceAdjustment") as DecimalTextBox;
                DecimalTextBox txtWeightAdjustment = row.FindControl("txtWeightAdjustment") as DecimalTextBox;
                CheckBox cbIsPreSelected = row.FindControl("cbIsPreSelected") as CheckBox;
                NumericTextBox txtDisplayOrder = row.FindControl("txtDisplayOrder") as NumericTextBox;

                int productVariantAttributeValueId = int.Parse(hfProductVariantAttributeValueId.Value);
                string name = txtName.Text;
                decimal priceAdjustment = txtPriceAdjustment.Value;
                decimal weightAdjustment = txtWeightAdjustment.Value;
                bool isPreSelected = cbIsPreSelected.Checked;
                int displayOrder = txtDisplayOrder.Value;

                ProductVariantAttributeValue productVariantAttributeValue = ProductAttributeManager.GetProductVariantAttributeValueById(productVariantAttributeValueId);

                if (productVariantAttributeValue != null)
                {
                    productVariantAttributeValue = ProductAttributeManager.UpdateProductVariantAttributeValue(productVariantAttributeValue.ProductVariantAttributeValueId,
                        productVariantAttributeValue.ProductVariantAttributeId, name,
                        priceAdjustment, weightAdjustment, isPreSelected, displayOrder);

                    SaveLocalizableContentGrid(productVariantAttributeValue);
                }
                BindData();
            }
        }
        
        protected void gvProductVariantAttributeValues_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int productVariantAttributeValueId = (int)gvProductVariantAttributeValues.DataKeys[e.RowIndex]["ProductVariantAttributeValueId"];
            ProductAttributeManager.DeleteProductVariantAttributeValue(productVariantAttributeValueId);
            BindData();
        }

        protected void gvProductVariantAttributeValues_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ProductVariantAttributeValue productVariantAttributeValue = (ProductVariantAttributeValue)e.Row.DataItem;

                Button btnUpdate = e.Row.FindControl("btnUpdate") as Button;
                if (btnUpdate != null)
                    btnUpdate.CommandArgument = e.Row.RowIndex.ToString();

                Repeater rptrLanguageDivs2 = e.Row.FindControl("rptrLanguageDivs2") as Repeater;
                if (rptrLanguageDivs2 != null)
                {
                    if (this.HasLocalizableContent)
                    {
                        var languages = this.GetLocalizableLanguagesSupported();
                        rptrLanguageDivs2.DataSource = languages;
                        rptrLanguageDivs2.DataBind();
                    }
                }
            }
        }

        protected void rptrLanguageDivs2_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var txtLocalizedName = (TextBox)e.Item.FindControl("txtLocalizedName");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");
                var hfProductVariantAttributeValueId = (HiddenField)e.Item.Parent.Parent.FindControl("hfProductVariantAttributeValueId");

                int languageId = int.Parse(lblLanguageId.Text);
                int pvavId = Convert.ToInt32(hfProductVariantAttributeValueId.Value);
                ProductVariantAttributeValue pvav = ProductAttributeManager.GetProductVariantAttributeValueById(pvavId);
                if (pvav != null)
                {
                    var content = ProductAttributeManager.GetProductVariantAttributeValueLocalizedByProductVariantAttributeValueIdAndLanguageId(pvavId, languageId);
                    if (content != null)
                    {
                        txtLocalizedName.Text = content.Name;
                    }
                }
            }
        }

        public int ProductVariantAttributeId
        {
            get
            {
                return CommonHelper.QueryStringInt("ProductVariantAttributeId");
            }
        }
    }
}