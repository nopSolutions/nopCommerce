using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;
using Nop.Data.Extensions;

namespace Nop.Services.Localization
{
    /// <summary>
    /// Provides information about localizable entities
    /// </summary>
    public partial class LocalizedEntityService : ILocalizedEntityService
    {
        #region Fields

        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly IStaticCacheManager _cacheManager;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public LocalizedEntityService(IRepository<LocalizedProperty> localizedPropertyRepository,
            IStaticCacheManager cacheManager,
            LocalizationSettings localizationSettings)
        {
            _localizedPropertyRepository = localizedPropertyRepository;
            _cacheManager = cacheManager;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets localized properties
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <returns>Localized properties</returns>
        protected virtual IList<LocalizedProperty> GetLocalizedProperties(int entityId, string localeKeyGroup)
        {
            if (entityId == 0 || string.IsNullOrEmpty(localeKeyGroup))
                return new List<LocalizedProperty>();

            var query = from lp in _localizedPropertyRepository.Table
                        orderby lp.Id
                        where lp.EntityId == entityId &&
                              lp.LocaleKeyGroup == localeKeyGroup
                        select lp;
            var props = query.ToList();
            return props;
        }

        /// <summary>
        /// Gets all cached localized properties
        /// </summary>
        /// <returns>Cached localized properties</returns>
        protected virtual IList<LocalizedPropertyForCaching> GetAllLocalizedPropertiesCached()
        {
            //cache
            return _cacheManager.Get(NopLocalizationDefaults.LocalizedPropertyAllCacheKey, () =>
            {
                var query = from lp in _localizedPropertyRepository.TableNoTracking
                            select lp;
                var localizedProperties = query.ToList();
                var list = new List<LocalizedPropertyForCaching>();
                foreach (var lp in localizedProperties)
                {
                    var localizedPropertyForCaching = new LocalizedPropertyForCaching
                    {
                        Id = lp.Id,
                        EntityId = lp.EntityId,
                        LanguageId = lp.LanguageId,
                        LocaleKeyGroup = lp.LocaleKeyGroup,
                        LocaleKey = lp.LocaleKey,
                        LocaleValue = lp.LocaleValue
                    };
                    list.Add(localizedPropertyForCaching);
                }

                return list;
            });
        }

        #endregion

        #region Nested classes

        /// <summary>
        /// LocalizedProperty (for caching)
        /// </summary>
        [Serializable]
        public class LocalizedPropertyForCaching
        {
            public int Id { get; set; }

            public int EntityId { get; set; }

            public int LanguageId { get; set; }

            public string LocaleKeyGroup { get; set; }

            public string LocaleKey { get; set; }

            public string LocaleValue { get; set; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        public virtual void DeleteLocalizedProperty(LocalizedProperty localizedProperty)
        {
            if (localizedProperty == null)
                throw new ArgumentNullException(nameof(localizedProperty));

            _localizedPropertyRepository.Delete(localizedProperty);

            //cache
            _cacheManager.RemoveByPrefix(NopLocalizationDefaults.LocalizedPropertyPrefixCacheKey);
        }

        /// <summary>
        /// Gets a localized property
        /// </summary>
        /// <param name="localizedPropertyId">Localized property identifier</param>
        /// <returns>Localized property</returns>
        public virtual LocalizedProperty GetLocalizedPropertyById(int localizedPropertyId)
        {
            if (localizedPropertyId == 0)
                return null;

            return _localizedPropertyRepository.GetById(localizedPropertyId);
        }

        /// <summary>
        /// Find localized value
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <param name="localeKey">Locale key</param>
        /// <returns>Found localized value</returns>
        public virtual string GetLocalizedValue(int languageId, int entityId, string localeKeyGroup, string localeKey)
        {
            if (_localizationSettings.LoadAllLocalizedPropertiesOnStartup)
            {
                var key = string.Format(NopLocalizationDefaults.LocalizedPropertyCacheKey, languageId, entityId, localeKeyGroup, localeKey);
                return _cacheManager.Get(key, () =>
                {
                    //load all records (we know they are cached)
                    var source = GetAllLocalizedPropertiesCached();
                    var query = from lp in source
                                where lp.LanguageId == languageId &&
                                lp.EntityId == entityId &&
                                lp.LocaleKeyGroup == localeKeyGroup &&
                                lp.LocaleKey == localeKey
                                select lp.LocaleValue;

                    //little hack here. nulls aren't cacheable so set it to ""
                    var localeValue = query.FirstOrDefault() ?? string.Empty;
                    
                    return localeValue;
                });
            }
            else
            {
                //gradual loading
                var key = string.Format(NopLocalizationDefaults.LocalizedPropertyCacheKey, languageId, entityId, localeKeyGroup, localeKey);
                return _cacheManager.Get(key, () =>
                {
                    var source = _localizedPropertyRepository.TableNoTracking;
                    var query = from lp in source
                                where lp.LanguageId == languageId &&
                                lp.EntityId == entityId &&
                                lp.LocaleKeyGroup == localeKeyGroup &&
                                lp.LocaleKey == localeKey
                                select lp.LocaleValue;
                    //little hack here. nulls aren't cacheable so set it to ""
                    var localeValue = query.FirstOrDefault() ?? string.Empty;
                    
                    return localeValue;
                });
            }
        }

        /// <summary>
        /// Inserts a localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        public virtual void InsertLocalizedProperty(LocalizedProperty localizedProperty)
        {
            if (localizedProperty == null)
                throw new ArgumentNullException(nameof(localizedProperty));

            _localizedPropertyRepository.Insert(localizedProperty);

            //cache
            _cacheManager.RemoveByPrefix(NopLocalizationDefaults.LocalizedPropertyPrefixCacheKey);
        }

        /// <summary>
        /// Updates the localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        public virtual void UpdateLocalizedProperty(LocalizedProperty localizedProperty)
        {
            if (localizedProperty == null)
                throw new ArgumentNullException(nameof(localizedProperty));

            _localizedPropertyRepository.Update(localizedProperty);

            //cache
            _cacheManager.RemoveByPrefix(NopLocalizationDefaults.LocalizedPropertyPrefixCacheKey);
        }

        /// <summary>
        /// Save localized value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="localeValue">Locale value</param>
        /// <param name="languageId">Language ID</param>
        public virtual void SaveLocalizedValue<T>(T entity,
            Expression<Func<T, string>> keySelector,
            string localeValue,
            int languageId) where T : BaseEntity, ILocalizedEntity
        {
            SaveLocalizedValue<T, string>(entity, keySelector, localeValue, languageId);
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
        public virtual void SaveLocalizedValue<T, TPropType>(T entity,
            Expression<Func<T, TPropType>> keySelector,
            TPropType localeValue,
            int languageId) where T : BaseEntity, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (languageId == 0)
                throw new ArgumentOutOfRangeException(nameof(languageId), "Language ID should not be 0");

            if (!(keySelector.Body is MemberExpression member))
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

            //load localized value (check whether it's a cacheable entity. In such cases we load its original entity type)
            var localeKeyGroup = entity.GetUnproxiedEntityType().Name;
            var localeKey = propInfo.Name;

            var props = GetLocalizedProperties(entity.Id, localeKeyGroup);
            var prop = props.FirstOrDefault(lp => lp.LanguageId == languageId &&
                lp.LocaleKey.Equals(localeKey, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

            var localeValueStr = CommonHelper.To<string>(localeValue);

            if (prop != null)
            {
                if (string.IsNullOrWhiteSpace(localeValueStr))
                {
                    //delete
                    DeleteLocalizedProperty(prop);
                }
                else
                {
                    //update
                    prop.LocaleValue = localeValueStr;
                    UpdateLocalizedProperty(prop);
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
                InsertLocalizedProperty(prop);
            }
        }

        #endregion
    }
}