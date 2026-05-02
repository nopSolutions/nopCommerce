using Nop.Core.Configuration;
using Nop.Plugin.Shipping.UPS.Domain;

namespace Nop.Plugin.Shipping.UPS;

/// <summary>
/// Represents settings of the UPS shipping plugin
/// </summary>
public class UPSSettings : ISettings
{
    /// <summary>
    /// Gets or sets the account number
    /// </summary>
    public string AccountNumber { get; set; }

    /// <summary>
    /// Gets or sets the client ID
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret
    /// </summary>
    public string ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use sandbox environment
    /// </summary>
    public bool UseSandbox { get; set; }

    /// <summary>
    /// Gets or sets an amount of the additional handling charge
    /// </summary>
    public decimal AdditionalHandlingCharge { get; set; }

    /// <summary>
    /// Gets or sets UPS customer classification
    /// </summary>
    public CustomerClassification CustomerClassification { get; set; }

    /// <summary>
    /// Gets or sets a pickup type
    /// </summary>
    public PickupType PickupType { get; set; }

    /// <summary>
    /// Gets or sets packaging type
    /// </summary>
    public PackagingType PackagingType { get; set; }

    /// <summary>
    /// Gets or sets offered carrier services
    /// </summary>
    public string CarrierServicesOffered { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether Saturday Delivery enabled
    /// </summary>
    public bool SaturdayDeliveryEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to insure packages
    /// </summary>
    public bool InsurePackage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to pass package dimensions
    /// </summary>
    public bool PassDimensions { get; set; }

    /// <summary>
    /// Gets or sets the packing package volume
    /// </summary>
    public int PackingPackageVolume { get; set; }

    /// <summary>
    /// Gets or sets packing type
    /// </summary>
    public PackingType PackingType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to record plugin tracing in log
    /// </summary>
    public bool Tracing { get; set; }

    /// <summary>
    /// Gets or sets package weight type (LBS or KGS)
    /// </summary>
    public string WeightType { get; set; }

    /// <summary>
    /// Gets or sets package dimensions type (IN or CM)
    /// </summary>
    public string DimensionsType { get; set; }

    /// <summary>
    /// Gets or sets a period (in seconds) before the request times out
    /// </summary>
    public int? RequestTimeout { get; set; }
}