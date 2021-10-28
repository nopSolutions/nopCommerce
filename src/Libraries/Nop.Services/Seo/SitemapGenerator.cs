using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Seo;
using Nop.Core.Events;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.News;
using Nop.Services.Topics;

namespace Nop.Services.Seo
{
    /// <summary>
    /// Represents a sitemap generator
    /// </summary>
    public partial class SitemapGenerator : ISitemapGenerator
    {
        #region Fields

        protected BlogSettings BlogSettings { get; }
        protected ForumSettings ForumSettings { get; }
        protected IActionContextAccessor ActionContextAccessor { get; }
        protected IBlogService BlogService { get; }
        protected ICategoryService CategoryService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected ILanguageService LanguageService { get; }
        protected IManufacturerService ManufacturerService { get; }
        protected INewsService NewsService { get; }
        protected IProductService ProductService { get; }
        protected IProductTagService ProductTagService { get; }
        protected IStoreContext StoreContext { get; }
        protected ITopicService TopicService { get; }
        protected IUrlHelperFactory UrlHelperFactory { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWebHelper WebHelper { get; }
        protected LocalizationSettings LocalizationSettings { get; }
        protected NewsSettings NewsSettings { get; }
        protected SitemapXmlSettings SitemapXmlSettings { get; }

        #endregion

        #region Ctor

        public SitemapGenerator(BlogSettings blogSettings,
            ForumSettings forumSettings,
            IActionContextAccessor actionContextAccessor,
            IBlogService blogService,
            ICategoryService categoryService,
            IEventPublisher eventPublisher,
            ILanguageService languageService,
            IManufacturerService manufacturerService,
            INewsService newsService,
            IProductService productService,
            IProductTagService productTagService,
            IStoreContext storeContext,
            ITopicService topicService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            LocalizationSettings localizationSettings,
            NewsSettings newsSettings,
            SitemapXmlSettings sitemapSettings)
        {
            BlogSettings = blogSettings;
            ForumSettings = forumSettings;
            ActionContextAccessor = actionContextAccessor;
            BlogService = blogService;
            CategoryService = categoryService;
            EventPublisher = eventPublisher;
            LanguageService = languageService;
            ManufacturerService = manufacturerService;
            NewsService = newsService;
            ProductService = productService;
            ProductTagService = productTagService;
            StoreContext = storeContext;
            TopicService = topicService;
            UrlHelperFactory = urlHelperFactory;
            UrlRecordService = urlRecordService;
            WebHelper = webHelper;
            LocalizationSettings = localizationSettings;
            NewsSettings = newsSettings;
            SitemapXmlSettings = sitemapSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get UrlHelper
        /// </summary>
        /// <returns>UrlHelper</returns>
        protected virtual IUrlHelper GetUrlHelper()
        {
            return UrlHelperFactory.GetUrlHelper(ActionContextAccessor.ActionContext);
        }

        /// <summary>
        /// Get HTTP protocol
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the protocol name as string
        /// </returns>
        protected virtual async Task<string> GetHttpProtocolAsync()
        {
            var store = await StoreContext.GetCurrentStoreAsync();

            return store.SslEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
        }

        /// <summary>
        /// Generate URLs for the sitemap
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of sitemap URLs
        /// </returns>
        protected virtual async Task<IList<SitemapUrl>> GenerateUrlsAsync()
        {
            var sitemapUrls = new List<SitemapUrl>
            {
                //home page
                await GetLocalizedSitemapUrlAsync("Homepage"),

                //search products
                await GetLocalizedSitemapUrlAsync("ProductSearch"),

                //contact us
                await GetLocalizedSitemapUrlAsync("ContactUs")
            };

            //news
            if (NewsSettings.Enabled)
                sitemapUrls.Add(await GetLocalizedSitemapUrlAsync("NewsArchive"));

            //blog
            if (BlogSettings.Enabled)
                sitemapUrls.Add(await GetLocalizedSitemapUrlAsync("Blog"));

            //forum
            if (ForumSettings.ForumsEnabled)
                sitemapUrls.Add(await GetLocalizedSitemapUrlAsync("Boards"));

            //categories
            if (SitemapXmlSettings.SitemapXmlIncludeCategories)
                sitemapUrls.AddRange(await GetCategoryUrlsAsync());

            //manufacturers
            if (SitemapXmlSettings.SitemapXmlIncludeManufacturers)
                sitemapUrls.AddRange(await GetManufacturerUrlsAsync());

            //products
            if (SitemapXmlSettings.SitemapXmlIncludeProducts)
                sitemapUrls.AddRange(await GetProductUrlsAsync());

            //product tags
            if (SitemapXmlSettings.SitemapXmlIncludeProductTags)
                sitemapUrls.AddRange(await GetProductTagUrlsAsync());

            //news
            if (SitemapXmlSettings.SitemapXmlIncludeNews && NewsSettings.Enabled)
                sitemapUrls.AddRange(await GetNewsItemUrlsAsync());

            //blog posts
            if (SitemapXmlSettings.SitemapXmlIncludeBlogPosts && BlogSettings.Enabled)
                sitemapUrls.AddRange(await GetBlogPostUrlsAsync());

            //topics
            if (SitemapXmlSettings.SitemapXmlIncludeTopics)
                sitemapUrls.AddRange(await GetTopicUrlsAsync());

            //custom URLs
            if (SitemapXmlSettings.SitemapXmlIncludeCustomUrls)
                sitemapUrls.AddRange(GetCustomUrls());

            //event notification
            await EventPublisher.PublishAsync(new SitemapCreatedEvent(sitemapUrls));

            return sitemapUrls;
        }

        /// <summary>
        /// Get news item URLs for the sitemap
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sitemap URLs
        /// </returns>
        protected virtual async Task<IEnumerable<SitemapUrl>> GetNewsItemUrlsAsync()
        {
            var store = await StoreContext.GetCurrentStoreAsync();

            return await (await NewsService.GetAllNewsAsync(storeId: store.Id))
                .SelectAwait(async news => await GetLocalizedSitemapUrlAsync("NewsItem",
                    async lang => new { SeName = await UrlRecordService.GetSeNameAsync(news, news.LanguageId, ensureTwoPublishedLanguages: false) },
                    news.CreatedOnUtc)).ToListAsync();
        }

        /// <summary>
        /// Get category URLs for the sitemap
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sitemap URLs
        /// </returns>
        protected virtual async Task<IEnumerable<SitemapUrl>> GetCategoryUrlsAsync()
        {
            var store = await StoreContext.GetCurrentStoreAsync();

            return await (await CategoryService.GetAllCategoriesAsync(storeId: store.Id))
                .SelectAwait(async category => await GetLocalizedSitemapUrlAsync("Category", GetSeoRouteParamsAwait(category), category.UpdatedOnUtc)).ToListAsync();
        }

        /// <summary>
        /// Get manufacturer URLs for the sitemap
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sitemap URLs
        /// </returns>
        protected virtual async Task<IEnumerable<SitemapUrl>> GetManufacturerUrlsAsync()
        {
            var store = await StoreContext.GetCurrentStoreAsync();

            return await (await ManufacturerService.GetAllManufacturersAsync(storeId: store.Id))
                .SelectAwait(async manufacturer => await GetLocalizedSitemapUrlAsync("Manufacturer", GetSeoRouteParamsAwait(manufacturer), manufacturer.UpdatedOnUtc)).ToListAsync();
        }

        /// <summary>
        /// Get product URLs for the sitemap
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sitemap URLs
        /// </returns>
        protected virtual async Task<IEnumerable<SitemapUrl>> GetProductUrlsAsync()
        {
            var store = await StoreContext.GetCurrentStoreAsync();

            return await (await ProductService.SearchProductsAsync(0, storeId: store.Id,
                visibleIndividuallyOnly: true, orderBy: ProductSortingEnum.CreatedOn))
                .SelectAwait(async product => await GetLocalizedSitemapUrlAsync("Product", GetSeoRouteParamsAwait(product), product.UpdatedOnUtc)).ToListAsync();
        }

        /// <summary>
        /// Get product tag URLs for the sitemap
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sitemap URLs
        /// </returns>
        protected virtual async Task<IEnumerable<SitemapUrl>> GetProductTagUrlsAsync()
        {
            return await (await ProductTagService.GetAllProductTagsAsync())
                .SelectAwait(async productTag => await GetLocalizedSitemapUrlAsync("ProductsByTag", GetSeoRouteParamsAwait(productTag))).ToListAsync();
        }

        /// <summary>
        /// Get topic URLs for the sitemap
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sitemap URLs
        /// </returns>
        protected virtual async Task<IEnumerable<SitemapUrl>> GetTopicUrlsAsync()
        {
            var store = await StoreContext.GetCurrentStoreAsync();

            return await (await TopicService.GetAllTopicsAsync(store.Id)).Where(t => t.IncludeInSitemap)
                .SelectAwait(async topic => await GetLocalizedSitemapUrlAsync("Topic", GetSeoRouteParamsAwait(topic))).ToListAsync();
        }

        /// <summary>
        /// Get blog post URLs for the sitemap
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sitemap URLs
        /// </returns>
        protected virtual async Task<IEnumerable<SitemapUrl>> GetBlogPostUrlsAsync()
        {
            var store = await StoreContext.GetCurrentStoreAsync();

            return await (await BlogService.GetAllBlogPostsAsync(store.Id))
                .Where(p => p.IncludeInSitemap)
                .SelectAwait(async post => await GetLocalizedSitemapUrlAsync("BlogPost",
                    async lang => new { SeName = await UrlRecordService.GetSeNameAsync(post, post.LanguageId, ensureTwoPublishedLanguages: false) },
                    post.CreatedOnUtc)).ToListAsync();
        }

        /// <summary>
        /// Get custom URLs for the sitemap
        /// </summary>
        /// <returns>Sitemap URLs</returns>
        protected virtual IEnumerable<SitemapUrl> GetCustomUrls()
        {
            var storeLocation = WebHelper.GetStoreLocation();

            return SitemapXmlSettings.SitemapCustomUrls.Select(customUrl =>
                new SitemapUrl(string.Concat(storeLocation, customUrl), new List<string>(), UpdateFrequency.Weekly, DateTime.UtcNow));
        }

        /// <summary>
        /// Get route params for URL localization
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="model">Model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the lambda for route params
        /// </returns>
        protected virtual Func<int?, Task<object>> GetSeoRouteParamsAwait<T>(T model)
            where T : BaseEntity, ISlugSupported
        {
            return async lang => new { SeName = await UrlRecordService.GetSeNameAsync(model, lang) };
        }

        /// <summary>
        /// Write sitemap index file into the stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="sitemapNumber">The number of sitemaps</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task WriteSitemapIndexAsync(Stream stream, int sitemapNumber)
        {
            var urlHelper = GetUrlHelper();

            await using var writer = new XmlTextWriter(stream, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };
            writer.WriteStartDocument();
            writer.WriteStartElement("sitemapindex");
            writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
            writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xmlns:xhtml", "http://www.w3.org/1999/xhtml");
            writer.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

            //write URLs of all available sitemaps
            for (var id = 1; id <= sitemapNumber; id++)
            {
                var url = urlHelper.RouteUrl("sitemap-indexed.xml", new { Id = id }, await GetHttpProtocolAsync());
                var location = await XmlHelper.XmlEncodeAsync(url);

                writer.WriteStartElement("sitemap");
                writer.WriteElementString("loc", location);
                writer.WriteElementString("lastmod", DateTime.UtcNow.ToString(NopSeoDefaults.SitemapDateFormat));
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write sitemap file into the stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="sitemapUrls">List of sitemap URLs</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task WriteSitemapAsync(Stream stream, IList<SitemapUrl> sitemapUrls)
        {
            await using var writer = new XmlTextWriter(stream, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };
            writer.WriteStartDocument();
            writer.WriteStartElement("urlset");
            writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
            writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xmlns:xhtml", "http://www.w3.org/1999/xhtml");
            writer.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

            //write URLs from list to the sitemap
            foreach (var sitemapUrl in sitemapUrls)
            {
                //write base url
                await WriteSitemapUrlAsync(writer, sitemapUrl);

                //write all alternate url if exists
                foreach (var alternate in sitemapUrl.AlternateLocations
                    .Where(p => !p.Equals(sitemapUrl.Location, StringComparison.InvariantCultureIgnoreCase)))
                {
                    await WriteSitemapUrlAsync(writer, new SitemapUrl(alternate, sitemapUrl));
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write sitemap
        /// </summary>
        /// <param name="writer">XML stream writer</param>
        /// <param name="sitemapUrl">Sitemap URL</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task WriteSitemapUrlAsync(XmlTextWriter writer, SitemapUrl sitemapUrl)
        {
            if (string.IsNullOrEmpty(sitemapUrl.Location))
                return;

            writer.WriteStartElement("url");

            var loc = await XmlHelper.XmlEncodeAsync(sitemapUrl.Location);
            writer.WriteElementString("loc", loc);

            //write all related url
            foreach (var alternate in sitemapUrl.AlternateLocations)
            {
                if (string.IsNullOrEmpty(alternate))
                    continue;

                //extract seo code
                var altLoc = await XmlHelper.XmlEncodeAsync(alternate);
                var altLocPath = new Uri(altLoc).PathAndQuery;
                var (_, lang) = await altLocPath.IsLocalizedUrlAsync(ActionContextAccessor.ActionContext.HttpContext.Request.PathBase, true);

                if (string.IsNullOrEmpty(lang?.UniqueSeoCode))
                    continue;

                writer.WriteStartElement("xhtml:link");
                writer.WriteAttributeString("rel", "alternate");
                writer.WriteAttributeString("hreflang", lang.UniqueSeoCode);
                writer.WriteAttributeString("href", altLoc);
                writer.WriteEndElement();
            }

            writer.WriteElementString("changefreq", sitemapUrl.UpdateFrequency.ToString().ToLowerInvariant());
            writer.WriteElementString("lastmod", sitemapUrl.UpdatedOn.ToString(NopSeoDefaults.SitemapDateFormat, CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }

        /// <summary>
        /// This will build an XML sitemap for better index with search engines.
        /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
        /// </summary>
        /// <param name="id">Sitemap identifier</param>
        /// <param name="stream">Stream of sitemap.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task GenerateAsync(Stream stream, int? id)
        {
            //generate all URLs for the sitemap
            var sitemapUrls = await GenerateUrlsAsync();

            //split URLs into separate lists based on the max size 
            var sitemaps = sitemapUrls
                .Select((url, index) => new { Index = index, Value = url })
                .GroupBy(group => group.Index / NopSeoDefaults.SitemapMaxUrlNumber)
                .Select(group => group
                    .Select(url => url.Value)
                    .ToList()).ToList();

            if (!sitemaps.Any())
                return;

            if (id.HasValue)
            {
                //requested sitemap does not exist
                if (id.Value == 0 || id.Value > sitemaps.Count)
                    return;

                //otherwise write a certain numbered sitemap file into the stream
                await WriteSitemapAsync(stream, sitemaps.ElementAt(id.Value - 1));
            }
            else
            {
                //URLs more than the maximum allowable, so generate a sitemap index file
                if (sitemapUrls.Count >= NopSeoDefaults.SitemapMaxUrlNumber)
                {
                    //write a sitemap index file into the stream
                    await WriteSitemapIndexAsync(stream, sitemaps.Count);
                }
                else
                {
                    //otherwise generate a standard sitemap
                    await WriteSitemapAsync(stream, sitemaps.First());
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// This will build an XML sitemap for better index with search engines.
        /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
        /// </summary>
        /// <param name="id">Sitemap identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sitemap.xml as string
        /// </returns>
        public virtual async Task<string> GenerateAsync(int? id)
        {
            await using var stream = new MemoryStream();
            await GenerateAsync(stream, id);

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Return localized urls
        /// </summary>
        /// <param name="routeName">Route name</param>
        /// <param name="getRouteParamsAwait">Lambda for route params object</param>
        /// <param name="dateTimeUpdatedOn">A time when URL was updated last time</param>
        /// <param name="updateFreq">How often to update url</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<SitemapUrl> GetLocalizedSitemapUrlAsync(string routeName,
            Func<int?, Task<object>> getRouteParamsAwait = null,
            DateTime? dateTimeUpdatedOn = null,
            UpdateFrequency updateFreq = UpdateFrequency.Weekly)
        {
            var urlHelper = GetUrlHelper();

            //url for current language
            var url = urlHelper.RouteUrl(routeName,
                getRouteParamsAwait != null ? await getRouteParamsAwait(null) : null,
                await GetHttpProtocolAsync());

            var updatedOn = dateTimeUpdatedOn ?? DateTime.UtcNow;
            var languages = LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled
                ? await LanguageService.GetAllLanguagesAsync()
                : null;

            if (languages == null)
                return new SitemapUrl(url, new List<string>(), updateFreq, updatedOn);

            var pathBase = ActionContextAccessor.ActionContext.HttpContext.Request.PathBase;
            //return list of localized urls
            var localizedUrls = await languages
                .SelectAwait(async lang =>
                {
                    var currentUrl = urlHelper.RouteUrl(routeName,
                        getRouteParamsAwait != null ? await getRouteParamsAwait(lang.Id) : null,
                        await GetHttpProtocolAsync());

                    if (string.IsNullOrEmpty(currentUrl))
                        return null;

                    //Extract server and path from url
                    var scheme = new Uri(currentUrl).GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
                    var path = new Uri(currentUrl).PathAndQuery;

                    //Replace seo code
                    var localizedPath = path
                        .RemoveLanguageSeoCodeFromUrl(pathBase, true)
                        .AddLanguageSeoCodeToUrl(pathBase, true, lang);

                    return new Uri(new Uri(scheme), localizedPath).ToString();
                })
                .Where(value => !string.IsNullOrEmpty(value))
                .ToListAsync();

            return new SitemapUrl(url, localizedUrls, updateFreq, updatedOn);
        }

        #endregion
    }
}