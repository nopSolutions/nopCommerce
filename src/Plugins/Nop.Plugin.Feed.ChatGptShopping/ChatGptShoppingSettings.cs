using Nop.Core.Configuration;

namespace Nop.Plugin.Feed.ChatGptShopping;

/// <summary>
/// Represents plugin settings
/// </summary>
public class ChatGptShoppingSettings : ISettings
{
    /// <summary>
    /// Gets or sets the currency identifier for which feed file(s) will be generated
    /// </summary>
    public int CurrencyId { get; set; }

    /// <summary>
    /// Gets or sets the language identifier for which feed file(s) will be generated
    /// </summary>
    public int LanguageId { get; set; }

    /// <summary>
    /// Gets or sets the comma separated list of target countries for which feed file(s) will be generated
    /// </summary>
    public string TargetCountries { get; set; }

    /// <summary>
    /// Gets or sets the store country for which feed file(s) will be generated
    /// </summary>
    public string StoreCountry { get; set; }

    /// <summary>
    /// Gets or sets the product picture size
    /// </summary>
    public int ProductPictureSize { get; set; }

    #region Advanced settings

    /// <summary>
    /// Gets or sets the id of the customer used for feed generation
    /// </summary>
    public int CustomerId { get; set; }

    #endregion
}
