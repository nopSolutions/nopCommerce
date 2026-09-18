using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Authentication;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Onboarding;
using PaypalServerSdk.Standard.Exceptions;
using SdkModels = PaypalServerSdk.Standard.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services;

/// <summary>
/// Represents the HTTP client to request PayPal API.
/// Uses the official PayPal Server SDK for Orders, Payments, Vault, and Tracking operations.
/// Falls back to direct HTTP for webhooks, identity tokens, and onboarding.
/// </summary>
public class PayPalCommerceHttpClient
{
    #region Fields

    private readonly HttpClient _httpClient;
    private readonly PayPalSdkClientFactory _sdkClientFactory;

    private static Dictionary<string, AccessToken> _accessTokens = new();

    private static readonly JsonSerializerSettings _jsonSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    #endregion

    #region Ctor

    public PayPalCommerceHttpClient(HttpClient httpClient, PayPalSdkClientFactory sdkClientFactory)
    {
        _httpClient = httpClient;
        _sdkClientFactory = sdkClientFactory;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get access token
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the access token
    /// </returns>
    private async Task<string> GetAccessTokenAsync(PayPalCommerceSettings settings)
    {
        if (!PayPalCommerceServiceManager.IsConfigured(settings))
            throw new NopException("Plugin is not configured");

        //no need to request a token if there is already a cached one and it has not expired (lifetime is about 9 hours)
        if (!_accessTokens.TryGetValue(settings.ClientId, out var accessToken) ||
            string.IsNullOrEmpty(accessToken?.Token) ||
            accessToken.IsExpired)
        {
            //get new access token
            accessToken = await RequestViaHttpAsync<GetAccessTokenRequest, GetAccessTokenResponse>(new()
            {
                ClientId = settings.ClientId,
                Secret = settings.SecretKey,
                GrantType = "client_credentials"
            }, settings);
            _accessTokens[settings.ClientId] = accessToken;
        }

        return accessToken.Token;
    }

    /// <summary>
    /// Convert SDK ApiException to NopException with detailed error information
    /// </summary>
    private static NopException ConvertSdkException(ApiException sdkException)
    {
        var error = $"Failed request ({sdkException.ResponseCode})";

        if (sdkException is ErrorException errorEx)
        {
            if (!string.IsNullOrEmpty(errorEx.Name))
            {
                error += $": {(!string.IsNullOrEmpty(errorEx.Message) ? errorEx.Message : errorEx.Name)}";
                if (errorEx.Details?.Any() == true)
                {
                    var details = string.Join(Environment.NewLine,
                        errorEx.Details.Select(d => $"  [{d.Field}] {d.Issue}: {d.Description}"));
                    error += $"{Environment.NewLine}{details}";
                }
            }
        }
        else
        {
            error += $": {sdkException.Message}";
        }

        return new NopException("Failed request", new NopException(error));
    }

    /// <summary>
    /// Serialize an SDK response model to JSON, then deserialize to an internal model type.
    /// Both model sets use Newtonsoft.Json and map to the same PayPal REST API JSON schema.
    /// </summary>
    private static TResponse ConvertSdkResponse<TResponse>(object sdkResponse) where TResponse : IApiResponse
    {
        if (typeof(TResponse) == typeof(EmptyResponse))
            return default;

        var json = JsonConvert.SerializeObject(sdkResponse, _jsonSettings);
        return JsonConvert.DeserializeObject<TResponse>(json) ?? default;
    }

    #region SDK-routed operations

    /// <summary>
    /// Try to handle the request via the PayPal Server SDK.
    /// Returns (true, response) if handled, (false, default) if the request type is not supported by the SDK.
    /// </summary>
    private async Task<(bool Handled, TResponse Response)> TryHandleViaSdkAsync<TRequest, TResponse>(
        TRequest request, PayPalCommerceSettings settings)
        where TRequest : IApiRequest where TResponse : IApiResponse
    {
        try
        {
            // Orders - Create
            if (request is Api.Orders.CreateOrderRequest createOrderReq)
            {
                var client = _sdkClientFactory.GetClient(settings);
                var json = JsonConvert.SerializeObject(createOrderReq, _jsonSettings);
                var sdkBody = JsonConvert.DeserializeObject<SdkModels.OrderRequest>(json);

                var input = new SdkModels.CreateOrderInput
                {
                    Body = sdkBody,
                    Prefer = "return=representation",
                    PaypalRequestId = Guid.NewGuid().ToString(),
                    PaypalPartnerAttributionId = PayPalCommerceDefaults.PartnerHeader.Value
                };

                var result = await client.OrdersController.CreateOrderAsync(input);
                return (true, ConvertSdkResponse<TResponse>(result.Data));
            }

            // Orders - Get
            if (request is Api.Orders.GetOrderRequest getOrderReq)
            {
                var client = _sdkClientFactory.GetClient(settings);
                var input = new SdkModels.GetOrderInput
                {
                    Id = getOrderReq.OrderId
                };

                var result = await client.OrdersController.GetOrderAsync(input);
                return (true, ConvertSdkResponse<TResponse>(result.Data));
            }

            // Orders - Patch/Update
            if (request.GetType().IsGenericType &&
                request.GetType().GetGenericTypeDefinition() == typeof(Api.Orders.UpdateOrderRequest<>))
            {
                var client = _sdkClientFactory.GetClient(settings);
                var orderIdProp = request.GetType().GetProperty("OrderId");
                var orderId = orderIdProp?.GetValue(request)?.ToString();

                // Serialize internal patches to JSON, then deserialize to SDK Patch list
                var json = JsonConvert.SerializeObject(request, _jsonSettings);
                var sdkPatches = JsonConvert.DeserializeObject<List<SdkModels.Patch>>(json);

                var input = new SdkModels.PatchOrderInput
                {
                    Id = orderId,
                    Body = sdkPatches
                };

                await client.OrdersController.PatchOrderAsync(input);
                return (true, default);
            }

            // Orders - Authorize
            if (request is Api.Orders.CreateAuthorizationRequest authReq)
            {
                var client = _sdkClientFactory.GetClient(settings);
                var input = new SdkModels.AuthorizeOrderInput
                {
                    Id = authReq.OrderId,
                    Prefer = "return=representation",
                    PaypalRequestId = Guid.NewGuid().ToString()
                };

                var result = await client.OrdersController.AuthorizeOrderAsync(input);
                return (true, ConvertSdkResponse<TResponse>(result.Data));
            }

            // Orders - Capture (capture an approved order)
            if (request is Api.Orders.CreateCaptureRequest captureOrderReq)
            {
                var client = _sdkClientFactory.GetClient(settings);
                var input = new SdkModels.CaptureOrderInput
                {
                    Id = captureOrderReq.OrderId,
                    Prefer = "return=representation",
                    PaypalRequestId = Guid.NewGuid().ToString()
                };

                var result = await client.OrdersController.CaptureOrderAsync(input);
                return (true, ConvertSdkResponse<TResponse>(result.Data));
            }

            // Orders - Tracking
            if (request is Api.Orders.CreateTrackingRequest trackingReq)
            {
                var client = _sdkClientFactory.GetClient(settings);

                var sdkItems = trackingReq.Items?.Select(item =>
                {
                    var itemJson = JsonConvert.SerializeObject(item, _jsonSettings);
                    return JsonConvert.DeserializeObject<SdkModels.OrderTrackerItem>(itemJson);
                }).ToList();

                var input = new SdkModels.CreateOrderTrackingInput
                {
                    Id = trackingReq.OrderId,
                    Body = new SdkModels.OrderTrackerRequest
                    {
                        CaptureId = trackingReq.CaptureId,
                        TrackingNumber = trackingReq.TrackingNumber,
                        NotifyPayer = trackingReq.NotifyPayer,
                        Items = sdkItems
                    }
                };

                // Set carrier - the SDK uses an enum, try to parse it
                if (!string.IsNullOrEmpty(trackingReq.Carrier))
                {
                    if (Enum.TryParse<SdkModels.ShipmentCarrier>(trackingReq.Carrier, true, out var sdkCarrier))
                        input.Body.Carrier = sdkCarrier;
                    else
                        input.Body.CarrierNameOther = trackingReq.Carrier;
                }

                var result = await client.OrdersController.CreateOrderTrackingAsync(input);
                return (true, ConvertSdkResponse<TResponse>(result.Data));
            }

            // Payments - Capture an authorization
            if (request is Api.Payments.CreateCaptureRequest captureAuthReq)
            {
                var client = _sdkClientFactory.GetClient(settings);

                var json = JsonConvert.SerializeObject(captureAuthReq, _jsonSettings);
                var sdkBody = JsonConvert.DeserializeObject<SdkModels.CaptureRequest>(json);

                var input = new SdkModels.CaptureAuthorizedPaymentInput
                {
                    AuthorizationId = captureAuthReq.AuthorizationId,
                    Body = sdkBody,
                    Prefer = "return=representation",
                    PaypalRequestId = Guid.NewGuid().ToString()
                };

                var result = await client.PaymentsController.CaptureAuthorizedPaymentAsync(input);
                return (true, ConvertSdkResponse<TResponse>(result.Data));
            }

            // Payments - Void
            if (request is Api.Payments.CreateVoidRequest voidReq)
            {
                var client = _sdkClientFactory.GetClient(settings);
                var input = new SdkModels.VoidPaymentInput
                {
                    AuthorizationId = voidReq.AuthorizationId,
                    PaypalRequestId = Guid.NewGuid().ToString()
                };

                await client.PaymentsController.VoidPaymentAsync(input);
                return (true, default);
            }

            // Payments - Refund
            if (request is Api.Payments.CreateRefundRequest refundReq)
            {
                var client = _sdkClientFactory.GetClient(settings);

                SdkModels.Money sdkAmount = null;
                if (refundReq.Amount is not null)
                {
                    sdkAmount = new SdkModels.Money
                    {
                        CurrencyCode = refundReq.Amount.CurrencyCode,
                        MValue = refundReq.Amount.Value
                    };
                }

                var input = new SdkModels.RefundCapturedPaymentInput
                {
                    CaptureId = refundReq.CaptureId,
                    Body = new SdkModels.RefundRequest
                    {
                        Amount = sdkAmount
                    },
                    Prefer = "return=representation",
                    PaypalRequestId = Guid.NewGuid().ToString()
                };

                var result = await client.PaymentsController.RefundCapturedPaymentAsync(input);
                return (true, ConvertSdkResponse<TResponse>(result.Data));
            }

            // Vault - Create Setup Token
            if (request is Api.PaymentTokens.CreateSetupTokenRequest setupTokenReq)
            {
                var client = _sdkClientFactory.GetClient(settings);
                var json = JsonConvert.SerializeObject(setupTokenReq, _jsonSettings);
                var sdkBody = JsonConvert.DeserializeObject<SdkModels.SetupTokenRequest>(json);

                var input = new SdkModels.CreateSetupTokenInput
                {
                    Body = sdkBody,
                    PaypalRequestId = Guid.NewGuid().ToString()
                };

                var result = await client.VaultController.CreateSetupTokenAsync(input);
                return (true, ConvertSdkResponse<TResponse>(result.Data));
            }

            // Vault - Create Payment Token
            if (request is Api.PaymentTokens.CreatePaymentTokenRequest paymentTokenReq)
            {
                var client = _sdkClientFactory.GetClient(settings);
                var json = JsonConvert.SerializeObject(paymentTokenReq, _jsonSettings);
                var sdkBody = JsonConvert.DeserializeObject<SdkModels.PaymentTokenRequest>(json);

                var input = new SdkModels.CreatePaymentTokenInput
                {
                    Body = sdkBody,
                    PaypalRequestId = Guid.NewGuid().ToString()
                };

                var result = await client.VaultController.CreatePaymentTokenAsync(input);
                return (true, ConvertSdkResponse<TResponse>(result.Data));
            }

            // Vault - Get (List) Payment Tokens
            if (request is Api.PaymentTokens.GetPaymentTokensRequest getTokensReq)
            {
                var client = _sdkClientFactory.GetClient(settings);
                var input = new SdkModels.ListCustomerPaymentTokensInput
                {
                    CustomerId = getTokensReq.VaultCustomerId,
                    TotalRequired = false
                };

                var result = await client.VaultController.ListCustomerPaymentTokensAsync(input);

                // Convert SDK response to internal format
                var json = JsonConvert.SerializeObject(result.Data, _jsonSettings);
                var response = JsonConvert.DeserializeObject<TResponse>(json);
                return (true, response ?? default);
            }

            // Vault - Delete Payment Token
            if (request is Api.PaymentTokens.DeletePaymentTokenRequest deleteTokenReq)
            {
                var client = _sdkClientFactory.GetClient(settings);
                await client.VaultController.DeletePaymentTokenAsync(deleteTokenReq.Id);
                return (true, default);
            }

            // Vault - Get single Payment Token
            if (request is Api.PaymentTokens.GetPaymentTokenRequest getTokenReq)
            {
                var client = _sdkClientFactory.GetClient(settings);
                var result = await client.VaultController.GetPaymentTokenAsync(getTokenReq.Id);
                return (true, ConvertSdkResponse<TResponse>(result.Data));
            }
        }
        catch (ApiException sdkEx)
        {
            throw ConvertSdkException(sdkEx);
        }

        // Not handled by SDK
        return (false, default);
    }

    #endregion

    /// <summary>
    /// Request remote service via direct HTTP (for operations not covered by SDK)
    /// </summary>
    private async Task<TResponse> RequestViaHttpAsync<TRequest, TResponse>(TRequest request, PayPalCommerceSettings settings)
        where TRequest : IApiRequest where TResponse : IApiResponse
    {
        //prepare request body, content is always JSON except for access token requests
        var requestString = JsonConvert.SerializeObject(request, _jsonSettings);
        var requestContent = request is GetAccessTokenRequest accessTokenRequest
            ? new FormUrlEncodedContent(PayPalCommerceServiceManager.ObjectToDictionary(accessTokenRequest))
            : (ByteArrayContent)new StringContent(requestString, Encoding.Default, MimeTypes.ApplicationJson);

        //URL depends on environment
        var baseUrl = settings.UseSandbox
            ? PayPalCommerceDefaults.ServiceUrl.Sandbox
            : PayPalCommerceDefaults.ServiceUrl.Live;

        var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), new Uri(new Uri(baseUrl), request.Path))
        {
            Content = requestContent
        };

