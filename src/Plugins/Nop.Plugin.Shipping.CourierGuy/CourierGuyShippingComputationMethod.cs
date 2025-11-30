using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Plugin.Shipping.CourierGuy.Domain;
using Nop.Plugin.Shipping.CourierGuy.Domain.NopEntityMappers;
using Nop.Plugin.Shipping.CourierGuy.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.CourierGuy;

public class CourierGuyShippingComputationMethod : BasePlugin, IShippingRateComputationMethod
{
    private const string CONTROLLER_NAME = "ShippingCourierGuy";
    private const string ACTION_NAME = "Configure";

    private const string CONFIGURATION_PAGE_URL_TEMPLATE = "{0}Admin/{1}/{2}";

    private readonly IWebHelper _webHelper;
    private readonly ISettingService _settingService;
    private readonly ILocalizationService _localizationService;
    private readonly ICourierGuyNopEntityMapper _courierGuyNopEntityMapper;
    private CourierGuyHttpClientFactory _shipLogicHttpClientFactory;
    private readonly ILogger _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public CourierGuyShippingComputationMethod(
        IWebHelper webHelper,
        ISettingService settingService,
        ILocalizationService localizationService,
        IHttpClientFactory httpClientFactory,
        ICourierGuyNopEntityMapper courierGuyNopEntityMapper, ILogger logger)
    {
        _webHelper = webHelper;
        _settingService = settingService;
        _localizationService = localizationService;
        _httpClientFactory = httpClientFactory;
        _courierGuyNopEntityMapper = courierGuyNopEntityMapper;
        _logger = logger;
        // https://api-docs.bob.co.za/shiplogic
        _shipLogicHttpClientFactory = new CourierGuyHttpClientFactory(httpClientFactory, _settingService);
    }

    public override string GetConfigurationPageUrl()
    {
        return string.Format(CONFIGURATION_PAGE_URL_TEMPLATE, _webHelper.GetStoreLocation(), CONTROLLER_NAME,
            ACTION_NAME);
    }

    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new CourierGuySettings()
        {
            UseSandbox = true,
            BaseUrl = new Uri("https://api.shiplogic.com/v2/"),
            ApiKey = "",
            RateRequestUri = new Uri("https://api.shiplogic.com/v2/rates/"),
            ShipmentRequestUri = new Uri("https://api.shiplogic.com/v2/shipments/"),
            TrackingUri = new Uri("https://api.shiplogic.com/v2/tracking/"),
            SandBoxApiKey = "a601d99c75fc4c64b5a64288f97d52b4",
        });

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Shipping.CourierGuy.Fields.UseSandbox"] = "Use Sandbox",
            ["Plugins.Shipping.CourierGuy.Fields.UseSandbox.Hint"] =
                "Check this to use the sandbox while in development and testing",
            ["Plugins.Shipping.CourierGuy.Fields.BaseUrl"] = "Base Url",
            ["Plugins.Shipping.CourierGuy.Fields.BaseUrl.Hint"] =
                "The Base URL of the Ship Logic API (You should not need to change this, but you can if you have an API override",
            ["Plugins.Shipping.CourierGuy.Fields.ApiKey"] = "Api Key",
            ["Plugins.Shipping.CourierGuy.Fields.ApiKey.Hint"] =
                "The API Key for the Ship Logic API, passed as a bearer token",
            ["Plugins.Shipping.CourierGuy.Fields.SandBoxApiKey"] = "SandBox Api Key",
            ["Plugins.Shipping.CourierGuy.Fields.SandBoxApiKey.Hint"] =
                "The API Key for your Ship Logic Sandbox API, passed as a bearer token",
            ["Plugins.Shipping.CourierGuy.Fields.TrackingUri"] = "Tracking Uri",
            ["Plugins.Shipping.CourierGuy.Fields.TrackingUri.Hint"] =
                "The Tracking endpoint URI, appended to the base URL",
            ["Plugins.Shipping.CourierGuy.Fields.RateRequestUri"] = "Rate Request Uri",
            ["Plugins.Shipping.CourierGuy.Fields.RateRequestUri.Hint"] =
                "The Rate Request endpoint URI, appended to the base URL",
            ["Plugins.Shipping.CourierGuy.Fields.ShipmentRequestUri"] = "Shipment Request Uri",
            ["Plugins.Shipping.CourierGuy.Fields.ShipmentRequestUri.Hint"] =
                "The Shipment Request endpoint URI, appended to the base URL",
            ["Plugins.Shipping.CourierGuy.Instructions"] = "Courier Guy Ship Logic API Configuration Documentation Can be found at https://api-docs.bob.co.za/shiplogic",
        });
        await base.InstallAsync();
    }

    public async Task<GetShippingOptionResponse> GetShippingOptionsAsync(
        GetShippingOptionRequest getShippingOptionRequest)
    {
        try
        {
            var courierGuyRateRequest =
                await _courierGuyNopEntityMapper.NopCourierGuyRateRequest(getShippingOptionRequest);
            var rateResponses = new List<CourierGuyRateResponse>();
            foreach (var warehouseCollection in courierGuyRateRequest)
            {
                using var requestMessage = new HttpRequestMessage();
                using var content = new StringContent(JsonConvert.SerializeObject(warehouseCollection), Encoding.UTF8,
                    "application/json");
                requestMessage.Content = content;
                requestMessage.Method = HttpMethod.Post;
                using var client = await _shipLogicHttpClientFactory.RateRequestHttpClient();
                using var response = await client.SendAsync(requestMessage);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    await _logger.InformationAsync($"Courier Guy Rate Request Failed: {response.ReasonPhrase}");
                    await _logger.ErrorAsync(responseContent);
                    continue;
                }
                var responseContentJson = await response.Content.ReadAsStringAsync();
                rateResponses.Add(JsonConvert.DeserializeObject<CourierGuyRateResponse>(responseContentJson));
                return await _courierGuyNopEntityMapper.MapToShippingOptionResponse(rateResponses);
            }

            return new GetShippingOptionResponse()
            {
                ShippingOptions = new List<ShippingOption>(),
                ShippingFromMultipleLocations = false,
            };
        }
        catch (Exception e)
        {
            await _logger.ErrorAsync(e.Message, e);
            throw;
        }
    }

    public async Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest)
    {
        try
        {
            var shippingOptions = await GetShippingOptionsAsync(getShippingOptionRequest);
            // Fixed Rate is Based off of the Shipping Rate Computation Method System Name
            if (!shippingOptions.ShippingOptions.Any())
            {
                return null;
            }

            var economyByLocalized =
                await _localizationService.GetResourceAsync(
                    "Plugins.Shipping.CourierGuy.ShippingComputationMethod.ECO.Description");
            return shippingOptions
                .ShippingOptions
                .Single(x => x.Name.Equals(economyByLocalized, StringComparison.OrdinalIgnoreCase))
                .Rate;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IShipmentTracker> GetShipmentTrackerAsync()
    {
        return new CourierGuyShipmentTracker(_settingService, _httpClientFactory, _logger);
    }
}