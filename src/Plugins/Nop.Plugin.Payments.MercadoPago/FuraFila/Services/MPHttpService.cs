using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.MercadoPago.Exceptions;
using Nop.Plugin.Payments.MercadoPago.FuraFila.Preferences;
using Nop.Services.Logging;

namespace FuraFila.Payments.MercadoPago.Services
{
    public class MPHttpService
    {
        private readonly HttpClient _client;

        private const string MP_URI = "https://api.mercadopago.com/";

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public MPHttpService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<PreferenceResponse> CreatePaymentPreference(PreferenceRequest rq, string accessToken, CancellationToken cancellationToken = default)
        {
            string jContent = JsonConvert.SerializeObject(rq, _settings);
            var content = new StringContent(jContent);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/checkout/preferences?access_token={accessToken}"))
            {
                request.Content = content;
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = await _client.SendAsync(request, cancellationToken))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var jResult = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<PreferenceResponse>(jResult);

                        return result;
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var jResult = await response.Content.ReadAsStringAsync();
                        var logger = EngineContext.Current.Resolve<ILogger>();

                        logger.Error($"PaymentService.Register({rq}) - error {jResult}");

                        throw new CreatePaymentPreferenceHttpException();
                    }
                    else
                    {
                        throw new CreatePaymentPreferenceHttpException();
                    }
                }
            }
        }
    }
}
