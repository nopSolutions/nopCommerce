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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Xml;
using NopSolutions.NopCommerce.Controls;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CustomerRegisterControl : BaseNopUserControl
    {
        private void ApplyLocalization()
        {
            var EmailRequired = CreateUserWizardStep1.ContentTemplateContainer.FindControl("EmailRequired") as RequiredFieldValidator;
            if (EmailRequired != null)
            {
                if (CustomerManager.UsernamesEnabled)
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
                if (CustomerManager.UsernamesEnabled)
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
                if (CustomerManager.UsernamesEnabled)
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
                if (CustomerManager.UsernamesEnabled)
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
                if (CustomerManager.UsernamesEnabled)
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
                switch (CustomerManager.CustomerRegistrationType)
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
            
            Customer customer = null;
            if (CustomerManager.UsernamesEnabled)
            {
                customer = CustomerManager.GetCustomerByUsername(UserName.Text.Trim());
            }
            else
            {
                customer = CustomerManager.GetCustomerByEmail(UserName.Text.Trim());
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
                customer.VatNumber = txtVatNumber.Text;
                customer.VatNumberStatus = TaxManager.GetVatNumberStatus(CountryManager.GetCountryById(customer.CountryId), customer.VatNumber);
                //admin notification
                if (!String.IsNullOrEmpty(customer.VatNumber) && TaxManager.EUVatEmailAdminWhenNewVATSubmitted)
                {
                    MessageManager.SendNewVATSubmittedStoreOwnerNotification(customer, LocalizationManager.DefaultAdminLanguage.LanguageId);
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
                CreatedOn = customer.RegistrationDate
            };
            if (CustomerManager.CanUseAddressAsBillingAddress(billingAddress))
            {
                billingAddress = CustomerManager.InsertAddress(billingAddress.CustomerId, billingAddress.IsBillingAddress,
                    billingAddress.FirstName, billingAddress.LastName, billingAddress.PhoneNumber,
                    billingAddress.Email, billingAddress.FaxNumber, billingAddress.Company,
                    billingAddress.Address1, billingAddress.Address2,
                    billingAddress.City, billingAddress.StateProvinceId,
                    billingAddress.ZipPostalCode, billingAddress.CountryId, DateTime.UtcNow, DateTime.UtcNow);
                customer = CustomerManager.SetDefaultBillingAddress(customer.CustomerId, billingAddress.AddressId);
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
                CreatedOn = customer.RegistrationDate
            };
            if (CustomerManager.CanUseAddressAsShippingAddress(shippingAddress))
            {
                shippingAddress = CustomerManager.InsertAddress(shippingAddress.CustomerId, shippingAddress.IsBillingAddress,
                    shippingAddress.FirstName, shippingAddress.LastName, shippingAddress.PhoneNumber,
                    shippingAddress.Email, shippingAddress.FaxNumber, shippingAddress.Company,
                    shippingAddress.Address1, shippingAddress.Address2,
                    shippingAddress.City, shippingAddress.StateProvinceId,
                    shippingAddress.ZipPostalCode, shippingAddress.CountryId, DateTime.UtcNow, DateTime.UtcNow);
                customer = CustomerManager.SetDefaultShippingAddress(customer.CustomerId, shippingAddress.AddressId);
            }
        }

        public void CreatingUser(object sender, LoginCancelEventArgs e)
        {
            if (SettingManager.GetSettingValueBoolean("Common.RegisterCaptchaImageEnabled"))
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
            if (SettingManager.GetSettingValueBoolean("Common.RegisterCaptchaImageEnabled"))
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
            var countryCollection = CountryManager.GetAllCountriesForRegistration();
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

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStateProvinceDropDowns();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();

            if (CustomerManager.CustomerRegistrationType == CustomerRegistrationTypeEnum.Disabled)
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
                    CustomerManager.Logout();
                    Response.Redirect("~/register.aspx");
                }

                #region Username/emails hack
                var pnlEmail = CreateUserWizardStep1.ContentTemplateContainer.FindControl("pnlEmail") as HtmlTableRow;
                if (pnlEmail != null)
                {
                    pnlEmail.Visible = CustomerManager.UsernamesEnabled;
                }
                var refUserNameOrEmail = CreateUserWizardStep1.ContentTemplateContainer.FindControl("refUserNameOrEmail") as RegularExpressionValidator;
                if (refUserNameOrEmail != null)
                {
                    refUserNameOrEmail.Enabled = !CustomerManager.UsernamesEnabled;
                }
                #endregion

                this.FillCountryDropDowns();
                this.FillStateProvinceDropDowns();
                this.DataBind();
            }

            var CaptchaCtrl = CreateUserWizardStep1.ContentTemplateContainer.FindControl("CaptchaCtrl") as CaptchaControl;
            if (CaptchaCtrl != null)
            {
                CaptchaCtrl.Visible = SettingManager.GetSettingValueBoolean("Common.RegisterCaptchaImageEnabled");
            }
        }
    }
}