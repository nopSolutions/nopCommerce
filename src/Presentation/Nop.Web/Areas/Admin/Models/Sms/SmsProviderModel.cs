using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Sms;

/// <summary>
/// Represents a sms provider model
/// </summary>
public partial record SmsProviderModel : BaseNopModel, IPluginModel
{
    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Sms.Providers.Fields.FriendlyName")]
    public string FriendlyName { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Sms.Providers.Fields.SystemName")]
    public string SystemName { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Sms.Providers.Fields.IsPrimaryProvider")]
    public bool IsPrimaryProvider { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Sms.Providers.Configure")]
    public string ConfigurationUrl { get; set; }

    public string LogoUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    #endregion
}