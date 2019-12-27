using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Events;
using Nop.Core.Infrastructure;

namespace Nop.Services.Caching.CacheEventConsumers
{
    public abstract partial class CacheEventConsumer<TEntity> : IConsumer<EntityInsertedEvent<TEntity>>,
        IConsumer<EntityUpdatedEvent<TEntity>>,
        IConsumer<EntityDeletedEvent<TEntity>> where TEntity : BaseEntity
    {
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ICacheManager _cacheManager;

        protected CacheEventConsumer()
        {
            _staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
            _cacheManager = EngineContext.Current.Resolve<ICacheManager>();
        }

        public virtual void ClearCashe(TEntity entity) { }

        protected virtual void RemoveByPrefix(string prefixCacheKey, bool useStaticCashe = true)
        {
            if (useStaticCashe)
                _staticCacheManager.RemoveByPrefix(prefixCacheKey);
            else
                _cacheManager.RemoveByPrefix(prefixCacheKey);
        }

        protected virtual void Remove(string cacheKey, bool useStaticCashe = true)
        {
            if (useStaticCashe)
                _staticCacheManager.Remove(cacheKey);
            else
                _cacheManager.Remove(cacheKey);
        }

        public virtual void HandleEvent(EntityInsertedEvent<TEntity> eventMessage)
        {
            var entity = eventMessage.Entity;
            ClearCashe(entity);
        }

        public virtual void HandleEvent(EntityUpdatedEvent<TEntity> eventMessage)
        {
            var entity = eventMessage.Entity;

            _staticCacheManager.Remove(string.Format(NopCachingDefaults.NopEntityCacheKey, typeof(TEntity).Name, entity.Id));
            ClearCashe(eventMessage.Entity);
        }

        public virtual void HandleEvent(EntityDeletedEvent<TEntity> eventMessage)
        {
            var entity = eventMessage.Entity;

            _staticCacheManager.Remove(string.Format(NopCachingDefaults.NopEntityCacheKey, typeof(TEntity).Name, entity.Id));
            ClearCashe(eventMessage.Entity);
        }
    }
}

