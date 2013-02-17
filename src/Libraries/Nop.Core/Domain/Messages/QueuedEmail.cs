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
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the From property
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the FromName property
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// Gets or sets the To property
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the ToName property
        /// </summary>
        public string ToName { get; set; }

        /// <summary>
        /// Gets or sets the CC
        /// </summary>
        public string CC { get; set; }

        /// <summary>
        /// Gets or sets the Bcc
        /// </summary>
        public string Bcc { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the date and time of item creation in UTC
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        public int SentTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public DateTime? SentOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the used email account identifier
        /// </summary>
        public int EmailAccountId { get; set; }

        /// <summary>
        /// Gets the email account
        /// </summary>
        public virtual EmailAccount EmailAccount { get; set; }
    }
}
