using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Tax.Avalara.Components;
using Nop.Plugin.Tax.Avalara.Domain;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Tax;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Tax.Avalara
{
    /// <summary>
    /// Represents Avalara tax provider
    /// </summary>
    public class AvalaraTaxProvider : BasePlugin, ITaxProvider, IWidgetPlugin
    {
        #region Fields

        protected readonly AvalaraTaxManager _avalaraTaxManager;
        protected readonly IActionContextAccessor _actionContextAccessor;
        protected readonly ILocalizationService _localizationService;
        protected readonly IScheduleTaskService _scheduleTaskService;
        protected readonly ISettingService _settingService;
        protected readonly ITaxPluginManager _taxPluginManager;
        protected readonly IUrlHelperFactory _urlHelperFactory;
        protected readonly TaxSettings _taxSettings;
        protected readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public AvalaraTaxProvider(AvalaraTaxManager avalaraTaxManager,
            IActionContextAccessor actionContextAccessor,
            ILocalizationService localizationService,
            IScheduleTaskService scheduleTaskService,
            ISettingService settingService,
            ITaxPluginManager taxPluginManager,
            IUrlHelperFactory urlHelperFactory,
            TaxSettings taxSettings,
            WidgetSettings widgetSettings)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _actionContextAccessor = actionContextAccessor;
            _localizationService = localizationService;
            _scheduleTaskService = scheduleTaskService;
            _settingService = settingService;
            _taxPluginManager = taxPluginManager;
            _urlHelperFactory = urlHelperFactory;
            _taxSettings = taxSettings;
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="taxRateRequest">Tax rate request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax
        /// </returns>
        public async Task<TaxRateResult> GetTaxRateAsync(TaxRateRequest taxRateRequest)
        {
            if (taxRateRequest.Address == null)
                return new TaxRateResult { Errors = new List<string> { "Address is not set" } };

            //get tax rate
            var taxRate = await _avalaraTaxManager.GetTaxRateAsync(taxRateRequest);
            if (!taxRate.HasValue)
                return new TaxRateResult { Errors = new List<string> { "No response from the service" } };

            return new TaxRateResult { TaxRate = taxRate.Value };
        }

        /// <summary>
        /// Gets tax total
        /// </summary>
        /// <param name="taxTotalRequest">Tax total request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax total
        /// </returns>
        public async Task<TaxTotalResult> GetTaxTotalAsync(TaxTotalRequest taxTotalRequest)
        {
            //cache tax total within the request
            var key = $"nop.TaxTotal-{taxTotalRequest.UsePaymentMethodAdditionalFee}";
            if (!(_actionContextAccessor.ActionContext.HttpContext.Items.TryGetValue(key, out var result) &&
                result is TaxTotalResult taxTotalResult))
            {
                //create a transaction
                var transaction = await _avalaraTaxManager.CreateTaxTotalTransactionAsync(taxTotalRequest);
                if (transaction?.totalTax == null)
                    return new TaxTotalResult { Errors = new List<string> { "No response from the service" } };

                //and get tax details
                taxTotalResult = new TaxTotalResult { TaxTotal = transaction.totalTax.Value };
                var taxRates = transaction.summary?
                    .Where(summary => summary.rate.HasValue && summary.tax.HasValue)
                    .Select(summary => new { Rate = summary.rate.Value * 100, Value = summary.tax.Value })
                    .ToList();

                foreach (var taxRate in taxRates)
                {
                    if (taxTotalResult.TaxRates.ContainsKey(taxRate.Rate))
                        taxTotalResult.TaxRates[taxRate.Rate] += taxRate.Value;
                    else
                        taxTotalResult.TaxRates.Add(taxRate.Rate, taxRate.Value);
                }

                _actionContextAccessor.ActionContext.HttpContext.Items.TryAdd(key, taxTotalResult);
            }

            return taxTotalResult;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(AvalaraTaxDefaults.ConfigurationRouteName);
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widget zones
        /// </returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            {
                AdminWidgetZones.CustomerDetailsBlock,
                AdminWidgetZones.CustomerRoleDetailsTop,
                AdminWidgetZones.ProductListButtons,
                PublicWidgetZones.CheckoutConfirmTop,
                PublicWidgetZones.OpCheckoutConfirmTop,
                PublicWidgetZones.OrderSummaryContentBefore
            });
        }

        /// <summary>
        /// Gets a type of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component type</returns>
        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone.Equals(AdminWidgetZones.CustomerDetailsBlock) ||
                widgetZone.Equals(AdminWidgetZones.CustomerRoleDetailsTop))
                return typeof(EntityUseCodeViewComponent);

            if (widgetZone.Equals(AdminWidgetZones.ProductListButtons))
                return typeof(ExportItemsViewComponent);

            if (widgetZone.Equals(PublicWidgetZones.CheckoutConfirmTop) ||
                widgetZone.Equals(PublicWidgetZones.OpCheckoutConfirmTop))
                return typeof(AddressValidationViewComponent);

            if (widgetZone.Equals(PublicWidgetZones.OrderSummaryContentBefore))
                return typeof(AppliedCertificateViewComponent);

            return null;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new AvalaraTaxSettings
            {
                CompanyCode = Guid.Empty.ToString(),
                UseSandbox = true,
                CommitTransactions = true,
                TaxOriginAddressType = TaxOriginAddressType.DefaultTaxAddress,
                EnableLogging = true,
                UseTaxRateTables = true,
                GetTaxRateByAddressOnly = true,
                TaxRateByAddressCacheTime = 480,
                AutoValidateCertificate = true,
                AllowEditCustomer = true,
                DisplayNoValidCertificatesMessage = true
            });

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(AvalaraTaxDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(AvalaraTaxDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            //schedule task
            if (await _scheduleTaskService.GetTaskByTypeAsync(AvalaraTaxDefaults.DownloadTaxRatesTask.Type) is null)
            {
                await _scheduleTaskService.InsertTaskAsync(new()
                {
                    Enabled = true,
                    LastEnabledUtc = DateTime.UtcNow,
                    Seconds = AvalaraTaxDefaults.DownloadTaxRatesTask.Days * 24 * 60 * 60,
                    Name = AvalaraTaxDefaults.DownloadTaxRatesTask.Name,
                    Type = AvalaraTaxDefaults.DownloadTaxRatesTask.Type
                });
            }

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Enums.Nop.Plugin.Tax.Avalara.Domain.TaxOriginAddressType.DefaultTaxAddress"] = "Default tax address",
                ["Enums.Nop.Plugin.Tax.Avalara.Domain.TaxOriginAddressType.ShippingOrigin"] = "Shipping origin address",
                ["Plugins.Tax.Avalara.AddressValidation.Confirm"] = "For the correct tax calculation we need the most accurate address, so we clarified the address you entered ({0}) through the validation system. Do you confirm the use of this updated address ({1})?",
                ["Plugins.Tax.Avalara.AddressValidation.Error"] = "For the correct tax calculation we need the most accurate address. There are some errors from the validation system: {0}",
                ["Plugins.Tax.Avalara.Configuration"] = "Configuration",
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
                ["Plugins.Tax.Avalara.Fields.AccountId"] = "Account ID",
                ["Plugins.Tax.Avalara.Fields.AccountId.Hint"] = "Specify Avalara account ID.",
                ["Plugins.Tax.Avalara.Fields.AccountId.Required"] = "Account ID is required",
                ["Plugins.Tax.Avalara.Fields.AllowEditCustomer"] = "Allow edit info",
                ["Plugins.Tax.Avalara.Fields.AllowEditCustomer.Hint"] = "Determine whether to allow customers to edit their info (name, phone, address, etc) when managing certificates. If disabled, the info will be auto updated when customers change details in their accounts.",
                ["Plugins.Tax.Avalara.Fields.AutoValidateCertificate"] = "Auto validate certificates",
                ["Plugins.Tax.Avalara.Fields.AutoValidateCertificate.Hint"] = "Determine whether the new certificates are automatically valid, this allows your customers to make exempt purchases right away, otherwise a customer is not treated as exempt until you validate the document.",
                ["Plugins.Tax.Avalara.Fields.CommitTransactions"] = "Commit transactions",
                ["Plugins.Tax.Avalara.Fields.CommitTransactions.Hint"] = "Determine whether to commit tax transactions right after they are saved.",
                ["Plugins.Tax.Avalara.Fields.Company"] = "Company",
                ["Plugins.Tax.Avalara.Fields.Company.Currency.Warning"] = "The default currency used by '{0}' company ({1}) does not match the primary store currency ({2})",
                ["Plugins.Tax.Avalara.Fields.Company.Hint"] = "Choose a company that was previously added to the Avalara account.",
                ["Plugins.Tax.Avalara.Fields.Company.NotExist"] = "There are no active companies",
                ["Plugins.Tax.Avalara.Fields.CustomerRoles"] = "Limited to customer roles",
                ["Plugins.Tax.Avalara.Fields.CustomerRoles.Hint"] = "Select customer roles for which exemption certificates will be available. Leave empty if you want this feature to be available to all customers.",
                ["Plugins.Tax.Avalara.Fields.DisplayNoValidCertificatesMessage"] = "Display 'No valid certificates' message",
                ["Plugins.Tax.Avalara.Fields.DisplayNoValidCertificatesMessage.Hint"] = "Determine whether to display a message that there are no valid certificates for the customer on the order confirmation page.",
                ["Plugins.Tax.Avalara.Fields.EnableCertificates"] = "Enable exemption certificates",
                ["Plugins.Tax.Avalara.Fields.EnableCertificates.Hint"] = "Determine whether to enable this feature. In this case, a new page will be added in the account section, so customers can manage their exemption certificates before making a purchase.",
                ["Plugins.Tax.Avalara.Fields.EnableCertificates.Warning"] = "To use this feature, you need the following information from customers: name, country, state, city, address, postal code. Ensure that the appropriate Customer form fields are enabled under <a href=\"{0}\" target=\"_blank\">Customer settings</a>",
                ["Plugins.Tax.Avalara.Fields.EnableLogging"] = "Enable logging",
                ["Plugins.Tax.Avalara.Fields.EnableLogging.Hint"] = "Determine whether to enable logging of all requests to Avalara services.",
                ["Plugins.Tax.Avalara.Fields.EntityUseCode"] = "Entity use code",
                ["Plugins.Tax.Avalara.Fields.EntityUseCode.Hint"] = "Choose a code that can be used to designate the reason for a particular sale being exempt. Each entity use code stands for a different exemption reason, the logic of which can be found in Avalara exemption reason documentation.",
                ["Plugins.Tax.Avalara.Fields.EntityUseCode.None"] = "None",
                ["Plugins.Tax.Avalara.Fields.GetTaxRateByAddressOnly"] = "Tax rates by address only",
                ["Plugins.Tax.Avalara.Fields.GetTaxRateByAddressOnly.Hint"] = "Determine whether to get tax rates by the address only. This may lead to not entirely accurate results (for example, when a customer is exempt to tax, or the product belongs to a tax category that has a specific rate), but it will significantly reduce the number of GetTax API calls. This applies only to tax rates in the catalog, on the checkout full information is always used in requests.",
                ["Plugins.Tax.Avalara.Fields.LicenseKey"] = "License key",
                ["Plugins.Tax.Avalara.Fields.LicenseKey.Hint"] = "Specify Avalara account license key.",
                ["Plugins.Tax.Avalara.Fields.LicenseKey.Required"] = "Account license key is required",
                ["Plugins.Tax.Avalara.Fields.TaxCodeDescription"] = "Description",
                ["Plugins.Tax.Avalara.Fields.TaxCodeType"] = "Type",
                ["Plugins.Tax.Avalara.Fields.TaxCodeType.Hint"] = "Choose a tax code type.",
                ["Plugins.Tax.Avalara.Fields.TaxOriginAddressType"] = "Tax origin address",
                ["Plugins.Tax.Avalara.Fields.TaxOriginAddressType.DefaultTaxAddress.Warning"] = "Ensure that you have correctly filled in the 'Default tax address' under <a href=\"{0}\" target=\"_blank\">Tax settings</a>",
                ["Plugins.Tax.Avalara.Fields.TaxOriginAddressType.Hint"] = "Choose which address will be used as the origin for tax requests to Avalara services.",
                ["Plugins.Tax.Avalara.Fields.TaxOriginAddressType.ShippingOrigin.Warning"] = "Ensure that you have correctly filled in the 'Shipping origin' under <a href=\"{0}\" target=\"_blank\">Shipping settings</a>",
                ["Plugins.Tax.Avalara.Fields.UseSandbox"] = "Use sandbox",
                ["Plugins.Tax.Avalara.Fields.UseSandbox.Hint"] = "Determine whether to use sandbox (testing environment).",
                ["Plugins.Tax.Avalara.Fields.UseTaxRateTables"] = "Use tax rate tables to estimate ",
                ["Plugins.Tax.Avalara.Fields.UseTaxRateTables.Hint"] = "Determine whether to use tax rate tables to estimate. This will be used as a default tax calculation for catalog pages and will be adjusted and reconciled to the final transaction tax during checkout. Tax rates are looked up by zip code (US only) in a file that will be periodically updated from the Avalara (see Schedule tasks).",
                ["Plugins.Tax.Avalara.Fields.ValidateAddress"] = "Validate address",
                ["Plugins.Tax.Avalara.Fields.ValidateAddress.Hint"] = "Determine whether to validate entered by customer addresses before the tax calculation.",
                ["Plugins.Tax.Avalara.Items.Export"] = "Export to Avalara (selected)",
                ["Plugins.Tax.Avalara.Items.Export.AlreadyExported"] = "Selected products have already been exported",
                ["Plugins.Tax.Avalara.Items.Export.Error"] = "An error has occurred on export products",
                ["Plugins.Tax.Avalara.Items.Export.Success"] = "Successfully exported {0} products",
                ["Plugins.Tax.Avalara.Log"] = "Log",
                ["Plugins.Tax.Avalara.Log.BackToList"] = "back to log",
                ["Plugins.Tax.Avalara.Log.ClearLog"] = "Clear log",
                ["Plugins.Tax.Avalara.Log.CreatedDate"] = "Created on",
                ["Plugins.Tax.Avalara.Log.CreatedDate.Hint"] = "Date and time the log entry was created.",
                ["Plugins.Tax.Avalara.Log.Customer"] = "Customer",
                ["Plugins.Tax.Avalara.Log.Customer.Hint"] = "Name of the customer.",
                ["Plugins.Tax.Avalara.Log.Deleted"] = "The log entry has been deleted successfully.",
                ["Plugins.Tax.Avalara.Log.Hint"] = "View log entry details",
                ["Plugins.Tax.Avalara.Log.RequestMessage"] = "Request message",
                ["Plugins.Tax.Avalara.Log.RequestMessage.Hint"] = "The details of the request.",
                ["Plugins.Tax.Avalara.Log.ResponseMessage"] = "Response message",
                ["Plugins.Tax.Avalara.Log.ResponseMessage.Hint"] = "The details of the response.",
                ["Plugins.Tax.Avalara.Log.StatusCode"] = "Status code",
                ["Plugins.Tax.Avalara.Log.StatusCode.Hint"] = "The status code of the response.",
                ["Plugins.Tax.Avalara.Log.Url"] = "Url",
                ["Plugins.Tax.Avalara.Log.Url.Hint"] = "The requested URL.",
                ["Plugins.Tax.Avalara.Log.Search.CreatedFrom"] = "Created from",
                ["Plugins.Tax.Avalara.Log.Search.CreatedFrom.Hint"] = "The creation from date for the search.",
                ["Plugins.Tax.Avalara.Log.Search.CreatedTo"] = "Created to",
                ["Plugins.Tax.Avalara.Log.Search.CreatedTo.Hint"] = "The creation to date for the search.",
                ["Plugins.Tax.Avalara.TaxCodes"] = "Avalara tax codes",
                ["Plugins.Tax.Avalara.TaxCodes.Delete"] = "Delete Avalara system tax codes",
                ["Plugins.Tax.Avalara.TaxCodes.Delete.Error"] = "An error has occurred on delete tax codes",
                ["Plugins.Tax.Avalara.TaxCodes.Delete.Success"] = "System tax codes successfully deleted",
                ["Plugins.Tax.Avalara.TaxCodes.Export"] = "Export tax codes to Avalara",
                ["Plugins.Tax.Avalara.TaxCodes.Export.AlreadyExported"] = "All tax codes have already been exported",
                ["Plugins.Tax.Avalara.TaxCodes.Export.Error"] = "An error has occurred on export tax codes",
                ["Plugins.Tax.Avalara.TaxCodes.Export.Success"] = "Successfully exported {0} tax codes",
                ["Plugins.Tax.Avalara.TaxCodes.Import"] = "Import Avalara system tax codes",
                ["Plugins.Tax.Avalara.TaxCodes.Import.Error"] = "An error has occurred on import tax codes",
                ["Plugins.Tax.Avalara.TaxCodes.Import.Success"] = "Successfully imported {0} tax codes",
                ["Plugins.Tax.Avalara.TestTax"] = "Test tax calculation",
                ["Plugins.Tax.Avalara.TestTax.Button"] = "Submit",
                ["Plugins.Tax.Avalara.TestTax.Error"] = "An error has occurred on tax request",
                ["Plugins.Tax.Avalara.TestTax.Success"] = "The tax was successfully received"
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //generic attributes
            await _avalaraTaxManager.DeleteAttributesAsync();

            //settings            
            _taxSettings.ActiveTaxProviderSystemName = (await _taxPluginManager.LoadAllPluginsAsync())
                .FirstOrDefault(taxProvider => !taxProvider.PluginDescriptor.SystemName.Equals(AvalaraTaxDefaults.SystemName))
                ?.PluginDescriptor.SystemName;
            await _settingService.SaveSettingAsync(_taxSettings);
            _widgetSettings.ActiveWidgetSystemNames.Remove(AvalaraTaxDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
            await _settingService.DeleteSettingAsync<AvalaraTaxSettings>();

            //schedule task
            var task = await _scheduleTaskService.GetTaskByTypeAsync(AvalaraTaxDefaults.DownloadTaxRatesTask.Type);
            if (task is not null)
                await _scheduleTaskService.DeleteTaskAsync(task);

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Enums.Nop.Plugin.Tax.Avalara.Domain");
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Tax.Avalara");

            await base.UninstallAsync();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => true;

        #endregion
    }
}