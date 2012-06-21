using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Configuration;

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
                             where CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string))
                             where CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).IsValid(setting)
                             let value = CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).ConvertFromInvariantString(setting)
                             select new { prop, value };

            // assign properties
            properties.ToList().ForEach(p => p.prop.SetValue(Settings, p.value, null));
        }

        public void SaveSettings(TSettings settings)
        {
            var properties = from prop in typeof(TSettings).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             where CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string))
                             select prop;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in properties)
            {
                string key = typeof(TSettings).Name + "." + prop.Name;
                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = prop.GetValue(settings, null);
                if (value != null)
                    _settingService.SetSetting(key, value, false);
                else
                    _settingService.SetSetting(key, "", false);
            }

            //and now clear cache
            _settingService.ClearCache();

            this.Settings = settings;
        }

        public void DeleteSettings()
        {
            var properties = from prop in typeof(TSettings).GetProperties()
                             select prop;

            var settingList = new List<Setting>();
            foreach (var prop in properties)
            {
                string key = typeof(TSettings).Name + "." + prop.Name;
                var setting = _settingService.GetSettingByKey(key);
                if (setting != null)
                    settingList.Add(setting);
            }

            foreach (var setting in settingList)
                _settingService.DeleteSetting(setting);

        }
    }
}
