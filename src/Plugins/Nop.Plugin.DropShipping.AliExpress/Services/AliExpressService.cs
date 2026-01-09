using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.DropShipping.AliExpress.AliExpressSdk.Clients;
using Nop.Plugin.DropShipping.AliExpress.AliExpressSdk.Models;
using Nop.Plugin.DropShipping.AliExpress.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;

namespace Nop.Plugin.DropShipping.AliExpress.Services;

/// <summary>
/// Service for interacting with AliExpress API
/// </summary>
public class AliExpressService : IAliExpressService
{
    private readonly ISettingService _settingService;
    private readonly ILogger _logger;
    private readonly IStoreContext _storeContext;


    public AliExpressService(
        ISettingService settingService,
        ILogger logger,
        IStoreContext storeContext)
    {
        _settingService = settingService;
        _logger = logger;
        _storeContext = storeContext;
    }

    public async Task<string> GetAuthorizationUrlAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var settings = await _settingService.LoadSettingAsync<AliExpressSettings>(store.Id);

        if (string.IsNullOrEmpty(settings.AppKey))
            return string.Empty;

        settings.RedirectUriHost = Uri.TryCreate(settings.RedirectUriHost, UriKind.Absolute, out var host) ? host.ToString() : new AliExpressSettings().RedirectUriHost;
        settings.RedirectUri = Uri.TryCreate(settings.RedirectUri, UriKind.Relative, out var redirect) ? redirect.ToString() : new AliExpressSettings().RedirectUri;
        var builder = new UriBuilder(settings.RedirectUriHost);
        builder.Path = settings.RedirectUri;
        settings.RedirectUri = builder.Uri.PathAndQuery;
        

