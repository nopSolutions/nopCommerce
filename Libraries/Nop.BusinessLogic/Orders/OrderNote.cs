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
using System.Collections.Generic;
using System.Text;


namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Represents an order note
    /// </summary>
    public partial class OrderNote : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the OrderNote class
        /// </summary>
        public OrderNote()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the order note identifier
        /// </summary>
        public int OrderNoteId { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer can see a note
        /// </summary>
        public bool DisplayToCustomer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the date and time of order note creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the order
        /// </summary>
        public virtual Order NpOrder { get; set; }

        #endregion
    }

}
