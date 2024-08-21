using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Models;

public record ConfigurationModel : BaseNopModel
{
    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.UseSandbox")]
    public bool UseSandbox { get; set; }
    public bool UseSandbox_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.GoogleId")]
    public string GoogleId { get; set; }
    public bool GoogleId_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.ApiSecret")]
    [NoTrim]
    [DataType(DataType.Password)]
    public string ApiSecret { get; set; }
    public bool ApiSecret_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.EnableEcommerce")]
    public bool EnableEcommerce { get; set; }
    public bool EnableEcommerce_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.TrackingScript")]
    public string TrackingScript { get; set; }
    public bool TrackingScript_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.IncludingTax")]
    public bool IncludingTax { get; set; }
    public bool IncludingTax_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.IncludeCustomerId")]
    public bool IncludeCustomerId { get; set; }
    public bool IncludeCustomerId_OverrideForStore { get; set; }
}