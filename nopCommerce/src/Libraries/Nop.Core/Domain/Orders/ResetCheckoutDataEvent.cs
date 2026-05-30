using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Orders;

/// <summary>
/// Reset checkout data event
/// </summary>
public partial class ResetCheckoutDataEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="storeId">Store identifier</param>
    public ResetCheckoutDataEvent(Customer customer, int storeId)
    {
        Customer = customer;
        StoreId = storeId;
    }

    /// <summary>
    /// Customer
    /// </summary>
    public Customer Customer { get; }

    /// <summary>
    /// Store identifier
    /// </summary>
    public int StoreId { get; }
}