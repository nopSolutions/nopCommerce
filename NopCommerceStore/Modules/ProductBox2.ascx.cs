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
using System.Threading;
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
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ProductBox2Control : BaseNopUserControl
    {
        Product product = null;

        public override void DataBind()
        {
            base.DataBind();
            this.BindData();
        }

        private void BindData()
        {
            if (product != null)
            {
                string productURL = SEOHelper.GetProductUrl(product);

                hlProduct.NavigateUrl = productURL;
                hlProduct.Text = Server.HtmlEncode(product.LocalizedName);

                var picture = product.DefaultPicture;
                if (picture != null)
                {
                    hlImageLink.ImageUrl = PictureManager.GetPictureUrl(picture, this.ProductImageSize, true);
                    hlImageLink.ToolTip = String.Format(GetLocaleResourceString("Media.Product.ImageLinkTitleFormat"), product.LocalizedName);
                    hlImageLink.Text = String.Format(GetLocaleResourceString("Media.Product.ImageAlternateTextFormat"), product.LocalizedName);
                }
                else
                {
                    hlImageLink.ImageUrl = PictureManager.GetDefaultPictureUrl(this.ProductImageSize);
                    hlImageLink.ToolTip = String.Format(GetLocaleResourceString("Media.Product.ImageLinkTitleFormat"), product.LocalizedName);
                    hlImageLink.Text = String.Format(GetLocaleResourceString("Media.Product.ImageAlternateTextFormat"), product.LocalizedName);
                }

                hlImageLink.NavigateUrl = productURL;
                lShortDescription.Text = product.LocalizedShortDescription;

                var productVariantCollection = product.ProductVariants;
                if (productVariantCollection.Count > 0)
                {
                    if (!product.HasMultipleVariants)
                    {
                        var productVariant = productVariantCollection[0];
                        btnAddToCart.Visible = (!productVariant.DisableBuyButton);
                        if (!SettingManager.GetSettingValueBoolean("Common.HidePricesForNonRegistered") ||
                            (NopContext.Current.User != null &&
                            !NopContext.Current.User.IsGuest))
                        {
                            //nothing
                        }
                        else
                        {
                            btnAddToCart.Visible = false;
                        }
                    }
                    else
                    {
                        btnAddToCart.Visible = false;
                    }
                }
                else
                {
                    btnAddToCart.Visible = false;
                }
            }
        }

        protected void btnProductDetails_Click(object sender, CommandEventArgs e)
        {
            int productId = Convert.ToInt32(e.CommandArgument);
            string productURL = SEOHelper.GetProductUrl(productId);
            Response.Redirect(productURL);
        }

        protected void btnAddToCart_Click(object sender, CommandEventArgs e)
        {
            int productId = Convert.ToInt32(e.CommandArgument);
            int productVariantId = 0;
            if (ProductManager.DirectAddToCartAllowed(productId, out productVariantId))
            {
                var addToCartWarnings = ShoppingCartManager.AddToCart(ShoppingCartTypeEnum.ShoppingCart,
                    productVariantId, string.Empty, decimal.Zero, 1);
                if (addToCartWarnings.Count == 0)
                {
                    bool displayCart = true;
                    if (this.RedirectCartAfterAddingProduct.HasValue)
                        displayCart = this.RedirectCartAfterAddingProduct.Value;
                    else
                        displayCart = SettingManager.GetSettingValueBoolean("Display.Products.DisplayCartAfterAddingProduct");

                    if (displayCart)
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
                    string productURL = SEOHelper.GetProductUrl(productId);
                    Response.Redirect(productURL);
                }
            }
            else
            {
                string productURL = SEOHelper.GetProductUrl(productId);
                Response.Redirect(productURL);
            }
        }

        public Product Product
        {
            get
            {
                return product;
            }
            set
            {
                product = value;
            }
        }

        public int ProductImageSize
        {
            get
            {
                if (ViewState["ProductImageSize"] == null)
                    return SettingManager.GetSettingValueInteger("Media.Product.ThumbnailImageSize", 125);
                else
                    return (int)ViewState["ProductImageSize"];
            }
            set
            {
                ViewState["ProductImageSize"] = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value whether we redirects a customer to shopping cart page after adding a product to the cart (overrides "Display.Products.DisplayCartAfterAddingProduct" settings)
        /// </summary>
        public bool? RedirectCartAfterAddingProduct
        {
            get
            {
                if (ViewState["RedirectCartAfterAddingProduct"] == null)
                    return null;
                else
                    return (bool)ViewState["RedirectCartAfterAddingProduct"];
            }
            set
            {
                ViewState["RedirectCartAfterAddingProduct"] = value;
            }
        }
    }
}