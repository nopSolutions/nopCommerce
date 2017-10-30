using System.Collections.Generic;

namespace Nop.Core.Domain.Messages
{
    /// <summary>
    /// Email subscribed event
    /// </summary>
    public class EmailSubscribedEvent
    {
        private readonly NewsLetterSubscription _subscription;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="subscription">Subscription</param>
        public EmailSubscribedEvent(NewsLetterSubscription subscription)
        {
            _subscription = subscription;
        }

        /// <summary>
        /// Subscription
        /// </summary>
        public NewsLetterSubscription Subscription
        {
            get { return _subscription; }
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other">Other event</param>
        /// <returns>Result</returns>
        public bool Equals(EmailSubscribedEvent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._subscription, _subscription);
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Result</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(EmailSubscribedEvent)) return false;
            return Equals((EmailSubscribedEvent)obj);
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return (_subscription != null ? _subscription.GetHashCode() : 0);
        }
    }

    /// <summary>
    /// Email unsubscribed event
    /// </summary>
    public class EmailUnsubscribedEvent
    {
        private readonly NewsLetterSubscription _subscription;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="subscription">Subscription</param>
        public EmailUnsubscribedEvent(NewsLetterSubscription subscription)
        {
            _subscription = subscription;
        }

        /// <summary>
        /// Subscription
        /// </summary>
        public NewsLetterSubscription Subscription
        {
            get { return _subscription; }
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other">Other event</param>
        /// <returns>Result</returns>
        public bool Equals(EmailUnsubscribedEvent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._subscription, _subscription);
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Result</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(EmailUnsubscribedEvent)) return false;
            return Equals((EmailUnsubscribedEvent)obj);
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return (_subscription != null ? _subscription.GetHashCode() : 0);
        }
    }

    /// <summary>
    /// A container for tokens that are added.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="U"></typeparam>
    public class EntityTokensAddedEvent<T, U> where T : BaseEntity
    {
        private readonly T _entity;
        private readonly IList<U> _tokens;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="tokens">Tokens</param>
        public EntityTokensAddedEvent(T entity, IList<U> tokens)
        {
            _entity = entity;
            _tokens = tokens;
        }

        /// <summary>
        /// Entity
        /// </summary>
        public T Entity { get { return _entity; } }
        /// <summary>
        /// Tokens
        /// </summary>
        public IList<U> Tokens { get { return _tokens; } }
    }

    /// <summary>
    /// A container for tokens that are added.
    /// </summary>
    /// <typeparam name="U">Type</typeparam>
    public class MessageTokensAddedEvent<U>
    {
        private readonly MessageTemplate _message;
        private readonly IList<U> _tokens;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="tokens">Tokens</param>
        public MessageTokensAddedEvent(MessageTemplate message, IList<U> tokens)
        {
            _message = message;
            _tokens = tokens;
        }

        /// <summary>
        /// Message
        /// </summary>
        public MessageTemplate Message { get { return _message; } }
        /// <summary>
        /// Tokens
        /// </summary>
        public IList<U> Tokens { get { return _tokens; } }
    }

    /// <summary>
    /// Event for "Additional tokens added"
    /// </summary>
    public class AdditionTokensAddedEvent
    {
        private readonly IList<string> _tokens;

        /// <summary>
        /// Ctor
        /// </summary>
        public AdditionTokensAddedEvent()
        {
            this._tokens=new List<string>();
        }

        /// <summary>
        /// Add tokens
        /// </summary>
        /// <param name="additionTokens">Additional tokens</param>
        public void AddTokens(params string[] additionTokens)
        {
            foreach (var additionToken in additionTokens)
            {
                this._tokens.Add(additionToken);
            }
        }

        /// <summary>
        /// Additional tokens
        /// </summary>
        public IList<string> AdditionTokens { get { return _tokens; } }
    }

    /// <summary>
    /// Event for "Additional tokens added for campaigns"
    /// </summary>
    public class CampaignAdditionTokensAddedEvent : AdditionTokensAddedEvent
    {
    }
}