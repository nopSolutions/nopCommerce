using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    public class AllSettingsListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Configuration.Settings.AllSettings.SearchSettingName")]
        public string SearchSettingName { get; set; }
        [NopResourceDisplayName("Admin.Configuration.Settings.AllSettings.SearchSettingValue")]
        public string SearchSettingValue { get; set; }
    }
}