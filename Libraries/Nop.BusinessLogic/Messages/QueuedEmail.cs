//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Messages
{
    /// <summary>
    /// Represents an email item
    /// </summary>
    public partial class QueuedEmail : BaseEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the ueued email identifier
        /// </summary>
        public int QueuedEmailId { get; set; }

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
        /// Gets or sets the date and time of item creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        public int SendTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public DateTime? SentOn { get; set; }

        /// <summary>
        /// Gets or sets the used email account identifier
        /// </summary>
        public int EmailAccountId { get; set; }

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets the used email account
        /// </summary>
        public EmailAccount EmailAccount
        {
            get
            {
                var emailAccount = IoC.Resolve<IMessageService>().GetEmailAccountById(this.EmailAccountId);
                if (emailAccount == null)
                    emailAccount = IoC.Resolve<IMessageService>().DefaultEmailAccount;
                return emailAccount;
            }
        }

        #endregion
    }
}
