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
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common.Utils;
 

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CheckoutShippingAddressControl : BaseNopUserControl
    {
        protected CheckoutStepChangedEventHandler handler;
        protected ShoppingCart cart = null;

        protected Address ShippingAddress
        {
            get
            {
                var address = ctrlShippingAddress.Address;
                if (address.AddressId != 0 && NopContext.Current.User != null)
                {
                    var prevAddress = CustomerManager.GetAddressById(address.AddressId);
                    if (prevAddress.CustomerId != NopContext.Current.User.CustomerId)
                        return null;
                    address.CustomerId = prevAddress.CustomerId;
                    address.CreatedOn = prevAddress.CreatedOn;
                }
                else
                {
                    address.CreatedOn = DateTime.UtcNow;
                }

                return address;
            }
        }

        protected void SelectAddress(Address shippingAddress)
        {
            if (shippingAddress == null)
            {
                NopContext.Current.User = CustomerManager.SetDefaultShippingAddress(NopContext.Current.User.CustomerId, 0);
                var args1 = new CheckoutStepEventArgs() { ShippingAddressSelected = true };
                OnCheckoutStepChanged(args1);
                if (!this.OnePageCheckout)
                    Response.Redirect("~/checkoutbillingaddress.aspx");
                return;
            }

            if (shippingAddress.AddressId == 0)
            {
                //check if address already exists
                var shippingAddress2 = NopContext.Current.User.ShippingAddresses.FindAddress(shippingAddress.FirstName,
                    shippingAddress.LastName, shippingAddress.PhoneNumber, shippingAddress.Email,
                    shippingAddress.FaxNumber, shippingAddress.Company,
                    shippingAddress.Address1, shippingAddress.Address2,
                    shippingAddress.City, shippingAddress.StateProvinceId, shippingAddress.ZipPostalCode,
                    shippingAddress.CountryId);

                if (shippingAddress2 != null)
                {
                    shippingAddress = shippingAddress2;
                }
                else
                {
                    shippingAddress = CustomerManager.InsertAddress(NopContext.Current.User.CustomerId, false, shippingAddress.FirstName,
                              shippingAddress.LastName, shippingAddress.PhoneNumber, shippingAddress.Email,
                              shippingAddress.FaxNumber, shippingAddress.Company, shippingAddress.Address1, shippingAddress.Address2,
                              shippingAddress.City, shippingAddress.StateProvinceId, shippingAddress.ZipPostalCode,
                              shippingAddress.CountryId, DateTime.UtcNow, DateTime.UtcNow);
                }
            }

            NopContext.Current.User = CustomerManager.SetDefaultShippingAddress(NopContext.Current.User.CustomerId, shippingAddress.AddressId);
            var args2 = new CheckoutStepEventArgs() { ShippingAddressSelected = true };
            OnCheckoutStepChanged(args2);
            if (!this.OnePageCheckout)
                Response.Redirect("~/checkoutbillingaddress.aspx");
        }

        protected List<Address> GetAllowedShippingAddresses(Customer customer)
        {
            var addresses = new List<Address>();
            if (customer == null)
                return addresses;

            foreach (var address in customer.ShippingAddresses)
            {
                var country = address.Country;
                if (country != null && country.AllowsShipping)
                {
                    addresses.Add(address);
                }
            }

            return addresses;
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
                SelectAddress(null);
            }
            else
            {
                var addresses = GetAllowedShippingAddresses(NopContext.Current.User);
                if (addresses.Count > 0)
                {
                    dlShippingAddresses.DataSource = addresses;
                    dlShippingAddresses.DataBind();
                    lEnterShippingAddress.Text = GetLocaleResourceString("Checkout.OrEnterNewAddress");
                }
                else
                {
                    pnlSelectShippingAddress.Visible = false;
                    lEnterShippingAddress.Text = GetLocaleResourceString("Checkout.EnterShippingAddress");
                }
            }
        }

        protected void btnSelect_Command(object sender, CommandEventArgs e)
        {
            if (Page.IsValid)
            {
                int addressId = int.Parse(e.CommandArgument.ToString());
                var shippingAddress = CustomerManager.GetAddressById(addressId);
                if (shippingAddress != null && NopContext.Current.User != null)
                {
                    var prevAddress = CustomerManager.GetAddressById(shippingAddress.AddressId);
                    if (prevAddress.CustomerId != NopContext.Current.User.CustomerId)
                        return;
                }

                SelectAddress(shippingAddress);
            }
        }

        protected void btnNextStep_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var shippingAddress = this.ShippingAddress;
                SelectAddress(shippingAddress);
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