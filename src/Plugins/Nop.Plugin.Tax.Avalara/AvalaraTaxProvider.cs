using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Tax.Avalara.Domain;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
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

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly ITaxPluginManager _taxPluginManager;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly TaxSettings _taxSettings;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public AvalaraTaxProvider(AvalaraTaxManager avalaraTaxManager,
            IActionContextAccessor actionContextAccessor,
            ILocalizationService localizationService,
            ISettingService settingService,
            ITaxPluginManager taxPluginManager,
            IUrlHelperFactory urlHelperFactory,
            TaxSettings taxSettings,
            WidgetSettings widgetSettings)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _actionContextAccessor = actionContextAccessor;
            _localizationService = localizationService;
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
        /// <returns>Tax</returns>
        public TaxRateResult GetTaxRate(TaxRateRequest taxRateRequest)
        {
            if (taxRateRequest.Address == null)
                return new TaxRateResult { Errors = new List<string> { "Address is not set" } };

            //get tax rate
            var taxRate = _avalaraTaxManager.GetTaxRate(taxRateRequest);
            if (!taxRate.HasValue)
                return new TaxRateResult { Errors = new List<string> { "No response from the service" } };

            return new TaxRateResult { TaxRate = taxRate.Value };
        }

        /// <summary>
        /// Gets tax total
        /// </summary>
        /// <param name="taxTotalRequest">Tax total request</param>
        /// <returns>Tax total</returns>
        public TaxTotalResult GetTaxTotal(TaxTotalRequest taxTotalRequest)
        {
            //cache tax total within the request
            var key = $"nop.TaxTotal-{taxTotalRequest.UsePaymentMethodAdditionalFee}";
            if (!(_actionContextAccessor.ActionContext.HttpContext.Items.TryGetValue(key, out var result) &&
                result is TaxTotalResult taxTotalResult))
            {
                //create a transaction
                var transaction = _avalaraTaxManager.CreateTaxTotalTransaction(taxTotalRequest);
                if (transaction?.totalTax == null)
                    return new TaxTotalResult { Errors = new List<string> { "No response from the service" } };

                //and get tax details
                taxTotalResult = new TaxTotalResult { TaxTotal = transaction.totalTax.Value };
                transaction.summary?
                    .Where(summary => summary.rate.HasValue && summary.tax.HasValue)
                    .Select(summary => new { Rate = summary.rate.Value * 100, Value = summary.tax.Value })
                    .ToList().ForEach(taxRate =>
                    {
                        if (taxTotalResult.TaxRates.ContainsKey(taxRate.Rate))
                            taxTotalResult.TaxRates[taxRate.Rate] += taxRate.Value;
                        else
                            taxTotalResult.TaxRates.Add(taxRate.Rate, taxRate.Value);
                    });

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
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string>
            {
                AdminWidgetZones.CustomerDetailsBlock,
                AdminWidgetZones.CustomerRoleDetailsTop,
                AdminWidgetZones.ProductListButtons,
                PublicWidgetZones.CheckoutConfirmTop,
                PublicWidgetZones.OpCheckoutConfirmTop
            };
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone.Equals(AdminWidgetZones.CustomerDetailsBlock) ||
                widgetZone.Equals(AdminWidgetZones.CustomerRoleDetailsTop))
            {
                return AvalaraTaxDefaults.ENTITY_USE_CODE_VIEW_COMPONENT_NAME;
            }

            if (widgetZone.Equals(AdminWidgetZones.ProductListButtons))
                return AvalaraTaxDefaults.EXPORT_ITEMS_VIEW_COMPONENT_NAME;

            if (widgetZone.Equals(PublicWidgetZones.CheckoutConfirmTop) ||
                widgetZone.Equals(PublicWidgetZones.OpCheckoutConfirmTop))
            {
                return AvalaraTaxDefaults.ADDRESS_VALIDATION_VIEW_COMPONENT_NAME;
            }

            return null;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new AvalaraTaxSettings
            {
                CompanyCode = Guid.Empty.ToString(),
                UseSandbox = true,
                CommitTransactions = true,
                TaxOriginAddressType = TaxOriginAddressType.DefaultTaxAddress,
                EnableLogging = true
            });

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(AvalaraTaxDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(AvalaraTaxDefaults.SystemName);
                _settingService.SaveSetting(_widgetSettings);
            }

            //locales
            _localizationService.AddPluginLocaleResource(new Dictionary<string, string>
            {
                ["Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Create"] = "Create request",
                ["Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.CreateResponse"] = "Create response",
                ["Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Error"] = "Error",
                ["Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Refund"] = "Refund request",
                ["Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.RefundResponse"] = "Refund response",
                ["Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Void"] = "Void request",
                ["Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.VoidResponse"] = "Void response",
                ["Enums.Nop.Plugin.Tax.Avalara.Domain.TaxOriginAddressType.DefaultTaxAddress"] = "Default tax address",
                ["Enums.Nop.Plugin.Tax.Avalara.Domain.TaxOriginAddressType.ShippingOrigin"] = "Shipping origin address",
                ["Plugins.Tax.Avalara.AddressValidation.Confirm"] = "For the correct tax calculation we need the most accurate address, so we clarified the address you entered ({0}) through the validation system. Do you confirm the use of this updated address ({1})?",
                ["Plugins.Tax.Avalara.AddressValidation.Error"] = "For the correct tax calculation we need the most accurate address. There are some errors from the validation system: {0}",
                ["Plugins.Tax.Avalara.Configuration"] = "Configuration",
                ["Plugins.Tax.Avalara.Fields.AccountId"] = "Account ID",
                ["Plugins.Tax.Avalara.Fields.AccountId.Hint"] = "Specify Avalara account ID.",
                ["Plugins.Tax.Avalara.Fields.AccountId.Required"] = "Account ID is required",
                ["Plugins.Tax.Avalara.Fields.CommitTransactions"] = "Commit transactions",
                ["Plugins.Tax.Avalara.Fields.CommitTransactions.Hint"] = "Determine whether to commit tax transactions right after they are saved.",
                ["Plugins.Tax.Avalara.Fields.Company"] = "Company",
                ["Plugins.Tax.Avalara.Fields.Company.Currency.Warning"] = "The default currency used by '{0}' company ({1}) does not match the primary store currency ({2})",
                ["Plugins.Tax.Avalara.Fields.Company.Hint"] = "Choose a company that was previously added to the Avalara account.",
                ["Plugins.Tax.Avalara.Fields.Company.NotExist"] = "There are no active companies",
                ["Plugins.Tax.Avalara.Fields.EnableLogging"] = "Enable logging",
                ["Plugins.Tax.Avalara.Fields.EnableLogging.Hint"] = "Determine whether to enable logging of all requests to Avalara services.",
                ["Plugins.Tax.Avalara.Fields.EntityUseCode"] = "Entity use code",
                ["Plugins.Tax.Avalara.Fields.EntityUseCode.Hint"] = "Choose a code that can be used to designate the reason for a particular sale being exempt. Each entity use code stands for a different exemption reason, the logic of which can be found in Avalara exemption reason documentation.",
                ["Plugins.Tax.Avalara.Fields.EntityUseCode.None"] = "None",
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
                ["Plugins.Tax.Avalara.TestTax.Error"] = "An error has occurred on tax request",
                ["Plugins.Tax.Avalara.TestTax.Success"] = "The tax was successfully received",
                ["Plugins.Tax.Avalara.VerifyCredentials"] = "Test connection",
                ["Plugins.Tax.Avalara.VerifyCredentials.Declined"] = "Credentials declined",
                ["Plugins.Tax.Avalara.VerifyCredentials.Verified"] = "Credentials verified"
            });

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //generic attributes
            _avalaraTaxManager.DeleteAttributes();

            //settings            
            _taxSettings.ActiveTaxProviderSystemName = _taxPluginManager.LoadAllPlugins()
                .FirstOrDefault(taxProvider => !taxProvider.PluginDescriptor.SystemName.Equals(AvalaraTaxDefaults.SystemName))
                ?.PluginDescriptor.SystemName;
            _settingService.SaveSetting(_taxSettings);
            _widgetSettings.ActiveWidgetSystemNames.Remove(AvalaraTaxDefaults.SystemName);
            _settingService.SaveSetting(_widgetSettings);
            _settingService.DeleteSetting<AvalaraTaxSettings>();

            //locales
            _localizationService.DeletePluginLocaleResources("Enums.Nop.Plugin.Tax.Avalara.Domain");
            _localizationService.DeletePluginLocaleResources("Plugins.Tax.Avalara");

            base.Uninstall();
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