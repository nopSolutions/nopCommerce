using Nop.Core.Configuration;
using Nop.Plugin.Payments.AmazonPay.Enums;

namespace Nop.Plugin.Payments.AmazonPay;

/// <summary>
/// Represents plugin settings
/// </summary>
public class AmazonPaySettings : ISettings
{
    #region Credentials

    /// <summary>
    /// Gets or sets a value indicating whether to manually set the credentials.
    /// For example, there is already an account created, or if the merchant wants to use the sandbox mode
    /// </summary>
    public bool SetCredentialsManually { get; set; }

    /// <summary>
    /// Gets or sets the payment region
    /// </summary>
    public PaymentRegion PaymentRegion { get; set; }

    /// <summary>
    /// Gets or sets the credential payload
    /// </summary>
    public string Payload { get; set; }

    /// <summary>
    /// Gets or sets the private key
    /// </summary>
    public string PrivateKey { get; set; }

    /// <summary>
    /// Gets or sets the public key
    /// </summary>
    public string PublicKey { get; set; }

    /// <summary>
    /// Gets or sets the public key ID
    /// </summary>
    public string PublicKeyId { get; set; }

    /// <summary>
    /// Gets or sets the merchant ID
    /// </summary>
    public string MerchantId { get; set; }

    /// <summary>
    /// Gets or sets the store ID
    /// </summary>
    public string StoreId { get; set; }

    #endregion

    #region Configuration

    /// <summary>
    /// Gets or sets a value indicating whether to use sandbox
    /// </summary>
    public bool UseSandbox { get; set; }

    /// <summary>
    /// Gets or sets the payment type
    /// </summary>
    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Gets or sets the button placements
    /// </summary>
    public List<ButtonPlacement> ButtonPlacement { get; set; } = new();

    /// <summary>
    /// Gets or sets the button color
    /// </summary>
    public ButtonColor ButtonColor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to log API requests and responses
    /// </summary>
    public bool EnableLogging { get; set; }

    /// <summary>
    /// Gets or sets the source code of logo in footer
    /// </summary>
    public string LogoInFooter { get; set; }

    #endregion
}