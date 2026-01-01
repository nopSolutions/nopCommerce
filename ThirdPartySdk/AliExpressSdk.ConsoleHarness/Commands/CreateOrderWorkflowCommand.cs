using System.Text.Json;
using Microsoft.Extensions.Options;
using AliExpressSdk.Clients;
using AliExpressSdk.ConsoleHarness.Configuration;
using AliExpressSdk.ConsoleHarness.Services;

namespace AliExpressSdk.ConsoleHarness.Commands;

/// <summary>
/// Handles full order workflow: search -> product details -> freight estimation -> order creation
/// Configured for South Africa (ZA) with ZAR currency and specific shipping address.
/// </summary>
public class CreateOrderWorkflowCommand
{
    private readonly AliExpressOptions _options;
    private readonly ApiCallPersistence _persistence;

    // South Africa shipping configuration
    private const string CountryCode = "ZA";
    private const string Currency = "ZAR";
    private const string ShippingAddress = "22 Guildford Road";
    private const string ShippingCity = "Cape Town";
    private const string ShippingProvince = "Western Cape";
    private const string ShippingZip = "7441";
    private const string ShippingSuburb = "Parklands";

    public CreateOrderWorkflowCommand(
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

        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("AliExpress Order Creation Workflow");
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine($"Search keyword: {searchKeyword}");
        Console.WriteLine($"Ship to: {CountryCode} ({Currency})");
        Console.WriteLine($"Address: {ShippingAddress}, {ShippingSuburb}, {ShippingCity}, {ShippingProvince} {ShippingZip}");
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine();

        // Step 1: Search for products
        Console.WriteLine("Step 1: Searching for products...");
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

        Console.WriteLine($"✓ Found product ID: {productId}");
        Console.WriteLine();

        // Step 3: Get product details
        Console.WriteLine("Step 2: Getting product details...");
        var productResult = await GetProductDetails(productId);
        if (!productResult.Success)
        {
            return 1;
        }

        // Extract SKU information
        var (skuId, skuAttr) = ExtractFirstSku(productResult.Data);
        if (string.IsNullOrWhiteSpace(skuId))
        {
            Console.Error.WriteLine("No SKU found for the product.");
            return 1;
        }

        Console.WriteLine($"✓ Found SKU ID: {skuId}");
        if (!string.IsNullOrWhiteSpace(skuAttr))
        {
            Console.WriteLine($"  SKU Attributes: {skuAttr}");
        }
        Console.WriteLine();

        // Step 4: Get freight estimation
        Console.WriteLine("Step 3: Getting freight estimation...");
        var freightResult = await GetFreightEstimation(productId, skuId);
        if (!freightResult.Success)
        {
            return 1;
        }

        // Extract logistics service name
        var logisticsServiceName = ExtractLogisticsServiceName(freightResult.Data);
        Console.WriteLine($"✓ Logistics service: {logisticsServiceName ?? "Default"}");
        Console.WriteLine();

        // Step 5: Create order
        Console.WriteLine("Step 4: Creating order...");
        var orderResult = await CreateOrder(productId, skuAttr, logisticsServiceName);
        if (!orderResult.Success)
        {
            return 1;
        }

        Console.WriteLine();
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("✓ Order workflow completed successfully!");
        Console.WriteLine("=".PadRight(80, '='));
        
        return 0;
    }

    private async Task<(bool Success, JsonElement Data)> SearchProducts(string keyword)
    {
        var client = new AEBaseClient(_options.AppKey, _options.AppSecret, _options.Session);
        
        var parameters = new Dictionary<string, string>
        {
            ["keyWord"] = keyword,
            ["local"] = "en_US",
            ["countryCode"] = CountryCode,
            ["currency"] = Currency,
            ["pageSize"] = "10",
            ["pageIndex"] = "1"
        };

        await _persistence.SaveRequest("aliexpress.ds.text.search", parameters);
        
        var result = await client.CallApiDirectly("aliexpress.ds.text.search", parameters);

        if (result.Ok && result.Data is { } data)
        {
            await _persistence.SaveResponse("aliexpress.ds.text.search", data);
            await SaveToSampleResponses("product-search", parameters, data);
            
            Console.WriteLine("  Search request succeeded.");
            return (true, data);
        }

        Console.Error.WriteLine("  Search request failed.");
        if (!string.IsNullOrWhiteSpace(result.Message))
        {
            Console.Error.WriteLine($"  Message: {result.Message}");
        }

        if (result.ErrorResponse is { } error)
        {
            Console.Error.WriteLine("  AliExpress error response:");
            Console.Error.WriteLine("  " + JsonSerializer.Serialize(error, new JsonSerializerOptions { WriteIndented = true }));
        }

        return (false, default);
    }

