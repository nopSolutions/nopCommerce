﻿using System.Globalization;
using System.Text;
using System.Xml;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Seo;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Sitemap;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the sitemap model factory implementation
/// </summary>
public partial class SitemapModelFactory : ISitemapModelFactory
{
    #region Fields

    protected readonly BlogSettings _blogSettings;
    protected readonly ForumSettings _forumSettings;
    protected readonly IBlogService _blogService;
    protected readonly ICategoryService _categoryService;
    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocker _locker;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly INewsService _newsService;
    protected readonly INopFileProvider _nopFileProvider;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IProductService _productService;
    protected readonly IProductTagService _productTagService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly ITopicService _topicService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly NewsSettings _newsSettings;
    protected readonly SitemapSettings _sitemapSettings;
    protected readonly SitemapXmlSettings _sitemapXmlSettings;

    #endregion

    #region Ctor

    public SitemapModelFactory(BlogSettings blogSettings,
        ForumSettings forumSettings,
        IBlogService blogService,
        ICategoryService categoryService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        IHttpContextAccessor httpContextAccessor,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILocker locker,
        IManufacturerService manufacturerService,
        INewsService newsService,
        INopFileProvider nopFileProvider,
        INopUrlHelper nopUrlHelper,
        IProductService productService,
        IProductTagService productTagService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        ITopicService topicService,
        IWebHelper webHelper,
        IWorkContext workContext,
        LocalizationSettings localizationSettings,
        NewsSettings newsSettings,
        SitemapSettings sitemapSettings,
        SitemapXmlSettings sitemapXmlSettings)
    {
        _blogSettings = blogSettings;
        _forumSettings = forumSettings;
        _blogService = blogService;
        _categoryService = categoryService;
        _customerService = customerService;
        _eventPublisher = eventPublisher;
        _httpContextAccessor = httpContextAccessor;
        _languageService = languageService;
        _localizationService = localizationService;
        _locker = locker;
        _manufacturerService = manufacturerService;
        _newsService = newsService;
        _nopFileProvider = nopFileProvider;
        _nopUrlHelper = nopUrlHelper;
        _productService = productService;
        _productTagService = productTagService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _topicService = topicService;
        _webHelper = webHelper;
        _workContext = workContext;
        _localizationSettings = localizationSettings;
        _newsSettings = newsSettings;
        _sitemapSettings = sitemapSettings;
        _sitemapXmlSettings = sitemapXmlSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get HTTP protocol
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the protocol name as string
    /// </returns>
    protected virtual async Task<string> GetHttpProtocolAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();

        return store.SslEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
    }

    /// <summary>
    /// Generate URLs for the sitemap
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of sitemap URLs
    /// </returns>
    protected virtual async Task<IList<SitemapUrlModel>> GenerateUrlsAsync()
    {
        var sitemapUrls = new List<SitemapUrlModel>
        {
            //home page
            await PrepareLocalizedSitemapUrlAsync(NopRouteNames.General.HOMEPAGE),

            //search products
            await PrepareLocalizedSitemapUrlAsync(NopRouteNames.General.SEARCH),

            //contact us
            await PrepareLocalizedSitemapUrlAsync(NopRouteNames.General.CONTACT_US)
        };

        //news
        if (_newsSettings.Enabled)
            sitemapUrls.Add(await PrepareLocalizedSitemapUrlAsync(NopRouteNames.General.NEWS));

        //blog
        if (_blogSettings.Enabled)
            sitemapUrls.Add(await PrepareLocalizedSitemapUrlAsync(NopRouteNames.General.BLOG));

        //forum
        if (_forumSettings.ForumsEnabled)
            sitemapUrls.Add(await PrepareLocalizedSitemapUrlAsync(NopRouteNames.General.BOARDS));

        //categories
        if (_sitemapXmlSettings.SitemapXmlIncludeCategories)
            sitemapUrls.AddRange(await GetCategoryUrlsAsync());

        //manufacturers
        if (_sitemapXmlSettings.SitemapXmlIncludeManufacturers)
            sitemapUrls.AddRange(await GetManufacturerUrlsAsync());

        //products
        if (_sitemapXmlSettings.SitemapXmlIncludeProducts)
            sitemapUrls.AddRange(await GetProductUrlsAsync());

        //product tags
        if (_sitemapXmlSettings.SitemapXmlIncludeProductTags)
            sitemapUrls.AddRange(await GetProductTagUrlsAsync());

        //news
        if (_sitemapXmlSettings.SitemapXmlIncludeNews && _newsSettings.Enabled)
            sitemapUrls.AddRange(await GetNewsItemUrlsAsync());

        //blog posts
        if (_sitemapXmlSettings.SitemapXmlIncludeBlogPosts && _blogSettings.Enabled)
            sitemapUrls.AddRange(await GetBlogPostUrlsAsync());

        //topics
        if (_sitemapXmlSettings.SitemapXmlIncludeTopics)
            sitemapUrls.AddRange(await GetTopicUrlsAsync());

        //custom URLs
        if (_sitemapXmlSettings.SitemapXmlIncludeCustomUrls)
            sitemapUrls.AddRange(GetCustomUrls());

        //event notification
        await _eventPublisher.PublishAsync(new SitemapCreatedEvent(sitemapUrls));

        return sitemapUrls;
    }

