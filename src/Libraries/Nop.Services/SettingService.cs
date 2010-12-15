//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Data;
using System.Globalization;
using Nop.Core;

namespace Nop.Services
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
        
        private readonly IRepository<Setting> _settingRespository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="settingRespository">Setting repository</param>
        public SettingService(ICacheManager cacheManager,
            IRepository<Setting> settingRespository)
        {
            this._cacheManager = cacheManager;
            this._settingRespository = settingRespository;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public void InsertSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            setting.Name = CommonHelper.EnsureNotNull(setting.Name);
            setting.Name = CommonHelper.EnsureMaximumLength(setting.Name, 200);
            setting.Value = CommonHelper.EnsureNotNull(setting.Value);
            setting.Value = CommonHelper.EnsureMaximumLength(setting.Value, 2000);
            setting.Description = CommonHelper.EnsureNotNull(setting.Description);

            _settingRespository.Insert(setting);

            //cache
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public void UpdateSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            setting.Name = CommonHelper.EnsureNotNull(setting.Name);
            setting.Name = CommonHelper.EnsureMaximumLength(setting.Name, 200);
            setting.Value = CommonHelper.EnsureNotNull(setting.Value);
            setting.Value = CommonHelper.EnsureMaximumLength(setting.Value, 2000);
            setting.Description = CommonHelper.EnsureNotNull(setting.Description);

            _settingRespository.Insert(setting);

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
        public Setting GetSettingById(int settingId)
        {
            if (settingId == 0)
                return null;

            var setting = _settingRespository.GetById(settingId);
            return setting;
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Setting value</returns>
        public T GetSettingByKey<T>(string key)
        {
            return GetSettingByKey(key, default(T));
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        public T GetSettingByKey<T>(string key, T defaultValue)
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            key = key.Trim().ToLowerInvariant();

            var settings = GetAllSettings();
            if (settings.ContainsKey(key))
            {
                var setting = settings[key];
                return (T)Convert.ChangeType(setting.Value, typeof(T), CultureInfo.InvariantCulture);
            }
            return defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void SetSetting<T>(string key, T value)
        {
            var settings = GetAllSettings();

            var valueStr = (string) Convert.ChangeType(value, typeof (string), CultureInfo.InvariantCulture);
            Setting setting = null;
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
                                  Description = string.Empty
                              };
                InsertSetting(setting);
            }
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public void DeleteSetting(Setting setting)
        {
            if (setting == null)
                return;

            _settingRespository.Delete(setting);

            //cache
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        public Dictionary<string, Setting> GetAllSettings()
        {
            //cache
            string cacheKey = string.Format(SETTINGS_ALL_KEY);
            object obj2 = _cacheManager.Get(cacheKey);
            if (obj2 != null)
            {
                return (Dictionary<string, Setting>)obj2;
            }

            var query = from s in _settingRespository.Table
                        orderby s.Name
                        select s;
            var settings = query.ToDictionary(s => s.Name.ToLowerInvariant());

            //cache
            _cacheManager.Add(cacheKey, settings);

            return settings;
        }

        #endregion
    }
}