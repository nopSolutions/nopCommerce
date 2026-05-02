using System.Linq.Expressions;
using System.Reflection;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Localization;
using Nop.Data;


namespace Nop.Services.Localization;

/// <summary>
/// Provides information about localizable entities
/// </summary>
public partial class LocalizedEntityService : ILocalizedEntityService
{
    #region Fields

    protected readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public LocalizedEntityService(IRepository<LocalizedProperty> localizedPropertyRepository,
        IStaticCacheManager staticCacheManager,
        LocalizationSettings localizationSettings)
    {
        _localizedPropertyRepository = localizedPropertyRepository;
        _staticCacheManager = staticCacheManager;
        _localizationSettings = localizationSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets localized properties
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="localeKeyGroup">Locale key group</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the localized properties
    /// </returns>
    protected virtual async Task<IList<LocalizedProperty>> GetLocalizedPropertiesAsync(int entityId, string localeKeyGroup)
    {
        if (entityId == 0 || string.IsNullOrEmpty(localeKeyGroup))
            return new List<LocalizedProperty>();

        var query = from lp in _localizedPropertyRepository.Table
            orderby lp.Id
            where lp.EntityId == entityId &&
                  lp.LocaleKeyGroup == localeKeyGroup
            select lp;

        var props = await query.ToListAsync();

        return props;
    }

    /// <summary>
    /// Gets all cached localized properties
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cached localized properties
    /// </returns>
    protected virtual async Task<IList<LocalizedProperty>> GetAllLocalizedPropertiesAsync()
    {
        return await _localizedPropertyRepository.GetAllAsync(query =>
        {
            return from lp in query
                select lp;
        }, cache => default);
    }

    /// <summary>
    /// Gets all cached localized properties by language
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the uncached localized properties
    /// </returns>
    protected virtual async Task<IList<LocalizedProperty>> GetAllLocalizedPropertiesAsync(int languageId)
    {
        // do not cache here
        return await _localizedPropertyRepository.GetAllAsync(query => query.Where(lp => lp.LanguageId == languageId));
    }

    /// <summary>
    /// Deletes a localized property
    /// </summary>
    /// <param name="localizedProperty">Localized property</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task DeleteLocalizedPropertyAsync(LocalizedProperty localizedProperty)
    {
        await _localizedPropertyRepository.DeleteAsync(localizedProperty);
    }

    /// <summary>
    /// Inserts a localized property
    /// </summary>
    /// <param name="localizedProperty">Localized property</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InsertLocalizedPropertyAsync(LocalizedProperty localizedProperty)
    {
        await _localizedPropertyRepository.InsertAsync(localizedProperty);
    }

    /// <summary>
    /// Updates the localized property
    /// </summary>
    /// <param name="localizedProperty">Localized property</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task UpdateLocalizedPropertyAsync(LocalizedProperty localizedProperty)
    {
        await _localizedPropertyRepository.UpdateAsync(localizedProperty);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Find localized properties
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="localeKeyGroup">Locale key group</param>
    /// <param name="localeKey">Locale key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the found localized properties
    /// </returns>
    public virtual async Task<IList<LocalizedProperty>> GetEntityLocalizedPropertiesAsync(int entityId, string localeKeyGroup, string localeKey)
    {
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopLocalizationDefaults.LocalizedPropertiesCacheKey,
            entityId, localeKeyGroup, localeKey);

        return await _staticCacheManager.GetAsync(key, async () =>
        {
            var source = _localizationSettings.LoadAllLocalizedPropertiesOnStartup
                //load all records (we know they are cached)
                ? (await GetAllLocalizedPropertiesAsync()).AsQueryable()
                //gradual loading
                : _localizedPropertyRepository.Table;

            var query = from lp in source
                where lp.EntityId == entityId &&
                      lp.LocaleKeyGroup == localeKeyGroup &&
                      lp.LocaleKey == localeKey
                select lp;

            return await query.ToListAsync();
        });
    }

    /// <summary>
    /// Find localized value
    /// </summary>
    /// <param name="languageId">Language identifier</param>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="localeKeyGroup">Locale key group</param>
    /// <param name="localeKey">Locale key</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the found localized value
    /// </returns>
    public virtual async Task<string> GetLocalizedValueAsync(int languageId, int entityId, string localeKeyGroup, string localeKey)
    {
        if (_localizationSettings.LoadAllLocalizedPropertiesOnStartup)
        {
            //value tuples aren't json-serializable by default, so we use a string key
            static string formatKey(string keyGroup, string key, int id) => $"{keyGroup}:{key}:{id}";

            var localizedValues = await _staticCacheManager.GetAsync(
                _staticCacheManager.PrepareKeyForDefaultCache(NopLocalizationDefaults.LocalizedPropertyLookupCacheKey, languageId),
                async () => (await GetAllLocalizedPropertiesAsync(languageId))
                    .GroupBy(p => formatKey(p.LocaleKeyGroup, p.LocaleKey, p.EntityId))
                    .ToDictionary(g => g.Key, g => g.First().LocaleValue));

            return localizedValues.TryGetValue(formatKey(localeKeyGroup, localeKey, entityId), out var localeValue)
                ? localeValue
                : string.Empty;
        }

        //gradual loading
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopLocalizationDefaults.LocalizedPropertyCacheKey
            , languageId, entityId, localeKeyGroup, localeKey);

        return await _staticCacheManager.GetAsync(key, async () =>
        {
            var query = from lp in _localizedPropertyRepository.Table
                where lp.LanguageId == languageId &&
                      lp.EntityId == entityId &&
                      lp.LocaleKeyGroup == localeKeyGroup &&
                      lp.LocaleKey == localeKey
                select lp.LocaleValue;

            //little hack here. nulls aren't cacheable so set it to ""
            return await query.FirstOrDefaultAsync() ?? string.Empty;
        });
    }

