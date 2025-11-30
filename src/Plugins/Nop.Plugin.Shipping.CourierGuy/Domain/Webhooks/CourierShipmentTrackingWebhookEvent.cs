using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Shipping.CourierGuy.Domain.Webhooks;

public class CourierShipmentTrackingWebhookEvent
{
    [JsonProperty("provider_id")]
    public int ProviderId { get; set; }

    [JsonProperty("shipment_id")]
    public int ShipmentId { get; set; }

    [JsonProperty("short_tracking_reference")]
    public string ShortTrackingReference { get; set; }

    [JsonProperty("custom_tracking_reference")]
    public string CustomTrackingReference { get; set; }

    [JsonProperty("parcel_tracking_references")]
    public List<string> ParcelTrackingReferences { get; set; } = new();

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("shipment_time_created")]
    public DateTimeOffset ShipmentTimeCreated { get; set; }

    [JsonProperty("shipment_time_modified")]
    public DateTimeOffset ShipmentTimeModified { get; set; }

    [JsonProperty("shipment_collected_date")]
    public DateTimeOffset? ShipmentCollectedDate { get; set; }

    [JsonProperty("shipment_delivered_date")]
    public DateTimeOffset? ShipmentDeliveredDate { get; set; }

    [JsonProperty("shipment_estimated_collection")]
    public DateTimeOffset ShipmentEstimatedCollection { get; set; }

    [JsonProperty("shipment_estimated_delivery_from")]
    public DateTimeOffset ShipmentEstimatedDeliveryFrom { get; set; }

    [JsonProperty("shipment_estimated_delivery_to")]
    public DateTimeOffset ShipmentEstimatedDeliveryTo { get; set; }

    [JsonProperty("collection_hub")]
    public string CollectionHub { get; set; }

    [JsonProperty("delivery_hub")]
    public string DeliveryHub { get; set; }

    [JsonProperty("collection_lat")]
    public double CollectionLat { get; set; }

    [JsonProperty("collection_lng")]
    public double CollectionLng { get; set; }

    [JsonProperty("delivery_lat")]
    public double DeliveryLat { get; set; }

    [JsonProperty("delivery_lng")]
    public double DeliveryLng { get; set; }

    [JsonProperty("service_level_code")]
    public string ServiceLevelCode { get; set; }

    [JsonProperty("event_time")]
    public DateTimeOffset EventTime { get; set; }

    [JsonProperty("update_type")]
    public string UpdateType { get; set; }

    [JsonProperty("tracking_events")]
    public List<TrackingEvent> TrackingEvents { get; set; } = new();
}

public class TrackingEvent
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("parcel_id")]
    public int ParcelId { get; set; }

    [JsonProperty("date")]
    public DateTimeOffset Date { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("source")]
    public string Source { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("location")]
    public string Location { get; set; }
}