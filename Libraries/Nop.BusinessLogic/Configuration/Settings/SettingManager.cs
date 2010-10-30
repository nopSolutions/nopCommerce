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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings
{
    /// <summary>
    /// Setting manager
    /// </summary>
    public partial class SettingManager : ISettingManager
    {
        #region Constants
        private const string SETTINGS_ALL_KEY = "Nop.setting.all";
        #endregion

        #region Fields

        /// <summary>
        /// object context
        /// </summary>
        protected NopObjectContext _context;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public SettingManager(NopObjectContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a setting
        /// </summary>
        /// <param name="settingId">Setting identifer</param>
        /// <returns>Setting</returns>
        public Setting GetSettingById(int settingId)
        {
            if (settingId == 0)
                return null;

            
            var query = from s in _context.Settings
                        where s.SettingId == settingId
                        select s;
            var setting = query.SingleOrDefault();
            return setting;
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="settingId">Setting identifer</param>
        public void DeleteSetting(int settingId)
        {
            var setting = GetSettingById(settingId);
            if (setting == null)
                return;

            
            if (!_context.IsAttached(setting))
                _context.Settings.Attach(setting);
            _context.DeleteObject(setting);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopStaticCache.RemoveByPattern(SETTINGS_ALL_KEY);
            }
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        public Dictionary<string, Setting> GetAllSettings()
        {
            string key = SETTINGS_ALL_KEY;
            object obj2 = NopStaticCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (Dictionary<string, Setting>)obj2;
            }

            
            var query = from s in _context.Settings
                        orderby s.Name
                        select s;
            var settings = query.ToDictionary(s => s.Name.ToLowerInvariant());

            if (this.CacheEnabled)
            {
                NopStaticCache.Add(key, settings);
            }
            return settings;
        }

         /// <summary>
        /// Inserts/updates a param
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="value">The value</param>
        /// <returns>Setting</returns>
        public Setting SetParam(string name, string value)
        {
            var setting = GetSettingByName(name);
            if (setting != null)
                return SetParam(name, value, setting.Description);
            else
                return SetParam(name, value, string.Empty);
        }

        /// <summary>
        /// Inserts/updates a param
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="value">The value</param>
        /// <param name="description">The description</param>
        /// <returns>Setting</returns>
        public Setting SetParam(string name, string value, string description)
        {
            var setting = GetSettingByName(name);
            if (setting != null)
            {
                if (setting.Name != name || setting.Value != value || setting.Description != description)
                    setting = UpdateSetting(setting.SettingId, name, value, description);
            }
            else
                setting = AddSetting(name, value, description);

            return setting;
        }

        /// <summary>
        /// Inserts/updates a param in US locale
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="value">The value</param>
        /// <returns>Setting</returns>
        public Setting SetParamNative(string name, decimal value)
        {
            return SetParamNative(name, value, string.Empty);
        }

        /// <summary>
        /// Inserts/updates a param in US locale
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="value">The value</param>
        /// <param name="description">The description</param>
        /// <returns>Setting</returns>
        public Setting SetParamNative(string name, decimal value, string description)
        {
            string valueStr = value.ToString(new CultureInfo("en-US"));
            return SetParam(name, valueStr, description);
        }

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="value">The value</param>
        /// <param name="description">The description</param>
        /// <returns>Setting</returns>
        public Setting AddSetting(string name, string value, string description)
        {
            name = CommonHelper.EnsureNotNull(name);
            name = CommonHelper.EnsureMaximumLength(name, 200);
            value = CommonHelper.EnsureNotNull(value);
            value = CommonHelper.EnsureMaximumLength(value, 2000);
            description = CommonHelper.EnsureNotNull(description);

            

            var setting = _context.Settings.CreateObject();
            setting.Name = name;
            setting.Value = value;
            setting.Description = description;

            _context.Settings.AddObject(setting);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopStaticCache.RemoveByPattern(SETTINGS_ALL_KEY);
            }

            return setting;
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <param name="name">The name</param>
        /// <param name="value">The value</param>
        /// <param name="description">The description</param>
        /// <returns>Setting</returns>
        public Setting UpdateSetting(int settingId, string name, string value, string description)
        {
            name = CommonHelper.EnsureNotNull(name);
            name = CommonHelper.EnsureMaximumLength(name, 200);
            value = CommonHelper.EnsureNotNull(value);
            value = CommonHelper.EnsureMaximumLength(value, 2000);
            description = CommonHelper.EnsureNotNull(description);

            var setting = GetSettingById(settingId);
            if (setting == null)
                return null;

            
            if (!_context.IsAttached(setting))
                _context.Settings.Attach(setting);

            setting.Name = name;
            setting.Value = value;
            setting.Description = description;
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopStaticCache.RemoveByPattern(SETTINGS_ALL_KEY);
            }

            return setting;
        }

        /// <summary>
        /// Gets a boolean value of a setting
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <returns>The setting value</returns>
        public bool GetSettingValueBoolean(string name)
        {
            return GetSettingValueBoolean(name, false);
        }

        /// <summary>
        /// Gets a boolean value of a setting
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>The setting value</returns>
        public bool GetSettingValueBoolean(string name, bool defaultValue)
        {
            string value = GetSettingValue(name);
            if (!String.IsNullOrEmpty(value))
            {
                return bool.Parse(value);
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets an integer value of a setting
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <returns>The setting value</returns>
        public int GetSettingValueInteger(string name)
        {
            return GetSettingValueInteger(name, 0);
        }

        /// <summary>
        /// Gets an integer value of a setting
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>The setting value</returns>
        public int GetSettingValueInteger(string name, int defaultValue)
        {
            string value = GetSettingValue(name);
            if (!String.IsNullOrEmpty(value))
            {
                return int.Parse(value);
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets a long value of a setting
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <returns>The setting value</returns>
        public long GetSettingValueLong(string name)
        {
            return GetSettingValueLong(name, 0);
        }

        /// <summary>
        ///  Gets a long value of a setting
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>The setting value</returns>
        public long GetSettingValueLong(string name, int defaultValue)
        {
            string value = GetSettingValue(name);
            if (!String.IsNullOrEmpty(value))
            {
                return long.Parse(value);
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets a decimal value of a setting in US locale
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <returns>The setting value</returns>
        public decimal GetSettingValueDecimalNative(string name)
        {
            return GetSettingValueDecimalNative(name, decimal.Zero);
        }

        /// <summary>
        /// Gets a decimal value of a setting in US locale
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>The setting value</returns>
        public decimal GetSettingValueDecimalNative(string name, decimal defaultValue)
        {
            string value = GetSettingValue(name);
            if (!String.IsNullOrEmpty(value))
            {
                return decimal.Parse(value, new CultureInfo("en-US"));
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets a setting value
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <returns>The setting value</returns>
        public string GetSettingValue(string name)
        {
            var setting = GetSettingByName(name);
            if (setting != null)
                return setting.Value;
            return string.Empty;
        }

        /// <summary>
        /// Gets a setting value
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>The setting value</returns>
        public string GetSettingValue(string name, string defaultValue)
        {
            string value = GetSettingValue(name);
            if (!String.IsNullOrEmpty(value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Get a setting by name
        /// </summary>
        /// <param name="name">The setting name</param>
        /// <returns>Setting instance</returns>
        public Setting GetSettingByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return null;

            name = name.Trim().ToLowerInvariant();

            var settings = GetAllSettings();
            if (settings.ContainsKey(name))
            {
                var setting = settings[name];
                return setting;
            }
            return null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets a store name
        /// </summary>
        public string StoreName
        {
            get
            {
                string storeName = IoCFactory.Resolve<ISettingManager>().GetSettingValue("Common.StoreName");
                return storeName;
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("Common.StoreName", value);
            }
        }

        /// <summary>
        /// Gets or sets a store URL (ending with "/")
        /// </summary>
        public string StoreUrl
        {
            get
            {
                string storeUrl = IoCFactory.Resolve<ISettingManager>().GetSettingValue("Common.StoreURL");
                if (!storeUrl.EndsWith("/"))
                    storeUrl += "/";
                return storeUrl;
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("Common.StoreURL", value);
            }
        }

        /// <summary>
        /// Get current version
        /// </summary>
        /// <returns></returns>
        public string CurrentVersion
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("Common.CurrentVersion");
            }
        }
        #endregion
    }
}
