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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Xml;
using NopSolutions.NopCommerce.Controls;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CustomerRegisterControl: BaseNopFrontendUserControl
    {
        private void ApplyLocalization()
        {
            var EmailRequired = CreateUserWizardStep1.ContentTemplateContainer.FindControl("EmailRequired") as RequiredFieldValidator;
            if (EmailRequired != null)
            {
                if (this.CustomerService.UsernamesEnabled)
                {
                    EmailRequired.ErrorMessage = GetLocaleResourceString("Account.E-MailRequired");
                    EmailRequired.ToolTip = GetLocaleResourceString("Account.E-MailRequired");
                }
                else
                {
                    //EmailRequired is not enabled
                }
            }

            var revEmail = CreateUserWizardStep1.ContentTemplateContainer.FindControl("revEmail") as RegularExpressionValidator;
            if (revEmail != null)
            {
                if (this.CustomerService.UsernamesEnabled)
                {
                    revEmail.ErrorMessage = GetLocaleResourceString("Account.InvalidEmail");
                    revEmail.ToolTip = GetLocaleResourceString("Account.InvalidEmail");
                }
                else
                {
                    //revEmail is not enabled
                }
            }


            var lUsernameOrEmail = CreateUserWizardStep1.ContentTemplateContainer.FindControl("lUsernameOrEmail") as Literal;
            if (lUsernameOrEmail != null)
            {
                if (this.CustomerService.UsernamesEnabled)
                {
                    lUsernameOrEmail.Text = GetLocaleResourceString("Account.Username");
                }
                else
                {
                    lUsernameOrEmail.Text = GetLocaleResourceString("Account.E-Mail");
                }
            }

            var UserNameOrEmailRequired = CreateUserWizardStep1.ContentTemplateContainer.FindControl("UserNameOrEmailRequired") as RequiredFieldValidator;
            if (UserNameOrEmailRequired != null)
            {
                if (this.CustomerService.UsernamesEnabled)
                {
                    UserNameOrEmailRequired.ErrorMessage = GetLocaleResourceString("Account.UserNameRequired");
                    UserNameOrEmailRequired.ToolTip = GetLocaleResourceString("Account.UserNameRequired");
                }
                else
                {
                    UserNameOrEmailRequired.ErrorMessage = GetLocaleResourceString("Account.E-MailRequired");
                    UserNameOrEmailRequired.ToolTip = GetLocaleResourceString("Account.E-MailRequired");
                }
            }

            var refUserNameOrEmail = CreateUserWizardStep1.ContentTemplateContainer.FindControl("refUserNameOrEmail") as RegularExpressionValidator;
            if (refUserNameOrEmail != null)
            {
                if (this.CustomerService.UsernamesEnabled)
                {
                    //refUserNameOrEmail is not enabled
                }
                else
                {
                    refUserNameOrEmail.ErrorMessage = GetLocaleResourceString("Account.InvalidEmail");
                    refUserNameOrEmail.ToolTip = GetLocaleResourceString("Account.InvalidEmail");
                }
            }

            var lblCompleteStep = CompleteWizardStep1.ContentTemplateContainer.FindControl("lblCompleteStep") as Label;
            if (lblCompleteStep != null)
            {
                switch (this.CustomerService.CustomerRegistrationType)
                {
                    case CustomerRegistrationTypeEnum.Standard:
                        {
                            lblCompleteStep.Text = GetLocaleResourceString("Account.RegistrationCompleted");
                        }
                        break;
                    case CustomerRegistrationTypeEnum.EmailValidation:
                        {
                            lblCompleteStep.Text = GetLocaleResourceString("Account.ActivationEmailHasBeenSent");
                        }
                        break;
                    case CustomerRegistrationTypeEnum.AdminApproval:
                        {
                            lblCompleteStep.Text = GetLocaleResourceString("Account.AdminApprovalRequired");
                        }
                        break;
                    case CustomerRegistrationTypeEnum.Disabled:
                        {
                            lblCompleteStep.Text = "Registration method error";
                        }
                        break;
                    default:
                        {
                            lblCompleteStep.Text = "Registration method error";
                        }
                        break;
                }
            }

        }

        protected override void OnInit(EventArgs e)
        {
            //form fields
            var phGender = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phGender");
            var phDateOfBirth = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phDateOfBirth");
            var phCompanyDetails = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phCompanyDetails");
            var phCompanyName = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phCompanyName");
            var phVatNumber = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phVatNumber");
            var rfvCompany = (RequiredFieldValidator)CreateUserWizardStep1.ContentTemplateContainer.FindControl("rfvCompany");
            var phStreetAddress = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phStreetAddress");
            var rfvStreetAddress = (RequiredFieldValidator)CreateUserWizardStep1.ContentTemplateContainer.FindControl("rfvStreetAddress");
            var phStreetAddress2 = CreateUserWizardStep1.ContentTemplateContainer.FindControl("phStreetAddress2");
            var rfvStreetAddress2 = (RequiredFieldValidator)CreateUserWizardStep1.ContentTemplateContainer.FindControl("rfvStreetAddress2");
            var phPostCode = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phPostCode");
            var rfvZipPostalCode = (RequiredFieldValidator)CreateUserWizardStep1.ContentTemplateContainer.FindControl("rfvZipPostalCode");
            var phCity = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phCity");
            var rfvCity = (RequiredFieldValidator)CreateUserWizardStep1.ContentTemplateContainer.FindControl("rfvCity");
            var phCountry = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phCountry");
            var phStateProvince = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phStateProvince");
            var phYourAddress = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phYourAddress");
            var phTelephoneNumber = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phTelephoneNumber");
            var rfvPhoneNumber = (RequiredFieldValidator)CreateUserWizardStep1.ContentTemplateContainer.FindControl("rfvPhoneNumber");
            var phFaxNumber = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phFaxNumber");
            var rfvFaxNumber = (RequiredFieldValidator)CreateUserWizardStep1.ContentTemplateContainer.FindControl("rfvFaxNumber");
            var phYourContactInformation = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phYourContactInformation");
            var phNewsletter = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phNewsletter");
            var phPreferences = (PlaceHolder)CreateUserWizardStep1.ContentTemplateContainer.FindControl("phPreferences");
            var trTimeZone = (HtmlTableRow)CreateUserWizardStep1.ContentTemplateContainer.FindControl("trTimeZone");
            
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

            trTimeZone.Visible = DateTimeHelper.AllowCustomersToSetTimeZone && this.CustomerService.FormFieldTimeZoneEnabled;
            phPreferences.Visible = trTimeZone.Visible;
            
            base.OnInit(e);
        }

        public void CreatedUser(object sender, EventArgs e)
        {
            var rbGenderM = (RadioButton)CreateUserWizardStep1.ContentTemplateContainer.FindControl("rbGenderM");
            var rbGenderF = (RadioButton)CreateUserWizardStep1.ContentTemplateContainer.FindControl("rbGenderF");
            var txtFirstName = (TextBox)CreateUserWizardStep1.ContentTemplateContainer.FindControl("txtFirstName");
            var txtLastName = (TextBox)CreateUserWizardStep1.ContentTemplateContainer.FindControl("txtLastName");
            var txtDateOfBirth = (TextBox)CreateUserWizardStep1.ContentTemplateContainer.FindControl("txtDateOfBirth");
            var UserName = (TextBox)CreateUserWizardStep1.ContentTemplateContainer.FindControl("UserName");
            var txtCompany = (TextBox)CreateUserWizardStep1.ContentTemplateContainer.FindControl("txtCompany");
            var txtVatNumber = (TextBox)CreateUserWizardStep1.ContentTemplateContainer.FindControl("txtVatNumber");
            var txtStreetAddress = (TextBox)CreateUserWizardStep1.ContentTemplateContainer.FindControl("txtStreetAddress");
            var txtStreetAddress2 = (TextBox)CreateUserWizardStep1.ContentTemplateContainer.FindControl("txtStreetAddress2");
            var txtZipPostalCode = (TextBox)CreateUserWizardStep1.ContentTemplateContainer.FindControl("txtZipPostalCode");
            var txtCity = (TextBox)CreateUserWizardStep1.ContentTemplateContainer.FindControl("txtCity");
            var txtPhoneNumber = (TextBox)CreateUserWizardStep1.ContentTemplateContainer.FindControl("txtPhoneNumber");
            var txtFaxNumber = (TextBox)CreateUserWizardStep1.ContentTemplateContainer.FindControl("txtFaxNumber");
            var ddlCountry = (DropDownList)CreateUserWizardStep1.ContentTemplateContainer.FindControl("ddlCountry");
            var ddlStateProvince = (DropDownList)CreateUserWizardStep1.ContentTemplateContainer.FindControl("ddlStateProvince");
            var cbNewsletter = (CheckBox)CreateUserWizardStep1.ContentTemplateContainer.FindControl("cbNewsletter");
            var dtDateOfBirth = (NopDatePicker)CreateUserWizardStep1.ContentTemplateContainer.FindControl("dtDateOfBirth");
            var ddlTimeZone = (DropDownList)CreateUserWizardStep1.ContentTemplateContainer.FindControl("ddlTimeZone");
            
            Customer customer = null;
            if (this.CustomerService.UsernamesEnabled)
            {
                customer = this.CustomerService.GetCustomerByUsername(UserName.Text.Trim());
            }
            else
            {
                customer = this.CustomerService.GetCustomerByEmail(UserName.Text.Trim());
            }
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
            if (DateTimeHelper.AllowCustomersToSetTimeZone && this.CustomerService.FormFieldTimeZoneEnabled)
            {
                if (ddlTimeZone.SelectedItem != null && !String.IsNullOrEmpty(ddlTimeZone.SelectedItem.Value))
                {
                    string timeZoneId = ddlTimeZone.SelectedItem.Value;
                    customer.TimeZoneId = DateTimeHelper.FindTimeZoneById(timeZoneId).Id;
                    this.CustomerService.UpdateCustomer(customer);
                }
            }

            //set VAT number after country is saved
            if (this.TaxService.EUVatEnabled)
            {
                string vatName = string.Empty;
                string vatAddress = string.Empty;

                customer.VatNumber = txtVatNumber.Text;
                customer.VatNumberStatus = this.TaxService.GetVatNumberStatus(this.CountryService.GetCountryById(customer.CountryId),
                    customer.VatNumber, out vatName, out vatAddress);
                //admin notification
                if (!String.IsNullOrEmpty(customer.VatNumber) && this.TaxService.EUVatEmailAdminWhenNewVATSubmitted)
                {
                    this.MessageService.SendNewVATSubmittedStoreOwnerNotification(customer,
                        vatName, vatAddress, this.LocalizationManager.DefaultAdminLanguage.LanguageId);
                }
            }

            //billing address
            var billingAddress = new Address()
            {
                CustomerId = customer.CustomerId,
                IsBillingAddress = true,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                FaxNumber = customer.FaxNumber,
                Company = customer.Company,
                Address1 = customer.StreetAddress,
                Address2 = customer.StreetAddress2,
                City = customer.City,
                StateProvinceId = customer.StateProvinceId,
                ZipPostalCode = customer.ZipPostalCode,
                CountryId = customer.CountryId,
                CreatedOn = customer.RegistrationDate,
                UpdatedOn = customer.RegistrationDate
            };
            if (this.CustomerService.CanUseAddressAsBillingAddress(billingAddress))
            {
                this.CustomerService.InsertAddress(billingAddress);

                //set default billing address
                customer.BillingAddressId = billingAddress.AddressId;
                this.CustomerService.UpdateCustomer(customer);
            }

            //shipping address
            var shippingAddress = new Address()
            {
                CustomerId = customer.CustomerId,
                IsBillingAddress = false,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                FaxNumber = customer.FaxNumber,
                Company = customer.Company,
                Address1 = customer.StreetAddress,
                Address2 = customer.StreetAddress2,
                City = customer.City,
                StateProvinceId = customer.StateProvinceId,
                ZipPostalCode = customer.ZipPostalCode,
                CountryId = customer.CountryId,
                CreatedOn = customer.RegistrationDate,
                UpdatedOn = customer.RegistrationDate
            };
            if (this.CustomerService.CanUseAddressAsShippingAddress(shippingAddress))
            {
                this.CustomerService.InsertAddress(shippingAddress);

                //set default shipping address
                customer.ShippingAddressId = shippingAddress.AddressId;
                this.CustomerService.UpdateCustomer(customer);
            }

            //notification
            if (this.CustomerService.NotifyNewCustomerRegistration)
            {
                this.MessageService.SendNewCustomerNotificationMessage(customer, this.LocalizationManager.DefaultAdminLanguage.LanguageId);
            }
        }

        public void CreatingUser(object sender, LoginCancelEventArgs e)
        {
            if (this.SettingManager.GetSettingValueBoolean("Common.RegisterCaptchaImageEnabled"))
            {
                var CaptchaCtrl = CreateUserWizardStep1.ContentTemplateContainer.FindControl("CaptchaCtrl") as CaptchaControl;
                if (CaptchaCtrl != null)
                {
                    if (!CaptchaCtrl.ValidateCaptcha())
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        protected void CreateUserError(object sender, EventArgs e)
        {
            if (this.SettingManager.GetSettingValueBoolean("Common.RegisterCaptchaImageEnabled"))
            {
                var CaptchaCtrl = CreateUserWizardStep1.ContentTemplateContainer.FindControl("CaptchaCtrl") as CaptchaControl;
                if (CaptchaCtrl != null)
                {
                    CaptchaCtrl.RegenerateCode();
                }
            }
        }

        private void FillCountryDropDowns()
        {
            var ddlCountry = (DropDownList)CreateUserWizardStep1.ContentTemplateContainer.FindControl("ddlCountry");
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
            var ddlCountry = (DropDownList)CreateUserWizardStep1.ContentTemplateContainer.FindControl("ddlCountry");
            var ddlStateProvince = (DropDownList)CreateUserWizardStep1.ContentTemplateContainer.FindControl("ddlStateProvince");
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
            if (DateTimeHelper.AllowCustomersToSetTimeZone && this.CustomerService.FormFieldTimeZoneEnabled)
            {
                var ddlTimeZone = (DropDownList)CreateUserWizardStep1.ContentTemplateContainer.FindControl("ddlTimeZone");

                ddlTimeZone.Items.Clear();
                var timeZones = DateTimeHelper.GetSystemTimeZones();
                foreach (var timeZone in timeZones)
                {
                    string timeZoneName = timeZone.DisplayName;
                    var ddlTimeZoneItem2 = new ListItem(timeZoneName, timeZone.Id);
                    ddlTimeZone.Items.Add(ddlTimeZoneItem2);
                }
                CommonHelper.SelectListItem(ddlTimeZone, DateTimeHelper.CurrentTimeZone.Id);
            }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStateProvinceDropDowns();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();

            if (this.CustomerService.CustomerRegistrationType == CustomerRegistrationTypeEnum.Disabled)
            {
                CreateUserForm.Visible = false;
                topicRegistrationNotAllowed.Visible = true;
            }
            else
            {
                CreateUserForm.Visible = true;
                topicRegistrationNotAllowed.Visible = false;
            }

            if (!Page.IsPostBack)
            {
                if (NopContext.Current.User != null && !NopContext.Current.User.IsGuest)
                {
                    this.CustomerService.Logout();
                    Response.Redirect("~/register.aspx");
                }

                #region Username/emails hack
                var pnlEmail = CreateUserWizardStep1.ContentTemplateContainer.FindControl("pnlEmail") as HtmlTableRow;
                if (pnlEmail != null)
                {
                    pnlEmail.Visible = this.CustomerService.UsernamesEnabled;
                }
                var refUserNameOrEmail = CreateUserWizardStep1.ContentTemplateContainer.FindControl("refUserNameOrEmail") as RegularExpressionValidator;
                if (refUserNameOrEmail != null)
                {
                    refUserNameOrEmail.Enabled = !this.CustomerService.UsernamesEnabled;
                }
                #endregion

                this.FillCountryDropDowns();
                this.FillStateProvinceDropDowns();
                this.FillTimeZones();
                this.DataBind();
            }

            var CaptchaCtrl = CreateUserWizardStep1.ContentTemplateContainer.FindControl("CaptchaCtrl") as CaptchaControl;
            if (CaptchaCtrl != null)
            {
                CaptchaCtrl.Visible = this.SettingManager.GetSettingValueBoolean("Common.RegisterCaptchaImageEnabled");
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            string returnUrl = CommonHelper.QueryString("ReturnUrl");
            if (!String.IsNullOrEmpty(returnUrl))
            {
                this.CreateUserForm.ContinueDestinationPageUrl = returnUrl;
            }

            base.OnPreRender(e);
        }
    }
}