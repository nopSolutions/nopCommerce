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

namespace NopSolutions.NopCommerce.Web
{
    public partial class AddressEditPage : BaseNopPage
    {
        private void BindData()
        {
            AddressEditControl.IsBillingAddress = this.IsBillingAddress;
            if (this.AddressId > 0)
            {
                var address = CustomerManager.GetAddressById(this.AddressId);
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
            var address = CustomerManager.GetAddressById(this.AddressId);
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
                    CustomerManager.DeleteAddress(address.AddressId);
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
                var oldAddress = CustomerManager.GetAddressById(this.AddressId);
                var inputedAddress = AddressEditControl.Address;
                if (oldAddress != null)
                {
                    CustomerManager.UpdateAddress(oldAddress.AddressId, oldAddress.CustomerId, oldAddress.IsBillingAddress,
                        inputedAddress.FirstName, inputedAddress.LastName,
                        inputedAddress.PhoneNumber, inputedAddress.Email, inputedAddress.FaxNumber,
                        inputedAddress.Company, inputedAddress.Address1, inputedAddress.Address2,
                        inputedAddress.City, inputedAddress.StateProvinceId, inputedAddress.ZipPostalCode,
                        inputedAddress.CountryId, oldAddress.CreatedOn, DateTime.UtcNow);
                }
                else
                {
                    CustomerManager.InsertAddress(NopContext.Current.User.CustomerId, this.IsBillingAddress,
                        inputedAddress.FirstName, inputedAddress.LastName,
                        inputedAddress.PhoneNumber, inputedAddress.Email, inputedAddress.FaxNumber,
                        inputedAddress.Company, inputedAddress.Address1, inputedAddress.Address2,
                        inputedAddress.City, inputedAddress.StateProvinceId, inputedAddress.ZipPostalCode,
                        inputedAddress.CountryId, DateTime.UtcNow, DateTime.UtcNow);
                }
                Response.Redirect(SEOHelper.GetMyAccountUrl());
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            CustomerManager.DeleteAddress(this.AddressId);
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