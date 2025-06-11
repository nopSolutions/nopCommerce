using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.CourierGuy.Domain;

public class CourierGuyTrackingResponse
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    [JsonProperty("shipments")]
    public List<CourierGuyShipment> Shipments { get; set; }

    [JsonProperty("tracking_steps")]
    public List<TrackingStep> TrackingSteps { get; set; }
}

public class CourierGuyShipment
{
    [JsonProperty("provider_id")]
    public int ProviderId { get; set; }

    [JsonProperty("shipment_id")]
    public int ShipmentId { get; set; }

    [JsonProperty("short_tracking_reference")]
    public string ShortTrackingReference { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("shipment_time_created")]
    public DateTime ShipmentTimeCreated { get; set; }

    [JsonProperty("shipment_time_modified")]
    public DateTime ShipmentTimeModified { get; set; }

    [JsonProperty("shipment_collected_date")]
    public object ShipmentCollectedDate { get; set; }

    [JsonProperty("shipment_delivered_date")]
    public object ShipmentDeliveredDate { get; set; }

    [JsonProperty("shipment_estimated_collection")]
    public object ShipmentEstimatedCollection { get; set; }

    [JsonProperty("shipment_estimated_delivery_from")]
    public object ShipmentEstimatedDeliveryFrom { get; set; }

    [JsonProperty("shipment_estimated_delivery_to")]
    public object ShipmentEstimatedDeliveryTo { get; set; }

    [JsonProperty("collection_from")]
    public string CollectionFrom { get; set; }

    [JsonProperty("delivery_to")]
    public string DeliveryTo { get; set; }

    [JsonProperty("collection_hub")]
    public string CollectionHub { get; set; }

    [JsonProperty("delivery_hub")]
    public string DeliveryHub { get; set; }

    [JsonProperty("service_level_code")]
    public string ServiceLevelCode { get; set; }

    [JsonProperty("service_level_name")]
    public string ServiceLevelName { get; set; }

    [JsonProperty("tracking_events")]
    public List<TrackingEvent> TrackingEvents { get; set; }
}

public class TrackingEvent
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("parcel_id")]
    public int ParcelId { get; set; }

    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("source")]
    public string Source { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}

public class TrackingStep
{
    [JsonProperty("step_number")]
    public int StepNumber { get; set; }

    [JsonProperty("label")]
    public string Label { get; set; }

    [JsonProperty("progress")]
    public string Progress { get; set; }
}