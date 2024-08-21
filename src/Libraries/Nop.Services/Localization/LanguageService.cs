using System.Globalization;
using Nop.Core.Caching;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Stores;

namespace Nop.Services.Localization;

/// <summary>
/// Language service
/// </summary>
public partial class LanguageService : ILanguageService
{
    #region Fields

    protected readonly IRepository<Language> _languageRepository;
    protected readonly ISettingService _settingService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public LanguageService(IRepository<Language> languageRepository,
        ISettingService settingService,
        IStaticCacheManager staticCacheManager,
        IStoreMappingService storeMappingService,
        LocalizationSettings localizationSettings)
    {
        _languageRepository = languageRepository;
        _settingService = settingService;
        _staticCacheManager = staticCacheManager;
        _storeMappingService = storeMappingService;
        _localizationSettings = localizationSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a language
    /// </summary>
    /// <param name="language">Language</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteLanguageAsync(Language language)
    {
        ArgumentNullException.ThrowIfNull(language);

        //update default admin area language (if required)
        if (_localizationSettings.DefaultAdminLanguageId == language.Id)
            foreach (var activeLanguage in await GetAllLanguagesAsync())
            {
                if (activeLanguage.Id == language.Id)
                    continue;

                _localizationSettings.DefaultAdminLanguageId = activeLanguage.Id;
                await _settingService.SaveSettingAsync(_localizationSettings);
                break;
            }

        await _languageRepository.DeleteAsync(language);
    }

    /// <summary>
    /// Gets all languages
    /// </summary>
    /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the languages
    /// </returns>
    public virtual async Task<IList<Language>> GetAllLanguagesAsync(bool showHidden = false, int storeId = 0)
    {
        //cacheable copy
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopLocalizationDefaults.LanguagesAllCacheKey, storeId, showHidden);

        var languages = await _staticCacheManager.GetAsync(key, async () =>
        {
            var allLanguages = await _languageRepository.GetAllAsync(query =>
            {
                if (!showHidden)
                    query = query.Where(l => l.Published);
                query = query.OrderBy(l => l.DisplayOrder).ThenBy(l => l.Id);

                return query;
            });

            //store mapping
            if (storeId > 0)
                allLanguages = await allLanguages
                    .WhereAwait(async l => await _storeMappingService.AuthorizeAsync(l, storeId))
                    .ToListAsync();

            return allLanguages;
        });

        return languages;
    }

    /// <summary>
    /// Gets all languages
    /// </summary>
    /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// The languages
    /// </returns>
    public virtual IList<Language> GetAllLanguages(bool showHidden = false, int storeId = 0)
    {
        //cacheable copy
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopLocalizationDefaults.LanguagesAllCacheKey, storeId, showHidden);

        var languages = _staticCacheManager.Get(key, () =>
        {
            var allLanguages = _languageRepository.GetAll(query =>
            {
                if (!showHidden)
                    query = query.Where(l => l.Published);
                query = query.OrderBy(l => l.DisplayOrder).ThenBy(l => l.Id);

                return query;
            });

            //store mapping
            if (storeId > 0)
                allLanguages = allLanguages
                    .Where(l => _storeMappingService.Authorize(l, storeId))
                    .ToList();

            return allLanguages;
        });

        return languages;
    }

    /// <summary>
    /// Gets a language
    /// </summary>
    /// <param name="languageId">Language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language
    /// </returns>
    public virtual async Task<Language> GetLanguageByIdAsync(int languageId)
    {
        return await _languageRepository.GetByIdAsync(languageId, cache => default);
    }

    /// <summary>
    /// Inserts a language
    /// </summary>
    /// <param name="language">Language</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertLanguageAsync(Language language)
    {
        await _languageRepository.InsertAsync(language);
    }

    /// <summary>
    /// Updates a language
    /// </summary>
    /// <param name="language">Language</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateLanguageAsync(Language language)
    {
        //update language
        await _languageRepository.UpdateAsync(language);
    }

    /// <summary>
    /// Get 2 letter ISO language code
    /// </summary>
    /// <param name="language">Language</param>
    /// <returns>ISO language code</returns>
    public virtual string GetTwoLetterIsoLanguageName(Language language)
    {
        ArgumentNullException.ThrowIfNull(language);

        if (string.IsNullOrEmpty(language.LanguageCulture))
            return "en";

        var culture = new CultureInfo(language.LanguageCulture);
        var code = culture.TwoLetterISOLanguageName;

        return string.IsNullOrEmpty(code) ? "en" : code;
    }

    #endregion
}