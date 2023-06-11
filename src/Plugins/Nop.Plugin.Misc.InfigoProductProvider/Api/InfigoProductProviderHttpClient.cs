using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nop.Plugin.Misc.InfigoProductProvider.Models;
using Nop.Services.Configuration;

namespace Nop.Plugin.Misc.InfigoProductProvider.Api;

public class InfigoProductProviderHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InfigoProductProviderHttpClient> _logger;
    private readonly ISettingService _settingService;

    public InfigoProductProviderHttpClient(HttpClient httpClient, ILogger<InfigoProductProviderHttpClient> logger, ISettingService settingService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _settingService = settingService;
    }

    public async Task<List<int>> RequestAllProductIds()
    {
        try
        {
            _logger.LogInformation("Performing the request");
            
            var settings = await _settingService.LoadSettingAsync<InfigoProductProviderConfiguration>();
            var url = settings.ApiBase + settings.ProductListUrl;
            var userName = settings.ApiUserName;
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", userName);

            var response = await _httpClient.GetAsync(url);

            var responseBody = await response.Content.ReadAsStringAsync();
            
            var productIdList = JsonConvert.DeserializeObject<List<int>>(responseBody);
        
            return productIdList;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load product id list");
            throw;
        }
    }

    public async Task<ApiProductModel> RequestProductById(int id)
    {
        try
        {
            var settings = await _settingService.LoadSettingAsync<InfigoProductProviderConfiguration>();
            var url = settings.ApiBase + settings.ProductDetailsUrl + $"?id={id}";
            var userName = settings.ApiUserName;
        
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", userName);
            
            var response = await _httpClient.GetAsync(url);

            var responseBody = await response.Content.ReadAsStringAsync();
            
            var product = JsonConvert.DeserializeObject<ApiProductModel>(responseBody);

            return product;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load product");
            throw;
        }
    } 
    
    public async Task<byte[]> GetPictureBinaryAsync(string url)
    {
        try
        {
            _logger.LogInformation("Performing the request");
            
            var response = await _httpClient.GetAsync(url);

            var imageBinary = await response.Content.ReadAsByteArrayAsync();

            return imageBinary;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load picture");
            throw;
        }
    }
}