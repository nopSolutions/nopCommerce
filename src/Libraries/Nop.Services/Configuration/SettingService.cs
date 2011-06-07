

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Domain.Configuration;
using Nop.Core.Infrastructure;


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
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="settingRepository">Setting repository</param>
        public SettingService(ICacheManager cacheManager,
            IRepository<Setting> settingRepository)
        {
            this._cacheManager = cacheManager;
            this._settingRepository = settingRepository;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void InsertSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Insert(setting);

            //cache
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void UpdateSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Update(setting);

            //cache
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifer</param>
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
        /// <returns>Setting value</returns>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T))
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            key = key.Trim().ToLowerInvariant();

            var settings = GetAllSettings();
            if (settings.ContainsKey(key)) {
                var setting = settings[key];
                return setting.As<T>();
            }
            return defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public virtual void SetSetting<T>(string key, T value)
        {
            var settings = GetAllSettings();

            key = key.Trim().ToLowerInvariant();

            Setting setting = null;
            string valueStr = TypeDescriptor.GetConverter(typeof(T)).ConvertToInvariantString(value);
            if (settings.ContainsKey(key))
            {
                //update
                setting = settings[key];
                setting.Value = valueStr;
                UpdateSetting(setting);
            }
            else
            {
                //insert
                setting = new Setting()
                              {
                                  Name = key,
                                  Value = valueStr,
                              };
                InsertSetting(setting);
            }
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
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        public virtual IDictionary<string, Setting> GetAllSettings()
        {
            //cache
            string key = string.Format(SETTINGS_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var query = from s in _settingRepository.Table
                            orderby s.Name
                            select s;
                var settings = query.ToDictionary(s => s.Name.ToLowerInvariant());

                return settings;
            });
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

        #endregion
    }
}