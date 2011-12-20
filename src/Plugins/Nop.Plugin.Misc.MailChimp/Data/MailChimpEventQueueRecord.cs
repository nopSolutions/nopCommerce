using System;
using Nop.Core;

namespace Nop.Plugin.Misc.MailChimp.Data
{
    public class MailChimpEventQueueRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is subscribe.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is subscribe; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsSubscribe { get; set; }

        public virtual DateTime CreatedOnUtc { get; set; }
    }
}