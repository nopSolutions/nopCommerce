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
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CustomerInfoControl: BaseNopFrontendUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            phUsername.Visible = this.CustomerService.UsernamesEnabled &&
                this.CustomerService.AllowCustomersToChangeUsernames;

            phGender.Visible = this.CustomerService.FormFieldGenderEnabled;
            phDateOfBirth.Visible = this.CustomerService.FormFieldDateOfBirthEnabled;

            phCompanyName.Visible = this.CustomerService.FormFieldCompanyEnabled;
            rfvCompany.Enabled = this.CustomerService.FormFieldCompanyEnabled &&
                this.CustomerService.FormFieldCompanyRequired;
            phVatNumber.Visible = this.TaxService.EUVatEnabled;
            phCompanyDetails.Visible = this.CustomerService.FormFieldCompanyEnabled ||
                this.TaxService.EUVatEnabled;

            phStreetAddress.Visible = this.CustomerService.FormFieldStreetAddressEnabled;
            rfvStreetAddress.Enabled = this.CustomerService.FormFieldStreetAddressEnabled &&
                this.CustomerService.FormFieldStreetAddressRequired;
            phStreetAddress2.Visible = this.CustomerService.FormFieldStreetAddress2Enabled;
            rfvStreetAddress2.Enabled = this.CustomerService.FormFieldStreetAddress2Enabled &&
                this.CustomerService.FormFieldStreetAddress2Required;
            phPostCode.Visible = this.CustomerService.FormFieldPostCodeEnabled;
            rfvZipPostalCode.Enabled = this.CustomerService.FormFieldPostCodeEnabled &&
                this.CustomerService.FormFieldPostCodeRequired;
            phCity.Visible = this.CustomerService.FormFieldCityEnabled;
            rfvCity.Enabled = this.CustomerService.FormFieldCityEnabled &&
                this.CustomerService.FormFieldCityRequired;
            phCountry.Visible = this.CustomerService.FormFieldCountryEnabled;
            phStateProvince.Visible = this.CustomerService.FormFieldCountryEnabled &&
                this.CustomerService.FormFieldStateEnabled;
            phYourAddress.Visible = this.CustomerService.FormFieldStreetAddressEnabled ||
                this.CustomerService.FormFieldStreetAddress2Enabled ||
                this.CustomerService.FormFieldPostCodeEnabled ||
                this.CustomerService.FormFieldCityEnabled ||
                this.CustomerService.FormFieldCountryEnabled;

            phTelephoneNumber.Visible = this.CustomerService.FormFieldPhoneEnabled;
            rfvPhoneNumber.Enabled = this.CustomerService.FormFieldPhoneEnabled &&
                this.CustomerService.FormFieldPhoneRequired;
            phFaxNumber.Visible = this.CustomerService.FormFieldFaxEnabled;
            rfvFaxNumber.Enabled = this.CustomerService.FormFieldFaxEnabled &&
                this.CustomerService.FormFieldFaxRequired;
            phYourContactInformation.Visible = this.CustomerService.FormFieldPhoneEnabled ||
                this.CustomerService.FormFieldFaxEnabled;

            phNewsletter.Visible = this.CustomerService.FormFieldNewsletterEnabled;

            trTimeZone.Visible = DateTimeHelper.AllowCustomersToSetTimeZone;
            trSignature.Visible = this.ForumService.ForumsEnabled && this.ForumService.SignaturesEnabled;
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
                this.FillCountryDropDowns();
                this.FillStateProvinceDropDowns();
                this.FillTimeZones();
                this.BindData();
            }
        }

        private void BindData()
        {
            var customer = NopContext.Current.User;

            txtEmail.Text = customer.Email;
            txtUsername.Text = customer.Username;

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
                lblVatNumberStatus.Text = string.Format(GetLocaleResourceString("Account.VATNumberStatus"), this.TaxService.GetVatNumberStatusName(customer.VatNumberStatus));
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

            if (this.ForumService.ForumsEnabled && this.ForumService.SignaturesEnabled)
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

                    //email
                    if (customer.Email.ToLowerInvariant() != txtEmail.Text.ToLowerInvariant().Trim())
                    {
                        customer = this.CustomerService.SetEmail(customer.CustomerId, txtEmail.Text.Trim());
                    }

                    //username
                    if (this.CustomerService.UsernamesEnabled &&
                       this.CustomerService.AllowCustomersToChangeUsernames)
                    {
                        if (customer.Username.ToLowerInvariant() != txtUsername.Text.ToLowerInvariant().Trim())
                        {
                            customer = this.CustomerService.ChangeCustomerUsername(customer.CustomerId, txtUsername.Text.Trim());
                        }
                    }

                    //form fields
                    if (this.CustomerService.FormFieldGenderEnabled)
                    {
                        if (rbGenderM.Checked)
                            customer.Gender = "M";
                        else
                            customer.Gender = "F";
                    }
                    customer.FirstName = txtFirstName.Text;
                    customer.LastName = txtLastName.Text;
                    if (this.CustomerService.FormFieldDateOfBirthEnabled)
                    {
                        customer.DateOfBirth = dtDateOfBirth.SelectedDate;
                        this.CustomerService.UpdateCustomer(customer);
                    }
                    if (this.CustomerService.FormFieldCompanyEnabled)
                    {
                        customer.Company = txtCompany.Text;
                    }
                    if (this.CustomerService.FormFieldStreetAddressEnabled)
                    {
                        customer.StreetAddress = txtStreetAddress.Text;
                    }
                    if (this.CustomerService.FormFieldStreetAddress2Enabled)
                    {
                        customer.StreetAddress2 = txtStreetAddress2.Text;
                    }
                    if (this.CustomerService.FormFieldPostCodeEnabled)
                    {
                        customer.ZipPostalCode = txtZipPostalCode.Text;
                    }
                    if (this.CustomerService.FormFieldCityEnabled)
                    {
                        customer.City = txtCity.Text;
                    }
                    if (this.CustomerService.FormFieldCountryEnabled)
                    {
                        customer.CountryId = int.Parse(ddlCountry.SelectedItem.Value);
                    }
                    if (this.CustomerService.FormFieldCountryEnabled &&
                        this.CustomerService.FormFieldStateEnabled)
                    {
                        customer.StateProvinceId = int.Parse(ddlStateProvince.SelectedItem.Value);
                    }
                    if (this.CustomerService.FormFieldPhoneEnabled)
                    {
                        customer.PhoneNumber = txtPhoneNumber.Text;
                    }
                    if (this.CustomerService.FormFieldFaxEnabled)
                    {
                        customer.FaxNumber = txtFaxNumber.Text;
                    }
                    if (this.CustomerService.FormFieldNewsletterEnabled)
                    {
                        customer.ReceiveNewsletter = cbNewsletter.Checked;
                    }

                    //set VAT number after country is saved
                    if (this.TaxService.EUVatEnabled)
                    {
                        string prevVatNumber = customer.VatNumber;
                        customer.VatNumber = txtVatNumber.Text;
                        //set VAT number status
                        if (!txtVatNumber.Text.Trim().Equals(prevVatNumber))
                        {
                            string vatName = string.Empty;
                            string vatAddress = string.Empty;
                            customer.VatNumberStatus = this.TaxService.GetVatNumberStatus(this.CountryService.GetCountryById(customer.CountryId), 
                                customer.VatNumber, out vatName, out vatAddress);

                            //admin notification
                            if (!String.IsNullOrEmpty(customer.VatNumber) && this.TaxService.EUVatEmailAdminWhenNewVATSubmitted)
                            {
                                this.MessageService.SendNewVATSubmittedStoreOwnerNotification(customer,
                                    vatName, vatAddress, this.LocalizationManager.DefaultAdminLanguage.LanguageId);
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

                    if (this.ForumService.ForumsEnabled && this.ForumService.SignaturesEnabled)
                    {
                        customer = this.CustomerService.SetCustomerSignature(customer.CustomerId, txtSignature.Text);
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
            var countryCollection = this.CountryService.GetAllCountriesForRegistration();
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

            var stateProvinceCollection = this.StateProvinceService.GetStateProvincesByCountryId(countryId);
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
