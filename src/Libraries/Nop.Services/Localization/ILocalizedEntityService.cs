using System;
using System.Linq.Expressions;
using Nop.Core;
using Nop.Core.Domain.Localization;

namespace Nop.Services.Localization
{
    /// <summary>
    /// Localized entity service interface
    /// </summary>
    public partial interface ILocalizedEntityService
    {
        /// <summary>
        /// Deletes a localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        void DeleteLocalizedProperty(LocalizedProperty localizedProperty);

        /// <summary>
        /// Gets a localized property
        /// </summary>
        /// <param name="localizedPropertyId">Localized property identifier</param>
        /// <returns>Localized property</returns>
        LocalizedProperty GetLocalizedPropertyById(int localizedPropertyId);

        /// <summary>
        /// Find localized value
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <param name="localeKey">Locale key</param>
        /// <returns>Found localized value</returns>
        string GetLocalizedValue(int languageId, int entityId, string localeKeyGroup, string localeKey);

        /// <summary>
        /// Inserts a localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        void InsertLocalizedProperty(LocalizedProperty localizedProperty);

        /// <summary>
        /// Updates the localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        void UpdateLocalizedProperty(LocalizedProperty localizedProperty);

        /// <summary>
        /// Save localized value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="localeValue">Locale value</param>
        /// <param name="languageId">Language ID</param>
        void SaveLocalizedValue<T>(T entity,
            Expression<Func<T, string>> keySelector,
            string localeValue,
            int languageId) where T : BaseEntity, ILocalizedEntity;

        /// <summary>
        /// Save localized value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="localeValue">Locale value</param>
        /// <param name="languageId">Language ID</param>
        void SaveLocalizedValue<T, TPropType>(T entity,
           Expression<Func<T, TPropType>> keySelector,
           TPropType localeValue,
           int languageId) where T : BaseEntity, ILocalizedEntity;
    }
}
