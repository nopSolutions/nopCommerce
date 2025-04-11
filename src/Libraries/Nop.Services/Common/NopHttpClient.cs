using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Messages;
using Nop.Core.Rss;
using Nop.Services.Localization;
using Nop.Services.Messages;

namespace Nop.Services.Common;

/// <summary>
/// Represents the HTTP client to request nopCommerce official site
/// </summary>
public partial class NopHttpClient
{
    #region Fields

    protected readonly AdminAreaSettings _adminAreaSettings;
    protected readonly EmailAccountSettings _emailAccountSettings;
    protected readonly HttpClient _httpClient;
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILanguageService _languageService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public NopHttpClient(AdminAreaSettings adminAreaSettings,
        EmailAccountSettings emailAccountSettings,
        HttpClient client,
        IEmailAccountService emailAccountService,
        IHttpContextAccessor httpContextAccessor,
        ILanguageService languageService,
        IWebHelper webHelper,
        IWorkContext workContext)
    {
        //configure client
        client.BaseAddress = new Uri("https://www.nopcommerce.com/");
        client.Timeout = TimeSpan.FromSeconds(5);
        client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"nopCommerce-{NopVersion.CURRENT_VERSION}");

        _adminAreaSettings = adminAreaSettings;
        _emailAccountSettings = emailAccountSettings;
        _httpClient = client;
        _emailAccountService = emailAccountService;
        _httpContextAccessor = httpContextAccessor;
        _languageService = languageService;
        _webHelper = webHelper;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Check whether the site is available
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result determines that request is completed
    /// </returns>
    public virtual async Task PingAsync()
    {
        await _httpClient.GetStringAsync("/");
    }

    /// <summary>
    /// Check the current store for license compliance
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the license check details
    /// </returns>
    public virtual async Task<string> GetLicenseCheckDetailsAsync()
    {
        var isLocal = _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request);
        var storeUrl = _webHelper.GetStoreLocation();
        if (!_adminAreaSettings.CheckLicense || isLocal || storeUrl.Contains("localhost"))
            return string.Empty;

        var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
                           ?? (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
        var url = string.Format(NopCommonDefaults.NopLicenseCheckPath,
            storeUrl,
            NopVersion.FULL_VERSION,
            WebUtility.UrlEncode(emailAccount.Email),
            language).ToLowerInvariant();

        return await _httpClient.GetStringAsync(url);
    }

    /// <summary>
    /// Get official news RSS
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains news RSS feed
    /// </returns>
    public virtual async Task<RssFeed> GetNewsRssAsync()
    {
        //prepare URL to request
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
        var url = string.Format(NopCommonDefaults.NopNewsRssPath,
            NopVersion.FULL_VERSION,
            _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request),
            _adminAreaSettings.HideAdvertisementsOnAdminArea,
            _webHelper.GetStoreLocation(),
            language).ToLowerInvariant();

        //get news feed
        await using var stream = await _httpClient.GetStreamAsync(url);
        return await RssFeed.LoadAsync(stream);
    }

    /// <summary>
    /// Notification about the successful installation
    /// </summary>
    /// <param name="email">Admin email</param>
    /// <param name="languageCode">Language code</param>
    /// <param name="culture">Culture name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the result string
    /// </returns>
    public virtual async Task<string> InstallationCompletedAsync(string email, string languageCode, string culture)
    {
        //prepare URL to request
        var url = string.Format(NopCommonDefaults.NopInstallationCompletedPath,
                NopVersion.FULL_VERSION,
                _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request),
                WebUtility.UrlEncode(email),
                _webHelper.GetStoreLocation(),
                languageCode,
                culture)
            .ToLowerInvariant();

        //this request takes some more time
        try
        {
            if (_httpClient.Timeout != TimeSpan.FromSeconds(30))
                _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }
        catch { }

        return await _httpClient.GetStringAsync(url);
    }

    /// <summary>
    /// Subscribe to nopCommerce newsletters during installation
    /// </summary>
    /// <param name="email">Admin email</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the result string
    /// </returns>
    public virtual async Task<HttpResponseMessage> SubscribeNewslettersAsync(string email)
    {
        //prepare URL to request
        var url = string.Format(NopCommonDefaults.NopSubscribeNewslettersPath,
                WebUtility.UrlEncode(email))
            .ToLowerInvariant();

        return await _httpClient.GetAsync(url);
    }

    /// <summary>
    /// Get a response regarding available categories of marketplace extensions
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the result string
    /// </returns>
    public virtual async Task<string> GetExtensionsCategoriesAsync()
    {
        //prepare URL to request
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
        var url = string.Format(NopCommonDefaults.NopExtensionsCategoriesPath, language).ToLowerInvariant();

        //get XML response
        return await _httpClient.GetStringAsync(url);
    }

    /// <summary>
    /// Get a response regarding available versions of marketplace extensions
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the result string
    /// </returns>
    public virtual async Task<string> GetExtensionsVersionsAsync()
    {
        //prepare URL to request
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
        var url = string.Format(NopCommonDefaults.NopExtensionsVersionsPath, language).ToLowerInvariant();

        //get XML response
        return await _httpClient.GetStringAsync(url);
    }

    /// <summary>
    /// Get a response regarding marketplace extensions
    /// </summary>
    /// <param name="categoryId">Category identifier</param>
    /// <param name="versionId">Version identifier</param>
    /// <param name="price">Price; 0 - all, 10 - free, 20 - paid</param>
    /// <param name="searchTerm">Search term</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result contains the result string
    /// </returns>
    public virtual async Task<string> GetExtensionsAsync(int categoryId = 0,
        int versionId = 0, int price = 0, string searchTerm = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        //prepare URL to request
        var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
        var url = string.Format(NopCommonDefaults.NopExtensionsPath,
            categoryId, versionId, price, WebUtility.UrlEncode(searchTerm), pageIndex, pageSize, language).ToLowerInvariant();

        //get XML response
        return await _httpClient.GetStringAsync(url);
    }

    #endregion
}