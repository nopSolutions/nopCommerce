using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Events;
using Nop.Core.Infrastructure;

namespace Nop.Services.Caching.CacheEventConsumers
{
    public abstract class CacheEventConsumer<T> : IConsumer<EntityInsertedEvent<T>>,
        IConsumer<EntityUpdatedEvent<T>>,
        IConsumer<EntityDeletedEvent<T>> where T : BaseEntity
    {
        protected readonly IStaticCacheManager _cacheManager;

        protected CacheEventConsumer()
        {
            _cacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
        }

        public abstract void ClearCashe(T entity);

        public virtual void HandleEvent(EntityInsertedEvent<T> eventMessage)
        {
            ClearCashe(eventMessage.Entity);
        }

        public virtual void HandleEvent(EntityUpdatedEvent<T> eventMessage)
        {
            ClearCashe(eventMessage.Entity);
        }

        public virtual void HandleEvent(EntityDeletedEvent<T> eventMessage)
        {
            ClearCashe(eventMessage.Entity);
        }
    }
}
