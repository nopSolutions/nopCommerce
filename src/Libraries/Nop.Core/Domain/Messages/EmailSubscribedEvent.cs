namespace Nop.Core.Domain.Messages
{
    /// <summary>
    /// Email subscribed event
    /// </summary>
    public partial class EmailSubscribedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="subscription">Subscription</param>
        public EmailSubscribedEvent(NewsLetterSubscription subscription)
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
        public bool Equals(EmailSubscribedEvent other)
        {
            if (ReferenceEquals(null, other)) 
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

            if (obj.GetType() != typeof(EmailSubscribedEvent)) 
                return false;

            return Equals((EmailSubscribedEvent)obj);
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