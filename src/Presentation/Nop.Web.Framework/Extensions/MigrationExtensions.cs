using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using FluentMigrator;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Helpers;

namespace Nop.Web.Framework.Extensions;

public static partial class MigrationExtensions
{
    #region Fields

    private static readonly Dictionary<string, IList<Setting>> _allSettings = new();

    #endregion

    #region Utilities

    /// <summary>
    /// Gets all settings
    /// </summary>
    /// <returns>Settings</returns>
    private static IDictionary<string, IList<Setting>> GetAllSettingsDictionary()
    {
        if (_allSettings.Any())
            return _allSettings;

        var syncCodeHelper = EngineContext.Current.Resolve<ISyncCodeHelper>();

        var settings = syncCodeHelper
            .GetAllEntities<Setting>(query => query.OrderBy(s => s.Name).ThenBy(s => s.StoreId), _ => default);

        foreach (var s in settings)
        {
            var resourceName = s.Name.ToLowerInvariant();
            var settingForCaching = new Setting { Id = s.Id, Name = s.Name, Value = s.Value, StoreId = s.StoreId };
            if (!_allSettings.TryGetValue(resourceName, out var value))
                //first setting
                _allSettings.Add(resourceName, new List<Setting> { settingForCaching });
            else
                //already added
                //most probably it's the setting with the same name but for some certain store (storeId > 0)
                value.Add(settingForCaching);
        }

        return _allSettings;
    }

    /// <summary>
    /// Get setting key (stored into database)
    /// </summary>
    /// <typeparam name="TSettings">Type of settings</typeparam>
    /// <typeparam name="T">Property type</typeparam>
    /// <param name="keySelector">Key selector</param>
    /// <returns>Key</returns>
    private static string GetSettingKey<TSettings, T>(Expression<Func<TSettings, T>> keySelector)
        where TSettings : ISettings, new()
    {
        if (keySelector.Body is not MemberExpression member)
            throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

        if (member.Member is not PropertyInfo propInfo)
            throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

        var key = $"{typeof(TSettings).Name}.{propInfo.Name}";

        return key;
    }

    /// <summary>
    /// Get setting value
    /// </summary>
    /// <typeparam name="TSettings">Type of settings</typeparam>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="settings">The setting</param>
    /// <param name="keySelector">Key selector</param>
    /// <returns>Value</returns>
    private static TPropType GetValue<TSettings, TPropType>(TSettings settings, Expression<Func<TSettings, TPropType>> keySelector)
    {
        if (keySelector.Body is not MemberExpression member)
            throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

        var propInfo = member.Member as PropertyInfo
            ?? throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

        return (TPropType)propInfo.GetValue(settings, null);
    }

