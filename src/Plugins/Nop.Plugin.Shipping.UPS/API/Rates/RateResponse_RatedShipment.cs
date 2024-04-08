using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.UPS.API.Rates;

public partial class RateResponse_RatedShipment
{
    /// <summary>
    /// <remarks>
    /// For some reason, the description of this field in the API definition
    /// does not correspond to reality. More precisely, it does not always correspond to reality,
    /// sometimes the answer comes as a single object and sometimes as a collection of objects.
    /// since we do not use this data in our code, we decided to change type to object (It might be ICollection of <see cref="RatedShipment_RatedPackage" /> or <see cref="RatedShipment_RatedPackage" />).
    ///
    /// Do not delete this field unless you have made sure that the description of the API
    /// or the response from the server has changed</remarks>
    /// </summary>
    [JsonProperty("RatedPackage", Required = Required.Always)]
    [System.ComponentModel.DataAnnotations.Required]
    public object RatedPackage { get; set; }

    /// <summary>
    /// <remarks>
    /// For some reason, the description of this field in the API definition
    /// does not correspond to reality. More precisely, it does not always correspond to reality,
    /// sometimes the answer comes as a single object and sometimes as a collection of objects.
    /// since we do not use this data in our code, we decided to change type to object (It might be ICollection of <see cref="RatedShipment_RatedShipmentAlert" /> or <see cref="RatedShipment_RatedShipmentAlert" />).
    ///
    /// Do not delete this field unless you have made sure that the description of the API
    /// or the response from the server has changed</remarks>
    /// </summary>
    [JsonProperty("RatedShipmentAlert", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public object RatedShipmentAlert { get; set; }
}