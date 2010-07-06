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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class AddressAddControl : BaseNopAdministrationUserControl
    {
        protected void BindData()
        {
            Customer customer = CustomerManager.GetCustomerById(this.CustomerId);
            if (customer != null)
            {
                this.lblCustomer.Text = Server.HtmlEncode(customer.Email);
                lnkBack.NavigateUrl = CommonHelper.GetStoreAdminLocation() + "CustomerDetails.aspx?CustomerID=" + customer.CustomerId.ToString();
            }
            else
                Response.Redirect("Customers.aspx");
        }

        protected void FillCountryDropDowns()
        {
            this.ddlCountry.Items.Clear();
            List<Country> countryCollection = null;
            if (IsBillingAddress)
                countryCollection = CountryManager.GetAllCountriesForBilling();
            else
                countryCollection = CountryManager.GetAllCountriesForShipping();
            foreach (Country country in countryCollection)
            {
                ListItem ddlCountryItem2 = new ListItem(country.Name, country.CountryId.ToString());
                this.ddlCountry.Items.Add(ddlCountryItem2);
            }
        }

        protected void FillStateProvinceDropDowns()
        {
            this.ddlStateProvince.Items.Clear();
            int countryId = int.Parse(this.ddlCountry.SelectedItem.Value);

            var stateProvinceCollection = StateProvinceManager.GetStateProvincesByCountryId(countryId);
            foreach (StateProvince stateProvince in stateProvinceCollection)
            {
                ListItem ddlStateProviceItem2 = new ListItem(stateProvince.Name, stateProvince.StateProvinceId.ToString());
                this.ddlStateProvince.Items.Add(ddlStateProviceItem2);
            }
            if (stateProvinceCollection.Count == 0)
            {
                ListItem ddlStateProvinceItem = new ListItem(GetLocaleResourceString("Admin.Common.State.Other"), "0");
                this.ddlStateProvince.Items.Add(ddlStateProvinceItem);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.FillCountryDropDowns();
                this.FillStateProvinceDropDowns();
                this.BindData();
            }
        }

        protected void AddButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    Address address = CustomerManager.InsertAddress(this.CustomerId, this.IsBillingAddress, txtFirstName.Text, txtLastName.Text,
                        txtPhoneNumber.Text, txtEmail.Text, txtFaxNumber.Text, txtCompany.Text,
                        txtAddress1.Text, txtAddress2.Text, txtCity.Text,
                        int.Parse(this.ddlStateProvince.SelectedItem.Value), txtZipPostalCode.Text,
                        int.Parse(this.ddlCountry.SelectedItem.Value), DateTime.UtcNow, DateTime.UtcNow);

                    Response.Redirect("AddressDetails.aspx?AddressID=" + address.AddressId.ToString());
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStateProvinceDropDowns();
        }

        public int CustomerId
        {
            get
            {
                return CommonHelper.QueryStringInt("CustomerId");
            }
        }

        public bool IsBillingAddress
        {
            get
            {
                return CommonHelper.QueryStringBool("IsBillingAddress");
            }
        }
    }
}