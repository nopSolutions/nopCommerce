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
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CustomerInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            pnlTimeZone.Visible = DateTimeHelper.AllowCustomersToSetTimeZone;
            pnlUsername.Visible = IoC.Resolve<ICustomerService>().UsernamesEnabled;

            Customer customer = IoC.Resolve<ICustomerService>().GetCustomerById(this.CustomerId);
            if (customer != null)
            {
                this.txtEmail.Text = customer.Email;

                this.txtUsername.Text = customer.Username;
                this.lblUsername.Text = customer.Username;
                if (IoC.Resolve<ICustomerService>().UsernamesEnabled)
                {
                    if (IoC.Resolve<ICustomerService>().AllowCustomersToChangeUsernames)
                    {
                        this.txtUsername.Visible = true;
                        this.lblUsername.Visible = false;
                    }
                    else
                    {
                        this.txtUsername.Visible = false;
                        this.lblUsername.Visible = true;
                    }
                }

                if (IoC.Resolve<ICustomerService>().FormFieldGenderEnabled)
                {
                    if (String.IsNullOrEmpty(customer.Gender) ||
                        customer.Gender.ToLower() == "m")
                        this.rbGenderM.Checked = true;
                    else
                        this.rbGenderF.Checked = true;
                }

                txtFirstName.Text = customer.FirstName;
                txtLastName.Text = customer.LastName;
                if (IoC.Resolve<ICustomerService>().FormFieldDateOfBirthEnabled)
                {
                    ctrlDateOfBirthDatePicker.SelectedDate = customer.DateOfBirth;
                }
                if (IoC.Resolve<ICustomerService>().FormFieldCompanyEnabled)
                {
                    txtCompany.Text = customer.Company;
                }
                if (IoC.Resolve<ITaxService>().EUVatEnabled)
                {
                    txtVatNumber.Text = customer.VatNumber;
                    lblVatNumberStatus.Text = string.Format(GetLocaleResourceString("Admin.CustomerInfo.VATNumberStatus"), IoC.Resolve<ITaxService>().GetVatNumberStatusName(customer.VatNumberStatus));
                }

                if (IoC.Resolve<ICustomerService>().FormFieldStreetAddressEnabled)
                {
                    txtStreetAddress.Text = customer.StreetAddress;
                }
                if (IoC.Resolve<ICustomerService>().FormFieldStreetAddress2Enabled)
                {
                    txtStreetAddress2.Text = customer.StreetAddress2;
                }
                if (IoC.Resolve<ICustomerService>().FormFieldPostCodeEnabled)
                {
                    txtZipPostalCode.Text = customer.ZipPostalCode;
                }
                if (IoC.Resolve<ICustomerService>().FormFieldCityEnabled)
                {
                    txtCity.Text = customer.City;
                }
                if (IoC.Resolve<ICustomerService>().FormFieldCountryEnabled)
                {
                    CommonHelper.SelectListItem(this.ddlCountry, customer.CountryId.ToString());
                }
                if (IoC.Resolve<ICustomerService>().FormFieldCountryEnabled &&
                    IoC.Resolve<ICustomerService>().FormFieldStateEnabled)
                {
                    FillStateProvinceDropDowns();
                    CommonHelper.SelectListItem(this.ddlStateProvince, customer.StateProvinceId.ToString());
                }
                if (IoC.Resolve<ICustomerService>().FormFieldPhoneEnabled)
                {
                    txtPhoneNumber.Text = customer.PhoneNumber;
                }
                if (IoC.Resolve<ICustomerService>().FormFieldFaxEnabled)
                {
                    txtFaxNumber.Text = customer.FaxNumber;
                }
                if (IoC.Resolve<ICustomerService>().FormFieldNewsletterEnabled)
                {
                    cbNewsletter.Checked = customer.ReceiveNewsletter;
                }

                if (DateTimeHelper.AllowCustomersToSetTimeZone)
                {
                    CommonHelper.SelectListItem(this.ddlTimeZone, customer.TimeZoneId);
                }

                CommonHelper.SelectListItem(this.ddlAffiliate, customer.AffiliateId);
                this.cbIsTaxExempt.Checked = customer.IsTaxExempt;
                this.cbIsAdmin.Checked = customer.IsAdmin;
                this.cbIsForumModerator.Checked = customer.IsForumModerator;
                this.txtAdminComment.Text = customer.AdminComment;
                this.cbActive.Checked = customer.Active;
                this.pnlRegistrationDate.Visible = true;
                this.lblRegistrationDate.Text = DateTimeHelper.ConvertToUserTime(customer.RegistrationDate, DateTimeKind.Utc).ToString();
            }
            else
            {
                if (IoC.Resolve<ICustomerService>().UsernamesEnabled)
                {
                    txtUsername.Visible = true;
                    lblUsername.Visible = false;
                }
                btnChangePassword.Visible = false;
                if (DateTimeHelper.AllowCustomersToSetTimeZone)
                {
                    CommonHelper.SelectListItem(this.ddlTimeZone, DateTimeHelper.DefaultStoreTimeZone.Id);
                }
                this.pnlRegistrationDate.Visible = false;
            }
        }

        private void FillAffiliatDropDowns()
        {
            this.ddlAffiliate.Items.Clear();
            ListItem ddlAffiliateItem = new ListItem(GetLocaleResourceString("Admin.CustomerInfo.Affiliate.None"), "0");
            this.ddlAffiliate.Items.Add(ddlAffiliateItem);
            var affiliateCollection = IoC.Resolve<IAffiliateService>().GetAllAffiliates();
            foreach (var affiliate in affiliateCollection)
            {
                ListItem ddlAffiliateItem2 = new ListItem(affiliate.LastName + " (ID=" + affiliate.AffiliateId.ToString() + ")", affiliate.AffiliateId.ToString());
                this.ddlAffiliate.Items.Add(ddlAffiliateItem2);
            }
        }

        private void FillCountryDropDowns()
        {
            ddlCountry.Items.Clear();
            var countryCollection = IoC.Resolve<ICountryService>().GetAllCountriesForRegistration();
            foreach (Country country in countryCollection)
            {
                ListItem ddlCountryItem2 = new ListItem(country.Name, country.CountryId.ToString());
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
            foreach (StateProvince stateProvince in stateProvinceCollection)
            {
                ListItem ddlStateProviceItem2 = new ListItem(stateProvince.Name, stateProvince.StateProvinceId.ToString());
                ddlStateProvince.Items.Add(ddlStateProviceItem2);
            }
            if (stateProvinceCollection.Count == 0)
            {
                ListItem ddlStateProvinceItem = new ListItem(GetLocaleResourceString("Admin.Common.State.Other"), "0");
                ddlStateProvince.Items.Add(ddlStateProvinceItem);
            }
        }

        private void FillTimeZones()
        {
            this.ddlTimeZone.Items.Clear();
            if (DateTimeHelper.AllowCustomersToSetTimeZone)
            {
                ReadOnlyCollection<TimeZoneInfo> timeZones = DateTimeHelper.GetSystemTimeZones();
                foreach (TimeZoneInfo timeZone in timeZones)
                {
                    string timeZoneName = timeZone.DisplayName;
                    ListItem ddlTimeZoneItem2 = new ListItem(timeZoneName, timeZone.Id);
                    this.ddlTimeZone.Items.Add(ddlTimeZoneItem2);
                }
            }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStateProvinceDropDowns();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.FillAffiliatDropDowns();
                this.FillCountryDropDowns();
                this.FillStateProvinceDropDowns();
                this.FillTimeZones();
                this.BindData();
            }
        }

        public Customer SaveInfo()
        {
            Customer customer = IoC.Resolve<ICustomerService>().GetCustomerById(this.CustomerId);

            string username = txtUsername.Text;
            string email = txtEmail.Text.Trim();
            int affiliateId = int.Parse(this.ddlAffiliate.SelectedItem.Value);
            bool isTaxExempt = cbIsTaxExempt.Checked;
            bool isAdmin = cbIsAdmin.Checked;
            bool isForumModerator = cbIsForumModerator.Checked;
            bool active = cbActive.Checked;
            string adminComment = txtAdminComment.Text.Trim();

            if (customer != null)
            {
                //username 
                if (IoC.Resolve<ICustomerService>().UsernamesEnabled &&
                    IoC.Resolve<ICustomerService>().AllowCustomersToChangeUsernames)
                {
                    if (customer.Username.ToLowerInvariant() != username.ToLowerInvariant().Trim())
                    {
                        if (!customer.IsGuest)
                        {
                            customer = IoC.Resolve<ICustomerService>().ChangeCustomerUsername(customer.CustomerId, username);
                        }
                    }
                }

                //email
                if (customer.Email.ToLowerInvariant() != email.ToLowerInvariant().Trim())
                {
                    if (!customer.IsGuest)
                    {
                        customer = IoC.Resolve<ICustomerService>().SetEmail(customer.CustomerId, email);
                    }
                }

                customer.AffiliateId = affiliateId;
                customer.IsTaxExempt = isTaxExempt;
                customer.IsAdmin = isAdmin;
                customer.IsForumModerator = isForumModerator;
                customer.AdminComment = adminComment;
                customer.Active = active;
                IoC.Resolve<ICustomerService>().UpdateCustomer(customer);
            }
            else
            {
                string password = txtPassword.Text;
                if (String.IsNullOrEmpty(password))
                    throw new NopException(GetLocaleResourceString("Customer.PasswordIsRequired"));
                MembershipCreateStatus createStatus = MembershipCreateStatus.Success;

                customer = IoC.Resolve<ICustomerService>().AddCustomer(Guid.NewGuid(), email, username,
                    password, affiliateId,
                    0, 0, 0, string.Empty, string.Empty, string.Empty,
                    NopContext.Current.WorkingLanguage.LanguageId,
                    NopContext.Current.WorkingCurrency.CurrencyId,
                    NopContext.Current.TaxDisplayType,
                    isTaxExempt, isAdmin,
                    false, isForumModerator,
                    0, string.Empty, adminComment, active,
                    false, DateTime.UtcNow, string.Empty,
                    0, null, out createStatus);

                if (createStatus != MembershipCreateStatus.Success)
                {
                    throw new NopException(string.Format("Could not create new customer: {0}", createStatus.ToString()));
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
                customer.DateOfBirth = ctrlDateOfBirthDatePicker.SelectedDate;
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
                    customer.VatNumberStatus = IoC.Resolve<ITaxService>().GetVatNumberStatus(IoC.Resolve<ICountryService>().GetCountryById(customer.CountryId), customer.VatNumber);
                }
            }

            if (DateTimeHelper.AllowCustomersToSetTimeZone)
            {
                if (ddlTimeZone.SelectedItem != null && !String.IsNullOrEmpty(ddlTimeZone.SelectedItem.Value))
                {
                    string timeZoneId = ddlTimeZone.SelectedItem.Value;
                    TimeZoneInfo timeZone = DateTimeHelper.FindTimeZoneById(timeZoneId);
                    if (timeZone != null)
                    {
                        customer.TimeZoneId = timeZone.Id;
                        IoC.Resolve<ICustomerService>().UpdateCustomer(customer);
                    }
                }
            }

            return customer;
        }

        protected void BtnChangePassword_OnClick(object sender, EventArgs e)
        {
            try
            {
                IoC.Resolve<ICustomerService>().ModifyPassword(CustomerId, txtPassword.Text);
                txtPassword.Text = String.Empty;
                ShowMessage(GetLocaleResourceString("Admin.CustomerInfo.PasswordChanged"));
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }

        protected void BtnMarkVatNumberAsValid_OnClick(object sender, EventArgs e)
        {
            try
            {
                Customer customer = IoC.Resolve<ICustomerService>().GetCustomerById(this.CustomerId);
                customer.VatNumberStatus = VatNumberStatusEnum.Valid;
                BindData();
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }

        protected void BtnMarkVatNumberAsInvalid_OnClick(object sender, EventArgs e)
        {
            try
            {
                Customer customer = IoC.Resolve<ICustomerService>().GetCustomerById(this.CustomerId);
                customer.VatNumberStatus = VatNumberStatusEnum.Invalid;
                BindData();
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }

        public int CustomerId
        {
            get
            {
                return CommonHelper.QueryStringInt("CustomerId");
            }
        }
    }
}