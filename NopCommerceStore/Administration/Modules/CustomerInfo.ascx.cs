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

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CustomerInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            pnlTimeZone.Visible = DateTimeHelper.AllowCustomersToSetTimeZone;
            pnlUsername.Visible = CustomerManager.UsernamesEnabled;

            Customer customer = CustomerManager.GetCustomerById(this.CustomerId);
            if (customer != null)
            {
                this.txtEmail.Text = customer.Email;

                if (CustomerManager.UsernamesEnabled)
                {
                    txtUsername.Visible = false;
                    lblUsername.Visible = true;
                    txtUsername.Text = customer.Username;
                    lblUsername.Text = customer.Username;
                }

                if (CustomerManager.FormFieldGenderEnabled)
                {
                    if (String.IsNullOrEmpty(customer.Gender) ||
                        customer.Gender.ToLower() == "m")
                        rbGenderM.Checked = true;
                    else
                        rbGenderF.Checked = true;
                }

                txtFirstName.Text = customer.FirstName;
                txtLastName.Text = customer.LastName;
                if (CustomerManager.FormFieldDateOfBirthEnabled)
                {
                    ctrlDateOfBirthDatePicker.SelectedDate = customer.DateOfBirth;
                }
                if (CustomerManager.FormFieldCompanyEnabled)
                {
                    txtCompany.Text = customer.Company;
                }
                if (TaxManager.EUVatEnabled)
                {
                    txtVatNumber.Text = customer.VatNumber;
                    lblVatNumberStatus.Text = string.Format(GetLocaleResourceString("Admin.CustomerInfo.VATNumberStatus"), TaxManager.GetVatNumberStatusName(customer.VatNumberStatus));
                }

                if (CustomerManager.FormFieldStreetAddressEnabled)
                {
                    txtStreetAddress.Text = customer.StreetAddress;
                }
                if (CustomerManager.FormFieldStreetAddress2Enabled)
                {
                    txtStreetAddress2.Text = customer.StreetAddress2;
                }
                if (CustomerManager.FormFieldPostCodeEnabled)
                {
                    txtZipPostalCode.Text = customer.ZipPostalCode;
                }
                if (CustomerManager.FormFieldCityEnabled)
                {
                    txtCity.Text = customer.City;
                }
                if (CustomerManager.FormFieldCountryEnabled)
                {
                    CommonHelper.SelectListItem(this.ddlCountry, customer.CountryId.ToString());
                }
                if (CustomerManager.FormFieldCountryEnabled &&
                    CustomerManager.FormFieldStateEnabled)
                {
                    FillStateProvinceDropDowns();
                    CommonHelper.SelectListItem(this.ddlStateProvince, customer.StateProvinceId.ToString());
                }
                if (CustomerManager.FormFieldPhoneEnabled)
                {
                    txtPhoneNumber.Text = customer.PhoneNumber;
                }
                if (CustomerManager.FormFieldFaxEnabled)
                {
                    txtFaxNumber.Text = customer.FaxNumber;
                }
                if (CustomerManager.FormFieldNewsletterEnabled)
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
                if (CustomerManager.UsernamesEnabled)
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
            var affiliateCollection = AffiliateManager.GetAllAffiliates();
            foreach (var affiliate in affiliateCollection)
            {
                ListItem ddlAffiliateItem2 = new ListItem(affiliate.LastName + " (ID=" + affiliate.AffiliateId.ToString() + ")", affiliate.AffiliateId.ToString());
                this.ddlAffiliate.Items.Add(ddlAffiliateItem2);
            }
        }

        private void FillCountryDropDowns()
        {
            ddlCountry.Items.Clear();
            var countryCollection = CountryManager.GetAllCountriesForRegistration();
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

            var stateProvinceCollection = StateProvinceManager.GetStateProvincesByCountryId(countryId);
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
            Customer customer = CustomerManager.GetCustomerById(this.CustomerId);

            string email = txtEmail.Text.Trim();
            int affiliateId = int.Parse(this.ddlAffiliate.SelectedItem.Value);
            bool isTaxExempt=cbIsTaxExempt.Checked;
            bool isAdmin=cbIsAdmin.Checked;
            bool isForumModerator=cbIsForumModerator.Checked;
            bool active= cbActive.Checked;
            string adminComment = txtAdminComment.Text.Trim();

            if (customer != null)
            {
                customer = CustomerManager.SetEmail(customer.CustomerId, email);

                customer = CustomerManager.UpdateCustomer(customer.CustomerId, customer.CustomerGuid,
                    customer.Email, customer.Username, customer.PasswordHash,
                    customer.SaltKey, affiliateId,
                    customer.BillingAddressId, customer.ShippingAddressId,
                    customer.LastPaymentMethodId, customer.LastAppliedCouponCode,
                    customer.GiftCardCouponCodes, customer.CheckoutAttributes,
                    customer.LanguageId, customer.CurrencyId, customer.TaxDisplayType,
                    isTaxExempt, isAdmin, customer.IsGuest, isForumModerator,
                    customer.TotalForumPosts, customer.Signature, adminComment, active,
                    customer.Deleted, customer.RegistrationDate,
                    customer.TimeZoneId, customer.AvatarId, customer.DateOfBirth);
            }
            else
            {
                string username = txtUsername.Text;
                string password = txtPassword.Text;
                if (String.IsNullOrEmpty(password))
                    throw new NopException(GetLocaleResourceString("Customer.PasswordIsRequired"));
                MembershipCreateStatus createStatus = MembershipCreateStatus.Success;

                customer = CustomerManager.AddCustomer(Guid.NewGuid(), email, username,
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
                customer = CustomerManager.SetCustomerDateOfBirth(customer.CustomerId, ctrlDateOfBirthDatePicker.SelectedDate);
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
                }
            }

            if (DateTimeHelper.AllowCustomersToSetTimeZone)
            {
                if (ddlTimeZone.SelectedItem != null && !String.IsNullOrEmpty(ddlTimeZone.SelectedItem.Value))
                {
                    string timeZoneId = ddlTimeZone.SelectedItem.Value;
                    TimeZoneInfo timeZone = DateTimeHelper.FindTimeZoneById(timeZoneId);
                    if (timeZone != null)
                        customer = CustomerManager.SetTimeZoneId(customer.CustomerId, timeZone.Id);
                }
            }

            return customer;
        }

        protected void BtnChangePassword_OnClick(object sender, EventArgs e)
        {
            try
            {
                CustomerManager.ModifyPassword(CustomerId, txtPassword.Text);
                txtPassword.Text = String.Empty;
            }
            catch(Exception ex)
            {
                ProcessException(ex);
            }
        }

        protected void BtnMarkVatNumberAsValid_OnClick(object sender, EventArgs e)
        {
            try
            {
                Customer customer = CustomerManager.GetCustomerById(this.CustomerId);
                customer.VatNumberStatus = VatNumberStatusEnum.Valid;
                BindData();
            }
            catch(Exception ex)
            {
                ProcessException(ex);
            }
        }

        protected void BtnMarkVatNumberAsInvalid_OnClick(object sender, EventArgs e)
        {
            try
            {
                Customer customer = CustomerManager.GetCustomerById(this.CustomerId);
                customer.VatNumberStatus = VatNumberStatusEnum.Invalid;
                BindData();
            }
            catch(Exception ex)
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