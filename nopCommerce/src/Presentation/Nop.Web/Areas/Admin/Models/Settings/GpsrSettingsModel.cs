using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

public partial record GpsrSettingsModel : BaseNopModel, ISettingsModel
{
    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gpsr.Enabled")]
    public bool Enabled { get; set; }
    public bool Enabled_OverrideForStore { get; set; }
}