using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

public partial record OtpSettingsModel : BaseNopModel, ISettingsModel
{
    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.LoginByPhoneEnabled")]
    public bool LoginByPhoneEnabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.OtpTimeLife")]
    public int OtpTimeLife { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.OtpCountAttemptsToSendCode")]
    public int OtpCountAttemptsToSendCode { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.OtpTimeToRepeat")]
    public int OtpTimeToRepeat { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.OtpLength")]
    public int OtpLength { get; set; }

    #endregion
}
