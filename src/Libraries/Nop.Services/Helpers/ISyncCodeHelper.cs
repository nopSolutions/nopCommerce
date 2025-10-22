using System.Linq.Expressions;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;

namespace Nop.Services.Helpers;

/// <summary>
/// User synchronous code helper interface
/// </summary>
public partial interface ISyncCodeHelper
{
    #region Database

    /// <summary>
    /// Get the entity entry
    /// </summary>
    /// <param name="id">Entity entry identifier</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="ISoftDeletedEntity"/> entities)</param>
    /// <returns>
    /// The entity entry
    /// </returns>
    TEntity GetEntityById<TEntity>(int? id, Func<ICacheKeyService, CacheKey> getCacheKey = null,
        bool includeDeleted = true)
        where TEntity : BaseEntity;

    /// <summary>
    /// Update entity entries
    /// </summary>
    /// <param name="entities">Entity entries</param>
    void UpdateEntities<TEntity>(IList<TEntity> entities)
        where TEntity : BaseEntity;

    /// <summary>
    /// Insert entity entries
    /// </summary>
    /// <param name="entities">Entity entries</param>
    void InsertEntities<TEntity>(IList<TEntity> entities)
        where TEntity : BaseEntity;

    /// <summary>
    /// Delete entity entries by the passed predicate
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <returns>
    /// The number of deleted records
    /// </returns>
    int DeleteEntities<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : BaseEntity;

    /// <summary>
    /// Insert the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    void InsertEntity<TEntity>(TEntity entity)
        where TEntity : BaseEntity;

    /// <summary>
    /// Delete the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    void DeleteEntity<TEntity>(TEntity entity)
        where TEntity : BaseEntity;

    /// <summary>
    /// Update the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    void UpdateEntity<TEntity>(TEntity entity)
        where TEntity : BaseEntity;

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="getCacheKey">Function to get a cache key; pass null to don't cache; return null from this function to use the default key</param>
    /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
    /// <returns>Entity entries</returns>
    IList<TEntity> GetAllEntities<TEntity>(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
        Func<ICacheKeyService, CacheKey> getCacheKey = null, bool includeDeleted = true)
        where TEntity : BaseEntity;

    #endregion

    #region Language

    /// <summary>
    /// Gets all languages
    /// </summary>
    /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// The languages
    /// </returns>
    IList<Language> GetAllLanguages(bool showHidden = false, int storeId = 0);

    #endregion

    #region Store

    /// <summary>
    /// Gets the current store
    /// </summary>
    Store GetCurrentStore();

    #endregion
}