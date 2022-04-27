using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Cms;
using Nop.Plugin.Shipping.EasyPost.Services;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Web.Framework;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Shipping.EasyPost
{
    /// <summary>
    /// Represents shipping provider implementation
    /// </summary>
    public class EasyPostProvider : BasePlugin, IShippingRateComputationMethod, IWidgetPlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly EasyPostService _easyPostService;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IShippingPluginManager _shippingPluginManager;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public EasyPostProvider(EasyPostService easyPostService,
            IActionContextAccessor actionContextAccessor,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IShippingPluginManager shippingPluginManager,
            IUrlHelperFactory urlHelperFactory,
            WidgetSettings widgetSettings)
        {
            _easyPostService = easyPostService;
            _actionContextAccessor = actionContextAccessor;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _shippingPluginManager = shippingPluginManager;
            _urlHelperFactory = urlHelperFactory;
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the represents a response of getting shipping rate options
        /// </returns>
        public async Task<GetShippingOptionResponse> GetShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest is null)
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

            if (getShippingOptionRequest.Customer is null)
                return new GetShippingOptionResponse { Errors = new[] { "Customer is not set" } };

            if (!getShippingOptionRequest.Items?.Any() ?? true)
                return new GetShippingOptionResponse { Errors = new[] { "No shipment items" } };

            if (getShippingOptionRequest.ShippingAddress?.CountryId is null)
                return new GetShippingOptionResponse { Errors = new[] { "Shipping address is not set" } };

            var (rates, error) = await _easyPostService.GetShippingOptionsAsync(getShippingOptionRequest);
            if (rates is null || !string.IsNullOrEmpty(error))
            {
                //add a friendly message besides the error
                var friendlyError = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Checkout.Error");
                return new GetShippingOptionResponse { Errors = new[] { error, friendlyError } };
            }

            return new GetShippingOptionResponse { ShippingOptions = rates };
        }

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the fixed shipping rate; or null in case there's no fixed shipping rate
        /// </returns>
        public Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            return Task.FromResult<decimal?>(null);
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(EasyPostDefaults.ConfigurationRouteName);
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
                AdminWidgetZones.ProductDetailsBlock,
                AdminWidgetZones.OrderShipmentDetailsButtons,
                PublicWidgetZones.OpCheckoutShippingMethodTop
            });
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone.Equals(AdminWidgetZones.ProductDetailsBlock))
                return EasyPostDefaults.PRODUCT_DETAILS_VIEW_COMPONENT_NAME;

            if (widgetZone.Equals(AdminWidgetZones.OrderShipmentDetailsButtons))
                return EasyPostDefaults.SHIPMENT_DETAILS_VIEW_COMPONENT_NAME;

            if (widgetZone.Equals(PublicWidgetZones.OpCheckoutShippingMethodTop))
                return EasyPostDefaults.SHIPPING_METHODS_VIEW_COMPONENT_NAME;

            return null;
        }

        /// <summary>
        /// Manage sitemap. You can use "SystemName" of menu items to manage existing sitemap or add a new menu item.
        /// </summary>
        /// <param name="rootNode">Root node of the sitemap.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _shippingPluginManager.IsPluginActiveAsync(EasyPostDefaults.SystemName))
                return;

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return;

            var salesNode = rootNode.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Sales"));
            if (salesNode is null)
                return;

            var shipmentsNode = salesNode.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Shipments"));
            if (shipmentsNode is null)
                return;

            salesNode.ChildNodes.Insert(salesNode.ChildNodes.IndexOf(shipmentsNode) + 1, new SiteMapNode
            {
                SystemName = "EasyPost Batches",
                Title = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Batch"),
                ControllerName = "EasyPost",
                ActionName = "BatchList",
                IconClass = "far fa-dot-circle",
                Visible = true,
                RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } }
            });
        }

        /// <summary>
        /// Get associated shipment tracker
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipment tracker
        /// </returns>
        public Task<IShipmentTracker> GetShipmentTrackerAsync()
        {
            return Task.FromResult<IShipmentTracker>(new EasyPostTracker(_easyPostService, _shippingPluginManager));
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            await _settingService.SaveSettingAsync(new EasyPostSettings
            {
                UseSandbox = true,
                UseAllAvailableCarriers = true,
                LogShipmentMessages = true,
                UseSmartRates = true
            });

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(EasyPostDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(EasyPostDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Shipping.EasyPost.Batch"] = "EasyPost Batches",
                ["Plugins.Shipping.EasyPost.Batch.BackToList"] = "back to batch list",
                ["Plugins.Shipping.EasyPost.Batch.DownloadLabel"] = "Download label",
                ["Plugins.Shipping.EasyPost.Batch.DownloadManifest"] = "Download manifest",
                ["Plugins.Shipping.EasyPost.Batch.Edit"] = "Edit batch details",
                ["Plugins.Shipping.EasyPost.Batch.Fields.CreatedOn"] = "Created on",
                ["Plugins.Shipping.EasyPost.Batch.Fields.Id"] = "Batch Id",
                ["Plugins.Shipping.EasyPost.Batch.Fields.PickupStatus"] = "Pickup status",
                ["Plugins.Shipping.EasyPost.Batch.Fields.Status"] = "Status",
                ["Plugins.Shipping.EasyPost.Batch.Fields.UpdatedOn"] = "Updated on",
                ["Plugins.Shipping.EasyPost.Batch.GenerateLabel"] = "Generate label",
                ["Plugins.Shipping.EasyPost.Batch.GenerateLabel.Pdf"] = "PDF",
                ["Plugins.Shipping.EasyPost.Batch.GenerateLabel.Zpl"] = "ZPL",
                ["Plugins.Shipping.EasyPost.Batch.GenerateManifest"] = "Generate manifest",
                ["Plugins.Shipping.EasyPost.Batch.Search.Status"] = "Status",
                ["Plugins.Shipping.EasyPost.Batch.Search.Status.Hint"] = "Search by the status.",
                ["Plugins.Shipping.EasyPost.Batch.Shipments"] = "Associated shipments",
                ["Plugins.Shipping.EasyPost.Batch.Shipments.Add"] = "Add shipments",
                ["Plugins.Shipping.EasyPost.Batch.Shipments.Add.Save"] = "Add selected",

                ["Plugins.Shipping.EasyPost.Checkout.AddressVerification.Warning"] = "Unable to verify the address: {0}. Please double check your input and if you are sure that data is correct, ignore this message.",
                ["Plugins.Shipping.EasyPost.Checkout.Error"] = "Failed to get shipping rates, please contact the manager.",

                ["Plugins.Shipping.EasyPost.Configuration.AddressVerification"] = "Address verification",
                ["Plugins.Shipping.EasyPost.Configuration.Carriers"] = "Carriers",
                ["Plugins.Shipping.EasyPost.Configuration.Credentials"] = "Credentials",
                ["Plugins.Shipping.EasyPost.Configuration.Currency.Warning"] = "Required currency ({0}) is not found. Make sure it is created on <a href=\"{1}\" target=\"_blank\">this page</a>",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.AddressVerification"] = "Address verification",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.AddressVerification.Hint"] = "Check to use the address verification. Verification system will automatically make minor corrections to spelling/format if applicable.",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.ApiKey"] = "Production API key",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.ApiKey.Hint"] = "Specify EasyPost production API key.",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.ApiKey.Required"] = "API key is required",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.CarrierAccounts"] = "Carrier accounts",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.CarrierAccounts.Hint"] = "Select carrier accounts whose shipping rates to use.",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.CarrierAccounts.None"] = "There are no configured carrier accounts",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.StrictAddressVerification"] = "Strict address verification",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.StrictAddressVerification.Hint"] = "Check to use the strict address verification. The failure of this verification causes the whole request to fail and the customer will need to correct and specify the address again.",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.TestApiKey"] = "Test API key",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.TestApiKey.Hint"] = "Specify EasyPost test API key.",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.UseAllAvailableCarriers"] = "Use all available carriers",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.UseAllAvailableCarriers.Hint"] = "Check to use all available carrier accounts to get shipping rates.",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.UseSandbox"] = "Test mode",
                ["Plugins.Shipping.EasyPost.Configuration.Fields.UseSandbox.Hint"] = "Check to use Test mode for testing purposes.",
                ["Plugins.Shipping.EasyPost.Configuration.Measures.Warning"] = "Required measures ({0} and {1}) are not found. Make sure they are created and the ratio is set correctly on <a href=\"{2}\" target=\"_blank\">this page</a>",

                ["Plugins.Shipping.EasyPost.Error"] = "Error: {0} (see details in the <a href=\"{1}\" target=\"_blank\">log</a>)",
                ["Plugins.Shipping.EasyPost.Error.Alert"] = "Error: {0} (see details in the log)",

                ["Plugins.Shipping.EasyPost.Pickup"] = "Pickup",
                ["Plugins.Shipping.EasyPost.Pickup.Buy"] = "Buy pickup",
                ["Plugins.Shipping.EasyPost.Pickup.Cancel"] = "Cancel pickup",
                ["Plugins.Shipping.EasyPost.Pickup.Create"] = "Create pickup",
                ["Plugins.Shipping.EasyPost.Pickup.Instructions"] = "Instructions",
                ["Plugins.Shipping.EasyPost.Pickup.Instructions.Hint"] = "Additional text to help the driver successfully obtain the package.",
                ["Plugins.Shipping.EasyPost.Pickup.MaxDate"] = "Max date",
                ["Plugins.Shipping.EasyPost.Pickup.MaxDate.Hint"] = "The latest time at which the package is available to pick up.",
                ["Plugins.Shipping.EasyPost.Pickup.MinDate"] = "Min date",
                ["Plugins.Shipping.EasyPost.Pickup.MinDate.Hint"] = "The earliest time at which the package is available to pick up.",
                ["Plugins.Shipping.EasyPost.Pickup.Rate"] = "Rate",
                ["Plugins.Shipping.EasyPost.Pickup.Rate.Hint"] = "Specify the rate to purchase a pickup.",
                ["Plugins.Shipping.EasyPost.Pickup.Rate.None"] = "No available rates",

                ["Plugins.Shipping.EasyPost.Product"] = "EasyPost",
                ["Plugins.Shipping.EasyPost.Product.CustomsInfo"] = "Customs Info",
                ["Plugins.Shipping.EasyPost.Product.Fields.HtsNumber"] = "Harmonized Tariff Schedule",
                ["Plugins.Shipping.EasyPost.Product.Fields.HtsNumber.Hint"] = "Specify the six digit code for your product as specified by harmonized system for tariffs.",
                ["Plugins.Shipping.EasyPost.Product.Fields.OriginCountry"] = "Origin country",
                ["Plugins.Shipping.EasyPost.Product.Fields.OriginCountry.Hint"] = "Specify where the product was manufactured or assembled.",
                ["Plugins.Shipping.EasyPost.Product.Fields.PredefinedPackage"] = "Predefined package",
                ["Plugins.Shipping.EasyPost.Product.Fields.PredefinedPackage.Hint"] = "Specify a predefined package for this product (used only if it's the single item in the cart with a quantity of 1).",
                ["Plugins.Shipping.EasyPost.Product.PredefinedPackage"] = "Predefined package",

                ["Plugins.Shipping.EasyPost.Shipment"] = "EasyPost",
                ["Plugins.Shipping.EasyPost.Shipment.BuyLabel"] = "Buy label",

                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo"] = "Customs Info",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.ContentsExplanation"] = "Content explanation",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.ContentsExplanation.Hint"] = "If you specify 'Other' as the content type, you must supply a brief description in this field.",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.ContentsType"] = "Content type",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.ContentsType.Hint"] = "Specify the type of item you are sending.",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.CustomsCertify"] = "Certify",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.CustomsCertify.Hint"] = "Determine a value that takes the place of the signature on the physical customs form. This is how you indicate that the information you have provided is accurate.",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.CustomsSigner"] = "Signer",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.CustomsSigner.Hint"] = "Enter the name of the person who is certifying that the information provided on the customs form is accurate. Use a name of the person in your organization who is responsible for this.",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.EelPfc"] = "EEL or PFC",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.EelPfc.Hint"] = "When shipping outside the US, you need to provide either an Exemption and Exclusion Legend (EEL) code or a Proof of Filing Citation (PFC). Which you need is based on the value of the goods being shipped.",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.NonDeliveryOption"] = "Non-delivery option",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.NonDeliveryOption.Hint"] = "Select an option that tells the carrier what you want to happen to the package if the shipment cannot be delivered. If you pass 'Abandon', you will not receive the package back if it cannot be delivered.",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.RestrictionComments"] = "Restriction comments",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.RestrictionComments.Hint"] = "If you specify 'Other' as the restriction type, you must supply a brief description of what is required.",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.RestrictionType"] = "Restriction type",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.RestrictionType.Hint"] = "Determine if your shipment requires any special treatment / quarantine when entering the country.",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.UseCustomsInfo"] = "Use customs info",
                ["Plugins.Shipping.EasyPost.Shipment.CustomsInfo.UseCustomsInfo.Hint"] = "Determine whether to pass the customs information.",

                ["Plugins.Shipping.EasyPost.Shipment.DownloadLabel"] = "Download label",
                ["Plugins.Shipping.EasyPost.Shipment.DownloadLabel.Png"] = "PNG",
                ["Plugins.Shipping.EasyPost.Shipment.DownloadLabel.Pdf"] = "PDF",
                ["Plugins.Shipping.EasyPost.Shipment.DownloadLabel.Zpl"] = "ZPL",
                ["Plugins.Shipping.EasyPost.Shipment.DownloadInvoice"] = "Download invoice",

                ["Plugins.Shipping.EasyPost.Shipment.Fields.RefundStatus"] = "Refund status",
                ["Plugins.Shipping.EasyPost.Shipment.Fields.Id"] = "Shipment id",
                ["Plugins.Shipping.EasyPost.Shipment.Fields.Insurance"] = "Insurance",
                ["Plugins.Shipping.EasyPost.Shipment.Fields.PickupStatus"] = "Pickup status",
                ["Plugins.Shipping.EasyPost.Shipment.Fields.Rate"] = "Rate name",
                ["Plugins.Shipping.EasyPost.Shipment.Fields.RateValue"] = "Rate value",
                ["Plugins.Shipping.EasyPost.Shipment.Fields.Status"] = "Status",
                ["Plugins.Shipping.EasyPost.Shipment.Insurance"] = "Insurance",
                ["Plugins.Shipping.EasyPost.Shipment.Insurance.Hint"] = "Specify an amount to insure shipment.",

                ["Plugins.Shipping.EasyPost.Shipment.Options"] = "Options",
                ["Plugins.Shipping.EasyPost.Shipment.Options.AdditionalHandling"] = "Additional handling",
                ["Plugins.Shipping.EasyPost.Shipment.Options.AdditionalHandling.Hint"] = @"
                    Setting this option, will add an additional handling charge. An Additional Handling charge may be applied to the following:
                    Any article that is encased in an outside shipping container made of metal or wood.
                    Any item, such as a barrel, drum, pail or tire, that is not fully encased in a corrugated cardboard shipping container.
                    Any package with the longest side exceeding 60 inches or its second longest side exceeding 30 inches.
                    Any package with an actual weight greater than 70 pounds.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.Alcohol"] = "Alcohol",
                ["Plugins.Shipping.EasyPost.Shipment.Options.Alcohol.Hint"] = "Set this option if your shipment contains alcohol.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.ByDrone"] = "By drone",
                ["Plugins.Shipping.EasyPost.Shipment.Options.ByDrone.Hint"] = "Setting this option will indicate to the carrier to prefer delivery by drone, if the carrier supports drone delivery.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.CarbonNeutral"] = "Carbon neutral",
                ["Plugins.Shipping.EasyPost.Shipment.Options.CarbonNeutral.Hint"] = "Setting this optione will add a charge to reduce carbon emissions.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.DeliveryConfirmation"] = "Delivery confirmation",
                ["Plugins.Shipping.EasyPost.Shipment.Options.DeliveryConfirmation.Hint"] = "Choose an option to request a signature. You may also request 'No Signature' to leave the package at the door.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.Endorsement"] = "Endorsement",
                ["Plugins.Shipping.EasyPost.Shipment.Options.Endorsement.Hint"] = "Choose an endorsement type.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.HandlingInstructions"] = "Handling instructions",
                ["Plugins.Shipping.EasyPost.Shipment.Options.HandlingInstructions.Hint"] = "This is to designate special instructions for the carrier like 'Do not drop!'.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.Hazmat"] = "Hazmat",
                ["Plugins.Shipping.EasyPost.Shipment.Options.Hazmat.Hint"] = "Dangerous goods indicator. Applies to USPS, FedEx and DHL eCommerce.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.InvoiceNumber"] = "Invoice number",
                ["Plugins.Shipping.EasyPost.Shipment.Options.InvoiceNumber.Hint"] = "This will print an invoice number on the postage label.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.Machinable"] = "Machinable",
                ["Plugins.Shipping.EasyPost.Shipment.Options.Machinable.Hint"] = "Whether or not the parcel can be processed by the carriers equipment.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.PrintCustom"] = "Custom message",
                ["Plugins.Shipping.EasyPost.Shipment.Options.PrintCustom.Hint"] = "You can optionally print up to 3 custom messages on labels. The locations of these fields show up on different spots on the carrier's labels.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.PrintCustomCode"] = "Custom message type",
                ["Plugins.Shipping.EasyPost.Shipment.Options.PrintCustomCode.Hint"] = "Specify the type of custom message.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.SpecialRatesEligibility"] = "Special rates",
                ["Plugins.Shipping.EasyPost.Shipment.Options.SpecialRatesEligibility.Hint"] = "This option allows you to request restrictive rates from USPS.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.CertifiedMail"] = "Certified mail",
                ["Plugins.Shipping.EasyPost.Shipment.Options.CertifiedMail.Hint"] = "Certified Mail provides the sender with a mailing receipt and, upon request, electronic verification that an article was delivered or that a delivery attempt was made.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.RegisteredMail"] = "Registered mail",
                ["Plugins.Shipping.EasyPost.Shipment.Options.RegisteredMail.Hint"] = "Registered Mail is the most secure service that the USPS offers. It incorporates a system of receipts to monitor the movement of the mail from the point of acceptance to delivery.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.RegisteredMailAmount"] = "Registered mail amount",
                ["Plugins.Shipping.EasyPost.Shipment.Options.RegisteredMailAmount.Hint"] = "The value of the package contents.",
                ["Plugins.Shipping.EasyPost.Shipment.Options.ReturnReceipt"] = "Return receipt",
                ["Plugins.Shipping.EasyPost.Shipment.Options.ReturnReceipt.Hint"] = "An electronic return receipt may be purchased at the time of mailing and provides a shipper with evidence of delivery (to whom the mail was delivered and date of delivery), and information about the recipient's actual delivery address. Only applies to the USPS.",

                ["Plugins.Shipping.EasyPost.Shipment.Rate"] = "Rate",
                ["Plugins.Shipping.EasyPost.Shipment.Rate.Hint"] = "Specify the rate to purchase a shipment.",
                ["Plugins.Shipping.EasyPost.Shipment.Rate.None"] = "No available rates",
                ["Plugins.Shipping.EasyPost.Shipment.Rate.Selected"] = "(selected by the customer)",
                ["Plugins.Shipping.EasyPost.Shipment.Rate.SmartRate.Display"] = "Display Smart Rates",
                ["Plugins.Shipping.EasyPost.Shipment.Rate.SmartRate.Display.Hint"] = "Check to display Smart Rates for this shipment.",
                ["Plugins.Shipping.EasyPost.Shipment.Rate.SmartRate.DeliveryDays"] = "Delivery days",
                ["Plugins.Shipping.EasyPost.Shipment.Rate.SmartRate.Name"] = "Name",
                ["Plugins.Shipping.EasyPost.Shipment.UpdateShipment"] = "Update",

                ["Plugins.Shipping.EasyPost.Success"] = "Successfully completed",
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            await _easyPostService.DeleteWebhookAsync();

            await _settingService.DeleteSettingAsync<EasyPostSettings>();

            if (_widgetSettings.ActiveWidgetSystemNames.Contains(EasyPostDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Remove(EasyPostDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Shipping.EasyPost");

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