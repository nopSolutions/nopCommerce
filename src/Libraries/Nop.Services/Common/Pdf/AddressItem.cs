﻿using System.ComponentModel;

namespace Nop.Services.Common.Pdf;

/// <summary>
/// Represents the address
/// </summary>
public partial record AddressItem
{
    #region Properties

    /// <summary>
    /// Gets or sets the company
    /// </summary>
    [DisplayName("Pdf.Address.Company")]
    public string Company { get; set; }

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    [DisplayName("Pdf.Address.Name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    [DisplayName("Pdf.Address.Phone")]
    public string Phone { get; set; }

    /// <summary>
    /// Gets or sets the fax number
    /// </summary>
    [DisplayName("Pdf.Address.Fax")]
    public string Fax { get; set; }

    /// <summary>
    /// Gets or sets the address
    /// </summary>
    [DisplayName("Pdf.Address")]
    public string Address { get; set; }

    /// <summary>
    /// Gets or sets the address 2
    /// </summary>
    [DisplayName("Pdf.Address2")]
    public string Address2 { get; set; }

    /// <summary>
    /// Gets or sets address in line
    /// </summary>
    [DisplayName("Pdf.AddressLine")]
    public string AddressLine { get; set; }

    /// <summary>
    /// Gets or sets the county
    /// </summary>
    public string County { get; set; }

    /// <summary>
    /// Gets or sets the country
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets the city
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the State Province Name
    /// </summary>
    public string StateProvinceName { get; set; }

    /// <summary>
    /// Gets or sets the Zip postal code
    /// </summary>
    public string ZipPostalCode { get; set; }

    /// <summary>
    /// Gets or sets the vat number
    /// </summary>
    [DisplayName("Pdf.Address.VATNumber")]
    public string VATNumber { get; set; }

    /// <summary>
    /// Gets or sets the payment method name
    /// </summary>
    [DisplayName("Pdf.Address.PaymentMethod")]
    public string PaymentMethod { get; set; }

    /// <summary>
    /// Gets or sets the shipping method name
    /// </summary>
    [DisplayName("Pdf.Address.ShippingMethod")]
    public string ShippingMethod { get; set; }

    /// <summary>
    /// Gets or sets the custom attributes (see "AddressAttribute" entity for more info)
    /// </summary>
    public List<string> AddressAttributes { get; set; } = new();

    /// <summary>
    /// Gets or sets the deserialized CustomValues (values from ProcessPaymentRequest)
    /// </summary>
    public Dictionary<string, object> CustomValues { get; set; } = new();

    #endregion
}