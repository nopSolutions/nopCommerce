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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Media;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class OrderTotalsControl : BaseNopUserControl
    {
        public void BindData(bool isShoppingCart)
        {
            this.IsShoppingCart = isShoppingCart;
            var cart = ShoppingCartManager.GetCurrentShoppingCart(ShoppingCartTypeEnum.ShoppingCart);

            if (cart.Count > 0)
            {
                //payment method (if already selected)
                int paymentMethodId = 0;
                if (NopContext.Current.User != null)
                    paymentMethodId = NopContext.Current.User.LastPaymentMethodId;

                //subtotal
                decimal subTotalDiscountBase = decimal.Zero;
                Discount appliedDiscount = null;
                //don't include checkout attributes on shopping cart page
                decimal subtotalBaseWithoutPromo = decimal.Zero;
                decimal subtotalBaseWithPromo = decimal.Zero;
                string SubTotalError = ShoppingCartManager.GetShoppingCartSubTotal(cart,
                    NopContext.Current.User, out subTotalDiscountBase,
                    out appliedDiscount, 
                    out subtotalBaseWithoutPromo, out subtotalBaseWithPromo);
                if (String.IsNullOrEmpty(SubTotalError))
                {
                    decimal subtotalWithoutPromo = CurrencyManager.ConvertCurrency(subtotalBaseWithoutPromo, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                    lblSubTotalAmount.Text = PriceHelper.FormatPrice(subtotalWithoutPromo);
                    lblSubTotalAmount.CssClass = "productPrice";

                    //discount
                    if (subTotalDiscountBase > decimal.Zero)
                    {
                        decimal subTotalDiscount = CurrencyManager.ConvertCurrency(subTotalDiscountBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                        lblSubTotalDiscountAmount.Text = PriceHelper.FormatPrice(-subTotalDiscount, true, false);
                        phSubTotalDiscount.Visible = true;
                    }
                    else
                    {
                        phSubTotalDiscount.Visible = false;
                    }
                }
                else
                {
                    //impossible
                    lblSubTotalAmount.Text = GetLocaleResourceString("ShoppingCart.CalculatedDuringCheckout");
                    lblSubTotalAmount.CssClass = string.Empty;
                }

                //shipping info
                bool shoppingCartRequiresShipping = ShippingManager.ShoppingCartRequiresShipping(cart);
                if (shoppingCartRequiresShipping)
                {
                    decimal? shoppingCartShippingBase = ShippingManager.GetShoppingCartShippingTotal(cart, NopContext.Current.User);
                    if (shoppingCartShippingBase.HasValue)
                    {
                        decimal shoppingCartShipping = CurrencyManager.ConvertCurrency(shoppingCartShippingBase.Value, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                        lblShippingAmount.Text = PriceHelper.FormatShippingPrice(shoppingCartShipping, true);
                        lblShippingAmount.CssClass = "productPrice";
                    }
                    else
                    {
                        lblShippingAmount.Text = GetLocaleResourceString("ShoppingCart.CalculatedDuringCheckout");
                        lblShippingAmount.CssClass = string.Empty;
                    }
                }
                else
                {
                    lblShippingAmount.Text = GetLocaleResourceString("ShoppingCart.ShippingNotRequired");
                    lblShippingAmount.CssClass = string.Empty;
                }

                //payment method fee
                bool displayPaymentMethodFee = true;
                decimal paymentMethodAdditionalFee = PaymentManager.GetAdditionalHandlingFee(paymentMethodId);
                decimal paymentMethodAdditionalFeeWithTaxBase = TaxManager.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, NopContext.Current.User);
                if (paymentMethodAdditionalFeeWithTaxBase > decimal.Zero)
                {
                    decimal paymentMethodAdditionalFeeWithTax = CurrencyManager.ConvertCurrency(paymentMethodAdditionalFeeWithTaxBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                    lblPaymentMethodAdditionalFee.Text = PriceHelper.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeWithTax, true);
                }
                else
                {
                    displayPaymentMethodFee = false;
                }
                phPaymentMethodAdditionalFee.Visible = displayPaymentMethodFee;

                //tax
                bool displayTax = true;
                if (TaxManager.HideTaxInOrderSummary && NopContext.Current.TaxDisplayType == TaxDisplayTypeEnum.IncludingTax)
                {
                    displayTax = false;
                }
                else
                {
                    string TaxError = string.Empty;
                    decimal shoppingCartTaxBase = TaxManager.GetTaxTotal(cart, paymentMethodId, NopContext.Current.User, ref TaxError);
                    decimal shoppingCartTax = CurrencyManager.ConvertCurrency(shoppingCartTaxBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);

                    if (String.IsNullOrEmpty(TaxError))
                    {
                        if (shoppingCartTaxBase == 0 && TaxManager.HideZeroTax)
                        {
                            displayTax = false;
                        }
                        else
                        {
                            lblTaxAmount.Text = PriceHelper.FormatPrice(shoppingCartTax, true, false);
                            lblTaxAmount.CssClass = "productPrice";
                        }
                    }
                    else
                    {
                        lblTaxAmount.Text = GetLocaleResourceString("ShoppingCart.CalculatedDuringCheckout");
                        lblTaxAmount.CssClass = string.Empty;
                    }
                }
                phTaxTotal.Visible = displayTax;

                //total
                List<AppliedGiftCard> appliedGiftCards = null;
                int redeemedRewardPoints = 0;
                decimal redeemedRewardPointsAmount = decimal.Zero;
                bool useRewardPoints = false;
                if (NopContext.Current.User != null)
                    useRewardPoints = NopContext.Current.User.UseRewardPointsDuringCheckout;
                decimal? shoppingCartTotalBase = ShoppingCartManager.GetShoppingCartTotal(cart,
                    paymentMethodId, NopContext.Current.User,
                    out appliedGiftCards, useRewardPoints,
                    out redeemedRewardPoints, out redeemedRewardPointsAmount);
                if (shoppingCartTotalBase.HasValue)
                {
                    decimal shoppingCartTotal = CurrencyManager.ConvertCurrency(shoppingCartTotalBase.Value, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                    lblTotalAmount.Text = PriceHelper.FormatPrice(shoppingCartTotal, true, false);
                    lblTotalAmount.CssClass = "productPrice";
                }
                else
                {
                    lblTotalAmount.Text = GetLocaleResourceString("ShoppingCart.CalculatedDuringCheckout");
                    lblTotalAmount.CssClass = string.Empty;
                }

                //gift cards
                if (appliedGiftCards != null && appliedGiftCards.Count > 0)
                {
                    rptrGiftCards.Visible = true;
                    rptrGiftCards.DataSource = appliedGiftCards;
                    rptrGiftCards.DataBind();
                }
                else
                {
                    rptrGiftCards.Visible = false;
                }

                //reward points
                if (redeemedRewardPointsAmount > decimal.Zero)
                {
                    phRewardPoints.Visible = true;

                    decimal redeemedRewardPointsAmountInCustomerCurrency = CurrencyManager.ConvertCurrency(redeemedRewardPointsAmount, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                    lRewardPointsTitle.Text = string.Format(GetLocaleResourceString("ShoppingCart.Totals.RewardPoints"), redeemedRewardPoints);
                    lblRewardPointsAmount.Text = PriceHelper.FormatPrice(-redeemedRewardPointsAmountInCustomerCurrency, true, false);
                }
                else
                {
                    phRewardPoints.Visible = false;
                }
            }
            else
            {
                this.Visible = false;
            }
        }

        protected void rptrGiftCards_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var appliedGiftCard = e.Item.DataItem as AppliedGiftCard;

                var lGiftCard = e.Item.FindControl("lGiftCard") as Literal;
                lGiftCard.Text = String.Format(GetLocaleResourceString("ShoppingCart.Totals.GiftCardInfo"), Server.HtmlEncode(appliedGiftCard.GiftCard.GiftCardCouponCode));

                var lblGiftCardAmount = e.Item.FindControl("lblGiftCardAmount") as Label;
                decimal amountCanBeUsed = CurrencyManager.ConvertCurrency(appliedGiftCard.AmountCanBeUsed, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                lblGiftCardAmount.Text = PriceHelper.FormatPrice(-amountCanBeUsed, true, false);

                var lGiftCardRemaining = e.Item.FindControl("lGiftCardRemaining") as Literal;
                decimal remainingAmountBase = GiftCardHelper.GetGiftCardRemainingAmount(appliedGiftCard.GiftCard) - appliedGiftCard.AmountCanBeUsed;
                decimal remainingAmount = CurrencyManager.ConvertCurrency(remainingAmountBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                lGiftCardRemaining.Text = string.Format(GetLocaleResourceString("ShoppingCart.Totals.GiftCardInfo.Remaining"), PriceHelper.FormatPrice(remainingAmount, true, false));

                var btnRemoveGC = e.Item.FindControl("btnRemoveGC") as LinkButton;
                btnRemoveGC.Visible = this.IsShoppingCart;
            }
        }

        protected void rptrGiftCards_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                if (this.IsShoppingCart)
                {
                    int giftCardId = Convert.ToInt32(e.CommandArgument);
                    GiftCard gc = OrderManager.GetGiftCardById(giftCardId);
                    if (gc != null)
                    {
                        string couponCodesXML = string.Empty;
                        if (NopContext.Current.User != null)
                            couponCodesXML = NopContext.Current.User.GiftCardCouponCodes;
                        couponCodesXML = GiftCardHelper.RemoveCouponCode(couponCodesXML, gc.GiftCardCouponCode);
                        CustomerManager.ApplyGiftCardCouponCode(couponCodesXML);
                    }
                    this.BindData(this.IsShoppingCart);
                }
            }
        }

        [DefaultValue(false)]
        public bool IsShoppingCart
        {
            get
            {
                object obj2 = this.ViewState["IsShoppingCart"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["IsShoppingCart"] = value;
            }
        }
    }
}