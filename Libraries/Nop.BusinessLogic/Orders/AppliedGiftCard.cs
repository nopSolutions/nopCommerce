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
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Applied gift card
    /// </summary>
    public class AppliedGiftCard
    {
        #region Fields
        private GiftCard _gc = null;
        #endregion

        #region Ctor

        /// <summary>
        /// Creates a new instance of the AppliedGiftCard class
        /// </summary>
        public AppliedGiftCard()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the goft card identifier
        /// </summary>
        public int GiftCardId { get; set; }

        /// <summary>
        /// Gets or sets the used value
        /// </summary>
        public decimal AmountCanBeUsed { get; set; }

        #endregion

        #region Customer Properties

        /// <summary>
        /// Gets the gift card
        /// </summary>
        public GiftCard GiftCard
        {
            get
            {
                if (_gc == null)
                    _gc = IoC.Resolve<IOrderService>().GetGiftCardById(this.GiftCardId);
                return _gc;
            }
        }

        #endregion

    }
}
