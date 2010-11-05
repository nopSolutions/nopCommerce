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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using System.Collections.Generic;
using NopSolutions.NopCommerce.BusinessLogic.IoC;
 

namespace NopSolutions.NopCommerce.Web
{
    public partial class CheckoutPage : BaseNopPage
    {
        protected ShoppingCart cart = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            CommonHelper.SetResponseNoCache(Response);

            if (this.Cart.Count == 0)
                Response.Redirect(SEOHelper.GetShoppingCartUrl());

            //user validation
            if (NopContext.Current.User == null && IoCFactory.Resolve<ICustomerService>().AnonymousCheckoutAllowed)
            {
                //create anonymous record
                IoCFactory.Resolve<ICustomerService>().CreateAnonymousUser();
            }

            if ((NopContext.Current.User == null) || (NopContext.Current.User.IsGuest && !IoCFactory.Resolve<ICustomerService>().AnonymousCheckoutAllowed))
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            //reset checkout data
            IoCFactory.Resolve<ICustomerService>().ResetCheckoutData(NopContext.Current.User.CustomerId, false);


            //validation
            var scWarnings = IoCFactory.Resolve<IShoppingCartService>().GetShoppingCartWarnings(Cart, NopContext.Current.User.CheckoutAttributes, true);
            if (scWarnings.Count > 0)
            {
                Response.Redirect(SEOHelper.GetShoppingCartUrl());
            }
            else
            {
                foreach (ShoppingCartItem sci in this.Cart)
                {
                    var sciWarnings = IoCFactory.Resolve<IShoppingCartService>().GetShoppingCartItemWarnings(
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
            
            if (IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Checkout.UseOnePageCheckout"))
            {
                Response.Redirect("~/checkoutonepage.aspx");
            }
            else
            {
                Response.Redirect("~/checkoutshippingaddress.aspx");
            }
        }

        public ShoppingCart Cart
        {
            get
            {
                if (cart == null)
                {
                    cart = IoCFactory.Resolve<IShoppingCartService>().GetCurrentShoppingCart(ShoppingCartTypeEnum.ShoppingCart);
                }
                return cart;
            }
        }

        public override PageSslProtectionEnum SslProtected
        {
            get
            {
                return PageSslProtectionEnum.Yes;
            }
        }
    }
} 