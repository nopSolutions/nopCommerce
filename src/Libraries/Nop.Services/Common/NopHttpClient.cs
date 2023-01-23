using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Rss;
using Nop.Services.Localization;

namespace Nop.Services.Common
{
    /// <summary>
    /// Represents the HTTP client to request nopCommerce official site
    /// </summary>
    public partial class NopHttpClient
    {
        #region Fields

        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public NopHttpClient(AdminAreaSettings adminAreaSettings,
            HttpClient client,
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            //configure client
            client.BaseAddress = new Uri("https://www.nopcommerce.com/");
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"nopCommerce-{NopVersion.CURRENT_VERSION}");

            _adminAreaSettings = adminAreaSettings;
            _httpClient = client;
            _httpContextAccessor = httpContextAccessor;
            _languageService = languageService;
            _storeContext = storeContext;
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
        /// Check the current store for the copyright removal key
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the asynchronous task whose result contains the warning text
        /// </returns>
        public virtual async Task<string> GetCopyrightWarningAsync()
        {
            //prepare URL to request
            var language = _languageService.GetTwoLetterIsoLanguageName(await _workContext.GetWorkingLanguageAsync());
            var store = await _storeContext.GetCurrentStoreAsync();
            var url = string.Format(NopCommonDefaults.NopCopyrightWarningPath,
                store.Url,
                _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request),
                language).ToLowerInvariant();

            //get the message
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
                NopVersion.CURRENT_VERSION,
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
                NopVersion.CURRENT_VERSION,
                _webHelper.IsLocalRequest(_httpContextAccessor.HttpContext.Request),
                WebUtility.UrlEncode(email),
                _webHelper.GetStoreLocation(),
                languageCode,
                culture)
                .ToLowerInvariant();

            //this request takes some more time
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            return await _httpClient.GetStringAsync(url);
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
}