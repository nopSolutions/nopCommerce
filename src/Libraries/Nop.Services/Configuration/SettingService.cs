using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Domain.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Events;

namespace Nop.Services.Configuration
{
    /// <summary>
    /// Setting manager
    /// </summary>
    public partial class SettingService : ISettingService
    {
        #region Constants
        private const string SETTINGS_ALL_KEY = "Nop.setting.all";
        #endregion

        #region Fields

        private readonly IRepository<Setting> _settingRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="settingRepository">Setting repository</param>
        public SettingService(ICacheManager cacheManager, IEventPublisher eventPublisher,
            IRepository<Setting> settingRepository)
        {
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
            this._settingRepository = settingRepository;
        }

        #endregion

        #region Nested classes

        [Serializable]
        public class SettingForCaching
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public int StoreId { get; set; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        protected virtual IDictionary<string, IList<SettingForCaching>> GetAllSettingsCached()
        {
            //cache
            string key = string.Format(SETTINGS_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var query = from s in _settingRepository.Table
                            orderby s.Name, s.StoreId
                            select s;
                var settings = query.ToList();
                var dictionary = new Dictionary<string, IList<SettingForCaching>>();
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new SettingForCaching()
                            {
                                Id = s.Id,
                                Name = s.Name,
                                Value = s.Value,
                                StoreId = s.StoreId
                            };
                    if (!dictionary.ContainsKey(resourceName))
                    {
                        //first setting
                        dictionary.Add(resourceName, new List<SettingForCaching>()
                        {
                            settingForCaching
                        });
                    }
                    else
                    {
                        //already added
                        //most probably it's the setting with the same name but for some certain store (storeId > 0)
                        dictionary[resourceName].Add(settingForCaching);
                    }
                }
                return dictionary;
            });
        }
        #endregion

        #region Methods

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void InsertSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Insert(setting);

            //cache
            if (clearCache)
                _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);

            //event notification
            _eventPublisher.EntityInserted(setting);
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void UpdateSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Update(setting);

            //cache
            if (clearCache)
                _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);

            //event notification
            _eventPublisher.EntityUpdated(setting);
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Delete(setting);

            //cache
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);

            //event notification
            _eventPublisher.EntityDeleted(setting);
        }

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>Setting</returns>
        public virtual Setting GetSettingById(int settingId)
        {
            if (settingId == 0)
                return null;

            var setting = _settingRepository.GetById(settingId);
            return setting;
        }
        
        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Setting value</returns>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T), int storeId = 0)
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (settings.ContainsKey(key))
            {
                var setting = settings[key].FirstOrDefault(x => x.StoreId == storeId);
                if (setting != null)
                    return CommonHelper.To<T>(setting.Value);
            }

            return defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void SetSetting<T>(string key, T value, int storeId = 0, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            key = key.Trim().ToLowerInvariant();
            string valueStr = CommonHelper.GetNopCustomTypeConverter(typeof(T)).ConvertToInvariantString(value);

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key) ? 
                allSettings[key].FirstOrDefault(x => x.StoreId == storeId) : null;
            if (settingForCaching != null)
            {
                //update
                var setting = GetSettingById(settingForCaching.Id);
                setting.Value = valueStr;
                UpdateSetting(setting, clearCache);
            }
            else
            {
                //insert
                var setting = new Setting()
                {
                    Name = key,
                    Value = valueStr,
                    StoreId = storeId
                };
                InsertSetting(setting, clearCache);
            }
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        public virtual IList<Setting> GetAllSettings()
        {
            var query = from s in _settingRepository.Table
                        orderby s.Name, s.StoreId
                        select s;
            var settings = query.ToList();
            return settings;
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="settingInstance">Setting instance</param>
        public virtual void SaveSetting<T>(T settingInstance) where T : ISettings, new()
        {
            //We should be sure that an appropriate ISettings object will not be cached in IoC tool after updating (by default cached per HTTP request)
            EngineContext.Current.Resolve<IConfigurationProvider<T>>().SaveSettings(settingInstance);
        }

        /// <summary>
        /// Delete all settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public virtual void DeleteSetting<T>() where T : ISettings, new()
        {
            EngineContext.Current.Resolve<IConfigurationProvider<T>>().DeleteSettings();
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }

        #endregion
    }
}