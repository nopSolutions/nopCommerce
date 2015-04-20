using System;
using System.Linq.Expressions;
using System.Reflection;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Configuration;

namespace Nop.Services.Localization
{
    public static class LocalizationExtensions
    {
        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <returns>Localized property</returns>
        public static string GetLocalized<T>(this T entity,
            Expression<Func<T, string>> keySelector)
            where T : BaseEntity, ILocalizedEntity
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            return GetLocalized(entity, keySelector, workContext.WorkingLanguage.Id);
        }
        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Localized property</returns>
        public static string GetLocalized<T>(this T entity, 
            Expression<Func<T, string>> keySelector, int languageId, 
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true) 
            where T : BaseEntity, ILocalizedEntity
        {
            return GetLocalized<T, string>(entity, keySelector, languageId, returnDefaultValue, ensureTwoPublishedLanguages);
        }
        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Localized property</returns>
        public static TPropType GetLocalized<T, TPropType>(this T entity,
            Expression<Func<T, TPropType>> keySelector, int languageId, 
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where T : BaseEntity, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            TPropType result = default(TPropType);
            string resultStr = string.Empty;

            //load localized value
            string localeKeyGroup = typeof(T).Name;
            string localeKey = propInfo.Name;

            if (languageId > 0)
            {
                //ensure that we have at least two published languages
                bool loadLocalizedValue = true;
                if (ensureTwoPublishedLanguages)
                {
                    var lService = EngineContext.Current.Resolve<ILanguageService>();
                    var totalPublishedLanguages = lService.GetAllLanguages().Count;
                    loadLocalizedValue = totalPublishedLanguages >= 2;
                }

                //localized value
                if (loadLocalizedValue)
                {
                    var leService = EngineContext.Current.Resolve<ILocalizedEntityService>();
                    resultStr = leService.GetLocalizedValue(languageId, entity.Id, localeKeyGroup, localeKey);
                    if (!String.IsNullOrEmpty(resultStr))
                        result = CommonHelper.To<TPropType>(resultStr);
                }
            }

            //set default value if required
            if (String.IsNullOrEmpty(resultStr) && returnDefaultValue)
            {
                var localizer = keySelector.Compile();
                result = localizer(entity);
            }
            
            return result;
        }



        /// <summary>
        /// Get localized property of setting
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Localized property</returns>
        public static string GetLocalizedSetting<T>(this T settings,
            Expression<Func<T, string>> keySelector, int languageId,
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where T : ISettings, new()
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            string key = settings.GetSettingKey(keySelector);

            //we do not support localized settings per store (overridden store settings)
            var setting = settingService.GetSetting(key, storeId: 0, loadSharedValueIfNotFound: false);
            if (setting == null)
                return null;

            return setting.GetLocalized(x => x.Value, languageId, returnDefaultValue, ensureTwoPublishedLanguages);
        }
        /// <summary>
        /// Save localized property of setting
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="value">Localizaed value</param>
        /// <returns>Localized property</returns>
        public static void SaveLocalizedSetting<T>(this T settings,
            Expression<Func<T, string>> keySelector, int languageId,
            string value)
            where T : ISettings, new()
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            var localizedEntityService = EngineContext.Current.Resolve<ILocalizedEntityService>();

            string key = settings.GetSettingKey(keySelector);

            //we do not support localized settings per store (overridden store settings)
            var setting = settingService.GetSetting(key, storeId: 0, loadSharedValueIfNotFound: false);
            if (setting == null)
                return;

