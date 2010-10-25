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
using NopSolutions.NopCommerce.BusinessLogic.IoC;
 

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CheckoutPaymentMethodControl : BaseNopUserControl
    {
        protected CheckoutStepChangedEventHandler handler;
        protected ShoppingCart cart = null;

        protected string FormatPaymentMethodInfo(PaymentMethod paymentMethod)
        {
            decimal paymentMethodAdditionalFee = IoCFactory.Resolve<IPaymentManager>().GetAdditionalHandlingFee(paymentMethod.PaymentMethodId);
            decimal rateBase = IoCFactory.Resolve<ITaxManager>().GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, NopContext.Current.User);
            decimal rate = IoCFactory.Resolve<ICurrencyManager>().ConvertCurrency(rateBase, IoCFactory.Resolve<ICurrencyManager>().PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
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
                if (paymentMethodId > 0)
                {
                    var paymentMethod = IoCFactory.Resolve<IPaymentMethodManager>().GetPaymentMethodById(paymentMethodId);
                    if (paymentMethod != null && paymentMethod.IsActive)
                    {
                        NopContext.Current.User = IoCFactory.Resolve<ICustomerManager>().SetLastPaymentMethodId(NopContext.Current.User.CustomerId, paymentMethodId);
                        var args1 = new CheckoutStepEventArgs() { PaymentMethodSelected = true };
                        OnCheckoutStepChanged(args1);
                        if (!this.OnePageCheckout)
                            Response.Redirect("~/checkoutpaymentinfo.aspx");
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if ((NopContext.Current.User == null) || (NopContext.Current.User.IsGuest && !IoCFactory.Resolve<ICustomerManager>().AnonymousCheckoutAllowed))
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
            set
            {
                foreach (DataListItem item in this.dlPaymentMethod.Items)
                {
                    RadioButton rdPaymentMethod = (RadioButton)item.FindControl("rdPaymentMethod");

                    if (value == null)
                    {
                        rdPaymentMethod.Checked = false;
                    }
                    else
                    {
                        int paymentMethodId = 0;
                        if (int.TryParse(this.dlPaymentMethod.DataKeys[item.ItemIndex].ToString(), out paymentMethodId))
                        {
                            if (paymentMethodId == value)
                            {
                                rdPaymentMethod.Checked = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        protected virtual void OnCheckoutStepChanged(CheckoutStepEventArgs e)
        {
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public bool IsPaymentWorkflowRequired()
        {
            bool result = true;

            //check whether order total equals zero
            if (NopContext.Current.User != null)
            {
                decimal? shoppingCartTotalBase = IoCFactory.Resolve<IShoppingCartManager>().GetShoppingCartTotal(this.Cart,
                NopContext.Current.User.LastPaymentMethodId, NopContext.Current.User);

                if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value == decimal.Zero)
                {
                    result = false;
                }
            }
            return result;
        }

        public void ApplyRewardPoints()
        {
            //reward points
            if (NopContext.Current.User != null)
            {
                NopContext.Current.User.UseRewardPointsDuringCheckout = cbUseRewardPoints.Checked;
            }

            //Check whether payment workflow is required
            bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired();
            if (!isPaymentWorkflowRequired)
            {
                NopContext.Current.User = IoCFactory.Resolve<ICustomerManager>().SetLastPaymentMethodId(NopContext.Current.User.CustomerId, 0);
                var args1 = new CheckoutStepEventArgs() { PaymentMethodSelected = true };
                OnCheckoutStepChanged(args1);
                if (!this.OnePageCheckout)
                    Response.Redirect("~/checkoutpaymentinfo.aspx");
            }
        }

        public void BindData()
        {
            //Check whether payment workflow is required
            bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired();
            if (!isPaymentWorkflowRequired)
            {
                NopContext.Current.User = IoCFactory.Resolve<ICustomerManager>().SetLastPaymentMethodId(NopContext.Current.User.CustomerId, 0);
                var args1 = new CheckoutStepEventArgs() { PaymentMethodSelected = true };
                OnCheckoutStepChanged(args1);
                if (!this.OnePageCheckout)
                    Response.Redirect("~/checkoutpaymentinfo.aspx");
            }

            //reward points
            if (IoCFactory.Resolve<IOrderManager>().RewardPointsEnabled && !this.Cart.IsRecurring)
            {
                int rewardPointsBalance = NopContext.Current.User.RewardPointsBalance;
                decimal rewardPointsAmountBase = IoCFactory.Resolve<IOrderManager>().ConvertRewardPointsToAmount(rewardPointsBalance);
                decimal rewardPointsAmount = IoCFactory.Resolve<ICurrencyManager>().ConvertCurrency(rewardPointsAmountBase, IoCFactory.Resolve<ICurrencyManager>().PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
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
            var paymentMethods = IoCFactory.Resolve<IPaymentMethodManager>().GetAllPaymentMethods(filterByCountryId);
            foreach (var pm in paymentMethods)
            {
                switch (pm.PaymentMethodType)
                {
                    case PaymentMethodTypeEnum.Unknown:
                    case PaymentMethodTypeEnum.Standard:
                        {
                            if (!Cart.IsRecurring || IoCFactory.Resolve<IPaymentManager>().SupportRecurringPayments(pm.PaymentMethodId) != RecurringPaymentTypeEnum.NotSupported)
                                boundPaymentMethods.Add(pm);
                        }
                        break;
                    case PaymentMethodTypeEnum.Button:
                        {
                            //PayPal Express is placed here as button
                            if (pm.SystemKeyword == "PayPalExpress")
                            {
                                if (!Cart.IsRecurring || IoCFactory.Resolve<IPaymentManager>().SupportRecurringPayments(pm.PaymentMethodId) != RecurringPaymentTypeEnum.NotSupported)
                                {
                                    //bind PayPal Express button
                                    btnPaypalExpressButton.BindData();
                                    hasButtonMethods = true;
                                }
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

                //select a default payment method
                if (dlPaymentMethod.Items.Count > 0)
                {
                    var tmp1 = dlPaymentMethod.Items[0];
                    var rdPaymentMethod = tmp1.FindControl("rdPaymentMethod") as RadioButton;
                    if (rdPaymentMethod != null)
                    {
                        rdPaymentMethod.Checked = true;
                    }
                    //or you can select it and go to the next step of checkout
                    //but it this case a customer will not be able apply a reward point a select a "button" payment metho
                }
            }
            else
            {
                phSelectPaymentMethod.Visible = true;
                pnlPaymentMethodsError.Visible = false;
                dlPaymentMethod.DataSource = boundPaymentMethods;
                dlPaymentMethod.DataBind();
                
                //select a default payment method
                if (dlPaymentMethod.Items.Count > 0)
                {
                    if (NopContext.Current.User != null &&
                        NopContext.Current.User.LastPaymentMethod != null)
                    {
                        //already selected payment method
                        this.SelectedPaymentMethodId = NopContext.Current.User.LastPaymentMethod.PaymentMethodId;
                    }
                    else
                    {
                        //otherwise, the first payment method
                        var tmp1 = dlPaymentMethod.Items[0];
                        var rdPaymentMethod = tmp1.FindControl("rdPaymentMethod") as RadioButton;
                        if (rdPaymentMethod != null)
                        {
                            rdPaymentMethod.Checked = true;
                        }
                    }
                }
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
                    cart = IoCFactory.Resolve<IShoppingCartManager>().GetCurrentShoppingCart(ShoppingCartTypeEnum.ShoppingCart);
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