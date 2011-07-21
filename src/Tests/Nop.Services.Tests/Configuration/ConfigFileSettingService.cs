using System;
using System.Collections.Generic;
using System.Configuration;
using Nop.Core.Configuration;
using Nop.Core.Domain.Configuration;
using Nop.Services.Configuration;

namespace Nop.Services.Tests.Configuration
{
    public class ConfigFileSettingService : ISettingService
    {
        public Setting GetSettingById(int settingId)
        {
            throw new InvalidOperationException("Get setting by id is not supported");
        }

        public void DeleteSetting(Setting setting)
        {
            throw new InvalidOperationException("Deleting settings is not supported");
        }
        
        public T GetSettingByKey<T>(string key, T defaultValue = default(T))
        {
            key = key.Trim().ToLowerInvariant();
            var settings = GetAllSettings();
            if (settings.ContainsKey(key))
                return settings[key].As<T>();

            return defaultValue;
        }

        public void SetSetting<T>(string key, T value, bool clearCache = true)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, Setting> GetAllSettings()
        {
            var settings = new Dictionary<string, Setting>();
            var appSettings = ConfigurationManager.AppSettings;
            foreach (var setting in appSettings.AllKeys) {
                settings.Add(setting.ToLowerInvariant(), new Setting(setting, appSettings[setting]));
            }

            return settings;
        }

        public void SaveSetting<T>(T settingInstance) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void ClearCache()
        {
            throw new NotImplementedException();
        }
    }
}
