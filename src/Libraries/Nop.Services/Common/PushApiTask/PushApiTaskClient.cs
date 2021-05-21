using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Services.Common.PushApiTask;

namespace Expo.Server.Client
{
    public class PushApiTaskClient
    {
        //Environemt Configuration
        private const string EXPOBACKENDHOST= "https://exp.host";
        private const string PUSHSENDAPI = "/--/api/v2/push/send";
        private const string PUSHGETRECEIPTSPATH= "/--/api/v2/push/getReceipts";

        //Make this static to avoid socket saturation and limit concurrent server connections to 6, but only for instances of this class.
        private static readonly HttpClientHandler _httpHandler = new HttpClientHandler() { MaxConnectionsPerServer = 6 };
        private static readonly HttpClient _httpClient = new HttpClient(_httpHandler);
        static PushApiTaskClient()
        {
            _httpClient.BaseAddress = new Uri(EXPOBACKENDHOST);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<PushApiTaskTicketResponse> PushSendAsync(PushApiTaskTicketRequest pushTicketRequest)
        {
            var ticketResponse = await PostAsync<PushApiTaskTicketRequest, PushApiTaskTicketResponse>(pushTicketRequest, PUSHSENDAPI);
            return ticketResponse;
        }

        public async Task<PushApiTaskReceiptResponse> PushGetReceiptsAsync(PushApiTaskReceiptRequest pushReceiptRequest)
        {
            var receiptResponse = await PostAsync<PushApiTaskReceiptRequest, PushApiTaskReceiptResponse>( pushReceiptRequest, PUSHGETRECEIPTSPATH);
            return receiptResponse;
        }

        public async Task<U> PostAsync<T, U>(T requestObj, string path) where T : new()
        {

            var serializedRequestObj = JsonConvert.SerializeObject(requestObj, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var requestBody = new StringContent(serializedRequestObj, System.Text.Encoding.UTF8, "application/json");
            var responseBody = default(U);
            var response = await _httpClient.PostAsync(path, requestBody);

            if (response.IsSuccessStatusCode)
            {
                var rawResponseBody = await response.Content.ReadAsStringAsync();
                responseBody = JsonConvert.DeserializeObject<U>(rawResponseBody);
            }

            return responseBody;
        }
    }
}
