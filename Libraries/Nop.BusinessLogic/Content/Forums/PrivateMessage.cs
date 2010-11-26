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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Content.Forums
{
    /// <summary>
    /// Represents a private message
    /// </summary>
    public partial class PrivateMessage : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the PrivateMessage class
        /// </summary>
        public PrivateMessage()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the private message identifier
        /// </summary>
        public int PrivateMessageId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier who sent the message
        /// </summary>
        public int FromUserId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier who should receive the message
        /// </summary>
        public int ToUserId { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indivating whether message is read
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Gets or sets a value indivating whether message is deleted by author
        /// </summary>
        public bool IsDeletedByAuthor { get; set; }

        /// <summary>
        /// Gets or sets a value indivating whether message is deleted by recipient
        /// </summary>
        public bool IsDeletedByRecipient { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        #endregion
        
        #region Custom Properties

        /// <summary>
        /// Gets the user who sent the message
        /// </summary>
        public Customer FromUser
        {
            get
            {
                return IoC.Resolve<ICustomerService>().GetCustomerById(this.FromUserId);
            }
        }

        /// <summary>
        /// Gets the user who should receive the message
        /// </summary>
        public Customer ToUser
        {
            get
            {
                return IoC.Resolve<ICustomerService>().GetCustomerById(this.ToUserId);
            }
        }

        #endregion
    }
}