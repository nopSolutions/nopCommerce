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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;
 

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CheckoutPaymentMethodControl : BaseNopUserControl
    {
        protected CheckoutStepChangedEventHandler handler;
        protected ShoppingCart cart = null;

        protected string FormatPaymentMethodInfo(PaymentMethod paymentMethod)
        {
            decimal paymentMethodAdditionalFee = PaymentManager.GetAdditionalHandlingFee(paymentMethod.PaymentMethodId);
            decimal rateBase = TaxManager.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, NopContext.Current.User);
            decimal rate = CurrencyManager.ConvertCurrency(rateBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
            if (rate > decimal.Zero)
            {
                string rateStr = PriceHelper.FormatPaymentMethodAdditionalFee(rate, true);
                return string.Format("({0})", rateStr);
            }
            else
            {
                return string.Empty;
            }
        }

        protected void btnNextStep_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //reward points
                ApplyRewardPoints();

                //payment methods
                int paymentMethodId = this.SelectedPaymentMethodId;
                var paymentMethod = PaymentMethodManager.GetPaymentMethodById(paymentMethodId);
                if (paymentMethod != null && paymentMethod.IsActive)
                {
                    NopContext.Current.User = CustomerManager.SetLastPaymentMethodId(NopContext.Current.User.CustomerId, paymentMethodId);
                    var args1 = new CheckoutStepEventArgs() { PaymentMethodSelected = true };
                    OnCheckoutStepChanged(args1);
                    if (!this.OnePageCheckout)
                        Response.Redirect("~/checkoutpaymentinfo.aspx");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if ((NopContext.Current.User == null) || (NopContext.Current.User.IsGuest && !CustomerManager.AnonymousCheckoutAllowed))
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            if (this.Cart.Count == 0)
                Response.Redirect(SEOHelper.GetShoppingCartUrl());
        }

        protected int SelectedPaymentMethodId
        {
            get
            {
                int selectedPaymentMethodId = 0;
                foreach (DataListItem item in this.dlPaymentMethod.Items)
                {
                    RadioButton rdPaymentMethod = (RadioButton)item.FindControl("rdPaymentMethod");
                    if (rdPaymentMethod.Checked)
                    {
                        selectedPaymentMethodId = Convert.ToInt32(this.dlPaymentMethod.DataKeys[item.ItemIndex].ToString());
                        break;
                    }
                }
                return selectedPaymentMethodId;
            }
        }

        protected virtual void OnCheckoutStepChanged(CheckoutStepEventArgs e)
        {
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void ApplyRewardPoints()
        {
            //reward points
            NopContext.Current.User.UseRewardPointsDuringCheckout = cbUseRewardPoints.Checked;
        }

        public void BindData()
        {
            //reward points
            if (OrderManager.RewardPointsEnabled && !this.Cart.IsRecurring)
            {
                int rewardPointsBalance = NopContext.Current.User.RewardPointsBalance;
                decimal rewardPointsAmountBase = OrderManager.ConvertRewardPointsToAmount(rewardPointsBalance);
                decimal rewardPointsAmount = CurrencyManager.ConvertCurrency(rewardPointsAmountBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                if (rewardPointsAmount > decimal.Zero)
                {
                    string rewardPointsAmountStr = PriceHelper.FormatPrice(rewardPointsAmount, true, false);
                    cbUseRewardPoints.Text = GetLocaleResourceString("Checkout.UseRewardPoints", rewardPointsBalance, rewardPointsAmountStr);
                    pnlRewardPoints.Visible = true;
                }
                else
                {
                    pnlRewardPoints.Visible = false;
                }
            }
            else
            {
                pnlRewardPoints.Visible = false;
            }

            //payment methods
            int? filterByCountryId = null;
            if (NopContext.Current.User.BillingAddress != null && NopContext.Current.User.BillingAddress.Country != null)
            {
                filterByCountryId = NopContext.Current.User.BillingAddress.CountryId;
            }

            bool hasButtonMethods = false;
            var boundPaymentMethods = new List<PaymentMethod>();
            var paymentMethods = PaymentMethodManager.GetAllPaymentMethods(filterByCountryId);
            foreach (var pm in paymentMethods)
            {
                switch (pm.PaymentMethodType)
                {
                    case PaymentMethodTypeEnum.Unknown:
                    case PaymentMethodTypeEnum.Standard:
                        {
                            if (!Cart.IsRecurring || PaymentManager.SupportRecurringPayments(pm.PaymentMethodId) != RecurringPaymentTypeEnum.NotSupported)
                                boundPaymentMethods.Add(pm);
                        }
                        break;
                    case PaymentMethodTypeEnum.Button:
                        {
                            //PayPal Express is placed here as button
                            if (pm.SystemKeyword == "PayPalExpress")
                            {
                                if (!Cart.IsRecurring || PaymentManager.SupportRecurringPayments(pm.PaymentMethodId) != RecurringPaymentTypeEnum.NotSupported)
                                    hasButtonMethods = true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            if (boundPaymentMethods.Count == 0)
            {
                if (hasButtonMethods)
                {
                    phSelectPaymentMethod.Visible = false;
                    pnlPaymentMethodsError.Visible = false;

                    //no reward points in this case
                    pnlRewardPoints.Visible = false;
                }
                else
                {
                    phSelectPaymentMethod.Visible = false;
                    pnlPaymentMethodsError.Visible = true;                    
                    lPaymentMethodsError.Text = GetLocaleResourceString("Checkout.NoPaymentMethods");

                    //no reward points in this case
                    pnlRewardPoints.Visible = false;
                }
            }
            else if (boundPaymentMethods.Count == 1)
            {
                phSelectPaymentMethod.Visible = true;
                pnlPaymentMethodsError.Visible = false;
                dlPaymentMethod.DataSource = boundPaymentMethods;
                dlPaymentMethod.DataBind();
            }
            else
            {
                phSelectPaymentMethod.Visible = true;
                pnlPaymentMethodsError.Visible = false;
                dlPaymentMethod.DataSource = boundPaymentMethods;
                dlPaymentMethod.DataBind();
            }
        }

        public event CheckoutStepChangedEventHandler CheckoutStepChanged
        {
            add
            {
                handler += value;
            }
            remove
            {
                handler -= value;
            }
        }

        public ShoppingCart Cart
        {
            get
            {
                if (cart == null)
                {
                    cart = ShoppingCartManager.GetCurrentShoppingCart(ShoppingCartTypeEnum.ShoppingCart);
                }
                return cart;
            }
        }

        public bool OnePageCheckout
        {
            get
            {
                if (ViewState["OnePageCheckout"] != null)
                    return (bool)ViewState["OnePageCheckout"];
                return false;
            }
            set
            {
                ViewState["OnePageCheckout"] = value;
            }
        }
    }
}