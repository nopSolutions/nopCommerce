using System.Threading.Tasks;
using Nop.Services.Common;
using Nop.Core;
using System.Net.Http;
using System.Text.Json;
using AbcWarehouse.Plugin.Widgets.UniFi;
using System.Net;
using AbcWarehouse.Plugin.Payments.UniFi.Models;
using Newtonsoft.Json;

namespace AbcWarehouse.Plugin.Payments.UniFi.Services
{
    public class TransactionLookupService : ITransactionLookupService
    {
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;
        private readonly UniFiPaymentsSettings _settings;
        private readonly UniFiSettings _uniFiSettings;

        public TransactionLookupService(
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
            UniFiPaymentsSettings settings,
            UniFiSettings uniFiSettings
        )
        {
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
            _settings = settings;
            _uniFiSettings = uniFiSettings;
        }

        public async Task<TransactionLookupResponse> TransactionLookupAsync(string transactionToken)
        {
            var httpClient = new HttpClient();
            var bearerToken = await _genericAttributeService.GetAttributeAsync<string>(
                await _workContext.GetCurrentCustomerAsync(),
                "UniFiBearerToken");
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken);
            httpClient.DefaultRequestHeaders.Add("X-SYF-ChannelId", "DY");

            var transactionLookupEndpoint = _settings.UseIntegration ?
                $"https://api-stg.syf.com/v1/dpos/utility/lookup/transaction/{transactionToken}?lookupType=PARTNERID&lookupId={_uniFiSettings.PartnerId}" :
                $"https://api.syf.com/v1/dpos/utility/lookup/transaction/{transactionToken}?lookupType=PARTNERID&lookupId={_uniFiSettings.PartnerId}";
            var response = await httpClient.GetAsync(transactionLookupEndpoint);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new NopException("Payments.UniFi: Failure to perform transaction lookup.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TransactionLookupResponse>(responseContent);
        }
    }
}