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

        protected AdminAreaSettings AdminAreaSettings { get; }
        protected HttpClient HttpClient { get; }
        protected IHttpContextAccessor HttpContextAccessor { get; }
        protected ILanguageService LanguageService { get; }
        protected IStoreContext StoreContext { get; }
        protected IWebHelper WebHelper { get; }
        protected IWorkContext WorkContext { get; }

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

            AdminAreaSettings = adminAreaSettings;
            HttpClient = client;
            HttpContextAccessor = httpContextAccessor;
            LanguageService = languageService;
            StoreContext = storeContext;
            WebHelper = webHelper;
            WorkContext = workContext;
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
            await HttpClient.GetStringAsync("/");
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
            var language = LanguageService.GetTwoLetterIsoLanguageName(await WorkContext.GetWorkingLanguageAsync());
            var store = await StoreContext.GetCurrentStoreAsync();
            var url = string.Format(NopCommonDefaults.NopCopyrightWarningPath,
                store.Url,
                WebHelper.IsLocalRequest(HttpContextAccessor.HttpContext.Request),
                language).ToLowerInvariant();

            //get the message
            return await HttpClient.GetStringAsync(url);
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
            var language = LanguageService.GetTwoLetterIsoLanguageName(await WorkContext.GetWorkingLanguageAsync());
            var url = string.Format(NopCommonDefaults.NopNewsRssPath,
                NopVersion.CURRENT_VERSION,
                WebHelper.IsLocalRequest(HttpContextAccessor.HttpContext.Request),
                AdminAreaSettings.HideAdvertisementsOnAdminArea,
                WebHelper.GetStoreLocation(),
                language).ToLowerInvariant();

            //get news feed
            await using var stream = await HttpClient.GetStreamAsync(url);
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
                WebHelper.IsLocalRequest(HttpContextAccessor.HttpContext.Request),
                WebUtility.UrlEncode(email),
                WebHelper.GetStoreLocation(),
                languageCode,
                culture)
                .ToLowerInvariant();

            //this request takes some more time
            HttpClient.Timeout = TimeSpan.FromSeconds(30);

            return await HttpClient.GetStringAsync(url);
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
            var language = LanguageService.GetTwoLetterIsoLanguageName(await WorkContext.GetWorkingLanguageAsync());
            var url = string.Format(NopCommonDefaults.NopExtensionsCategoriesPath, language).ToLowerInvariant();

            //get XML response
            return await HttpClient.GetStringAsync(url);
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
            var language = LanguageService.GetTwoLetterIsoLanguageName(await WorkContext.GetWorkingLanguageAsync());
            var url = string.Format(NopCommonDefaults.NopExtensionsVersionsPath, language).ToLowerInvariant();

            //get XML response
            return await HttpClient.GetStringAsync(url);
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
            var language = LanguageService.GetTwoLetterIsoLanguageName(await WorkContext.GetWorkingLanguageAsync());
            var url = string.Format(NopCommonDefaults.NopExtensionsPath,
                categoryId, versionId, price, WebUtility.UrlEncode(searchTerm), pageIndex, pageSize, language).ToLowerInvariant();

            //get XML response
            return await HttpClient.GetStringAsync(url);
        }

        #endregion
    }
}