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
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web
{
    public partial class AddressEditPage : BaseNopPage
    {
        private void BindData()
        {
            AddressEditControl.IsBillingAddress = this.IsBillingAddress;
            if (this.AddressId > 0)
            {
                var address = IoCFactory.Resolve<ICustomerManager>().GetAddressById(this.AddressId);
                if (address != null)
                {
                    lHeaderTitle.Text = GetLocaleResourceString("Address.UpdateAddressTitle");
                    btnSave.Text = GetLocaleResourceString("Address.UpdateAddress");
                    btnDelete.Visible = true;
                    AddressEditControl.IsNew = false;
                    AddressEditControl.IsBillingAddress = address.IsBillingAddress;
                    AddressEditControl.Address = address;
                }
                else
                    Response.Redirect(CommonHelper.GetStoreLocation());
            }
            else
            {
                lHeaderTitle.Text = GetLocaleResourceString("Address.NewAddressTitle");
                btnSave.Text = GetLocaleResourceString("Address.AddAddress");
                btnDelete.Visible = false;
                AddressEditControl.IsNew = true;
                AddressEditControl.IsBillingAddress = this.IsBillingAddress;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string title = GetLocaleResourceString("PageTitle.AddressEdit");
            SEOHelper.RenderTitle(this, title, true);

            CommonHelper.SetResponseNoCache(Response);

            if (NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }
            var address = IoCFactory.Resolve<ICustomerManager>().GetAddressById(this.AddressId);
            if (address != null)
            {
                var addressCustomer = address.Customer;
                if (addressCustomer == null || addressCustomer.CustomerId != NopContext.Current.User.CustomerId)
                {
                    string loginURL = SEOHelper.GetLoginPageUrl(true);
                    Response.Redirect(loginURL);
                }

                if (DeleteAddress)
                {
                    IoCFactory.Resolve<ICustomerManager>().DeleteAddress(address.AddressId);
                    Response.Redirect(SEOHelper.GetMyAccountUrl());
                }
            }

            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var oldAddress = IoCFactory.Resolve<ICustomerManager>().GetAddressById(this.AddressId);
                var inputedAddress = AddressEditControl.Address;
                if (oldAddress != null)
                {
                    oldAddress.FirstName = inputedAddress.FirstName;
                    oldAddress.LastName = inputedAddress.LastName;
                    oldAddress.PhoneNumber = inputedAddress.PhoneNumber;
                    oldAddress.Email = inputedAddress.Email;
                    oldAddress.FaxNumber = inputedAddress.FaxNumber;
                    oldAddress.Company = inputedAddress.Company;
                    oldAddress.Address1 = inputedAddress.Address1;
                    oldAddress.Address2 = inputedAddress.Address2;
                    oldAddress.City = inputedAddress.City;
                    oldAddress.StateProvinceId = inputedAddress.StateProvinceId;
                    oldAddress.ZipPostalCode = inputedAddress.ZipPostalCode;
                    oldAddress.CountryId = inputedAddress.CountryId;
                    oldAddress.UpdatedOn = DateTime.UtcNow;

                    IoCFactory.Resolve<ICustomerManager>().UpdateAddress(oldAddress);
                }
                else
                {
                    inputedAddress.CustomerId = NopContext.Current.User.CustomerId;
                    inputedAddress.IsBillingAddress = this.IsBillingAddress;
                    inputedAddress.CreatedOn = DateTime.UtcNow;
                    inputedAddress.UpdatedOn = DateTime.UtcNow;
                    IoCFactory.Resolve<ICustomerManager>().InsertAddress(inputedAddress);
                }
                Response.Redirect(SEOHelper.GetMyAccountUrl());
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            IoCFactory.Resolve<ICustomerManager>().DeleteAddress(this.AddressId);
            Response.Redirect(SEOHelper.GetMyAccountUrl());
        }

        public int AddressId
        {
            get
            {
                return CommonHelper.QueryStringInt("AddressId");
            }
        }

        public bool IsBillingAddress
        {
            get
            {
                return CommonHelper.QueryStringBool("IsBillingAddress");
            }
        }
        
        public bool DeleteAddress
        {
            get
            {
                return CommonHelper.QueryStringBool("Delete");
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