namespace Nop.Core.Domain.Messages
{
    /// <summary>
    /// Email unsubscribed event
    /// </summary>
    public class EmailUnsubscribedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="subscription">Subscription</param>
        public EmailUnsubscribedEvent(NewsLetterSubscription subscription)
        {
            Subscription = subscription;
        }

        /// <summary>
        /// Subscription
        /// </summary>
        public NewsLetterSubscription Subscription { get; }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other">Other event</param>
        /// <returns>Result</returns>
        public bool Equals(EmailUnsubscribedEvent other)
        {
            if (other is null) 
                return false;

            if (ReferenceEquals(this, other)) 
                return true;

            return Equals(other.Subscription, Subscription);
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Result</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) 
                return false;

            if (ReferenceEquals(this, obj)) 
                return true;

            if (obj.GetType() != typeof(EmailUnsubscribedEvent)) 
                return false;

            return Equals((EmailUnsubscribedEvent)obj);
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return Subscription != null ? Subscription.GetHashCode() : 0;
        }
    }
}