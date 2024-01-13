using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.FacebookPixel.Domain;

public class ConversionsEventUserData
{
    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    [JsonIgnore]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the client ip address
    /// </summary>
    [JsonProperty(PropertyName = "client_ip_address")]
    public string ClientIpAddress { get; set; }

    /// <summary>
    /// Gets or sets the client user agent
    /// </summary>
    [JsonProperty(PropertyName = "client_user_agent")]
    public string ClientUserAgent { get; set; }

    /// <summary>
    /// Gets or sets the hashed email address
    /// </summary>
    [JsonProperty(PropertyName = "em")]
    public List<string> EmailAddress { get; set; }

    /// <summary>
    /// Gets or sets the hashed phone number
    /// </summary>
    [JsonProperty(PropertyName = "ph")]
    public List<string> PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the hashed first name
    /// </summary>
    [JsonProperty(PropertyName = "fn")]
    public List<string> FirstName { get; set; }

    /// <summary>
    /// Gets or sets the hashed last name
    /// </summary>
    [JsonProperty(PropertyName = "ln")]
    public List<string> LastName { get; set; }

    /// <summary>
    /// Gets or sets the hashed date of birth
    /// </summary>
    [JsonProperty(PropertyName = "db")]
    public List<string> DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the hashed gender
    /// </summary>
    [JsonProperty(PropertyName = "ge")]
    public List<string> Gender { get; set; }

    /// <summary>
    /// Gets or sets the hashed city
    /// </summary>
    [JsonProperty(PropertyName = "ct")]
    public List<string> City { get; set; }

    /// <summary>
    /// Gets or sets the hashed state
    /// </summary>
    [JsonProperty(PropertyName = "st")]
    public List<string> State { get; set; }

    /// <summary>
    /// Gets or sets the hashed zip code
    /// </summary>
    [JsonProperty(PropertyName = "zp")]
    public List<string> Zip { get; set; }

    /// <summary>
    /// Gets or sets the hashed country
    /// </summary>
    [JsonProperty(PropertyName = "country")]
    public List<string> Country { get; set; }

    /// <summary>
    /// Gets or sets the hashed external id
    /// </summary>
    [JsonProperty(PropertyName = "external_id")]
    public List<string> ExternalId { get; set; }
}