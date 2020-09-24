using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Events;

namespace Nop.Services.Caching
{
    /// <summary>
    /// Represents the base entity cache event consumer
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract partial class CacheEventConsumer<TEntity> :
        IConsumer<EntityInsertedEvent<TEntity>>,
        IConsumer<EntityUpdatedEvent<TEntity>>,
        IConsumer<EntityDeletedEvent<TEntity>>
        where TEntity : BaseEntity
    {
        #region Fields

        protected readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        protected CacheEventConsumer()
        {
            _staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected virtual void ClearCache(TEntity entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Insert)
            {
                RemoveByPrefix(NopEntityCacheDefaults<TEntity>.ByIdsPrefix);
                RemoveByPrefix(NopEntityCacheDefaults<TEntity>.AllPrefix);
            }

            if (entityEventType == EntityEventType.Update)
            {
                Remove(NopEntityCacheDefaults<TEntity>.ByIdCacheKey, entity);
                RemoveByPrefix(NopEntityCacheDefaults<TEntity>.ByIdsPrefix);
                RemoveByPrefix(NopEntityCacheDefaults<TEntity>.AllPrefix);
            }

            if (entityEventType == EntityEventType.Delete)
            {
                Remove(NopEntityCacheDefaults<TEntity>.ByIdCacheKey, entity);
                RemoveByPrefix(NopEntityCacheDefaults<TEntity>.ByIdsPrefix);
                RemoveByPrefix(NopEntityCacheDefaults<TEntity>.AllPrefix);
            }

            ClearCache(entity);
        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected virtual void ClearCache(TEntity entity)
        {
        }

        /// <summary>
        /// Removes items by cache key prefix
        /// </summary>
        /// <param name="prefix">Cache key prefix</param>
        /// <param name="prefixParameters">Parameters to create cache key prefix</param>
        protected virtual void RemoveByPrefix(string prefix, params object[] prefixParameters)
        {
            _staticCacheManager.RemoveByPrefix(prefix, prefixParameters);
        }

        /// <summary>
        /// Remove the value with the specified key from the cache
        /// </summary>
        /// <param name="cacheKey">Cache key</param>
        /// <param name="cacheKeyParameters">Parameters to create cache key</param>
        public void Remove(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            _staticCacheManager.Remove(cacheKey, cacheKeyParameters);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle entity inserted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public virtual void HandleEvent(EntityInsertedEvent<TEntity> eventMessage)
        {
            ClearCache(eventMessage.Entity, EntityEventType.Insert);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public virtual void HandleEvent(EntityUpdatedEvent<TEntity> eventMessage)
        {
            ClearCache(eventMessage.Entity, EntityEventType.Update);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public virtual void HandleEvent(EntityDeletedEvent<TEntity> eventMessage)
        {
            ClearCache(eventMessage.Entity, EntityEventType.Delete);
        }

        #endregion

        #region Nested

        protected enum EntityEventType
        {
            Insert,
            Update,
            Delete
        }

        #endregion
    }
}