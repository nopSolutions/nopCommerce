using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Domain;

/// <summary>
/// Represents the Pay Later message configuration
/// </summary>
public class MessageConfiguration
{
    #region Properties

    /// <summary>
    /// Gets or sets the placement where the message is to be shown
    /// </summary>
    [JsonProperty(PropertyName = "placement")]
    public string Placement { get; set; }

    /// <summary>
    /// Gets or sets the status, a value whether the placement is checked or not in the configurator
    /// </summary>
    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the message layout
    /// </summary>
    [JsonProperty(PropertyName = "layout")]
    public string Layout { get; set; }

    /// <summary>
    /// Gets or sets the logo type used in the message
    /// </summary>
    [JsonProperty(PropertyName = "logo-type")]
    public string LogoType { get; set; }

    /// <summary>
    /// Gets or sets the position of the PayPal or PayPal Credit logo in the message
    /// </summary>
    [JsonProperty(PropertyName = "logo-position")]
    public string LogoPosition { get; set; }

    /// <summary>
    /// Gets or sets the text and logo color of the message
    /// </summary>
    [JsonProperty(PropertyName = "text-color")]
    public string TextColor { get; set; }

    /// <summary>
    /// Gets or sets the size of the message text
    /// </summary>
    [JsonProperty(PropertyName = "text-size")]
    public string TextSize { get; set; }

    #endregion
}