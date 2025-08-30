using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Net.Http.Headers;
using Nop.Core.Configuration;
using Nop.Plugin.Payments.Momo.Models;

namespace Nop.Plugin.Payments.Momo.Services;

public class MomoPaymentClient
{
    private readonly HttpClient _httpClient;
    private readonly MomoPaymentSettings _settings;
    private TokenResponse _currentToken;
    private readonly SemaphoreSlim _tokenSemaphore = new SemaphoreSlim(1, 1);

    public MomoPaymentClient(HttpClient httpClient, MomoPaymentSettings settings)
    {
        _settings = settings;
        httpClient.BaseAddress = new Uri(MtnMomoDefaults.BASE_ADDRESS);
        httpClient.Timeout = TimeSpan.FromSeconds(20);
        httpClient.DefaultRequestHeaders.Add(MtnMomoDefaults.SUBSCRIPTION_HEADER, _settings.SubscriptionKey);

        _httpClient = httpClient;
    }

    private async Task EnsureValidTokenAsync()
    {
        // First quick check without locking
        if (_currentToken != null && !_currentToken.IsExpired)
            return;

        // If token is null or expired, acquire lock and check again
        await _tokenSemaphore.WaitAsync();
        try
        {
            // Double-check if token is still invalid after acquiring lock
            if (_currentToken != null && !_currentToken.IsExpired)
                return;

            // Get new token
            _currentToken = await GetNewTokenAsync();

            // Configure client with new token
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue(_currentToken.TokenType, _currentToken.AccessToken);
        }
        finally
        {
            _tokenSemaphore.Release();
        }
    }

    private async Task<TokenResponse> GetNewTokenAsync()
    {
        // Basic auth with API User and API Key
        var authString = $"{_settings.ApiUser}:{_settings.ApiKey}";
        var base64Auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));
        
        var request = new HttpRequestMessage(HttpMethod.Post, MtnMomoDefaults.TOKEN_ENDPOINT);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var token = JsonSerializer.Deserialize<TokenResponse>(content);

        if (token == null)
            throw new Exception("Failed to deserialize token response");

        return token;
    }

    public async Task<HttpResponseMessage> RequestToPayAsync(string phoneNumber, decimal amount, string currency = "EUR")
    {
        await EnsureValidTokenAsync();

        var payload = new
        {
            amount = amount.ToString(),
            currency = currency,
            externalId = Guid.NewGuid().ToString(),
            payer = new
            {
                partyIdType = "MSISDN",
                partyId = phoneNumber
            },
            payerMessage = "Payment for order",
            payeeNote = "Order payment"
        };

        var jsonContent = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var referenceId = Guid.NewGuid().ToString();
        _httpClient.DefaultRequestHeaders.Add("X-Reference-Id", referenceId);

        var response = await _httpClient.PostAsync(MtnMomoDefaults.PAYMENT_ENDPOINT, content);
        return response;
    }

    public async Task<HttpResponseMessage> GetPaymentStatusAsync(string referenceId)
    {
        await EnsureValidTokenAsync();
        return await _httpClient.GetAsync($"{MtnMomoDefaults.PAYMENT_ENDPOINT}/{referenceId}");
    }
}
