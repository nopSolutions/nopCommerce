using Newtonsoft.Json;

namespace Nop.Plugin.Payments.AmazonPay.Domain.Onboarding;

/// <summary>
/// Represents account credentials details
/// </summary>
public class Credentials
{
    #region Properties

    /// <summary>
    /// Gets or sets the merchant id used in API requests and widget render requests
    /// </summary>
    [JsonProperty(PropertyName = "merchantId")]
    public string MerchantId { get; set; }

    /// <summary>
    /// Gets or sets the store identifier
    /// </summary>
    [JsonProperty(PropertyName = "storeId")]
    public string StoreId { get; set; }

    /// <summary>
    /// Gets or sets the public key identifier used to sign API requests
    /// </summary>
    [JsonProperty(PropertyName = "publicKeyId")]
    public string PublicKeyId { get; set; }

    #endregion
}

/// <summary>
/// Represents account credentials details (plain text format)
/// </summary>
public class PlainCredentials
{
    #region Properties

    /// <summary>
    /// Gets or sets the merchant id used in API requests and widget render requests
    /// </summary>
    [JsonProperty(PropertyName = "merchant_id")]
    public string MerchantId { get; set; }

    /// <summary>
    /// Gets or sets the store identifier
    /// </summary>
    [JsonProperty(PropertyName = "store_id")]
    public string StoreId { get; set; }

    /// <summary>
    /// Gets or sets the public key identifier used to sign API requests
    /// </summary>
    [JsonProperty(PropertyName = "public_key_id")]
    public string PublicKeyId { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Convert to credentials
    /// </summary>
    /// <param name="plainCredentials">Credentials in plain text format</param>
    /// <returns>Credentials</returns>
    public static Credentials ToCredentials(PlainCredentials plainCredentials)
    {
        return new() { MerchantId = plainCredentials.MerchantId, StoreId = plainCredentials.StoreId, PublicKeyId = plainCredentials.PublicKeyId };
    }

    #endregion
}