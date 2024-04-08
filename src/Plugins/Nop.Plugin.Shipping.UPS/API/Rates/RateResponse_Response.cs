using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.UPS.API.Rates;

public partial class RateResponse_Response
{
    /// <summary>
    /// <remarks>
    /// For some reason, the description of this field in the API definition
    /// does not correspond to reality. More precisely, it does not always correspond to reality,
    /// sometimes the answer comes as a single object and sometimes as a collection of objects.
    /// since we do not use this data in our code, we decided to change type to object (It might be ICollection of <see cref="Response_Alert" /> or <see cref="Response_Alert" />).
    ///
    /// Do not delete this field unless you have made sure that the description of the API
    /// or the response from the server has changed</remarks>
    /// </summary>
    [JsonProperty("Alert", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public object Alert { get; set; }

    /// <summary>
    /// <remarks>
    /// For some reason, the description of this field in the API definition
    /// does not correspond to reality. More precisely, it does not always correspond to reality,
    /// sometimes the answer comes as a single object and sometimes as a collection of objects.
    /// since we do not use this data in our code, we decided to change type to object (It might be ICollection of <see cref="Response_AlertDetail" /> or <see cref="Response_AlertDetail" />).
    ///
    /// Do not delete this field unless you have made sure that the description of the API
    /// or the response from the server has changed</remarks>
    /// </summary>
    [JsonProperty("AlertDetail", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public object AlertDetail { get; set; }

}