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

using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;

namespace NopSolutions.NopCommerce.BusinessLogic.Content.Polls
{
    /// <summary>
    /// Represents a poll voting record
    /// </summary>
    public partial class PollVotingRecord : BaseEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the poll voting record identifier
        /// </summary>
        public int PollVotingRecordId { get; set; }

        /// <summary>
        /// Gets or sets the poll answer identifier
        /// </summary>
        public int PollAnswerId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the poll answer
        /// </summary>
        public virtual PollAnswer NpPollAnswer { get; set; }

        /// <summary>
        /// Gets the customer
        /// </summary>
        public virtual Customer NpCustomer { get; set; }

        #endregion
    }

}
