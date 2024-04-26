using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a common configuration model
/// </summary>
public partial record CommonConfigModel : BaseNopModel, IConfigModel
{
    #region Properties

    [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.DisplayFullErrorStack")]
    public bool DisplayFullErrorStack { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.UserAgentStringsPath")]
    public string UserAgentStringsPath { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.CrawlerOnlyUserAgentStringsPath")]
    public string CrawlerOnlyUserAgentStringsPath { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.CrawlerOnlyAdditionalUserAgentStringsPath")]
    public string CrawlerOnlyAdditionalUserAgentStringsPath { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.UseSessionStateTempDataProvider")]
    public bool UseSessionStateTempDataProvider { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.ScheduleTaskRunTimeout")]
    [UIHint("Int32Nullable")]
    public int? ScheduleTaskRunTimeout { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.StaticFilesCacheControl")]
    public string StaticFilesCacheControl { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.PluginStaticFileExtensionsBlacklist")]
    public string PluginStaticFileExtensionsBlacklist { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.ServeUnknownFileTypes")]
    public bool ServeUnknownFileTypes { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.UseAutofac")]
    public bool UseAutofac { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.PermitLimit")]
    public int PermitLimit { get; set; } = 0;

    [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.QueueCount")]
    public int QueueCount { get; set; } = 0;

    [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.RejectionStatusCode")]
    public int RejectionStatusCode { get; set; } = 503;

    #endregion
}