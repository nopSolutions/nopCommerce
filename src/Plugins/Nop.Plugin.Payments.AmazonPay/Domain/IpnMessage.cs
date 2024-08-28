namespace Nop.Plugin.Payments.AmazonPay.Domain;

/// <summary>
/// Represents IPN message 
/// </summary>
public class IpnMessage
{
    /// <summary>
    /// Gets or sets a message id
    /// </summary>
    public string MessageId { get; set; }

    /// <summary>
    /// Gets or sets a merchant identifier
    /// </summary>
    public string MerchantID { get; set; }

    /// <summary>
    /// Gets or sets a message object type
    /// </summary>
    public string ObjectType { get; set; }

    /// <summary>
    /// Gets or sets a message object identifier
    /// </summary>
    public string ObjectId { get; set; }

    /// <summary>
    /// Gets or sets a charge permission identifier
    /// </summary>
    public string ChargePermissionId { get; set; }

    /// <summary>
    /// Gets or sets a notification type
    /// </summary>
    public string NotificationType { get; set; }

    /// <summary>
    /// Gets or sets a notification identifier
    /// </summary>
    public string NotificationId { get; set; }

    /// <summary>
    /// Gets or sets a notification version
    /// </summary>
    public string NotificationVersion { get; set; }
}