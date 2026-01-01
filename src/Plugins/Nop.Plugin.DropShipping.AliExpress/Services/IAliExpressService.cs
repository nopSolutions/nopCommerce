using Nop.Plugin.DropShipping.AliExpress.Models;

namespace Nop.Plugin.DropShipping.AliExpress.Services;

/// <summary>
/// Interface for AliExpress API service
/// </summary>
public interface IAliExpressService
{
    /// <summary>
    /// Gets the authorization URL for OAuth flow
    /// </summary>
    Task<string> GetAuthorizationUrlAsync();

    /// <summary>
    /// Exchanges authorization code for access token
    /// </summary>
    Task<bool> ExchangeAuthorizationCodeAsync(string code);

    /// <summary>
    /// Refreshes the access token
    /// </summary>
    Task<bool> RefreshAccessTokenAsync();

    /// <summary>
    /// Checks if the current token is valid
    /// </summary>
    bool IsTokenValid();

    /// <summary>
    /// Searches for products on AliExpress
    /// </summary>
    Task<List<AliExpressProductSearchResultModel>> SearchProductsAsync(string keyword, int pageNo = 1, int pageSize = 20);

    /// <summary>
    /// Gets detailed product information
    /// </summary>
    Task<AliExpressProductDetailsModel?> GetProductDetailsAsync(long productId, string? shipToCountry = null);

    /// <summary>
    /// Gets freight/shipping information for a product
    /// </summary>
    Task<List<AliExpressFreightModel>> GetFreightInfoAsync(long productId, int quantity, string countryCode);

    /// <summary>
    /// Creates an order on AliExpress
    /// </summary>
    Task<long?> CreateOrderAsync(int nopOrderId);

    /// <summary>
    /// Gets order tracking information
    /// </summary>
    Task<AliExpressOrderTrackingModel?> GetOrderTrackingAsync(long aliExpressOrderId);
}
