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

        public async Task<string> RequestAsync(string uri, ProductProviderSettings settings)
        {

            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(settings.ApiKeyType, settings.ApiKey);

            var httpResponse = await _httpClient.GetAsync(uri);
            var responseString = await httpResponse.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}
