namespace Nop.Core.Events {
    public static class EventPublisherExtensions {
        public static void EntityInserted<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity {
            eventPublisher.Publish(new EntityInserted<T>(entity));
        }

        public static void EntityUpdated<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity {
            eventPublisher.Publish(new EntityUpdated<T>(entity));
        }

        public static void EntityDeleted<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity {
            eventPublisher.Publish(new EntityDeleted<T>(entity));
        }

        /// <summary>
        /// Publishes the subscribe event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="email">The email.</param>
        public static void PublishSubscribe(this IEventPublisher eventPublisher, string email) {
            eventPublisher.Publish(new EmailSubscribed(email));
        }

        /// <summary>
        /// Publishes the unsubscribe event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="email">The email.</param>
        public static void PublishUnsubscribe(this IEventPublisher eventPublisher, string email) {
            eventPublisher.Publish(new EmailUnsubscribed(email));
        }
    }
}