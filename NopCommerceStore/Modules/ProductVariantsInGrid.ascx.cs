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
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.SEO;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ProductVariantsInGridControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                BindData();
        }

        protected void BindData()
        {
            var product = ProductManager.GetProductById(this.ProductId);
            if (product != null)
            {
                var productVariants = product.ProductVariants;
                if (productVariants.Count > 0)
                {
                    rptVariants.DataSource = productVariants;
                    rptVariants.DataBind();
                }
                else
                    this.Visible = false;
            }
            else
                this.Visible = false;
        }

        protected void rptVariants_OnItemCommand(Object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AddToCart" || e.CommandName == "AddToWishlist")
            {
                var txtQuantity = e.Item.FindControl("txtQuantity") as NumericTextBox;
                var txtCustomerEnteredPrice = e.Item.FindControl("txtCustomerEnteredPrice") as DecimalTextBox;
                var productVariantId = e.Item.FindControl("ProductVariantId") as Label;
                var ctrlProductAttributes = e.Item.FindControl("ctrlProductAttributes") as ProductAttributesControl;
                var ctrlGiftCardAttributes = e.Item.FindControl("ctrlGiftCardAttributes") as GiftCardAttributesControl;                
                var lblError = e.Item.FindControl("lblError") as Label;

                var pv = ProductManager.GetProductVariantById(Convert.ToInt32(productVariantId.Text));
                if (pv == null)
                    return;

                string attributes = ctrlProductAttributes.SelectedAttributes;
                decimal customerEnteredPrice = txtCustomerEnteredPrice.Value;
                decimal customerEnteredPriceConverted = CurrencyManager.ConvertCurrency(customerEnteredPrice, NopContext.Current.WorkingCurrency, CurrencyManager.PrimaryStoreCurrency);
                int quantity = txtQuantity.Value;

                //gift cards
                if (pv.IsGiftCard)
                {
                    string recipientName = ctrlGiftCardAttributes.RecipientName;
                    string recipientEmail = ctrlGiftCardAttributes.RecipientEmail;
                    string senderName = ctrlGiftCardAttributes.SenderName;
                    string senderEmail = ctrlGiftCardAttributes.SenderEmail;
                    string giftCardMessage = ctrlGiftCardAttributes.GiftCardMessage;

                    attributes = ProductAttributeHelper.AddGiftCardAttribute(attributes,
                        recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
                }

                try
                {
                    if (e.CommandName == "AddToCart")
                    {
                        string sep = "<br />";
                        List<string> addToCartWarnings = ShoppingCartManager.AddToCart(
                            ShoppingCartTypeEnum.ShoppingCart,
                            pv.ProductVariantId, 
                            attributes,
                            customerEnteredPriceConverted,
                            quantity);
                        if (addToCartWarnings.Count == 0)
                        {
                            if (SettingManager.GetSettingValueBoolean("Display.Products.DisplayCartAfterAddingProduct"))
                            {
                                //redirect to shopping cart page
                                Response.Redirect(SEOHelper.GetShoppingCartUrl());
                            }
                            else
                            {
                                //display notification message
                                this.DisplayAlertMessage(GetLocaleResourceString("Products.ProductHasBeenAddedToTheCart"));
                            }
                        }
                        else
                        {
                            StringBuilder addToCartWarningsSb = new StringBuilder();
                            for (int i = 0; i < addToCartWarnings.Count; i++)
                            {
                                addToCartWarningsSb.Append(Server.HtmlEncode(addToCartWarnings[i]));
                                if (i != addToCartWarnings.Count - 1)
                                {
                                    addToCartWarningsSb.Append(sep);
                                }
                            }
                            string errorFull = addToCartWarningsSb.ToString();
                            lblError.Text = errorFull;
                            if (SettingManager.GetSettingValueBoolean("Common.ShowAlertForProductAttributes"))
                            {
                                this.DisplayAlertMessage(errorFull.Replace(sep, "\\n"));
                            }
                        }
                    }

                    if (e.CommandName == "AddToWishlist")
                    {
                        string sep = "<br />";
                        var addToCartWarnings = ShoppingCartManager.AddToCart(
                            ShoppingCartTypeEnum.Wishlist,
                            pv.ProductVariantId, 
                            attributes,
                            customerEnteredPriceConverted,
                            quantity);
                        if (addToCartWarnings.Count == 0)
                        {
                            Response.Redirect(SEOHelper.GetWishlistUrl());
                        }
                        else
                        {
                            var addToCartWarningsSb = new StringBuilder();
                            for (int i = 0; i < addToCartWarnings.Count; i++)
                            {
                                addToCartWarningsSb.Append(Server.HtmlEncode(addToCartWarnings[i]));
                                if (i != addToCartWarnings.Count - 1)
                                {
                                    addToCartWarningsSb.Append(sep);
                                }
                            } 
                            string errorFull = addToCartWarningsSb.ToString();
                            lblError.Text = errorFull;
                            if (SettingManager.GetSettingValueBoolean("Common.ShowAlertForProductAttributes"))
                            {
                                this.DisplayAlertMessage(errorFull.Replace(sep, "\\n"));
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    LogManager.InsertLog(LogTypeEnum.CustomerError, exc.Message, exc);
                    lblError.Text = Server.HtmlEncode(exc.Message);
                }
            }
        }

        protected void rptVariants_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var productVariant = e.Item.DataItem as ProductVariant;
                var pnlDownloadSample = e.Item.FindControl("pnlDownloadSample") as Panel;
                var hlDownloadSample = e.Item.FindControl("hlDownloadSample") as HyperLink;
                var iProductVariantPicture = e.Item.FindControl("iProductVariantPicture") as Image;
                var pnlStockAvailablity = e.Item.FindControl("pnlStockAvailablity") as Panel;
                var lblStockAvailablity = e.Item.FindControl("lblStockAvailablity") as Label;
                var phSKU = e.Item.FindControl("phSKU") as PlaceHolder;
                var lSKU = e.Item.FindControl("lSKU") as Literal;
                var txtCustomerEnteredPrice = e.Item.FindControl("txtCustomerEnteredPrice") as DecimalTextBox;
                var txtQuantity = e.Item.FindControl("txtQuantity") as NumericTextBox;
                var btnAddToCart = e.Item.FindControl("btnAddToCart") as Button;
                var btnAddToWishlist = e.Item.FindControl("btnAddToWishlist") as Button;

                if (iProductVariantPicture != null)
                {
                    var productVariantPicture = productVariant.Picture;
                    string pictureUrl = PictureManager.GetPictureUrl(productVariantPicture, SettingManager.GetSettingValueInteger("Media.Product.VariantImageSize", 125), false);
                    if (String.IsNullOrEmpty(pictureUrl))
                        iProductVariantPicture.Visible = false;
                    else
                        iProductVariantPicture.ImageUrl = pictureUrl;
                    iProductVariantPicture.ToolTip = String.Format(GetLocaleResourceString("Media.Product.ImageAlternateTextFormat"), productVariant.LocalizedName);
                    iProductVariantPicture.AlternateText = String.Format(GetLocaleResourceString("Media.Product.ImageAlternateTextFormat"), productVariant.LocalizedName);
                }

                btnAddToWishlist.Visible = SettingManager.GetSettingValueBoolean("Common.EnableWishlist");

                //stock
                if (pnlStockAvailablity != null && lblStockAvailablity != null)
                {
                    string stockMessage = PriceHelper.FormatStockMessage(productVariant);
                    if (!String.IsNullOrEmpty(stockMessage))
                    {
                        lblStockAvailablity.Text = stockMessage;
                    }
                    else
                    {
                        pnlStockAvailablity.Visible = false;
                    }
                }

                //sku
                if (SettingManager.GetSettingValueBoolean("Display.Products.ShowSKU") &&
                    !String.IsNullOrEmpty(productVariant.SKU))
                {
                    phSKU.Visible = true;
                    lSKU.Text = Server.HtmlEncode(productVariant.SKU);
                }
                else
                {
                    phSKU.Visible = false;
                }
                
                //price entered by a customer
                if (productVariant.CustomerEntersPrice)
                {
                    int minimumCustomerEnteredPrice = Convert.ToInt32(Math.Ceiling(CurrencyManager.ConvertCurrency(productVariant.MinimumCustomerEnteredPrice, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency)));
                    int maximumCustomerEnteredPrice = Convert.ToInt32(Math.Truncate(CurrencyManager.ConvertCurrency(productVariant.MaximumCustomerEnteredPrice, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency)));
                    txtCustomerEnteredPrice.Visible = true;
                    txtCustomerEnteredPrice.ValidationGroup = string.Format("ProductVariant{0}", productVariant.ProductVariantId);
                    txtCustomerEnteredPrice.Value = minimumCustomerEnteredPrice;
                    txtCustomerEnteredPrice.MinimumValue = minimumCustomerEnteredPrice.ToString();
                    txtCustomerEnteredPrice.MaximumValue = maximumCustomerEnteredPrice.ToString();
                    txtCustomerEnteredPrice.RangeErrorMessage = string.Format(GetLocaleResourceString("Products.CustomerEnteredPrice.Range"), minimumCustomerEnteredPrice, maximumCustomerEnteredPrice);
                }
                else
                {
                    txtCustomerEnteredPrice.Visible = false;
                }

                //buttons
                if (!productVariant.DisableBuyButton)
                {
                    txtQuantity.ValidationGroup = string.Format("ProductVariant{0}", productVariant.ProductVariantId);
                    btnAddToCart.ValidationGroup = string.Format("ProductVariant{0}", productVariant.ProductVariantId);
                    btnAddToWishlist.ValidationGroup = string.Format("ProductVariant{0}", productVariant.ProductVariantId);

                    txtQuantity.Value = productVariant.OrderMinimumQuantity;
                }
                else
                {
                    txtQuantity.Visible = false;
                    btnAddToCart.Visible = false;
                    btnAddToWishlist.Visible = false;
                }

                //sample downloads
                if (pnlDownloadSample != null && hlDownloadSample != null)
                {
                    if (productVariant.IsDownload && productVariant.HasSampleDownload)
                    {
                        pnlDownloadSample.Visible = true;
                        hlDownloadSample.NavigateUrl = DownloadManager.GetSampleDownloadUrl(productVariant);
                    }
                    else
                    {
                        pnlDownloadSample.Visible = false;
                    }
                }

                //final check - hide prices for non-registered customers
                if (!SettingManager.GetSettingValueBoolean("Common.HidePricesForNonRegistered") ||
                        (NopContext.Current.User != null &&
                        !NopContext.Current.User.IsGuest))
                {
                    //
                }
                else
                {
                    txtCustomerEnteredPrice.Visible = false;
                    txtQuantity.Visible = false;
                    btnAddToCart.Visible = false;
                    btnAddToWishlist.Visible = false;
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