    private string? ExtractFirstProductId(JsonElement searchData)
    {
        try
        {
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
            Console.Error.WriteLine($"  Error extracting product ID: {ex.Message}");
        }

        return null;
    }

    private async Task<(bool Success, JsonElement Data)> GetProductDetails(string productId)
    {
        var client = new AEBaseClient(_options.AppKey, _options.AppSecret, _options.Session);
        
        var parameters = new Dictionary<string, string>
        {
            ["product_id"] = productId,
            ["ship_to_country"] = CountryCode,
            ["target_currency"] = Currency,
            ["target_language"] = "en"
        };

        await _persistence.SaveRequest("aliexpress.ds.product.get", parameters);
        
        var result = await client.CallApiDirectly("aliexpress.ds.product.get", parameters);

        if (result.Ok && result.Data is { } data)
        {
            await _persistence.SaveResponse("aliexpress.ds.product.get", data);
            await SaveToSampleResponses("product-details", parameters, data);
            
            Console.WriteLine("  Product details request succeeded.");
            return (true, data);
        }

        Console.Error.WriteLine("  Product details request failed.");
        if (!string.IsNullOrWhiteSpace(result.Message))
        {
            Console.Error.WriteLine($"  Message: {result.Message}");
        }

        if (result.ErrorResponse is { } error)
        {
            Console.Error.WriteLine("  AliExpress error response:");
            Console.Error.WriteLine("  " + JsonSerializer.Serialize(error, new JsonSerializerOptions { WriteIndented = true }));
        }

        return (false, default);
    }

