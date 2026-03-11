using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Paystack.Models;
using Nop.Services.Logging;

namespace Nop.Plugin.Payments.Paystack.Services;

/// <summary>
/// Paystack API payment client
/// </summary>
public class PaystackPaymentClient
{
    private readonly HttpClient _httpClient;
    private readonly PaystackPaymentSettings _settings;
    private readonly ILogger _logger;

    public PaystackPaymentClient(HttpClient httpClient, PaystackPaymentSettings settings, ILogger logger)
    {
        _settings = settings;
        _logger = logger;
        httpClient.BaseAddress = new Uri(PaystackDefaults.BASE_ADDRESS);
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.SecretKey);
        _httpClient = httpClient;
    }

    /// <summary>
    /// Initialize a transaction and get the authorization URL for the customer
    /// </summary>
    /// <param name="email">Customer email</param>
    /// <param name="amount">Amount in main currency unit (e.g. 200.50 NGN); will be converted to subunit (kobo)</param>
    /// <param name="reference">Unique transaction reference (optional; generated if not provided)</param>
    /// <param name="callbackUrl">Override callback URL (optional)</param>
    /// <param name="metadata">Custom metadata (optional)</param>
    /// <returns>Initialize response with authorization_url and reference, or null on failure</returns>
    public Task<InitializeTransactionResponse> InitializeTransactionAsync(
        string email,
        decimal amount,
        string reference = null,
        string callbackUrl = null,
        object metadata = null)
    {
        return InitializeTransactionAsync(_settings, email, amount, callbackUrl ,reference, metadata);
    }

    /// <summary>
    /// Initialize a transaction using the given settings (e.g. for a specific store from the processor).
    /// </summary>
    private async Task<InitializeTransactionResponse> InitializeTransactionAsync(
        PaystackPaymentSettings settings,
        string email,
        decimal amount,
        string callbackUrl,
        string reference = null,
        object metadata = null)
    {
        if (string.IsNullOrWhiteSpace(settings?.SecretKey))
            return null;

        var amountInSubUnit = (int)Math.Round(amount * 100);
        var payload = new
        {
            email,
            amount = amountInSubUnit,
            reference = reference ?? Guid.NewGuid().ToString("N"),
            callback_url = callbackUrl,
            metadata
        };

        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, PaystackDefaults.INITIALIZE_TRANSACTION_ENDPOINT);
        var secretKey = (settings.SecretKey ?? "").Trim();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", secretKey);
        request.Content = content;

        var keyPreview = string.IsNullOrEmpty(secretKey) ? "(empty)" : secretKey.Length <= 12 ? secretKey : secretKey[..12] + "...";
        await _logger.InformationAsync($"[Paystack] InitializeTransaction: SecretKey preview={keyPreview}, Length={secretKey.Length}");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new InvalidOperationException(
                    "Paystack API returned 401. Please verify your Secret Key is configured correctly in Admin > Configuration > Payment methods > Paystack. Use your Secret Key (sk_test_... or sk_live_...), not the Public Key.");
            }
            return null;
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<InitializeTransactionResponse>(responseBody);
    }

    /// <summary>
    /// Verify a transaction using the given settings (e.g. for callback from a specific store).
    /// </summary>
    public async Task<VerifyTransactionResponse?> VerifyTransactionAsync(string reference)
    {
        if (string.IsNullOrWhiteSpace(_settings?.SecretKey) || string.IsNullOrWhiteSpace(reference))
            return null;

        var url = $"{PaystackDefaults.VERIFY_TRANSACTION_ENDPOINT}/{reference}";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.SecretKey);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<VerifyTransactionResponse>(responseBody);
    }
}
