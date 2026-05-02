using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents WebOptimizer config model
/// </summary>
public partial record WebOptimizerConfigModel : BaseNopModel, IConfigModel
{
    #region Properties

    [NopResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.EnableJavaScriptBundling")]
    public bool EnableJavaScriptBundling { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.EnableCssBundling")]
    public bool EnableCssBundling { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.EnableDiskCache")]
    public bool EnableDiskCache { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.CacheDirectory")]
    public string CacheDirectory { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.JavaScriptBundleSuffix")]
    public string JavaScriptBundleSuffix { get; set; }

    [NopResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.CssBundleSuffix")]
    public string CssBundleSuffix { get; set; }

    #endregion
}