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
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CustomerWishlistControl : BaseNopAdministrationUserControl
    {
        private Customer customer = null;

        private void BindData()
        {
            if (customer != null)
            {
                if (SettingManager.GetSettingValueBoolean("Common.EnableWishlist"))
                {
                    ShoppingCart cart = ShoppingCartManager.GetCustomerShoppingCart(customer.CustomerId, ShoppingCartTypeEnum.Wishlist);
                    if (cart.Count > 0)
                    {
                        pnlMessage.Visible = false;
                        pnlCart.Visible = true;
                        gvProductVariants.DataSource = cart;
                        gvProductVariants.DataBind();
                    }
                    else
                    {
                        pnlMessage.Visible = true;
                        lblMessage.Text = GetLocaleResourceString("Admin.CustomerWishlist.Empty");
                        pnlCart.Visible = false;
                    }
                }
                else
                {
                    pnlMessage.Visible = true;
                    lblMessage.Text = GetLocaleResourceString("Admin.CustomerWishlist.Disabled");
                    pnlCart.Visible = false;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            customer = CustomerManager.GetCustomerById(this.CustomerId);
            if (!Page.IsPostBack)
            {              
                this.BindData();
            }
        }

        public string GetProductVariantUrl(ShoppingCartItem shoppingCartItem)
        {
            string result = string.Empty;
            if (shoppingCartItem == null)
                return result;
            ProductVariant productVariant = shoppingCartItem.ProductVariant;
            if (productVariant != null)
                result = "ProductVariantDetails.aspx?ProductVariantID=" + productVariant.ProductVariantId.ToString();
            else
                result = "Not available. Product variant ID=" + shoppingCartItem.ProductVariantId.ToString();
            return result;
        }

        public string GetProductVariantName(ShoppingCartItem shoppingCartItem)
        {
            ProductVariant productVariant = shoppingCartItem.ProductVariant;
            if (productVariant != null)
                return productVariant.FullProductName;
            return "Not available";
        }

        public string GetAttributeDescription(ShoppingCartItem shoppingCartItem)
        {
            string result = ProductAttributeHelper.FormatAttributes(shoppingCartItem.ProductVariant, shoppingCartItem.AttributesXml, customer, "<br />");
            if (!String.IsNullOrEmpty(result))
                result = "<br />" + result;
            return result;
        }

        public string GetShoppingCartItemUnitPriceString(ShoppingCartItem shoppingCartItem)
        {
            StringBuilder sb = new StringBuilder();
            decimal taxRate = decimal.Zero;
            decimal shoppingCartUnitPriceWithDiscountBase = TaxManager.GetPrice(shoppingCartItem.ProductVariant, PriceHelper.GetUnitPrice(shoppingCartItem, customer, true), customer, out taxRate);
            decimal shoppingCartUnitPriceWithDiscount = CurrencyManager.ConvertCurrency(shoppingCartUnitPriceWithDiscountBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
            string unitPriceString = PriceHelper.FormatPrice(shoppingCartUnitPriceWithDiscount);

            sb.Append(unitPriceString);
            return sb.ToString();
        }

        public string GetShoppingCartItemSubTotalString(ShoppingCartItem shoppingCartItem)
        {
            StringBuilder sb = new StringBuilder();
            decimal taxRate = decimal.Zero;
            decimal shoppingCartItemSubTotalWithDiscountBase = TaxManager.GetPrice(shoppingCartItem.ProductVariant, PriceHelper.GetSubTotal(shoppingCartItem, customer, true), customer, out taxRate);
            decimal shoppingCartItemSubTotalWithDiscount = CurrencyManager.ConvertCurrency(shoppingCartItemSubTotalWithDiscountBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
            string subTotalString = PriceHelper.FormatPrice(shoppingCartItemSubTotalWithDiscount);

            sb.Append(subTotalString);

            decimal shoppingCartItemDiscountBase = TaxManager.GetPrice(shoppingCartItem.ProductVariant, PriceHelper.GetDiscountAmount(shoppingCartItem, customer), customer, out taxRate);
            if (shoppingCartItemDiscountBase > decimal.Zero)
            {
                decimal shoppingCartItemDiscount = CurrencyManager.ConvertCurrency(shoppingCartItemDiscountBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                string discountString = PriceHelper.FormatPrice(shoppingCartItemDiscount);

                sb.Append("<br />");
                //sb.Append(GetLocaleResourceString("ShoppingCart.ItemYouSave"));
                sb.Append("Saved:");
                sb.Append("&nbsp;&nbsp;");
                sb.Append(discountString);
            }
            return sb.ToString();
        }
        
        public void SaveInfo()
        {
        }

        public int CustomerId
        {
            get
            {
                return CommonHelper.QueryStringInt("CustomerId");
            }
        }
    }
}