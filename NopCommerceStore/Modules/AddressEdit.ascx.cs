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
using System.ComponentModel;
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
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class AddressEdit : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (IsNew)
                {
                    lblShippingAddressId.Text = string.Empty;
                    txtFirstName.Text = string.Empty;
                    txtLastName.Text = string.Empty;
                    txtPhoneNumber.Text = string.Empty;
                    txtEmail.Text = string.Empty;
                    txtFaxNumber.Text = string.Empty;
                    txtCompany.Text = string.Empty;
                    txtAddress1.Text = string.Empty;
                    txtAddress2.Text = string.Empty;

                    txtCity.Text = string.Empty;
                    txtZipPostalCode.Text = string.Empty;
                    if (IsBillingAddress)
                        this.FillCountryDropDownsForBilling();
                    else
                        this.FillCountryDropDownsForShipping();
                    this.FillStateProvinceDropDowns();
                }
            }
        }

        private void FillCountryDropDownsForShipping()
        {
            this.ddlCountry.Items.Clear();
            var countryCollection = CountryManager.GetAllCountriesForShipping();
            foreach (var country in countryCollection)
            {
                var ddlCountryItem2 = new ListItem(country.Name, country.CountryId.ToString());
                this.ddlCountry.Items.Add(ddlCountryItem2);
            }
        }

        private void FillCountryDropDownsForBilling()
        {
            this.ddlCountry.Items.Clear();
            var countryCollection = CountryManager.GetAllCountriesForBilling();
            foreach (var country in countryCollection)
            {
                var ddlCountryItem2 = new ListItem(country.Name, country.CountryId.ToString());
                this.ddlCountry.Items.Add(ddlCountryItem2);
            }
        }

        private void FillStateProvinceDropDowns()
        {
            this.ddlStateProvince.Items.Clear();
            int countryId = 0;
            if (this.ddlCountry.SelectedItem != null)
                countryId = int.Parse(this.ddlCountry.SelectedItem.Value);

            var stateProvinceCollection = StateProvinceManager.GetStateProvincesByCountryId(countryId);
            foreach (var stateProvince in stateProvinceCollection)
            {
                var ddlStateProviceItem2 = new ListItem(stateProvince.Name, stateProvince.StateProvinceId.ToString());
                this.ddlStateProvince.Items.Add(ddlStateProviceItem2);
            }
            if (stateProvinceCollection.Count == 0)
            {
                var ddlStateProvinceItem = new ListItem(GetLocaleResourceString("Address.StateProvinceNonUS"), "0");
                this.ddlStateProvince.Items.Add(ddlStateProvinceItem);
            }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStateProvinceDropDowns();
        }

        public Address Address
        {
            get
            {
                var address = new Address();
                if (!String.IsNullOrEmpty(lblShippingAddressId.Text))
                    address.AddressId = int.Parse(lblShippingAddressId.Text);
                address.FirstName = txtFirstName.Text.Trim();
                address.LastName = txtLastName.Text.Trim();
                address.PhoneNumber = txtPhoneNumber.Text.Trim();
                address.Email = txtEmail.Text.Trim();
                address.FaxNumber = txtFaxNumber.Text.Trim();
                address.Company = txtCompany.Text.Trim();
                address.Address1 = txtAddress1.Text.Trim();
                address.Address2 = txtAddress2.Text.Trim();
                address.City = txtCity.Text.Trim();

                if (this.ddlCountry.SelectedItem == null)
                    throw new NopException("Countries are not populated");
                address.CountryId = int.Parse(this.ddlCountry.SelectedItem.Value);

                if (this.ddlStateProvince.SelectedItem == null)
                    throw new NopException("State/Provinces are not populated");
                var stateProvince = StateProvinceManager.GetStateProvinceById(int.Parse(this.ddlStateProvince.SelectedItem.Value));
                if (stateProvince != null && stateProvince.CountryId == address.CountryId)
                    address.StateProvinceId = stateProvince.StateProvinceId;

                address.ZipPostalCode = txtZipPostalCode.Text.Trim();
                return address;
            }
            set
            {
                Address address = value;
                if (address != null)
                {
                    this.lblShippingAddressId.Text = address.AddressId.ToString();
                    this.txtFirstName.Text = address.FirstName;
                    this.txtLastName.Text = address.LastName;
                    this.txtPhoneNumber.Text = address.PhoneNumber;
                    this.txtEmail.Text = address.Email;
                    this.txtFaxNumber.Text = address.FaxNumber;
                    this.txtCompany.Text = address.Company;
                    this.txtAddress1.Text = address.Address1;
                    this.txtAddress2.Text = address.Address2;
                    this.txtCity.Text = address.City;

                    if (address.IsBillingAddress)
                        this.FillCountryDropDownsForBilling();
                    else
                        this.FillCountryDropDownsForShipping();
                    CommonHelper.SelectListItem(this.ddlCountry, address.CountryId);

                    FillStateProvinceDropDowns();
                    CommonHelper.SelectListItem(this.ddlStateProvince, address.StateProvinceId);

                    this.txtZipPostalCode.Text = address.ZipPostalCode;
                }
            }
        }

        [DefaultValue(true)]
        public bool IsNew
        {
            get
            {
                object obj2 = this.ViewState["IsNew"];
                return ((obj2 != null) && ((bool)obj2));

            }
            set
            {
                this.ViewState["IsNew"] = value;
            }
        }

        [DefaultValue(true)]
        public bool IsBillingAddress
        {
            get
            {
                object obj2 = this.ViewState["IsBillingAddress"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["IsBillingAddress"] = value;
            }
        }

        public string ValidationGroup
        {
            get
            {
                return txtFirstName.ValidationGroup;
            }
            set
            {
                txtFirstName.ValidationGroup = value;
                txtLastName.ValidationGroup = value;
                txtPhoneNumber.ValidationGroup = value;
                txtEmail.ValidationGroup = value;
                txtFaxNumber.ValidationGroup = value;
                txtCompany.ValidationGroup = value;
                txtAddress1.ValidationGroup = value;
                txtAddress2.ValidationGroup = value;
                txtCity.ValidationGroup = value;
                txtZipPostalCode.ValidationGroup = value;
            }
        }
    }
}