using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Settings;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    [Validator(typeof(SettingValidator))]
    public partial class SettingModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.Value")]
        public string Value { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.StoreName")]
        public string Store { get; set; }
        public int StoreId { get; set; }
    }
}