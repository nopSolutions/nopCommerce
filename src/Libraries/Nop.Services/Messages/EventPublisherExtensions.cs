using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.Events;

namespace Nop.Services.Messages
{
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
        public static void PublishNewsletterSubscribe(this IEventPublisher eventPublisher, NewsLetterSubscription subscription)
        {
            eventPublisher.Publish(new EmailSubscribedEvent(subscription));
        }

        /// <summary>
        /// Publishes the newsletter unsubscribe event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="subscription">The newsletter subscription.</param>
        public static void PublishNewsletterUnsubscribe(this IEventPublisher eventPublisher, NewsLetterSubscription subscription)
        {
            eventPublisher.Publish(new EmailUnsubscribedEvent(subscription));
        }

        /// <summary>
        /// Entity tokens added
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <typeparam name="U">Type</typeparam>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="entity">Entity</param>
        /// <param name="tokens">Tokens</param>
        public static void EntityTokensAdded<T, U>(this IEventPublisher eventPublisher, T entity, System.Collections.Generic.IList<U> tokens) where T : BaseEntity
        {
            eventPublisher.Publish(new EntityTokensAddedEvent<T, U>(entity, tokens));
        }

        /// <summary>
        /// Message token added
        /// </summary>
        /// <typeparam name="U">Type</typeparam>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="message">Message</param>
        /// <param name="tokens">Tokens</param>
        public static void MessageTokensAdded<U>(this IEventPublisher eventPublisher, MessageTemplate message, System.Collections.Generic.IList<U> tokens)
        {
            eventPublisher.Publish(new MessageTokensAddedEvent<U>(message, tokens));
        }
    }
}