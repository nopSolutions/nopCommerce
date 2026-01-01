using System.Text.Json;
using Microsoft.Extensions.Options;
using AliExpressSdk.Clients;
using AliExpressSdk.ConsoleHarness.Configuration;
using AliExpressSdk.ConsoleHarness.Services;

namespace AliExpressSdk.ConsoleHarness.Commands;

/// <summary>
/// Handles product search workflow: search for products and retrieve details.
/// </summary>
public class ProductSearchCommand
{
    private readonly AliExpressOptions _options;
    private readonly ApiCallPersistence _persistence;

    public ProductSearchCommand(
        IOptions<AliExpressOptions> options,
        ApiCallPersistence persistence)
    {
        _options = options.Value;
        _persistence = persistence;
    }

    public async Task<int> ExecuteAsync(string searchKeyword)
    {
        if (string.IsNullOrWhiteSpace(_options.Session))
        {
            Console.Error.WriteLine("Session token is required. Set AE_SESSION environment variable or configure in appsettings.");
            return 1;
        }

        if (string.IsNullOrWhiteSpace(searchKeyword))
        {
            Console.Error.WriteLine("Search keyword is required.");
            return 1;
        }

        Console.WriteLine($"Searching for: {searchKeyword}");
        Console.WriteLine();

        // Step 1: Search for products
        var searchResult = await SearchProducts(searchKeyword);
        if (!searchResult.Success)
        {
            return 1;
        }

        // Step 2: Extract first product
        var productId = ExtractFirstProductId(searchResult.Data);
        if (string.IsNullOrWhiteSpace(productId))
        {
            Console.Error.WriteLine("No products found in search results.");
            return 1;
        }

        Console.WriteLine($"Found product ID: {productId}");
        Console.WriteLine();

        // Step 3: Get product details
        var productResult = await GetProductDetails(productId);
        if (!productResult.Success)
        {
            return 1;
        }

        Console.WriteLine("Product search workflow completed successfully!");
        return 0;
    }

    private async Task<(bool Success, JsonElement Data)> SearchProducts(string keyword)
    {
        Console.WriteLine("Calling aliexpress.ds.text.search...");
        
        var client = new AEBaseClient(_options.AppKey, _options.AppSecret, _options.Session);
        
        var parameters = new Dictionary<string, string>
        {
            ["keyWord"] = keyword,
            ["local"] = "en_US",
            ["countryCode"] = "ZA",
            ["currency"] = "ZAR",
            ["pageSize"] = "10",
            ["pageIndex"] = "1"
        };

        await _persistence.SaveRequest("aliexpress.ds.text.search", parameters);
        
        var result = await client.CallApiDirectly("aliexpress.ds.text.search", parameters);

        if (result.Ok && result.Data is { } data)
        {
            await _persistence.SaveResponse("aliexpress.ds.text.search", data);
            
            // Also save to SampleResponses directory
            await SaveToSampleResponses("product-search", parameters, data);
            
            Console.WriteLine("Search request succeeded.");
            Console.WriteLine(JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
            Console.WriteLine();
            
            return (true, data);
        }

        Console.Error.WriteLine("Search request failed.");
        if (!string.IsNullOrWhiteSpace(result.Message))
        {
            Console.Error.WriteLine($"Message: {result.Message}");
        }

        if (result.ErrorResponse is { } error)
        {
            Console.Error.WriteLine("AliExpress error response:");
            Console.Error.WriteLine(JsonSerializer.Serialize(error, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }

        return (false, default);
    }

    private string? ExtractFirstProductId(JsonElement searchData)
    {
        try
        {
            // Navigate through the response structure
            if (searchData.TryGetProperty("aliexpress_ds_text_search_response", out var response))
            {
                if (response.TryGetProperty("data", out var dataObj))
                {
                    if (dataObj.TryGetProperty("products", out var productsObj))
                    {
                        if (productsObj.TryGetProperty("selection_search_product", out var products))
                        {
                            if (products.ValueKind == JsonValueKind.Array && products.GetArrayLength() > 0)
                            {
                                var firstProduct = products[0];
                                if (firstProduct.TryGetProperty("itemId", out var productIdElement))
                                {
                                    return productIdElement.ToString();
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error extracting product ID: {ex.Message}");
        }

        return null;
    }

    private async Task<(bool Success, JsonElement Data)> GetProductDetails(string productId)
    {
        Console.WriteLine($"Calling aliexpress.ds.product.get for product {productId}...");
        
        var client = new AEBaseClient(_options.AppKey, _options.AppSecret, _options.Session);
        
        var parameters = new Dictionary<string, string>
        {
            ["product_id"] = productId,
            ["ship_to_country"] = "ZA",
            ["target_currency"] = "ZAR",
            ["target_language"] = "en"
        };

        await _persistence.SaveRequest("aliexpress.ds.product.get", parameters);
        
        var result = await client.CallApiDirectly("aliexpress.ds.product.get", parameters);

        if (result.Ok && result.Data is { } data)
        {
            await _persistence.SaveResponse("aliexpress.ds.product.get", data);
            
            // Also save to SampleResponses directory
            await SaveToSampleResponses("product-details", parameters, data);
            
            Console.WriteLine("Product details request succeeded.");
            Console.WriteLine(JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
            Console.WriteLine();
            
            return (true, data);
        }

        Console.Error.WriteLine("Product details request failed.");
        if (!string.IsNullOrWhiteSpace(result.Message))
        {
            Console.Error.WriteLine($"Message: {result.Message}");
        }

        if (result.ErrorResponse is { } error)
        {
            Console.Error.WriteLine("AliExpress error response:");
            Console.Error.WriteLine(JsonSerializer.Serialize(error, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }

        return (false, default);
    }

    private async Task SaveToSampleResponses(string apiName, object request, object response)
    {
        var directory = Path.Combine("SampleResponses", apiName);
        Directory.CreateDirectory(directory);
        
        var requestPath = Path.Combine(directory, "request.json");
        var responsePath = Path.Combine(directory, "response.json");
        
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        await File.WriteAllTextAsync(requestPath, json);
        
        json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        await File.WriteAllTextAsync(responsePath, json);
    }
}
