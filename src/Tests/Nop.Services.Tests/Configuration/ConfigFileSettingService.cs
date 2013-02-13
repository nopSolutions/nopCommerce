using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Domain.Configuration;
using Nop.Services.Configuration;
using Nop.Services.Events;

namespace Nop.Services.Tests.Configuration
{
    public class ConfigFileSettingService : SettingService
    {
        public ConfigFileSettingService(ICacheManager cacheManager, 
            IEventPublisher eventPublisher,
            IRepository<Setting> settingRepository):
            base (cacheManager, eventPublisher, settingRepository)
        {
            
        }
        public override Setting GetSettingById(int settingId)
        {
            throw new InvalidOperationException("Get setting by id is not supported");
        }
        
        public override T GetSettingByKey<T>(string key, T defaultValue = default(T), int storeId = 0)
        {
            key = key.Trim().ToLowerInvariant();
            var settings = GetAllSettings();
            var setting = settings.FirstOrDefault(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase) &&
                x.StoreId == storeId);
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

        public override void ClearCache()
        {
        }
    }
}
