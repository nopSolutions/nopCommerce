using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nop.Plugin.Shipping.CourierGuy.Domain;

public class CourierGuyRateRequest
{
    [JsonProperty("collection_address")]
    public Address CollectionAddress { get; set; }

    [JsonProperty("delivery_address")]
    public Address DeliveryAddress { get; set; }

    [JsonProperty("parcels")]
    public List<Parcel> Parcels { get; set; }

    // Only include for insurance
    [JsonProperty("declared_value")]
    public decimal DeclaredValue { get; set; }

    [JsonProperty("collection_min_date")]
    public DateTime CollectionMinDate { get; set; }

    [JsonProperty("delivery_min_date")]
    public DateTime DeliveryMinDate { get; set; }

    public class Address
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonRequired]
        // ensure all lowercase
        public AddressType Type { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("street_address")]
        public string StreetAddress { get; set; }

        [JsonProperty("local_area")]
        public string LocalArea { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("zone")]
        public string Zone { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }

    public class Parcel
    {
        [JsonProperty("submitted_length_cm")]
        public decimal SubmittedLengthCm { get; set; }

        [JsonProperty("submitted_width_cm")]
        public decimal SubmittedWidthCm { get; set; }

        [JsonProperty("submitted_height_cm")]
        public decimal SubmittedHeightCm { get; set; }

        [JsonProperty("submitted_weight_kg")]
        public decimal SubmittedWeightKg { get; set; }
    }

    public CourierGuyRateRequest()
    {
        CollectionAddress = new Address();
        DeliveryAddress = new Address();
        Parcels = new List<Parcel>();
    }

    public enum AddressType
    {
        business,
        residential
    }
}