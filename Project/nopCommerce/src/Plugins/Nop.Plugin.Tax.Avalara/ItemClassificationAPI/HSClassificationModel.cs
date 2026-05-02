using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nop.Plugin.Tax.Avalara.ItemClassificationAPI;

/// <summary>
/// Represents classification model
/// </summary>
public class HSClassificationModel : Response
{
    /// <summary>
    /// The id of the HS classification request. This id is created by CreateHSClassificationRequest and returned in the response
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// Item
    /// </summary>
    /// <remarks>
    /// Required
    /// </remarks>
    [JsonProperty(PropertyName = "item")]
    public ItemClassificationModel Item { get; set; }

    /// <summary>
    /// The required classification activity for the request. Accepted values are: HS_FULL : full HS code classification (country specific). 
    /// This is the default activity if no value is specified in the request. HS6: 6 digits HS code classification
    /// </summary>
    [JsonProperty(PropertyName = "activity")]
    [JsonConverter(typeof(StringEnumConverter))]
    public ActivityEnum Activity { get; set; }

    /// <summary>
    /// Country of destination (ISO code of country)
    /// </summary>
    /// <remarks>
    /// Required
    /// </remarks>
    [JsonProperty(PropertyName = "countryOfDestination")]
    public string CountryOfDestination { get; set; }

    /// <summary>
    /// This value is returned by get HS Classification method. It’ll be ignored if it’s provided in the request
    /// </summary>
    /// <remarks>
    /// Required
    /// </remarks>
    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    /// <summary>
    /// This is the HS classification result. It’ll be returned in the body for GetHSClassification method
    /// </summary>
    [JsonProperty(PropertyName = "hsCode")]
    public string HsCode { get; set; }

    /// <summary>
    /// This value will be returned by getHSClassification method only if an item cannot be classified (status=CANNOT_BE_CLASSIFIED). 
    /// It contains a detailed description of the issue. It’ll be ignored if it’s provided in the request.
    /// </summary>
    [JsonProperty(PropertyName = "resolution")]
    public string Resolution { get; set; }

    /// <summary>
    /// If the information provided in the request is not enough, an accurately classification might not be possible. 
    /// In this case, the assumption made for classification will be returned in this field
    /// </summary>
    [JsonProperty(PropertyName = "confidence")]
    public string Confidence { get; set; }

    /// <summary>
    /// This is a detailed description of the assumptions made in order to classify an item when information 
    /// provided in the request is not enough (e.g. similar product URL, product number or anything which can be used as identifier behind the classification logic)
    /// </summary>
    [JsonProperty(PropertyName = "confidenceDescription")]
    public string ConfidenceDescription { get; set; }
}