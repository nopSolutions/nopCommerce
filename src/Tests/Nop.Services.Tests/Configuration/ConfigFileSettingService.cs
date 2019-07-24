using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Configuration;
using Nop.Services.Configuration;
using Nop.Services.Events;

namespace Nop.Services.Tests.Configuration
{
    public class ConfigFileSettingService : SettingService
    {
        public ConfigFileSettingService(IEventPublisher eventPublisher, IRepository<Setting> settingRepository, IStaticCacheManager cacheManager) : base(eventPublisher, settingRepository, cacheManager)
        {
        }

        public override Setting GetSettingById(int settingId)
        {
            throw new InvalidOperationException("Get setting by id is not supported");
        }

        public override T GetSettingByKey<T>(string key, T defaultValue = default(T),
            int storeId = 0, bool loadSharedValueIfNotFound = false)
        {

            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettings();
            key = key.Trim().ToLowerInvariant();

            var setting = settings.FirstOrDefault(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase) &&
                x.StoreId == storeId);

            //load shared value?
            if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
            {
                setting = settings.FirstOrDefault(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase) &&
                    x.StoreId == 0);
            }

            if (setting != null)
                return CommonHelper.To<T>(setting.Value);

            return defaultValue;
        }

        public override void DeleteSetting(Setting setting)
        {
            throw new InvalidOperationException("Deleting settings is not supported");
        }
        
        public override void SetSetting<T>(string key, T value, int storeId = 0, bool clearCache = true)
        {
            throw new NotImplementedException();
        }

        public override IList<Setting> GetAllSettings()
        {
            var settings = new List<Setting>();
            var appSettings = new Dictionary<string, string>
            {
                { "Setting1", "SomeValue"},
                { "Setting2", "25"},
                { "Setting3", "12/25/2010"},
                { "TestSettings.ServerName", "Ruby"},
                { "TestSettings.Ip", "192.168.0.1"},
                { "TestSettings.PortNumber", "21"},
                { "TestSettings.Username", "admin"},
                { "TestSettings.Password", "password"}
            };
            foreach (var setting in appSettings)
            {
                settings.Add(new Setting
                {
                    Name = setting.Key.ToLowerInvariant(),
                    Value = setting.Value
                });
            }

            return settings;
        }

        public override void ClearCache()
        {
        }
    }
}
