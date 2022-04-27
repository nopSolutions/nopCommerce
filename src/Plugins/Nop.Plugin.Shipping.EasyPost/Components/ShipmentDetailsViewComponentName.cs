using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Shipping.EasyPost.Domain.Shipment;
using Nop.Plugin.Shipping.EasyPost.Factories;
using Nop.Plugin.Shipping.EasyPost.Models.Shipment;
using Nop.Plugin.Shipping.EasyPost.Services;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Shipping.EasyPost.Components
{
    /// <summary>
    /// Represents view component to render an additional block on the shipment details page in the admin area
    /// </summary>
    [ViewComponent(Name = EasyPostDefaults.SHIPMENT_DETAILS_VIEW_COMPONENT_NAME)]
    public class ShipmentDetailsViewComponentName : NopViewComponent
    {
        #region Fields

        private readonly EasyPostModelFactory _easyPostModelFactory;
        private readonly EasyPostService _easyPostService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IShipmentService _shipmentService;
        private readonly IShippingPluginManager _shippingPluginManager;

        #endregion

        #region Ctor

        public ShipmentDetailsViewComponentName(EasyPostModelFactory easyPostModelFactory,
            EasyPostService easyPostService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IOrderService orderService,
            IPermissionService permissionService,
            IPriceFormatter priceFormatter,
            IShipmentService shipmentService,
            IShippingPluginManager shippingPluginManager)
        {
            _easyPostModelFactory = easyPostModelFactory;
            _easyPostService = easyPostService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _orderService = orderService;
            _permissionService = permissionService;
            _priceFormatter = priceFormatter;
            _shipmentService = shipmentService;
            _shippingPluginManager = shippingPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke the widget view component
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="additionalData">Additional parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            if (!await _shippingPluginManager.IsPluginActiveAsync(EasyPostDefaults.SystemName))
                return Content(string.Empty);

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return Content(string.Empty);

            if (!widgetZone.Equals(AdminWidgetZones.OrderShipmentDetailsButtons))
                return Content(string.Empty);

            if (additionalData is not ShipmentModel shipmentModel)
                return Content(string.Empty);

            var shipmentEntry = await _shipmentService.GetShipmentByIdAsync(shipmentModel.Id);
            if (shipmentEntry is null)
                return Content(string.Empty);

            var order = await _orderService.GetOrderByIdAsync(shipmentEntry.OrderId);
            if (order is null)
                return Content(string.Empty);

            if (order.ShippingRateComputationMethodSystemName != EasyPostDefaults.SystemName)
                return Content(string.Empty);

            TEnum getEnumValue<TEnum>(string value) where TEnum : Enum => typeof(TEnum).GetFields()
                .FirstOrDefault(field => field.IsLiteral &&
                    field.GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault()?.Value == value)
                ?.GetValue(null) is TEnum enumValue ? enumValue : default;

            async Task<List<SelectListItem>> prepareSelectList<TEnum>(TEnum enumValue) where TEnum : struct =>
                (await enumValue.ToSelectListAsync(markCurrentAsSelected: false, useLocalization: false))
                    .Select(item => new SelectListItem(item.Text == "None" ? "---" : item.Text, item.Value))
                    .ToList();

            var model = new ShipmentDetailsModel { Id = shipmentEntry.Id };

            var shipmentId = await _genericAttributeService.GetAttributeAsync<string>(shipmentEntry, EasyPostDefaults.ShipmentIdAttribute);
            var (shipment, shipmentError) = await _easyPostService.GetShipmentAsync(shipmentId);
            if (!string.IsNullOrEmpty(shipmentError))
            {
                model.Error = string
                    .Format(await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Error.Alert"), shipmentError);
                return View("~/Plugins/Shipping.EasyPost/Views/Shipment/ShipmentDetails.cshtml", model);
            }

            model.ShipmentId = shipment.id;

            //whether the shipment has already been created and purchased
            if (shipment.selected_rate is not null)
            {
                model.Status = shipment.status;
                model.RefundStatus = shipment.refund_status;
                model.InvoiceExists = shipment.forms
                    ?.FirstOrDefault(form => string.Equals(form.form_type, "commercial_invoice", StringComparison.InvariantCultureIgnoreCase))
                    is not null;

                var rateValue = await _easyPostService.ConvertRateAsync(shipment.selected_rate.rate, shipment.selected_rate.currency);
                model.RateValue = await _priceFormatter.FormatShippingPriceAsync(rateValue, true);
                model.RateName = $"{shipment.selected_rate.carrier} {shipment.selected_rate.service}".TrimEnd(' ');

                if (!string.IsNullOrEmpty(shipment.insurance))
                {
                    var insurance = await _easyPostService.ConvertRateAsync(shipment.insurance, null);
                    model.InsuranceValue = await _priceFormatter.FormatShippingPriceAsync(insurance, true);
                }

                model.PickupModel = await _easyPostModelFactory
                    .PreparePickupModelAsync(model.PickupModel, shipmentEntry, null, order.ShippingAddressId);
                model.PickupStatus = model.PickupModel.Status;

                return View("~/Plugins/Shipping.EasyPost/Views/Shipment/ShipmentDetails.cshtml", model);
            }

            //prepare shipment rates to select
            var (rates, _) = await _easyPostService.GetShippingRatesAsync(shipment, true);
            if (rates?.Any() ?? false)
            {
                foreach (var rate in rates.OrderBy(rate => rate.Rate))
                {
                    var rateName = $"{rate.Carrier} {rate.Service}".TrimEnd(' ');
                    var text = $"{await _priceFormatter.FormatShippingPriceAsync(rate.Rate, true)} {rateName}";
                    var selected = string.Equals(rateName, order.ShippingMethod, StringComparison.InvariantCultureIgnoreCase);
                    if (selected)
                    {
                        model.RateId = rate.Id;
                        text = $"{text} {await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Shipment.Rate.Selected")}";
                    }
                    model.AvailableRates.Add(new SelectListItem(text, rate.Id, selected));

                    if (rate.TimeInTransit?.Any(pair => pair.DeliveryDays.HasValue) ?? false)
                    {
                        var timeInTransit = rate.TimeInTransit.ToDictionary(pair => pair.Percentile, pair => pair.DeliveryDays);
                        model.SmartRates.Add((rateName, rate.DeliveryDays, timeInTransit));
                    }
                }
            }
            else
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Shipment.Rate.None");
                model.AvailableRates.Add(new SelectListItem(locale, string.Empty));
            }

            if (shipment.options is not null)
            {
                model.AdditionalHandling = shipment.options.additional_handling ?? false;
                model.Alcohol = shipment.options.alcohol ?? false;
                model.ByDrone = shipment.options.by_drone ?? false;
                model.CarbonNeutral = shipment.options.carbon_neutral ?? false;
                model.DeliveryConfirmation = (int)getEnumValue<DeliveryConfirmation>(shipment.options.delivery_confirmation);
                model.Endorsement = (int)getEnumValue<Endorsement>(shipment.options.endorsement);
                model.HandlingInstructions = shipment.options.handling_instructions;
                model.Hazmat = (int)getEnumValue<HazmatType>(shipment.options.hazmat);
                model.InvoiceNumber = shipment.options.invoice_number;
                model.Machinable = bool.TryParse(shipment.options.machinable, out var machinable) && machinable;
                model.PrintCustom1 = shipment.options.print_custom_1;
                model.PrintCustomCode1 = (int)getEnumValue<CustomCode>(shipment.options.print_custom_1_code);
                model.PrintCustom2 = shipment.options.print_custom_2;
                model.PrintCustomCode2 = (int)getEnumValue<CustomCode>(shipment.options.print_custom_2_code);
                model.PrintCustom3 = shipment.options.print_custom_3;
                model.PrintCustomCode3 = (int)getEnumValue<CustomCode>(shipment.options.print_custom_3_code);
                model.SpecialRatesEligibility = (int)getEnumValue<SpecialRate>(shipment.options.special_rates_eligibility);
                model.CertifiedMail = shipment.options.certified_mail ?? false;
                model.RegisteredMail = shipment.options.registered_mail ?? false;
                model.RegisteredMailAmount = Convert.ToDecimal(shipment.options.registered_mail_amount);
                model.ReturnReceipt = shipment.options.return_receipt ?? false;
            }

            if (shipment.customs_info is not null)
            {
                model.UseCustomsInfo = true;
                model.ContentsType = (int)getEnumValue<ContentsType>(shipment.customs_info.contents_type);
                model.RestrictionType = (int)getEnumValue<RestrictionType>(shipment.customs_info.restriction_type);
                model.NonDeliveryOption = (int)getEnumValue<NonDeliveryOption>(shipment.customs_info.non_delivery_option);
                model.ContentsExplanation = shipment.customs_info.contents_explanation;
                model.RestrictionComments = shipment.customs_info.restriction_comments;
                model.CustomsCertify = bool.TryParse(shipment.customs_info.customs_certify, out var customsCertify) && customsCertify;
                model.CustomsSigner = shipment.customs_info.customs_signer;
                model.EelPfc = shipment.customs_info.eel_pfc;
            }

            model.AvailableDeliveryConfirmations = await prepareSelectList(DeliveryConfirmation.None);
            model.AvailableEndorsements = await prepareSelectList(Endorsement.None);
            model.AvailableHazmatTypes = await prepareSelectList(HazmatType.None);
            model.AvailableCustomCodes = await prepareSelectList(CustomCode.None);
            model.AvailableSpecialRates = await prepareSelectList(SpecialRate.None);
            model.AvailableContentsTypes = await ContentsType.Other.ToSelectListAsync(false);
            model.AvailableRestrictionTypes = await RestrictionType.None.ToSelectListAsync(false);
            model.AvailableNonDeliveryOptions = await NonDeliveryOption.Return.ToSelectListAsync(false);

            return View("~/Plugins/Shipping.EasyPost/Views/Shipment/ShipmentDetails.cshtml", model);
        }

        #endregion
    }
}