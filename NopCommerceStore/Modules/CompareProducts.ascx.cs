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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CompareProductsControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnClearCompareProductsList_Click(object sender, EventArgs e)
        {
            ProductManager.ClearCompareProducts();
            Page.Response.Redirect(CommonHelper.GetStoreLocation());
        }

        protected override void OnInit(EventArgs e)
        {
            this.EnsureChildControls();
            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            if (!base.ChildControlsCreated)
            {
                this.GenerateCompareTable();
                base.CreateChildControls();
                base.ChildControlsCreated = true;
            }
        }

        private void btnRemoveFromList_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                ProductManager.RemoveProductFromCompareList(Convert.ToInt32(e.CommandArgument));
                Page.Response.Redirect("~/compareproducts.aspx");
            }
        }

        private HtmlTableCell AddCell(HtmlTableRow row, string text)
        {
            var cell = new HtmlTableCell();
            cell.InnerHtml = text;
            row.Cells.Add(cell);
            return cell;
        }

        protected void GenerateCompareTable()
        {
            this.tblCompareProducts.Rows.Clear();
            this.tblCompareProducts.Width = "100%";
            var compareProducts = ProductManager.GetCompareProducts();
            if (compareProducts.Count > 0)
            {
                var headerRow = new HtmlTableRow();
                this.AddCell(headerRow, "&nbsp;");
                var productNameRow = new HtmlTableRow();
                this.AddCell(productNameRow, "&nbsp;");
                var priceRow = new HtmlTableRow();
                var cell = new HtmlTableCell();
                cell.InnerText = GetLocaleResourceString("Products.CompareProductsPrice");
                cell.Align = "center";
                priceRow.Cells.Add(cell);

                var specificationAttributeIds = new List<int>();
                foreach (var product in compareProducts)
                {
                    var productSpecificationAttributes = SpecificationAttributeManager.GetProductSpecificationAttributesByProductId(product.ProductId, null, true);
                    foreach (var attribute in productSpecificationAttributes)
                        if (!specificationAttributeIds.Contains(attribute.SpecificationAttribute.SpecificationAttributeId))
                            specificationAttributeIds.Add(attribute.SpecificationAttribute.SpecificationAttributeId);
                }

                foreach (var product in compareProducts)
                {
                    var headerCell = new HtmlTableCell();
                    var headerCellDiv = new HtmlGenericControl("div");
                    var btnRemoveFromList = new Button();
                    btnRemoveFromList.ToolTip = GetLocaleResourceString("Products.CompareProductsRemoveFromList");
                    btnRemoveFromList.Text = GetLocaleResourceString("Products.CompareProductsRemoveFromList");
                    btnRemoveFromList.CommandName = "Remove";
                    btnRemoveFromList.Command += new CommandEventHandler(this.btnRemoveFromList_Command);
                    btnRemoveFromList.CommandArgument = product.ProductId.ToString();
                    btnRemoveFromList.CausesValidation = false;
                    btnRemoveFromList.CssClass = "remove-button";
                    btnRemoveFromList.ID = "btnRemoveFromList" + product.ProductId.ToString();
                    headerCellDiv.Controls.Add(btnRemoveFromList);

                    var productImagePanel = new HtmlGenericControl("p");
                    productImagePanel.Attributes.Add("align", "center");

                    var productImage = new HtmlImage();
                    productImage.Border = 0;
                    //productImage.Align = "center";
                    productImage.Alt = "Product image";
                    var picture = product.DefaultPicture;
                    if (picture != null)
                        productImage.Src = PictureManager.GetPictureUrl(picture, SettingManager.GetSettingValueInteger("Media.Product.ThumbnailImageSize", 125), true);
                    else
                        productImage.Src = PictureManager.GetDefaultPictureUrl(SettingManager.GetSettingValueInteger("Media.Product.ThumbnailImageSize", 125));
                    productImagePanel.Controls.Add(productImage);
                    headerCellDiv.Controls.Add(productImagePanel);
                    
                    headerCell.Controls.Add(headerCellDiv);
                    headerRow.Cells.Add(headerCell);
                    var productNameCell = new HtmlTableCell();
                    var productLink = new HyperLink();
                    productLink.Text = Server.HtmlEncode(product.LocalizedName);
                    productLink.NavigateUrl = SEOHelper.GetProductUrl(product);
                    productLink.Attributes.Add("class", "link");
                    productNameCell.Align = "center";
                    productNameCell.Controls.Add(productLink);
                    productNameRow.Cells.Add(productNameCell);
                    var priceCell = new HtmlTableCell();
                    priceCell.Align = "center";
                    var productVariantCollection = product.ProductVariants;
                    if (productVariantCollection.Count > 0)
                    {
                        var productVariant = productVariantCollection[0];
                        decimal taxRate = decimal.Zero;
                        decimal finalPriceWithoutDiscountBase = TaxManager.GetPrice(productVariant, PriceHelper.GetFinalPrice(productVariant, false), out taxRate);
                        decimal finalPriceWithoutDiscount = CurrencyManager.ConvertCurrency(finalPriceWithoutDiscountBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                        priceCell.InnerText = PriceHelper.FormatPrice(finalPriceWithoutDiscount);
                    }
                    priceRow.Cells.Add(priceCell);
                }
                productNameRow.Attributes.Add("class", "product-name");
                priceRow.Attributes.Add("class", "productPrice");
                this.tblCompareProducts.Rows.Add(headerRow);
                this.tblCompareProducts.Rows.Add(productNameRow);
                if (!SettingManager.GetSettingValueBoolean("Common.HidePricesForNonRegistered") ||
                    (NopContext.Current.User != null &&
                    !NopContext.Current.User.IsGuest))
                {
                    this.tblCompareProducts.Rows.Add(priceRow);
                }

                foreach (int specificationAttributeId in specificationAttributeIds)
                {
                    var specificationAttribute = SpecificationAttributeManager.GetSpecificationAttributeById(specificationAttributeId);
                    var productRow = new HtmlTableRow();
                    this.AddCell(productRow, Server.HtmlEncode(specificationAttribute.LocalizedName)).Align = "left";

                    foreach (var product2 in compareProducts)
                    {
                        var productCell = new HtmlTableCell();
                        {
                            var productSpecificationAttributes2 = SpecificationAttributeManager.GetProductSpecificationAttributesByProductId(product2.ProductId, null, true);
                            foreach (var psa2 in productSpecificationAttributes2)
                            {
                                if (specificationAttribute.SpecificationAttributeId == psa2.SpecificationAttribute.SpecificationAttributeId)
                                {
                                    productCell.InnerHtml = (!String.IsNullOrEmpty(psa2.SpecificationAttributeOption.LocalizedName)) ? Server.HtmlEncode(psa2.SpecificationAttributeOption.LocalizedName) : "&nbsp;";
                                }
                            }
                        }
                        productCell.Align = "center";
                        productCell.VAlign = "top";
                        productRow.Cells.Add(productCell);
                    }
                    this.tblCompareProducts.Rows.Add(productRow);
                }

                string width = Math.Round((decimal)(90M / compareProducts.Count), 0).ToString() + "%";
                for (int i = 0; i < this.tblCompareProducts.Rows.Count; i++)
                {
                    var row = this.tblCompareProducts.Rows[i];
                    for (int j = 1; j < row.Cells.Count; j++)
                    {
                        if (j == (row.Cells.Count - 1))
                        {
                            row.Cells[j].Style.Add("width", width);
                            row.Cells[j].Style.Add("text-align", "center");
                        }
                        else
                        {
                            row.Cells[j].Style.Add("width", width);
                            row.Cells[j].Style.Add("text-align", "center");
                        }
                    }
                }
            }
            else
            {
                btnClearCompareProductsList.Visible = false;
                tblCompareProducts.Visible = false;
            }
        }
    }
}