        //set timeout
        try
        {
            var timeout = TimeSpan.FromSeconds(settings.RequestTimeout ?? PayPalCommerceDefaults.RequestTimeout);
            if (_httpClient.Timeout != timeout)
                _httpClient.Timeout = timeout;
        }
        catch { }

        //add authorization and some custom headers
        var authorization = request switch
        {
            IAuthorizedRequest => $"Bearer {await GetAccessTokenAsync(settings)}",
            GetCredentialsRequest credentialsRequest => $"Bearer {credentialsRequest.AccessToken}",
            GetAccessTokenRequest tokenRequest =>
                $"Basic {Convert.ToBase64String(Encoding.Default.GetBytes($"{tokenRequest.ClientId}:{tokenRequest.Secret}"))}",
            _ => null
        };
        if (!string.IsNullOrEmpty(authorization))
            requestMessage.Headers.Add(HeaderNames.Authorization, authorization);
        requestMessage.Headers.Add(HeaderNames.UserAgent, PayPalCommerceDefaults.UserAgent);
        requestMessage.Headers.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);
        requestMessage.Headers.Add(PayPalCommerceDefaults.PartnerHeader.Name, PayPalCommerceDefaults.PartnerHeader.Value);
        requestMessage.Headers.Add("PayPal-Request-Id", Guid.NewGuid().ToString());
        requestMessage.Headers.Add("Prefer", "return=representation");

        //execute the request and get a result
        var httpResponse = await _httpClient.SendAsync(requestMessage);
        var responseString = await httpResponse.Content.ReadAsStringAsync();

        //successful request processing
        if (httpResponse.IsSuccessStatusCode)
        {
            if (typeof(TResponse) == typeof(EmptyResponse))
                return default;

            return JsonConvert.DeserializeObject<TResponse>(responseString ?? string.Empty) ?? default;
        }

        //failed request processing
        var error = $"Failed request ({httpResponse.StatusCode})";
        var identityErrorResponse = JsonConvert.DeserializeObject<IdentityErrorResponse>(responseString ?? string.Empty);
        if (!string.IsNullOrEmpty(identityErrorResponse?.Error))
        {
            var description = !string.IsNullOrEmpty(identityErrorResponse.ErrorDescription)
                ? identityErrorResponse.ErrorDescription
                : identityErrorResponse.Error;
            error += $": {description}";
        }

        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseString ?? string.Empty);
        if (!string.IsNullOrEmpty(errorResponse?.Name))
        {
            error += $": {(!string.IsNullOrEmpty(errorResponse.Message) ? errorResponse.Message : errorResponse.Name)}";
            error += $"{Environment.NewLine}{JsonConvert.SerializeObject(errorResponse, Formatting.Indented)}";
        }

        throw new NopException("Failed request", new NopException(error));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Request remote service
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="request">Request</param>
    /// <param name="settings">Plugin settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the response details
    /// </returns>
    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request, PayPalCommerceSettings settings)
        where TRequest : IApiRequest where TResponse : IApiResponse
    {
        // Try to route through the official PayPal Server SDK
        if (PayPalCommerceServiceManager.IsConfigured(settings))
        {
            var (handled, response) = await TryHandleViaSdkAsync<TRequest, TResponse>(request, settings);
            if (handled)
                return response;
        }

        // Fall back to direct HTTP for operations not covered by the SDK
        // (webhooks, identity tokens, access tokens, onboarding credentials)
        return await RequestViaHttpAsync<TRequest, TResponse>(request, settings);
    }

    #endregion
}
