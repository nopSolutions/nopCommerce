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

        private readonly IWorkContext _workContext;
        private readonly IRepository<Setting> _settingRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="workContext">Work context</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="settingRepository">Setting repository</param>
        public SettingService(IWorkContext workContext, 
            ICacheManager cacheManager,
            IRepository<Setting> settingRepository)
        {
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._settingRepository = settingRepository;
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

            _settingRepository.Insert(setting);

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

            _settingRepository.Insert(setting);

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
        public T GetSettingByKey<T>(string key, T defaultValue = default(T))
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
        public void SetSetting<T>(string key, T value)
        {
            var settings = GetAllSettings();

            Setting setting = null;
            if (settings.ContainsKey(key))
            {
                //update
                setting = settings[key];
                setting.Value = value.ToString();
                UpdateSetting(setting);
            }
            else
            {
                //insert
                setting = new Setting()
                              {
                                  Name = key,
                                  Value = value.ToString(),
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

            _settingRepository.Delete(setting);

            //cache
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        public IDictionary<string, Setting> GetAllSettings()
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

        #endregion
    }
}