using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        
        public T GetSettingByKey<T>(string key, T defaultValue = default(T), int storeId = 0)
        {
            key = key.Trim().ToLowerInvariant();
            var settings = GetAllSettings();
            var setting = settings.FirstOrDefault(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase) &&
                x.StoreId == storeId);
            if (setting != null)
                return CommonHelper.To<T>(setting.Value);

            return defaultValue;
        }

        public void SetSetting<T>(string key, T value, int storeId = 0, bool clearCache = true)
        {
            throw new NotImplementedException();
        }

        public IList<Setting> GetAllSettings()
        {
            var settings = new List<Setting>();
            var appSettings = ConfigurationManager.AppSettings;
            foreach (var setting in appSettings.AllKeys)
            {
                settings.Add(new Setting()
                                 {
                                     Name = setting.ToLowerInvariant(),
                                     Value = appSettings[setting]
                                 });
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
