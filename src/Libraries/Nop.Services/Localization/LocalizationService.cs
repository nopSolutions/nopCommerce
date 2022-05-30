using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.ExportImport;
using Nop.Services.Logging;
using Nop.Services.Plugins;

namespace Nop.Services.Localization
{
    /// <summary>
    /// Provides information about localization
    /// </summary>
    public partial class LocalizationService : ILocalizationService
    {
        #region Fields

        protected readonly ILanguageService _languageService;
        protected readonly ILocalizedEntityService _localizedEntityService;
        protected readonly ILogger _logger;
        protected readonly IRepository<LocaleStringResource> _lsrRepository;
        protected readonly ISettingService _settingService;
        protected readonly IStaticCacheManager _staticCacheManager;
        protected readonly IWorkContext _workContext;
        protected readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public LocalizationService(ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            ILogger logger,
            IRepository<LocaleStringResource> lsrRepository,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IWorkContext workContext,
            LocalizationSettings localizationSettings)
        {
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _logger = logger;
            _lsrRepository = lsrRepository;
            _settingService = settingService;
            _staticCacheManager = staticCacheManager;
            _workContext = workContext;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities
        
        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the locale string resources
        /// </returns>
        protected virtual async Task<IList<LocaleStringResource>> GetAllResourcesAsync(int languageId)
        {
            var locales = await _lsrRepository.GetAllAsync(query =>
            {
                return from l in query
                    orderby l.ResourceName
                    where l.LanguageId == languageId
                    select l;
            });

            return locales;
        }
        
        protected virtual HashSet<(string name, string value)> LoadLocaleResourcesFromStream(StreamReader xmlStreamReader, string language)
        {
            var result = new HashSet<(string name, string value)>();

            using (var xmlReader = XmlReader.Create(xmlStreamReader))
                while (xmlReader.ReadToFollowing("Language"))
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    using var languageReader = xmlReader.ReadSubtree();
                    while (languageReader.ReadToFollowing("LocaleResource"))
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.GetAttribute("Name") is string name)
                        {
                            using var lrReader = languageReader.ReadSubtree();
                            if (lrReader.ReadToFollowing("Value") && lrReader.NodeType == XmlNodeType.Element)
                                result.Add((name.ToLowerInvariant(), lrReader.ReadString()));
                        }

                    break;
                }

            return result;
        }

        protected virtual Dictionary<string, KeyValuePair<int, string>> ResourceValuesToDictionary(IEnumerable<LocaleStringResource> locales)
        {
            //format: <name, <id, value>>
            var dictionary = new Dictionary<string, KeyValuePair<int, string>>();
            foreach (var locale in locales)
            {
                var resourceName = locale.ResourceName.ToLowerInvariant();
                if (!dictionary.ContainsKey(resourceName))
                    dictionary.Add(resourceName, new KeyValuePair<int, string>(locale.Id, locale.ResourceValue));
            }

            return dictionary;
        }

