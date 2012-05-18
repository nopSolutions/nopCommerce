using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;
using Nop.Core.Events;
using Nop.Services.Logging;

namespace Nop.Services.Localization
{
    /// <summary>
    /// Provides information about localization
    /// </summary>
    public partial class LocalizationService : ILocalizationService
    {
        #region Constants
        private const string LOCALSTRINGRESOURCES_ALL_KEY = "Nop.lsr.all-{0}";
        private const string LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY = "Nop.lsr.{0}-{1}";
        private const string LOCALSTRINGRESOURCES_PATTERN_KEY = "Nop.lsr.";
        #endregion

        #region Fields

        private readonly IRepository<LocaleStringResource> _lsrRepository;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly ICacheManager _cacheManager;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="logger">Logger</param>
        /// <param name="workContext">Work context</param>
        /// <param name="lsrRepository">Locale string resource repository</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="eventPublisher">Event published</param>
        public LocalizationService(ICacheManager cacheManager,
            ILogger logger, IWorkContext workContext,
            IRepository<LocaleStringResource> lsrRepository, LocalizationSettings localizationSettings, IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _logger = logger;
            _workContext = workContext;
            _lsrRepository = lsrRepository;
            _localizationSettings = localizationSettings;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public virtual void DeleteLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            _lsrRepository.Delete(localeStringResource);

            //cache
            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(localeStringResource);
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource identifier</param>
        /// <returns>Locale string resource</returns>
        public virtual LocaleStringResource GetLocaleStringResourceById(int localeStringResourceId)
        {
            if (localeStringResourceId == 0)
                return null;

            var localeStringResource = _lsrRepository.GetById(localeStringResourceId);
            return localeStringResource;
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <returns>Locale string resource</returns>
        public virtual LocaleStringResource GetLocaleStringResourceByName(string resourceName)
        {
            if (_workContext.WorkingLanguage != null)
                return GetLocaleStringResourceByName(resourceName, _workContext.WorkingLanguage.Id);

            return null;
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <returns>Locale string resource</returns>
        public virtual LocaleStringResource GetLocaleStringResourceByName(string resourceName, int languageId,
            bool logIfNotFound = true)
        {
            var query = from lsr in _lsrRepository.Table
                        orderby lsr.ResourceName
                        where lsr.LanguageId == languageId && lsr.ResourceName == resourceName
                        select lsr;
            var localeStringResource = query.FirstOrDefault();

            if (localeStringResource == null && logIfNotFound)
                _logger.Warning(string.Format("Resource string ({0}) not found. Language ID = {1}", resourceName, languageId));
            return localeStringResource;
        }

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Locale string resources</returns>
        public virtual IList<LocaleStringResource> GetAllResources(int languageId)
        {
            var query = from l in _lsrRepository.Table
                        orderby l.ResourceName
                        where l.LanguageId == languageId
                        select l;
            var locales = query.ToList();
            return locales;
        }

        /// <summary>
        /// Inserts a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public virtual void InsertLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");
            
            _lsrRepository.Insert(localeStringResource);

            //cache
            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(localeStringResource);
        }

        /// <summary>
        /// Updates the locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public virtual void UpdateLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            _lsrRepository.Update(localeStringResource);

            //cache
            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(localeStringResource);
        }

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Locale string resources</returns>
        public virtual Dictionary<string, KeyValuePair<int,string>> GetAllResourceValues(int languageId)
        {
            string key = string.Format(LOCALSTRINGRESOURCES_ALL_KEY, languageId);
            return _cacheManager.Get(key, () =>
            {
                var query = from l in _lsrRepository.Table
                            orderby l.ResourceName
                            where l.LanguageId == languageId
                            select l;
                var locales = query.ToList();
                //format: <name, <id, value>>
                var dictionary = new Dictionary<string, KeyValuePair<int, string>>();
                foreach (var locale in locales)
                {
                    var resourceName = locale.ResourceName.ToLowerInvariant();
                    if (!dictionary.ContainsKey(resourceName))
                        dictionary.Add(resourceName, new KeyValuePair<int, string>(locale.Id, locale.ResourceValue));
                }
                return dictionary;
            });
        }

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <returns>A string representing the requested resource string.</returns>
        public virtual string GetResource(string resourceKey)
        {
            if (_workContext.WorkingLanguage != null)
                return GetResource(resourceKey, _workContext.WorkingLanguage.Id);
            
            return "";
        }
        
        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="returnEmptyIfNotFound">A value indicating whether to empty string will be returned if a resource is not found and default value is set to empty string</param>
        /// <returns>A string representing the requested resource string.</returns>
        public virtual string GetResource(string resourceKey, int languageId,
            bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false)
        {
            string result = string.Empty;
            if (resourceKey == null)
                resourceKey = string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();
            if (_localizationSettings.LoadAllLocaleRecordsOnStartup)
            {
                //load all records (we know they are cached)
                var resources = GetAllResourceValues(languageId);
                if (resources.ContainsKey(resourceKey))
                {
                    result = resources[resourceKey].Value;
                }
            }
            else
            {
                //gradual loading
                string key = string.Format(LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY, languageId, resourceKey);
                string lsr = _cacheManager.Get(key, () =>
                {
                    var query = from l in _lsrRepository.Table
                                where l.ResourceName == resourceKey
                                && l.LanguageId == languageId
                                select l.ResourceValue;
                    return query.FirstOrDefault();
                });

                if (lsr != null) 
                    result = lsr;
            }
            if (String.IsNullOrEmpty(result))
            {
                if (logIfNotFound)
                    _logger.Warning(string.Format("Resource string ({0}) is not found. Language ID = {1}", resourceKey, languageId));
                
                if (!String.IsNullOrEmpty(defaultValue))
                {
                    result = defaultValue;
                }
                else
                {
                    if (!returnEmptyIfNotFound)
                        result = resourceKey;
                }
            }
            return result;
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
        }

        #endregion
    }
}
