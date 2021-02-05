using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Localization;

namespace Nop.Services.Localization
{
    /// <summary>
    /// Language service interface
    /// </summary>
    public partial interface ILanguageService
    {
        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="language">Language</param>
        Task DeleteLanguageAsync(Language language);

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Languages</returns>
        Task<IList<Language>> GetAllLanguagesAsync(bool showHidden = false, int storeId = 0);

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Language</returns>
        Task<Language> GetLanguageByIdAsync(int languageId);

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        Task InsertLanguageAsync(Language language);

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        Task UpdateLanguageAsync(Language language);

        //TODO: migrate to an extension method
        /// <summary>
        /// Get 2 letter ISO language code
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>ISO language code</returns>
        string GetTwoLetterIsoLanguageName(Language language);
    }
}