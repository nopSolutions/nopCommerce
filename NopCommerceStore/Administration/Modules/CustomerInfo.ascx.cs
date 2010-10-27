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
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CustomerInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            pnlTimeZone.Visible = DateTimeHelper.AllowCustomersToSetTimeZone;
            pnlUsername.Visible = IoCFactory.Resolve<ICustomerManager>().UsernamesEnabled;

            Customer customer = IoCFactory.Resolve<ICustomerManager>().GetCustomerById(this.CustomerId);
            if (customer != null)
            {
                this.txtEmail.Text = customer.Email;

                this.txtUsername.Text = customer.Username;
                this.lblUsername.Text = customer.Username;
                if (IoCFactory.Resolve<ICustomerManager>().UsernamesEnabled)
                {
                    if (IoCFactory.Resolve<ICustomerManager>().AllowCustomersToChangeUsernames)
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

                if (IoCFactory.Resolve<ICustomerManager>().FormFieldGenderEnabled)
                {
                    if (String.IsNullOrEmpty(customer.Gender) ||
                        customer.Gender.ToLower() == "m")
                        this.rbGenderM.Checked = true;
                    else
                        this.rbGenderF.Checked = true;
                }

                txtFirstName.Text = customer.FirstName;
                txtLastName.Text = customer.LastName;
                if (IoCFactory.Resolve<ICustomerManager>().FormFieldDateOfBirthEnabled)
                {
                    ctrlDateOfBirthDatePicker.SelectedDate = customer.DateOfBirth;
                }
                if (IoCFactory.Resolve<ICustomerManager>().FormFieldCompanyEnabled)
                {
                    txtCompany.Text = customer.Company;
                }
                if (IoCFactory.Resolve<ITaxManager>().EUVatEnabled)
                {
                    txtVatNumber.Text = customer.VatNumber;
                    lblVatNumberStatus.Text = string.Format(GetLocaleResourceString("Admin.CustomerInfo.VATNumberStatus"), IoCFactory.Resolve<ITaxManager>().GetVatNumberStatusName(customer.VatNumberStatus));
                }

                if (IoCFactory.Resolve<ICustomerManager>().FormFieldStreetAddressEnabled)
                {
                    txtStreetAddress.Text = customer.StreetAddress;
                }
                if (IoCFactory.Resolve<ICustomerManager>().FormFieldStreetAddress2Enabled)
                {
                    txtStreetAddress2.Text = customer.StreetAddress2;
                }
                if (IoCFactory.Resolve<ICustomerManager>().FormFieldPostCodeEnabled)
                {
                    txtZipPostalCode.Text = customer.ZipPostalCode;
                }
                if (IoCFactory.Resolve<ICustomerManager>().FormFieldCityEnabled)
                {
                    txtCity.Text = customer.City;
                }
                if (IoCFactory.Resolve<ICustomerManager>().FormFieldCountryEnabled)
                {
                    CommonHelper.SelectListItem(this.ddlCountry, customer.CountryId.ToString());
                }
                if (IoCFactory.Resolve<ICustomerManager>().FormFieldCountryEnabled &&
                    IoCFactory.Resolve<ICustomerManager>().FormFieldStateEnabled)
                {
                    FillStateProvinceDropDowns();
                    CommonHelper.SelectListItem(this.ddlStateProvince, customer.StateProvinceId.ToString());
                }
                if (IoCFactory.Resolve<ICustomerManager>().FormFieldPhoneEnabled)
                {
                    txtPhoneNumber.Text = customer.PhoneNumber;
                }
                if (IoCFactory.Resolve<ICustomerManager>().FormFieldFaxEnabled)
                {
                    txtFaxNumber.Text = customer.FaxNumber;
                }
                if (IoCFactory.Resolve<ICustomerManager>().FormFieldNewsletterEnabled)
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
                if (IoCFactory.Resolve<ICustomerManager>().UsernamesEnabled)
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
            var affiliateCollection = IoCFactory.Resolve<IAffiliateManager>().GetAllAffiliates();
            foreach (var affiliate in affiliateCollection)
            {
                ListItem ddlAffiliateItem2 = new ListItem(affiliate.LastName + " (ID=" + affiliate.AffiliateId.ToString() + ")", affiliate.AffiliateId.ToString());
                this.ddlAffiliate.Items.Add(ddlAffiliateItem2);
            }
        }

        private void FillCountryDropDowns()
        {
            ddlCountry.Items.Clear();
            var countryCollection = IoCFactory.Resolve<ICountryManager>().GetAllCountriesForRegistration();
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

            var stateProvinceCollection = IoCFactory.Resolve<IStateProvinceManager>().GetStateProvincesByCountryId(countryId);
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
            Customer customer = IoCFactory.Resolve<ICustomerManager>().GetCustomerById(this.CustomerId);

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
                if (IoCFactory.Resolve<ICustomerManager>().UsernamesEnabled &&
                    IoCFactory.Resolve<ICustomerManager>().AllowCustomersToChangeUsernames)
                {
                    if (customer.Username.ToLowerInvariant() != username.ToLowerInvariant().Trim())
                    {
                        if (!customer.IsGuest)
                        {
                            customer = IoCFactory.Resolve<ICustomerManager>().ChangeCustomerUsername(customer.CustomerId, username);
                        }
                    }
                }

                //email
                if (customer.Email.ToLowerInvariant() != email.ToLowerInvariant().Trim())
                {
                    if (!customer.IsGuest)
                    {
                        customer = IoCFactory.Resolve<ICustomerManager>().SetEmail(customer.CustomerId, email);
                    }
                }

                customer.AffiliateId = affiliateId;
                customer.IsTaxExempt = isTaxExempt;
                customer.IsAdmin = isAdmin;
                customer.IsForumModerator = isForumModerator;
                customer.AdminComment = adminComment;
                customer.Active = active;
                IoCFactory.Resolve<ICustomerManager>().UpdateCustomer(customer);
            }
            else
            {
                string password = txtPassword.Text;
                if (String.IsNullOrEmpty(password))
                    throw new NopException(GetLocaleResourceString("Customer.PasswordIsRequired"));
                MembershipCreateStatus createStatus = MembershipCreateStatus.Success;

                customer = IoCFactory.Resolve<ICustomerManager>().AddCustomer(Guid.NewGuid(), email, username,
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
                customer.DateOfBirth = ctrlDateOfBirthDatePicker.SelectedDate;
                IoCFactory.Resolve<ICustomerManager>().UpdateCustomer(customer);
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
                    customer.VatNumberStatus = IoCFactory.Resolve<ITaxManager>().GetVatNumberStatus(IoCFactory.Resolve<ICountryManager>().GetCountryById(customer.CountryId), customer.VatNumber);
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
                        IoCFactory.Resolve<ICustomerManager>().UpdateCustomer(customer);
                    }
                }
            }

            return customer;
        }

        protected void BtnChangePassword_OnClick(object sender, EventArgs e)
        {
            try
            {
                IoCFactory.Resolve<ICustomerManager>().ModifyPassword(CustomerId, txtPassword.Text);
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
                Customer customer = IoCFactory.Resolve<ICustomerManager>().GetCustomerById(this.CustomerId);
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
                Customer customer = IoCFactory.Resolve<ICustomerManager>().GetCustomerById(this.CustomerId);
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