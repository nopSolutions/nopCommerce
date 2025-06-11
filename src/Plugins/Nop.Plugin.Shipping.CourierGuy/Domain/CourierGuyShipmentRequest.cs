using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.CourierGuy.Domain;

public class CourierGuyShipmentRequest
{
    [JsonProperty("collection_address")]
    public CollectionAddress CollectionAddress { get; set; }

    [JsonProperty("collection_contact")]
    public CollectionContact CollectionContact { get; set; }

    [JsonProperty("delivery_address")]
    public DeliveryAddress DeliveryAddress { get; set; }

    [JsonProperty("delivery_contact")]
    public DeliveryContact DeliveryContact { get; set; }

    [JsonProperty("parcels")]
    public List<Parcel> Parcels { get; set; }

    [JsonProperty("special_instructions_collection")]
    public string SpecialInstructionsCollection { get; set; }

    [JsonProperty("special_instructions_delivery")]
    public string SpecialInstructionsDelivery { get; set; }

    [JsonProperty("declared_value")]
    public int? DeclaredValue { get; set; }

    [JsonProperty("collection_min_date")]
    public DateTime? CollectionMinDate { get; set; }

    [JsonProperty("collection_after")]
    public string CollectionAfter { get; set; }

    [JsonProperty("collection_before")]
    public string CollectionBefore { get; set; }

    [JsonProperty("delivery_min_date")]
    public DateTime? DeliveryMinDate { get; set; }

    [JsonProperty("delivery_after")]
    public string DeliveryAfter { get; set; }

    [JsonProperty("delivery_before")]
    public string DeliveryBefore { get; set; }

    [JsonProperty("custom_tracking_reference")]
    public string CustomTrackingReference { get; set; }

    [JsonProperty("customer_reference")]
    public string CustomerReference { get; set; }

    [JsonProperty("service_level_code")]
    public string ServiceLevelCode { get; set; }

    [JsonProperty("mute_notifications")]
    public bool? MuteNotifications { get; set; }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public partial class CollectionAddress
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("company")]
    public string Company { get; set; }

    [JsonProperty("street_address")]
    public string StreetAddress { get; set; }

    [JsonProperty("local_area")]
    public string LocalArea { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("zone")]
    public string Zone { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }

    [JsonProperty("lat")]
    public double? Lat { get; set; }

    [JsonProperty("lng")]
    public double? Lng { get; set; }
}

public partial class CollectionContact
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("mobile_number")]
    public string MobileNumber { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }
}

public partial class DeliveryAddress
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("company")]
    public string Company { get; set; }

    [JsonProperty("street_address")]
    public string StreetAddress { get; set; }

    [JsonProperty("local_area")]
    public string LocalArea { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("zone")]
    public string Zone { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }

    [JsonProperty("lat")]
    public double? Lat { get; set; }

    [JsonProperty("lng")]
    public double? Lng { get; set; }
}

public partial class DeliveryContact
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("mobile_number")]
    public string MobileNumber { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }
}

public partial class Parcel
{
    [JsonProperty("parcel_description")]
    public string ParcelDescription { get; set; }

    [JsonProperty("submitted_length_cm")]
    public int? SubmittedLengthCm { get; set; }

    [JsonProperty("submitted_width_cm")]
    public int? SubmittedWidthCm { get; set; }

    [JsonProperty("submitted_height_cm")]
    public int? SubmittedHeightCm { get; set; }

    [JsonProperty("submitted_weight_kg")]
    public int? SubmittedWeightKg { get; set; }
}