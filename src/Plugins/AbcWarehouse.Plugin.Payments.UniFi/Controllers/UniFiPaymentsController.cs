using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Core;
using AbcWarehouse.Plugin.Payments.UniFi.Models;
using System.Net.Http;
using AbcWarehouse.Plugin.Widgets.UniFi;
using Nop.Services.Common;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace AbcWarehouse.Plugin.Payments.UniFi.Controllers
{
    public class UniFiPaymentsController : BasePluginController
    {
        private readonly UniFiSettings _uniFiSettings;
        private readonly UniFiPaymentsSettings _settings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;

        public UniFiPaymentsController(
            UniFiSettings uniFiSettings,
            UniFiPaymentsSettings settings,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            ISettingService settingService,
            IWorkContext workContext)
        {
            _uniFiSettings = uniFiSettings;
            _settings = settings;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _settingService = settingService;
            _workContext = workContext;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [AutoValidateAntiforgeryToken]
        public ActionResult Configure()
        {
            return View(
                "~/Plugins/Payments.UniFi/Views/Configure.cshtml",
                _settings.ToModel());
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> Configure(ConfigModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            await _settingService.SaveSettingAsync(UniFiPaymentsSettings.FromModel(model));

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return Configure();
        }

        public async Task<ActionResult> TransactionLookup(string transactionToken)
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
            var responseJson = JsonDocument.Parse(responseContent);
            var transactionMessage = responseJson.RootElement
                                                 .GetProperty("creditAuthorizationInfo")
                                                 .GetProperty("transactionMessage").GetString();

            return Ok(new
            {
                transactionMessage = transactionMessage
            });
        }
    }
}

