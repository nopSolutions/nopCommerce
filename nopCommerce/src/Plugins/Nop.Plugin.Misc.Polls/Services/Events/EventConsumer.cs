using Nop.Core.Caching;
using Nop.Core.Events;
using Nop.Plugin.Misc.Polls.Domain;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.Polls.Services.Events;

public class EventConsumer :
    IConsumer<EntityInsertedEvent<Poll>>,
    IConsumer<EntityUpdatedEvent<Poll>>,
    IConsumer<EntityDeletedEvent<Poll>>
{
    #region Fields

    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public EventConsumer(IStaticCacheManager staticCacheManager)
    {
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle entity inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<Poll> eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(PollsDefaults.PollsPrefixCacheKey);
    }

    /// <summary>
    /// Handle entity updated event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<Poll> eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(PollsDefaults.PollsPrefixCacheKey);
    }

    /// <summary>
    /// Handle entity deleted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<Poll> eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(PollsDefaults.PollsPrefixCacheKey);
    }

    #endregion
}