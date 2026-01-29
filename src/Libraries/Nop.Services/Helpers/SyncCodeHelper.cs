using System.Linq.Expressions;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Stores;

namespace Nop.Services.Helpers;

/// <summary>
/// User synchronous code helper implementation
/// </summary>
public partial class SyncCodeHelper : ISyncCodeHelper
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreService _storeService;

    protected Store _cachedStore;

    #endregion

    #region Ctor

    public SyncCodeHelper(CatalogSettings catalogSettings,
        IHttpContextAccessor httpContextAccessor,
        INopDataProvider dataProvider,
        IStaticCacheManager staticCacheManager,
        IStoreService storeService)
    {
        _catalogSettings = catalogSettings;
        _httpContextAccessor = httpContextAccessor;
        _dataProvider = dataProvider;
        _staticCacheManager = staticCacheManager;
        _storeService = storeService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Adds "deleted" filter to query which contains <see cref="ISoftDeletedEntity"/> entries, if its need
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="query">Entity entries</param>
    /// <param name="includeDeleted">Whether to include deleted items</param>
    /// <returns>Entity entries</returns>
    protected virtual IQueryable<TEntity> AddDeletedFilter<TEntity>(IQueryable<TEntity> query, in bool includeDeleted)
        where TEntity : BaseEntity
    {
        if (includeDeleted)
            return query;

        if (typeof(TEntity).GetInterface(nameof(ISoftDeletedEntity)) == null)
            return query;

        return query.OfType<ISoftDeletedEntity>().Where(entry => !entry.Deleted).OfType<TEntity>();
    }

    #endregion

    #region Methods

    #region Database

    /// <summary>
    /// Update entity entries
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entity entries</param>
    public virtual void UpdateEntities<TEntity>(IList<TEntity> entities)
        where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (!entities.Any())
            return;

        _dataProvider.UpdateEntities(entities);
    }

    /// <summary>
    /// Insert entity entries
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entities">Entity entries</param>
    public virtual void InsertEntities<TEntity>(IList<TEntity> entities)
        where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entities);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        _dataProvider.BulkInsertEntities(entities);
        transaction.Complete();
    }

    /// <summary>
    /// Delete entity entries by the passed predicate
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <returns>The number of deleted records</returns>
    public virtual int DeleteEntities<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(predicate);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var countDeletedRecords = _dataProvider.BulkDeleteEntities(predicate);
        transaction.Complete();

        return countDeletedRecords;
    }

    /// <summary>
    /// Insert the entity entry
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entity">Entity entry</param>
    public virtual void InsertEntity<TEntity>(TEntity entity)
        where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dataProvider.InsertEntity(entity);
    }

    /// <summary>
    /// Delete the entity entry
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entity">Entity entry</param>
    public virtual void DeleteEntity<TEntity>(TEntity entity)
        where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        switch (entity)
        {
            case ISoftDeletedEntity softDeletedEntity:
                softDeletedEntity.Deleted = true;
                _dataProvider.UpdateEntity(entity);
                break;

            default:
                _dataProvider.DeleteEntity(entity);
                break;
        }
    }

    /// <summary>
    /// Update the entity entry
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entity">Entity entry</param>
    public virtual void UpdateEntity<TEntity>(TEntity entity)
        where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dataProvider.UpdateEntity(entity);
    }

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="func">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
    /// <returns>Entity entries</returns>
    public virtual IList<TEntity> GetAllEntities<TEntity>(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true)
        where TEntity : BaseEntity
    {
        if (getCacheKey == null)
            return getAll();

        //caching
        var cacheKey = getCacheKey(_staticCacheManager)
            ?? _staticCacheManager.PrepareKeyForDefaultCache(NopEntityCacheDefaults<TEntity>.AllCacheKey);

        return _staticCacheManager.Get(cacheKey, getAll);

        IList<TEntity> getAll()
        {
            var query = AddDeletedFilter(_dataProvider.GetTable<TEntity>(), includeDeleted);
            query = func != null ? func(query) : query;

            return query.ToList();
        }
    }

    #endregion

    #region Language

    /// <summary>
    /// Gets all languages
    /// </summary>
    /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>The languages</returns>
    public virtual IList<Language> GetAllLanguages(bool showHidden = false, int storeId = 0)
    {
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopLocalizationDefaults.LanguagesAllCacheKey, storeId,
            showHidden);

        var languages = _staticCacheManager.Get(key, () =>
        {
            var allLanguages = GetAllEntities<Language>(query =>
            {
                if (!showHidden)
                    query = query.Where(l => l.Published);
                query = query.OrderBy(l => l.DisplayOrder).ThenBy(l => l.Id);

                return query;
            });

            //store mapping
            if (storeId > 0)
            {
                allLanguages = allLanguages
                    .Where(authorizeStoreMapping)
                    .ToList();
            }

            return allLanguages;
        });

        return languages;

        bool authorizeStoreMapping<TEntity>(TEntity entity)
            where TEntity : BaseEntity, IStoreMappingSupported
        {
            if (entity == null)
                return false;

            if (storeId == 0)
                //return true if no store specified/found
                return true;

            if (_catalogSettings.IgnoreStoreLimitations)
                return true;

            if (!entity.LimitedToStores)
                return true;

            return getStoresIdsWithAccess().Any(storeIdWithAccess => storeId == storeIdWithAccess);

            //no permission found
            int[] getStoresIdsWithAccess()
            {
                ArgumentNullException.ThrowIfNull(entity);

                var entityId = entity.Id;
                var entityName = entity.GetType().Name;

                var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopStoreDefaults.StoreMappingIdsCacheKey, entityId,
                    entityName);

                var query = GetAllEntities<StoreMapping>(query => query.Where(sm => sm.EntityId == entityId && sm.EntityName == entityName));

                return _staticCacheManager.Get(cacheKey, () => query.Select(sm => sm.StoreId).ToArray());
            }
        }
    }

    #endregion

    #region Store

    /// <summary>
    /// Gets the current store
    /// </summary>
    /// <returns>Store</returns>
    public virtual Store GetCurrentStore()
    {
        if (_cachedStore != null)
            return _cachedStore;

        //try to determine the current store by HOST header
        string host = _httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Host];

        var allStores = GetAllEntities<Store>(query => query.OrderBy(s => s.DisplayOrder).ThenBy(s => s.Id), _ => default, includeDeleted: false);
        var store = allStores
            .FirstOrDefault(s => _storeService.ContainsHostValue(s, host))
            ?? allStores.FirstOrDefault();

        _cachedStore = store ?? throw new Exception("No store could be loaded");

        return _cachedStore;
    }

    #endregion

    #endregion
}