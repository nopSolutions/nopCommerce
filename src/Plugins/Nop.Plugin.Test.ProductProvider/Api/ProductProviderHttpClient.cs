using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Test.ProductProvider.Api.Models;

namespace Nop.Plugin.Test.ProductProvider.Api
{
    public class ProductProviderHttpClient
    {
        protected readonly HttpClient _httpClient;

        public ProductProviderHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // public async Task<IEnumerable<int>> GetProducts(ProductProviderSettings settings)
        // {
        //     var uri = $"{settings.BaseUrl}/{settings.ProductListEndpoint}";
        //     
        //     _httpClient.Timeout = TimeSpan.FromSeconds(10);
        //     _httpClient.DefaultRequestHeaders.Authorization =
        //         new AuthenticationHeaderValue(settings.ApiKeyType, settings.ApiKey);
        //     
        //     var httpResponse = await _httpClient.GetAsync(uri);
        //     var responseString = await httpResponse.Content.ReadAsStringAsync();
        //     
        //     return JsonConvert.DeserializeObject<IEnumerable<int>>(responseString);
        // }

        public async Task GetProductDetails(ProductProviderSettings settings, int id)
        {
            var uri = $"{settings.BaseUrl}/{settings.ProductDetailEndpoint}";

            var query = new Dictionary<string, string>()
            {
                ["id"] = id.ToString()
            };

            uri = QueryHelpers.AddQueryString(uri, query);

            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(settings.ApiKeyType, settings.ApiKey);

            var httpResponse = await _httpClient.GetAsync(uri);
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            
            var af = 4;
        }
    }
}
