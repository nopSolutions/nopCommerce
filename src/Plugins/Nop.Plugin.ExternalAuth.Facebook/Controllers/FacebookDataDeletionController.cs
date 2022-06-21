using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.ExternalAuth.Facebook.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;

namespace Nop.Plugin.ExternalAuth.Facebook.Controllers
{
    public class FacebookDataDeletionController : Controller
    {
        #region Fields

        private readonly FacebookExternalAuthSettings _facebookExternalAuthSettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public FacebookDataDeletionController(FacebookExternalAuthSettings facebookExternalAuthSettings,
            IActionContextAccessor actionContextAccessor,
            IExternalAuthenticationService externalAuthenticationService,
            ILocalizationService localizationService,
            ILogger logger,
            INotificationService notificationService,
            IUrlHelperFactory urlHelperFactory,
            IWebHelper webHelper)
        {
            _facebookExternalAuthSettings = facebookExternalAuthSettings;
            _actionContextAccessor = actionContextAccessor;
            _externalAuthenticationService = externalAuthenticationService;
            _localizationService = localizationService;
            _logger = logger;
            _notificationService = notificationService;
            _urlHelperFactory = urlHelperFactory;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        // Convert string to a valid Base64 encoded string
        protected static string DecodeUrlBase64(string str)
        {
            if(string.IsNullOrEmpty(str))
                return null;

            str = str.Replace("-", "+").Replace("_", "/");
            var paddingToAdd = (str.Length % 4) == 3 ? 1 : (str.Length % 4);
            var charToAdd = new string('=', paddingToAdd);

            return str += charToAdd;
        }

        #endregion

        #region Methods
        
        [HttpPost]
        public async Task<IActionResult> DataDeletionCallback(IFormCollection form)
        {
            try
            {
                string signed_request = form["signed_request"];

                if (!string.IsNullOrEmpty(signed_request))
                {
                    var split = signed_request.Split('.');

                    var signatureRaw = DecodeUrlBase64(split[0]);

                    if (string.IsNullOrEmpty(signatureRaw))
                        return null;

                    var dataRaw = DecodeUrlBase64(split[1]);

                    if (string.IsNullOrEmpty(dataRaw))
                        return null;

                    // the decoded signature
                    var signature = Convert.FromBase64String(signatureRaw);
                    var dataBuffer = Convert.FromBase64String(dataRaw);

                    // JSON object
                    var json = Encoding.UTF8.GetString(dataBuffer);

                    var appSecretBytes = Encoding.UTF8.GetBytes(_facebookExternalAuthSettings.ClientSecret);
                    HMAC hmac = new HMACSHA256(appSecretBytes);
                    var expectedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(split[1]));

                    if (!expectedHash.SequenceEqual(signature))
                    {
                        await _logger.ErrorAsync($"Facebook datadeletioncallback error: Hashing data disn't match. Data: {json}");
                        return null;
                    }

                    var fbUser = JsonConvert.DeserializeObject<FacebookUserDTO>(json);
                    var authenticationParameters = new ExternalAuthenticationParameters
                    {
                        ProviderSystemName = FacebookAuthenticationDefaults.SystemName,
                        AccessToken = await HttpContext.GetTokenAsync(FacebookDefaults.AuthenticationScheme, "access_token"),
                        ExternalIdentifier = fbUser.UserId
                    };

                    var externalAuthenticationRecord = await _externalAuthenticationService.GetExternalAuthenticationRecordByExternalAuthenticationParametersAsync(authenticationParameters);

                    if (externalAuthenticationRecord == null)
                        return null;

                    var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
                    var notificationUrl = urlHelper.RouteUrl(FacebookAuthenticationDefaults.DataDeletionStatusCheckRoute, new { earId = externalAuthenticationRecord.Id }, _webHelper.GetCurrentRequestProtocol());

                    await _logger.InformationAsync($"Facebook datadeletioncallback successful for CustomerId: {externalAuthenticationRecord.CustomerId}, CustomerEmail: {externalAuthenticationRecord.Email}, ExternalAuthenticationRecordId: {externalAuthenticationRecord.Id}, ExternalIdentifier: {externalAuthenticationRecord.ExternalIdentifier}");

                    await _externalAuthenticationService.DeleteExternalAuthenticationRecordAsync(externalAuthenticationRecord);

                    return Ok(new { url = notificationUrl, confirmation_code = $"{fbUser.UserId}" });
                }

                return null;

            }
            catch (Exception exception)
            {
                await _logger.ErrorAsync($"Facebook datadeletioncallback error: {exception.Message}.", exception);
            }
            return null;
        }

        #endregion

    }
}
