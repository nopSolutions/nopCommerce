using Nop.Plugin.Shipping.UPS.Services;
using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.UPS.API.Track;

public partial class TrackClient
{
    private UPSSettings _upsSettings;
    private string _accessToken;

    public TrackClient(HttpClient httpClient, UPSSettings upsSettings, string accessToken) : this(httpClient)
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

    /// <summary>
    /// Get tracking info
    /// </summary>
    /// <param name="trackingNumber">The tracking number</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the tracking info
    /// </returns>
    public virtual async Task<TrackResponse> TrackAsync(string trackingNumber)
    {
        var response = await TrackV1DetailsAsync(trackingNumber, Guid.NewGuid().ToString(),
            _upsSettings.UseSandbox ? "testing" : UPSDefaults.SystemName, string.Empty);

        return response.TrackResponse;
    }

    partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
    {
        settings.ContractResolver = new NullToEmptyStringResolver();
    }
}