    /// <summary>
    /// Set setting value
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    private static void SetSetting<T>(string key, T value)
    {
        var syncCodeHelper = EngineContext.Current.Resolve<ISyncCodeHelper>();

        setSetting(typeof(T));

        return;

        void setSetting(Type type)
        {
            ArgumentNullException.ThrowIfNull(key);
            key = key.Trim().ToLowerInvariant();
            var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);

            var allSettings = GetAllSettingsDictionary();
            var setting = allSettings.TryGetValue(key, out var settings)
                ? settings.FirstOrDefault(x => x.StoreId == 0)
                : null;

            if (setting != null)
            {
                //update
                setting.Value = valueStr;

                syncCodeHelper.UpdateEntity(setting);
            }
            else
            {
                //insert
                setting = new Setting { Name = key, Value = valueStr, StoreId = 0 };

                syncCodeHelper.InsertEntity(setting);

                if (!_allSettings.ContainsKey(key))
                    _allSettings.Add(key, new List<Setting>());

                _allSettings[key].Add(setting);
            }
        }
    }

    #endregion

    #region Localization

    /// <summary>
    /// Rename locales
    /// </summary>
    /// <param name="_">Migration</param>
    /// <param name="localesToRename">Locales to rename. Key - old name, Value - new name</param>
    public static void RenameLocales(this IMigration _, Dictionary<string, string> localesToRename)
    {
        var syncCodeHelper = EngineContext.Current.Resolve<ISyncCodeHelper>();

        var allLanguages = syncCodeHelper.GetAllLanguages(true);

        var resourceNames = localesToRename.Keys.Union(localesToRename.Values).Select(k => k.ToLowerInvariant()).ToArray();
        var allResources = syncCodeHelper
            .GetAllEntities<LocaleStringResource>(query => query.Where(lsr => resourceNames.Contains(lsr.ResourceName)).OrderBy(lsr => lsr.ResourceName))
            .ToList();

        foreach (var lang in allLanguages)
        {
            foreach (var locale in localesToRename)
            {
                var lsr = allResources.FirstOrDefault(lsr => lsr.LanguageId == lang.Id && string.Equals(locale.Key, lsr.ResourceName, StringComparison.InvariantCultureIgnoreCase));
                if (lsr == null)
                    continue;

                var exist = allResources.FirstOrDefault(lsr => lsr.LanguageId == lang.Id && string.Equals(locale.Value, lsr.ResourceName, StringComparison.InvariantCultureIgnoreCase));
                if (exist != null)
                {
                    syncCodeHelper.DeleteEntity(lsr);
                }
                else
                {
                    lsr.ResourceName = locale.Value.ToLowerInvariant();
                    syncCodeHelper.UpdateEntity(lsr);
                }
            }
        }

        return;
    }

    /// <summary>
    /// Delete locale resources
    /// </summary>
    /// <param name="_">Migration</param>
    /// <param name="resourceNames">Resource names</param>
    /// <param name="languageId">Language identifier; pass null to delete the passed resources from all languages</param>
    public static void DeleteLocaleResources(this IMigration _, IList<string> resourceNames, int? languageId = null)
    {
        var syncCodeHelper = EngineContext.Current.Resolve<ISyncCodeHelper>();
        var staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();

        syncCodeHelper.DeleteEntities<LocaleStringResource>(locale =>
            (!languageId.HasValue || locale.LanguageId == languageId.Value) &&
            resourceNames.Contains(locale.ResourceName, StringComparer.InvariantCultureIgnoreCase));

        //clear cache
        staticCacheManager.RemoveByPrefix(NopEntityCacheDefaults<LocaleStringResource>.Prefix);
    }

    /// <summary>
    /// Add locale resources
    /// </summary>
    /// <param name="_">Migration</param>
    /// <param name="resources">Resource name-value pairs</param>
    public static void AddOrUpdateLocaleResource(this IMigration _, Dictionary<string, string> resources)
    {
        var syncCodeHelper = EngineContext.Current.Resolve<ISyncCodeHelper>();
        var staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();

        var languages = syncCodeHelper.GetAllLanguages(true);
        var languageId = languages
            .FirstOrDefault(lang => lang.UniqueSeoCode == new CultureInfo(NopCommonDefaults.DefaultLanguageCulture).TwoLetterISOLanguageName)?.Id;

        //first update all previous locales with the passed names if they exist
        var resourcesToInsert = updateLocaleResource();

        if (resourcesToInsert.Any())
        {
            //insert new locale resources
            var locales = languages
                .Where(language => !languageId.HasValue || language.Id == languageId.Value)
                .SelectMany(language => resourcesToInsert.Select(resource => new LocaleStringResource
                {
                    LanguageId = language.Id,
                    ResourceName = resource.Key.Trim().ToLowerInvariant(),
                    ResourceValue = resource.Value
                }))
                .ToList();

            syncCodeHelper.InsertEntities(locales);
        }

        //clear cache
        staticCacheManager.RemoveByPrefix(NopEntityCacheDefaults<LocaleStringResource>.Prefix);

        return;

        IDictionary<string, string> updateLocaleResource()
        {
            var localResources = new Dictionary<string, string>(resources, StringComparer.InvariantCultureIgnoreCase);
            var keys = localResources.Keys.Select(key => key.ToLowerInvariant()).ToArray();
            var resourcesToUpdate = syncCodeHelper.GetAllEntities<LocaleStringResource>(query =>
            {
                var rez = query.Where(p => !languageId.HasValue || p.LanguageId == languageId)
                    .Where(p => keys.Contains(p.ResourceName.ToLower()));

                return rez;
            });

            var existsResources = new List<string>();

            foreach (var localeStringResource in resourcesToUpdate.ToList())
            {
                var newValue = localResources[localeStringResource.ResourceName];

                if (localeStringResource.ResourceValue.Equals(newValue))
                    resourcesToUpdate.Remove(localeStringResource);

                localeStringResource.ResourceValue = newValue;
                existsResources.Add(localeStringResource.ResourceName);
            }

            syncCodeHelper.UpdateEntities(resourcesToUpdate);

            return localResources
                .Where(item => !existsResources.Contains(item.Key, StringComparer.InvariantCultureIgnoreCase))
                .ToDictionary(p => p.Key, p => p.Value);
        }
    }

    #endregion

    #region Setting

    /// <summary>
    /// Deletes a setting
    /// </summary>
    /// <param name="_">Migration</param>
    /// <param name="predicate">A function to test each element for a condition</param>
    public static void DeleteSettings(this IMigration _, Expression<Func<Setting, bool>> predicate)
    {
        var syncCodeHelper = EngineContext.Current.Resolve<ISyncCodeHelper>();

        syncCodeHelper.DeleteEntities(predicate);

        _allSettings.Clear();
    }

    /// <summary>
    /// Get setting value by key
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="_">Migration</param>
    /// <param name="key">Key</param>
    /// <param name="defaultValue">Default value</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
    /// <returns>Setting value</returns>
    public static T GetSettingByKey<T>(this IMigration _, string key, T defaultValue = default,
        int storeId = 0, bool loadSharedValueIfNotFound = false)
    {
        if (string.IsNullOrEmpty(key))
            return defaultValue;

        var settings = GetAllSettingsDictionary();
        key = key.Trim().ToLowerInvariant();
        if (!settings.TryGetValue(key, out var value))
            return defaultValue;

        var settingsByKey = value;
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
    /// <param name="migration">Migration</param>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    public static void SetSettingIfNotExists<T>(this IMigration migration, string key, T value)
    {
        if (GetSettingByKey<string>(migration, key) != null)
            return;

        SetSetting(key, value);
    }

    /// <summary>
    /// Set setting value
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="migration">Migration</param>
    /// <param name="keySelector">Key selector</param>
    public static void SetSettingIfNotExists<T, TPropType>(this IMigration migration, Expression<Func<T, TPropType>> keySelector)
        where T : ISettings, new()
    {
        SetSettingIfNotExists(migration, keySelector, setting => GetValue(setting, keySelector));
    }

    /// <summary>
    /// Set setting value
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="migration">Migration</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="value">The value to set</param>
    public static void SetSettingIfNotExists<T, TPropType>(this IMigration migration, Expression<Func<T, TPropType>> keySelector, TPropType value)
        where T : ISettings, new()
    {
        var key = GetSettingKey(keySelector);

        if (GetSettingByKey<string>(migration, key, storeId: 0) != null)
            return;

        SetSetting(key, value);
    }

    /// <summary>
    /// Set setting value
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="migration">Migration</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="action">Action</param>
    public static void SetSettingIfNotExists<T, TPropType>(this IMigration migration, Expression<Func<T, TPropType>> keySelector, Action<T> action)
        where T : ISettings, new()
    {
        if (GetSettingByKey<string>(migration, GetSettingKey(keySelector), storeId: 0) != null)
            return;

        SetSetting(migration, keySelector, action);
    }

    /// <summary>
    /// Set setting value
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="migration">Migration</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="action">Action</param>
    public static void SetSetting<T, TPropType>(this IMigration migration, Expression<Func<T, TPropType>> keySelector, Action<T> action)
        where T : ISettings, new()
    {
        var loadedSetting = loadSetting();

        action(loadedSetting);

        var settingValue = GetValue(loadedSetting, keySelector);
        var settingKey = GetSettingKey(keySelector);

        if (settingValue != null)
            SetSetting(settingKey, settingValue);
        else
            SetSetting(settingKey, string.Empty);

        return;

        T loadSetting()
        {
            var type = typeof(T);
            var settings = Activator.CreateInstance(type);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return (T)settings;

            foreach (var prop in type.GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = type.Name + "." + prop.Name;

                var setting = GetSettingByKey<string>(migration, key);

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

            return (T)settings;
        }
    }

    /// <summary>
    /// Deletes a setting
    /// </summary>
    /// <param name="_">Migration</param>
    /// <param name="settingNames">Array of setting's name</param>
    public static void DeleteSettingsByNames(this IMigration _, IList<string> settingNames)
    {
        var syncCodeHelper = EngineContext.Current.Resolve<ISyncCodeHelper>();
        var names = settingNames.Select(n => n.ToLowerInvariant()).ToArray();

        syncCodeHelper.DeleteEntities<Setting>(setting => names.Contains(setting.Name));

        foreach (var name in names)
            _allSettings.Remove(name);
    }

    #endregion
}