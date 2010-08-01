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
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.PayPal;

namespace NopSolutions.NopCommerce.Web
{
    public partial class PaypalExpressReturnPage : BaseNopPage
    {
        protected void btnNextStep_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    PayPalExpressPaymentProcessor payPalExpress = new PayPalExpressPaymentProcessor();
                    string token = CommonHelper.QueryString("token");
                    PaypalPayer payer = payPalExpress.GetExpressCheckout(token);
                    if (string.IsNullOrEmpty(payer.PayerID))
                        throw new NopException("Payer ID is not set");

                    PaymentInfo paymentInfo = new PaymentInfo();

                    PaymentMethod paypalExpressPaymentMethod = PaymentMethodManager.GetPaymentMethodBySystemKeyword("PayPalExpress");
            
                    paymentInfo.PaymentMethodId = paypalExpressPaymentMethod.PaymentMethodId;
                    paymentInfo.BillingAddress = NopContext.Current.User.BillingAddress;
                    paymentInfo.ShippingAddress = NopContext.Current.User.ShippingAddress;
                    paymentInfo.PaypalPayerId = payer.PayerID;
                    paymentInfo.PaypalToken = token;
                    paymentInfo.CustomerLanguage = NopContext.Current.WorkingLanguage;
                    paymentInfo.CustomerCurrency = NopContext.Current.WorkingCurrency;

                    int orderId = 0;
                    string result = OrderManager.PlaceOrder(paymentInfo, NopContext.Current.User, out orderId);

                    Order order = OrderManager.GetOrderById(orderId);
                    if (!String.IsNullOrEmpty(result))
                    {
                        lConfirmOrderError.Text = Server.HtmlEncode(result);
                        btnNextStep.Visible = false;
                        return;
                    }
                    else
                        PaymentManager.PostProcessPayment(order);
                    Response.Redirect("~/checkoutcompleted.aspx");
                }
                catch (Exception exc)
                {
                    LogManager.InsertLog(LogTypeEnum.OrderError, exc.Message, exc);
                    lConfirmOrderError.Text = Server.HtmlEncode(exc.ToString());
                    btnNextStep.Visible = false;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CommonHelper.SetResponseNoCache(Response);

            if ((NopContext.Current.User == null) || (NopContext.Current.User.IsGuest && !CustomerManager.AnonymousCheckoutAllowed))
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            ShoppingCart cart = ShoppingCartManager.GetCurrentShoppingCart(ShoppingCartTypeEnum.ShoppingCart);
            if (cart.Count == 0)
                Response.Redirect(SEOHelper.GetShoppingCartUrl());

            this.btnNextStep.Attributes.Add("onclick", "this.disabled = true;" + Page.ClientScript.GetPostBackEventReference(this.btnNextStep, ""));
        }

        public override bool AllowGuestNavigation
        {
            get
            {
                return true;
            }
        }
    }
}