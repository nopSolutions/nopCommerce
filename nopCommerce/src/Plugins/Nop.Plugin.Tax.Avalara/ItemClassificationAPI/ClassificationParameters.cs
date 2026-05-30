using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nop.Plugin.Tax.Avalara.ItemClassificationAPI;

public class ClassificationParameters
{
    /// <summary>
    /// The system has predefined list of parameter names that are known and processed accordingly
    /// </summary>
    /// <remarks>
    /// Required
    /// </remarks>
    [JsonProperty(PropertyName = "name")]
    [JsonConverter(typeof(StringEnumConverter))]
    public NameEnum Name { get; set; }

    /// <summary>
    /// The parameter’s value
    /// </summary>
    /// <remarks>
    /// Required
    /// </remarks>
    [JsonProperty(PropertyName = "value")]
    public string Value { get; set; }

    /// <summary>
    /// The unit of measurement code for the parameter. For price parameter the unit is required and must be currency code alpha ISO 4217. The only currency currently supported is “USD”
    /// </summary>
    [JsonProperty(PropertyName = "unit")]
    public string Unit { get; set; }
}