    /// <summary>
    /// Get news item URLs for the sitemap
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap URLs
    /// </returns>
    protected virtual async Task<IEnumerable<SitemapUrlModel>> GetNewsItemUrlsAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var newsList = await _newsService.GetAllNewsAsync(storeId: store.Id);

        return await newsList
            .SelectAwait(async news => await PrepareLocalizedSitemapUrlAsync(news, news.CreatedOnUtc))
            .ToListAsync();
    }

    /// <summary>
    /// Get category URLs for the sitemap
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap URLs
    /// </returns>
    protected virtual async Task<IEnumerable<SitemapUrlModel>> GetCategoryUrlsAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var categories = await _categoryService.GetAllCategoriesAsync(storeId: store.Id);

        return await categories
            .SelectAwait(async category => await PrepareLocalizedSitemapUrlAsync(category, category.UpdatedOnUtc))
            .ToListAsync();
    }

    /// <summary>
    /// Get manufacturer URLs for the sitemap
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap URLs
    /// </returns>
    protected virtual async Task<IEnumerable<SitemapUrlModel>> GetManufacturerUrlsAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var manufacturers = await _manufacturerService.GetAllManufacturersAsync(storeId: store.Id);

        return await manufacturers
            .SelectAwait(async manufacturer => await PrepareLocalizedSitemapUrlAsync(manufacturer, manufacturer.UpdatedOnUtc))
            .ToListAsync();
    }

    /// <summary>
    /// Get product URLs for the sitemap
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap URLs
    /// </returns>
    protected virtual async Task<IEnumerable<SitemapUrlModel>> GetProductUrlsAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var products = await _productService
            .SearchProductsAsync(0, storeId: store.Id, visibleIndividuallyOnly: true, orderBy: ProductSortingEnum.CreatedOn);

        return await products
            .SelectAwait(async product => await PrepareLocalizedSitemapUrlAsync(product, product.UpdatedOnUtc))
            .ToListAsync();
    }

    /// <summary>
    /// Get product tag URLs for the sitemap
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap URLs
    /// </returns>
    protected virtual async Task<IEnumerable<SitemapUrlModel>> GetProductTagUrlsAsync()
    {
        var productTags = await _productTagService.GetAllProductTagsAsync();

        return await productTags
            .SelectAwait(async productTag => await PrepareLocalizedSitemapUrlAsync(productTag))
            .ToListAsync();
    }

    /// <summary>
    /// Get topic URLs for the sitemap
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap URLs
    /// </returns>
    protected virtual async Task<IEnumerable<SitemapUrlModel>> GetTopicUrlsAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var topics = await _topicService.GetAllTopicsAsync(store.Id);

        return await topics
            .Where(t => t.IncludeInSitemap)
            .SelectAwait(async topic => await PrepareLocalizedSitemapUrlAsync(topic))
            .ToListAsync();
    }

    /// <summary>
    /// Get blog post URLs for the sitemap
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap URLs
    /// </returns>
    protected virtual async Task<IEnumerable<SitemapUrlModel>> GetBlogPostUrlsAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var blogPosts = await _blogService.GetAllBlogPostsAsync(store.Id);

        return await blogPosts
            .Where(p => p.IncludeInSitemap)
            .SelectAwait(async post => await PrepareLocalizedSitemapUrlAsync(post, post.CreatedOnUtc))
            .ToListAsync();
    }

    /// <summary>
    /// Get custom URLs for the sitemap
    /// </summary>
    /// <returns>Sitemap URLs</returns>
    protected virtual IEnumerable<SitemapUrlModel> GetCustomUrls()
    {
        var storeLocation = _webHelper.GetStoreLocation();

        return _sitemapXmlSettings.SitemapCustomUrls.Select(customUrl =>
            new SitemapUrlModel(string.Concat(storeLocation, customUrl), new List<string>(), UpdateFrequency.Weekly, DateTime.UtcNow));
    }

    /// <summary>
    /// Write sitemap index file into the stream
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="sitemapNumber">The number of sitemaps</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task WriteSitemapIndexAsync(MemoryStream stream, int sitemapNumber)
    {
        await using var writer = XmlWriter.Create(stream, new XmlWriterSettings
        {
            Async = true,
            Encoding = Encoding.UTF8,
            Indent = true,
            ConformanceLevel = ConformanceLevel.Auto
        });

        await writer.WriteStartDocumentAsync();
        await writer.WriteStartElementAsync(prefix: null, localName: "sitemapindex", ns: "http://www.sitemaps.org/schemas/sitemap/0.9");

        //write URLs of all available sitemaps
        for (var id = 1; id <= sitemapNumber; id++)
        {
            var url = _nopUrlHelper.RouteUrl(NopRouteNames.Standard.SITEMAP_INDEXED_XML, new { Id = id }, await GetHttpProtocolAsync());
            var location = await XmlHelper.XmlEncodeAsync(url);

            await writer.WriteStartElementAsync(null, "sitemap", null);
            await writer.WriteElementStringAsync(null, "loc", null, location);
            await writer.WriteElementStringAsync(null, "lastmod", null, DateTime.UtcNow.ToString(NopSeoDefaults.SitemapDateFormat));
            await writer.WriteEndElementAsync();
        }

        await writer.WriteEndElementAsync();
    }

    /// <summary>
    /// Write sitemap file into the stream
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="sitemapUrls">List of sitemap URLs</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task WriteSitemapAsync(MemoryStream stream, IList<SitemapUrlModel> sitemapUrls)
    {
        await using var writer = XmlWriter.Create(stream, new XmlWriterSettings
        {
            Async = true,
            Encoding = Encoding.UTF8,
            Indent = true,
            ConformanceLevel = ConformanceLevel.Auto
        });

        await writer.WriteStartDocumentAsync();
        await writer.WriteStartElementAsync(prefix: null, localName: "urlset", ns: "http://www.sitemaps.org/schemas/sitemap/0.9");
        await writer.WriteAttributeStringAsync(prefix: "xsi", "schemaLocation",
            "http://www.w3.org/2001/XMLSchema-instance",
            "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd http://www.w3.org/1999/xhtml http://www.w3.org/2002/08/xhtml/xhtml1-strict.xsd");
        await writer.WriteAttributeStringAsync(prefix: "xmlns", "xhtml", null, "http://www.w3.org/1999/xhtml");

        //write URLs from list to the sitemap
        foreach (var sitemapUrl in sitemapUrls)
        {
            //write base url
            await WriteSitemapUrlAsync(writer, sitemapUrl);

            //write all alternate url if exists
            foreach (var alternate in sitemapUrl.AlternateLocations
                         .Where(p => !p.Equals(sitemapUrl.Location, StringComparison.InvariantCultureIgnoreCase)))
            {
                await WriteSitemapUrlAsync(writer, new SitemapUrlModel(alternate, sitemapUrl));
            }
        }

        await writer.WriteEndElementAsync();
    }

    /// <summary>
    /// Write sitemap
    /// </summary>
    /// <param name="writer">XML stream writer</param>
    /// <param name="sitemapUrl">Sitemap URL</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task WriteSitemapUrlAsync(XmlWriter writer, SitemapUrlModel sitemapUrl)
    {
        if (string.IsNullOrEmpty(sitemapUrl.Location))
            return;

        await writer.WriteStartElementAsync(null, "url", null);

        var loc = await XmlHelper.XmlEncodeAsync(sitemapUrl.Location);
        await writer.WriteElementStringAsync(null, "loc", null, loc);

        //write all related url
        foreach (var alternate in sitemapUrl.AlternateLocations)
        {
            if (string.IsNullOrEmpty(alternate))
                continue;

            //extract seo code
            var altLoc = await XmlHelper.XmlEncodeAsync(alternate);
            var altLocPath = new Uri(altLoc).PathAndQuery;
            var (_, lang) = await altLocPath.IsLocalizedUrlAsync(_httpContextAccessor.HttpContext.Request.PathBase, true);

            if (string.IsNullOrEmpty(lang?.UniqueSeoCode))
                continue;

            await writer.WriteStartElementAsync("xhtml", "link", null);
            await writer.WriteAttributeStringAsync(null, "rel", null, "alternate");
            await writer.WriteAttributeStringAsync(null, "hreflang", null, lang.UniqueSeoCode);
            await writer.WriteAttributeStringAsync(null, "href", null, altLoc);
            await writer.WriteEndElementAsync();
        }

        await writer.WriteElementStringAsync(null, "changefreq", null, sitemapUrl.UpdateFrequency.ToString().ToLowerInvariant());
        await writer.WriteElementStringAsync(null, "lastmod", null, sitemapUrl.UpdatedOn.ToString(NopSeoDefaults.SitemapDateFormat, CultureInfo.InvariantCulture));
        await writer.WriteEndElementAsync();
    }

    /// <summary>
    /// This will build an XML sitemap for better index with search engines.
    /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
    /// </summary>
    /// <param name="fullPath">The path and name of the sitemap file</param>
    /// <param name="id">Sitemap identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task GenerateAsync(string fullPath, int id = 0)
    {
        //generate all URLs for the sitemap
        var sitemapUrls = await GenerateUrlsAsync();

        //split URLs into separate lists based on the max size
        var numberOfParts = (int)Math.Ceiling((decimal)sitemapUrls.Sum(x => (x.AlternateLocations?.Count ?? 0) + 1) / NopSeoDefaults.SitemapMaxUrlNumber);

        var sitemaps = sitemapUrls
            .Select((url, index) => new { Index = index, Value = url })
            .GroupBy(group => group.Index % numberOfParts)
            .Select(group => group
                .Select(url => url.Value)
                .ToList()).ToList();

        if (!sitemaps.Any())
            return;

        await using var stream = new MemoryStream();

        if (id > 0)
        {
            //requested sitemap does not exist
            if (id > sitemaps.Count)
                return;

            //otherwise write a certain numbered sitemap file into the stream
            await WriteSitemapAsync(stream, sitemaps.ElementAt(id - 1));
        }
        else
        {
            //URLs more than the maximum allowable, so generate a sitemap index file
            if (numberOfParts > 1)
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

        if (_nopFileProvider.FileExists(fullPath))
            _nopFileProvider.DeleteFile(fullPath);


        using var fileStream = _nopFileProvider.GetOrCreateFile(fullPath);
        stream.Position = 0;
        await stream.CopyToAsync(fileStream, 81920);
    }

    /// <summary>
    /// Gets localized URL with SEO code
    /// </summary>
    /// <param name="currentUrl">URL to add SEO code</param>
    /// <param name="lang">Language for localization</param>
    /// <returns>Localized URL with SEO code</returns>
    protected virtual string GetLocalizedUrl(string currentUrl, Language lang)
    {
        if (string.IsNullOrEmpty(currentUrl))
            return null;

        var pathBase = _httpContextAccessor.HttpContext.Request.PathBase;

        //Extract server and path from url
        var scheme = new Uri(currentUrl).GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
        var path = new Uri(currentUrl).PathAndQuery;

        //Replace seo code
        var localizedPath = path
            .RemoveLanguageSeoCodeFromUrl(pathBase, true)
            .AddLanguageSeoCodeToUrl(pathBase, true, lang);

        return new Uri(new Uri(scheme), localizedPath).ToString();
    }

    /// <summary>
    /// Return localized urls
    /// </summary>
    /// <param name="entity">An entity which supports slug</param>
    /// <param name="dateTimeUpdatedOn">A time when URL was updated last time</param>
    /// <param name="updateFreq">How often to update url</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<SitemapUrlModel> PrepareLocalizedSitemapUrlAsync<TEntity>(TEntity entity,
        DateTime? dateTimeUpdatedOn = null,
        UpdateFrequency updateFreq = UpdateFrequency.Weekly) where TEntity : BaseEntity, ISlugSupported
    {
        var isSingleLanguageEntity = entity is BlogPost or NewsItem;
        var url = await _nopUrlHelper
            .RouteGenericUrlAsync(entity, await GetHttpProtocolAsync(), ensureTwoPublishedLanguages: !isSingleLanguageEntity);

        var store = await _storeContext.GetCurrentStoreAsync();

        var updatedOn = dateTimeUpdatedOn ?? DateTime.UtcNow;
        var languages = _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled
            ? await _languageService.GetAllLanguagesAsync(storeId: store.Id)
            : null;

        if (languages == null || languages.Count == 1)
            return new SitemapUrlModel(url, new List<string>(), updateFreq, updatedOn);

        if (isSingleLanguageEntity)
        {
            var languageId = entity is BlogPost blogPost ? blogPost.LanguageId : (entity is NewsItem newsItem ? newsItem.LanguageId : 0);
            if (await _languageService.GetLanguageByIdAsync(languageId) is not Language language || !language.Published)
                return new SitemapUrlModel(url, new List<string>(), updateFreq, updatedOn);

            var localizedUrl = await _nopUrlHelper
                .RouteGenericUrlAsync(entity, await GetHttpProtocolAsync(), languageId: languageId, ensureTwoPublishedLanguages: false);
            localizedUrl = GetLocalizedUrl(url, language);
            return new SitemapUrlModel(localizedUrl, new List<string>(), updateFreq, updatedOn);
        }

        //return list of localized urls
        var localizedUrls = await languages
            .SelectAwait(async lang =>
            {
                var currentUrl = await _nopUrlHelper.RouteGenericUrlAsync(entity, await GetHttpProtocolAsync(), languageId: lang.Id);
                return GetLocalizedUrl(currentUrl, lang);
            })
            .Where(value => !string.IsNullOrEmpty(value))
            .ToListAsync();

        return new SitemapUrlModel(url, localizedUrls, updateFreq, updatedOn);
    }

    /// <summary>
    /// Return localized urls
    /// </summary>
    /// <param name="routeName">Route name</param>
    /// <param name="dateTimeUpdatedOn">A time when URL was updated last time</param>
    /// <param name="updateFreq">How often to update url</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<SitemapUrlModel> PrepareLocalizedSitemapUrlAsync(string routeName,
        DateTime? dateTimeUpdatedOn = null,
        UpdateFrequency updateFreq = UpdateFrequency.Weekly)
    {
        var url = _nopUrlHelper.RouteUrl(routeName, null, await GetHttpProtocolAsync());

        var store = await _storeContext.GetCurrentStoreAsync();

        var updatedOn = dateTimeUpdatedOn ?? DateTime.UtcNow;
        var languages = _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled
            ? await _languageService.GetAllLanguagesAsync(storeId: store.Id)
            : null;

        if (languages == null || languages.Count == 1)
            return new SitemapUrlModel(url, new List<string>(), updateFreq, updatedOn);

        //return list of localized urls
        var localizedUrls = await languages
            .Select(lang => GetLocalizedUrl(url, lang))
            .Where(value => !string.IsNullOrEmpty(value))
            .ToListAsync();

        return new SitemapUrlModel(url, localizedUrls, updateFreq, updatedOn);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the sitemap model
    /// </summary>
    /// <param name="pageModel">Sitemap page model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap model
    /// </returns>
    public virtual async Task<SitemapModel> PrepareSitemapModelAsync(SitemapPageModel pageModel)
    {
        ArgumentNullException.ThrowIfNull(pageModel);

        var language = await _workContext.GetWorkingLanguageAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.SitemapPageModelKey,
            language, customerRoleIds, store);

        var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var model = new SitemapModel();

            //prepare common items
            var commonGroupTitle = await _localizationService.GetResourceAsync("Sitemap.General");

            //home page
            model.Items.Add(new SitemapModel.SitemapItemModel
            {
                GroupTitle = commonGroupTitle,
                Name = await _localizationService.GetResourceAsync("Homepage"),
                Url = _nopUrlHelper.RouteUrl(NopRouteNames.General.HOMEPAGE)
            });

            //search
            model.Items.Add(new SitemapModel.SitemapItemModel
            {
                GroupTitle = commonGroupTitle,
                Name = await _localizationService.GetResourceAsync("Search"),
                Url = _nopUrlHelper.RouteUrl(NopRouteNames.General.SEARCH)
            });

            //news
            if (_newsSettings.Enabled)
            {
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = await _localizationService.GetResourceAsync("News"),
                    Url = _nopUrlHelper.RouteUrl(NopRouteNames.General.NEWS)
                });
            }

            //blog
            if (_blogSettings.Enabled)
            {
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = await _localizationService.GetResourceAsync("Blog"),
                    Url = _nopUrlHelper.RouteUrl(NopRouteNames.General.BLOG)
                });
            }

            //forums
            if (_forumSettings.ForumsEnabled)
            {
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = await _localizationService.GetResourceAsync("Forum.Forums"),
                    Url = _nopUrlHelper.RouteUrl(NopRouteNames.General.BOARDS)
                });
            }

            //contact us
            model.Items.Add(new SitemapModel.SitemapItemModel
            {
                GroupTitle = commonGroupTitle,
                Name = await _localizationService.GetResourceAsync("ContactUs"),
                Url = _nopUrlHelper.RouteUrl(NopRouteNames.General.CONTACT_US)
            });

            //customer info
            model.Items.Add(new SitemapModel.SitemapItemModel
            {
                GroupTitle = commonGroupTitle,
                Name = await _localizationService.GetResourceAsync("Account.MyAccount"),
                Url = _nopUrlHelper.RouteUrl(NopRouteNames.General.CUSTOMER_INFO)
            });

            //at the moment topics are in general category too
            if (_sitemapSettings.SitemapIncludeTopics)
            {
                var topics = (await _topicService.GetAllTopicsAsync(storeId: store.Id))
                    .Where(topic => topic.IncludeInSitemap);

                model.Items.AddRange(await topics.SelectAwait(async topic => new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = await _localizationService.GetLocalizedAsync(topic, x => x.Title),
                    Url = await _nopUrlHelper.RouteGenericUrlAsync(topic)
                }).ToListAsync());
            }

            //blog posts
            if (_sitemapSettings.SitemapIncludeBlogPosts && _blogSettings.Enabled)
            {
                var blogPostsGroupTitle = await _localizationService.GetResourceAsync("Sitemap.BlogPosts");
                var blogPosts = (await _blogService.GetAllBlogPostsAsync(storeId: store.Id))
                    .Where(p => p.IncludeInSitemap);

                model.Items.AddRange(await blogPosts.SelectAwait(async post => new SitemapModel.SitemapItemModel
                {
                    GroupTitle = blogPostsGroupTitle,
                    Name = post.Title,
                    Url = await _nopUrlHelper.RouteGenericUrlAsync(post, languageId: post.LanguageId, ensureTwoPublishedLanguages: false)
                }).ToListAsync());
            }

            //news
            if (_sitemapSettings.SitemapIncludeNews && _newsSettings.Enabled)
            {
                var newsGroupTitle = await _localizationService.GetResourceAsync("Sitemap.News");
                var news = await _newsService.GetAllNewsAsync(storeId: store.Id);
                model.Items.AddRange(await news.SelectAwait(async newsItem => new SitemapModel.SitemapItemModel
                {
                    GroupTitle = newsGroupTitle,
                    Name = newsItem.Title,
                    Url = await _nopUrlHelper.RouteGenericUrlAsync(newsItem, languageId: newsItem.LanguageId, ensureTwoPublishedLanguages: false)
                }).ToListAsync());
            }

            //categories
            if (_sitemapSettings.SitemapIncludeCategories)
            {
                var categoriesGroupTitle = await _localizationService.GetResourceAsync("Sitemap.Categories");
                var categories = await _categoryService.GetAllCategoriesAsync(storeId: store.Id);
                model.Items.AddRange(await categories.SelectAwait(async category => new SitemapModel.SitemapItemModel
                {
                    GroupTitle = categoriesGroupTitle,
                    Name = await _localizationService.GetLocalizedAsync(category, x => x.Name),
                    Url = await _nopUrlHelper.RouteGenericUrlAsync(category)
                }).ToListAsync());
            }

            //manufacturers
            if (_sitemapSettings.SitemapIncludeManufacturers)
            {
                var manufacturersGroupTitle = await _localizationService.GetResourceAsync("Sitemap.Manufacturers");
                var manufacturers = await _manufacturerService.GetAllManufacturersAsync(storeId: store.Id);
                model.Items.AddRange(await manufacturers.SelectAwait(async manufacturer => new SitemapModel.SitemapItemModel
                {
                    GroupTitle = manufacturersGroupTitle,
                    Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                    Url = await _nopUrlHelper.RouteGenericUrlAsync(manufacturer)
                }).ToListAsync());
            }

            //products
            if (_sitemapSettings.SitemapIncludeProducts)
            {
                var productsGroupTitle = await _localizationService.GetResourceAsync("Sitemap.Products");
                var products = await _productService.SearchProductsAsync(0, storeId: store.Id, visibleIndividuallyOnly: true);
                model.Items.AddRange(await products.SelectAwait(async product => new SitemapModel.SitemapItemModel
                {
                    GroupTitle = productsGroupTitle,
                    Name = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                    Url = await _nopUrlHelper.RouteGenericUrlAsync(product)
                }).ToListAsync());
            }

            //product tags
            if (_sitemapSettings.SitemapIncludeProductTags)
            {
                var productTagsGroupTitle = await _localizationService.GetResourceAsync("Sitemap.ProductTags");
                var productTags = await _productTagService.GetAllProductTagsAsync();
                model.Items.AddRange(await productTags.SelectAwait(async productTag => new SitemapModel.SitemapItemModel
                {
                    GroupTitle = productTagsGroupTitle,
                    Name = await _localizationService.GetLocalizedAsync(productTag, x => x.Name),
                    Url = await _nopUrlHelper.RouteGenericUrlAsync(productTag)
                }).ToListAsync());
            }

            return model;
        });

        //prepare model with pagination
        pageModel.PageSize = Math.Max(pageModel.PageSize, _sitemapSettings.SitemapPageSize);
        pageModel.PageNumber = Math.Max(pageModel.PageNumber, 1);

        var pagedItems = new PagedList<SitemapModel.SitemapItemModel>(cachedModel.Items, pageModel.PageNumber - 1, pageModel.PageSize);
        var sitemapModel = new SitemapModel { Items = pagedItems };
        sitemapModel.PageModel.LoadPagedList(pagedItems);

        return sitemapModel;
    }

    /// <summary>
    /// Prepare sitemap model.
    /// This will build an XML sitemap for better index with search engines.
    /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
    /// </summary>
    /// <param name="id">Sitemap identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap model with sitemap.xml as string
    /// </returns>
    public virtual async Task<SitemapXmlModel> PrepareSitemapXmlModelAsync(int id = 0)
    {
        var language = await _workContext.GetWorkingLanguageAsync();
        var store = await _storeContext.GetCurrentStoreAsync();

        var fileName = string.Format(NopSeoDefaults.SitemapXmlFilePattern, store.Id, language.Id, id);
        var fullPath = _nopFileProvider.GetAbsolutePath(NopSeoDefaults.SitemapXmlDirectory, fileName);

        if (_nopFileProvider.FileExists(fullPath) && _nopFileProvider.GetLastWriteTimeUtc(fullPath) > DateTime.UtcNow.AddHours(-_sitemapXmlSettings.RebuildSitemapXmlAfterHours))
        {
            return new SitemapXmlModel { SitemapXmlPath = fullPath };
        }

        //execute task with lock
        if (!await _locker.PerformActionWithLockAsync(
                fullPath,
                TimeSpan.FromSeconds(_sitemapXmlSettings.SitemapBuildOperationDelay),
                async () => await GenerateAsync(fullPath, id)))
        {
            throw new InvalidOperationException();
        }

        return new SitemapXmlModel { SitemapXmlPath = fullPath };
    }

    #endregion
}