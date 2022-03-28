using EllaSoftware.Plugin.Misc.CronTasks.Infrastructure;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Events;
using Nop.Services.Events;
using System.Threading.Tasks;

namespace EllaSoftware.Plugin.Misc.CronTasks.EventConsumers
{
    public class GenericAttributeEventConsumer :
        IConsumer<EntityDeletedEvent<GenericAttribute>>,
        IConsumer<EntityInsertedEvent<GenericAttribute>>,
        IConsumer<EntityUpdatedEvent<GenericAttribute>>
    {
        private readonly IStaticCacheManager _cacheManager;

        public GenericAttributeEventConsumer(
            IStaticCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public async Task HandleEventAsync(EntityDeletedEvent<GenericAttribute> eventMessage)
        {
            if (eventMessage.Entity.Key == CronTasksDefaults.CronExpressionGenericAttributeKey)
                await _cacheManager.RemoveAsync(new CacheKey(CronTasksCacheKey.CRON_TASKS_ALL_KEY));
        }

        public async Task HandleEventAsync(EntityInsertedEvent<GenericAttribute> eventMessage)
        {
            if (eventMessage.Entity.Key == CronTasksDefaults.CronExpressionGenericAttributeKey)
                await _cacheManager.RemoveAsync(new CacheKey(CronTasksCacheKey.CRON_TASKS_ALL_KEY));
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<GenericAttribute> eventMessage)
        {
            if (eventMessage.Entity.Key == CronTasksDefaults.CronExpressionGenericAttributeKey)
                await _cacheManager.RemoveAsync(new CacheKey(CronTasksCacheKey.CRON_TASKS_ALL_KEY));
        }
    }
}