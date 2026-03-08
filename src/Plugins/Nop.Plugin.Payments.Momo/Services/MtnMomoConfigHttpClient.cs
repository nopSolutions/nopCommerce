using Microsoft.Net.Http.Headers;
using Nop.Plugin.Payments.Momo;

public class MtnMomoConfigHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly MomoPaymentSettings _settings;

    public MtnMomoConfigHttpClient(HttpClient httpClient, MomoPaymentSettings settings)
    {
        _settings = settings;
        httpClient.BaseAddress = new Uri(MtnMomoDefaults.BASE_ADDRESS);
        httpClient.Timeout = TimeSpan.FromSeconds(20);
        httpClient.DefaultRequestHeaders.Add(MtnMomoDefaults.SUBSCRIPTION_HEADER, _settings.SubscriptionKey);

        _httpClient = httpClient;
    }

    public Task<HttpResponseMessage> CreateUserAsync(MomoPaymentSettings momoSettings)
    {
        _httpClient.DefaultRequestHeaders.Add(MtnMomoDefaults.REFERENCE_ID, momoSettings.ApiUser);
        return _httpClient.PostAsync(MtnMomoDefaults.CREATE_API_USER_ENDPOINT, 
            new StringContent($"{{\"providerCallbackHost\":\"{_settings.CallbackUrl}\"}}"));
    }

    public Task<HttpResponseMessage> GenerateApiKeyAsync()
    {
        var endpoint = string.Format(MtnMomoDefaults.GENERATE_API_KEY_ENDPOINT, _settings.ApiUser);
        return _httpClient.PostAsync(endpoint, null);
    }

    public Task<HttpResponseMessage> GetUserAsync()
    {
        var endpoint = string.Format(MtnMomoDefaults.GET_API_USER_ENDPOINT, _settings.ApiUser);
        return _httpClient.GetAsync(endpoint);
    }
}