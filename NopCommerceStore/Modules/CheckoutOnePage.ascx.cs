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
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CheckoutOnePageControl : BaseNopUserControl
    {
        protected ShoppingCart _cart = null;

        protected enum CheckoutStepEnum
        {
            ShippingAddress,
            BillingAddress,
            ShippingMethod,
            PaymentMethod,
            PaymentInfo,
            Confirm
        }

        protected void SelectPane(CheckoutStepEnum step)
        {
            cpeShippingAddress.ClientState = "true";
            cpeBillingAddress.ClientState = "true";
            cpeShippingMethods.ClientState = "true";
            cpePaymentMethods.ClientState = "true";
            cpePaymentInfo.ClientState = "true";
            cpeConfirm.ClientState = "true";

            cpeShippingAddress.Collapsed = true;
            cpeBillingAddress.Collapsed = true;
            cpeShippingMethods.Collapsed = true;
            cpePaymentMethods.Collapsed = true;
            cpePaymentInfo.Collapsed = true;
            cpeConfirm.Collapsed = true;

            btnModifyShippingAddress.Enabled = false;
            btnModifyBillingAddress.Enabled = false;
            btnModifyShippingMethod.Enabled = false;
            btnModifyPaymentMethod.Enabled = false;
            btnModifyPaymentInfo.Enabled = false;


            //process step selection
            switch(step)
            {
                case CheckoutStepEnum.ShippingAddress:
                    if(ShippingManager.ShoppingCartRequiresShipping(Cart))
                    {
                        cpeShippingAddress.Collapsed = false;
                        cpeShippingAddress.ClientState = "false";
                        ctrlCheckoutShippingAddress.BindData();
                    }
                    else
                    {
                        SelectPane(CheckoutStepEnum.BillingAddress);
                    }
                    break;

                case CheckoutStepEnum.BillingAddress:

                    btnModifyShippingAddress.Enabled = true;

                    cpeBillingAddress.ClientState = "false";
                    cpeBillingAddress.Collapsed = false;
                    ctrlCheckoutBillingAddress.BindData();
                    break;

                case CheckoutStepEnum.ShippingMethod:
                    if(ShippingManager.ShoppingCartRequiresShipping(Cart))
                    {

                        btnModifyShippingAddress.Enabled = true;
                        btnModifyBillingAddress.Enabled = true;
                        
                        cpeShippingMethods.ClientState = "false";
                        cpeShippingMethods.Collapsed = false;
                        ctrlCheckoutShippingMethod.BindData();
                    }
                    else
                    {
                        SelectPane(CheckoutStepEnum.PaymentMethod);
                    }
                    break;

                case CheckoutStepEnum.PaymentMethod:

                    btnModifyShippingAddress.Enabled = true;
                    btnModifyBillingAddress.Enabled = true;
                    btnModifyShippingMethod.Enabled = true;

                    cpePaymentMethods.ClientState = "false";
                    cpePaymentMethods.Collapsed = false;
                    ctrlCheckoutPaymentMethod.BindData();
                    break;

                case CheckoutStepEnum.PaymentInfo:
                    PaymentMethod paymentMethod = null;
                    if(NopContext.Current.User != null)
                    {
                        paymentMethod = NopContext.Current.User.LastPaymentMethod;
                    }
                    if(paymentMethod != null && paymentMethod.IsActive)
                    {
                        btnModifyShippingAddress.Enabled = true;
                        btnModifyBillingAddress.Enabled = true;
                        btnModifyShippingMethod.Enabled = true;
                        btnModifyPaymentMethod.Enabled = true;

                        cpePaymentInfo.ClientState = "false";
                        cpePaymentInfo.Collapsed = false;
                        ctrlCheckoutPaymentInfo.LoadPaymentControl();
                        ctrlCheckoutPaymentInfo.BindData();
                    }
                    else
                    {
                        SelectPane(CheckoutStepEnum.PaymentMethod);
                    }
                    break;

                case CheckoutStepEnum.Confirm:

                    btnModifyShippingAddress.Enabled = true;
                    btnModifyBillingAddress.Enabled = true;
                    btnModifyShippingMethod.Enabled = true;
                    btnModifyPaymentMethod.Enabled = true;
                    btnModifyPaymentInfo.Enabled = true;

                    cpeConfirm.ClientState = "false";
                    cpeConfirm.Collapsed = false;
                    ctrlCheckoutConfirm.BindData();
                    OrderSummaryControl.BindData();
                    break;

                default:
                    throw new NopException("Not supported checkout step");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if((NopContext.Current.User == null) || (NopContext.Current.User.IsGuest && !CustomerManager.AnonymousCheckoutAllowed))
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            if(Cart.Count == 0)
            {
                Response.Redirect(SEOHelper.GetShoppingCartUrl());
            }

            //validation
            var scWarnings = ShoppingCartManager.GetShoppingCartWarnings(Cart, NopContext.Current.User.CheckoutAttributes, true);
            if (scWarnings.Count > 0)
            {
                Response.Redirect(SEOHelper.GetShoppingCartUrl());
            }
            else
            {
                foreach (ShoppingCartItem sci in this.Cart)
                {
                    List<String> sciWarnings = ShoppingCartManager.GetShoppingCartItemWarnings(
                        sci.ShoppingCartType, 
                        sci.ProductVariantId, 
                        sci.AttributesXml, 
                        sci.CustomerEnteredPrice,
                        sci.Quantity);
                    if (sciWarnings.Count > 0)
                    {
                        Response.Redirect(SEOHelper.GetShoppingCartUrl());
                    }
                }
            }

            if(!Page.IsPostBack)
            {
                if(!ShippingManager.ShoppingCartRequiresShipping(Cart))
                {
                    pnlShippingAddress.Visible = false;
                    pnlShippingMethods.Visible = false;
                }

                SelectPane(CheckoutStepEnum.ShippingAddress);
            }
        }

        protected void ctrlCheckoutShippingAddress_CheckoutStepChanged(object sender, CheckoutStepEventArgs e)
        {
            if(e.ShippingAddressSelected)
            {
                SelectPane(CheckoutStepEnum.BillingAddress);
            }
        }

        protected void ctrlCheckoutBillingAddress_CheckoutStepChanged(object sender, CheckoutStepEventArgs e)
        {
            if(e.BillingAddressSelected)
            {
                SelectPane(CheckoutStepEnum.ShippingMethod);
            }
        }

        protected void ctrlCheckoutShippingMethod_CheckoutStepChanged(object sender, CheckoutStepEventArgs e)
        {
            if(e.ShippingMethodSelected)
            {
                SelectPane(CheckoutStepEnum.PaymentMethod);
            }
        }

        protected void ctrlCheckoutPaymentMethod_CheckoutStepChanged(object sender, CheckoutStepEventArgs e)
        {
            if(e.PaymentMethodSelected)
            {
                SelectPane(CheckoutStepEnum.PaymentInfo);
            }
        }

        protected void ctrlCheckoutPaymentInfo_CheckoutStepChanged(object sender, CheckoutStepEventArgs e)
        {
            if(e.PaymentInfoEntered)
            {
                SelectPane(CheckoutStepEnum.Confirm);
            }
        }

        protected void ctrlCheckoutConfirm_CheckoutStepChanged(object sender, CheckoutStepEventArgs e)
        {
            if(e.OrderConfirmed)
            {
                Response.Redirect("~/checkoutcompleted.aspx");
            }
            else
            {
                SelectPane(CheckoutStepEnum.PaymentInfo);
            }
        }

        protected void BtnModifyShippingAddress_OnClick(object sender, EventArgs e)
        {
            SelectPane(CheckoutStepEnum.ShippingAddress);
        }

        protected void BtnModifyBillingAddress_OnClick(object sender, EventArgs e)
        {
            SelectPane(CheckoutStepEnum.BillingAddress);
        }

        protected void BtnModifyShippingMethod_OnClick(object sender, EventArgs e)
        {
            SelectPane(CheckoutStepEnum.ShippingMethod);
        }

        protected void BtnModifyPaymentMethod_OnClick(object sender, EventArgs e)
        {
            SelectPane(CheckoutStepEnum.PaymentMethod);
        }

        protected void BtnModifyPaymentInfo_OnClick(object sender, EventArgs e)
        {
            SelectPane(CheckoutStepEnum.PaymentInfo);
        }

        public ShoppingCart Cart
        {
            get
            {
                if(_cart == null)
                {
                    _cart = ShoppingCartManager.GetCurrentShoppingCart(ShoppingCartTypeEnum.ShoppingCart);
                }
                return _cart;
            }
        }
    }
}