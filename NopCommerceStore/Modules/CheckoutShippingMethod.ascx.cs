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
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;


namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CheckoutShippingMethodControl : BaseNopUserControl
    {
        protected CheckoutStepChangedEventHandler handler;
        protected ShoppingCart cart = null;

        protected string FormatShippingOption(ShippingOption shippingOption)
        {
            //calculate discounted and taxed rate
            Discount appliedDiscount = null;
            decimal shippingTotalWithoutDiscount = shippingOption.Rate;
            decimal discountAmount = ShippingManager.GetShippingDiscount(NopContext.Current.User, 
                shippingTotalWithoutDiscount, out appliedDiscount);
            decimal shippingTotalWithDiscount = shippingTotalWithoutDiscount - discountAmount;
            if (shippingTotalWithDiscount < decimal.Zero)
                shippingTotalWithDiscount = decimal.Zero;
            shippingTotalWithDiscount = Math.Round(shippingTotalWithDiscount, 2);

            decimal rateBase = TaxManager.GetShippingPrice(shippingTotalWithDiscount, NopContext.Current.User);
            decimal rate = CurrencyManager.ConvertCurrency(rateBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
            string rateStr = PriceHelper.FormatShippingPrice(rate, true);
            return string.Format("({0})", rateStr);
        }

        protected ShippingOption SelectedShippingOption
        {
            get
            {
                ShippingOption shippingOption = null;
                foreach (DataListItem item in this.dlShippingOptions.Items)
                {
                    var rdShippingOption = (RadioButton)item.FindControl("rdShippingOption");
                    var hfShippingRateComputationMethodId = (HiddenField)item.FindControl("hfShippingRateComputationMethodId");
                    var hfName = (HiddenField)item.FindControl("hfName");

                    if (rdShippingOption.Checked)
                    {
                        int shippingRateComputationMethodId = Convert.ToInt32(hfShippingRateComputationMethodId.Value);
                        string name = hfName.Value;

                        string error = string.Empty;
                        var shippingOptions = ShippingManager.GetShippingOptions(Cart, NopContext.Current.User, NopContext.Current.User.ShippingAddress, shippingRateComputationMethodId, ref error);
                        shippingOption = shippingOptions.Find((so) => so.Name == name);
                        break;
                    }
                }
                return shippingOption;
            }
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
            bool shoppingCartRequiresShipping = ShippingManager.ShoppingCartRequiresShipping(Cart);
            if (!shoppingCartRequiresShipping)
            {
                NopContext.Current.User.LastShippingOption = null;
                var args1 = new CheckoutStepEventArgs() { ShippingMethodSelected = true };
                OnCheckoutStepChanged(args1);
                if (!this.OnePageCheckout)
                    Response.Redirect("~/checkoutpaymentmethod.aspx");
            }
            else
            {
                string error = string.Empty;
                Address address = NopContext.Current.User.ShippingAddress;
                var shippingOptions = ShippingManager.GetShippingOptions(Cart, NopContext.Current.User, address, ref error);
                if (!String.IsNullOrEmpty(error))
                {
                    LogManager.InsertLog(LogTypeEnum.ShippingError, error, error);
                    phSelectShippingMethod.Visible = false;
                    lShippingMethodsError.Text = Server.HtmlEncode(error);
                }
                else
                {
                    if (shippingOptions.Count > 0)
                    {
                        phSelectShippingMethod.Visible = true;
                        dlShippingOptions.DataSource = shippingOptions;
                        dlShippingOptions.DataBind();
                    }
                    else
                    {
                        phSelectShippingMethod.Visible = false;
                        lShippingMethodsError.Text = GetLocaleResourceString("Checkout.ShippingIsNotAllowed");
                    }
                }
            }
        }

        protected void btnNextStep_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var shippingOption = this.SelectedShippingOption;
                if (shippingOption != null)
                {
                    NopContext.Current.User.LastShippingOption = shippingOption;
                    var args1 = new CheckoutStepEventArgs() { ShippingMethodSelected = true };
                    OnCheckoutStepChanged(args1);
                    if (!this.OnePageCheckout)
                        Response.Redirect("~/checkoutpaymentmethod.aspx");
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