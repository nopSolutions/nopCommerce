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
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CheckoutBillingAddressControl : BaseNopUserControl
    {
        protected CheckoutStepChangedEventHandler handler;
        protected ShoppingCart cart = null;

        protected Address BillingAddress
        {
            get
            {
                var address = ctrlBillingAddress.Address;
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

        protected void SelectAddress(Address billingAddress)
        {
            if (billingAddress == null)
            {
                NopContext.Current.User = CustomerManager.SetDefaultBillingAddress(NopContext.Current.User.CustomerId, 0);
                var args1 = new CheckoutStepEventArgs() { BillingAddressSelected = true };
                OnCheckoutStepChanged(args1);
                if (!this.OnePageCheckout) 
                    Response.Redirect("~/checkoutshippingmethod.aspx");
                return;
            }

            if (billingAddress.AddressId == 0)
            {
                //check if address already exists
                var billingAddress2 = NopContext.Current.User.BillingAddresses.FindAddress(billingAddress.FirstName,
                     billingAddress.LastName, billingAddress.PhoneNumber, billingAddress.Email,
                     billingAddress.FaxNumber, billingAddress.Company,
                     billingAddress.Address1, billingAddress.Address2,
                     billingAddress.City, billingAddress.StateProvinceId, billingAddress.ZipPostalCode,
                     billingAddress.CountryId);

                if (billingAddress2 != null)
                {
                    billingAddress = billingAddress2;
                }
                else
                {
                    billingAddress = CustomerManager.InsertAddress(NopContext.Current.User.CustomerId, true, billingAddress.FirstName,
                        billingAddress.LastName, billingAddress.PhoneNumber, billingAddress.Email,
                        billingAddress.FaxNumber, billingAddress.Company, billingAddress.Address1, billingAddress.Address2,
                        billingAddress.City, billingAddress.StateProvinceId, billingAddress.ZipPostalCode,
                        billingAddress.CountryId, DateTime.UtcNow, DateTime.UtcNow);
                }
            }

            NopContext.Current.User = CustomerManager.SetDefaultBillingAddress(NopContext.Current.User.CustomerId, billingAddress.AddressId);
            var args2 = new CheckoutStepEventArgs() { BillingAddressSelected = true };
            OnCheckoutStepChanged(args2);
            if (!this.OnePageCheckout)
                Response.Redirect("~/checkoutshippingmethod.aspx");
        }
        
        protected List<Address> GetAllowedBillingAddresses(Customer customer)
        {
            var addresses = new List<Address>();
            if (customer == null)
                return addresses;

            foreach (var address in customer.BillingAddresses)
            {
                var country = address.Country;
                if (country != null && country.AllowsBilling)
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
            var shippingAddress = NopContext.Current.User.ShippingAddress;
            pnlTheSameAsShippingAddress.Visible = CustomerManager.CanUseAddressAsBillingAddress(shippingAddress);

            var addresses = GetAllowedBillingAddresses(NopContext.Current.User);

            if (addresses.Count > 0)
            {
                //bind data
                dlBillingAddresses.DataSource = addresses;
                dlBillingAddresses.DataBind();
                lEnterBillingAddress.Text = GetLocaleResourceString("Checkout.OrEnterNewAddress");
            }
            else
            {
                pnlSelectBillingAddress.Visible = false;
                lEnterBillingAddress.Text = GetLocaleResourceString("Checkout.EnterBillingAddress");
            }
        }

        protected void btnSelect_Command(object sender, CommandEventArgs e)
        {
            if (Page.IsValid)
            {
                int addressId = int.Parse(e.CommandArgument.ToString());
                var billingAddress = CustomerManager.GetAddressById(addressId);
                if (billingAddress != null && NopContext.Current.User != null)
                {
                    var prevAddress = CustomerManager.GetAddressById(billingAddress.AddressId);
                    if (prevAddress.CustomerId != NopContext.Current.User.CustomerId)
                        return;
                }

                SelectAddress(billingAddress);
            }
        }

        protected void btnNextStep_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var billingAddress = this.BillingAddress;
                SelectAddress(billingAddress);
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

        protected void btnTheSameAsShippingAddress_Click(object sender, EventArgs e)
        {
            var shippingAddress = NopContext.Current.User.ShippingAddress;
            if (shippingAddress != null && CustomerManager.CanUseAddressAsBillingAddress(shippingAddress))
            {
                var billingAddress = new Address();
                billingAddress.AddressId = 0;
                billingAddress.CustomerId = shippingAddress.CustomerId;
                billingAddress.IsBillingAddress = true;
                billingAddress.FirstName = shippingAddress.FirstName;
                billingAddress.LastName = shippingAddress.LastName;
                billingAddress.PhoneNumber = shippingAddress.PhoneNumber;
                billingAddress.Email = shippingAddress.Email;
                billingAddress.FaxNumber = shippingAddress.FaxNumber;
                billingAddress.Company = shippingAddress.Company;
                billingAddress.Address1 = shippingAddress.Address1;
                billingAddress.Address2 = shippingAddress.Address2;
                billingAddress.City = shippingAddress.City;
                billingAddress.StateProvinceId = shippingAddress.StateProvinceId;
                billingAddress.ZipPostalCode = shippingAddress.ZipPostalCode;
                billingAddress.CountryId = shippingAddress.CountryId;
                billingAddress.CreatedOn = shippingAddress.CreatedOn;

                ctrlBillingAddress.Address = billingAddress;
            }
            else
            {
                pnlTheSameAsShippingAddress.Visible = false;
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