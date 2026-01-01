using System.Text.Json;
using AliExpressSdk.Clients;
using AliExpressSdk.Models;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.DropShipping.AliExpress.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Nop.Plugin.DropShipping.AliExpress.Services;

/// <summary>
/// Service for interacting with AliExpress API
/// </summary>
public class AliExpressService : IAliExpressService
{
    private readonly ISettingService _settingService;
    private readonly ILogger _logger;
    private readonly IStoreContext _storeContext;
    private readonly AliExpressSettings _settings;

    public AliExpressService(
        ISettingService settingService,
        ILogger logger,
        IStoreContext storeContext)
    {
        _settingService = settingService;
        _logger = logger;
        _storeContext = storeContext;
        
        var storeId = _storeContext.GetCurrentStoreAsync().GetAwaiter().GetResult()?.Id ?? 0;
        _settings = _settingService.LoadSettingAsync<AliExpressSettings>(storeId).GetAwaiter().GetResult();
    }

    public async Task<string> GetAuthorizationUrlAsync()
    {
        if (string.IsNullOrEmpty(_settings.AppKey))
            throw new NopException("App Key is not configured");

        // Build the OAuth authorization URL
        var redirectUri = "https://oauth.aliexpress.com/authorize";
        var authUrl = $"https://oauth.aliexpress.com/authorize?response_type=code&client_id={_settings.AppKey}&redirect_uri={Uri.EscapeDataString(redirectUri)}";
        
        return await Task.FromResult(authUrl);
    }