            localizedEntityService.SaveLocalizedValue(setting, x => x.Value, value, languageId);
        }


        /// <summary>
        /// Get localized value of enum
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="workContext">Work context</param>
        /// <returns>Localized value</returns>
        public static string GetLocalizedEnum<T>(this T enumValue, ILocalizationService localizationService, IWorkContext workContext)
            where T : struct
        {
            if (workContext == null)
                throw new ArgumentNullException("workContext");

            return GetLocalizedEnum(enumValue, localizationService, workContext.WorkingLanguage.Id);
        }
        /// <summary>
        /// Get localized value of enum
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized value</returns>
        public static string GetLocalizedEnum<T>(this T enumValue, ILocalizationService localizationService, int languageId)
            where T : struct
        {
            if (localizationService == null)
                throw new ArgumentNullException("localizationService");

            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

            //localized value
            string resourceName = string.Format("Enums.{0}.{1}", 
                typeof(T).ToString(), 
                //Convert.ToInt32(enumValue)
                enumValue.ToString());
            string result = localizationService.GetResource(resourceName, languageId, false, "", true);

            //set default value if required
            if (String.IsNullOrEmpty(result))
                result = CommonHelper.ConvertEnum(enumValue.ToString());

            return result;
        }


        /// <summary>
        /// Get localized value of permission
        /// We don't have UI to manage permission localizable name. That's why we're using this extension method
        /// </summary>
        /// <param name="permissionRecord">Permission record</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="workContext">Work context</param>
        /// <returns>Localized value</returns>
        public static string GetLocalizedPermissionName(this PermissionRecord permissionRecord,
            ILocalizationService localizationService, IWorkContext workContext)
        {
            if (workContext == null)
                throw new ArgumentNullException("workContext");

            return GetLocalizedPermissionName(permissionRecord, localizationService, workContext.WorkingLanguage.Id);
        }
        /// <summary>
        /// Get localized value of enum
        /// We don't have UI to manage permission localizable name. That's why we're using this extension method
        /// </summary>
        /// <param name="permissionRecord">Permission record</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized value</returns>
        public static string GetLocalizedPermissionName(this PermissionRecord permissionRecord, 
            ILocalizationService localizationService, int languageId)
        {
            if (permissionRecord == null)
                throw new ArgumentNullException("permissionRecord");

            if (localizationService == null)
                throw new ArgumentNullException("localizationService");

            //localized value
            string resourceName = string.Format("Permission.{0}", permissionRecord.SystemName);
            string result = localizationService.GetResource(resourceName, languageId, false, "", true);

            //set default value if required
            if (String.IsNullOrEmpty(result))
                result = permissionRecord.Name;

            return result;
        }
        /// <summary>
        /// Save localized name of a permission
        /// </summary>
        /// <param name="permissionRecord">Permission record</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageService">Language service</param>
        public static void SaveLocalizedPermissionName(this PermissionRecord permissionRecord,
            ILocalizationService localizationService, ILanguageService languageService)
        {
            if (permissionRecord == null)
                throw new ArgumentNullException("permissionRecord");
            if (localizationService == null)
                throw new ArgumentNullException("localizationService");
            if (languageService == null)
                throw new ArgumentNullException("languageService");

            string resourceName = string.Format("Permission.{0}", permissionRecord.SystemName);
            string resourceValue = permissionRecord.Name;

            foreach (var lang in languageService.GetAllLanguages(true))
            {
                var lsr = localizationService.GetLocaleStringResourceByName(resourceName, lang.Id, false);
                if (lsr == null)
                {
                    lsr = new LocaleStringResource
                    {
                        LanguageId = lang.Id,
                        ResourceName = resourceName,
                        ResourceValue = resourceValue
                    };
                    localizationService.InsertLocaleStringResource(lsr);
                }
                else
                {
                    lsr.ResourceValue = resourceValue;
                    localizationService.UpdateLocaleStringResource(lsr);
                }
            }
        }
        /// <summary>
        /// Delete a localized name of a permission
        /// </summary>
        /// <param name="permissionRecord">Permission record</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageService">Language service</param>
        public static void DeleteLocalizedPermissionName(this PermissionRecord permissionRecord,
            ILocalizationService localizationService, ILanguageService languageService)
        {
            if (permissionRecord == null)
                throw new ArgumentNullException("permissionRecord");
            if (localizationService == null)
                throw new ArgumentNullException("localizationService");
            if (languageService == null)
                throw new ArgumentNullException("languageService");

            string resourceName = string.Format("Permission.{0}", permissionRecord.SystemName);
            foreach (var lang in languageService.GetAllLanguages(true))
            {
                var lsr = localizationService.GetLocaleStringResourceByName(resourceName, lang.Id, false);
                if (lsr != null)
                    localizationService.DeleteLocaleStringResource(lsr);
            }
        }



        /// <summary>
        /// Delete a locale resource
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <param name="resourceName">Resource name</param>
        public static void DeletePluginLocaleResource(this BasePlugin plugin,
            string resourceName)
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var languageService = EngineContext.Current.Resolve<ILanguageService>();
            DeletePluginLocaleResource(plugin, localizationService,
                languageService, resourceName);
        }
        /// <summary>
        /// Delete a locale resource
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="resourceName">Resource name</param>
        public static void DeletePluginLocaleResource(this BasePlugin plugin,
            ILocalizationService localizationService, ILanguageService languageService,
            string resourceName)
        {
            //actually plugin instance is not required
            if (plugin == null)
                throw new ArgumentNullException("plugin");
            if (localizationService == null)
                throw new ArgumentNullException("localizationService");
            if (languageService == null)
                throw new ArgumentNullException("languageService");

            foreach (var lang in languageService.GetAllLanguages(true))
            {
                var lsr = localizationService.GetLocaleStringResourceByName(resourceName, lang.Id, false);
                if (lsr != null)
                    localizationService.DeleteLocaleStringResource(lsr);
            }
        }
        /// <summary>
        /// Add a locale resource (if new) or update an existing one
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <param name="resourceName">Resource name</param>
        /// <param name="resourceValue">Resource value</param>
        /// <param name="languageCulture">Language culture code. If null or empty, then a resource will be added for all languages</param>
        public static void AddOrUpdatePluginLocaleResource(this BasePlugin plugin,
            string resourceName, string resourceValue, string languageCulture = null)
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var languageService = EngineContext.Current.Resolve<ILanguageService>();
             AddOrUpdatePluginLocaleResource(plugin, localizationService,
                 languageService, resourceName, resourceValue, languageCulture);
        }
        /// <summary>
        /// Add a locale resource (if new) or update an existing one
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="resourceName">Resource name</param>
        /// <param name="resourceValue">Resource value</param>
        /// <param name="languageCulture">Language culture code. If null or empty, then a resource will be added for all languages</param>
        public static void AddOrUpdatePluginLocaleResource(this BasePlugin plugin, 
            ILocalizationService localizationService, ILanguageService languageService, 
            string resourceName, string resourceValue, string languageCulture = null)
        {
            //actually plugin instance is not required
            if (plugin == null)
                throw new ArgumentNullException("plugin");
            if (localizationService == null)
                throw new ArgumentNullException("localizationService");
            if (languageService == null)
                throw new ArgumentNullException("languageService");
            
            foreach (var lang in languageService.GetAllLanguages(true))
            {
                if (!String.IsNullOrEmpty(languageCulture) && !languageCulture.Equals(lang.LanguageCulture))
                    continue;

                var lsr = localizationService.GetLocaleStringResourceByName(resourceName, lang.Id, false);
                if (lsr == null)
                {
                    lsr = new LocaleStringResource
                    {
                        LanguageId = lang.Id,
                        ResourceName = resourceName,
                        ResourceValue = resourceValue
                    };
                    localizationService.InsertLocaleStringResource(lsr);
                }
                else
                {
                    lsr.ResourceValue = resourceValue;
                    localizationService.UpdateLocaleStringResource(lsr);
                }
            }
        }



        /// <summary>
        /// Get localized friendly name of a plugin
        /// </summary>
        /// <typeparam name="T">Plugin</typeparam>
        /// <param name="plugin">Plugin</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <returns>Localized value</returns>
        public static string GetLocalizedFriendlyName<T>(this T plugin, ILocalizationService localizationService, 
            int languageId, bool returnDefaultValue = true)
            where T : IPlugin
        {   
            if (localizationService == null)
                throw new ArgumentNullException("localizationService");

            if (plugin == null)
                throw new ArgumentNullException("plugin");

            if (plugin.PluginDescriptor == null)
                throw new ArgumentException("Plugin descriptor cannot be loaded");

            string systemName = plugin.PluginDescriptor.SystemName;
            //localized value
            string resourceName = string.Format("Plugins.FriendlyName.{0}",
                systemName);
            string result = localizationService.GetResource(resourceName, languageId, false, "", true);

            //set default value if required
            if (String.IsNullOrEmpty(result) && returnDefaultValue)
                result = plugin.PluginDescriptor.FriendlyName;

            return result;
        }
        /// <summary>
        /// Save localized friendly name of a plugin
        /// </summary>
        /// <typeparam name="T">Plugin</typeparam>
        /// <param name="plugin">Plugin</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="localizedFriendlyName">Localized friendly name</param>
        public static void SaveLocalizedFriendlyName<T>(this T plugin, 
            ILocalizationService localizationService, int languageId,
            string localizedFriendlyName)
            where T : IPlugin
        {
            if (localizationService == null)
                throw new ArgumentNullException("localizationService");

            if (languageId == 0)
                throw new ArgumentOutOfRangeException("languageId", "Language ID should not be 0");

            if (plugin == null)
                throw new ArgumentNullException("plugin");

            if (plugin.PluginDescriptor == null)
                throw new ArgumentException("Plugin descriptor cannot be loaded");

            string systemName = plugin.PluginDescriptor.SystemName;
            //localized value
            string resourceName = string.Format("Plugins.FriendlyName.{0}", systemName);
            var resource = localizationService.GetLocaleStringResourceByName(resourceName, languageId, false);

            if (resource != null)
            {
                if (string.IsNullOrWhiteSpace(localizedFriendlyName))
                {
                    //delete
                    localizationService.DeleteLocaleStringResource(resource);
                }
                else
                {
                    //update
                    resource.ResourceValue = localizedFriendlyName;
                    localizationService.UpdateLocaleStringResource(resource);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(localizedFriendlyName))
                {
                    //insert
                    resource = new LocaleStringResource
                    {
                        LanguageId = languageId,
                        ResourceName = resourceName,
                        ResourceValue = localizedFriendlyName,
                    };
                    localizationService.InsertLocaleStringResource(resource);
                }
            }
        }
    }
}
