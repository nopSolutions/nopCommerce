using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services;

/// <summary>
/// Represents the HTTP client to request onboarding services
/// </summary>
public class OnboardingHttpClient
{
    #region Fields

    protected readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public OnboardingHttpClient(HttpClient httpClient)
    {
        //configure client
        httpClient.BaseAddress = new Uri(PayPalCommerceDefaults.Onboarding.ServiceUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(20);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, PayPalCommerceDefaults.UserAgent);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);

        _httpClient = httpClient;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get the merchant details
    /// </summary>
    /// <param name="merchantGuid">Internal merchant id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the merchant details
    /// </returns>
    public async Task<Merchant> GetMerchantAsync(string merchantGuid)
    {
        var httpResponse = await _httpClient.GetAsync($"paypal/merchant/{Uri.EscapeDataString(merchantGuid)}");
        var responseString = await httpResponse.Content.ReadAsStringAsync();
        httpResponse.EnsureSuccessStatusCode();
        var result = new { Result = string.Empty, Data = new Merchant() };
        result = JsonConvert.DeserializeAnonymousType(responseString ?? string.Empty, result) ?? default;
        return result.Data;
    }

    #endregion
}