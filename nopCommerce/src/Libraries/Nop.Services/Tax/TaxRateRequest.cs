using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Tax;

/// <summary>
/// Represents a request to get tax rate
/// </summary>
public partial class TaxRateRequest
{
    /// <summary>
    /// Gets or sets a customer
    /// </summary>
    public Customer Customer { get; set; }

    /// <summary>
    /// Gets or sets a product
    /// </summary>
    public Product Product { get; set; }

    /// <summary>
    /// Gets or sets an address
    /// </summary>
    public Address Address { get; set; }

    /// <summary>
    /// Gets or sets a tax category identifier
    /// </summary>
    public int TaxCategoryId { get; set; }

    /// <summary>
    /// Gets or sets a price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets a current store identifier
    /// </summary>
    public int CurrentStoreId { get; set; }
}