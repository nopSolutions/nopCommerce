using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Events;

namespace Nop.Services.Caching
{
    public abstract partial class CacheEventConsumer<TEntity> : IConsumer<EntityInsertedEvent<TEntity>>,
        IConsumer<EntityUpdatedEvent<TEntity>>,
        IConsumer<EntityDeletedEvent<TEntity>> where TEntity : BaseEntity
    {
        protected readonly ICacheKeyService _cacheKeyService;
        private readonly IStaticCacheManager _staticCacheManager;

        protected CacheEventConsumer()
        {
            _cacheKeyService = EngineContext.Current.Resolve<ICacheKeyService>();
            _staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
        }

        /// <summary>
        /// entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected virtual void ClearCache(TEntity entity, EntityEventType entityEventType)
        {
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
        /// Removes items by key prefix
        /// </summary>
        /// <param name="prefixCacheKey">String key prefix</param>
        protected virtual void RemoveByPrefix(string prefixCacheKey)
        {
            _staticCacheManager.RemoveByPrefix(prefixCacheKey);
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="cacheKey">Key of cached item</param>
        protected virtual void Remove(CacheKey cacheKey)
        {
            _staticCacheManager.Remove(cacheKey);
        }

        /// <summary>
        /// Handle entity inserted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public virtual void HandleEvent(EntityInsertedEvent<TEntity> eventMessage)
        {
            var entity = eventMessage.Entity;
            ClearCache(entity, EntityEventType.Insert);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public virtual void HandleEvent(EntityUpdatedEvent<TEntity> eventMessage)
        {
            var entity = eventMessage.Entity;

            _staticCacheManager.Remove(new CacheKey(entity.EntityCacheKey));
            ClearCache(eventMessage.Entity, EntityEventType.Update);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public virtual void HandleEvent(EntityDeletedEvent<TEntity> eventMessage)
        {
            var entity = eventMessage.Entity;

            _staticCacheManager.Remove(new CacheKey(entity.EntityCacheKey));
            ClearCache(eventMessage.Entity, EntityEventType.Delete);
        }

        protected enum EntityEventType
        {
            Insert,
            Update,
            Delete
        }
    }
}