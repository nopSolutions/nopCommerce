using System;

namespace Nop.Core.Domain.Messages
{
    /// <summary>
    /// Represents an email item
    /// </summary>
    public partial class QueuedEmail : BaseEntity
    {
        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        public virtual int Priority { get; set; }

        /// <summary>
        /// Gets or sets the From property
        /// </summary>
        public virtual string From { get; set; }

        /// <summary>
        /// Gets or sets the FromName property
        /// </summary>
        public virtual string FromName { get; set; }

        /// <summary>
        /// Gets or sets the To property
        /// </summary>
        public virtual string To { get; set; }

        /// <summary>
        /// Gets or sets the ToName property
        /// </summary>
        public virtual string ToName { get; set; }

        /// <summary>
        /// Gets or sets the CC
        /// </summary>
        public virtual string CC { get; set; }

        /// <summary>
        /// Gets or sets the Bcc
        /// </summary>
        public virtual string Bcc { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public virtual string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public virtual string Body { get; set; }

        /// <summary>
        /// Gets or sets the date and time of item creation in UTC
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        public virtual int SentTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public virtual DateTime? SentOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the used email account identifier
        /// </summary>
        public virtual int EmailAccountId { get; set; }

        /// <summary>
        /// Gets the email account
        /// </summary>
        public virtual EmailAccount EmailAccount { get; set; }
    }
}
