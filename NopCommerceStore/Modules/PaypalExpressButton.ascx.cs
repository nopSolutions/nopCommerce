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
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.PayPal;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class PaypalExpressButton : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //validate payment method availablity
            var ppePaymentMethod = PaymentMethodManager.GetPaymentMethodBySystemKeyword("PayPalExpress");
            if (ppePaymentMethod == null || !ppePaymentMethod.IsActive)
            {
                this.Visible = false;
                return;
            }

            //cart validation
            var cart = ShoppingCartManager.GetCurrentShoppingCart(ShoppingCartTypeEnum.ShoppingCart);
            if (cart.Count == 0)
            {
                this.Visible = false;
                return;
            }

            //order total validation
            decimal? cartTotal = ShoppingCartManager.GetShoppingCartTotal(cart,
                ppePaymentMethod.PaymentMethodId, NopContext.Current.User);
            if (!cartTotal.HasValue || cartTotal.Value == decimal.Zero)
            {
                this.Visible = false;
                return;
            }

            //recurring order support
            if (cart.IsRecurring && PaymentManager.SupportRecurringPayments(ppePaymentMethod.PaymentMethodId) == RecurringPaymentTypeEnum.NotSupported)
            {
                this.Visible = false;
                return;
            }
        }
        
        protected void btnPaypalExpress_Click(object sender, EventArgs e)
        {
            if ((NopContext.Current.User == null) || (NopContext.Current.User.IsGuest && !CustomerManager.AnonymousCheckoutAllowed))
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            var payPalExpress = new PayPalExpressPaymentProcessor();
            var ppePaymentMethod = PaymentMethodManager.GetPaymentMethodBySystemKeyword("PayPalExpress");
            if (ppePaymentMethod != null && ppePaymentMethod.IsActive)
            {
                //apply reward points
                CheckoutPaymentMethodControl checkoutPaymentMethodControl = CommonHelper.FindControlRecursive<CheckoutPaymentMethodControl>(this.Page.Controls);
                if (checkoutPaymentMethodControl != null)
                    checkoutPaymentMethodControl.ApplyRewardPoints();

                //payment
                var cart = ShoppingCartManager.GetCurrentShoppingCart(ShoppingCartTypeEnum.ShoppingCart);
                decimal? cartTotal = ShoppingCartManager.GetShoppingCartTotal(cart, 
                    ppePaymentMethod.PaymentMethodId, NopContext.Current.User);
                if (cartTotal.HasValue && cartTotal.Value > decimal.Zero)
                {
                    string expressCheckoutURL = payPalExpress.SetExpressCheckout(cartTotal.Value,
                        CommonHelper.GetStoreLocation(false) + "paypalexpressreturn.aspx",
                        CommonHelper.GetStoreLocation(false));
                    Response.Redirect(expressCheckoutURL);
                }
            }
        }
    }
}