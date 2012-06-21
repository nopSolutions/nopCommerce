using System;
using System.Collections.Generic;
using System.Configuration;
using Nop.Core;
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

        public Setting GetSettingByKey(string key)
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
                return CommonHelper.To<T>(settings[key].Value);

            return defaultValue;
        }

        public void SetSetting<T>(string key, T value, bool clearCache = true)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, KeyValuePair<int, string>> GetAllSettings()
        {
            var settings = new Dictionary<string, KeyValuePair<int, string>>();
            var appSettings = ConfigurationManager.AppSettings;
            foreach (var setting in appSettings.AllKeys) {
                settings.Add(setting.ToLowerInvariant(), new KeyValuePair<int,string>(0, appSettings[setting]));
            }

            return settings;
        }

        public void SaveSetting<T>(T settingInstance) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete all settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public void DeleteSetting<T>() where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void ClearCache()
        {
            throw new NotImplementedException();
        }
    }
}
