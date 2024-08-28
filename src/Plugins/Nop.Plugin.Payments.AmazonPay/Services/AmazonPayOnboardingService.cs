extern alias Crypto;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Plugin.Payments.AmazonPay.Domain.Onboarding;
using Nop.Plugin.Payments.AmazonPay.Enums;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using Crypto.Org.BouncyCastle.Crypto;
using Crypto.Org.BouncyCastle.Crypto.Parameters;
using Crypto.Org.BouncyCastle.OpenSsl;
using Crypto.Org.BouncyCastle.Security;

namespace Nop.Plugin.Payments.AmazonPay.Services;

/// <summary>
/// Represents the service to onboard merchants
/// </summary>
public class AmazonPayOnboardingService
{
    #region Fields

    private readonly AmazonPayApiService _amazonPayApiService;
    private readonly AmazonPaySettings _amazonPaySettings;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger _logger;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly IStoreService _storeService;
    private readonly IWebHelper _webHelper;
    private readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public AmazonPayOnboardingService(AmazonPayApiService amazonPayApiService,
        AmazonPaySettings amazonPaySettings,
        IHttpContextAccessor httpContextAccessor,
        ILogger logger,
        INopUrlHelper nopUrlHelper,
        ISettingService settingService,
        IStoreContext storeContext,
        IStoreService storeService,
        IWebHelper webHelper,
        IWorkContext workContext)
    {
        _amazonPayApiService = amazonPayApiService;
        _amazonPaySettings = amazonPaySettings;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _nopUrlHelper = nopUrlHelper;
        _settingService = settingService;
        _storeContext = storeContext;
        _storeService = storeService;
        _webHelper = webHelper;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Generate RSA private/public key pair
    /// </summary>
    /// <returns>Private key; public key</returns>
    private static (string PrivateKey, string PublicKey) GenerateKeys()
    {
        var rsa = RSA.Create(2048);

        //SPKI is expected by Amazon service
        var publicKey = rsa.ExportSubjectPublicKeyInfoPem();
        var privateKey = rsa.ExportRSAPrivateKeyPem();

        return (privateKey, publicKey);
    }

    /// <summary>
    /// Decrypt signature with RSA algorithm
    /// </summary>
    /// <param name="signature">Signature</param>
    /// <param name="privateKey">Private key</param>
    /// <returns>Decrypted text</returns>
    private static string Decrypt(string signature, string privateKey)
    {
        using var reader = new StringReader(privateKey);
        var pemReader = new PemReader(reader);
        var rsaPrivateKey = (AsymmetricCipherKeyPair)pemReader.ReadObject();
        var rsaParameters = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)rsaPrivateKey.Private);
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(rsaParameters);

        var signatureBytes = Convert.FromBase64String(signature);
        var bytes = rsa.Decrypt(signatureBytes, false);
        var value = Encoding.Default.GetString(bytes);

