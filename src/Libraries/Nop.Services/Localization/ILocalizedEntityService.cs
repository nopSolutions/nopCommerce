using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
        Task DeleteLocalizedPropertyAsync(LocalizedProperty localizedProperty);

        //TODO: may be deleted
        /// <summary>
        /// Gets a localized property
        /// </summary>
        /// <param name="localizedPropertyId">Localized property identifier</param>
        /// <returns>Localized property</returns>
        Task<LocalizedProperty> GetLocalizedPropertyByIdAsync(int localizedPropertyId);

        /// <summary>
        /// Find localized value
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <param name="localeKey">Locale key</param>
        /// <returns>Found localized value</returns>
        Task<string> GetLocalizedValueAsync(int languageId, int entityId, string localeKeyGroup, string localeKey);

        //TODO: may be deleted from interface
        /// <summary>
        /// Inserts a localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        Task InsertLocalizedPropertyAsync(LocalizedProperty localizedProperty);

        //TODO: may be deleted from interface
        /// <summary>
        /// Updates the localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        Task UpdateLocalizedPropertyAsync(LocalizedProperty localizedProperty);

        /// <summary>
        /// Save localized value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="localeValue">Locale value</param>
        /// <param name="languageId">Language ID</param>
        Task SaveLocalizedValueAsync<T>(T entity,
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
        Task SaveLocalizedValueAsync<T, TPropType>(T entity,
           Expression<Func<T, TPropType>> keySelector,
           TPropType localeValue,
           int languageId) where T : BaseEntity, ILocalizedEntity;
    }
}
