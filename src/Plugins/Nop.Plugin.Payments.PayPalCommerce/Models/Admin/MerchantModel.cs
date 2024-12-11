using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Models.Admin;

/// <summary>
/// Represents the merchant model
/// </summary>
public record MerchantModel : BaseNopModel
{
    #region Properties

    public string MerchantId { get; set; }

    public bool DisplayStatus { get; set; }

    public bool AccountCreated { get; set; }

    public bool EmailConfirmed { get; set; }

    public bool PaymentsReceivable { get; set; }

    public bool AdvancedCardsEnabled { get; set; }

    public bool ApplePayEnabled { get; set; }

    public bool GooglePayEnabled { get; set; }

    public bool VaultingEnabled { get; set; }

    public bool ConfiguratorSupported { get; set; }

    public (List<string> Success, List<string> Warning, List<string> Error) Messages { get; set; } = new();

    #endregion
}