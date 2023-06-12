using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core.Configuration;
using Nop.Plugin.Test.ProductProvider.Models;
using Nop.Services.Configuration;

namespace Nop.Plugin.Test.ProductProvider.Api
{
    public class ProductProviderHttpClient
    {
        protected readonly HttpClient _httpClient;
        private readonly ISettingService _settingService;

        public ProductProviderHttpClient(HttpClient httpClient, ISettingService settingService)
        {
            _httpClient = httpClient;
            _settingService = settingService;
        }

        public async Task<IEnumerable<int>> GetProductsAsync()
        {
            var settings = await _settingService.LoadSettingAsync<ProductProviderSettings>();
            
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(settings.ApiKeyType, settings.ApiKey);

            var url = $"{settings.BaseUrl}/{settings.ProductListEndpoint}";
            
            var httpResponse = await _httpClient.GetAsync(url);
            var responseString = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<int>>(responseString);
        }

        public async Task<ExternalProductModel> GetProductDetailsAsync(int id)
        {
            var settings = await _settingService.LoadSettingAsync<ProductProviderSettings>();

            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(settings.ApiKeyType, settings.ApiKey);

            var url = $"{settings.BaseUrl}/{settings.ProductDetailEndpoint}/{id}";

            var httpResponse = await _httpClient.GetAsync(url);
            var responseString = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ExternalProductModel>(responseString);
        }
    }
}
