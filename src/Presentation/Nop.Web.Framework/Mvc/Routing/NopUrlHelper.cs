using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Seo;
using Nop.Services.Topics;

namespace Nop.Web.Framework.Mvc.Routing;

/// <summary>
/// Represents the helper implementation to build specific URLs within an application 
/// </summary>
public partial class NopUrlHelper : INopUrlHelper
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly ICategoryService _categoryService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IStoreContext _storeContext;
    protected readonly ITopicService _topicService;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IUrlRecordService _urlRecordService;

    #endregion

    #region Ctor

    public NopUrlHelper(CatalogSettings catalogSettings,
        IActionContextAccessor actionContextAccessor,
        ICategoryService categoryService,
        IManufacturerService manufacturerService,
        IStoreContext storeContext,
        ITopicService topicService,
        IUrlHelperFactory urlHelperFactory,
        IUrlRecordService urlRecordService)
    {
        _catalogSettings = catalogSettings;
        _actionContextAccessor = actionContextAccessor;
        _categoryService = categoryService;
        _manufacturerService = manufacturerService;
        _storeContext = storeContext;
        _topicService = topicService;
        _urlHelperFactory = urlHelperFactory;
        _urlRecordService = urlRecordService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Generate a URL for a product with the specified route values
    /// </summary>
    /// <param name="values">An object that contains route values</param>
    /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
    /// <param name="host">The host name for the URL</param>
    /// <param name="fragment">The fragment for the URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated URL
    /// </returns>
    protected virtual async Task<string> RouteProductUrlAsync(object values = null, string protocol = null, string host = null, string fragment = null)
    {
        if (_catalogSettings.ProductUrlStructureTypeId == (int)ProductUrlStructureType.Product)
            return RouteUrl(NopRoutingDefaults.RouteName.Generic.Product, values, protocol, host, fragment);

        var routeValues = new RouteValueDictionary(values);
        if (!routeValues.TryGetValue(NopRoutingDefaults.RouteValue.SeName, out var slug))
            return RouteUrl(NopRoutingDefaults.RouteName.Generic.Product, values, protocol, host, fragment);

        var urlRecord = await _urlRecordService.GetBySlugAsync(slug.ToString());
        if (urlRecord is null || !urlRecord.EntityName.Equals(nameof(Product), StringComparison.InvariantCultureIgnoreCase))
            return RouteUrl(NopRoutingDefaults.RouteName.Generic.Product, values, protocol, host, fragment);

        var catalogSeName = string.Empty;
        if (_catalogSettings.ProductUrlStructureTypeId == (int)ProductUrlStructureType.CategoryProduct)
        {
            var productCategory = (await _categoryService.GetProductCategoriesByProductIdAsync(urlRecord.EntityId)).FirstOrDefault();
            var category = await _categoryService.GetCategoryByIdAsync(productCategory?.CategoryId ?? 0);
            catalogSeName = category is not null ? await _urlRecordService.GetSeNameAsync(category) : string.Empty;
        }
        if (_catalogSettings.ProductUrlStructureTypeId == (int)ProductUrlStructureType.ManufacturerProduct)
        {
            var productManufacturer = (await _manufacturerService.GetProductManufacturersByProductIdAsync(urlRecord.EntityId)).FirstOrDefault();
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(productManufacturer?.ManufacturerId ?? 0);
            catalogSeName = manufacturer is not null ? await _urlRecordService.GetSeNameAsync(manufacturer) : string.Empty;
        }
        if (string.IsNullOrEmpty(catalogSeName))
            return RouteUrl(NopRoutingDefaults.RouteName.Generic.Product, values, protocol, host, fragment);

        routeValues[NopRoutingDefaults.RouteValue.CatalogSeName] = catalogSeName;
        return RouteUrl(NopRoutingDefaults.RouteName.Generic.ProductCatalog, routeValues, protocol, host, fragment);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Generate a generic URL for the specified entity which supports slug
    /// </summary>
    /// <typeparam name="TEntity">Entity type that supports slug</typeparam>
    /// <param name="entity">An entity which supports slug</param>
    /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
    /// <param name="host">The host name for the URL</param>
    /// <param name="fragment">The fragment for the URL</param>
    /// <param name="languageId">Language identifier; pass null to use the current language</param>
    /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated URL
    /// </returns>
    public virtual async Task<string> RouteGenericUrlAsync<TEntity>(TEntity entity,
        string protocol = null, string host = null, string fragment = null, int? languageId = null, bool ensureTwoPublishedLanguages = true)
        where TEntity : BaseEntity, ISlugSupported
    {
        if (entity is null)
            return string.Empty;

        var seName = await _urlRecordService.GetSeNameAsync(entity, languageId, true, ensureTwoPublishedLanguages);
        return await RouteGenericUrlAsync<TEntity>(new { SeName = seName }, protocol, host, fragment);
    }

    /// <summary>
    /// Generate a generic URL for the specified entity type and route values
    /// </summary>
    /// <typeparam name="TEntity">Entity type that supports slug</typeparam>
    /// <param name="values">An object that contains route values</param>
    /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
    /// <param name="host">The host name for the URL</param>
    /// <param name="fragment">The fragment for the URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated URL
    /// </returns>
    public virtual async Task<string> RouteGenericUrlAsync<TEntity>(object values = null, string protocol = null, string host = null, string fragment = null)
        where TEntity : BaseEntity, ISlugSupported
    {
        return typeof(TEntity) switch
        {
            var entityType when entityType == typeof(Product)
                => await RouteProductUrlAsync(values, protocol, host, fragment),
            var entityType when entityType == typeof(Category)
                => RouteUrl(NopRoutingDefaults.RouteName.Generic.Category, values, protocol, host, fragment),
            var entityType when entityType == typeof(Manufacturer)
                => RouteUrl(NopRoutingDefaults.RouteName.Generic.Manufacturer, values, protocol, host, fragment),
            var entityType when entityType == typeof(Vendor)
                => RouteUrl(NopRoutingDefaults.RouteName.Generic.Vendor, values, protocol, host, fragment),
            var entityType when entityType == typeof(NewsItem)
                => RouteUrl(NopRoutingDefaults.RouteName.Generic.NewsItem, values, protocol, host, fragment),
            var entityType when entityType == typeof(BlogPost)
                => RouteUrl(NopRoutingDefaults.RouteName.Generic.BlogPost, values, protocol, host, fragment),
            var entityType when entityType == typeof(Topic)
                => RouteUrl(NopRoutingDefaults.RouteName.Generic.Topic, values, protocol, host, fragment),
            var entityType when entityType == typeof(ProductTag)
                => RouteUrl(NopRoutingDefaults.RouteName.Generic.ProductTag, values, protocol, host, fragment),
            var entityType => RouteUrl(entityType.Name, values, protocol, host, fragment)
        };
    }

    /// <summary>
    /// Generate a URL for topic by the specified system name
    /// </summary>
    /// <param name="systemName">Topic system name</param>
    /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
    /// <param name="host">The host name for the URL</param>
    /// <param name="fragment">The fragment for the URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated URL
    /// </returns>
    public virtual async Task<string> RouteTopicUrlAsync(string systemName, string protocol = null, string host = null, string fragment = null)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var topic = await _topicService.GetTopicBySystemNameAsync(systemName, store.Id);

        return await RouteGenericUrlAsync(topic, protocol, host, fragment);
    }

    /// <summary>
    /// Generate a URL for the specified route name
    /// </summary>
    /// <param name="routeName">The name of the route that is used to generate URL</param>
    /// <param name="values">An object that contains route values</param>
    /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
    /// <param name="host">The host name for the URL</param>
    /// <param name="fragment">The fragment for the URL</param>
    /// <returns>
    /// The generated URL
    /// </returns>
    public virtual string RouteUrl(string routeName, object values = null, string protocol = null, string host = null, string fragment = null)
    {
        if (_actionContextAccessor.ActionContext is null)
            return string.Empty;

        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

        return urlHelper.RouteUrl(routeName, values, protocol, host, fragment);
    }

    #endregion
}