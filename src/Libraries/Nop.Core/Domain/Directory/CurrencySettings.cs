using Nop.Core.Configuration;

namespace Nop.Core.Domain.Directory;

/// <summary>
/// Currency settings
/// </summary>
public partial class CurrencySettings : ISettings
{
    /// <summary>
    /// A value indicating whether to display currency labels
    /// </summary>
    public bool DisplayCurrencyLabel { get; set; }

    /// <summary>
    /// Primary store currency identifier
    /// </summary>
    public int PrimaryStoreCurrencyId { get; set; }

    /// <summary>
    ///  Primary exchange rate currency identifier
    /// </summary>
    public int PrimaryExchangeRateCurrencyId { get; set; }

    /// <summary>
    /// Active exchange rate provider system name (of a plugin)
    /// </summary>
    public string ActiveExchangeRateProviderSystemName { get; set; }

    /// <summary>
    /// A value indicating whether to enable automatic currency rate updates
    /// </summary>
    public bool AutoUpdateEnabled { get; set; }
}