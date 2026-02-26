using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Sms.Twilio.Models;

/// <summary>
/// Represents plugin configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    #region Properties

    [NopResourceDisplayName("Plugins.Sms.Twilio.Credentials.Fields.AuthToken")]
    public string AuthToken { get; set; }

    [NopResourceDisplayName("Plugins.Sms.Twilio.Credentials.Fields.AccountSID")]
    public string AccountSID { get; set; }

    [NopResourceDisplayName("Plugins.Sms.Twilio.Credentials.Fields.PhoneNumber")]
    public string PhoneNumber { get; set; }

    [NopResourceDisplayName("Plugins.Sms.Twilio.Credentials.Fields.BalanceInfo")]
    public string BalanceInfo { get; set; }

    public bool IsConfigured { get; set; }

    #endregion
}