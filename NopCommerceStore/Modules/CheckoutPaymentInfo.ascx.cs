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
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Templates.Payment;
 

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CheckoutPaymentInfoControl : BaseNopUserControl
    {
        protected CheckoutStepChangedEventHandler handler;
        protected ShoppingCart cart = null;
        protected bool paymentControlLoaded = false;

        public void LoadPaymentControl()
        {
            PaymentMethod paymentMethod = null;
            if (NopContext.Current.User != null)
            {
                paymentMethod = NopContext.Current.User.LastPaymentMethod;
            }
            if (paymentMethod != null && paymentMethod.IsActive)
            {
                if (paymentControlLoaded)
                {
                    this.PaymentInfoPlaceHolder.Controls.Clear();
                }
                Control child = null;
                child = base.LoadControl(paymentMethod.UserTemplatePath);
                this.PaymentInfoPlaceHolder.Controls.Add(child);
                paymentControlLoaded = true;
            }
            else
            {
                if (!this.OnePageCheckout)
                    Response.Redirect("~/checkoutpaymentmethod.aspx");
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.LoadPaymentControl();
        }

        public bool ValidateForm()
        {
            var ctrl = GetPaymentModule();
            if (ctrl != null)
                return ctrl.ValidateForm() && Page.IsValid;
            return Page.IsValid;
        }

        public PaymentInfo GetPaymentInfo()
        {
            PaymentInfo paymentInfo = null;
            var ctrl = GetPaymentModule();
            if (ctrl != null)
            {
                paymentInfo = ctrl.GetPaymentInfo();
                paymentInfo.PaymentMethodId = NopContext.Current.User.LastPaymentMethodId;
            }
            return paymentInfo;
        }

        protected IPaymentMethodModule GetPaymentModule()
        {
            foreach (var ctrl in this.PaymentInfoPlaceHolder.Controls)
                if (ctrl is IPaymentMethodModule)
                    return (IPaymentMethodModule)ctrl;
            return null;
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
            //validate whether we need to display this control
            PaymentMethod paymentMethod = null;
            if (NopContext.Current.User != null)
            {
                paymentMethod = NopContext.Current.User.LastPaymentMethod;
            }
            if (paymentMethod != null &&
                paymentMethod.IsActive &&
                paymentMethod.HidePaymentInfoForZeroOrders)
            {

                decimal? shoppingCartTotalBase = ShoppingCartManager.GetShoppingCartTotal(this.Cart,
                NopContext.Current.User.LastPaymentMethodId, NopContext.Current.User);

                if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value == decimal.Zero)
                {
                    this.PaymentInfo = this.GetPaymentInfo();
                    var args1 = new CheckoutStepEventArgs() { PaymentInfoEntered = true };
                    OnCheckoutStepChanged(args1);
                    if (!this.OnePageCheckout)
                        Response.Redirect("~/checkoutconfirm.aspx");
                }
            }
        }

        protected void btnNextStep_Click(object sender, EventArgs e)
        {
            if (this.ValidateForm())
            {
                this.PaymentInfo = this.GetPaymentInfo();
                var args1 = new CheckoutStepEventArgs() { PaymentInfoEntered = true };
                OnCheckoutStepChanged(args1);
                if (!this.OnePageCheckout)
                    Response.Redirect("~/checkoutconfirm.aspx");
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

        protected PaymentInfo PaymentInfo
        {
            set
            {
                this.Session["OrderPaymentInfo"] = value;
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