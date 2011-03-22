
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
        /// Gets or sets a value indicating whether "Return requests" are allowed
        /// </summary>
        public bool ReturnRequestsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a list of return request reasons
        /// </summary>
        public string ReturnRequestReasons { get; set; }

        /// <summary>
        /// Gets or sets a list of return request actions
        /// </summary>
        public string ReturnRequestActions { get; set; }
    }
}