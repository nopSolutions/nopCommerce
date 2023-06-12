using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nop.Core.Configuration;
using Nop.Plugin.Test.ProductProvider.Exceptions;
using Nop.Plugin.Test.ProductProvider.Models;
using Nop.Services.Configuration;

namespace Nop.Plugin.Test.ProductProvider.Api
{
    public class ProductProviderHttpClient
    {
        protected readonly HttpClient _httpClient;
        private readonly ISettingService _settingService;
        private readonly ILogger<ProductProviderHttpClient> _logger;
        private bool _isCredentialsSet = false;
            
        public ProductProviderHttpClient(HttpClient httpClient, ISettingService settingService, ILogger<ProductProviderHttpClient> logger)
        {
            _httpClient = httpClient;
            _settingService = settingService;
            _logger = logger;
        }

        public async Task<IEnumerable<int>> GetProductsAsync()
        {
            try
            {
                if (!_isCredentialsSet)
                    await SetCredentials();

                var settings = await _settingService.LoadSettingAsync<ProductProviderSettings>();
                var url = $"{settings.BaseUrl}/{settings.ProductListEndpoint}";
                var httpResponse = await _httpClient.GetAsync(url);
                var responseString = await httpResponse.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<IEnumerable<int>>(responseString);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Couldn't get external products");
                throw;
            }
        }

        public async Task<ExternalProductModel> GetProductDetailsAsync(int id)
        {
            try
            {
                if (!_isCredentialsSet)
                    await SetCredentials();
                
                var settings = await _settingService.LoadSettingAsync<ProductProviderSettings>();
                var url = $"{settings.BaseUrl}/{settings.ProductDetailEndpoint}/{id}";
                var httpResponse = await _httpClient.GetAsync(url);

                if (httpResponse == null)
                    throw new ExternalProductNotFoundException();
                    
                var responseString = await httpResponse.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ExternalProductModel>(responseString);
            }
            catch(Exception)
            {
                _logger.LogWarning("Couldn't get external product by id");
                throw;
            }
        }

        private async Task SetCredentials()
        {
            var settings = await _settingService.LoadSettingAsync<ProductProviderSettings>();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(ProductProviderDefaults.ApiKeyType, settings.ApiKey);
            
            _isCredentialsSet = true;
        }
    }
}
