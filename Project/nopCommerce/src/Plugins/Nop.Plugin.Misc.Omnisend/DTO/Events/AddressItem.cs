using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public class AddressItem
{
    [JsonProperty("address1")] public string Address1 { get; set; }
    [JsonProperty("address2")] public string Address2 { get; set; }
    [JsonProperty("city")] public string City { get; set; }
    [JsonProperty("company")] public string Company { get; set; }
    [JsonProperty("country")] public string Country { get; set; }
    [JsonProperty("countryCode")] public string CountryCode { get; set; }
    [JsonProperty("firstName")] public string FirstName { get; set; }
    [JsonProperty("lastName")] public string LastName { get; set; }
    [JsonProperty("phone")] public string Phone { get; set; }
    [JsonProperty("state")] public string State { get; set; }
    [JsonProperty("stateCode")] public string StateCode { get; set; }
    [JsonProperty("zip")] public string Zip { get; set; }
}