        protected virtual async Task<IDictionary<string, string>> UpdateLocaleResourceAsync(IDictionary<string, string> resources, int? languageId = null, bool clearCache = true)
        {
            var localResources = new Dictionary<string, string>(resources, StringComparer.InvariantCultureIgnoreCase);
            var keys = localResources.Keys.Select(key => key.ToLowerInvariant()).ToArray();
            var resourcesToUpdate = await _lsrRepository.GetAllAsync(query =>
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

            await _lsrRepository.UpdateAsync(resourcesToUpdate);

            //clear cache
            if (clearCache)
                await _staticCacheManager.RemoveByPrefixAsync(NopEntityCacheDefaults<LocaleStringResource>.Prefix);

            return localResources.Where(item => !existsResources.Contains(item.Key, StringComparer.InvariantCultureIgnoreCase))
                .ToDictionary(p => p.Key, p => p.Value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteLocaleStringResourceAsync(LocaleStringResource localeStringResource)
        {
            await _lsrRepository.DeleteAsync(localeStringResource);
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the locale string resource
        /// </returns>
        public virtual async Task<LocaleStringResource> GetLocaleStringResourceByIdAsync(int localeStringResourceId)
        {
            return await _lsrRepository.GetByIdAsync(localeStringResourceId, cache => default);
        }
        
        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the locale string resource
        /// </returns>
        public virtual async Task<LocaleStringResource> GetLocaleStringResourceByNameAsync(string resourceName, int languageId,
            bool logIfNotFound = true)
        {
            var query = from lsr in _lsrRepository.Table
                        orderby lsr.ResourceName
                        where lsr.LanguageId == languageId && lsr.ResourceName == resourceName.ToLowerInvariant()
                        select lsr;

            var localeStringResource = await query.FirstOrDefaultAsync();

            if (localeStringResource == null && logIfNotFound)
                await _logger.WarningAsync($"Resource string ({resourceName}) not found. Language ID = {languageId}");

            return localeStringResource;
        }

        /// <summary>
        /// Inserts a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertLocaleStringResourceAsync(LocaleStringResource localeStringResource)
        {
            if (!string.IsNullOrEmpty(localeStringResource?.ResourceName))
                localeStringResource.ResourceName = localeStringResource.ResourceName.Trim().ToLowerInvariant();

            await _lsrRepository.InsertAsync(localeStringResource);
        }

        /// <summary>
        /// Updates the locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateLocaleStringResourceAsync(LocaleStringResource localeStringResource)
        {
            await _lsrRepository.UpdateAsync(localeStringResource);
        }

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="loadPublicLocales">A value indicating whether to load data for the public store only (if "false", then for admin area only. If null, then load all locales. We use it for performance optimization of the site startup</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the locale string resources
        /// </returns>
        public virtual async Task<Dictionary<string, KeyValuePair<int, string>>> GetAllResourceValuesAsync(int languageId, bool? loadPublicLocales)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopLocalizationDefaults.LocaleStringResourcesAllCacheKey, languageId);

            //get all locale string resources by language identifier
            var allLocales =
                await _staticCacheManager.GetAsync(key, () => (Dictionary<string, KeyValuePair<int, string>>)null);

            if (!loadPublicLocales.HasValue || allLocales != null)
            {
                var rez = allLocales ?? await _staticCacheManager.GetAsync(key, () =>
                {
                    //we use no tracking here for performance optimization
                    //anyway records are loaded only for read-only operations
                    var query = from l in _lsrRepository.Table
                                orderby l.ResourceName
                                where l.LanguageId == languageId
                                select l;

                    return ResourceValuesToDictionary(query);
                });

                //remove separated resource 
                await _staticCacheManager.RemoveAsync(NopLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey, languageId);
                await _staticCacheManager.RemoveAsync(NopLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey, languageId);

                return rez;
            }

            //performance optimization of the site startup
            key = _staticCacheManager.PrepareKeyForDefaultCache(loadPublicLocales.Value
                ? NopLocalizationDefaults.LocaleStringResourcesAllPublicCacheKey
                : NopLocalizationDefaults.LocaleStringResourcesAllAdminCacheKey,
                languageId);

            return await _staticCacheManager.GetAsync(key, () =>
            {
                //we use no tracking here for performance optimization
                //anyway records are loaded only for read-only operations
                var query = from l in _lsrRepository.Table
                            orderby l.ResourceName
                            where l.LanguageId == languageId
                            select l;
                query = loadPublicLocales.Value ? query.Where(r => !r.ResourceName.StartsWith(NopLocalizationDefaults.AdminLocaleStringResourcesPrefix)) : query.Where(r => r.ResourceName.StartsWith(NopLocalizationDefaults.AdminLocaleStringResourcesPrefix));
                
                return ResourceValuesToDictionary(query);
            });
        }

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a string representing the requested resource string.
        /// </returns>
        public virtual async Task<string> GetResourceAsync(string resourceKey)
        {
            var workingLanguage = await _workContext.GetWorkingLanguageAsync();

            if (workingLanguage != null)
                return await GetResourceAsync(resourceKey, workingLanguage.Id);

            return string.Empty;
        }

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="returnEmptyIfNotFound">A value indicating whether an empty string will be returned if a resource is not found and default value is set to empty string</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a string representing the requested resource string.
        /// </returns>
        public virtual async Task<string> GetResourceAsync(string resourceKey, int languageId,
            bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false)
        {
            var result = string.Empty;
            if (resourceKey == null)
                resourceKey = string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();
            if (_localizationSettings.LoadAllLocaleRecordsOnStartup)
            {
                //load all records (we know they are cached)
                var resources = await GetAllResourceValuesAsync(languageId, !resourceKey.StartsWith(NopLocalizationDefaults.AdminLocaleStringResourcesPrefix, StringComparison.InvariantCultureIgnoreCase));
                if (resources.ContainsKey(resourceKey))
                {
                    result = resources[resourceKey].Value;
                }
            }
            else
            {
                //gradual loading
                var key = _staticCacheManager.PrepareKeyForDefaultCache(NopLocalizationDefaults.LocaleStringResourcesByNameCacheKey
                    , languageId, resourceKey);

                var query = from l in _lsrRepository.Table
                    where l.ResourceName == resourceKey
                          && l.LanguageId == languageId
                    select l.ResourceValue;

                var lsr = await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());

                if (lsr != null)
                    result = lsr;
            }

            if (!string.IsNullOrEmpty(result))
                return result;

            if (logIfNotFound)
                await _logger.WarningAsync($"Resource string ({resourceKey}) is not found. Language ID = {languageId}");

            if (!string.IsNullOrEmpty(defaultValue))
            {
                result = defaultValue;
            }
            else
            {
                if (!returnEmptyIfNotFound)
                    result = resourceKey;
            }

            return result;
        }

        /// <summary>
        /// Export language resources to XML
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in XML format
        /// </returns>
        public virtual async Task<string> ExportResourcesToXmlAsync(Language language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            var settings = new XmlWriterSettings
            {
                Async = true,
                Encoding = Encoding.UTF8,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stream = new MemoryStream();
            await using var xmlWriter = XmlWriter.Create(stream, settings);
            
            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync("Language");
            await xmlWriter.WriteAttributeStringAsync("Name", language.Name);
            await xmlWriter.WriteAttributeStringAsync("SupportedVersion", NopVersion.CURRENT_VERSION);

            var resources = await GetAllResourcesAsync(language.Id);
            foreach (var resource in resources)
            {
                await xmlWriter.WriteStartElementAsync("LocaleResource");
                await xmlWriter.WriteAttributeStringAsync("Name", resource.ResourceName);
                await xmlWriter.WriteElementStringAsync("Value", null, resource.ResourceValue);
                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Import language resources from XML file
        /// </summary>
        /// <param name="language">Language</param>
        /// <param name="xmlStreamReader">Stream reader of XML file</param>
        /// <param name="updateExistingResources">A value indicating whether to update existing resources</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ImportResourcesFromXmlAsync(Language language, StreamReader xmlStreamReader, bool updateExistingResources = true)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (xmlStreamReader.EndOfStream)
                return;
            
            var lsNamesList = new Dictionary<string, LocaleStringResource>();

            foreach (var localeStringResource in _lsrRepository.Table.Where(lsr => lsr.LanguageId == language.Id)
                .OrderBy(lsr => lsr.Id))
                lsNamesList[localeStringResource.ResourceName.ToLowerInvariant()] = localeStringResource;

            var lrsToUpdateList = new List<LocaleStringResource>();
            var lrsToInsertList = new Dictionary<string, LocaleStringResource>();

            foreach (var (name, value) in LoadLocaleResourcesFromStream(xmlStreamReader, language.Name))
            {
                if (lsNamesList.ContainsKey(name))
                {
                    if (!updateExistingResources) 
                        continue;

                    var lsr = lsNamesList[name];
                    lsr.ResourceValue = value;
                    lrsToUpdateList.Add(lsr);
                }
                else
                {
                    var lsr = new LocaleStringResource { LanguageId = language.Id, ResourceName = name, ResourceValue = value };
                    lrsToInsertList[name] = lsr;
                }
            }

            await _lsrRepository.UpdateAsync(lrsToUpdateList, false);
            await _lsrRepository.InsertAsync(lrsToInsertList.Values.ToList(), false);

            //clear cache
            await _staticCacheManager.RemoveByPrefixAsync(NopEntityCacheDefaults<LocaleStringResource>.Prefix);
        }

        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier; pass null to use the current working language; pass 0 to get standard language value</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized property
        /// </returns>
        public virtual async Task<TPropType> GetLocalizedAsync<TEntity, TPropType>(TEntity entity, Expression<Func<TEntity, TPropType>> keySelector,
            int? languageId = null, bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where TEntity : BaseEntity, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (keySelector.Body is not MemberExpression member)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            if (member.Member is not PropertyInfo propInfo)
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var result = default(TPropType);
            var resultStr = string.Empty;

            var localeKeyGroup = entity.GetType().Name;
            var localeKey = propInfo.Name;

            var workingLanguage = await _workContext.GetWorkingLanguageAsync();

            if (!languageId.HasValue)
                languageId = workingLanguage.Id;

            if (languageId > 0)
            {
                //ensure that we have at least two published languages
                var loadLocalizedValue = true;
                if (ensureTwoPublishedLanguages)
                {
                    var totalPublishedLanguages = (await _languageService.GetAllLanguagesAsync()).Count;
                    loadLocalizedValue = totalPublishedLanguages >= 2;
                }

                //localized value
                if (loadLocalizedValue)
                {
                    resultStr = await _localizedEntityService
                        .GetLocalizedValueAsync(languageId.Value, entity.Id, localeKeyGroup, localeKey);
                    if (!string.IsNullOrEmpty(resultStr))
                        result = CommonHelper.To<TPropType>(resultStr);
                }
            }

            //set default value if required
            if (!string.IsNullOrEmpty(resultStr) || !returnDefaultValue)
                return result;
            var localizer = keySelector.Compile();
            result = localizer(entity);

            return result;
        }

        /// <summary>
        /// Get localized property of setting
        /// </summary>
        /// <typeparam name="TSettings">Settings type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized property
        /// </returns>
        public virtual async Task<string> GetLocalizedSettingAsync<TSettings>(TSettings settings, Expression<Func<TSettings, string>> keySelector,
            int languageId, int storeId, bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where TSettings : ISettings, new()
        {
            var key = _settingService.GetSettingKey(settings, keySelector);

            //we do not support localized settings per store (overridden store settings)
            var setting = await _settingService.GetSettingAsync(key, storeId: storeId, loadSharedValueIfNotFound: true);
            if (setting == null)
                return null;

            return await GetLocalizedAsync(setting, x => x.Value, languageId, returnDefaultValue, ensureTwoPublishedLanguages);
        }

        /// <summary>
        /// Save localized property of setting
        /// </summary>
        /// <typeparam name="TSettings">Settings type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="value">Localized value</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized property
        /// </returns>
        public virtual async Task SaveLocalizedSettingAsync<TSettings>(TSettings settings, Expression<Func<TSettings, string>> keySelector,
            int languageId, string value) where TSettings : ISettings, new()
        {
            var key = _settingService.GetSettingKey(settings, keySelector);

            //we do not support localized settings per store (overridden store settings)
            var setting = await _settingService.GetSettingAsync(key);
            if (setting == null)
                return;

            await _localizedEntityService.SaveLocalizedValueAsync(setting, x => x.Value, value, languageId);
        }

        /// <summary>
        /// Get localized value of enum
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="languageId">Language identifier; pass null to use the current working language</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized value
        /// </returns>
        public virtual async Task<string> GetLocalizedEnumAsync<TEnum>(TEnum enumValue, int? languageId = null) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            //localized value
            var workingLanguage = await _workContext.GetWorkingLanguageAsync();
            var resourceName = $"{NopLocalizationDefaults.EnumLocaleStringResourcesPrefix}{typeof(TEnum)}.{enumValue}";
            var result = await GetResourceAsync(resourceName, languageId ?? workingLanguage.Id, false, string.Empty, true);

            //set default value if required
            if (string.IsNullOrEmpty(result))
                result = CommonHelper.ConvertEnum(enumValue.ToString());

            return result;
        }

        /// <summary>
        /// Get localized value of enum
        /// We don't have UI to manage permission localizable name. That's why we're using this method
        /// </summary>
        /// <param name="permissionRecord">Permission record</param>
        /// <param name="languageId">Language identifier; pass null to use the current working language</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized value
        /// </returns>
        public virtual async Task<string> GetLocalizedPermissionNameAsync(PermissionRecord permissionRecord, int? languageId = null)
        {
            if (permissionRecord == null)
                throw new ArgumentNullException(nameof(permissionRecord));

            //localized value
            var workingLanguage = await _workContext.GetWorkingLanguageAsync();
            var resourceName = $"{NopLocalizationDefaults.PermissionLocaleStringResourcesPrefix}{permissionRecord.SystemName}";
            var result = await GetResourceAsync(resourceName, languageId ?? workingLanguage.Id, false, string.Empty, true);

            //set default value if required
            if (string.IsNullOrEmpty(result))
                result = permissionRecord.Name;

            return result;
        }

        /// <summary>
        /// Save localized name of a permission
        /// </summary>
        /// <param name="permissionRecord">Permission record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SaveLocalizedPermissionNameAsync(PermissionRecord permissionRecord)
        {
            if (permissionRecord == null)
                throw new ArgumentNullException(nameof(permissionRecord));

            var resourceName = $"{NopLocalizationDefaults.PermissionLocaleStringResourcesPrefix}{permissionRecord.SystemName}";
            var resourceValue = permissionRecord.Name;

            foreach (var lang in await _languageService.GetAllLanguagesAsync(true))
            {
                var lsr = await GetLocaleStringResourceByNameAsync(resourceName, lang.Id, false);
                if (lsr == null)
                {
                    lsr = new LocaleStringResource
                    {
                        LanguageId = lang.Id,
                        ResourceName = resourceName,
                        ResourceValue = resourceValue
                    };
                    await InsertLocaleStringResourceAsync(lsr);
                }
                else
                {
                    lsr.ResourceValue = resourceValue;
                    await UpdateLocaleStringResourceAsync(lsr);
                }
            }
        }

        /// <summary>
        /// Delete a localized name of a permission
        /// </summary>
        /// <param name="permissionRecord">Permission record</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteLocalizedPermissionNameAsync(PermissionRecord permissionRecord)
        {
            if (permissionRecord == null)
                throw new ArgumentNullException(nameof(permissionRecord));

            var resourceName = $"{NopLocalizationDefaults.PermissionLocaleStringResourcesPrefix}{permissionRecord.SystemName}";
            foreach (var lang in await _languageService.GetAllLanguagesAsync(true))
            {
                var lsr = await GetLocaleStringResourceByNameAsync(resourceName, lang.Id, false);
                if (lsr != null)
                    await DeleteLocaleStringResourceAsync(lsr);
            }
        }

        /// <summary>
        /// Add a locale resource (if new) or update an existing one
        /// </summary>
        /// <param name="resourceName">Resource name</param>
        /// <param name="resourceValue">Resource value</param>
        /// <param name="languageCulture">Language culture code. If null or empty, then a resource will be added for all languages</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddOrUpdateLocaleResourceAsync(string resourceName, string resourceValue, string languageCulture = null)
        {
            foreach (var lang in await _languageService.GetAllLanguagesAsync(true))
            {
                if (!string.IsNullOrEmpty(languageCulture) && !languageCulture.Equals(lang.LanguageCulture))
                    continue;

                var lsr = await GetLocaleStringResourceByNameAsync(resourceName, lang.Id, false);
                if (lsr == null)
                {
                    lsr = new LocaleStringResource
                    {
                        LanguageId = lang.Id,
                        ResourceName = resourceName,
                        ResourceValue = resourceValue
                    };
                    await InsertLocaleStringResourceAsync(lsr);
                }
                else
                {
                    lsr.ResourceValue = resourceValue;
                    await UpdateLocaleStringResourceAsync(lsr);
                }
            }
        }

        /// <summary>
        /// Add locale resources
        /// </summary>
        /// <param name="resources">Resource name-value pairs</param>
        /// <param name="languageId">Language identifier; pass null to add the passed resources for all languages</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddOrUpdateLocaleResourceAsync(IDictionary<string, string> resources, int? languageId = null)
        {
            //first update all previous locales with the passed names if they exist
            var resourcesToInsert = await UpdateLocaleResourceAsync(resources, languageId, false);

            if (resourcesToInsert.Any())
            {
                //insert new locale resources
                var locales = (await _languageService.GetAllLanguagesAsync(true))
                    .Where(language => !languageId.HasValue || language.Id == languageId.Value)
                    .SelectMany(language => resourcesToInsert.Select(resource => new LocaleStringResource
                    {
                        LanguageId = language.Id, ResourceName = resource.Key.Trim().ToLowerInvariant(), ResourceValue = resource.Value
                    }))
                    .ToList();

                await _lsrRepository.InsertAsync(locales, false);
            }

            //clear cache
            await _staticCacheManager.RemoveByPrefixAsync(NopEntityCacheDefaults<LocaleStringResource>.Prefix);
        }

        /// <summary>
        /// Delete a locale resource
        /// </summary>
        /// <param name="resourceName">Resource name</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteLocaleResourceAsync(string resourceName)
        {
            foreach (var lang in await _languageService.GetAllLanguagesAsync(true))
            {
                var lsr = await GetLocaleStringResourceByNameAsync(resourceName, lang.Id, false);
                if (lsr != null)
                    await DeleteLocaleStringResourceAsync(lsr);
            }
        }

        /// <summary>
        /// Delete locale resources
        /// </summary>
        /// <param name="resourceNames">Resource names</param>
        /// <param name="languageId">Language identifier; pass null to delete the passed resources from all languages</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteLocaleResourcesAsync(IList<string> resourceNames, int? languageId = null)
        {
            await _lsrRepository.DeleteAsync(locale => (!languageId.HasValue || locale.LanguageId == languageId.Value) &&
                resourceNames.Contains(locale.ResourceName, StringComparer.InvariantCultureIgnoreCase));

            //clear cache
            await _staticCacheManager.RemoveByPrefixAsync(NopEntityCacheDefaults<LocaleStringResource>.Prefix);
        }

        /// <summary>
        /// Delete locale resources by the passed name prefix
        /// </summary>
        /// <param name="resourceNamePrefix">Resource name prefix</param>
        /// <param name="languageId">Language identifier; pass null to delete resources by prefix from all languages</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteLocaleResourcesAsync(string resourceNamePrefix, int? languageId = null)
        {
            await _lsrRepository.DeleteAsync(locale => (!languageId.HasValue || locale.LanguageId == languageId.Value) &&
                !string.IsNullOrEmpty(locale.ResourceName) &&
                locale.ResourceName.StartsWith(resourceNamePrefix, StringComparison.InvariantCultureIgnoreCase));

            //clear cache
            await _staticCacheManager.RemoveByPrefixAsync(NopEntityCacheDefaults<LocaleStringResource>.Prefix);
        }

        /// <summary>
        /// Get localized friendly name of a plugin
        /// </summary>
        /// <typeparam name="TPlugin">Plugin type</typeparam>
        /// <param name="plugin">Plugin</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized value
        /// </returns>
        public virtual async Task<string> GetLocalizedFriendlyNameAsync<TPlugin>(TPlugin plugin, int languageId, bool returnDefaultValue = true)
            where TPlugin : IPlugin
        {
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));

            if (plugin.PluginDescriptor == null)
                throw new ArgumentException("Plugin descriptor cannot be loaded");

            var systemName = plugin.PluginDescriptor.SystemName;
            //localized value
            var resourceName = $"{NopLocalizationDefaults.PluginNameLocaleStringResourcesPrefix}{systemName}";
            var result = await GetResourceAsync(resourceName, languageId, false, string.Empty, true);

            //set default value if required
            if (string.IsNullOrEmpty(result) && returnDefaultValue)
                result = plugin.PluginDescriptor.FriendlyName;

            return result;
        }

        /// <summary>
        /// Save localized friendly name of a plugin
        /// </summary>
        /// <typeparam name="TPlugin">Plugin</typeparam>
        /// <param name="plugin">Plugin</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="localizedFriendlyName">Localized friendly name</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SaveLocalizedFriendlyNameAsync<TPlugin>(TPlugin plugin, int languageId, string localizedFriendlyName)
            where TPlugin : IPlugin
        {
            if (languageId == 0)
                throw new ArgumentOutOfRangeException(nameof(languageId), "Language ID should not be 0");

            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));

            if (plugin.PluginDescriptor == null)
                throw new ArgumentException("Plugin descriptor cannot be loaded");

            var systemName = plugin.PluginDescriptor.SystemName;
            //localized value
            var resourceName = $"{NopLocalizationDefaults.PluginNameLocaleStringResourcesPrefix}{systemName}";
            var resource = await GetLocaleStringResourceByNameAsync(resourceName, languageId, false);

            if (resource != null)
            {
                if (string.IsNullOrWhiteSpace(localizedFriendlyName))
                {
                    //delete
                    await DeleteLocaleStringResourceAsync(resource);
                }
                else
                {
                    //update
                    resource.ResourceValue = localizedFriendlyName;
                    await UpdateLocaleStringResourceAsync(resource);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(localizedFriendlyName))
                    return;

                //insert
                resource = new LocaleStringResource
                {
                    LanguageId = languageId,
                    ResourceName = resourceName,
                    ResourceValue = localizedFriendlyName
                };
                await InsertLocaleStringResourceAsync(resource);
            }
        }

        #endregion
    }
}