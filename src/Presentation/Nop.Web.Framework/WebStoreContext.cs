using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Stores;

namespace Nop.Web.Framework;

/// <summary>
/// Store context for web application
/// </summary>
public partial class WebStoreContext : IStoreContext
{
    #region Fields

    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IRepository<Store> _storeRepository;
    protected readonly IStoreService _storeService;

    protected Store _cachedStore;
    protected int? _cachedActiveStoreScopeConfiguration;

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="genericAttributeService">Generic attribute service</param>
    /// <param name="httpContextAccessor">HTTP context accessor</param>
    /// <param name="storeRepository">Store repository</param>
    /// <param name="storeService">Store service</param>
    public WebStoreContext(IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        IRepository<Store> storeRepository,
        IStoreService storeService)
    {
        _genericAttributeService = genericAttributeService;
        _httpContextAccessor = httpContextAccessor;
        _storeRepository = storeRepository;
        _storeService = storeService;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current store
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<Store> GetCurrentStoreAsync()
    {
        if (_cachedStore != null)
            return _cachedStore;

        //try to determine the current store by HOST header
        string host = _httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Host];

        var allStores = await _storeService.GetAllStoresAsync();
        var store = allStores.FirstOrDefault(s => _storeService.ContainsHostValue(s, host)) ?? allStores.FirstOrDefault();

        _cachedStore = store ?? throw new Exception("No store could be loaded");

        return _cachedStore;
    }

    /// <summary>
    /// Gets the current store
    /// </summary>
    public virtual Store GetCurrentStore()
    {
        if (_cachedStore != null)
            return _cachedStore;

        //try to determine the current store by HOST header
        string host = _httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Host];

        //we cannot call async methods here. otherwise, an application can hang. so it's a workaround to avoid that
        var allStores = _storeRepository.GetAll(query =>
        {
            return from s in query orderby s.DisplayOrder, s.Id select s;
        }, _ => default, includeDeleted: false);

        var store = allStores.FirstOrDefault(s => _storeService.ContainsHostValue(s, host)) ?? allStores.FirstOrDefault();

        _cachedStore = store ?? throw new Exception("No store could be loaded");

        return _cachedStore;
    }

    /// <summary>
    /// Gets active store scope configuration
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<int> GetActiveStoreScopeConfigurationAsync()
    {
        if (_cachedActiveStoreScopeConfiguration.HasValue)
            return _cachedActiveStoreScopeConfiguration.Value;

        //ensure that we have 2 (or more) stores
        if ((await _storeService.GetAllStoresAsync()).Count > 1)
        {
            //do not inject IWorkContext via constructor because it'll cause circular references
            var currentCustomer = await EngineContext.Current.Resolve<IWorkContext>().GetCurrentCustomerAsync();

            //try to get store identifier from attributes
            var storeId = await _genericAttributeService
                .GetAttributeAsync<int>(currentCustomer, NopCustomerDefaults.AdminAreaStoreScopeConfigurationAttribute);

            _cachedActiveStoreScopeConfiguration = (await _storeService.GetStoreByIdAsync(storeId))?.Id ?? 0;
        }
        else
            _cachedActiveStoreScopeConfiguration = 0;

        return _cachedActiveStoreScopeConfiguration ?? 0;
    }

    #endregion
}