namespace Nop.Core.Events;

/// <summary>
/// Event publisher extensions
/// </summary>
public static class EventPublisherExtensions
{
    /// <summary>
    /// Entity inserted
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task EntityInsertedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
    {
        await eventPublisher.PublishAsync(new EntityInsertedEvent<T>(entity));
    }

    /// <summary>
    /// Entity inserted
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="entity">Entity</param>
    public static void EntityInserted<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
    {
        eventPublisher.Publish(new EntityInsertedEvent<T>(entity));
    }

    /// <summary>
    /// Entity updated
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task EntityUpdatedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
    {
        await eventPublisher.PublishAsync(new EntityUpdatedEvent<T>(entity));
    }

    /// <summary>
    /// Entity updated
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="entity">Entity</param>
    public static void EntityUpdated<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
    {
        eventPublisher.Publish(new EntityUpdatedEvent<T>(entity));
    }

    /// <summary>
    /// Entity deleted
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task EntityDeletedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
    {
        await eventPublisher.PublishAsync(new EntityDeletedEvent<T>(entity));
    }

    /// <summary>
    /// Entity deleted
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="entity">Entity</param>
    public static void EntityDeleted<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
    {
        eventPublisher.Publish(new EntityDeletedEvent<T>(entity));
    }
}