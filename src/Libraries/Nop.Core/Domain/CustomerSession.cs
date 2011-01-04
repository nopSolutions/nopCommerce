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

namespace Nop.Core.Domain
{
    /// <summary>
    /// Represents a customer session
    /// </summary>
    public partial class CustomerSession : BaseEntity
    {
        /// <summary>
        /// Gets or sets the customer session identifier
        /// </summary>
        public Guid CustomerSessionGuid { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the last accessed date and time
        /// </summary>
        public DateTime LastAccessedUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer session is expired
        /// </summary>
        public bool IsExpired { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }
    }

}
