using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Tax.Avalara.Data
{
    [NopMigration("2021-09-06 00:00:00", "Tax.Avalara 2.50. Add certificates feature", MigrationProcessType.Update)]
    public class CertificatesMigration : MigrationBase
    {
        #region Fields

        protected readonly AvalaraTaxSettings _avalaraTaxSettings;
        protected readonly ILanguageService _languageService;
        protected readonly ILocalizationService _localizationService;
        protected readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public CertificatesMigration(AvalaraTaxSettings avalaraTaxSettings,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ISettingService settingService)
        {
            _avalaraTaxSettings = avalaraTaxSettings;
            _languageService = languageService;
            _localizationService = localizationService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //locales
            var (languageId, languages) = this.GetLanguageData();

            _localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Tax.Avalara.Configuration.Certificates"] = "Exemption certificates",
                ["Plugins.Tax.Avalara.Configuration.Certificates.InProgress"] = "Exemption certificates",
                ["Plugins.Tax.Avalara.Configuration.Certificates.NotProvisioned"] = "The selected company isn't configured to use exemption certificates, use the button 'Request certificate setup' below to access this feature",
                ["Plugins.Tax.Avalara.Configuration.Certificates.Provisioned"] = "The selected company is configured to use exemption certificates",
                ["Plugins.Tax.Avalara.Configuration.Certificates.Button"] = "Request certificate setup",
                ["Plugins.Tax.Avalara.Configuration.Common"] = "Common settings",
                ["Plugins.Tax.Avalara.Configuration.Credentials.Button"] = "Check connection",
                ["Plugins.Tax.Avalara.Configuration.Credentials.Declined"] = "Credentials declined",
                ["Plugins.Tax.Avalara.Configuration.Credentials.Verified"] = "Credentials verified",
                ["Plugins.Tax.Avalara.Configuration.TaxCalculation"] = "Tax calculation",
                ["Plugins.Tax.Avalara.ExemptionCertificates"] = "Tax exemption certificates",
                ["Plugins.Tax.Avalara.ExemptionCertificates.Add.ExposureZone"] = "State",
                ["Plugins.Tax.Avalara.ExemptionCertificates.Add.Fail"] = "An error occurred while adding a certificate",
                ["Plugins.Tax.Avalara.ExemptionCertificates.Add.Success"] = "Certificate added successfully",
                ["Plugins.Tax.Avalara.ExemptionCertificates.Description"] = @"
                    <h3>Here you can view and manage your certificates.</h3>
                    <p>
                        The certificate document contains information about a customer's eligibility for exemption from sales.<br />
                        When you add a certificate, it will be processed and become available for use in calculating tax exemptions.<br />
                    </p>
                    <p>
                        You can also go to <a href=""{0}"" target=""_blank"">CertExpress website</a> where you can follow a step-by-step guide to enter information about your exemption certificates.
                    </p>
                    <p>
                        The certificates entered will be recorded and automatically linked to your account.
                    </p>
                    <p>If you have any questions, please <a href=""{1}"" target=""_blank"">contact us</a>.</p>",
                ["Plugins.Tax.Avalara.ExemptionCertificates.ExpirationDate"] = "Expiration date",
                ["Plugins.Tax.Avalara.ExemptionCertificates.ExposureZone"] = "State",
                ["Plugins.Tax.Avalara.ExemptionCertificates.None"] = "No downloaded certificates yet",
                ["Plugins.Tax.Avalara.ExemptionCertificates.OrderReview"] = "Tax",
                ["Plugins.Tax.Avalara.ExemptionCertificates.OrderReview.Applied"] = "Exemption certificate applied",
                ["Plugins.Tax.Avalara.ExemptionCertificates.OrderReview.None"] = @"You have no valid certificates in the selected region. You can add them in your account on <a href=""{0}"" target=""_blank"" style=""color: #4ab2f1;"">this page</a>.",
                ["Plugins.Tax.Avalara.ExemptionCertificates.SignedDate"] = "Signed date",
                ["Plugins.Tax.Avalara.ExemptionCertificates.Status"] = "Status",
                ["Plugins.Tax.Avalara.ExemptionCertificates.View"] = "View",
                ["Plugins.Tax.Avalara.Fields.AllowEditCustomer"] = "Allow edit info",
                ["Plugins.Tax.Avalara.Fields.AllowEditCustomer.Hint"] = "Determine whether to allow customers to edit their info (name, phone, address, etc) when managing certificates. If disabled, the info will be auto updated when customers change details in their accounts.",
                ["Plugins.Tax.Avalara.Fields.AutoValidateCertificate"] = "Auto validate certificates",
                ["Plugins.Tax.Avalara.Fields.AutoValidateCertificate.Hint"] = "Determine whether the new certificates are automatically valid, this allows your customers to make exempt purchases right away, otherwise a customer is not treated as exempt until you validate the document.",
                ["Plugins.Tax.Avalara.Fields.CustomerRoles"] = "Limited to customer roles",
                ["Plugins.Tax.Avalara.Fields.CustomerRoles.Hint"] = "Select customer roles for which exemption certificates will be available. Leave empty if you want this feature to be available to all customers.",
                ["Plugins.Tax.Avalara.Fields.DisplayNoValidCertificatesMessage"] = "Display 'No valid certificates' message",
                ["Plugins.Tax.Avalara.Fields.DisplayNoValidCertificatesMessage.Hint"] = "Determine whether to display a message that there are no valid certificates for the customer on the order confirmation page.",
                ["Plugins.Tax.Avalara.Fields.EnableCertificates"] = "Enable exemption certificates",
                ["Plugins.Tax.Avalara.Fields.EnableCertificates.Hint"] = "Determine whether to enable this feature. In this case, a new page will be added in the account section, so customers can manage their exemption certificates before making a purchase.",
                ["Plugins.Tax.Avalara.Fields.EnableCertificates.Warning"] = "To use this feature, you need the following information from customers: name, country, state, city, address, postal code. Ensure that the appropriate Customer form fields are enabled under <a href=\"{0}\" target=\"_blank\">Customer settings</a>",
                ["Plugins.Tax.Avalara.TestTax.Button"] = "Submit",
            }, languageId);

            _localizationService.DeleteLocaleResources(new List<string>
            {
                "Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Create",
                "Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.CreateResponse",
                "Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Error",
                "Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Refund",
                "Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.RefundResponse",
                "Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Void",
                "Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.VoidResponse",
                "Plugins.Tax.Avalara.VerifyCredentials",
                "Plugins.Tax.Avalara.VerifyCredentials.Declined",
                "Plugins.Tax.Avalara.VerifyCredentials.Verified",
            });

            //settings
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.CompanyId))
                _avalaraTaxSettings.CompanyId = null;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.EnableCertificates))
                _avalaraTaxSettings.EnableCertificates = false;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.AutoValidateCertificate))
                _avalaraTaxSettings.AutoValidateCertificate = true;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.AllowEditCustomer))
                _avalaraTaxSettings.AllowEditCustomer = true;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.DisplayNoValidCertificatesMessage))
                _avalaraTaxSettings.DisplayNoValidCertificatesMessage = true;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.CustomerRoleIds))
                _avalaraTaxSettings.CustomerRoleIds = null;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.PreviewCertificate))
                _avalaraTaxSettings.PreviewCertificate = false;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.UploadOnly))
                _avalaraTaxSettings.UploadOnly = false;
            if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.FillOnly))
                _avalaraTaxSettings.FillOnly = false;
            _settingService.SaveSetting(_avalaraTaxSettings);
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            //nothing
        }

        #endregion
    }
}