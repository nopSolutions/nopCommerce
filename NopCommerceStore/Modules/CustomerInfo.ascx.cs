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
using System.Collections.ObjectModel;
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
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Xml;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CustomerInfoControl : BaseNopUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            phGender.Visible = CustomerManager.FormFieldGenderEnabled;
            phDateOfBirth.Visible = CustomerManager.FormFieldDateOfBirthEnabled;

            phCompanyName.Visible = CustomerManager.FormFieldCompanyEnabled;
            rfvCompany.Enabled = CustomerManager.FormFieldCompanyEnabled &&
                CustomerManager.FormFieldCompanyRequired;
            phVatNumber.Visible = TaxManager.EUVatEnabled;
            phCompanyDetails.Visible = CustomerManager.FormFieldCompanyEnabled ||
                TaxManager.EUVatEnabled;

            phStreetAddress.Visible = CustomerManager.FormFieldStreetAddressEnabled;
            rfvStreetAddress.Enabled = CustomerManager.FormFieldStreetAddressEnabled &&
                CustomerManager.FormFieldStreetAddressRequired;
            phStreetAddress2.Visible = CustomerManager.FormFieldStreetAddress2Enabled;
            rfvStreetAddress2.Enabled = CustomerManager.FormFieldStreetAddress2Enabled &&
                CustomerManager.FormFieldStreetAddress2Required;
            phPostCode.Visible = CustomerManager.FormFieldPostCodeEnabled;
            rfvZipPostalCode.Enabled = CustomerManager.FormFieldPostCodeEnabled &&
                CustomerManager.FormFieldPostCodeRequired;
            phCity.Visible = CustomerManager.FormFieldCityEnabled;
            rfvCity.Enabled = CustomerManager.FormFieldCityEnabled &&
                CustomerManager.FormFieldCityRequired;
            phCountry.Visible = CustomerManager.FormFieldCountryEnabled;
            phStateProvince.Visible = CustomerManager.FormFieldCountryEnabled &&
                CustomerManager.FormFieldStateEnabled;
            phYourAddress.Visible = CustomerManager.FormFieldStreetAddressEnabled ||
                CustomerManager.FormFieldStreetAddress2Enabled ||
                CustomerManager.FormFieldPostCodeEnabled ||
                CustomerManager.FormFieldCityEnabled ||
                CustomerManager.FormFieldCountryEnabled;

            phTelephoneNumber.Visible = CustomerManager.FormFieldPhoneEnabled;
            rfvPhoneNumber.Enabled = CustomerManager.FormFieldPhoneEnabled &&
                CustomerManager.FormFieldPhoneRequired;
            phFaxNumber.Visible = CustomerManager.FormFieldFaxEnabled;
            rfvFaxNumber.Enabled = CustomerManager.FormFieldFaxEnabled &&
                CustomerManager.FormFieldFaxRequired;
            phYourContactInformation.Visible = CustomerManager.FormFieldPhoneEnabled ||
                CustomerManager.FormFieldFaxEnabled;

            phNewsletter.Visible = CustomerManager.FormFieldNewsletterEnabled;

            trTimeZone.Visible = DateTimeHelper.AllowCustomersToSetTimeZone;
            trSignature.Visible = ForumManager.ForumsEnabled && ForumManager.SignaturesEnabled;
            divPreferences.Visible = trTimeZone.Visible || trSignature.Visible;

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }
            
            if (!Page.IsPostBack)
            {
                FillCountryDropDowns();
                FillStateProvinceDropDowns();
                FillTimeZones();
                BindData();
            }
        }

        private void BindData()
        {
            var customer = NopContext.Current.User;

            txtEmail.Text = customer.Email;

            if (String.IsNullOrEmpty(customer.Gender) || 
                customer.Gender.ToLower() == "m")
                rbGenderM.Checked = true;
            else
                rbGenderF.Checked = true;

            txtFirstName.Text = customer.FirstName;
            txtLastName.Text = customer.LastName;

            dtDateOfBirth.SelectedDate = customer.DateOfBirth;

            txtCompany.Text = customer.Company;
            txtVatNumber.Text = customer.VatNumber;
            if (customer.VatNumberStatus != VatNumberStatusEnum.Empty)
            {
                lblVatNumberStatus.Visible = true;
                lblVatNumberStatus.Text = string.Format(GetLocaleResourceString("Account.VATNumberStatus"), TaxManager.GetVatNumberStatusName(customer.VatNumberStatus));
            }
            else
            {
                lblVatNumberStatus.Visible = false;
            }
            txtStreetAddress.Text = customer.StreetAddress;
            txtStreetAddress2.Text = customer.StreetAddress2;
            txtZipPostalCode.Text = customer.ZipPostalCode;
            txtCity.Text = customer.City;
            txtPhoneNumber.Text = customer.PhoneNumber;
            txtFaxNumber.Text = customer.FaxNumber;
            CommonHelper.SelectListItem(ddlCountry, customer.CountryId.ToString());

            FillStateProvinceDropDowns();

            CommonHelper.SelectListItem(ddlStateProvince, customer.StateProvinceId.ToString());

            cbNewsletter.Checked = customer.ReceiveNewsletter;

            if (DateTimeHelper.AllowCustomersToSetTimeZone)
            {
                CommonHelper.SelectListItem(this.ddlTimeZone, DateTimeHelper.CurrentTimeZone.Id);
            }

            if (ForumManager.ForumsEnabled && ForumManager.SignaturesEnabled)
            {
                txtSignature.Text = customer.Signature;
            }
        }

        protected void btnSaveCustomerInfo_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    var customer = NopContext.Current.User;
                    if (customer.Email.ToLower() != txtEmail.Text.ToLower().Trim())
                    {
                        customer = CustomerManager.SetEmail(customer.CustomerId, txtEmail.Text.Trim());
                    }

                    if (CustomerManager.FormFieldGenderEnabled)
                    {
                        if (rbGenderM.Checked)
                            customer.Gender = "M";
                        else
                            customer.Gender = "F";
                    }
                    customer.FirstName = txtFirstName.Text;
                    customer.LastName = txtLastName.Text;
                    if (CustomerManager.FormFieldDateOfBirthEnabled)
                    {
                        customer = CustomerManager.SetCustomerDateOfBirth(customer.CustomerId, dtDateOfBirth.SelectedDate);
                    }
                    if (CustomerManager.FormFieldCompanyEnabled)
                    {
                        customer.Company = txtCompany.Text;
                    }
                    if (CustomerManager.FormFieldStreetAddressEnabled)
                    {
                        customer.StreetAddress = txtStreetAddress.Text;
                    }
                    if (CustomerManager.FormFieldStreetAddress2Enabled)
                    {
                        customer.StreetAddress2 = txtStreetAddress2.Text;
                    }
                    if (CustomerManager.FormFieldPostCodeEnabled)
                    {
                        customer.ZipPostalCode = txtZipPostalCode.Text;
                    }
                    if (CustomerManager.FormFieldCityEnabled)
                    {
                        customer.City = txtCity.Text;
                    }
                    if (CustomerManager.FormFieldCountryEnabled)
                    {
                        customer.CountryId = int.Parse(ddlCountry.SelectedItem.Value);
                    }
                    if (CustomerManager.FormFieldCountryEnabled &&
                        CustomerManager.FormFieldStateEnabled)
                    {
                        customer.StateProvinceId = int.Parse(ddlStateProvince.SelectedItem.Value);
                    }
                    if (CustomerManager.FormFieldPhoneEnabled)
                    {
                        customer.PhoneNumber = txtPhoneNumber.Text;
                    }
                    if (CustomerManager.FormFieldFaxEnabled)
                    {
                        customer.FaxNumber = txtFaxNumber.Text;
                    }
                    if (CustomerManager.FormFieldNewsletterEnabled)
                    {
                        customer.ReceiveNewsletter = cbNewsletter.Checked;
                    }

                    //set VAT number after country is saved
                    if (TaxManager.EUVatEnabled)
                    {
                        string prevVatNumber = customer.VatNumber;
                        customer.VatNumber = txtVatNumber.Text;
                        //set VAT number status
                        if (!txtVatNumber.Text.Trim().Equals(prevVatNumber))
                        {
                            customer.VatNumberStatus = TaxManager.GetVatNumberStatus(CountryManager.GetCountryById(customer.CountryId), customer.VatNumber);

                            //admin notification
                            if (!String.IsNullOrEmpty(customer.VatNumber) && TaxManager.EUVatEmailAdminWhenNewVATSubmitted)
                            {
                                MessageManager.SendNewVATSubmittedStoreOwnerNotification(customer, LocalizationManager.DefaultAdminLanguage.LanguageId);
                            }
                        }
                    }

                    if (DateTimeHelper.AllowCustomersToSetTimeZone)
                    {
                        if (ddlTimeZone.SelectedItem != null && !String.IsNullOrEmpty(ddlTimeZone.SelectedItem.Value))
                        {
                            string timeZoneId = ddlTimeZone.SelectedItem.Value;
                            DateTimeHelper.CurrentTimeZone = DateTimeHelper.FindTimeZoneById(timeZoneId);
                        }
                    }

                    if (ForumManager.ForumsEnabled && ForumManager.SignaturesEnabled)
                    {
                        customer = CustomerManager.SetCustomerSignature(customer.CustomerId, txtSignature.Text);
                    }

                    //bind data
                    BindData();
                }
                catch (Exception exc)
                {
                    ErrorMessage.Text = exc.Message;
                }
            }
        }

        private void FillCountryDropDowns()
        {
            ddlCountry.Items.Clear();
            var countryCollection = CountryManager.GetAllCountriesForRegistration();
            foreach (var country in countryCollection)
            {
                var ddlCountryItem2 = new ListItem(country.Name, country.CountryId.ToString());
                ddlCountry.Items.Add(ddlCountryItem2);
            }
        }

        private void FillStateProvinceDropDowns()
        {
            ddlStateProvince.Items.Clear();
            int countryId = 0;
            if (ddlCountry.SelectedItem != null)
                countryId = int.Parse(ddlCountry.SelectedItem.Value);

            var stateProvinceCollection = StateProvinceManager.GetStateProvincesByCountryId(countryId);
            foreach (var stateProvince in stateProvinceCollection)
            {
                var ddlStateProviceItem2 = new ListItem(stateProvince.Name, stateProvince.StateProvinceId.ToString());
                ddlStateProvince.Items.Add(ddlStateProviceItem2);
            }
            if (stateProvinceCollection.Count == 0)
            {
                var ddlStateProvinceItem = new ListItem(GetLocaleResourceString("Address.StateProvinceNonUS"), "0");
                ddlStateProvince.Items.Add(ddlStateProvinceItem);
            }
        }

        private void FillTimeZones()
        {
            this.ddlTimeZone.Items.Clear();
            if (DateTimeHelper.AllowCustomersToSetTimeZone)
            {
                var timeZones = DateTimeHelper.GetSystemTimeZones();
                foreach (var timeZone in timeZones)
                {
                    string timeZoneName = timeZone.DisplayName;
                    var ddlTimeZoneItem2 = new ListItem(timeZoneName, timeZone.Id);
                    this.ddlTimeZone.Items.Add(ddlTimeZoneItem2);
                }
            }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStateProvinceDropDowns();
        }
    }
}