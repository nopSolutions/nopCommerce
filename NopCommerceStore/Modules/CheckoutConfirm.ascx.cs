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
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
 

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CheckoutConfirmControl : BaseNopUserControl
    {
        protected CheckoutStepChangedEventHandler handler;
        protected ShoppingCart cart = null;

        protected void btnNextStep_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    var paymentInfo = this.PaymentInfo;
                    if (paymentInfo == null)
                    {
                        var args1 = new CheckoutStepEventArgs() { OrderConfirmed = false };
                        OnCheckoutStepChanged(args1);
                        if (!this.OnePageCheckout)
                        {
                            Response.Redirect("~/checkoutpaymentinfo.aspx");
                        }
                        else
                        {
                            return;
                        }
                    }
                    paymentInfo.BillingAddress = NopContext.Current.User.BillingAddress;
                    paymentInfo.ShippingAddress = NopContext.Current.User.ShippingAddress;
                    paymentInfo.CustomerLanguage = NopContext.Current.WorkingLanguage;
                    paymentInfo.CustomerCurrency = NopContext.Current.WorkingCurrency;

                    int orderId = 0;
                    string result = IoC.Resolve<IOrderService>().PlaceOrder(paymentInfo, NopContext.Current.User, out orderId);
                    this.PaymentInfo = null;
                    var order = IoC.Resolve<IOrderService>().GetOrderById(orderId);
                    if (!String.IsNullOrEmpty(result))
                    {
                        lConfirmOrderError.Text = Server.HtmlEncode(result);
                        return;
                    }
                    else
                    {
                        IoC.Resolve<IPaymentService>().PostProcessPayment(order);
                    }
                    var args2 = new CheckoutStepEventArgs() { OrderConfirmed = true };
                    OnCheckoutStepChanged(args2);
                    if (!this.OnePageCheckout)
                        Response.Redirect("~/checkoutcompleted.aspx");
                }
                catch (Exception exc)
                {
                    IoC.Resolve<ILogService>().InsertLog(LogTypeEnum.OrderError, exc.Message, exc);
                    lConfirmOrderError.Text = Server.HtmlEncode(exc.ToString());
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if ((NopContext.Current.User == null) || (NopContext.Current.User.IsGuest && !IoC.Resolve<ICustomerService>().AnonymousCheckoutAllowed))
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            if (this.Cart.Count == 0)
                Response.Redirect(SEOHelper.GetShoppingCartUrl());
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.btnNextStep.Attributes.Add("onclick", "this.disabled = true;" + Page.ClientScript.GetPostBackEventReference(this.btnNextStep, ""));

            //use postback if we're on one-page checkout page
            //we need it to properly process redirects (hosted payment methods)
            if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Checkout.UseOnePageCheckout"))
            {
                var sm = ScriptManager.GetCurrent(this.Page);
                if (sm != null)
                {
                    sm.RegisterPostBackControl(btnNextStep);
                }
            }

            base.OnPreRender(e);
        }
        
        protected virtual void OnCheckoutStepChanged(CheckoutStepEventArgs e)
        {
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void BindData()
        {
            //min order amount validation
            bool minOrderTotalAmountOK = IoC.Resolve<IOrderService>().ValidateMinOrderTotalAmount(this.Cart, NopContext.Current.User);
            if (minOrderTotalAmountOK)
            {
                lMinOrderTotalAmount.Visible = false;
                btnNextStep.Visible = true;
            }
            else
            {
                decimal minOrderTotalAmount = IoC.Resolve<ICurrencyService>().ConvertCurrency(IoC.Resolve<IOrderService>().MinOrderTotalAmount, IoC.Resolve<ICurrencyService>().PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                lMinOrderTotalAmount.Text = string.Format(GetLocaleResourceString("Checkout.MinOrderTotalAmount"), PriceHelper.FormatPrice(minOrderTotalAmount, true, false));
                lMinOrderTotalAmount.Visible = true;
                btnNextStep.Visible = false;
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

        protected PaymentInfo PaymentInfo
        {
            get
            {
                if (this.Session["OrderPaymentInfo"] != null)
                    return (PaymentInfo)(this.Session["OrderPaymentInfo"]);
                return null;
            }
            set
            {
                this.Session["OrderPaymentInfo"] = value;
            }
        }

        public ShoppingCart Cart
        {
            get
            {
                if (cart == null)
                {
                    cart = IoC.Resolve<IShoppingCartService>().GetCurrentShoppingCart(ShoppingCartTypeEnum.ShoppingCart);
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