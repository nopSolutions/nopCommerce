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
            phUsername.Visible = IoC.Resolve<ICustomerService>().UsernamesEnabled &&
                IoC.Resolve<ICustomerService>().AllowCustomersToChangeUsernames;

            phGender.Visible = IoC.Resolve<ICustomerService>().FormFieldGenderEnabled;
            phDateOfBirth.Visible = IoC.Resolve<ICustomerService>().FormFieldDateOfBirthEnabled;

            phCompanyName.Visible = IoC.Resolve<ICustomerService>().FormFieldCompanyEnabled;
            rfvCompany.Enabled = IoC.Resolve<ICustomerService>().FormFieldCompanyEnabled &&
                IoC.Resolve<ICustomerService>().FormFieldCompanyRequired;
            phVatNumber.Visible = IoC.Resolve<ITaxService>().EUVatEnabled;
            phCompanyDetails.Visible = IoC.Resolve<ICustomerService>().FormFieldCompanyEnabled ||
                IoC.Resolve<ITaxService>().EUVatEnabled;

            phStreetAddress.Visible = IoC.Resolve<ICustomerService>().FormFieldStreetAddressEnabled;
            rfvStreetAddress.Enabled = IoC.Resolve<ICustomerService>().FormFieldStreetAddressEnabled &&
                IoC.Resolve<ICustomerService>().FormFieldStreetAddressRequired;
            phStreetAddress2.Visible = IoC.Resolve<ICustomerService>().FormFieldStreetAddress2Enabled;
            rfvStreetAddress2.Enabled = IoC.Resolve<ICustomerService>().FormFieldStreetAddress2Enabled &&
                IoC.Resolve<ICustomerService>().FormFieldStreetAddress2Required;
            phPostCode.Visible = IoC.Resolve<ICustomerService>().FormFieldPostCodeEnabled;
            rfvZipPostalCode.Enabled = IoC.Resolve<ICustomerService>().FormFieldPostCodeEnabled &&
                IoC.Resolve<ICustomerService>().FormFieldPostCodeRequired;
            phCity.Visible = IoC.Resolve<ICustomerService>().FormFieldCityEnabled;
            rfvCity.Enabled = IoC.Resolve<ICustomerService>().FormFieldCityEnabled &&
                IoC.Resolve<ICustomerService>().FormFieldCityRequired;
            phCountry.Visible = IoC.Resolve<ICustomerService>().FormFieldCountryEnabled;
            phStateProvince.Visible = IoC.Resolve<ICustomerService>().FormFieldCountryEnabled &&
                IoC.Resolve<ICustomerService>().FormFieldStateEnabled;
            phYourAddress.Visible = IoC.Resolve<ICustomerService>().FormFieldStreetAddressEnabled ||
                IoC.Resolve<ICustomerService>().FormFieldStreetAddress2Enabled ||
                IoC.Resolve<ICustomerService>().FormFieldPostCodeEnabled ||
                IoC.Resolve<ICustomerService>().FormFieldCityEnabled ||
                IoC.Resolve<ICustomerService>().FormFieldCountryEnabled;

            phTelephoneNumber.Visible = IoC.Resolve<ICustomerService>().FormFieldPhoneEnabled;
            rfvPhoneNumber.Enabled = IoC.Resolve<ICustomerService>().FormFieldPhoneEnabled &&
                IoC.Resolve<ICustomerService>().FormFieldPhoneRequired;
            phFaxNumber.Visible = IoC.Resolve<ICustomerService>().FormFieldFaxEnabled;
            rfvFaxNumber.Enabled = IoC.Resolve<ICustomerService>().FormFieldFaxEnabled &&
                IoC.Resolve<ICustomerService>().FormFieldFaxRequired;
            phYourContactInformation.Visible = IoC.Resolve<ICustomerService>().FormFieldPhoneEnabled ||
                IoC.Resolve<ICustomerService>().FormFieldFaxEnabled;

            phNewsletter.Visible = IoC.Resolve<ICustomerService>().FormFieldNewsletterEnabled;

            trTimeZone.Visible = DateTimeHelper.AllowCustomersToSetTimeZone;
            trSignature.Visible = IoC.Resolve<IForumService>().ForumsEnabled && IoC.Resolve<IForumService>().SignaturesEnabled;
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
                lblVatNumberStatus.Text = string.Format(GetLocaleResourceString("Account.VATNumberStatus"), IoC.Resolve<ITaxService>().GetVatNumberStatusName(customer.VatNumberStatus));
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

            if (IoC.Resolve<IForumService>().ForumsEnabled && IoC.Resolve<IForumService>().SignaturesEnabled)
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
                        customer = IoC.Resolve<ICustomerService>().SetEmail(customer.CustomerId, txtEmail.Text.Trim());
                    }

                    //username
                    if (IoC.Resolve<ICustomerService>().UsernamesEnabled &&
                       IoC.Resolve<ICustomerService>().AllowCustomersToChangeUsernames)
                    {
                        if (customer.Username.ToLowerInvariant() != txtUsername.Text.ToLowerInvariant().Trim())
                        {
                            customer = IoC.Resolve<ICustomerService>().ChangeCustomerUsername(customer.CustomerId, txtUsername.Text.Trim());
                        }
                    }

                    //form fields
                    if (IoC.Resolve<ICustomerService>().FormFieldGenderEnabled)
                    {
                        if (rbGenderM.Checked)
                            customer.Gender = "M";
                        else
                            customer.Gender = "F";
                    }
                    customer.FirstName = txtFirstName.Text;
                    customer.LastName = txtLastName.Text;
                    if (IoC.Resolve<ICustomerService>().FormFieldDateOfBirthEnabled)
                    {
                        customer.DateOfBirth = dtDateOfBirth.SelectedDate;
                        IoC.Resolve<ICustomerService>().UpdateCustomer(customer);
                    }
                    if (IoC.Resolve<ICustomerService>().FormFieldCompanyEnabled)
                    {
                        customer.Company = txtCompany.Text;
                    }
                    if (IoC.Resolve<ICustomerService>().FormFieldStreetAddressEnabled)
                    {
                        customer.StreetAddress = txtStreetAddress.Text;
                    }
                    if (IoC.Resolve<ICustomerService>().FormFieldStreetAddress2Enabled)
                    {
                        customer.StreetAddress2 = txtStreetAddress2.Text;
                    }
                    if (IoC.Resolve<ICustomerService>().FormFieldPostCodeEnabled)
                    {
                        customer.ZipPostalCode = txtZipPostalCode.Text;
                    }
                    if (IoC.Resolve<ICustomerService>().FormFieldCityEnabled)
                    {
                        customer.City = txtCity.Text;
                    }
                    if (IoC.Resolve<ICustomerService>().FormFieldCountryEnabled)
                    {
                        customer.CountryId = int.Parse(ddlCountry.SelectedItem.Value);
                    }
                    if (IoC.Resolve<ICustomerService>().FormFieldCountryEnabled &&
                        IoC.Resolve<ICustomerService>().FormFieldStateEnabled)
                    {
                        customer.StateProvinceId = int.Parse(ddlStateProvince.SelectedItem.Value);
                    }
                    if (IoC.Resolve<ICustomerService>().FormFieldPhoneEnabled)
                    {
                        customer.PhoneNumber = txtPhoneNumber.Text;
                    }
                    if (IoC.Resolve<ICustomerService>().FormFieldFaxEnabled)
                    {
                        customer.FaxNumber = txtFaxNumber.Text;
                    }
                    if (IoC.Resolve<ICustomerService>().FormFieldNewsletterEnabled)
                    {
                        customer.ReceiveNewsletter = cbNewsletter.Checked;
                    }

                    //set VAT number after country is saved
                    if (IoC.Resolve<ITaxService>().EUVatEnabled)
                    {
                        string prevVatNumber = customer.VatNumber;
                        customer.VatNumber = txtVatNumber.Text;
                        //set VAT number status
                        if (!txtVatNumber.Text.Trim().Equals(prevVatNumber))
                        {
                            string vatName = string.Empty;
                            string vatAddress = string.Empty;
                            customer.VatNumberStatus = IoC.Resolve<ITaxService>().GetVatNumberStatus(IoC.Resolve<ICountryService>().GetCountryById(customer.CountryId), 
                                customer.VatNumber, out vatName, out vatAddress);

                            //admin notification
                            if (!String.IsNullOrEmpty(customer.VatNumber) && IoC.Resolve<ITaxService>().EUVatEmailAdminWhenNewVATSubmitted)
                            {
                                IoC.Resolve<IMessageService>().SendNewVATSubmittedStoreOwnerNotification(customer,
                                    vatName, vatAddress, IoC.Resolve<ILocalizationManager>().DefaultAdminLanguage.LanguageId);
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

                    if (IoC.Resolve<IForumService>().ForumsEnabled && IoC.Resolve<IForumService>().SignaturesEnabled)
                    {
                        customer = IoC.Resolve<ICustomerService>().SetCustomerSignature(customer.CustomerId, txtSignature.Text);
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
            var countryCollection = IoC.Resolve<ICountryService>().GetAllCountriesForRegistration();
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

            var stateProvinceCollection = IoC.Resolve<IStateProvinceService>().GetStateProvincesByCountryId(countryId);
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
