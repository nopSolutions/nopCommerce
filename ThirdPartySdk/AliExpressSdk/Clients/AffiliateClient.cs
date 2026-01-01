using System.Net.Http;
using System.Text.Json;
using AliExpressSdk.Models;

namespace AliExpressSdk.Clients;

public class AffiliateClient : AESystemClient
{
    public AffiliateClient(string appKey, string appSecret, string session, HttpClient? httpClient = null)
        : base(appKey, appSecret, session, httpClient)
    {
    }

    public Task<Result<JsonElement>> GenerateAffiliateLinks(IDictionary<string, string> args)
        => Execute("aliexpress.affiliate.link.generate", args);

    public Task<Result<JsonElement>> GetCategories(IDictionary<string, string> args)
        => Execute("aliexpress.affiliate.category.get", args);

    public Task<Result<JsonElement>> FeaturedPromoInfo(IDictionary<string, string> args)
        => Execute("aliexpress.affiliate.featuredpromo.get", args);

    public Task<Result<JsonElement>> FeaturedPromoProducts(IDictionary<string, string> args)
        => Execute("aliexpress.affiliate.featuredpromo.products.get", args);

    public Task<Result<JsonElement>> GetHotProductsDownload(IDictionary<string, string> args)
        => Execute("aliexpress.affiliate.hotproduct.download", args);

    public Task<Result<JsonElement>> GetHotProducts(IDictionary<string, string> args)
        => Execute("aliexpress.affiliate.hotproduct.query", args);

    public Task<Result<JsonElement>> OrderInfo(IDictionary<string, string> args)
        => Execute("aliexpress.affiliate.order.get", args);

    public Task<Result<JsonElement>> OrdersList(IDictionary<string, string> args)
        => Execute("aliexpress.affiliate.order.list", args);

    public Task<Result<JsonElement>> OrdersListByIndex(IDictionary<string, string> args)
        => Execute("aliexpress.affiliate.order.listbyindex", args);

    public Task<Result<JsonElement>> ProductDetails(IDictionary<string, string> args)
        => Execute("aliexpress.affiliate.productdetail.get", args);

    public Task<Result<JsonElement>> QueryProducts(IDictionary<string, string> args)
        => Execute("aliexpress.affiliate.product.query", args);

    public Task<Result<JsonElement>> SmartMatchProducts(IDictionary<string, string> args)
        => Execute("aliexpress.affiliate.product.smartmatch", args);
}
