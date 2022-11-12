<<<<<<< HEAD
using Nop.Core.Domain.Orders;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Represents a PostProcessPaymentRequest
    /// </summary>
    public partial class PostProcessPaymentRequest
    {
        /// <summary>
        /// Gets or sets an order. Used when order is already saved (payment gateways that redirect a customer to a third-party URL)
        /// </summary>
        public Order Order { get; set; }
    }
}
=======
using Nop.Core.Domain.Orders;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Represents a PostProcessPaymentRequest
    /// </summary>
    public partial class PostProcessPaymentRequest
    {
        /// <summary>
        /// Gets or sets an order. Used when order is already saved (payment gateways that redirect a customer to a third-party URL)
        /// </summary>
        public Order Order { get; set; }
    }
}
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
