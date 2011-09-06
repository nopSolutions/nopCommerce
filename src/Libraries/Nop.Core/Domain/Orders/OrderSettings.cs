using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Orders
{
    public class OrderSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether customer can make re-order
        /// </summary>
        public bool IsReOrderAllowed { get; set; }

        /// <summary>
        /// Gets or sets a minimum order subtotal amount
        /// </summary>
        public decimal MinOrderSubtotalAmount { get; set; }

        /// <summary>
        /// Gets or sets a minimum order total amount
        /// </summary>
        public decimal MinOrderTotalAmount { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether anonymous checkout allowed
        /// </summary>
        public bool AnonymousCheckoutAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Terms of service' enabled
        /// </summary>
        public bool TermsOfServiceEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'One-page checkout' is enabled
        /// </summary>
        public bool OnePageCheckoutEnabled { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether "Return requests" are allowed
        /// </summary>
        public bool ReturnRequestsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a list of return request reasons
        /// </summary>
        public List<string> ReturnRequestReasons { get; set; }

        /// <summary>
        /// Gets or sets a list of return request actions
        /// </summary>
        public List<string> ReturnRequestActions { get; set; }

        /// <summary>
        /// Gets or sets a number of days that the Return Request Link will be available for customers after order placing.
        /// </summary>
        public int NumberOfDaysReturnRequestAvailable { get; set; }

        /// <summary>
        ///  Gift cards are activated when the order status is
        /// </summary>
        public int GiftCards_Activated_OrderStatusId { get; set; }

        /// <summary>
        ///  Gift cards are deactivated when the order status is
        /// </summary>
        public int GiftCards_Deactivated_OrderStatusId { get; set; }

    }
}