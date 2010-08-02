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
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Media;


namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class WishlistControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Visible = SettingManager.GetSettingValueBoolean("Common.EnableWishlist");
        }

        public void BindData()
        {
            var cart = ShoppingCartManager.GetShoppingCartByCustomerSessionGuid(ShoppingCartTypeEnum.Wishlist, this.CustomerSessionGuid);

            if (cart.Count > 0)
            {
                pnlEmptyCart.Visible = false;
                pnlCart.Visible = true;

                rptShoppingCart.DataSource = cart;
                rptShoppingCart.DataBind();
                ValidateWishlist();
                ValidateWishlistItems();
            }
            else
            {
                pnlEmptyCart.Visible = true;
                pnlCart.Visible = false;
            }
        }
        
        /// <summary>
        /// Validates shopping cart
        /// </summary>
        /// <returns>Indicates whether there're some warnings/errors</returns>
        protected bool ValidateWishlist()
        {
            bool hasErrors = false;

            //shopping cart
            var cart = ShoppingCartManager.GetCurrentShoppingCart(ShoppingCartTypeEnum.Wishlist);
            var warnings = ShoppingCartManager.GetShoppingCartWarnings(cart, string.Empty, false);
            if (warnings.Count > 0)
            {
                hasErrors = true;
                pnlCommonWarnings.Visible = true;
                lblCommonWarning.Visible = true;

                StringBuilder scWarningsSb = new StringBuilder();
                for (int i = 0; i < warnings.Count; i++)
                {
                    scWarningsSb.Append(Server.HtmlEncode(warnings[i]));
                    if (i != warnings.Count - 1)
                    {
                        scWarningsSb.Append("<br />");
                    }
                }

                lblCommonWarning.Text = scWarningsSb.ToString();
            }
            else
            {
                pnlCommonWarnings.Visible = false;
                lblCommonWarning.Visible = false;
            }

            return hasErrors;
        }

        /// <summary>
        /// Validates wishlist
        /// </summary>
        /// <returns>Indicates whether there're some warnings/errors</returns>
        protected bool ValidateWishlistItems()
        {
            bool hasErrors = false;
            foreach (RepeaterItem item in rptShoppingCart.Items)
            {
                var txtQuantity = item.FindControl("txtQuantity") as TextBox;
                var lblShoppingCartItemId = item.FindControl("lblShoppingCartItemId") as Label;
                var cbRemoveFromCart = item.FindControl("cbRemoveFromCart") as CheckBox;
                var pnlWarnings = item.FindControl("pnlWarnings") as Panel;
                var lblWarning = item.FindControl("lblWarning") as Label;

                int shoppingCartItemId = 0;
                int quantity = 0;
                if (txtQuantity != null && lblShoppingCartItemId != null && cbRemoveFromCart != null)
                {
                    int.TryParse(lblShoppingCartItemId.Text, out shoppingCartItemId);
                    if (!cbRemoveFromCart.Checked)
                    {
                        int.TryParse(txtQuantity.Text, out quantity);
                        var sci = ShoppingCartManager.GetShoppingCartItemById(shoppingCartItemId);

                        var warnings = ShoppingCartManager.GetShoppingCartItemWarnings(
                            sci.ShoppingCartType, 
                            sci.ProductVariantId, 
                            sci.AttributesXml, 
                            sci.CustomerEnteredPrice,
                            quantity);

                        if (warnings.Count > 0)
                        {
                            hasErrors = true;
                            if (pnlWarnings != null && lblWarning != null)
                            {
                                pnlWarnings.Visible = true;
                                lblWarning.Visible = true;

                                var addToCartWarningsSb = new StringBuilder();
                                for (int i = 0; i < warnings.Count; i++)
                                {
                                    addToCartWarningsSb.Append(Server.HtmlEncode(warnings[i]));
                                    if (i != warnings.Count - 1)
                                    {
                                        addToCartWarningsSb.Append("<br />");
                                    }
                                }

                                lblWarning.Text = addToCartWarningsSb.ToString();
                            }
                        }
                    }
                }
            }

            return hasErrors;
        }

        protected void UpdateWishlist()
        {
            if (!IsEditable)
                return;

            bool hasErrors = ValidateWishlistItems();

            if (!hasErrors)
            {
                foreach (RepeaterItem item in rptShoppingCart.Items)
                {
                    var txtQuantity = item.FindControl("txtQuantity") as TextBox;
                    var lblShoppingCartItemId = item.FindControl("lblShoppingCartItemId") as Label;
                    var cbRemoveFromCart = item.FindControl("cbRemoveFromCart") as CheckBox;

                    int shoppingCartItemId = 0;
                    int quantity = 0;
                    if (txtQuantity != null && lblShoppingCartItemId != null && cbRemoveFromCart != null)
                    {
                        int.TryParse(lblShoppingCartItemId.Text, out shoppingCartItemId);
                        if (cbRemoveFromCart.Checked)
                            ShoppingCartManager.DeleteShoppingCartItem(shoppingCartItemId, false);
                        else
                        {
                            int.TryParse(txtQuantity.Text, out quantity);
                            List<string> addToCartWarning = ShoppingCartManager.UpdateCart(shoppingCartItemId, quantity, false);
                        }
                    }
                }

                Response.Redirect(SEOHelper.GetWishlistUrl());
            }
        }

        public string GetProductVariantName(ShoppingCartItem shoppingCartItem)
        {
            var productVariant = shoppingCartItem.ProductVariant;
            if (productVariant != null)
                return productVariant.LocalizedFullProductName;
            return "Not available";
        }

        public string GetProductVariantImageUrl(ShoppingCartItem shoppingCartItem)
        {
            string pictureUrl = String.Empty;
            var productVariant = shoppingCartItem.ProductVariant;
            if (productVariant != null)
            {
                var productVariantPicture = productVariant.Picture;
                pictureUrl = PictureManager.GetPictureUrl(productVariantPicture, SettingManager.GetSettingValueInteger("Media.ShoppingCart.ThumbnailImageSize", 80), false);
                if (String.IsNullOrEmpty(pictureUrl))
                {
                    var product = productVariant.Product;
                    var picture = product.DefaultPicture;
                    if (picture != null)
                    {
                        pictureUrl = PictureManager.GetPictureUrl(picture, SettingManager.GetSettingValueInteger("Media.ShoppingCart.ThumbnailImageSize", 80));
                    }
                    else
                    {
                        pictureUrl = PictureManager.GetDefaultPictureUrl(SettingManager.GetSettingValueInteger("Media.ShoppingCart.ThumbnailImageSize", 80));
                    }
                }
            }
            return pictureUrl;
        }

        public string GetProductUrl(ShoppingCartItem shoppingCartItem)
        {
            var productVariant = shoppingCartItem.ProductVariant;
            if (productVariant != null)
                return SEOHelper.GetProductUrl(productVariant.ProductId);
            return string.Empty;
        }

        public string GetAttributeDescription(ShoppingCartItem shoppingCartItem)
        {
            string result = ProductAttributeHelper.FormatAttributes(shoppingCartItem.ProductVariant, shoppingCartItem.AttributesXml);
            if (!String.IsNullOrEmpty(result))
                result = "<br />" + result;
            return result;
        }
        
        public string GetRecurringDescription(ShoppingCartItem shoppingCartItem)
        {
            string result = string.Empty;
            if (shoppingCartItem.ProductVariant.IsRecurring)
            {
                result = string.Format(GetLocaleResourceString("Wishlist.RecurringPeriod"), shoppingCartItem.ProductVariant.CycleLength, ((RecurringProductCyclePeriodEnum)shoppingCartItem.ProductVariant.CyclePeriod).ToString());
                if (!String.IsNullOrEmpty(result))
                    result = "<br />" + result;
            }
            return result;
        }

        public string GetShoppingCartItemUnitPriceString(ShoppingCartItem shoppingCartItem)
        {
            var sb = new StringBuilder();
            if (shoppingCartItem.ProductVariant.CallForPrice)
            {
                sb.Append("<span class=\"productPrice\">");
                sb.Append(GetLocaleResourceString("Products.CallForPrice"));
                sb.Append("</span>");
            }
            else
            {
                decimal taxRate = decimal.Zero;
                decimal shoppingCartUnitPriceWithDiscountBase = TaxManager.GetPrice(shoppingCartItem.ProductVariant, PriceHelper.GetUnitPrice(shoppingCartItem, true), out taxRate);
                decimal shoppingCartUnitPriceWithDiscount = CurrencyManager.ConvertCurrency(shoppingCartUnitPriceWithDiscountBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                string unitPriceString = PriceHelper.FormatPrice(shoppingCartUnitPriceWithDiscount);

                sb.Append("<span class=\"productPrice\">");
                sb.Append(unitPriceString);
                sb.Append("</span>");
            }
            return sb.ToString();
        }

        public string GetShoppingCartItemSubTotalString(ShoppingCartItem shoppingCartItem)
        {
            var sb = new StringBuilder();
            if (shoppingCartItem.ProductVariant.CallForPrice)
            {
                sb.Append("<span class=\"productPrice\">");
                sb.Append(GetLocaleResourceString("Products.CallForPrice"));
                sb.Append("</span>");
            }
            else
            {
                //sub total
                decimal taxRate = decimal.Zero;
                decimal shoppingCartItemSubTotalWithDiscountBase = TaxManager.GetPrice(shoppingCartItem.ProductVariant, PriceHelper.GetSubTotal(shoppingCartItem, true), out taxRate);
                decimal shoppingCartItemSubTotalWithDiscount = CurrencyManager.ConvertCurrency(shoppingCartItemSubTotalWithDiscountBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                string subTotalString = PriceHelper.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                sb.Append("<span class=\"productPrice\">");
                sb.Append(subTotalString);
                sb.Append("</span>");

                //display an applied discount amount
                decimal shoppingCartItemSubTotalWithoutDiscountBase = TaxManager.GetPrice(shoppingCartItem.ProductVariant, PriceHelper.GetSubTotal(shoppingCartItem, false), out taxRate);
                decimal shoppingCartItemDiscountBase = shoppingCartItemSubTotalWithoutDiscountBase - shoppingCartItemSubTotalWithDiscountBase;
                if (shoppingCartItemDiscountBase > decimal.Zero)
                {
                    decimal shoppingCartItemDiscount = CurrencyManager.ConvertCurrency(shoppingCartItemDiscountBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                    string discountString = PriceHelper.FormatPrice(shoppingCartItemDiscount);

                    sb.Append("<br />");
                    sb.Append(GetLocaleResourceString("Wishlist.ItemYouSave"));
                    sb.Append("&nbsp;&nbsp;");
                    sb.Append(discountString);
                }
            }
            return sb.ToString();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateWishlist();
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in rptShoppingCart.Items)
            {
                var lblShoppingCartItemId = item.FindControl("lblShoppingCartItemId") as Label;
                var cbAddToCart = item.FindControl("cbAddToCart") as CheckBox;

                int shoppingCartItemId = 0;
                if (lblShoppingCartItemId != null && cbAddToCart != null)
                {
                    int.TryParse(lblShoppingCartItemId.Text, out shoppingCartItemId);
                    if (cbAddToCart.Checked)
                    {
                        var sci  = ShoppingCartManager.GetShoppingCartItemById(shoppingCartItemId);
                        if (sci != null)
                        {
                            ShoppingCartManager.AddToCart(
                                ShoppingCartTypeEnum.ShoppingCart,
                                sci.ProductVariantId, 
                                sci.AttributesXml,
                                sci.CustomerEnteredPrice,
                                sci.Quantity);
                        }
                    }
                }
            }

            Response.Redirect(SEOHelper.GetShoppingCartUrl());
        }

        [DefaultValue(false)]
        public bool IsEditable
        {
            get
            {
                object obj2 = this.ViewState["IsEditable"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["IsEditable"] = value;
            }
        }

        public Guid CustomerSessionGuid
        {
            get
            {
                object obj2 = this.ViewState["CustomerSessionGuid"];
                if (obj2 != null)
                    return (Guid)obj2;
                else
                    return Guid.Empty;
            }
            set
            {
                this.ViewState["CustomerSessionGuid"] = value;
            }
        }
    }
}