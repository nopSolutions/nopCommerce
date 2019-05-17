using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Stores;

namespace Nop.Services.Localization
{
    /// <summary>
    /// Language service
    /// </summary>
    public partial class LanguageService : ILanguageService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<Language> _languageRepository;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public LanguageService(IEventPublisher eventPublisher,
            IRepository<Language> languageRepository,
            ISettingService settingService,
            IStaticCacheManager cacheManager,
            IStoreMappingService storeMappingService,
            LocalizationSettings localizationSettings)
        {
            _eventPublisher = eventPublisher;
            _languageRepository = languageRepository;
            _settingService = settingService;
            _cacheManager = cacheManager;
            _storeMappingService = storeMappingService;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void DeleteLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (language is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            //update default admin area language (if required)
            if (_localizationSettings.DefaultAdminLanguageId == language.Id)
            {
                foreach (var activeLanguage in GetAllLanguages())
                {
                    if (activeLanguage.Id == language.Id) 
                        continue;

                    _localizationSettings.DefaultAdminLanguageId = activeLanguage.Id;
                    _settingService.SaveSetting(_localizationSettings);
                    break;
                }
            }

            _languageRepository.Delete(language);

            //cache
            _cacheManager.RemoveByPrefix(NopLocalizationDefaults.LanguagesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(language);
        }

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="loadCacheableCopy">A value indicating whether to load a copy that could be cached (workaround until Entity Framework supports 2-level caching)</param>
        /// <returns>Languages</returns>
        public virtual IList<Language> GetAllLanguages(bool showHidden = false, int storeId = 0, bool loadCacheableCopy = true)
        {
            IList<Language> LoadLanguagesFunc()
            {
                var query = _languageRepository.Table;
                if (!showHidden) query = query.Where(l => l.Published);
                query = query.OrderBy(l => l.DisplayOrder).ThenBy(l => l.Id);
                return query.ToList();
            }

            IList<Language> languages;
            if (loadCacheableCopy)
            {
                //cacheable copy
                var key = string.Format(NopLocalizationDefaults.LanguagesAllCacheKey, showHidden);
                languages = _cacheManager.Get(key, () =>
                {
                    var result = new List<Language>();
                    foreach (var language in LoadLanguagesFunc())
                        result.Add(new LanguageForCaching(language));
                    return result;
                });
            }
            else
            {
                languages = LoadLanguagesFunc();
            }

            //store mapping
            if (storeId > 0)
            {
                languages = languages
                    .Where(l => _storeMappingService.Authorize(l, storeId))
                    .ToList();
            }

            return languages;
        }

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="loadCacheableCopy">A value indicating whether to load a copy that could be cached (workaround until Entity Framework supports 2-level caching)</param>
        /// <returns>Language</returns>
        public virtual Language GetLanguageById(int languageId, bool loadCacheableCopy = true)
        {
            if (languageId == 0)
                return null;

            Language LoadLanguageFunc()
            {
                return _languageRepository.GetById(languageId);
            }

            if (!loadCacheableCopy) 
                return LoadLanguageFunc();

            //cacheable copy
            var key = string.Format(NopLocalizationDefaults.LanguagesByIdCacheKey, languageId);
            return _cacheManager.Get(key, () =>
            {
                var language = LoadLanguageFunc();
                return language == null ? null : new LanguageForCaching(language);
            });
        }

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void InsertLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (language is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            _languageRepository.Insert(language);

            //cache
            _cacheManager.RemoveByPrefix(NopLocalizationDefaults.LanguagesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(language);
        }

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void UpdateLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (language is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            //update language
            _languageRepository.Update(language);

            //cache
            _cacheManager.RemoveByPrefix(NopLocalizationDefaults.LanguagesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(language);
        }

        /// <summary>
        /// Get 2 letter ISO language code
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>ISO language code</returns>
        public virtual string GetTwoLetterIsoLanguageName(Language language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (string.IsNullOrEmpty(language.LanguageCulture))
                return "en";

            var culture = new CultureInfo(language.LanguageCulture);
            var code = culture.TwoLetterISOLanguageName;

            return string.IsNullOrEmpty(code) ? "en" : code;
        }

        #endregion
    }
}