        return value;
    }

    /// <summary>
    /// Encrypt text with RSA algorithm
    /// </summary>
    /// <param name="value">Text value</param>
    /// <param name="publicKey">Public key</param>
    /// <returns>Encrypted value</returns>
    private static string Encrypt(string value, string publicKey)
    {
        using var reader = new StringReader(publicKey);
        var pemReader = new PemReader(reader);
        var rsaPublicKey = (AsymmetricKeyParameter)pemReader.ReadObject();
        var rsaParameters = DotNetUtilities.ToRSAParameters((RsaKeyParameters)rsaPublicKey);
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(rsaParameters);

        var bytes = Encoding.Default.GetBytes(value);
        var signatureBytes = rsa.Encrypt(bytes, false);
        var signature = Convert.ToBase64String(signatureBytes);

        return signature;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Register merchant account
    /// </summary>
    /// <param name="paymentRegion">Payment region</param>
    /// <param name="amazonPaySettings">Amazon Pay settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the keys to encrypt credentials
    /// </returns>
    public async Task<(string PrivateKey, string PublicKey)> RegisterAsync(PaymentRegion paymentRegion, AmazonPaySettings amazonPaySettings)
    {
        try
        {
            //currently, automated key sharing is applicable only in the North American payment region
            var post = new RemotePost(_httpContextAccessor, _webHelper)
            {
                FormName = AmazonPayDefaults.Onboarding.FormName,
                Url = paymentRegion == PaymentRegion.Us
                    ? AmazonPayDefaults.Onboarding.RegisterUrl.Us
                    : AmazonPayDefaults.Onboarding.RegisterUrl.Eu,
                Method = HttpMethods.Post,
                NewInputForEachValue = true
            };

            //generate new keys if not exist
            var (privateKey, publicKey) = string.IsNullOrEmpty(amazonPaySettings.PrivateKey) && string.IsNullOrEmpty(amazonPaySettings.PublicKey)
                ? GenerateKeys()
                : (amazonPaySettings.PrivateKey, amazonPaySettings.PublicKey);

            //public key is PEM-encoded initially, we should remove PEM labels
            post.Add("publicKey", publicKey
                .Replace("-----BEGIN PUBLIC KEY-----", string.Empty)
                .Replace("-----END PUBLIC KEY-----", string.Empty)
                .Replace("\n", string.Empty));

            post.Add("keyShareURL", _amazonPayApiService
                .GetUrl(AmazonPayDefaults.Onboarding.KeyShareRouteName)
                .Replace($"{Uri.UriSchemeHttp}{Uri.SchemeDelimiter}", $"{Uri.UriSchemeHttps}{Uri.SchemeDelimiter}"));

            foreach (var store in await _storeService.GetAllStoresAsync())
            {
                if (!string.IsNullOrEmpty(store.Url) && !store.Url.Contains("localhost"))
                {
                    post.Add("merchantLoginDomains[]", store.Url.TrimEnd('/'));
                    //post.Add("merchantLoginRedirectURLs[]", ""); //not used
                }
            }
            post.Add("merchantStoreDescription", CommonHelper.EnsureMaximumLength((await _storeContext.GetCurrentStoreAsync()).Name, 2048));
            //post.Add("merchantCountry", ""); //not yet processed

            post.Add("spId", AmazonPayDefaults.SpId);
            post.Add("onboardingVersion", "2");
            post.Add("locale", (await _workContext.GetWorkingLanguageAsync()).LanguageCulture?.Replace('-', '_'));
            post.Add("spSoftwareVersion", NopVersion.FULL_VERSION);
            post.Add("spAmazonPluginVersion", AmazonPayDefaults.PluginVersion);
            post.Add("source", "SPPL"); //SPPL – used when coming from the configuration section of the platform admin panel
                                        //post.Add("ld", ""); //no ld code is provided
            post.Add("merchantPrivacyNoticeURL", await _nopUrlHelper.RouteTopicUrlAsync("privacyinfo", _webHelper.GetCurrentRequestProtocol()));

            var ipnUrl = _amazonPayApiService.GetUrl(AmazonPayDefaults.IPNHandlerRouteName);
            post.Add("merchantSandboxIPNURL", ipnUrl);
            post.Add("merchantProductionIPNURL", ipnUrl);

            post.Post();

            return (privateKey, publicKey);
        }
        catch (Exception exception)
        {
            var logMessage = $"{AmazonPayDefaults.PluginSystemName} error:{System.Environment.NewLine}{exception.Message}";
            await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());

            return (null, null);
        }
    }

    /// <summary>
    /// Auto key sharing
    /// </summary>
    /// <param name="payload">Request payload</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the error if exists
    /// </returns>
    public async Task<string> AutomaticKeyExchangeAsync(string payload)
    {
        try
        {
            if (_amazonPaySettings.EnableLogging)
            {
                var logMessage = $"{AmazonPayDefaults.PluginSystemName} key exchange request payload:{System.Environment.NewLine}{payload}";
                await _logger.InsertLogAsync(LogLevel.Debug, $"{AmazonPayDefaults.PluginSystemName} key exchange request payload", logMessage);
            }

            if (string.IsNullOrEmpty(payload))
                throw new NopException("Invalid payload");

            //decode payload
            payload = WebUtility.UrlDecode(payload);

            if (_amazonPaySettings.EnableLogging)
            {
                var logMessage = $"{AmazonPayDefaults.PluginSystemName} URL decoded payload:{System.Environment.NewLine}{payload}";
                await _logger.InsertLogAsync(LogLevel.Debug, $"{AmazonPayDefaults.PluginSystemName} URL decoded payload", logMessage);
            }

            //try to get credentials from payload
            var (credentials, error) = await PrepareCredentialsAsync(payload, _amazonPaySettings.PrivateKey, true);
            if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(credentials?.PublicKeyId))
                throw new NopException("Invalid payload");

            _amazonPaySettings.MerchantId = credentials.MerchantId;
            _amazonPaySettings.StoreId = credentials.StoreId;
            _amazonPaySettings.PublicKeyId = credentials.PublicKeyId;
            _amazonPaySettings.PublicKey = null;

            //save credentials in the settings for the current store
            var store = await _storeContext.GetCurrentStoreAsync();
            await _settingService.SaveSettingOverridablePerStoreAsync(_amazonPaySettings, settings => settings.MerchantId, true, store.Id, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(_amazonPaySettings, settings => settings.StoreId, true, store.Id, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(_amazonPaySettings, settings => settings.PublicKeyId, true, store.Id, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(_amazonPaySettings, settings => settings.PublicKey, true, store.Id, false);

            //save credentials in the shared settings if not exist
            var settings = await _settingService.LoadSettingAsync<AmazonPaySettings>();
            if (string.IsNullOrEmpty(settings.MerchantId) || !string.IsNullOrEmpty(settings.StoreId) || !string.IsNullOrEmpty(settings.PublicKeyId))
                await _settingService.SaveSettingAsync(_amazonPaySettings);
            else
                await _settingService.ClearCacheAsync();

            return null;
        }
        catch (Exception exception)
        {
            await _logger.ErrorAsync($"{AmazonPayDefaults.PluginSystemName} key exchange error", exception);
            return exception.Message;
        }
    }

    /// <summary>
    /// Prepare account credentials
    /// </summary>
    /// <param name="payload">Payload</param>
    /// <param name="privateKey">Private key to decrypt payload</param>
    /// <param name="autoExchange">Whether this is an automatic key exchange</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the credentials; error if exists
    /// </returns>
    public async Task<(Credentials Credentials, string Error)> PrepareCredentialsAsync(string payload, string privateKey, bool autoExchange = false)
    {
        try
        {
            var credentials = autoExchange
                ? JsonConvert.DeserializeObject<Credentials>(payload)
                : PlainCredentials.ToCredentials(JsonConvert.DeserializeObject<PlainCredentials>(payload));

            //decode public key identifier
            if (autoExchange)
            {
                if (_amazonPaySettings.EnableLogging && !string.IsNullOrEmpty(credentials.PublicKeyId))
                {
                    var logMessage = $"{AmazonPayDefaults.PluginSystemName} encrypted public key id:{System.Environment.NewLine}{credentials.PublicKeyId}";
                    await _logger.InsertLogAsync(LogLevel.Debug, $"{AmazonPayDefaults.PluginSystemName} encrypted public key id", logMessage);
                }

                credentials.PublicKeyId = Decrypt(credentials.PublicKeyId, privateKey);

                if (_amazonPaySettings.EnableLogging && !string.IsNullOrEmpty(credentials.PublicKeyId))
                {
                    var logMessage = $"{AmazonPayDefaults.PluginSystemName} decrypted public key id:{System.Environment.NewLine}{credentials.PublicKeyId}";
                    await _logger.InsertLogAsync(LogLevel.Debug, $"{AmazonPayDefaults.PluginSystemName} decrypted public key id", logMessage);
                }
            }

            return (credentials, null);
        }
        catch (Exception exception)
        {
            await _logger.ErrorAsync($"{AmazonPayDefaults.PluginSystemName} decrypting error", exception);
            return (null, exception.Message);
        }
    }

    #endregion
}