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
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Tax;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CurrentShoppingCartsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindGrid();
            }
        }

        protected void BindGrid()
        {
            var customerSessions = CustomerManager.GetAllCustomerSessionsWithNonEmptyShoppingCart();
            if (customerSessions.Count == 0)
            {
                lblCurrentShoppingCartsEmpty.Visible = true;
                gvCustomerSessions.Visible = false;
            }
            else
            {
                lblCurrentShoppingCartsEmpty.Visible = false;
                gvCustomerSessions.Visible = true;
                gvCustomerSessions.DataSource = customerSessions;
                gvCustomerSessions.DataBind();
            }
        }

        protected void gvCustomerSessions_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCustomerSessions.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvCustomerSessions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CustomerSession customerSession = (CustomerSession)e.Row.DataItem;

                GridView gvProductVariants = e.Row.FindControl("gvProductVariants") as GridView;

                if (gvProductVariants != null)
                {
                    var cart = ShoppingCartManager.GetShoppingCartByCustomerSessionGuid(ShoppingCartTypeEnum.ShoppingCart, 
                        customerSession.CustomerSessionGuid);

                    gvProductVariants.DataSource = cart;
                    gvProductVariants.DataBind();
                }
            }
        }

        protected string GetCustomerInfo(CustomerSession customerSession)
        {
            string customerInfo = string.Empty;
            Customer customer = customerSession.Customer;

            if (customer != null)
            {
                if (customer.IsGuest)
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, GetLocaleResourceString("Admin.CustomerShoppingCart.CustomerColumn.Guest"));
                }
                else
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, Server.HtmlEncode(customer.Email));
                }
            }
            else
            {
                customerInfo = GetLocaleResourceString("Admin.CustomerShoppingCart.CustomerColumn.Guest");
            }
            return customerInfo;
        }

        protected string GetLastAccessInfo(CustomerSession customerSession)
        {
            string info = string.Format(GetLocaleResourceString("Admin.CustomerShoppingCart.CustomerColumn.LastAccess"), 
                DateTimeHelper.ConvertToUserTime(customerSession.LastAccessed, DateTimeKind.Utc));
            return info;
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
            Customer customer = shoppingCartItem.CustomerSession.Customer;
            string result = ProductAttributeHelper.FormatAttributes(shoppingCartItem.ProductVariant, shoppingCartItem.AttributesXml, customer, "<br />");
            if (!String.IsNullOrEmpty(result))
                result = "<br />" + result;
            return result;
        }

        public string GetShoppingCartItemUnitPriceString(ShoppingCartItem shoppingCartItem)
        {
            Customer customer = shoppingCartItem.CustomerSession.Customer;
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
            Customer customer = shoppingCartItem.CustomerSession.Customer;
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
    }
}