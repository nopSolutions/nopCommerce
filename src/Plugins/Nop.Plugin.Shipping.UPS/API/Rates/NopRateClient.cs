using Newtonsoft.Json;
using Nop.Plugin.Shipping.UPS.Services;

namespace Nop.Plugin.Shipping.UPS.API.Rates;

public partial class RateClient
{
    private UPSSettings _upsSettings;
    private string _accessToken;

    public RateClient(HttpClient httpClient, UPSSettings upsSettings, string accessToken) : this(httpClient)
    {
        _upsSettings = upsSettings;
        _accessToken = accessToken;

        if (!_upsSettings.UseSandbox)
            BaseUrl = UPSDefaults.ApiUrl;
    }

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request,
        string url)
    {
        client.PrepareRequest(request, _upsSettings, _accessToken);
    }

    public async Task<RateResponse> ProcessRateAsync(RateRequest request)
    {
        var response = await RateAsync(Guid.NewGuid().ToString(),
            _upsSettings.UseSandbox ? "testing" : UPSDefaults.SystemName, string.Empty, "v1", "Shop", new RATERequestWrapper
            {
                RateRequest = request
            });

        return response.RateResponse;
    }

    partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
    {
        settings.ContractResolver = new NullToEmptyStringResolver();
    }
}

public partial class RateResponse_RatedShipment
{
    /// <summary>
    /// <remarks>
    /// For some reason, the description of this field in the API definition
    /// does not correspond to reality. More precisely, it does not always correspond to reality,
    /// sometimes the answer comes as a single object and sometimes as a collection of objects.
    /// since we do not use this data in our code, we decided to change type to object (It might be ICollection<RatedShipment_RatedPackage> or RatedShipment_RatedPackage).
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
    /// since we do not use this data in our code, we decided to change type to object (It might be ICollection<RatedShipment_RatedShipmentAlert> or RatedShipment_RatedShipmentAlert).
    ///
    /// Do not delete this field unless you have made sure that the description of the API
    /// or the response from the server has changed</remarks>
    /// </summary>
    [JsonProperty("RatedShipmentAlert", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public object RatedShipmentAlert { get; set; }
}