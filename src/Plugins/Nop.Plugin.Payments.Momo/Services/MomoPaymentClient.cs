using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Momo.Models;

namespace Nop.Plugin.Payments.Momo.Services
{
    /// <summary>
    /// MTN MoMo payment client implementation
    /// </summary>
    public class MomoPaymentClient
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly MomoPaymentSettings _settings;
        private TokenResponse _currentToken;
        private readonly SemaphoreSlim _tokenSemaphore = new SemaphoreSlim(1, 1);

        #endregion

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
            var token = JsonConvert.DeserializeObject<TokenResponse>(content);

            if (token == null)
                throw new Exception("Failed to deserialize token response");

            return token;
        }

        public async Task<HttpResponseMessage> RequestToPayAsync(string phoneNumber, decimal amount,
            string currency = "EUR")
        {
            await EnsureValidTokenAsync();

            var payload = new
            {
                amount = amount.ToString(),
                currency = currency,
                externalId = Guid.NewGuid().ToString(),
                payer = new { partyIdType = "MSISDN", partyId = phoneNumber },
                payerMessage = "Payment for order",
                payeeNote = "Order payment"
            };

            var jsonContent = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var referenceId = Guid.NewGuid().ToString();
            _httpClient.DefaultRequestHeaders.Add(MtnMomoDefaults.REFERENCE_ID, referenceId);
            _httpClient.DefaultRequestHeaders.Add(MtnMomoDefaults.TARGET_ENVIRONMENT, _settings.Environment);

            var response = await _httpClient.PostAsync(MtnMomoDefaults.PAYMENT_ENDPOINT, content);

            // Store reference ID for status checks
            if (response.IsSuccessStatusCode)
            {
                response.Headers.Add("X-Reference-Id", referenceId);
            }

            return response;
        }

        public async Task<PaymentStatusResponse> GetPaymentStatusAsync(string referenceId)
        {
            if (string.IsNullOrEmpty(referenceId))
                throw new ArgumentNullException(nameof(referenceId));

            await EnsureValidTokenAsync();

            try
            {
                _httpClient.DefaultRequestHeaders.Add(MtnMomoDefaults.TARGET_ENVIRONMENT, _settings.Environment);
                var response = await _httpClient.GetAsync($"{MtnMomoDefaults.PAYMENT_STATUS_ENDPOINT}/{referenceId}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var momoResponse = JsonConvert.DeserializeObject<MomoResponse>(content);

                    return new PaymentStatusResponse
                    {
                        Success = momoResponse.Status == "SUCCESSFUL",
                        Status = momoResponse.Status,
                        Message = momoResponse.Reason
                    };
                }

                return new PaymentStatusResponse
                {
                    Success = false, Status = "FAILED", Message = $"Failed to get status: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new PaymentStatusResponse { Success = false, Status = "ERROR", Message = ex.Message };
            }
        }
    }
}
