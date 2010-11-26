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

using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Messages
{
    /// <summary>
    /// Represents a localized message template
    /// </summary>
    public partial class LocalizedMessageTemplate : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the LocalizedMessageTemplate class
        /// </summary>
        public LocalizedMessageTemplate()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the localized message template identifier
        /// </summary>
        public int MessageTemplateLocalizedId { get; set; }

        /// <summary>
        /// Gets or sets the message template identifier
        /// </summary>
        public int MessageTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the BCC Email addresses
        /// </summary>
        public string BccEmailAddresses { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the template is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the used email account identifier
        /// </summary>
        public int EmailAccountId { get; set; }

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets the language
        /// </summary>
        public Language Language
        {
            get
            {
                return IoC.Resolve<ILanguageService>().GetLanguageById(this.LanguageId);
            }
        }

        /// <summary>
        /// Gets the message template
        /// </summary>
        public MessageTemplate MessageTemplate
        {
            get
            {
                return IoC.Resolve<IMessageService>().GetMessageTemplateById(this.MessageTemplateId);
            }
        }

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

        #region Navigation Properties

        /// <summary>
        /// Gets the message template
        /// </summary>
        public virtual MessageTemplate NpMessageTemplate { get; set; }

        #endregion
    }

}