    private (string? SkuId, string? SkuAttr) ExtractFirstSku(JsonElement productData)
    {
        try
        {
            if (productData.TryGetProperty("aliexpress_ds_product_get_response", out var response))
            {
                if (response.TryGetProperty("result", out var result))
                {
                    if (result.TryGetProperty("ae_item_sku_info_dtos", out var skuInfos))
                    {
                        if (skuInfos.TryGetProperty("ae_item_sku_info_d_t_o", out var skuArray))
                        {
                            if (skuArray.ValueKind == JsonValueKind.Array && skuArray.GetArrayLength() > 0)
                            {
                                var firstSku = skuArray[0];
                                var skuId = firstSku.TryGetProperty("sku_id", out var skuIdElement) 
                                    ? skuIdElement.ToString() 
                                    : null;
                                var skuAttr = firstSku.TryGetProperty("sku_attr", out var skuAttrElement) 
                                    ? skuAttrElement.ToString() 
                                    : null;
                                return (skuId, skuAttr);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"  Error extracting SKU: {ex.Message}");
        }

        return (null, null);
    }

    private async Task<(bool Success, JsonElement Data)> GetFreightEstimation(string productId, string skuId)
    {
        var client = new AEBaseClient(_options.AppKey, _options.AppSecret, _options.Session);
        
        var queryDeliveryReq = new
        {
            quantity = 1,
            shipToCountry = CountryCode,
            productId = productId,
            provinceCode = ShippingProvince,
            cityCode = ShippingCity,
            selectedSkuId = skuId,
            language = "en_US",
            currency = Currency,
            locale = "en_US"
        };
        
        var parameters = new Dictionary<string, string>
        {
            ["queryDeliveryReq"] = JsonSerializer.Serialize(queryDeliveryReq, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        };

        await _persistence.SaveRequest("aliexpress.ds.freight.query", parameters);
        
        var result = await client.CallApiDirectly("aliexpress.ds.freight.query", parameters);

        if (result.Ok && result.Data is { } data)
        {
            await _persistence.SaveResponse("aliexpress.ds.freight.query", data);
            await SaveToSampleResponses("freight-estimation", parameters, data);
            
            Console.WriteLine("  Freight estimation request succeeded.");
            return (true, data);
        }

        Console.Error.WriteLine("  Freight estimation request failed.");
        if (!string.IsNullOrWhiteSpace(result.Message))
        {
            Console.Error.WriteLine($"  Message: {result.Message}");
        }

        if (result.ErrorResponse is { } error)
        {
            Console.Error.WriteLine("  AliExpress error response:");
            Console.Error.WriteLine("  " + JsonSerializer.Serialize(error, new JsonSerializerOptions { WriteIndented = true }));
        }

        return (false, default);
    }

    private string? ExtractLogisticsServiceName(JsonElement freightData)
    {
        try
        {
            if (freightData.TryGetProperty("aliexpress_ds_freight_query_response", out var response))
            {
                if (response.TryGetProperty("result", out var result))
                {
                    if (result.TryGetProperty("delivery_options", out var deliveryOptions))
                    {
                        if (deliveryOptions.TryGetProperty("delivery_option_d_t_o", out var optionsArray))
                        {
                            if (optionsArray.ValueKind == JsonValueKind.Array && optionsArray.GetArrayLength() > 0)
                            {
                                var firstOption = optionsArray[0];
                                if (firstOption.TryGetProperty("code", out var codeElement))
                                {
                                    return codeElement.ToString();
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"  Error extracting logistics service: {ex.Message}");
        }

        return null;
    }

    private async Task<(bool Success, JsonElement Data)> CreateOrder(string productId, string? skuAttr, string? logisticsServiceName)
    {
        var client = new AEBaseClient(_options.AppKey, _options.AppSecret, _options.Session);
        
        var dsExtendRequest = new
        {
            payment = new
            {
                pay_currency = Currency
            }
        };
        
        var placeOrderRequest = new
        {
            out_order_id = Guid.NewGuid().ToString(),
            logistics_address = new
            {
                address = ShippingAddress,
                city = ShippingCity,
                contact_person = "AliExpress Test",
                country = CountryCode,
                full_name = "Test Customer",
                mobile_no = "0211234567",
                phone_country = "+27",
                phone_number = "0211234567",
                province = ShippingProvince,
                zip = ShippingZip
            },
            product_items = new[]
            {
                new
                {
                    product_id = long.Parse(productId),
                    product_count = 1,
                    sku_attr = skuAttr ?? "",
                    logistics_service_name = logisticsServiceName ?? ""
                }
            }
        };
        
        var parameters = new Dictionary<string, string>
        {
            ["ds_extend_request"] = JsonSerializer.Serialize(dsExtendRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            }),
            ["param_place_order_request4_open_api_d_t_o"] = JsonSerializer.Serialize(placeOrderRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            })
        };

        await _persistence.SaveRequest("aliexpress.ds.order.create", parameters);
        
        var result = await client.CallApiDirectly("aliexpress.ds.order.create", parameters);

        if (result.Ok && result.Data is { } data)
        {
            await _persistence.SaveResponse("aliexpress.ds.order.create", data);
            await SaveToSampleResponses("order-create", parameters, data);
            
            Console.WriteLine("  Order creation request succeeded.");
            Console.WriteLine("  Response:");
            Console.WriteLine("  " + JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
            
            return (true, data);
        }

        Console.Error.WriteLine("  Order creation request failed.");
        if (!string.IsNullOrWhiteSpace(result.Message))
        {
            Console.Error.WriteLine($"  Message: {result.Message}");
        }

        if (result.ErrorResponse is { } error)
        {
            Console.Error.WriteLine("  AliExpress error response:");
            Console.Error.WriteLine("  " + JsonSerializer.Serialize(error, new JsonSerializerOptions { WriteIndented = true }));
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
