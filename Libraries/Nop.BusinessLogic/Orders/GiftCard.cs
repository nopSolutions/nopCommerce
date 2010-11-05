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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Represents a gift card
    /// </summary>
    public partial class GiftCard : BaseEntity
    {
        #region Fields
        private OrderProductVariant _opv = null;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the GiftCardUsageHistory class
        /// </summary>
        public GiftCard()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the gift card identifier
        /// </summary>
        public int GiftCardId { get; set; }

        /// <summary>
        /// Gets or sets the purchased order product variant identifier
        /// </summary>
        public int PurchasedOrderProductVariantId { get; set; }

        /// <summary>
        /// Gets or sets the amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gift card is activated
        /// </summary>
        public bool IsGiftCardActivated { get; set; }

        /// <summary>
        /// Gets or sets a gift card coupon code
        /// </summary>
        public string GiftCardCouponCode { get; set; }

        /// <summary>
        /// Gets or sets a recipient name
        /// </summary>
        public string RecipientName { get; set; }

        /// <summary>
        /// Gets or sets a recipient email
        /// </summary>
        public string RecipientEmail { get; set; }

        /// <summary>
        /// Gets or sets a sender name
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// Gets or sets a sender email
        /// </summary>
        public string SenderEmail { get; set; }

        /// <summary>
        /// Gets or sets a message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether recipient is notified
        /// </summary>
        public bool IsRecipientNotified { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        #endregion

        #region Customer Properties

        /// <summary>
        /// Gets the purchased order product variant
        /// </summary>
        public OrderProductVariant PurchasedOrderProductVariant
        {
            get
            {
                if (_opv == null)
                    _opv = IoCFactory.Resolve<IOrderService>().GetOrderProductVariantById(PurchasedOrderProductVariantId);
                return _opv;
            }
        }
        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the order product variant (initial product)
        /// </summary>
        public virtual OrderProductVariant NpOrderProductVariant { get; set; }

        /// <summary>
        /// Gets the gift card usage history
        /// </summary>
        public virtual ICollection<GiftCardUsageHistory> NpGiftCardUsageHistory { get; set; }
        
        #endregion
    }
}
