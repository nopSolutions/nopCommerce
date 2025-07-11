using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AbcWarehouse.Plugin.Misc.SearchSpring.Models;
using System.Web;
using System.Linq;
using Nop.Services.Logging;
using Nop.Core.Domain.Logging;

namespace AbcWarehouse.Plugin.Misc.SearchSpring.Services
{
    public class SearchSpringService : ISearchSpringService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl = "https://4lt84w.a.searchspring.io";
        private readonly ILogger _logger;

        public SearchSpringService(IHttpClientFactory httpClientFactory, ILogger logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<SearchResultModel> SearchAsync(string query, string sessionId = null,
                                                         string userId = null, string siteId = "4lt84w",
                                                         int page = 1, Dictionary<string, List<string>> filters = null,
                                                         string sort = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Search query must not be null or empty.", nameof(query));

            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };

            var client = new HttpClient(handler);

            var queryParams = new List<string>
            {
                $"q={HttpUtility.UrlEncode(query)}",
                "resultsFormat=json",
                "resultsPerPage=25",
                $"page={page}",
                "redirectResponse=direct"
            };

            if (!string.IsNullOrEmpty(sessionId))
                queryParams.Add($"ss-sessionId={HttpUtility.UrlEncode(sessionId)}");

            if (!string.IsNullOrEmpty(siteId))
                queryParams.Add($"siteId={HttpUtility.UrlEncode(siteId)}");

            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    foreach (var value in filter.Value)
                    {
                        queryParams.Add($"filter.{HttpUtility.UrlEncode(filter.Key)}={HttpUtility.UrlEncode(value)}");
                    }
                }
            }

            if (!string.IsNullOrEmpty(sort))
            {
                var parts = sort.Contains(":") ? sort.Split(':') :
                            sort.Contains("_") ? sort.Split('_') : null;

                if (parts?.Length == 2)
                {
                    var field = parts[0];
                    var direction = parts[1];
                    queryParams.Add($"sort.{HttpUtility.UrlEncode(field)}={HttpUtility.UrlEncode(direction)}");
                }
                else
                {
                    queryParams.Add("sort.relevance=desc");
                }
            }
            else
            {
                queryParams.Add("sort.relevance=desc"); // default fallback
            }

            var url = $"{_baseUrl}/api/search/search.json?{string.Join("&", queryParams)}";

            Console.WriteLine($"[SearchSpring] Final Request URL: {url}");

            var response = await client.GetAsync(url);

            if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
            {
                if (response.Headers.Location != null)
                {
                    var redirectUrl = response.Headers.Location.ToString();
                    Console.WriteLine($"[SearchSpring] Redirect detected: {redirectUrl}");
                    return new SearchResultModel
                    {
                        RedirectResponse = redirectUrl
                    };
                }
            }

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[SearchSpring] Response ({(int)response.StatusCode}): {json}");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Searchspring returned error {response.StatusCode}: {json}\nURL: {url}");

            try
            {
                var productList = new List<SearchSpringProductModel>();
                int currentPage = 1, pageSize = 24, totalResults = 0;
                var facets = new Dictionary<string, FacetDetail>();

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Legacy redirect parsing fallback (JSON-based)
                if (root.TryGetProperty("redirect", out var redirectProp) &&
                    redirectProp.TryGetProperty("url", out var redirectUrlProp) &&
                    redirectUrlProp.ValueKind == JsonValueKind.String &&
                    !string.IsNullOrEmpty(redirectUrlProp.GetString()))
                {
                    var redirectUrl = redirectUrlProp.GetString();

                    return new SearchResultModel
                    {
                        RedirectResponse = redirectUrl
                    };
                }

                if (root.TryGetProperty("results", out var resultsElement) && resultsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in resultsElement.EnumerateArray())
                    {
                        var model = new SearchSpringProductModel
                        {
                            Id = item.TryGetProperty("id", out var idProp) ? idProp.GetString() : "",
                            Name = item.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "",
                            ProductUrl = item.TryGetProperty("url", out var urlProp) ? urlProp.GetString() : "",
                            ImageUrl = item.TryGetProperty("imageUrl", out var imgProp) ? imgProp.GetString() : "",
                            Price = item.TryGetProperty("price", out var priceProp) ? priceProp.GetString() : "",
                            Brand = item.TryGetProperty("brand", out var brandProp) ? brandProp.GetString() : "",
                            Category = item.TryGetProperty("category", out var catProp) ? catProp.GetString() : "",
                            ItemNumber = item.TryGetProperty("item_number", out var itemNumProp) ? itemNumProp.GetString() : "",
                            RetailPrice = item.TryGetProperty("retail_price", out var retailPriceProp) ? retailPriceProp.GetString() : "",
                            Sku = item.TryGetProperty("sku", out var skuProp) ? skuProp.GetString() : ""
                        };
                        productList.Add(model);
                    }
                }

                if (root.TryGetProperty("pagination", out var pagination) && pagination.ValueKind == JsonValueKind.Object)
                {
                    currentPage = pagination.TryGetProperty("currentPage", out var pageProp) ? pageProp.GetInt32() : 1;
                    pageSize = pagination.TryGetProperty("pageSize", out var sizeProp) ? sizeProp.GetInt32() : 25;
                    totalResults = pagination.TryGetProperty("totalResults", out var totalProp) ? totalProp.GetInt32() : 0;
                }

                if (root.TryGetProperty("facets", out var facetsProp) && facetsProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var facet in facetsProp.EnumerateArray())
                    {
                        var field = facet.TryGetProperty("field", out var fieldProp) ? fieldProp.GetString() : "";

                        var detail = new FacetDetail
                        {
                            Field = field,
                            Label = facet.TryGetProperty("label", out var labelProp) ? labelProp.GetString() : "",
                            Multiple = facet.TryGetProperty("multiple", out var multipleProp) ? multipleProp.GetString() : "",
                            Collapse = facet.TryGetProperty("collapse", out var collapseProp) && collapseProp.GetInt32() == 1
                        };

                        if (facet.TryGetProperty("values", out var valuesProp) && valuesProp.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var val in valuesProp.EnumerateArray())
                            {
                                detail.Values.Add(new FacetValue
                                {
                                    Value = val.TryGetProperty("value", out var v) ? v.GetString() : "",
                                    Label = val.TryGetProperty("label", out var l) ? l.GetString() : "",
                                    Count = val.TryGetProperty("count", out var c) ? c.GetInt32() : 0
                                });
                            }
                        }

                        if (!string.IsNullOrEmpty(field))
                            facets[field] = detail;
                    }
                }

                var sortOptions = new List<SortOption>();

                if (root.TryGetProperty("sorting", out var sortingProp) &&
                    sortingProp.TryGetProperty("options", out var optionsProp) &&
                    optionsProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var sortOption in optionsProp.EnumerateArray())
                    {
                        sortOptions.Add(new SortOption
                        {
                            Field = sortOption.TryGetProperty("field", out var fieldProp) ? fieldProp.GetString() : "",
                            Direction = sortOption.TryGetProperty("direction", out var dirProp) ? dirProp.GetString() : "",
                            Label = sortOption.TryGetProperty("label", out var labelProp) ? labelProp.GetString() : ""
                        });
                    }
                }

                var bannersByPosition = new Dictionary<string, List<string>>();

                if (root.TryGetProperty("merchandising", out var merchProp) &&
                    merchProp.TryGetProperty("content", out var contentProp) &&
                    contentProp.ValueKind == JsonValueKind.Object)
                {
                    foreach (var position in new[] { "header", "banner", "footer", "left" })
                    {
                        if (contentProp.TryGetProperty(position, out var bannerArray) &&
                            bannerArray.ValueKind == JsonValueKind.Array)
                        {
                            var banners = bannerArray.EnumerateArray()
                                .Where(b => b.ValueKind == JsonValueKind.String)
                                .Select(b => b.GetString())
                                .Where(html => !string.IsNullOrEmpty(html))
                                .Select(html =>
                                {
                                    var split = html.Split(new[] { "</script>" }, StringSplitOptions.RemoveEmptyEntries);
                                    return split.Length > 1 ? split[1].Trim() : html.Trim();
                                })
                                .Where(cleaned => !string.IsNullOrWhiteSpace(cleaned))
                                .ToList();


                            if (banners.Any())
                            {
                                bannersByPosition[position] = banners;
                            }
                        }
                    }
                }
                else
                {
                    await _logger.InsertLogAsync(LogLevel.Information, "[SearchSpring] No merchandising or content block found.");
                }

                return new SearchResultModel
                {
                    Results = productList,
                    PageNumber = currentPage,
                    PageSize = pageSize,
                    TotalResults = totalResults,
                    Facets = facets,
                    SortOptions = sortOptions,
                    BannersByPosition = bannersByPosition
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SearchSpring] JSON Deserialization failed: {ex.Message}");
                throw new Exception("Failed to parse Searchspring response.", ex);
            }
        }
    }
}
