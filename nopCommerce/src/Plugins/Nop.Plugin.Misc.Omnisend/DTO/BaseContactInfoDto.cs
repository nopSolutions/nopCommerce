using Newtonsoft.Json;

namespace Nop.Plugin.Misc.Omnisend.DTO;

public class BaseContactInfoDto
{
    [JsonProperty("firstName")] public string FirstName { get; set; }
    [JsonProperty("lastName")] public string LastName { get; set; }
    [JsonProperty("tags")] public List<string> Tags => OmnisendDefaults.ContactTags;
    [JsonProperty("country")] public string Country { get; set; }
    [JsonProperty("countryCode")] public string CountryCode { get; set; }
    [JsonProperty("state")] public string State { get; set; }
    [JsonProperty("city")] public string City { get; set; }
    [JsonProperty("address")] public string Address { get; set; }
    [JsonProperty("postalCode")] public string PostalCode { get; set; }
    [JsonProperty("gender")] public string Gender { get; set; }
    [JsonProperty("birthdate")] public string BirthDate { get; set; }
}