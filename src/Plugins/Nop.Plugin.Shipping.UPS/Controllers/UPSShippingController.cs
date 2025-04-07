using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Shipping.UPS.Domain;
using Nop.Plugin.Shipping.UPS.Models;
using Nop.Plugin.Shipping.UPS.Services;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Shipping.UPS.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class UPSShippingController(ILocalizationService localizationService,
        IMeasureService measureService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        UPSService upsService,
        UPSSettings upsSettings) : BasePluginController
{
    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public async Task<IActionResult> Configure()
    {
        //prepare common model
        var model = new UPSShippingModel
        {
            AccountNumber = upsSettings.AccountNumber,
            ClientId = upsSettings.ClientId,
            ClientSecret = upsSettings.ClientSecret,
            UseSandbox = upsSettings.UseSandbox,
            AdditionalHandlingCharge = upsSettings.AdditionalHandlingCharge,
            InsurePackage = upsSettings.InsurePackage,
            CustomerClassification = (int)upsSettings.CustomerClassification,
            PickupType = (int)upsSettings.PickupType,
            PackagingType = (int)upsSettings.PackagingType,
            SaturdayDeliveryEnabled = upsSettings.SaturdayDeliveryEnabled,
            PassDimensions = upsSettings.PassDimensions,
            PackingPackageVolume = upsSettings.PackingPackageVolume,
            PackingType = (int)upsSettings.PackingType,
            Tracing = upsSettings.Tracing,
            WeightType = upsSettings.WeightType,
            DimensionsType = upsSettings.DimensionsType
        };

        //prepare offered delivery services
        var servicesCodes = upsSettings.CarrierServicesOffered?.Split(':', StringSplitOptions.RemoveEmptyEntries)
            .Select(idValue => idValue.Trim('[', ']')).ToList() ?? new List<string>();

        //prepare available options
        model.AvailableCustomerClassifications = (await CustomerClassification.DailyRates.ToSelectListAsync(false))
            .Select(item => new SelectListItem(item.Text, item.Value)).ToList();
        model.AvailablePickupTypes = (await PickupType.DailyPickup.ToSelectListAsync(false))
            .Select(item => new SelectListItem(item.Text, item.Value)).ToList();
        model.AvailablePackagingTypes = (await PackagingType.CustomerSuppliedPackage.ToSelectListAsync(false))
            .Select(item => new SelectListItem(item.Text?.TrimStart('_'), item.Value)).ToList();
        model.AvaliablePackingTypes = (await PackingType.PackByDimensions.ToSelectListAsync(false))
            .Select(item => new SelectListItem(item.Text, item.Value)).ToList();
        model.AvailableCarrierServices = (await DeliveryService.Standard.ToSelectListAsync(false))
            .Select(item =>
            {
                var serviceCode = upsService.GetUpsCode((DeliveryService)int.Parse(item.Value));
                return new SelectListItem($"UPS {item.Text?.TrimStart('_')}", serviceCode, servicesCodes.Contains(serviceCode));
            }).ToList();
        model.AvaliableWeightTypes = new List<SelectListItem> { new SelectListItem("LBS", "LBS"), new SelectListItem("KGS", "KGS") };
        model.AvaliableDimensionsTypes = new List<SelectListItem> { new SelectListItem("IN", "IN"), new SelectListItem("CM", "CM") };

        //check measures
        var weightSystemName = upsSettings.WeightType switch { "LBS" => "lb", "KGS" => "kg", _ => null };
        if (await measureService.GetMeasureWeightBySystemKeywordAsync(weightSystemName) == null)
            notificationService.ErrorNotification($"Could not load '{weightSystemName}' <a href=\"{Url.Action("List", "Measure")}\" target=\"_blank\">measure weight</a>", false);

        var dimensionSystemName = upsSettings.DimensionsType switch { "IN" => "inches", "CM" => "centimeters", _ => null };
        if (await measureService.GetMeasureDimensionBySystemKeywordAsync(dimensionSystemName) == null)
            notificationService.ErrorNotification($"Could not load '{dimensionSystemName}' <a href=\"{Url.Action("List", "Measure")}\" target=\"_blank\">measure dimension</a>", false);

        return View("~/Plugins/Shipping.UPS/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public async Task<IActionResult> Configure(UPSShippingModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        //save settings
        upsSettings.AccountNumber = model.AccountNumber;
        upsSettings.ClientId = model.ClientId;
        upsSettings.ClientSecret = model.ClientSecret;
        upsSettings.UseSandbox = model.UseSandbox;
        upsSettings.AdditionalHandlingCharge = model.AdditionalHandlingCharge;
        upsSettings.CustomerClassification = (CustomerClassification)model.CustomerClassification;
        upsSettings.PickupType = (PickupType)model.PickupType;
        upsSettings.PackagingType = (PackagingType)model.PackagingType;
        upsSettings.InsurePackage = model.InsurePackage;
        upsSettings.SaturdayDeliveryEnabled = model.SaturdayDeliveryEnabled;
        upsSettings.PassDimensions = model.PassDimensions;
        upsSettings.PackingPackageVolume = model.PackingPackageVolume;
        upsSettings.PackingType = (PackingType)model.PackingType;
        upsSettings.Tracing = model.Tracing;
        upsSettings.WeightType = model.WeightType;
        upsSettings.DimensionsType = model.DimensionsType;

        //use default services if no one is selected 
        if (!model.CarrierServices.Any())
        {
            model.CarrierServices = new List<string>
            {
                upsService.GetUpsCode(DeliveryService.Ground),
                upsService.GetUpsCode(DeliveryService.WorldwideExpedited),
                upsService.GetUpsCode(DeliveryService.Standard),
                upsService.GetUpsCode(DeliveryService._3DaySelect)
            };
        }
        upsSettings.CarrierServicesOffered = string.Join(':', model.CarrierServices.Select(service => $"[{service}]"));

        await settingService.SaveSettingAsync(upsSettings);

        notificationService.SuccessNotification(await localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    #endregion
}