        // Build the OAuth authorization URL
        var redirectUri = $"{settings.RedirectUriHost}{settings.RedirectUri}";
        var authUrl = BuildUrl(redirectUri, settings);
        return await Task.FromResult(authUrl);
    }

    private string BuildUrl(
        string redirectUri,
        AliExpressSettings settings)
    {
        var uriBuilder = new UriBuilder(settings.AuthorizationUrl);
        var queryStringBuilder = new StringBuilder();
        queryStringBuilder.Append("response_type=code");
        queryStringBuilder.Append("&force_auth=true");
        queryStringBuilder.Append($"&redirect_uri={redirectUri ?? "https://localhost"}");
        queryStringBuilder.Append($"&client_id={settings.AppKey}");
        uriBuilder.Query = queryStringBuilder.ToString();
        var uri = uriBuilder.ToString();
        return uri;
    }

    public async Task<bool> ExchangeAuthorizationCodeAsync(
        string code)
    {
        try
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var settings = await _settingService.LoadSettingAsync<AliExpressSettings>(store.Id);

            if (string.IsNullOrEmpty(settings.AppKey) || string.IsNullOrEmpty(settings.AppSecret))
                return false;

            // Create parameters dictionary with only the token-specific parameters
            // The Execute method in AESystemClient will add app_key, timestamp, sign_method, etc.
            var parameters = new Dictionary<string, string>
            {
                ["code"] = code
            };
            
            var client = new AESystemClient(settings.AppKey, settings.AppSecret, "");
            var response = await client.GenerateToken(parameters);

            var tokenResponse =
                JsonConvert.DeserializeObject<TokenResponse>(response.Data.GetRawText());


            if (response.Ok && tokenResponse.AccessToken != null)
            {
                var storeId = (await _storeContext.GetCurrentStoreAsync())?.Id ?? 0;

                settings.AccessToken = tokenResponse.AccessToken;
                settings.RefreshToken = tokenResponse.RefreshToken;
                /*
                 * Here is a breakdown of the time duration:
                   Value: 2,592,000
                   Units: Seconds
                   Equivalent time: 30 days (calculated as 2,592,000 seconds / 86,400 seconds per day).
                 */
                settings.AccessTokenExpiresOnUtc = DateTime.UtcNow.AddSeconds(double.TryParse(tokenResponse.ExpiresIn.ToString(), out var expiresIn) ? expiresIn : 0);
                settings.RefreshTokenExpiresOnUtc = DateTime.UtcNow.AddSeconds(double.TryParse(tokenResponse.RefreshExpiresIn.ToString(), out var refreshExpiresIn) ? refreshExpiresIn : 0);

                await _settingService.SaveSettingAsync(settings, storeId);

                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error exchanging authorization code: {ex.Message}", ex);
            return false;
        }
    }

    public async Task<bool> RefreshAccessTokenAsync()
    {
        try
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var settings = await _settingService.LoadSettingAsync<AliExpressSettings>(store.Id);


            if (string.IsNullOrEmpty(settings.AppKey) ||
                string.IsNullOrEmpty(settings.AppSecret) ||
                string.IsNullOrEmpty(settings.RefreshToken))
                return false;

            var client = new AESystemClient(settings.AppKey, settings.AppSecret, string.Empty);
            var tokenResult = await client.RefreshToken(new Dictionary<string, string>() { ["refresh_token"] = settings.RefreshToken });

            var response = ParseTokenResponse(tokenResult.Data);

            if (tokenResult.Ok && response.AccessToken != null)
            {
                var storeId = (await _storeContext.GetCurrentStoreAsync())?.Id ?? 0;

                settings.AccessToken = response.AccessToken;
                settings.RefreshToken = response.RefreshToken;
                settings.AccessTokenExpiresOnUtc = DateTime.UtcNow.AddSeconds(response.ExpiresIn);
                settings.RefreshTokenExpiresOnUtc = DateTime.UtcNow.AddSeconds(response.RefreshTokenValidTime);

                await _settingService.SaveSettingAsync(settings, storeId);

                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error refreshing access token: {ex.Message}", ex);
            return false;
        }
    }

    public async Task<bool> IsTokenValid()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var settings = await _settingService.LoadSettingAsync<AliExpressSettings>(store.Id);

        if (string.IsNullOrEmpty(settings.AccessToken))
            return false;

        if (!settings.AccessTokenExpiresOnUtc.HasValue)
            return false;

        return settings.AccessTokenExpiresOnUtc.Value > DateTime.UtcNow;
    }

    public async Task<List<AliExpressProductSearchResultModel>> SearchProductsAsync(
        string keyword,
        int pageNo = 1,
        int pageSize = 20)
    {
        var results = new List<AliExpressProductSearchResultModel>();

        try
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var settings = await _settingService.LoadSettingAsync<AliExpressSettings>(store.Id);

            if (!await IsTokenValid() || string.IsNullOrEmpty(settings.AccessToken))
                return results;

            var client = new AEBaseClient(settings.AppKey, settings.AppSecret, settings.AccessToken);

            var parameters = new Dictionary<string, string>
            {
                ["keyWord"] = keyword,
                ["local"] = settings.DefaultLanguage ?? "en_US",
                ["countryCode"] = settings.DefaultShippingCountry ?? "ZA",
                ["currency"] = settings.DefaultCurrency ?? "ZAR",
                ["pageSize"] = pageSize.ToString(),
                ["pageIndex"] = pageNo.ToString()
            };

            // Use the dropshipper product search API - matches ConsoleHarness exactly
            var responseResult = await client.CallApiDirectly("aliexpress.ds.text.search", parameters);

            if (responseResult.Ok && responseResult.Data is { } data)
            {
                var response = System.Text.Json.JsonSerializer.Deserialize<ProductSearchResponse>(data.GetRawText());

                if (response?.AliexpressDsTextSearchResponse?.Data?.Products?.SelectionSearchProduct != null)
                {
                    results = response.AliexpressDsTextSearchResponse.Data.Products.SelectionSearchProduct
                        .Select(p => new AliExpressProductSearchResultModel
                        {
                            ProductId = long.TryParse(p.ItemId, out var id) ? id : 0,
                            ProductTitle = p.Title,
                            ProductUrl = p.ItemUrl,
                            ImageUrl = p.ItemMainPic,
                            OriginalPrice = decimal.TryParse(p.TargetOriginalPrice, out var origPrice) ? origPrice : null,
                            SalePrice = decimal.TryParse(p.TargetSalePrice, out var salePrice) ? salePrice : null,
                            Currency = p.TargetOriginalPriceCurrency,
                            SalesCount = int.TryParse(p.Orders?.TrimEnd('+'), out var orders) ? (int?)orders : null,
                            Rating = decimal.TryParse(p.Score, out var score) ? score : null
                        }).ToList();
                }
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error searching products: {ex.Message}", ex);
        }

        return results;
    }

    public async Task<AliExpressProductDetailsModel?> GetProductDetailsAsync(
        long productId,
        string? shipToCountry = null)
    {
        try
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var settings = await _settingService.LoadSettingAsync<AliExpressSettings>(store.Id);

            if (!await IsTokenValid() || string.IsNullOrEmpty(settings.AccessToken))
                return null;

            var client = new AEBaseClient(settings.AppKey, settings.AppSecret, settings.AccessToken);

            var parameters = new Dictionary<string, string>
            {
                ["product_id"] = productId.ToString(), ["ship_to_country"] = shipToCountry ?? settings.DefaultShippingCountry ?? "ZA", ["target_currency"] = settings.DefaultCurrency ?? "ZAR", ["target_language"] = "en"
            };

            var result = await client.CallApiDirectly("aliexpress.ds.product.get", parameters);

            if (result.Ok && result.Data is { } data)
            {
                var response = System.Text.Json.JsonSerializer.Deserialize<ProductDetailsResponse>(data.GetRawText());

                if (response?.AliexpressDsProductGetResponse?.Result != null)
                {
                    var productResult = response.AliexpressDsProductGetResponse.Result;

                    return new AliExpressProductDetailsModel
                    {
                        ProductId = productId,
                        ProductTitle = productResult.Subject,
                        ProductUrl = $"https://www.aliexpress.com/item/{productId}.html",
                        ImageUrls = productResult.ProductMainImageUrl != null
                            ? new List<string> { productResult.ProductMainImageUrl }
                            : new List<string>(),
                        Description = productResult.Subject,
                        IsAvailable = productResult.AeItemSkuInfoDtos?.AeItemSkuInfoDto?.Any() ?? false,
                        Currency = productResult.AeItemSkuInfoDtos?.AeItemSkuInfoDto?.FirstOrDefault()?.CurrencyCode
                    };
                }
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error getting product details: {ex.Message}", ex);
        }

        return null;
    }

    public async Task<List<AliExpressFreightModel>> GetFreightInfoAsync(
        long productId,
        int quantity,
        string countryCode)
    {
        var results = new List<AliExpressFreightModel>();

        try
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var settings = await _settingService.LoadSettingAsync<AliExpressSettings>(store.Id);

            if (!await IsTokenValid() || string.IsNullOrEmpty(settings.AccessToken))
                return results;

            var client = new AEBaseClient(settings.AppKey, settings.AppSecret, settings.AccessToken);

            // First get product details to get SKU information
            var productParams = new Dictionary<string, string> { ["product_id"] = productId.ToString(), ["ship_to_country"] = countryCode, ["target_currency"] = settings.DefaultCurrency ?? "ZAR", ["target_language"] = "en" };

            var productResult = await client.CallApiDirectly("aliexpress.ds.product.get", productParams);
            string? skuId = null;

            if (productResult.Ok && productResult.Data is { } productData)
            {
                var productResponse = System.Text.Json.JsonSerializer.Deserialize<ProductDetailsResponse>(productData.GetRawText());
                skuId = productResponse?.AliexpressDsProductGetResponse?.Result?.AeItemSkuInfoDtos?.AeItemSkuInfoDto?.FirstOrDefault()?.SkuId;
            }

            if (string.IsNullOrEmpty(skuId))
            {
                await _logger.WarningAsync($"No SKU found for product {productId}, cannot get freight info");
                return results;
            }

            // Build the freight query request matching ConsoleHarness exactly
            // Note: Province and city are hardcoded for South African default.
            // In production with actual orders, these should be obtained from the
            // customer's shipping address to get accurate freight estimates.
            var queryDeliveryReq = new
            {
                quantity = quantity,
                shipToCountry = countryCode,
                productId = productId.ToString(),
                provinceCode = "Western Cape", // TODO: Get from actual shipping address in order flow
                cityCode = "Cape Town", // TODO: Get from actual shipping address in order flow
                selectedSkuId = skuId,
                language = "en_US",
                currency = settings.DefaultCurrency ?? "ZAR",
                locale = "en_US"
            };

            var parameters = new Dictionary<string, string>
            {
                ["queryDeliveryReq"] = System.Text.Json.JsonSerializer.Serialize(queryDeliveryReq, new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase })
            };

            var result = await client.CallApiDirectly("aliexpress.ds.freight.query", parameters);

            if (result.Ok && result.Data is { } data)
            {
                var response = System.Text.Json.JsonSerializer.Deserialize<FreightEstimationResponse>(data.GetRawText());

                if (response?.AliexpressDsFreightQueryResponse?.Result?.DeliveryOptions?.DeliveryOptionDto != null)
                {
                    results = response.AliexpressDsFreightQueryResponse.Result.DeliveryOptions.DeliveryOptionDto
                        .Select(d => new AliExpressFreightModel
                        {
                            ServiceName = d.Code,
                            // Note: AliExpress API doesn't provide shipping cost in the freight query response
                            // when free_shipping is true. For paid shipping, cost would need to be calculated
                            // separately or obtained from a different API endpoint.
                            ShippingCost = 0,
                            Currency = settings.DefaultCurrency ?? "ZAR",
                            EstimatedDeliveryDays = d.MaxDeliveryDays,
                            TrackingAvailable = d.Tracking ? "Yes" : "No"
                        }).ToList();
                }
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error getting freight info: {ex.Message}", ex);
        }

        return results;
    }

    public async Task<long?> CreateOrderAsync(
        int nopOrderId)
    {
        try
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var settings = await _settingService.LoadSettingAsync<AliExpressSettings>(store.Id);
            if (!await IsTokenValid() || string.IsNullOrEmpty(settings.AccessToken))
                return null;

            await _logger.InformationAsync($"Creating AliExpress order for NopCommerce order {nopOrderId}");

            // Note: This is a placeholder. The actual implementation would need:
            // 1. Get the NopCommerce order details
            // 2. Get the product mapping (AliExpress product ID, SKU)
            // 3. Get customer shipping address
            // 4. Get freight estimation to get logistics service name
            // 5. Build the order request matching the ConsoleHarness CreateOrderWorkflowCommand

            // For now, returning null to indicate not implemented yet
            // Full implementation would follow the exact pattern from CreateOrderWorkflowCommand.cs

            return null;
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error creating order: {ex.Message}", ex);
            return null;
        }
    }

    public async Task<AliExpressOrderTrackingModel?> GetOrderTrackingAsync(
        long aliExpressOrderId)
    {
        try
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var settings = await _settingService.LoadSettingAsync<AliExpressSettings>(store.Id);


            if (!await IsTokenValid() || string.IsNullOrEmpty(settings.AccessToken))
                return null;

            await _logger.InformationAsync($"Getting tracking for AliExpress order {aliExpressOrderId}");

            // Placeholder - implement tracking logic
            return null;
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error getting order tracking: {ex.Message}", ex);
            return null;
        }
    }

    private TokenResponse ParseTokenResponse(
        System.Text.Json.JsonElement data)
    {
        try
        {
            // Check if this is an error response
            // Error responses have "code" field that is not "0" (success)
            if (data.TryGetProperty("code", out var codeProperty) &&
                codeProperty.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                var code = codeProperty.GetString();
                if (!string.IsNullOrEmpty(code) && code != "0")
                {
                    // This is an error response
                    if (data.TryGetProperty("message", out var messageProperty))
                    {
                        Console.Error.WriteLine($"API Error ({code}): {messageProperty.GetString()}");
                    }

                    return null;
                }
            }

            // The response might be nested under a specific property or be at root
            var tokenData = data;

            // Try to find the token data - it might be nested
            if (data.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                // Check if there's a nested response structure
                foreach (var property in data.EnumerateObject())
                {
                    // Look for common response wrapper properties
                    if (property.Name.Contains("result") || property.Name.Contains("data"))
                    {
                        tokenData = property.Value;
                        break;
                    }
                }
            }

            var token = new TokenResponse
            {
                AccessToken = GetStringProperty(tokenData, "access_token"),
                RefreshToken = GetStringProperty(tokenData, "refresh_token"),
                AccountPlatform = GetStringProperty(tokenData, "account_platform"),
                UserNick = GetStringProperty(tokenData, "user_nick"),
                UserId = GetStringProperty(tokenData, "user_id"),
                SellerId = GetStringProperty(tokenData, "seller_id"),
                HavanaId = GetStringProperty(tokenData, "havana_id"),
                Account = GetStringProperty(tokenData, "account"),
                Locale = GetStringProperty(tokenData, "locale"),
                Sp = GetStringProperty(tokenData, "sp"),
                ExpiresIn = GetLongProperty(tokenData, "expires_in"),
                RefreshExpiresIn = GetLongProperty(tokenData, "refresh_expires_in"),
                ExpireTime = GetLongProperty(tokenData, "expire_time"),
                RefreshTokenValidTime = GetLongProperty(tokenData, "refresh_token_valid_time"),
                Code = GetStringProperty(tokenData, "code"),
                RequestId = GetStringProperty(tokenData, "request_id")
            };

            return token;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error parsing token response: {ex.Message}");
            return null;
        }
    }

    private string? GetStringProperty(
        System.Text.Json.JsonElement element,
        string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property))
        {
            return property.ValueKind == System.Text.Json.JsonValueKind.String
                ? property.GetString()
                : property.ToString();
        }

        return null;
    }

    private long GetLongProperty(
        System.Text.Json.JsonElement element,
        string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property))
        {
            if (property.ValueKind == System.Text.Json.JsonValueKind.Number)
            {
                return property.GetInt64();
            }

            // Try to parse as string in case it's returned as a string
            if (property.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                if (long.TryParse(property.GetString(), out var value))
                {
                    return value;
                }
            }
        }

        return 0;
    }
}