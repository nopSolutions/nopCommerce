using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Domain.Configuration;
using Nop.Data;

namespace Nop.Services.Configuration
{
    /// <summary>
    /// Setting manager
    /// </summary>
    public partial class SettingService : ISettingService
    {
        #region Fields

        protected readonly IRepository<Setting> _settingRepository;
        protected readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public SettingService(IRepository<Setting> settingRepository,
            IStaticCacheManager staticCacheManager)
        {
            _settingRepository = settingRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the settings
        /// </returns>
        protected virtual async Task<IDictionary<string, IList<Setting>>> GetAllSettingsDictionaryAsync()
        {
            return await _staticCacheManager.GetAsync(NopSettingsDefaults.SettingsAllAsDictionaryCacheKey, async () =>
            {
                var settings = await GetAllSettingsAsync();

                var dictionary = new Dictionary<string, IList<Setting>>();
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new Setting
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value,
                        StoreId = s.StoreId
                    };
                    if (!dictionary.ContainsKey(resourceName))
                        //first setting
                        dictionary.Add(resourceName, new List<Setting>
                        {
                            settingForCaching
                        });
                    else
                        //already added
                        //most probably it's the setting with the same name but for some certain store (storeId > 0)
                        dictionary[resourceName].Add(settingForCaching);
                }

                return dictionary;
            });
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>
        /// Settings
        /// </returns>
        protected virtual IDictionary<string, IList<Setting>> GetAllSettingsDictionary()
        {
            return _staticCacheManager.Get(NopSettingsDefaults.SettingsAllAsDictionaryCacheKey, () =>
            {
                var settings = GetAllSettings();

                var dictionary = new Dictionary<string, IList<Setting>>();
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new Setting
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value,
                        StoreId = s.StoreId
                    };
                    if (!dictionary.ContainsKey(resourceName))
                        //first setting
                        dictionary.Add(resourceName, new List<Setting>
                        {
                            settingForCaching
                        });
                    else
                        //already added
                        //most probably it's the setting with the same name but for some certain store (storeId > 0)
                        dictionary[resourceName].Add(settingForCaching);
                }

                return dictionary;
            });
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task SetSettingAsync(Type type, string key, object value, int storeId = 0, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            key = key.Trim().ToLowerInvariant();
            var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);

