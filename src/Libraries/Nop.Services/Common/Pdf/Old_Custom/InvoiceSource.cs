using System.ComponentModel;

namespace Nop.Services.Common.Pdf;

/// <summary>
/// Represents the data source for the invoice
/// </summary>
public partial class InvoiceSource : DocumentSource
{
    #region Ctor

    public InvoiceSource()
    {
        Products = new();
        Totals = new();
        FooterTextColumn1 = new();
        FooterTextColumn2 = new();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the logo binary
    /// </summary>
    public byte[] LogoData { get; set; }

    /// <summary>
    /// Gets or sets the date and time of order creation
    /// </summary>
    [DisplayName("Pdf.OrderDate")]
    public DateTime OrderDateUser { get; set; }

    /// <summary>
    /// Gets or sets the order number
    /// </summary>
    [DisplayName("Pdf.Order")]
    public string OrderNumberText { get; set; }

    /// <summary>
    /// Gets or sets store location
    /// </summary>
    public string StoreUrl { get; set; }

    /// <summary>
    /// Gets or sets a collection of order items
    /// </summary>
    public List<ProductItem> Products { get; set; }

    /// <summary>
    /// Gets or sets the billing address
    /// </summary>
    [DisplayName("Pdf.BillingInformation")]
    public AddressItem BillingAddress { get; set; }

    /// <summary>
    /// Gets or sets the shipping address
    /// </summary>
    [DisplayName("Pdf.ShippingInformation")]
    public AddressItem ShippingAddress { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display product SKU in the invoice document
    /// </summary>
    public bool ShowSkuInProductList { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display vendor name in the invoice document
    /// </summary>
    public bool ShowVendorInProductList { get; set; }

    /// <summary>
    /// Gets or sets the checkout attribute description
    /// </summary>
    public string CheckoutAttributes { get; set; }

    /// <summary>
    /// Gets or sets order totals
    /// </summary>
    public InvoiceTotals Totals { get; set; }

    /// <summary>
    /// Gets or sets order notes
    /// </summary>
    [DisplayName("Pdf.OrderNotes")]
    public List<(string, string)> OrderNotes { get; set; }

    /// <summary>
    /// Gets or sets the text that will appear at the bottom of invoice (column 1)
    /// </summary>
    public List<string> FooterTextColumn1 { get; set; }

    /// <summary>
    /// Gets or sets the text that will appear at the bottom of invoice (column 2)
    /// </summary>
    public List<string> FooterTextColumn2 { get; set; }

    #endregion
}