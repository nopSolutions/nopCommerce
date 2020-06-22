using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Represents a request to get tax total
    /// </summary>
    public partial class TaxTotalRequest
    {
        #region Ctor

        public TaxTotalRequest()
        {
            ShoppingCart = new List<ShoppingCartItem>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a shopping cart
        /// </summary>
        public IList<ShoppingCartItem> ShoppingCart { get; set; }

        /// <summary>
        /// Gets or sets a customer
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use payment method additional fee
        /// </summary>
        public bool UsePaymentMethodAdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets a store identifier
        /// </summary>
        public int StoreId { get; set; }

        #endregion
    }
}