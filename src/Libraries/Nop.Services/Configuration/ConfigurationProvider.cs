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
            this.BuildConfiguration(0);
        }

        public TSettings Settings { get; protected set; }

        public void BuildConfiguration(int storeId) 
        {
            Settings = Activator.CreateInstance<TSettings>();

            foreach (var prop in typeof (TSettings).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof (TSettings).Name + "." + prop.Name;
                //load by store
                string setting = _settingService.GetSettingByKey<string>(key, storeId: storeId);
                if (setting == null && storeId > 0)
                {
                    //load for all stores if not found
                    setting = _settingService.GetSettingByKey<string>(key, storeId: 0);
                }

                if (setting == null)
                    continue;

                if (!CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).IsValid(setting))
                    continue;

                object value = CommonHelper.GetNopCustomTypeConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(Settings, value, null);

            }
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
                var storeId = 0;
                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = prop.GetValue(settings, null);
                if (value != null)
                    _settingService.SetSetting(key, value, storeId, false);
                else
                    _settingService.SetSetting(key, "", storeId, false);
            }

            //and now clear cache
            _settingService.ClearCache();

            this.Settings = settings;
        }

        public void DeleteSettings()
        {
            var properties = from prop in typeof(TSettings).GetProperties()
                             select prop;

            var settingsToDelete = new List<Setting>();
            var allSettings = _settingService.GetAllSettings();
            foreach (var prop in properties)
            {
                string key = typeof(TSettings).Name + "." + prop.Name;
                settingsToDelete.AddRange(allSettings.Where(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
            }

            foreach (var setting in settingsToDelete)
                _settingService.DeleteSetting(setting);
        }
    }
}