    /// <summary>
    /// Save localized value
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="localeValue">Locale value</param>
    /// <param name="languageId">Language ID</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SaveLocalizedValueAsync<T>(T entity,
        Expression<Func<T, string>> keySelector,
        string localeValue,
        int languageId) where T : BaseEntity, ILocalizedEntity
    {
        await SaveLocalizedValueAsync<T, string>(entity, keySelector, localeValue, languageId);
    }

    /// <summary>
    /// Save localized value
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="localeValue">Locale value</param>
    /// <param name="languageId">Language ID</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SaveLocalizedValueAsync<T, TPropType>(T entity,
        Expression<Func<T, TPropType>> keySelector,
        TPropType localeValue,
        int languageId) where T : BaseEntity, ILocalizedEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (languageId == 0)
            throw new ArgumentOutOfRangeException(nameof(languageId), "Language ID should not be 0");

        if (keySelector.Body is not MemberExpression member)
        {
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a method, not a property.",
                keySelector));
        }

        var propInfo = member.Member as PropertyInfo ?? throw new ArgumentException(string.Format(
            "Expression '{0}' refers to a field, not a property.",
            keySelector));

        //load localized value (check whether it's a cacheable entity. In such cases we load its original entity type)
        var localeKeyGroup = entity.GetType().Name;
        var localeKey = propInfo.Name;

        var props = await GetLocalizedPropertiesAsync(entity.Id, localeKeyGroup);
        var prop = props.FirstOrDefault(lp => lp.LanguageId == languageId &&
                                              lp.LocaleKey.Equals(localeKey, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

        var localeValueStr = CommonHelper.To<string>(localeValue);

        if (prop != null)
        {
            if (string.IsNullOrWhiteSpace(localeValueStr))
            {
                //delete
                await DeleteLocalizedPropertyAsync(prop);
            }
            else
            {
                //update
                prop.LocaleValue = localeValueStr;
                await UpdateLocalizedPropertyAsync(prop);
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(localeValueStr))
                return;

            //insert
            prop = new LocalizedProperty
            {
                EntityId = entity.Id,
                LanguageId = languageId,
                LocaleKey = localeKey,
                LocaleKeyGroup = localeKeyGroup,
                LocaleValue = localeValueStr
            };
            await InsertLocalizedPropertyAsync(prop);
        }
    }

    #endregion
}