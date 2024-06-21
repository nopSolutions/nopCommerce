using Newtonsoft.Json;
using Nop.Plugin.Shipping.UPS.Services;

namespace Nop.Plugin.Shipping.UPS.API.Rates;

public partial class RateClient
{
    private readonly UPSSettings _upsSettings;
    private readonly string _accessToken;

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
            _upsSettings.UseSandbox ? "testing" : UPSDefaults.SystemName, "timeintransit", "v2403", "Shop", new RATERequestWrapper
            {
                RateRequest = request
            });

        return response.RateResponse;
    }

    static partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
    {
        settings.ContractResolver = new NullToEmptyStringResolver();
    }
}