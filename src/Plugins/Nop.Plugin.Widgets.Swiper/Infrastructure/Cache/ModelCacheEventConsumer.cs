using Nop.Core.Caching;
using Nop.Core.Domain.Configuration;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Nop.Plugin.Widgets.Swiper.Infrastructure.Cache;

/// <summary>
/// Model cache event consumer (used for caching of presentation layer models)
/// </summary>
public class ModelCacheEventConsumer(IStaticCacheManager staticCacheManager) :
    IConsumer<EntityInsertedEvent<Setting>>,
    IConsumer<EntityUpdatedEvent<Setting>>,
    IConsumer<EntityDeletedEvent<Setting>>
{
    #region Methods

    /// <summary>
    /// Handle entity inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<Setting> eventMessage)
    {
        await staticCacheManager.RemoveByPrefixAsync(PictureUrlPrefix);
    }

    /// <summary>
    /// Handle entity updated event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
    {
        await staticCacheManager.RemoveByPrefixAsync(PictureUrlPrefix);
    }

    /// <summary>
    /// Handle entity deleted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<Setting> eventMessage)
    {
        await staticCacheManager.RemoveByPrefixAsync(PictureUrlPrefix);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a key pattern to clear cache
    /// </summary>
    protected static string PictureUrlPrefix => "Nop.plugins.widgets.swiper";

    /// <summary>
    /// Key for caching
    /// </summary>
    /// <remarks>
    /// {0} : picture id
    /// {1} : secured
    /// </remarks>
    public static CacheKey PictureUrlModelKey => new("Nop.plugins.widgets.swiper.pictureurl-{0}-{1}");

    #endregion
}