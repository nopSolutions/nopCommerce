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
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CustomerRegisterControl : BaseNopUserControl
    {
        private void ApplyLocalization()
        {
            var EmailRequired = CreateUserWizardStep1.ContentTemplateContainer.FindControl("EmailRequired") as RequiredFieldValidator;
            if (EmailRequired != null)
            {
                if (IoCFactory.Resolve<ICustomerManager>().UsernamesEnabled)
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
                if (IoCFactory.Resolve<ICustomerManager>().UsernamesEnabled)
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
                if (IoCFactory.Resolve<ICustomerManager>().UsernamesEnabled)
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
                if (IoCFactory.Resolve<ICustomerManager>().UsernamesEnabled)
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
                if (IoCFactory.Resolve<ICustomerManager>().UsernamesEnabled)
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
                switch (IoCFactory.Resolve<ICustomerManager>().CustomerRegistrationType)
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
            if (IoCFactory.Resolve<ICustomerManager>().UsernamesEnabled)
            {
                customer = IoCFactory.Resolve<ICustomerManager>().GetCustomerByUsername(UserName.Text.Trim());
            }
            else
            {
                customer = IoCFactory.Resolve<ICustomerManager>().GetCustomerByEmail(UserName.Text.Trim());
            }
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
                customer.DateOfBirth = dtDateOfBirth.SelectedDate;
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
                string vatName = string.Empty;
                string vatAddress = string.Empty;

                customer.VatNumber = txtVatNumber.Text;
                customer.VatNumberStatus = IoCFactory.Resolve<ITaxManager>().GetVatNumberStatus(IoCFactory.Resolve<ICountryManager>().GetCountryById(customer.CountryId),
                    customer.VatNumber, out vatName, out vatAddress);
                //admin notification
                if (!String.IsNullOrEmpty(customer.VatNumber) && IoCFactory.Resolve<ITaxManager>().EUVatEmailAdminWhenNewVATSubmitted)
                {
                    IoCFactory.Resolve<IMessageManager>().SendNewVATSubmittedStoreOwnerNotification(customer,
                        vatName, vatAddress, LocalizationManager.DefaultAdminLanguage.LanguageId);
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
            if (IoCFactory.Resolve<ICustomerManager>().CanUseAddressAsBillingAddress(billingAddress))
            {
                IoCFactory.Resolve<ICustomerManager>().InsertAddress(billingAddress);

                //set default billing address
                customer.BillingAddressId = billingAddress.AddressId;
                IoCFactory.Resolve<ICustomerManager>().UpdateCustomer(customer);
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
            if (IoCFactory.Resolve<ICustomerManager>().CanUseAddressAsShippingAddress(shippingAddress))
            {
                IoCFactory.Resolve<ICustomerManager>().InsertAddress(shippingAddress);

                //set default shipping address
                customer.ShippingAddressId = shippingAddress.AddressId;
                IoCFactory.Resolve<ICustomerManager>().UpdateCustomer(customer);
            }

            //notification
            if (IoCFactory.Resolve<ICustomerManager>().NotifyNewCustomerRegistration)
            {
                IoCFactory.Resolve<IMessageManager>().SendNewCustomerNotificationMessage(customer, LocalizationManager.DefaultAdminLanguage.LanguageId);
            }
        }

        public void CreatingUser(object sender, LoginCancelEventArgs e)
        {
            if (IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Common.RegisterCaptchaImageEnabled"))
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
            if (IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Common.RegisterCaptchaImageEnabled"))
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
            var countryCollection = IoCFactory.Resolve<ICountryManager>().GetAllCountriesForRegistration();
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

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStateProvinceDropDowns();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();

            if (IoCFactory.Resolve<ICustomerManager>().CustomerRegistrationType == CustomerRegistrationTypeEnum.Disabled)
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
                    IoCFactory.Resolve<ICustomerManager>().Logout();
                    Response.Redirect("~/register.aspx");
                }

                #region Username/emails hack
                var pnlEmail = CreateUserWizardStep1.ContentTemplateContainer.FindControl("pnlEmail") as HtmlTableRow;
                if (pnlEmail != null)
                {
                    pnlEmail.Visible = IoCFactory.Resolve<ICustomerManager>().UsernamesEnabled;
                }
                var refUserNameOrEmail = CreateUserWizardStep1.ContentTemplateContainer.FindControl("refUserNameOrEmail") as RegularExpressionValidator;
                if (refUserNameOrEmail != null)
                {
                    refUserNameOrEmail.Enabled = !IoCFactory.Resolve<ICustomerManager>().UsernamesEnabled;
                }
                #endregion

                this.FillCountryDropDowns();
                this.FillStateProvinceDropDowns();
                this.DataBind();
            }

            var CaptchaCtrl = CreateUserWizardStep1.ContentTemplateContainer.FindControl("CaptchaCtrl") as CaptchaControl;
            if (CaptchaCtrl != null)
            {
                CaptchaCtrl.Visible = IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Common.RegisterCaptchaImageEnabled");
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