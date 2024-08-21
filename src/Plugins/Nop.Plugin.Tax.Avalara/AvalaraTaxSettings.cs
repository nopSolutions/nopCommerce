using Nop.Core.Configuration;
using Nop.Plugin.Tax.Avalara.Domain;

namespace Nop.Plugin.Tax.Avalara;

/// <summary>
/// Represents plugin settings
/// </summary>
public class AvalaraTaxSettings : ISettings
{
    #region Common

    /// <summary>
    /// Gets or sets Avalara account ID
    /// </summary>
    public string AccountId { get; set; }

    /// <summary>
    /// Gets or sets Avalara account license key
    /// </summary>
    public string LicenseKey { get; set; }

    /// <summary>
    /// Gets or sets company code
    /// </summary>
    public string CompanyCode { get; set; }

    /// <summary>
    /// Gets or sets company id
    /// </summary>
    public int? CompanyId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use sandbox (testing environment)
    /// </summary>
    public bool UseSandbox { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable logging of all requests
    /// </summary>
    public bool EnableLogging { get; set; }

    #endregion

    #region Tax calculation

    /// <summary>
    /// Gets or sets a value indicating whether to commit tax transactions right after they are saved
    /// </summary>
    public bool CommitTransactions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to validate entered by customer addresses before the tax calculation
    /// </summary>
    public bool ValidateAddress { get; set; }

    /// <summary>
    /// Gets or sets a type of the tax origin address
    /// </summary>
    public TaxOriginAddressType TaxOriginAddressType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use tax rate tables based on the zip rate lookup to estimate 
    /// </summary>
    public bool UseTaxRateTables { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to get the tax rate by address only 
    /// </summary>
    public bool GetTaxRateByAddressOnly { get; set; }

    /// <summary>
    /// Gets or sets the tax rate (by address) cache time in minutes
    /// </summary>
    public int TaxRateByAddressCacheTime { get; set; }

    #endregion

    #region Certificates

    /// <summary>
    /// Gets or sets a value indicating whether the feature of exemption certificates is enabled
    /// </summary>
    public bool EnableCertificates { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether new certificates are automatically valid
    /// </summary>
    public bool AutoValidateCertificate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to allow customers to edit their info 
    /// </summary>
    public bool AllowEditCustomer { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display a message that there are no valid certificates for the customer on the order confirmation page
    /// </summary>
    public bool DisplayNoValidCertificatesMessage { get; set; }

    /// <summary>
    /// Gets or sets the identifiers of customer roles for which certificates are available
    /// </summary>
    public List<int> CustomerRoleIds { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether to disaply a preview of customer's new certificate before signing and submitting
    /// </summary>
    public bool PreviewCertificate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the customer must upload a scanned copy of their exemption certificate
    /// </summary>
    public bool UploadOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to disable the upload of prefilled documents, so that customers must generate a new copy of the certificate by entering their information
    /// </summary>
    public bool FillOnly { get; set; }

    #endregion

    #region Item Classification

    /// <summary>
    /// Gets or sets a value indicating whether to use item classification
    /// </summary>
    public bool UseItemClassification { get; set; }

    /// <summary>
    /// Gets or sets the identifiers of countries for which the HS code needs to be calculated
    /// </summary>
    public List<int> SelectedCountryIds { get; set; } = new();

    #endregion
}