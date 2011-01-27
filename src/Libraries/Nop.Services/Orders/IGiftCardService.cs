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
using System.Globalization;
using System.Linq;
using System.Text;
using Nop.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Customers;
using System.Xml;
using System.Diagnostics;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Gift card service interface
    /// </summary>
    public partial interface IGiftCardService
    {
        /// <summary>
        /// Deletes a gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        void DeleteGiftCard(GiftCard giftCard);

        /// <summary>
        /// Gets a gift card
        /// </summary>
        /// <param name="giftCardId">Gift card identifier</param>
        /// <returns>Gift card entry</returns>
        GiftCard GetGiftCardById(int giftCardId);

        /// <summary>
        /// Gets all gift cards
        /// </summary>
        /// <param name="startTime">Order start time; null to load all records</param>
        /// <param name="endTime">Order end time; null to load all records</param>
        /// <param name="isGiftCardActivated">Value indicating whether gift card is activated; null to load all records</param>
        /// <param name="giftCardCouponCode">Gift card coupon code; null or string.empty to load all records</param>
        /// <returns>Gift cards</returns>
        IList<GiftCard> GetAllGiftCards(DateTime? startTime = null, DateTime? endTime = null,
            bool? isGiftCardActivated = null, string giftCardCouponCode = "");

        /// <summary>
        /// Inserts a gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        void InsertGiftCard(GiftCard giftCard);

        /// <summary>
        /// Updates the gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        void UpdateGiftCard(GiftCard giftCard);


        /// <summary>
        /// Get active gift cards that are applied by a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Active gift cards</returns>
        IList<GiftCard> GetActiveGiftCardsAppliedByCustomer(Customer customer);

        /// <summary>
        /// Generate new gift card code
        /// </summary>
        /// <returns>Result</returns>
        string GenerateGiftCardCode();
    }
}
