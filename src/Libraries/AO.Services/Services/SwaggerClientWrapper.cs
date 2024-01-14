using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AO.Services.Services
{
    public sealed class SwaggerClientWrapper
    {        
        private static readonly Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(() =>
        {
            var httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 0, 30);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        });

        public static HttpClient Instance => _httpClient.Value;
    }
}
