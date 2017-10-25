using System.Collections.Generic;
using Nop.Core.Domain.Localization;

namespace Nop.Services.Localization
{
    /// <summary>
    /// 语言服务接口
    /// Language service interface
    /// </summary>
    public partial interface ILanguageService
    {
        /// <summary>
        /// 删除语言
        /// Deletes a language
        /// </summary>
        /// <param name="language">Language</param>
        void DeleteLanguage(Language language);

        /// <summary>
        /// 获得所有语言
        /// Gets all languages
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Languages</returns>
        IList<Language> GetAllLanguages(bool showHidden = false, int storeId = 0);

        /// <summary>
        /// 根据语言编号获得一个语言实体
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Language</returns>
        Language GetLanguageById(int languageId);

        /// <summary>
        /// 新增一种语言
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        void InsertLanguage(Language language);

        /// <summary>
        /// 更新一个语言
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        void UpdateLanguage(Language language);
    }
}
