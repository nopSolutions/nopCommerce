using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;

namespace Nop.Plugin.Widgets.AbcBonusBundle.Models
{
    public class ConfigurationModel
    {
        [NopResourceDisplayName("Store Name")]
        public string StoreName { get; set; }

        [NopResourceDisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        public static ConfigurationModel FromSettings(AbcBonusBundleSettings settings)
        {
            return new ConfigurationModel
            {
                StoreName = settings.StoreName,
                PhoneNumber = settings.PhoneNumber
            };
        }

        public AbcBonusBundleSettings ToSettings()
        {
            return new AbcBonusBundleSettings
            {
                StoreName = StoreName,
                PhoneNumber = PhoneNumber
            };
        }
    }
}
