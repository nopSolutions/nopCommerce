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
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CustomerInfoControl : BaseNopUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            phUsername.Visible = IoCFactory.Resolve<ICustomerManager>().UsernamesEnabled &&
                IoCFactory.Resolve<ICustomerManager>().AllowCustomersToChangeUsernames;

            phGender.Visible = IoCFactory.Resolve<ICustomerManager>().FormFieldGenderEnabled;
            phDateOfBirth.Visible = IoCFactory.Resolve<ICustomerManager>().FormFieldDateOfBirthEnabled;

            phCompanyName.Visible = IoCFactory.Resolve<ICustomerManager>().FormFieldCompanyEnabled;
            rfvCompany.Enabled = IoCFactory.Resolve<ICustomerManager>().FormFieldCompanyEnabled &&
                IoCFactory.Resolve<ICustomerManager>().FormFieldCompanyRequired;
            phVatNumber.Visible = IoCFactory.Resolve<ITaxManager>().EUVatEnabled;
            phCompanyDetails.Visible = IoCFactory.Resolve<ICustomerManager>().FormFieldCompanyEnabled ||
                IoCFactory.Resolve<ITaxManager>().EUVatEnabled;

            phStreetAddress.Visible = IoCFactory.Resolve<ICustomerManager>().FormFieldStreetAddressEnabled;
            rfvStreetAddress.Enabled = IoCFactory.Resolve<ICustomerManager>().FormFieldStreetAddressEnabled &&
                IoCFactory.Resolve<ICustomerManager>().FormFieldStreetAddressRequired;
            phStreetAddress2.Visible = IoCFactory.Resolve<ICustomerManager>().FormFieldStreetAddress2Enabled;
            rfvStreetAddress2.Enabled = IoCFactory.Resolve<ICustomerManager>().FormFieldStreetAddress2Enabled &&
                IoCFactory.Resolve<ICustomerManager>().FormFieldStreetAddress2Required;
            phPostCode.Visible = IoCFactory.Resolve<ICustomerManager>().FormFieldPostCodeEnabled;
            rfvZipPostalCode.Enabled = IoCFactory.Resolve<ICustomerManager>().FormFieldPostCodeEnabled &&
                IoCFactory.Resolve<ICustomerManager>().FormFieldPostCodeRequired;
            phCity.Visible = IoCFactory.Resolve<ICustomerManager>().FormFieldCityEnabled;
            rfvCity.Enabled = IoCFactory.Resolve<ICustomerManager>().FormFieldCityEnabled &&
                IoCFactory.Resolve<ICustomerManager>().FormFieldCityRequired;
            phCountry.Visible = IoCFactory.Resolve<ICustomerManager>().FormFieldCountryEnabled;
            phStateProvince.Visible = IoCFactory.Resolve<ICustomerManager>().FormFieldCountryEnabled &&
                IoCFactory.Resolve<ICustomerManager>().FormFieldStateEnabled;
            phYourAddress.Visible = IoCFactory.Resolve<ICustomerManager>().FormFieldStreetAddressEnabled ||
                IoCFactory.Resolve<ICustomerManager>().FormFieldStreetAddress2Enabled ||
                IoCFactory.Resolve<ICustomerManager>().FormFieldPostCodeEnabled ||
                IoCFactory.Resolve<ICustomerManager>().FormFieldCityEnabled ||
                IoCFactory.Resolve<ICustomerManager>().FormFieldCountryEnabled;

            phTelephoneNumber.Visible = IoCFactory.Resolve<ICustomerManager>().FormFieldPhoneEnabled;
            rfvPhoneNumber.Enabled = IoCFactory.Resolve<ICustomerManager>().FormFieldPhoneEnabled &&
                IoCFactory.Resolve<ICustomerManager>().FormFieldPhoneRequired;
            phFaxNumber.Visible = IoCFactory.Resolve<ICustomerManager>().FormFieldFaxEnabled;
            rfvFaxNumber.Enabled = IoCFactory.Resolve<ICustomerManager>().FormFieldFaxEnabled &&
                IoCFactory.Resolve<ICustomerManager>().FormFieldFaxRequired;
            phYourContactInformation.Visible = IoCFactory.Resolve<ICustomerManager>().FormFieldPhoneEnabled ||
                IoCFactory.Resolve<ICustomerManager>().FormFieldFaxEnabled;

            phNewsletter.Visible = IoCFactory.Resolve<ICustomerManager>().FormFieldNewsletterEnabled;

            trTimeZone.Visible = DateTimeHelper.AllowCustomersToSetTimeZone;
            trSignature.Visible = IoCFactory.Resolve<IForumManager>().ForumsEnabled && IoCFactory.Resolve<IForumManager>().SignaturesEnabled;
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
                lblVatNumberStatus.Text = string.Format(GetLocaleResourceString("Account.VATNumberStatus"), IoCFactory.Resolve<ITaxManager>().GetVatNumberStatusName(customer.VatNumberStatus));
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

            if (IoCFactory.Resolve<IForumManager>().ForumsEnabled && IoCFactory.Resolve<IForumManager>().SignaturesEnabled)
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
                        customer = IoCFactory.Resolve<ICustomerManager>().SetEmail(customer.CustomerId, txtEmail.Text.Trim());
                    }

                    //username
                    if (IoCFactory.Resolve<ICustomerManager>().UsernamesEnabled &&
                       IoCFactory.Resolve<ICustomerManager>().AllowCustomersToChangeUsernames)
                    {
                        if (customer.Username.ToLowerInvariant() != txtUsername.Text.ToLowerInvariant().Trim())
                        {
                            customer = IoCFactory.Resolve<ICustomerManager>().ChangeCustomerUsername(customer.CustomerId, txtUsername.Text.Trim());
                        }
                    }

                    //form fields
                    if (IoCFactory.Resolve<ICustomerManager>().FormFieldGenderEnabled)
                    {
                        if (rbGenderM.Checked)
                            customer.Gender = "M";
                        else
                            customer.Gender = "F";
                    }
                    customer.FirstName = txtFirstName.Text;
                    customer.LastName = txtLastName.Text;
                    if (IoCFactory.Resolve<ICustomerManager>().FormFieldDateOfBirthEnabled)
                    {
                        customer = IoCFactory.Resolve<ICustomerManager>().SetCustomerDateOfBirth(customer.CustomerId, dtDateOfBirth.SelectedDate);
                    }
                    if (IoCFactory.Resolve<ICustomerManager>().FormFieldCompanyEnabled)
                    {
                        customer.Company = txtCompany.Text;
                    }
                    if (IoCFactory.Resolve<ICustomerManager>().FormFieldStreetAddressEnabled)
                    {
                        customer.StreetAddress = txtStreetAddress.Text;
                    }
                    if (IoCFactory.Resolve<ICustomerManager>().FormFieldStreetAddress2Enabled)
                    {
                        customer.StreetAddress2 = txtStreetAddress2.Text;
                    }
                    if (IoCFactory.Resolve<ICustomerManager>().FormFieldPostCodeEnabled)
                    {
                        customer.ZipPostalCode = txtZipPostalCode.Text;
                    }
                    if (IoCFactory.Resolve<ICustomerManager>().FormFieldCityEnabled)
                    {
                        customer.City = txtCity.Text;
                    }
                    if (IoCFactory.Resolve<ICustomerManager>().FormFieldCountryEnabled)
                    {
                        customer.CountryId = int.Parse(ddlCountry.SelectedItem.Value);
                    }
                    if (IoCFactory.Resolve<ICustomerManager>().FormFieldCountryEnabled &&
                        IoCFactory.Resolve<ICustomerManager>().FormFieldStateEnabled)
                    {
                        customer.StateProvinceId = int.Parse(ddlStateProvince.SelectedItem.Value);
                    }
                    if (IoCFactory.Resolve<ICustomerManager>().FormFieldPhoneEnabled)
                    {
                        customer.PhoneNumber = txtPhoneNumber.Text;
                    }
                    if (IoCFactory.Resolve<ICustomerManager>().FormFieldFaxEnabled)
                    {
                        customer.FaxNumber = txtFaxNumber.Text;
                    }
                    if (IoCFactory.Resolve<ICustomerManager>().FormFieldNewsletterEnabled)
                    {
                        customer.ReceiveNewsletter = cbNewsletter.Checked;
                    }

                    //set VAT number after country is saved
                    if (IoCFactory.Resolve<ITaxManager>().EUVatEnabled)
                    {
                        string prevVatNumber = customer.VatNumber;
                        customer.VatNumber = txtVatNumber.Text;
                        //set VAT number status
                        if (!txtVatNumber.Text.Trim().Equals(prevVatNumber))
                        {
                            string vatName = string.Empty;
                            string vatAddress = string.Empty;
                            customer.VatNumberStatus = IoCFactory.Resolve<ITaxManager>().GetVatNumberStatus(IoCFactory.Resolve<ICountryManager>().GetCountryById(customer.CountryId), 
                                customer.VatNumber, out vatName, out vatAddress);

                            //admin notification
                            if (!String.IsNullOrEmpty(customer.VatNumber) && IoCFactory.Resolve<ITaxManager>().EUVatEmailAdminWhenNewVATSubmitted)
                            {
                                IoCFactory.Resolve<IMessageManager>().SendNewVATSubmittedStoreOwnerNotification(customer,
                                    vatName, vatAddress, LocalizationManager.DefaultAdminLanguage.LanguageId);
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

                    if (IoCFactory.Resolve<IForumManager>().ForumsEnabled && IoCFactory.Resolve<IForumManager>().SignaturesEnabled)
                    {
                        customer = IoCFactory.Resolve<ICustomerManager>().SetCustomerSignature(customer.CustomerId, txtSignature.Text);
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
            var countryCollection = IoCFactory.Resolve<ICountryManager>().GetAllCountriesForRegistration();
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

            var stateProvinceCollection = IoCFactory.Resolve<IStateProvinceManager>().GetStateProvincesByCountryId(countryId);
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
