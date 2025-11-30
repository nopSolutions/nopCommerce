using System.Configuration;
using Nop.Core.Configuration;
using Nop.Services.Configuration;

namespace Nop.Plugin.Shipping.CourierGuy
{
    public class CourierGuySettings : ISettings
    {
        public Uri BaseUrl { get; set; }

        public string ApiKey { get; set; }

        public bool UseSandbox { get; set; }

        public string SandBoxApiKey { get; set; }

        public Uri TrackingUri { get; set; }

        public Uri RateRequestUri { get; set; }

        public Uri ShipmentRequestUri { get; set; }

        public string PushoverApiKey { get; set; } = "asz7kc48pkkivvf8uo8drfodeowb37";

        public string PushoverUserKey { get; set; } = "grxswvwt4yi1mqtmkvuxk71pndyt4t";


    }

    public class CourierGuyHttpClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISettingService _settingService;
        private CourierGuySettings _settings;

        public CourierGuyHttpClientFactory(
            IHttpClientFactory httpClientFactory,
            ISettingService settingService)
        {
            _httpClientFactory = httpClientFactory;
            _settingService = settingService;
            _settings = _settingService.LoadSettingAsync<CourierGuySettings>()
                .GetAwaiter()
                .GetResult();
        }

        private async Task<HttpClient> CreateHttpClient(string apiKey, Uri requestUri)
        {
            var client = _httpClientFactory.CreateClient();
            var clientBaseAddress = requestUri;
            client.BaseAddress = clientBaseAddress;
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }

        public Task<HttpClient> RateRequestHttpClient() => CreateHttpClient(_settings.ApiKey, _settings.RateRequestUri);

        public Task<HttpClient> ShipmentRequestHttpClient() => CreateHttpClient(_settings.ApiKey, _settings.ShipmentRequestUri);

        public Task<HttpClient> TrackingHttpClient() => CreateHttpClient(_settings.ApiKey, _settings.TrackingUri);

        public Task<HttpClient> SandBoxRateRequestHttpClient() => CreateHttpClient(_settings.SandBoxApiKey, _settings.RateRequestUri);

        public Task<HttpClient> SandBoxShipmentRequestHttpClient() => CreateHttpClient(_settings.SandBoxApiKey, _settings.ShipmentRequestUri);

        public Task<HttpClient> SandBoxTrackingHttpClient() => CreateHttpClient(_settings.SandBoxApiKey, _settings.TrackingUri);
    }
}