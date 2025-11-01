﻿using System.ComponentModel;

namespace Nop.Plugin.Misc.RFQ.Services.Pdf;

/// <summary>
/// Represents product entry
/// </summary>
public class PdfQuoteItem
{
    #region Properties

    /// <summary>
    /// Gets or sets the product name
    /// </summary>
    [DisplayName("Plugins.Misc.RFQ.Product(s)")]
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the product price
    /// </summary>
    [DisplayName("Plugins.Misc.RFQ.OfferedUnitPrice")]
    public string Price { get; set; }

    /// <summary>
    /// Gets or sets the product quantity
    /// </summary>
    [DisplayName("Plugins.Misc.RFQ.OfferedQty")]
    public string Quantity { get; set; }
    
    /// <summary>
    /// Gets or sets the product attribute description
    /// </summary>
    public List<string> ProductAttributes { get; set; } = new();

    #endregion
}