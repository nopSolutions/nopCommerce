using Nop.Plugin.Shipping.UPS.Services;

namespace Nop.Plugin.Shipping.UPS.API.OAuth;

public partial class OAuthClient
{
    private UPSSettings _upsSettings;

    public OAuthClient(HttpClient httpClient, UPSSettings upsSettings) : this(httpClient)
    {
        _upsSettings = upsSettings;

        if (!_upsSettings.UseSandbox)
            BaseUrl = UPSDefaults.ApiUrl.Replace("/api", "");
    }

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request,
        string url)
    {
        client.PrepareRequest(request, _upsSettings);
    }

    public virtual Task<GenerateTokenSuccessResponse> GenerateTokenAsync()
    {
        return GenerateTokenAsync(null, new Body(), CancellationToken.None);
    }

    partial void ProcessResponse(HttpClient client, HttpResponseMessage response)
    {
        client.ProcessResponse(response, _upsSettings);
    }
}