            var allSettings = await GetAllSettingsDictionaryAsync();
            var settingForCaching = allSettings.TryGetValue(key, out var settings) ?
                settings.FirstOrDefault(x => x.StoreId == storeId) : null;
            if (settingForCaching != null)
            {
                //update
                var setting = await GetSettingByIdAsync(settingForCaching.Id);
                setting.Value = valueStr;
                await UpdateSettingAsync(setting, clearCache);
            }
            else
            {
                //insert
                var setting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                    StoreId = storeId
                };
                await InsertSettingAsync(setting, clearCache);
            }
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        protected virtual void SetSetting(Type type, string key, object value, int storeId = 0, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            key = key.Trim().ToLowerInvariant();
            var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);

            var allSettings = GetAllSettingsDictionary();
            var settingForCaching = allSettings.TryGetValue(key, out var settings) ?
                settings.FirstOrDefault(x => x.StoreId == storeId) : null;
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
                var setting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                    StoreId = storeId
                };
                InsertSetting(setting, clearCache);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertSettingAsync(Setting setting, bool clearCache = true)
        {
            await _settingRepository.InsertAsync(setting);

            //cache
            if (clearCache)
                await ClearCacheAsync();
        }

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void InsertSetting(Setting setting, bool clearCache = true)
        {
            _settingRepository.Insert(setting);

            //cache
            if (clearCache)
                ClearCache();
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateSettingAsync(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            await _settingRepository.UpdateAsync(setting);

            //cache
            if (clearCache)
                await ClearCacheAsync();
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void UpdateSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            _settingRepository.Update(setting);

            //cache
            if (clearCache)
                ClearCache();
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteSettingAsync(Setting setting)
        {
            await _settingRepository.DeleteAsync(setting);

            //cache
            await ClearCacheAsync();
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void DeleteSetting(Setting setting)
        {
            _settingRepository.Delete(setting);

            //cache
            ClearCache();
        }

        /// <summary>
        /// Deletes settings
        /// </summary>
        /// <param name="settings">Settings</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteSettingsAsync(IList<Setting> settings)
        {
            await _settingRepository.DeleteAsync(settings);

            //cache
            await ClearCacheAsync();
        }

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting
        /// </returns>
        public virtual async Task<Setting> GetSettingByIdAsync(int settingId)
        {
            return await _settingRepository.GetByIdAsync(settingId, cache => default);
        }

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>
        /// The setting
        /// </returns>
        public virtual Setting GetSettingById(int settingId)
        {
            return _settingRepository.GetById(settingId, cache => default);
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting
        /// </returns>
        public virtual async Task<Setting> GetSettingAsync(string key, int storeId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var settings = await GetAllSettingsDictionaryAsync();
            key = key.Trim().ToLowerInvariant();
            if (!settings.ContainsKey(key))
                return null;

            var settingsByKey = settings[key];
            var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);

            //load shared value?
            if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

            return setting != null ? await GetSettingByIdAsync(setting.Id) : null;
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>
        /// The setting
        /// </returns>
        public virtual Setting GetSetting(string key, int storeId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var settings = GetAllSettingsDictionary();
            key = key.Trim().ToLowerInvariant();
            if (!settings.ContainsKey(key))
                return null;

            var settingsByKey = settings[key];
            var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);

            //load shared value?
            if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

            return setting != null ? GetSettingById(setting.Id) : null;
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting value
        /// </returns>
        public virtual async Task<T> GetSettingByKeyAsync<T>(string key, T defaultValue = default,
            int storeId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var settings = await GetAllSettingsDictionaryAsync();
            key = key.Trim().ToLowerInvariant();
            if (!settings.ContainsKey(key))
                return defaultValue;

            var settingsByKey = settings[key];
            var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);

            //load shared value?
            if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

            return setting != null ? CommonHelper.To<T>(setting.Value) : defaultValue;
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>
        /// Setting value
        /// </returns>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default,
            int storeId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettingsDictionary();
            key = key.Trim().ToLowerInvariant();
            if (!settings.ContainsKey(key))
                return defaultValue;

            var settingsByKey = settings[key];
            var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);

            //load shared value?
            if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

            return setting != null ? CommonHelper.To<T>(setting.Value) : defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetSettingAsync<T>(string key, T value, int storeId = 0, bool clearCache = true)
        {
            await SetSettingAsync(typeof(T), key, value, storeId, clearCache);
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
            SetSetting(typeof(T), key, value, storeId, clearCache);
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the settings
        /// </returns>
        public virtual async Task<IList<Setting>> GetAllSettingsAsync()
        {
            var settings = await _settingRepository.GetAllAsync(query =>
            {
                return from s in query
                       orderby s.Name, s.StoreId
                       select s;
            }, cache => default);

            return settings;
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>
        /// Settings
        /// </returns>
        public virtual IList<Setting> GetAllSettings()
        {
            var settings = _settingRepository.GetAll(query => from s in query
                                                              orderby s.Name, s.StoreId
                                                              select s, cache => default);

            return settings;
        }

        /// <summary>
        /// Determines whether a setting exists
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the true -setting exists; false - does not exist
        /// </returns>
        public virtual async Task<bool> SettingExistsAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int storeId = 0)
            where T : ISettings, new()
        {
            var key = GetSettingKey(settings, keySelector);

            var setting = await GetSettingByKeyAsync<string>(key, storeId: storeId);
            return setting != null;
        }

        /// <summary>
        /// Determines whether a setting exists
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// The true -setting exists; false - does not exist
        /// </returns>
        public virtual bool SettingExists<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int storeId = 0)
            where T : ISettings, new()
        {
            var key = GetSettingKey(settings, keySelector);

            var setting = GetSettingByKey<string>(key, storeId: storeId);
            return setting != null;
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store identifier for which settings should be loaded</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<T> LoadSettingAsync<T>(int storeId = 0) where T : ISettings, new()
        {
            return (T)await LoadSettingAsync(typeof(T), storeId);
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store identifier for which settings should be loaded</param>
        public virtual T LoadSetting<T>(int storeId = 0) where T : ISettings, new()
        {
            return (T)LoadSetting(typeof(T), storeId);
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="storeId">Store identifier for which settings should be loaded</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<ISettings> LoadSettingAsync(Type type, int storeId = 0)
        {
            var settings = Activator.CreateInstance(type);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return settings as ISettings;

            foreach (var prop in type.GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = type.Name + "." + prop.Name;
                //load by store
                var setting = await GetSettingByKeyAsync<string>(key, storeId: storeId, loadSharedValueIfNotFound: true);
                if (setting == null)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                    continue;

                var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings as ISettings;
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="storeId">Store identifier for which settings should be loaded</param>
        /// <returns>Settings</returns>
        public virtual ISettings LoadSetting(Type type, int storeId = 0)
        {
            var settings = Activator.CreateInstance(type);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return settings as ISettings;

            foreach (var prop in type.GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = type.Name + "." + prop.Name;
                //load by store
                var setting = GetSettingByKey<string>(key, storeId: storeId, loadSharedValueIfNotFound: true);
                if (setting == null)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                    continue;

                var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings as ISettings;
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store identifier</param>
        /// <param name="settings">Setting instance</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SaveSettingAsync<T>(T settings, int storeId = 0) where T : ISettings, new()
        {
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                var value = prop.GetValue(settings, null);
                if (value != null)
                    await SetSettingAsync(prop.PropertyType, key, value, storeId, false);
                else
                    await SetSettingAsync(key, string.Empty, storeId, false);
            }

            //and now clear cache
            await ClearCacheAsync();
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store identifier</param>
        /// <param name="settings">Setting instance</param>
        public virtual void SaveSetting<T>(T settings, int storeId = 0) where T : ISettings, new()
        {
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                var value = prop.GetValue(settings, null);
                if (value != null)
                    SetSetting(prop.PropertyType, key, value, storeId, false);
                else
                    SetSetting(key, string.Empty, storeId, false);
            }

            //and now clear cache
            ClearCache();
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SaveSettingAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            int storeId = 0, bool clearCache = true) where T : ISettings, new()
        {
            if (keySelector.Body is not MemberExpression member)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var key = GetSettingKey(settings, keySelector);
            var value = (TPropType)propInfo.GetValue(settings, null);
            if (value != null)
                await SetSettingAsync(key, value, storeId, clearCache);
            else
                await SetSettingAsync(key, string.Empty, storeId, clearCache);
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void SaveSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            int storeId = 0, bool clearCache = true) where T : ISettings, new()
        {
            if (keySelector.Body is not MemberExpression member)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var key = GetSettingKey(settings, keySelector);
            var value = (TPropType)propInfo.GetValue(settings, null);
            if (value != null)
                SetSetting(key, value, storeId, clearCache);
            else
                SetSetting(key, string.Empty, storeId, clearCache);
        }

        /// <summary>
        /// Save settings object (per store). If the setting is not overridden per store then it'll be delete
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="overrideForStore">A value indicating whether to setting is overridden in some store</param>
        /// <param name="storeId">Store ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SaveSettingOverridablePerStoreAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            bool overrideForStore, int storeId = 0, bool clearCache = true) where T : ISettings, new()
        {
            if (overrideForStore || storeId == 0)
                await SaveSettingAsync(settings, keySelector, storeId, clearCache);
            else if (storeId > 0)
                await DeleteSettingAsync(settings, keySelector, storeId);
        }

        /// <summary>
        /// Delete all settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteSettingAsync<T>() where T : ISettings, new()
        {
            var settingsToDelete = new List<Setting>();
            var allSettings = await GetAllSettingsAsync();
            foreach (var prop in typeof(T).GetProperties())
            {
                var key = typeof(T).Name + "." + prop.Name;
                settingsToDelete.AddRange(allSettings.Where(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
            }

            await DeleteSettingsAsync(settingsToDelete);
        }

        /// <summary>
        /// Delete settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store ID</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteSettingAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int storeId = 0) where T : ISettings, new()
        {
            var key = GetSettingKey(settings, keySelector);
            key = key.Trim().ToLowerInvariant();

            var allSettings = await GetAllSettingsDictionaryAsync();
            var settingForCaching = allSettings.TryGetValue(key, out var settings_) ?
                settings_.FirstOrDefault(x => x.StoreId == storeId) : null;
            if (settingForCaching == null)
                return;

            //update
            var setting = await GetSettingByIdAsync(settingForCaching.Id);
            await DeleteSettingAsync(setting);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ClearCacheAsync()
        {
            await _staticCacheManager.RemoveByPrefixAsync(NopEntityCacheDefaults<Setting>.Prefix);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            _staticCacheManager.RemoveByPrefix(NopEntityCacheDefaults<Setting>.Prefix);
        }

        /// <summary>
        /// Get setting key (stored into database)
        /// </summary>
        /// <typeparam name="TSettings">Type of settings</typeparam>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <returns>Key</returns>
        public virtual string GetSettingKey<TSettings, T>(TSettings settings, Expression<Func<TSettings, T>> keySelector)
            where TSettings : ISettings, new()
        {
            if (keySelector.Body is not MemberExpression member)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            if (member.Member is not PropertyInfo propInfo)
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var key = $"{typeof(TSettings).Name}.{propInfo.Name}";

            return key;
        }
        #endregion
    }
}
