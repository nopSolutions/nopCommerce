using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;

namespace Nop.Services.Messages;

/// <summary>
/// Event publisher extensions
/// </summary>
public static class EventPublisherExtensions
{
    /// <summary>
    /// Publishes the newsletter subscribe event.
    /// </summary>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="subscription">The newsletter subscription.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task PublishNewsLetterSubscribeAsync(this IEventPublisher eventPublisher, NewsLetterSubscription subscription)
    {
        await eventPublisher.PublishAsync(new EmailSubscribedEvent(subscription));
    }

    /// <summary>
    /// Publishes the newsletter unsubscribe event.
    /// </summary>
    /// <param name="eventPublisher">The event publisher.</param>
    /// <param name="subscription">The newsletter subscription.</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task PublishNewsLetterUnsubscribeAsync(this IEventPublisher eventPublisher, NewsLetterSubscription subscription)
    {
        await eventPublisher.PublishAsync(new EmailUnsubscribedEvent(subscription));
    }

    /// <summary>
    /// Entity tokens added
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="entity">Entity</param>
    /// <param name="tokens">Tokens</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task EntityTokensAddedAsync<T>(this IEventPublisher eventPublisher, T entity, IList<Token> tokens) where T : BaseEntity
    {
        await eventPublisher.PublishAsync(new EntityTokensAddedEvent<T>(entity, tokens));
    }

    /// <summary>
    /// Message token added
    /// </summary>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="message">Message</param>
    /// <param name="tokens">Tokens</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task MessageTokensAddedAsync(this IEventPublisher eventPublisher, MessageTemplate message, IList<Token> tokens)
    {
        await eventPublisher.PublishAsync(new MessageTokensAddedEvent(message, tokens));
    }
}