using Newtonsoft.Json;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Shipping.CourierGuy.Domain;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.CourierGuy;

public class CourierGuyShipmentTracker : IShipmentTracker
{
    private readonly ISettingService _settingService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private readonly CourierGuySettings _settings;
    private CourierGuyHttpClientFactory _shipLogicHttpClientFactory;

    public CourierGuyShipmentTracker(ISettingService settingService, IHttpClientFactory httpClientFactory, ILogger logger)
    {
        _settingService = settingService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _settings = _settingService.LoadSettingAsync<CourierGuySettings>().GetAwaiter().GetResult();
        _shipLogicHttpClientFactory = new CourierGuyHttpClientFactory(_httpClientFactory, _settingService);
    }

    public async Task<string> GetUrlAsync(string trackingNumber, Shipment shipment = null)
    {
        if (_settings.UseSandbox)
        {
            // https://sandbox.shiplogic.com/track?ref=9FBQFM
            return $"https://sandbox.shiplogic.com/track?ref={trackingNumber}";
        }

        return $"https://portal.thecourierguy.co.za/track?ref={trackingNumber}";
    }

    public async Task<IList<ShipmentStatusEvent>> GetShipmentEventsAsync(string trackingNumber, Shipment shipment = null)
    {
        // https://api.shiplogic.com/v2/tracking/
        var client = await _shipLogicHttpClientFactory.TrackingHttpClient();
        var response = await client.GetAsync($"shipments?tracking_reference={trackingNumber}");

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var headers = response.Headers;
            var callingUrl = response.RequestMessage?.RequestUri?.AbsoluteUri;

            var payload = new
            {
                TrackingNumber = trackingNumber,
                ResponseStatusCode = response.StatusCode,
                ResponseContent = responseContent,
                Headers = headers,
                CallingUrl = callingUrl
            };
            var warning = JsonConvert.SerializeObject(payload, Formatting.Indented);
            await _logger.WarningAsync(warning);
            await _logger.ErrorAsync($"CourierGuyShipmentTracker.GetShipmentEventsAsync: Failed to get shipment events for tracking number {trackingNumber}. Response: {response.StatusCode}");
            return new List<ShipmentStatusEvent>
            {
                new()
                {
                    Date = DateTime.UtcNow,
                    Location = "Cape Town",
                    Status = "In transit",
                    CountryCode = "ZA",
                    EventName = $"Order sent to courier for delivery track using url: {GetUrlAsync(trackingNumber).GetAwaiter().GetResult()}",
                },
            };
        }
/*
 *  "shipments": [
        {
            "provider_id": 10,
            "shipment_id": 55506034,
            "short_tracking_reference": "9FBQFM",
            "status": "collection-assigned",
            "shipment_time_created": "2024-11-26T20:37:26.176041Z",
            "shipment_time_modified": "2024-11-26T20:37:26.839676Z",
            "shipment_collected_date": null,
            "shipment_delivered_date": null,
            "shipment_estimated_collection": null,
            "shipment_estimated_delivery_from": null,
            "shipment_estimated_delivery_to": null,
            "collection_from": "Music Industries",
            "delivery_to": "JohnMark van Niekerk",
            "collection_hub": "Western Cape",
            "delivery_hub": "Western Cape",
            "service_level_code": "LOF",
            "service_level_name": "Economy",
            "tracking_events": [
                {
                    "id": 1319960641,
                    "parcel_id": 0,
                    "date": "2024-11-26T20:37:26.758754Z",
                    "status": "submitted",
                    "source": "shipping-293658c1f9f210-976f-4678-8e25-eae7d84622f1",
                    "message": ""
                }
            ]
        }
    ],
    "tracking_steps": [
        {
            "step_number": 1,
            "label": "created",
            "progress": "current"
        },
        {
            "step_number": 2,
            "label": "collected",
            "progress": "pending"
        },
        {
            "step_number": 3,
            "label": "in-transit",
            "progress": "pending"
        },
        {
            "step_number": 4,
            "label": "out-for-delivery",
            "progress": "pending"
        },
        {
            "step_number": 5,
            "label": "delivered",
            "progress": "pending"
        }
    ]
 */

        var shippingEventList = new List<ShipmentStatusEvent>();
        var responseContentJson = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<CourierGuyTrackingResponse>(responseContentJson);
        var trackingSteps = responseObject.TrackingSteps.OrderBy(x => x.StepNumber).ToArray();
        var courierGuyShipment = responseObject.Shipments.FirstOrDefault();

        var firstEvent = new ShipmentStatusEvent
        {
            Date = courierGuyShipment?.ShipmentTimeCreated,
            EventName = $"Order sent to courier for warehouse collection, track using url: {GetUrlAsync(trackingNumber).GetAwaiter().GetResult()}",
            Status = courierGuyShipment?.Status,
        };

        shippingEventList.Add(firstEvent);

        foreach (var trackingStep in trackingSteps)
        {
            var shippingEvent = new ShipmentStatusEvent
            {
                Date = courierGuyShipment?.ShipmentTimeCreated,
                Status = courierGuyShipment?.Status,
                CountryCode = "ZA",
                EventName = $"step: {trackingStep.StepNumber} | {trackingStep.Label} ({trackingStep.Progress})",
            };
            shippingEventList.Add(shippingEvent);
        }

        return shippingEventList;
    }
}