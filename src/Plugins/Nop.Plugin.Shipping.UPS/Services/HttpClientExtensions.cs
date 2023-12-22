using Microsoft.Net.Http.Headers;
using System.Text;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using static System.TimeSpan;

namespace Nop.Plugin.Shipping.UPS.Services;

public static class HttpClientExtensions
{
    public static void PrepareRequest(this HttpClient httpClient,
        HttpRequestMessage request, UPSSettings upsSettings, string accessToken = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(upsSettings);

        httpClient.Timeout = FromSeconds(upsSettings.RequestTimeout ?? UPSDefaults.RequestTimeout);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, UPSDefaults.UserAgent);

        if (!string.IsNullOrEmpty(accessToken))
            request.Headers.Add(HeaderNames.Authorization,$"Bearer {accessToken}");
        else
        {
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{upsSettings.ClientId}:{upsSettings.ClientSecret}"));
            request.Headers.Add(HeaderNames.Authorization, $"Basic {base64}");
        }

        //save debug info
        if (!upsSettings.Tracing) 
            return;

        var logger = EngineContext.Current.Resolve<ILogger>();
        logger.Information($"UPS rates. Request: {request}{Environment.NewLine}Content: {request.Content?.ReadAsStringAsync().Result}");
    }

    public static void ProcessResponse(this HttpClient httpClient, HttpResponseMessage response, UPSSettings upsSettings)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(upsSettings);

        //save debug info
        if (!upsSettings.Tracing)
            return;

        var logger = EngineContext.Current.Resolve<ILogger>();
        logger.Information($"UPS rates. Response: {response}{Environment.NewLine}Content: {response.Content.ReadAsStringAsync().Result}");
    }
}