    public async Task<bool> ExchangeAuthorizationCodeAsync(string code)
    {
        try
        {
            if (string.IsNullOrEmpty(_settings.AppKey) || string.IsNullOrEmpty(_settings.AppSecret))
                return false;

            var client = new AESystemClient(_settings.AppKey, _settings.AppSecret, string.Empty);
            var response = await client.GenerateToken(new Dictionary<string, string>()
            {
                // Code = code
                ["code"] = code
            });

            var tokenResponse = 
                JsonConvert.DeserializeObject<TokenResponse>(response.Data.GetRawText());
            
            
            

            if (response.Ok && tokenResponse.AccessToken != null)
            {
                var storeId = (await _storeContext.GetCurrentStoreAsync())?.Id ?? 0;
                
                _settings.AccessToken = tokenResponse.AccessToken;
                _settings.RefreshToken = tokenResponse.RefreshToken;
                /*
                 * Here is a breakdown of the time duration:
                   Value: 2,592,000
                   Units: Seconds
                   Equivalent time: 30 days (calculated as 2,592,000 seconds / 86,400 seconds per day). 
                 */
                _settings.AccessTokenExpiresOnUtc = DateTime.UtcNow.AddSeconds(double.TryParse(tokenResponse.ExpiresIn.ToString(), out var expiresIn) ? expiresIn : 0);
                _settings.RefreshTokenExpiresOnUtc = DateTime.UtcNow.AddSeconds(double.TryParse(tokenResponse.RefreshExpiresIn.ToString(), out var refreshExpiresIn) ? refreshExpiresIn : 0);
                
                await _settingService.SaveSettingAsync(_settings, storeId);
                
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
            if (string.IsNullOrEmpty(_settings.AppKey) || 
                string.IsNullOrEmpty(_settings.AppSecret) || 
                string.IsNullOrEmpty(_settings.RefreshToken))
                return false;

            var client = new AESystemClient(_settings.AppKey, _settings.AppSecret, string.Empty);
            var tokenResult = await client.RefreshToken(new Dictionary<string, string>()
            {
                ["refresh_token"] = _settings.RefreshToken
            });

            var response = ParseTokenResponse(tokenResult.Data);
            
            if (tokenResult.Ok && response.AccessToken != null)
            {
                var storeId = (await _storeContext.GetCurrentStoreAsync())?.Id ?? 0;
                
                _settings.AccessToken = response.AccessToken;
                _settings.RefreshToken = response.RefreshToken;
                _settings.AccessTokenExpiresOnUtc = DateTime.UtcNow.AddSeconds(response.ExpiresIn);
                _settings.RefreshTokenExpiresOnUtc = DateTime.UtcNow.AddSeconds(response.RefreshTokenValidTime);
                
                await _settingService.SaveSettingAsync(_settings, storeId);
                
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

    public bool IsTokenValid()
    {
        if (string.IsNullOrEmpty(_settings.AccessToken))
            return false;

        if (!_settings.AccessTokenExpiresOnUtc.HasValue)
            return false;

        return _settings.AccessTokenExpiresOnUtc.Value > DateTime.UtcNow;
    }

    public async Task<List<AliExpressProductSearchResultModel>> SearchProductsAsync(string keyword, int pageNo = 1, int pageSize = 20)
    {
        var results = new List<AliExpressProductSearchResultModel>();

        try
        {
            if (!IsTokenValid() || string.IsNullOrEmpty(_settings.AccessToken))
                return results;

            var client = new AEBaseClient(_settings.AppKey, _settings.AppSecret, _settings.AccessToken);
            
            var parameters = new Dictionary<string, string>
            {
                ["keyWord"] = keyword,
                ["local"] = _settings.DefaultLanguage ?? "en_US",
                ["countryCode"] = _settings.DefaultShippingCountry ?? "ZA",
                ["currency"] = _settings.DefaultCurrency ?? "ZAR",
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
                            SalesCount = null, // Not directly in the response
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

    public async Task<AliExpressProductDetailsModel?> GetProductDetailsAsync(long productId, string? shipToCountry = null)
    {
        try
        {
            if (!IsTokenValid() || string.IsNullOrEmpty(_settings.AccessToken))
                return null;

            var client = new AEBaseClient(_settings.AppKey, _settings.AppSecret, _settings.AccessToken);
            
            var parameters = new Dictionary<string, string>
            {
                ["product_id"] = productId.ToString(),
                ["ship_to_country"] = shipToCountry ?? _settings.DefaultShippingCountry ?? "ZA",
                ["target_currency"] = _settings.DefaultCurrency ?? "ZAR",
                ["target_language"] = "en"
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

    public async Task<List<AliExpressFreightModel>> GetFreightInfoAsync(long productId, int quantity, string countryCode)
    {
        var results = new List<AliExpressFreightModel>();

        try
        {
            if (!IsTokenValid() || string.IsNullOrEmpty(_settings.AccessToken))
                return results;

            var client = new AEBaseClient(_settings.AppKey, _settings.AppSecret, _settings.AccessToken);

            // First get product details to get SKU information
            var productParams = new Dictionary<string, string>
            {
                ["product_id"] = productId.ToString(),
                ["ship_to_country"] = countryCode,
                ["target_currency"] = _settings.DefaultCurrency ?? "ZAR",
                ["target_language"] = "en"
            };
            
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
            var queryDeliveryReq = new
            {
                quantity = quantity,
                shipToCountry = countryCode,
                productId = productId.ToString(),
                provinceCode = "Western Cape", // Default for South Africa
                cityCode = "Cape Town",
                selectedSkuId = skuId,
                language = "en_US",
                currency = _settings.DefaultCurrency ?? "ZAR",
                locale = "en_US"
            };
            
            var parameters = new Dictionary<string, string>
            {
                ["queryDeliveryReq"] = System.Text.Json.JsonSerializer.Serialize(queryDeliveryReq, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                })
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
                            ShippingCost = d.FreeShipping ? 0 : 0, // Free shipping doesn't provide cost
                            Currency = _settings.DefaultCurrency ?? "ZAR",
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

    public async Task<long?> CreateOrderAsync(int nopOrderId)
    {
        try
        {
            if (!IsTokenValid() || string.IsNullOrEmpty(_settings.AccessToken))
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

    public async Task<AliExpressOrderTrackingModel?> GetOrderTrackingAsync(long aliExpressOrderId)
    {
        try
        {
            if (!IsTokenValid() || string.IsNullOrEmpty(_settings.AccessToken))
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
      private TokenResponse ParseTokenResponse(System.Text.Json.JsonElement data)
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
    private string? GetStringProperty(System.Text.Json.JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property))
        {
            return property.ValueKind == System.Text.Json.JsonValueKind.String 
                ? property.GetString() 
                : property.ToString();
        }
        return null;
    }
    
    private long GetLongProperty(System.Text.Json.JsonElement element, string propertyName)
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
