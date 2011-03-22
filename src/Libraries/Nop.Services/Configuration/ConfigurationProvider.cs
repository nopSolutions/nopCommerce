using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Nop.Core;
using Nop.Core.Configuration;

namespace Nop.Services.Configuration
{
    public class ConfigurationProvider<TSettings> : IConfigurationProvider<TSettings> where TSettings : ISettings, new()
    {
        readonly ISettingService _settingService;

        public ConfigurationProvider(ISettingService settingService) 
        {
            this._settingService = settingService;
            this.BuildConfiguration();
        }

        public TSettings Settings { get; protected set; }

        private void BuildConfiguration() 
        {
            Settings = Activator.CreateInstance<TSettings>();

            // get properties we can write to
            var properties = from prop in typeof(TSettings).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             let setting = _settingService.GetSettingByKey<string>(typeof(TSettings).Name + "." + prop.Name)
                             where setting != null
                             where TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string))
                             let value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting)
                             select new { prop, value };

            // assign properties
            properties.ToList().ForEach(p => p.prop.SetValue(Settings, p.value, null));
        }

        public void SaveSettings(TSettings settings)
        {
            var properties = from prop in typeof(TSettings).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             where TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string))
                             select prop;
            foreach (var prop in properties)
            {
                string key = typeof(TSettings).Name + "." + prop.Name;
                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = prop.GetValue(settings, null);
                if (value != null)
                    _settingService.SetSetting(key, value);
                else
                    _settingService.SetSetting(key, "");
            }


            this.Settings = settings;
        }
    }
}
