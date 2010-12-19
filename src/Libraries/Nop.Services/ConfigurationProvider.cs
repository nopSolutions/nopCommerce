using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Configuration;
using System.ComponentModel;

namespace Nop.Services
{
    public class ConfigurationProvider<TSettings> : IConfiguration<TSettings> where TSettings : ISettings, new()
    {
        readonly ISettingService settingService;

        public ConfigurationProvider(ISettingService settingService) {
            this.settingService = settingService;
            this.BuildConfiguration();
        }
        
        public TSettings Settings { get; private set; }

        private void BuildConfiguration() {
            Settings = new TSettings();

            // get properties we can write to
            var properties = from prop in typeof(TSettings).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             let setting = settingService.GetSettingByKey<string>(typeof(TSettings).Name + "." + prop.Name)
                             where setting != null
                             where TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string))
                             let value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromString(setting)
                             select new { prop, value };

            // assign properties
            properties.ToList().ForEach(p => p.prop.SetValue(Settings, p.value, null));
        }
    }
}
